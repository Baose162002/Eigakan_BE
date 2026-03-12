using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Eigakan.Application.Interface;
using Eigakan.Application.Service;
using Eigakan.DomainMongoDB.Models;
using Eigakan.DomainMongoDB.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Eigakan.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveRooms()
        {
            var rooms = await _roomService.GetActiveRoomsAsync();
            return Ok(new { Success = true, Message = "List of active rooms", Data = rooms });
        }
        
        [Authorize(Roles = "VIP MEMBER")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomCreateRequest roomRequest)
        {
            try
            {
                var room = await _roomService.CreateRoomAsync(roomRequest);
                return Ok(new { Success = true, Message = "Create rooom successfuly!", Data = room });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
        
        [Authorize(Roles = "VIP MEMBER")]
        [HttpPost("join")]
        public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest request)
        {
            try
            {
                var usersInRoom = await _roomService.JoinRoomAsync(request);
                return Ok(new { Success = true, Message = "Join room successfully!", Data = usersInRoom });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message});
            }
        }

        [HttpPut("end/{roomId}")]
        public async Task<IActionResult> EndRoom(string roomId)
        {
            await _roomService.EndRoomAsync(roomId);
            return Ok(new { Success = true, Message = "Room is ended!" });
        }
        
        [Authorize(Roles = "VIP MEMBER")]
        [HttpGet("share-link/{roomId}")]
        public async Task<IActionResult> GetRoomLink(string roomId)
        {
            try
            {
                var link = await _roomService.GetRoomLinkAsync(roomId);
                return Ok(new { Success = true, Message = "Get link successfully!", Data = link });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message});
            }
        }

        [Authorize(Roles = "VIP MEMBER")]
        [HttpGet("check-user-in-room/{roomId}")]
        public async Task<IActionResult> CheckUserInRoom(string roomId)
        {
            try
            {
                var (isInRoom, isHost) = await _roomService.CheckUserInRoomAsync(roomId);
                return Ok(new { Success = true, IsInRoom = isInRoom, IsHost = isHost });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
        
        [Authorize(Roles = "VIP MEMBER")]
        [HttpPost("leave/{roomId}")]
        public async Task<IActionResult> LeaveRoom(string roomId)
        {
            try
            {
                bool result = await _roomService.LeaveRoomAsync(roomId);
                if (result)
                {
                    return Ok(new { Success = true, Message = "Leave room successfully!" });
                }
                return BadRequest(new { Success = false, Message = "Can't leave room!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

        [Authorize(Roles = "VIP MEMBER")]
        [HttpGet("get-users-in-room/{roomId}")]
        public async Task<IActionResult> GetUsersInRoom(string roomId)
        {
            try
            {
                var users = await _roomService.GetUsersInRoomAsync(roomId);
                return Ok(new { Success = true, Data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }

		[Authorize(Roles = "VIP MEMBER")]
		[HttpGet("get-host-room")]
		public async Task<IActionResult> GetHostRoom()
		{
			var room = await _roomService.GetHostRoomIdAsync();

			return Ok(new
			{
				Success = true,
				Message = room != null ? "Get User is host room successful" : "No active host room found",
				Data = room
			});
		}

		[HttpGet("GetAll")]
        public async Task<IActionResult> GetAllRooms()
        {
            try
            {
                var rooms = await _roomService.GetAllRoomsAsync();
                return Ok(new { Success = true, Message = "Get all rooms successful", Data = rooms });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
        
        [Authorize(Roles = "VIP MEMBER")]
        [HttpGet("GetRoomByUser")]
        public async Task<IActionResult> GetRoomsByUserId()
        {
            try
            {
                var room = await _roomService.GetHostRoomIdAsync();
                return Ok(new { Success = true, Message = "Get user is host room successful", Data = room });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = ex.Message });
            }
        }
        
        [Authorize(Roles = "VIP MEMBER")]
        [HttpGet("GetById/{roomId}")]
        public async Task<IActionResult> GetRoomById(string roomId)
        {
            var results = await _roomService.GetRoomByIdAsync(roomId);
			if (results.Success != false)
			{
				return Ok(new
				{
					results.Success,
					results.Message,
                    results.Data
				});
			}
			return BadRequest(new
			{
				results.Success,
				results.Message
			});
		}

    }

}
