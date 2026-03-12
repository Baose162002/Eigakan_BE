using Eigakan.Application.Interface.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Eigakan.Application.Shared.Response;
using Eigakan.Application.Interface;
using Eigakan.Domain.Request.ContractRequest;
using Eigakan.Domain.Models;
using Xceed.Words.NET;
using Eigakan.Domain.Response.ContractResponse;
using AutoMapper;
using Eigakan.Application.Helper;
using System.Globalization;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using Eigakan.Domain.Enum;
using System.Security.Cryptography;
using Amazon.S3.Model;
using System.Text.RegularExpressions;
using Eigakan.Domain.Response.Movie;
using Eigakan.Domain.Response.Media;
using Spire.Doc;
namespace Eigakan.Application.Service
{
    public class ContractService : IContractService
    {
        private readonly string _templateUrl;
        private readonly ILogger<ContractService> _logger;
        private readonly HttpClient _httpClient;
        private readonly IContractRepository _contractRepository;
        private readonly string _templateFilePath;
        private readonly IUserRepository _userRepository;
        private readonly IMoviesRepository _moviesRepository;
		private readonly IConfiguration _configuration;
		private readonly IMapper _mapper;
		private readonly AmazonS3Client _s3Client;
		private readonly string _bucketName;
		private readonly string _s3Key = "contract-template/Contract.docx";
		private readonly Lazy<IAmazonS3> _s3cc;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IEmailService _emailService;
		private readonly ICacheService _cacheService;

