using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Application.Helper.SignalR
{
    public class RoomHub : Hub
    {
        private static readonly Dictionary<string, List<dynamic>> RoomUsers = new();
        private static readonly Dictionary<string, string> ConnectionToRoom = new();
        private static readonly Dictionary<string, HashSet<string>> UserConnections = new();
        private static readonly Dictionary<string, double> RoomVideoStates = new();
        private static readonly Dictionary<string, bool> RoomPlayingStates = new();
		private static Dictionary<string, DateTime> RoomStartTime = new();
		private static Dictionary<string, string> RoomHosts = new Dictionary<string, string>();

		//Connect - Disconnect
		public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

		public override async Task OnDisconnectedAsync(Exception exception)
		{
			if (ConnectionToRoom.TryGetValue(Context.ConnectionId, out var roomId))
			{
				await CleanupUserFromRoom(Context.ConnectionId, roomId);
			}

			// Cleanup user connections
			var userName = RoomUsers
				.SelectMany(r => r.Value)
				.FirstOrDefault(u => ((dynamic)u).Id == Context.ConnectionId)?
				.UserName as string;

			if (userName != null && UserConnections.ContainsKey(userName))
			{
				UserConnections[userName].Remove(Context.ConnectionId);
				if (!UserConnections[userName].Any())
				{
					UserConnections.Remove(userName);
				}
			}

			await base.OnDisconnectedAsync(exception);
		}


		//Join - Leave Room
		public async Task JoinRoom(string roomId, string userName, string avatar = null, string userId = null)
		{
			Console.WriteLine($"JoinRoom: {roomId}, {userName}, {avatar}, {userId}");
			try
			{
				await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
				ConnectionToRoom[Context.ConnectionId] = roomId;

				// Add user to UserConnections for tracking
				if (!UserConnections.ContainsKey(userName))
				{
					UserConnections[userName] = new HashSet<string>();
				}
				UserConnections[userName].Add(Context.ConnectionId);

				// Ensure we have a room users list
				if (!RoomUsers.ContainsKey(roomId))
				{
					RoomUsers[roomId] = new List<object>();
				}

				// 🏆 Xác định host trước khi thêm user
				bool isHost = !RoomHosts.ContainsKey(roomId);
				if (isHost)
				{
					RoomHosts[roomId] = Context.ConnectionId;
				}

				// Create user object with consistent ID
				var userObject = new
				{
					Id = Context.ConnectionId,
					UserId = userId,
					UserName = userName,
					Avatar = avatar ?? "/default-avatar.png",
					IsMuted = false,
					IsVideoOff = false,
					IsHost = isHost
				};

				// Add to room users list if not already present
				if (!RoomUsers[roomId].Any(u => ((dynamic)u).Id == Context.ConnectionId))
				{
					RoomUsers[roomId].Add(userObject);
				}
				Console.WriteLine($"User {userName} joined room {roomId}, isHost : {isHost}");



				// Notify everyone about the new user
				await Clients.Group(roomId).SendAsync("UserJoined", userObject);

				// Send the updated participants list to ensure consistency
				await Clients.Group(roomId).SendAsync("UpdateParticipants", RoomUsers[roomId]);



			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error in JoinRoom: {ex.Message}");
			}
		}

		private async Task CleanupUserFromRoom(string connectionId, string roomId)
		{
			if (RoomUsers.ContainsKey(roomId))
			{
				var user = RoomUsers[roomId].FirstOrDefault(u => ((dynamic)u).Id == connectionId);
				if (user != null)
				{
					RoomUsers[roomId].RemoveAll(u => ((dynamic)u).Id == connectionId);
					await Groups.RemoveFromGroupAsync(connectionId, roomId);
					ConnectionToRoom.Remove(connectionId);

					Console.WriteLine($"User {((dynamic)user).UserName} left room {roomId}");

					// Thông báo user rời khỏi phòng
					await Clients.Group(roomId).SendAsync("UserLeft", new
					{
						Id = connectionId,
						ConnectionId = connectionId,
						UserName = ((dynamic)user).UserName,
						Avatar = ((dynamic)user).Avatar,
						UserId = ((dynamic)user).UserId
					});

					// Kiểm tra nếu user rời đi có phải là host hay không
					if (RoomHosts.TryGetValue(roomId, out string currentHostId) && currentHostId == connectionId)
					{
						if (RoomUsers[roomId].Any())
						{
							// Gán host mới là user tiếp theo trong danh sách
							var newHost = RoomUsers[roomId].FirstOrDefault();
							if (newHost != null)
							{
								string newHostId = ((dynamic)newHost).Id;
								RoomHosts[roomId] = newHostId;

								Console.WriteLine($"New host assigned: {((dynamic)newHost).UserName}");

								// Cập nhật trạng thái host cho toàn bộ phòng
								await Clients.Group(roomId).SendAsync("UpdateHost", new
								{
									Id = newHostId,
									UserId = ((dynamic)newHost).UserId,
									UserName = ((dynamic)newHost).UserName,
									Avatar = ((dynamic)newHost).Avatar,
									IsHost = true
								});
							}

							var hostData = new
							{
								Message = $"{((dynamic)newHost).UserName} is now the host!",
								UserId = ((dynamic)newHost).UserId
							};
							Console.WriteLine(hostData.Message);

							await Clients.Group(roomId).SendAsync("HostChanged", hostData);
						}
						else
						{
							// Nếu phòng trống, xóa luôn host
							RoomHosts.Remove(roomId);
						}
					}


					// Cập nhật danh sách user
					await Clients.Group(roomId).SendAsync("UpdateParticipants", RoomUsers[roomId]);

					// Nếu phòng trống, dọn dẹp state và pause video
					if (!RoomUsers[roomId].Any())
					{
						RoomUsers.Remove(roomId);
						RoomHosts.Remove(roomId);
						RoomVideoStates.Remove(roomId);
						RoomPlayingStates.Remove(roomId);

						await Clients.Group(roomId).SendAsync("ReceiveVideoControl", "pause",
							RoomVideoStates.GetValueOrDefault(roomId));
					}
				}
			}
		}

		public async Task LeaveRoom(string roomId)
		{
			await CleanupUserFromRoom(Context.ConnectionId, roomId);
		}

		
		//Chat
		public async Task SendMessage(string roomId, string userName, string message, string avatar = null)
		{
			var chatMessage = new
			{
				Id = Guid.NewGuid().ToString(),
				Text = message,
				Sender = new
				{
					UserName = userName,
					Avatar = avatar ?? "/default-avatar.png"
				},
				Timestamp = DateTime.UtcNow
			};
			Console.WriteLine(chatMessage);
			await Clients.Group(roomId).SendAsync("ReceiveMessage", chatMessage);
		}

		
		//Manage Room
		public async Task RequestCurrentTime(string roomId)
		{
			if (RoomVideoStates.TryGetValue(roomId, out double currentTime))
			{
				var callerId = Context.ConnectionId;
				await Clients.Client(callerId).SendAsync("ReceiveVideoTime", currentTime);
			}
		}

		public async Task SyncPlayPause(object data)
		{

			await Clients.Others.SendAsync("SyncPlayPause", data);
			Console.WriteLine($"SyncPlayPause: {data}");
		}

		public async Task SyncTime(object data)
		{
			await Clients.Others.SendAsync("SyncTime", data);
			Console.WriteLine($"SyncTime: {data}");
		}


	}
}