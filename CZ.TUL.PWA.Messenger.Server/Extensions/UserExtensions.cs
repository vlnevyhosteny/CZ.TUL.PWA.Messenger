using System;
using CZ.TUL.PWA.Messenger.Server.Model;
using CZ.TUL.PWA.Messenger.Server.ViewModels;

namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class UserExtensions
    {
        public static UserViewModel ToViewModel(this User user) 
        {
            return new UserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName
            };
        }
    }
}