		public ContractService( IContractRepository contractRepository, IUserRepository userRepository,
                                IMoviesRepository moviesRepository, IConfiguration configuration,
                                IMapper mapper, ILogger<ContractService> logger,
                                HttpClient httpClient, Lazy<IAmazonS3> s3cc,
                                IHttpContextAccessor httpContextAccessor, IEmailService emailService,
								ICacheService cacheService)  
        {
            _mapper = mapper;
            _contractRepository = contractRepository;
            _userRepository = userRepository;
            _moviesRepository = moviesRepository;
			_configuration = configuration;
			_logger = logger;
            _httpClient = httpClient;
            _s3cc = s3cc;
			_httpContextAccessor = httpContextAccessor;
			_emailService = emailService;
			_cacheService = cacheService;
			_bucketName = configuration["AWS:BucketName"];
			var accessKey = configuration["AWS:AccessKey"];
			var secretKey = configuration["AWS:SecretKey"];
			var region = RegionEndpoint.GetBySystemName(configuration["AWS:Region"]);
			_s3Client = new AmazonS3Client(accessKey, secretKey, region);
		}

		
        // Create PDF
        public async Task<Result<string>> GenerateContractAsync(ContractGenerationRequest request)
        {
			    await using var transaction = await _contractRepository.BeginTransactionAsync();
				try
                {
                var movieExists = await _moviesRepository.GetMovieById(request.MovieId);
                if (movieExists == null)
                {
                    return new Result<string>
                    {
                        Success = false,
                        Message = "Invalid MovieId: Movie does not exist."
                    };
                }

				if (string.IsNullOrEmpty(movieExists.FileUrl))
					return new Result<string> { Success = false, Message = "File copy-right not found, please upload again!" };



				DateTime? startDate = request.GetStartDate();
                if (startDate == null)
                {
                    return new Result<string> { Success = false, Message = "Invalid StartDate format. Use dd/MM/yyyy." };
                }
                if (request.Duration <= 0)
                {
                    return new Result<string> { Success = false, Message = "Duration must be greater than 0." };
                }
                if (string.IsNullOrWhiteSpace(request.PublisherName))
                {
                    return new Result<string> { Success = false, Message = "PublisherName cannot be empty." };
                }
                if (string.IsNullOrWhiteSpace(request.DistributorName))
                {
                    return new Result<string> { Success = false, Message = "DistributorName cannot be empty." };
                }
                if (request.Price == null || request.Price.Value <= 0)
                {
                    return new Result<string> { Success = false, Message = "Price must be greater than 0." };
                }

                DateTime endDate = startDate.Value.AddDays(request.Duration);

                byte[] docxBytes = await GenerateContractFromWordTemplate(request);
                byte[] pdfBytes = await ConvertDocxToPdf(docxBytes);

                // Upload to AWS S3
                var uploadResult = await UploadToS3(pdfBytes, request);
                
                if (!uploadResult.Success)
                {
                    return uploadResult;
                }
				
                string token = CreateRandomToken();

				var contract = new Contract
                {
                    Id = Guid.NewGuid().ToString(),
                    ContractDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    StartDate = startDate.Value,
                    EndDate = endDate,
                    Duration = request.Duration,
                    PublisherName = request.PublisherName,
                    DistributorName = request.DistributorName,
                    Price = request.Price.Value,
                    FileUrl = uploadResult.Data, 
                    CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    Status = ContractStatusEnum.WAITING_FOR_REVIEWING.ToString(),   
                    SignToken = token,
                    MovieId = request.MovieId,
                    UserId = movieExists.UserId
				};

                await _contractRepository.InsertTransaction(contract);

				
                var updateUrlMovie = await MoveFileToMovieFolderAsync(movieExists.FileUrl, request.MovieId);
				movieExists.FileUrl= updateUrlMovie;

				await _moviesRepository.UpdateTransaction(movieExists);

				//thua ko biết savechange 1 lần duy nhất 
				await _moviesRepository.SaveChangeTransaction();
				await _contractRepository.SaveChangeTransaction();
				
				await transaction.CommitAsync();

				string frontendreseturl = _configuration["FrontendSettings:ResetPasswordUrl"];
				string resetPasswordUrl = $"{frontendreseturl}{token}";

				// Tạo email với liên kết thực sự
				var mailrequest = new EmailSetting.MailResponse
				{
					ToEmail = "minhtuankf@gmail.com",
					Subject = "Your OTP Code for Contract Verification",
					Body = $@"
                    <div style='font-family: ""Segoe UI"", Arial, sans-serif; color: #333; max-width: 600px; margin: 0 auto; background-color: #f9f9f9;'>
                        <!-- Header -->
                        <div style='background-color: #1a1a2e; padding: 20px; text-align: center; border-radius: 8px 8px 0 0;'>
                            <img src='https://res.cloudinary.com/dn8bn2sty/image/upload/v1739771796/image_vxdaik.png' alt='FFilms logo' style='width: 180px; height: auto;'/>
                        </div>
        
                        <!-- Content -->
                        <div style='background-color: #ffffff; padding: 30px; border-radius: 0 0 8px 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.05);'>
                            <h2 style='color: #1a1a2e; text-align: center; margin-bottom: 20px; font-size: 24px; font-weight: 600;'>Contract Verification</h2>
            
                            <p style='text-align: center; color: #666; margin-bottom: 5px; font-size: 14px;'>Account</p>
                            <p style='text-align: center; color: #1a1a2e; font-weight: 500; margin-top: 0; margin-bottom: 25px;'>{movieExists.User.Email}</p>
            
                            <p style='text-align: center; color: #555; margin-bottom: 20px;'>Please use the following OTP code to verify and sign your contract:</p>
            
                            <div style='text-align: center; margin: 30px 0;'>
                                <div style='display: inline-block; background-color: #f0f0f7; border: 1px solid #e0e0e0; padding: 15px 40px; border-radius: 8px; letter-spacing: 5px; font-size: 28px; font-weight: bold; color: #1a1a2e;'>
                                    {token}
                                </div>
                            </div>
            
                            <p style='text-align: center; color: #777; font-size: 13px; margin-top: 25px;'>This code is confidential. Please do not share it with anyone.</p>
                            <p style='text-align: center; color: #777; font-size: 13px;'>The code will expire in 10 minutes.</p>
                        </div>
        
                        <!-- Footer -->
                        <div style='padding: 20px; text-align: center;'>
                            <p style='color: #666; font-size: 14px; margin-bottom: 5px;'>© 2024 Eigakan</p>
                            <p style='color: #999; font-size: 12px; margin-top: 0;'>Secure Contract Management</p>
                        </div>
                    </div>",
				};

				await _emailService.SendEmailAsync(mailrequest);

				return new Result<string>
                {
                    Success = true,
                    Message = "Contract generated and uploaded successfully",
                    Data = uploadResult.Data
                };
            }
            catch (Exception ex)
            {
				transaction.Rollback();
                _logger.LogError(ex, "Error generating contract for request: {@Request}", request);
                return new Result<string> { Success = false, Message = $"Failed to generate contract: {ex.Message}" };
            }
        }

