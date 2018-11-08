using CZ.TUL.PWA.Messenger.Server.ViewModels;

namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class PagingViewModelExtensions
    {
        public static int ItemSkipCount(this PagingViewModel pagingViewModel)
        {
            return pagingViewModel.PageSize * (pagingViewModel.PageNumber - 1);
        }
    }
}
