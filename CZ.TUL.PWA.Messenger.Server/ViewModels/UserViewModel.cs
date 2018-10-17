using System;
namespace CZ.TUL.PWA.Messenger.Server.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel(string id, string userName, string name)
        {
            this.Id = id;
            this.UserName = userName;
            this.Name = name;
        }

        public string Id
        {
            get;
        }

        public string UserName
        {
            get;
        }

        public string Name 
        {
            get;
        }
    }
}