        public async Task<Result<string>> ExtendContractAsync(string originalContractId, ContractGenerationRequest request)
        {
            await using var transaction = await _contractRepository.BeginTransactionAsync();
            try
            {
                // Lấy contract gốc
                var originalContract = await _contractRepository.GetContractById(originalContractId);
                if (originalContract == null)
                {
                    return new Result<string> { Success = false, Message = "Original contract not found." };
                }
                if (originalContract.ExtendRequest != "PENDING")
                {
                    return new Result<string> { Success = false, Message = "No pending extend request found for this contract. Please submit an extend request first." };
                }

                // Kiểm tra điều kiện như cũ
                var movieExists = await _moviesRepository.GetMovieById(request.MovieId);
                if (movieExists == null || movieExists.Id != originalContract.MovieId)
                {
                    return new Result<string> { Success = false, Message = "Invalid movie for extension." };
                }

                DateTime? startDate = request.GetStartDate();
                if (startDate == null)
                    return new Result<string> { Success = false, Message = "Invalid StartDate format. Use dd/MM/yyyy." };
                if (request.Duration <= 0)
                    return new Result<string> { Success = false, Message = "Duration must be greater than 0." };

                DateTime endDate = startDate.Value.AddDays(request.Duration);

                byte[] docxBytes = await GenerateContractFromWordTemplate(request);
                byte[] pdfBytes = await ConvertDocxToPdf(docxBytes);
                var uploadResult = await UploadToS3(pdfBytes, request);
                if (!uploadResult.Success)
                    return uploadResult;

                string token = CreateRandomToken();

                // Tạo contract mới
                var newContract = new Contract
                {
                    Id = Guid.NewGuid().ToString(),
                    ContractDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    StartDate = startDate.Value,
                    EndDate = endDate,
                    Duration = request.Duration,
                    PublisherName = request.PublisherName,
                    DistributorName = request.DistributorName,
                    Price = request.Price.Value,
                    FileUrl = uploadResult.Data,
                    CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")),
                    Status = ContractStatusEnum.WAITING_FOR_REVIEWING.ToString(),
                    SignToken = token,
                    MovieId = request.MovieId,
                    UserId = movieExists.UserId,
                    OriginalContractId = originalContractId // Gán vào đây
                };

                await _contractRepository.InsertTransaction(newContract);

                // Cập nhật contract gốc
                originalContract.ExtendStatus = "EXTENDED";
                await _contractRepository.UpdateTransaction(originalContract);
                // Cập nhật trạng thái movie thành ACTIVE
                movieExists.Status = "ACTIVE";
                await _moviesRepository.UpdateTransaction(movieExists);

                // Lưu thay đổi
                await _moviesRepository.SaveChangeTransaction();
                await _contractRepository.SaveChangeTransaction();
                await transaction.CommitAsync();

                // Gửi OTP mail
                var mailRequest = new EmailSetting.MailResponse
                {
                    ToEmail = movieExists.User.Email,
                    Subject = "OTP Contract Extension",
                    Body = $@"
                 <div style='font-family:Arial, sans-serif; display:flex; justify-content:center;'>
               <div style='max-width: 600px; border: 1px solid #e0e0e0; border-radius: 10px; padding: 20px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                   <div style='text-align:center; padding:20px;'>
                       <img src='https://res.cloudinary.com/dn8bn2sty/image/upload/v1739771796/image_vxdaik.png' alt='FFilms logo' style='width: 250px; margin-bottom:10px;'/>
                   </div>
                   <h2 style='text-align:center;'>OTP Contract Extension</h2>
                   <p style='text-align:center; color: red;'>{movieExists.User.Email}</p>
                   <p style='text-align:center;'>This is your OTP for signing the extended contract. Please keep it secret!</p>
                   <div style=""text-align:center; border: 2px solid black; padding: 15px; display: inline-block; border-radius: 8px; font-size: 24px; font-weight: bold;"">
                     {token}
                 </div>
                   <p style='text-align:center;'>Eigakan</p>
               </div>
             </div>"
                };

                await _emailService.SendEmailAsync(mailRequest);

                return new Result<string> { Success = true, Message = "Contract extended successfully", Data = uploadResult.Data };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error extending contract {ContractId}", originalContractId);
                return new Result<string> { Success = false, Message = $"Error extending contract: {ex.Message}" };
            }
        }

