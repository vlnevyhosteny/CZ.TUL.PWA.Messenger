using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Tests.Utilities;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CZ.TUL.PWA.Messenger.Server.Tests.Controllers
{
    public class MessagesControllerTests
        : IClassFixture<MessengerWebApplicationFactory<Startup>>
    {
        private readonly MessengerWebApplicationFactory<Startup> factory;

        public MessagesControllerTests(MessengerWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task Get_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            this.SeedDataForMessagesTesting(context);
            User testUser = await context.Users.FirstAsync();

            var response = await client.GetAsync("/api/messages");
            response.EnsureSuccessStatusCode();

            var messages = response.Content.ReadAsAsync(typeof(IEnumerable<MessageViewModel>)).Result as IEnumerable<MessageViewModel>;

            int messagesCount = await context.Messages.CountAsync(x => x.OwnerId == testUser.Id);

            Assert.NotNull(messages);
            Assert.Equal(messagesCount, messages.Count());
        }

        [Fact]
        public async Task GetById_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            this.SeedDataForMessagesTesting(context);
            User testUser = await context.Users.FirstAsync();
            Message message = await context.Messages.Skip(1).FirstAsync();

            var response = await client.GetAsync($"/api/messages/{message.MessageId}");
            response.EnsureSuccessStatusCode();

            var responseMessage = response.Content.ReadAsAsync(typeof(MessageViewModel)).Result as MessageViewModel;

            Assert.NotNull(responseMessage);
            Assert.Equal(message.Content, message.Content);
        }

        [Fact]
        public async Task PutUpdate_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            UserManager<User> userManager = this.factory.Server.Host.Services.GetService(typeof(UserManager<User>))
                                    as UserManager<User>;

            foreach (var item in this.GenerateUsers(5))
            {
                await userManager.CreateAsync(item);
            }

            var dbUser = await context.Users.Skip(2).FirstAsync();

            string newName = "New Name";

            var requestBody = new UserViewModel { Id = dbUser.Id, UserName = dbUser.UserName, Name = newName };
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/users/{dbUser.Id}", requestContent);
            response.EnsureSuccessStatusCode();

            userManager = this.factory.Server.Host.Services.GetService(typeof(UserManager<User>))
                                    as UserManager<User>;

            var updatedUser = await userManager.FindByIdAsync(dbUser.Id);

            Assert.Equal(newName, updatedUser.Name);
        }

        [Fact]
        public async Task Delete_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            await context.Users.AddRangeAsync(this.GenerateUsers(5));
            await context.SaveChangesAsync();

            var dbUser = await context.Users.Skip(2).FirstAsync();
            int previousCount = await context.Users.CountAsync();

            var response = await client.DeleteAsync($"/api/users/{dbUser.Id}");
            response.EnsureSuccessStatusCode();

            int currentCount = await context.Users.CountAsync();

            Assert.NotEqual(previousCount, currentCount);
            Assert.Equal(previousCount, currentCount + 1);
            Assert.False(await context.Users.AnyAsync(x => x.Id == dbUser.Id));
        }

        private async void SeedDataForMessagesTesting(MessengerContext context)
        {
            User testUser = context.Users.First();

            Conversation conversation = new Conversation
            {
                Name = "test conversation"
            };
            await context.Conversations.AddAsync(conversation);
            await context.SaveChangesAsync();

            UserConversation userConversation = new UserConversation
            {
                ConversationId = conversation.ConversationId,
                IsOwner = true,
                UserId = testUser.Id
            };
            await context.UserConversation.AddAsync(userConversation);
            await context.SaveChangesAsync();

            await context.Messages.AddRangeAsync(new Message[]
            {
                new Message
                {
                    Content = "test message 1",
                    OwnerId = testUser.Id,
                    DateSent = DateTime.Now,
                    Conversation = conversation
                },
                new Message
                {
                    Content = "test message 2",
                    OwnerId = testUser.Id,
                    DateSent = DateTime.Now.AddSeconds(5),
                    Conversation = conversation
                }
            });

            await context.SaveChangesAsync();
        }
    }
}
