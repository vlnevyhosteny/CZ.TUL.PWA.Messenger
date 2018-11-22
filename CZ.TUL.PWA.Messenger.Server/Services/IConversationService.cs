using System.Threading.Tasks;
namespace CZ.TUL.PWA.Messenger.Server.Services
{
    public interface IConversationService
    {
        Task<bool> BelongsToUser(string userId, int conversationId);
    }
}
