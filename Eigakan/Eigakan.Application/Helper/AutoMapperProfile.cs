using AutoMapper;
using Eigakan.Domain.Models;
using Eigakan.Domain.Request.UserRequest;
using Eigakan.Domain.Response;
using Eigakan.Domain.Response.ContractResponse;
using Eigakan.Domain.Response.Genre;
using Eigakan.Domain.Response.Media;
using Eigakan.Domain.Response.Movie;
using Eigakan.Domain.Response.Person;
using Eigakan.Domain.Response.News;
using Eigakan.Domain.Response.SubscriptionPackageResponse;
using Eigakan.Domain.Response.SubscriptionPurchaseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Eigakan.DomainMongoDB.Models;
using Eigakan.DomainMongoDB.Request;
using Eigakan.Domain.Response.MovieHistory;
using Eigakan.Domain.Response.MovieEarning;
using Eigakan.Domain.Response.UserEarning;
using Eigakan.Domain.Response.UserWallet;
using Eigakan.Domain.Response.AdPurchaseTransaction;
using Eigakan.Domain.Response.AdPurchaseItem;
using Eigakan.Domain.Response.AdMediaResponse;
using Eigakan.Domain.Response.WalletTransaction;

namespace Eigakan.Application.Helper
{
    public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			//User 
			CreateMap<User, UserGetAllResponse>()
				.ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.Name));
			CreateMap<User, UserGetAllResponse>();

			CreateMap<User, UserEdit>();
			
			CreateMap<Movie, MovieGetListResponse>()
				.ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.MovieGenres))
				.ForMember(dest => dest.MoviePersons, opt => opt.MapFrom(src => src.MoviePersons))
				.ForMember(dest => dest.Medias, opt => opt.MapFrom(src => src.Media));

			CreateMap<Contract, ContractGetAllResponse>();
            CreateMap<Contract, ContractGetAllResponse>()
                .ForMember(dest => dest.Movie, opt => opt.MapFrom(src => src.Movie));

			CreateMap<Movie, MovieGetById>()
				.ForMember(dest => dest.MovieGenres, opt => opt.MapFrom(src => src.MovieGenres))
				.ForMember(dest => dest.MoviePersons, opt => opt.MapFrom(src => src.MoviePersons))
				.ForMember(dest => dest.Medias, opt => opt.MapFrom(src => src.Media))
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
				.ForMember(dest => dest.contracts, opt => opt.MapFrom(src => src.contracts));

			CreateMap<Contract, ContractGetName>()
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate));


			CreateMap<Movie, MovieGetAllResponse>()
				.ForMember(dest => dest.Medias, opt => opt.MapFrom(src => src.Media));


            CreateMap<Genre, GenreListNameResponse>();
			CreateMap<Person, PersonListResponse>();
            CreateMap<Person, PersonReturnMovieListResponse>();
			CreateMap<Media, MediaResponse>();
            CreateMap<Media, MediaShortRespone>(); 

            CreateMap<SubscriptionPackage, SubscriptionPackageGetAllResponse>();
			CreateMap<SubscriptionPurchase, SubscriptionPurchaseGetAllResponse>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));


			CreateMap<News, NewsResponse>()
				.ForMember(dest => dest.UserName,
					opt => opt.MapFrom(src => src.User.FullName));


            CreateMap<RoomCreateRequest, Room>()
				.ForMember(dest => dest.Id, opt => opt.Ignore()) // Id sẽ được gán thủ công
				.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true)) // Mặc định IsActive = true
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => "Active")); // Set mặc định "Active"


			//moviehistory	
			CreateMap<MovieHistory, MovieHistoryResponse>()
				 .ForMember(dest => dest.Movies, opt => opt.MapFrom(src => src.Movie));
			
			CreateMap<Movie, MovieResponse>()
				.ForMember(dest => dest.Medias, opt => opt.MapFrom(src => src.Media));

			//Movie Earning
			CreateMap<MovieEarning, MovieEarningResponse>()
					.ForMember(dest => dest.MovieName, opt => opt.MapFrom(src => src.Movie.OriginName));

			//User Earning
			CreateMap<UserEarning, UserEarningResponse>()
					.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName));


			//User Wallet
			CreateMap<UserWallet, UserWalletGetAllResponse>();


			//AdPurchaseTranasction
			CreateMap<AdPurchaseTransaction, AdPurchaseTransactionGetAllResponse>();

            //AdPurchaseItem
            CreateMap<AdPurchaseItems, AdPurchaseItemGetAllResponse>()
				.ForMember(dest => dest.AdPackageName, opt => opt.MapFrom(src => src.AdPackage != null ? src.AdPackage.PackageName : null))
				.ForMember(dest => dest.AdMediaUrl, opt => opt.MapFrom(src => src.AdMedia != null ? src.AdMedia.Url : null))
				.ForMember(dest => dest.AdMediaStatus, opt => opt.MapFrom(src => src.AdMedia != null ? src.AdMedia.status : null))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AdPurchaseTransaction != null ? src.AdPurchaseTransaction.UserId : null))
				.ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.AdPurchaseTransaction != null && src.AdPurchaseTransaction.User != null ? src.AdPurchaseTransaction.User.FullName : null));

			CreateMap<AdPurchaseItems, AdPurchaseItemsResponse>();

			//AdMedia
			CreateMap<AdMedia, AdMediaWithPositionDto>();
			CreateMap<AdMedia, AdMediaGetAllResponse>();

            //WalletTransaction
            CreateMap<WalletTransaction, WalletTransactionGetAllResponse>();
        }
    }
}
