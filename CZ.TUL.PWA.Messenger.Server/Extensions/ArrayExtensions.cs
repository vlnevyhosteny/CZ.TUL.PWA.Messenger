using System;
namespace CZ.TUL.PWA.Messenger.Server.Extensions
{
    public static class ArrayExtensions
    {
        public static bool Equals(this byte[] arrayOne, byte[] arrayTwo) 
        {
            int i;
            if (arrayOne.Length == arrayTwo.Length)
            {
                i = 0;
                while (i < arrayOne.Length && (arrayOne[i] == arrayTwo[i]))
                {
                    i++;
                }

                if (i == arrayOne.Length)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
