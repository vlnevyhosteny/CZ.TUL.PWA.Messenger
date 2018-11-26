using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.Tests.Utilities;
using CZ.TUL.PWA.Messenger.Server.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

            var messages = response.Content.ReadAsAsync(typeof(IEnumerable<EditMessageViewModel>)).Result as IEnumerable<EditMessageViewModel>;

            int messagesCount = await context.Messages.CountAsync(x => x.OwnerId == testUser.Id);

            Assert.NotNull(messages);
            Assert.Equal(messagesCount, messages.Count());
        }

        [Fact]
        public async Task GetByConversation_ShouldSuccess()
        {
            var client = this.factory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                await AuthenticationUtilities.GetTestUserAccessTokenAsync(client));

            MessengerContext context = this.factory.Server.Host.Services.GetService(typeof(MessengerContext))
                                               as MessengerContext;

            this.SeedDataForMessagesTesting(context);
            User testUser = await context.Users.FirstAsync();
            Conversation testConversation = await context.Conversations.FirstAsync();

            var response = await client.GetAsync($"/api/messages/Conversation/{testConversation.ConversationId}");
            response.EnsureSuccessStatusCode();

            var messages = response.Content.ReadAsAsync(typeof(IEnumerable<EditMessageViewModel>)).Result as IEnumerable<EditMessageViewModel>;

            int messagesCount = await context.Messages.CountAsync(x => x.OwnerId == testUser.Id
                                                                  && x.Conversation.ConversationId == testConversation.ConversationId);

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

            var responseMessage = response.Content.ReadAsAsync(typeof(EditMessageViewModel)).Result as EditMessageViewModel;

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

            this.SeedDataForMessagesTesting(context);
            User testUser = await context.Users.FirstAsync();
            Message message = await context.Messages.Skip(1).FirstAsync();

            string newContent = "new content";

            var requestBody = new EditMessageViewModel { MessageId = message.MessageId, Content = newContent };
            var requestContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/messages/{message.MessageId}", requestContent);
            response.EnsureSuccessStatusCode();

            Message updated = await context.Messages.SingleAsync(x => x.MessageId == message.MessageId);

            Assert.Equal(newContent, updated.Content);
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

            this.SeedDataForMessagesTesting(context);
            User testUser = await context.Users.FirstAsync();
            Message message = await context.Messages.Skip(1).FirstAsync();

            int previousCount = await context.Messages.CountAsync();

            var response = await client.DeleteAsync($"/api/messages/{message.MessageId}");
            response.EnsureSuccessStatusCode();

            int currentCount = await context.Messages.CountAsync();

            Assert.NotEqual(previousCount, currentCount);
            Assert.Equal(previousCount, currentCount + 1);
            Assert.False(await context.Messages.AnyAsync(x => x.MessageId == message.MessageId));
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
            await context.UserConversations.AddAsync(userConversation);
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