        public async Task<Result<ContractGetAllResponse>> RequestContractExtensionAsync(string contractId)
        {
            var contract = await _contractRepository.GetContractById(contractId);
            if (contract == null)
            {
                return new Result<ContractGetAllResponse>
                {
                    Success = false,
                    Message = "Contract not found.",
                    Data = null
                };
            }

            bool isCancelling = contract.ExtendRequest == "PENDING";
            contract.ExtendRequest = isCancelling ? null : "PENDING";
            
            contract.UpdateDate = DateTime.UtcNow;

            await _contractRepository.Update(contract);

            return new Result<ContractGetAllResponse>
            {
                Success = true,
                Message = isCancelling ? "Extension request has been cancelled." : "Extension request submitted successfully.",
                Data = _mapper.Map<ContractGetAllResponse>(contract)
            };
        }

        public async Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractByUserId(string userId, int page, int pageSize, string? status, string? title)
        {


            var listContract = await _contractRepository.GetAllContractUserById(page, pageSize, userId, status, title);

            if (listContract == null)
            {
                return (new List<ContractGetAllResponse>(), 0);
            }

            var total = await _contractRepository.CountAllContractByUserIdAsync(userId);

            var response = listContract.Select(contract =>
            {
                var contractResponse = _mapper.Map<ContractGetAllResponse>(contract);
                if (contract.Movie != null)
                {
                    contractResponse.Movie = _mapper.Map<MovieGetAllResponse>(contract.Movie);

                    var mediaTypes = contract.Movie.Media?.Select(m => m.Type).ToList() ?? new List<string>();
                    contractResponse.Movie.IsFilmVipOrTrailer = mediaTypes.Contains("FILMVIP") && mediaTypes.Contains("TRAILER");

                    contractResponse.Movie.Medias = contract.Movie.Media?
                        .Where(m => m.Type == "POSTER")
                        .Select(m => _mapper.Map<MediaShortRespone>(m))
                        .ToList();



                }
                return contractResponse;
            }).ToList();


            return (_mapper.Map<List<ContractGetAllResponse>>(response), total);
        }

        private DateTime? ParseDate(string? dateString)
        {
            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }
            return null;
        }

		public async Task<byte[]> GenerateContractFromWordTemplate(ContractGenerationRequest request)
		{
			//  Lấy file từ aws
			byte[] templateBytes;
			using (var response = await _s3Client.GetObjectAsync(_bucketName, _s3Key))
			using (var memoryStream = new MemoryStream())
			{
				await response.ResponseStream.CopyToAsync(memoryStream);
				templateBytes = memoryStream.ToArray();
			}

			//  Load file 
			using (var memoryStream = new MemoryStream(templateBytes))
			using (var document = DocX.Load(memoryStream))
			{
				
				DateTime? startDateParsed = ParseDate(request.StartDate);
				int? durationDays = request.Duration;
				var movieExists = await _moviesRepository.GetMovieById(request.MovieId);
				string movieTitle = movieExists?.Title ?? "N/A";
				DateTime? endDateParsed = (startDateParsed.HasValue && durationDays.HasValue)
					? startDateParsed.Value.AddDays(durationDays.Value)
					: null;

				string startDate = startDateParsed?.ToString("dd/MM/yyyy") ?? "N/A";
				string endDate = endDateParsed?.ToString("dd/MM/yyyy") ?? "N/A";
				string duration = durationDays?.ToString() ?? "N/A";
				string price = request.Price.HasValue ? request.Price.Value.ToString("N0") + " VNĐ" : "N/A";

				
				document.ReplaceText("[publisherName]", request.PublisherName ?? "N/A");
				document.ReplaceText("[distributorName]", request.DistributorName ?? "N/A");
				document.ReplaceText("[duration]", duration);
				document.ReplaceText("[price]", price);
				document.ReplaceText("[startDate]", startDate);
				document.ReplaceText("[endDate]", endDate);
				document.ReplaceText("[movieName]", movieTitle);

				
				using (var outputStream = new MemoryStream())
				{
					document.SaveAs(outputStream);
					return outputStream.ToArray();
				}
			}
		}

