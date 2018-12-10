using System;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;

namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class UserConversationExtenstions
    {
        public static UserConversation ToModel(this UserConversationViewModel viewModel)
        {
            return new UserConversation
            {
                IsOwner = viewModel.IsOwner,
                ConversationId = viewModel.ConversationId,
                NotRead = viewModel.NotRead,
                NotReadCount = viewModel.NotReadCount,
                UserId = viewModel.UserId
            };
        }

        public static UserConversationViewModel ToViewModel(this UserConversation model)
        {
            return new UserConversationViewModel
            {
                ConversationId = model.ConversationId,
                IsOwner = model.IsOwner,
                NotRead = model.NotRead,
                NotReadCount = model.NotReadCount,
                UserId = model.UserId
            };
        }
    }
}
