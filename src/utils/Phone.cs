using System.Text.RegularExpressions;

namespace FnbReservationAPI.src.utils
{
    public static class Phone
    {

        private static readonly string PhonePattern = @"^\+?[0-9]{8,15}$";

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            try
            {
                var cleanedNumber = Regex.Replace(phoneNumber, @"[^\d+]", "");
                return Regex.IsMatch(cleanedNumber, PhonePattern);
            }
            catch
            {
                return false;
            }
        }

        public static string NormalizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return phoneNumber;

            // Remove all non-digit characters except plus sign
            var cleanedNumber = Regex.Replace(phoneNumber, @"[^\d+]", "");

            // If it starts with a plus sign, keep it
            if (cleanedNumber.StartsWith("+"))
            {
                return cleanedNumber;
            }

            // Otherwise, add the plus sign if it's a valid number
            return IsValidPhoneNumber(cleanedNumber) ? $"+{cleanedNumber}" : phoneNumber;
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return phoneNumber;

            var cleanedNumber = NormalizePhoneNumber(phoneNumber);
            
            // If it's not a valid number, return the original
            if (!IsValidPhoneNumber(cleanedNumber))
                return phoneNumber;

            // Remove the plus sign for formatting
            var numberWithoutPlus = cleanedNumber.TrimStart('+');

            // Format based on length
            if (numberWithoutPlus.Length == 10)
            {
                return $"+{numberWithoutPlus.Substring(0, 3)}-{numberWithoutPlus.Substring(3, 3)}-{numberWithoutPlus.Substring(6)}";
            }
            else if (numberWithoutPlus.Length == 11 && numberWithoutPlus.StartsWith("0"))
            {
                return $"+{numberWithoutPlus.Substring(1, 3)}-{numberWithoutPlus.Substring(4, 3)}-{numberWithoutPlus.Substring(7)}";
            }

            // For other lengths, just add spaces every 3 digits
            return $"+{Regex.Replace(numberWithoutPlus, ".{3}", "$&-").TrimEnd('-')}";
        }
    }
} 