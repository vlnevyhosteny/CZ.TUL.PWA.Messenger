using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CZ.TUL.PWA.Messenger.Server.Model
{
    public class RefreshToken
    {
        public string UserId { get; set; }

        public DateTime Expires { get; set; }

        public bool Revoked { get; set; }

        public string Token { get; set; }

        public User User { get; set; }
    }
}