		public async Task<byte[]> ConvertDocxToPdf(byte[] docxBytes)
		{
			using (MemoryStream inputStream = new MemoryStream(docxBytes))
			using (MemoryStream outputStream = new MemoryStream())
			{
				Document document = new Document();
				document.LoadFromStream(inputStream, FileFormat.Docx);
				document.SaveToStream(outputStream, FileFormat.PDF);

				return outputStream.ToArray();
			}
		}

		private async Task<Result<string>> UploadToS3(byte[] fileBytes, ContractGenerationRequest request)
        {
            var movieExists = await _moviesRepository.GetMovieById(request.MovieId);
            if (movieExists == null)
            {
                return new Result<string>
                {
                    Success = false,
                    Message = "Invalid MovieId: Movie does not exist."
                };
            }

            var movieOwnerId = movieExists.UserId;
            if (string.IsNullOrEmpty(movieOwnerId))
            {
                return new Result<string>
                {
                    Success = false,
                    Message = "Movie does not have an associated user."
                };
            }

            string fileName = $"{movieOwnerId}/{movieExists.Title}-Contract.pdf";
            try
            {
                using var stream = new MemoryStream(fileBytes);
                var key = $"contracts/{fileName}";

                var uploadRequest = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    Key = key,
                    BucketName = _bucketName,
                    ContentType = "application/pdf",
                    
                };

                var fileTransferUtility = new TransferUtility(_s3cc.Value);
                await fileTransferUtility.UploadAsync(uploadRequest);

                string fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{key}";

                return new Result<string>
                {
                    Success = true,
                    Data = fileUrl
                };
            }
            catch (AmazonS3Exception s3Ex)
            {
                _logger.LogError(s3Ex, "AWS S3 Error: {Message}", s3Ex.Message);
                return new Result<string>
                {
                    Success = false,
                    Message = "AWS S3 upload failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error uploading contract to AWS S3");
                return new Result<string>
                {
                    Success = false,
                    Message = "Unexpected error occurred while uploading file."
                };
            }
        }

		private string CreateRandomToken()
		{
			return Convert.ToHexString(RandomNumberGenerator.GetBytes(64)).Substring(0, 4);
		}

		// Update PDF
		public async Task<Result<string>> UpdateContractAsync(string contractId, ContractGenerationRequest request)
		{
			try
			{
				var movieExists = await _moviesRepository.GetMovieById(request.MovieId);
				if (movieExists == null)
				{
					return new Result<string>
					{
						Success = false,
						Message = "Invalid MovieId: Movie does not exist."
					};
				}
				var movieOwnerId = movieExists.UserId;
				if (string.IsNullOrEmpty(movieOwnerId))
				{
					return new Result<string>
					{
						Success = false,
						Message = "Movie does not have an associated user."
					};
				}

				DateTime? startDate = ParseDate(request.StartDate);
				int? durationDays = request.Duration;

				if (startDate == null)
				{
					return new Result<string> { Success = false, Message = "Invalid StartDate format. Use dd/MM/yyyy." };
				}
				if (durationDays == null || durationDays <= 0)
				{
					return new Result<string> { Success = false, Message = "Duration must be greater than 0." };
				}

				DateTime endDate = startDate.Value.AddDays(durationDays.Value);

				if (string.IsNullOrWhiteSpace(request.PublisherName))
				{
					return new Result<string> { Success = false, Message = "PublisherName cannot be empty." };
				}
				if (string.IsNullOrWhiteSpace(request.DistributorName))
				{
					return new Result<string> { Success = false, Message = "DistributorName cannot be empty." };
				}
				if (request.Price == null || request.Price.Value <= 0)
				{
					return new Result<string> { Success = false, Message = "Price must be greater than 0." };
				}

				var existingContract = await _contractRepository.GetContractById(contractId);
				if (existingContract == null)
				{
					return new Result<string> { Success = false, Message = "Invalid ContractId: Contract does not exist." };
				}

				byte[] docxBytes = await GenerateContractFromWordTemplate(request);
				byte[] pdfBytes = await ConvertDocxToPdf(docxBytes);

				// Upload to AWS S3
				var uploadResult = await UploadToS3(pdfBytes, request);
				if (!uploadResult.Success)
				{
					return uploadResult;
				}

				existingContract.ContractDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
				existingContract.StartDate = startDate.Value;
				existingContract.EndDate = endDate;
				existingContract.Duration = durationDays.Value;
				existingContract.PublisherName = request.PublisherName;
				existingContract.DistributorName = request.DistributorName;
				existingContract.Price = request.Price.Value;
				existingContract.FileUrl = uploadResult.Data;
				existingContract.UpdateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
				existingContract.Status = ContractStatusEnum.WAITING_FOR_REVIEWING.ToString();
				existingContract.MovieId = request.MovieId;
				existingContract.UserId = movieOwnerId;

				await _contractRepository.Update(existingContract);

				movieExists.ReasonForRejection = null;
				await _moviesRepository.Update(movieExists);

				return new Result<string> { Success = true, Message = "Contract updated and uploaded successfully.", Data = uploadResult.Data };
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating contract with Id: {ContractId}, Request: {@Request}", contractId, request);
				return new Result<string> { Success = false, Message = $"Failed to update contract: {ex.Message}" };
			}
		}

        public async Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractAsync(int page, int pageSize, string? status, string? title)       
        {

				//string CacheKey = $"ContractListAll_{page}_{pageSize}";
				//var cachedContracts = await _cacheService.GetCacheResponseAsync<(List<ContractGetAllResponse>, int)?>(CacheKey);

				var listContract = await _contractRepository.GetAllContract(page, pageSize,status,title);
                var total = await _contractRepository.CountAllContractAsync();

                var response = listContract.Select(contract =>
                {
                    var contractResponse = _mapper.Map<ContractGetAllResponse>(contract);
                    if (contract.Movie != null)
                    {
                        contractResponse.Movie = _mapper.Map<MovieGetAllResponse>(contract.Movie);
						var mediaTypes = contract.Movie.Media?.Select(m => m.Type).ToList() ?? new List<string>();
						contractResponse.Movie.IsFilmVipOrTrailer = mediaTypes.Contains("FILMVIP") && mediaTypes.Contains("TRAILER");

						contractResponse.Movie.Medias = contract.Movie.Media?
                            .Where(m => m.Type == "POSTER")
                            .Select(m => _mapper.Map<MediaShortRespone>(m)) 
                            .ToList();

                    }
                    return contractResponse;
                }).ToList();
			// **Lưu cache với thời gian 10 phút**
			//await _cacheService.SetCacheResponseAsync(CacheKey, (response, total), TimeSpan.FromMinutes(1000));

			return (_mapper.Map<List<ContractGetAllResponse>>(response), total);

		}
        
		public async Task<(List<ContractGetAllResponse> Contracts, int Total, int TotalSigned, decimal? TotalEarning)> GetAllContractByLogin(int page, int pageSize, string? status, string? title)        
		{
				var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);
				
				//string CacheKey = $"ContractListByLogin_{UserId.Value}_{page}_{pageSize}";
				//var cachedContracts = await _cacheService.GetCacheResponseAsync<(List<ContractGetAllResponse>, int)?>(CacheKey);

				//if (cachedContracts.HasValue) return cachedContracts.Value;

				var listContract = await _contractRepository.GetAllContractUserById(page, pageSize, UserId.Value, status, title); 
				
				var listContractNoPaging = await _contractRepository.GetAllContractNoPagingByUserId(UserId.Value);

				var response = listContract.Select(contract =>
				{
					var contractResponse = _mapper.Map<ContractGetAllResponse>(contract);
					if (contract.Movie != null)
					{
						contractResponse.Movie = _mapper.Map<MovieGetAllResponse>(contract.Movie);

						var mediaTypes = contract.Movie.Media?.Select(m => m.Type).ToList() ?? new List<string>();
						contractResponse.Movie.IsFilmVipOrTrailer = mediaTypes.Contains("FILMVIP") && mediaTypes.Contains("TRAILER");

						contractResponse.Movie.Medias = contract.Movie.Media?
							.Where(m => m.Type == "POSTER")
							.Select(m => _mapper.Map<MediaShortRespone>(m))
							.ToList();
					}
					return contractResponse;

				}).ToList();

				var total = await _contractRepository.CountAllContractByUserIdAsync(UserId.Value);
				
				var totalSigned = listContractNoPaging.Count(c => c.Status == ContractStatusEnum.SIGNED.ToString());

			var totalEarning = listContractNoPaging
					.Where(c => c.Status == ContractStatusEnum.SIGNED.ToString())
					.Sum(c => c.Price ?? 0);


			// **Lưu cache với thời gian 10 phút**
			//await _cacheService.SetCacheResponseAsync(CacheKey, (response, total), TimeSpan.FromMinutes(100000));


			return (response, total, totalSigned, totalEarning);
			
		}

        public async Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractAsyncByMovie(string movieId, int page, int pageSize, string? status, string? title)
        {

            //string CacheKey = $"ContractListAll_{page}_{pageSize}";
            //var cachedContracts = await _cacheService.GetCacheResponseAsync<(List<ContractGetAllResponse>, int)?>(CacheKey);

            var listContract = await _contractRepository.GetAllContractByMovie(movieId, page, pageSize, status, title);
            var total = await _contractRepository.CountAllContractByMovieIdAsync(movieId);

            var response = listContract.Select(contract =>
            {
                var contractResponse = _mapper.Map<ContractGetAllResponse>(contract);
                if (contract.Movie != null)
                {
                    contractResponse.Movie = _mapper.Map<MovieGetAllResponse>(contract.Movie);
                    var mediaTypes = contract.Movie.Media?.Select(m => m.Type).ToList() ?? new List<string>();
                    contractResponse.Movie.IsFilmVipOrTrailer = mediaTypes.Contains("FILMVIP") && mediaTypes.Contains("TRAILER");

                    contractResponse.Movie.Medias = contract.Movie.Media?
                        .Where(m => m.Type == "POSTER")
                        .Select(m => _mapper.Map<MediaShortRespone>(m))
                        .ToList();

                }
                return contractResponse;
            }).ToList();
            // **Lưu cache với thời gian 10 phút**
            //await _cacheService.SetCacheResponseAsync(CacheKey, (response, total), TimeSpan.FromMinutes(1000));

            return (_mapper.Map<List<ContractGetAllResponse>>(response), total);

        }
        
        public async Task<(List<ContractGetAllResponse> Contracts, int Total)> GetAllContractUserByMovie(string movieId ,int page, int pageSize, string? status, string? title)
		{
            var UserId = _httpContextAccessor.HttpContext?.User.FindFirst(MySetting.CLAIM_USERID);


            var listContract = await _contractRepository.GetAllContractUserByMovieId(page, pageSize, UserId.Value, movieId, status,title);
                
                if (listContract == null)
                {
				    return (new List<ContractGetAllResponse>(), 0);
			    }

				var total = await _contractRepository.CountAllContractByUserIdAndMovieIdAsync(UserId.Value, movieId);

				var response = listContract.Select(contract =>
                {
                    var contractResponse = _mapper.Map<ContractGetAllResponse>(contract);
                    if (contract.Movie != null)
                    {
                        contractResponse.Movie = _mapper.Map<MovieGetAllResponse>(contract.Movie);

						var mediaTypes = contract.Movie.Media?.Select(m => m.Type).ToList() ?? new List<string>();
						contractResponse.Movie.IsFilmVipOrTrailer = mediaTypes.Contains("FILMVIP") && mediaTypes.Contains("TRAILER");

						contractResponse.Movie.Medias = contract.Movie.Media?
                            .Where(m => m.Type == "POSTER")
                            .Select(m => _mapper.Map<MediaShortRespone>(m))
                            .ToList();

						

					}
                    return contractResponse;
                }).ToList();

		
			return (_mapper.Map<List<ContractGetAllResponse>>(response), total);
		}
       
		public async Task<Result<ContractGetAllResponse>> GetContractById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return new Result<ContractGetAllResponse>
                {
                    Success = false,
                    Message = "Id must not be null or empty."
                };
            }

            try
            {
                var contract = await _contractRepository.GetContractById(id);

                if (contract == null)
                {
                    return new Result<ContractGetAllResponse>
                    {
                        Success = false,
                        Message = "Contract with the specified Id does not exist."
                    };
                }

                var contractResponse = _mapper.Map<ContractGetAllResponse>(contract);

                if (contract.Movie != null)
                {
                    contractResponse.Movie = _mapper.Map<MovieGetAllResponse>(contract.Movie);

                    if (contract.Movie.Media != null)
                    {
						var mediaTypes = contract.Movie.Media?.Select(m => m.Type).ToList() ?? new List<string>();
						contractResponse.Movie.IsFilmVipOrTrailer = mediaTypes.Contains("FILMVIP") && mediaTypes.Contains("TRAILER");

						contractResponse.Movie.Medias = contract.Movie.Media
                            .Where(m => m.Type == "POSTER")
                            .Select(m => _mapper.Map<MediaShortRespone>(m))
                            .ToList();
                    }
                }

                return new Result<ContractGetAllResponse>
                {
                    Success = true,
                    Message = "Contract retrieved successfully.",
                    Data = contractResponse
                };
            }
            catch (Exception ex)
            {
                return new Result<ContractGetAllResponse>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving the contract: {ex.Message}"
                };
            }
        }

        public async Task<Result<Contract>> AcceptedContract(AcceptContractRequest acceptContractRequest)
		{
			try
			{

				var existingContract = await _contractRepository.GetContractById(acceptContractRequest.Id);

				if (existingContract == null)
					return new Result<Contract> { Success = false, Message = "Id does not exist" };

				if (existingContract.SignToken != acceptContractRequest.SignToken)
                {
                    return new Result<Contract> { Success = false, Message = "Invalid SignToken" };
                }

				if (existingContract.Status == ContractStatusEnum.SIGNED.ToString() || existingContract.Status == ContractStatusEnum.DENIED.ToString())
					return new Result<Contract> { Success = false, Message = "Can not update this register" };

				

				existingContract.Status = ContractStatusEnum.SIGNED.ToString();
                existingContract.IsSigned = true;

				await _contractRepository.Update(existingContract);

                var movie = await _moviesRepository.GetMovieById(existingContract.MovieId);

				return new Result<Contract>
				{
					Success = true,
					Message = "Update status successfull"
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, nameof(ContractService));
				return new Result<Contract> { Success = false, Message = ex.Message };
			}
		}

		public async Task<Result<Contract>> DeniedContract(DeniedContractRequest deniedContractRequest)
		{
			try
			{

				var existingContract = await _contractRepository.GetContractById(deniedContractRequest.Id);

				if (existingContract == null)
					return new Result<Contract> { Success = false, Message = "Id does not exist" };

				if (existingContract.Status == ContractStatusEnum.DENIED.ToString() || existingContract.Status == ContractStatusEnum.SIGNED.ToString())
					return new Result<Contract> { Success = false, Message = "Can not update this register" };
				

				existingContract.Status = ContractStatusEnum.DENIED.ToString();
				existingContract.ReasonForDenying = deniedContractRequest.ReasonForDenying;

				await _contractRepository.Update(existingContract);

				return new Result<Contract>
				{
					Success = true,
					Message = "Update status successfull"
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, nameof(Contract));
				return new Result<Contract> { Success = false, Message = ex.Message };
			}
		}

		//di chuyển folder tạm sang chính với uid user
		private async Task<string> MoveFileToMovieFolderAsync(string tempFileUrl, string movieId)
		{
			var tempFileMatch = Regex.Match(tempFileUrl, @".*/temp-uploads/(?<id>[a-f0-9-]+)/(?<filename>.+)");
			if (tempFileMatch.Success)
			{
				var fileId = tempFileMatch.Groups["id"].Value;
				var fileName = tempFileMatch.Groups["filename"].Value;

				var sourceKey = $"temp-uploads/{fileId}/{fileName}";
				var destinationKey = $"movie-uploads/{movieId}/{fileName}";

				// Copy file từ temp-uploads vào user-uploads/{userId}/
				var copyRequest = new CopyObjectRequest
				{
					SourceBucket = "file-eigakan",
					DestinationBucket = "file-eigakan",
					SourceKey = sourceKey,
					DestinationKey = destinationKey
				};

				await _s3Client.CopyObjectAsync(copyRequest);

				// Xóa file trong temp-uploads
				//await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
				//{
				//	BucketName = "file-eigakan",
				//	Key = sourceKey
				//});


				return $"https://file-eigakan.s3.ap-southeast-2.amazonaws.com/{destinationKey}";
			}

			throw new ArgumentException("Invalid temp file URL", nameof(tempFileUrl));
		}

	}
}
