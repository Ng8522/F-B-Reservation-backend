using System.Text.RegularExpressions;

namespace FnbReservationAPI.src.utils
{
    public static class Email
    {
        private static readonly string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                return Regex.IsMatch(email, EmailPattern);
            }
            catch
            {
                return false;
            }
        }
    }
} 