using System;
using System.Security.Cryptography;
using System.Text;

namespace VisitorManagment.Core.Security
{
    /// <summary>هش‌کردن و مقایسهٔ پاسخ امنیتی با salt مستقل و PBKDF2-SHA256.</summary>
    public static class SecurityAnswerHasher
    {
        private const int Iterations = 120000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        public static string Hash(string answer)
        {
            var normalized = Normalize(answer);
            var salt = new byte[SaltSize];
            using (var random = RandomNumberGenerator.Create()) random.GetBytes(salt);
            using (var derive = new Rfc2898DeriveBytes(normalized, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var hash = derive.GetBytes(HashSize);
                return Iterations + "$" + Convert.ToBase64String(salt) + "$" + Convert.ToBase64String(hash);
            }
        }

        public static bool Verify(string answer, string storedValue)
        {
            if (string.IsNullOrWhiteSpace(storedValue)) return false;
            var parts = storedValue.Split('$');
            if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations) || iterations < 10000) return false;
            try
            {
                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);
                using (var derive = new Rfc2898DeriveBytes(Normalize(answer), salt, iterations, HashAlgorithmName.SHA256))
                {
                    var actual = derive.GetBytes(expected.Length);
                    return actual.Length == expected.Length && CryptographicOperations.FixedTimeEquals(actual, expected);
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string Normalize(string value)
        {
            if (value == null) return string.Empty;
            var normalized = value.Trim().Replace('ي', 'ی').Replace('ك', 'ک').ToUpperInvariant();
            var builder = new StringBuilder(normalized.Length);
            var previousWasSpace = false;
            foreach (var character in normalized)
            {
                if (char.IsWhiteSpace(character))
                {
                    if (!previousWasSpace) builder.Append(' ');
                    previousWasSpace = true;
                }
                else
                {
                    builder.Append(character);
                    previousWasSpace = false;
                }
            }
            return builder.ToString();
        }
    }

    public static class PasswordPolicy
    {
        private static readonly System.Text.RegularExpressions.Regex StrongPassword =
            new System.Text.RegularExpressions.Regex(@"\A(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,64}\z",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant);
        public static bool IsValid(string password) => !string.IsNullOrEmpty(password) && StrongPassword.IsMatch(password);
    }

    public static class SecurityQuestionCatalog
    {
        public const int FirstSchool = 1;
        public const int FirstTeacher = 2;
        public const int FirstStoryBook = 3;

        public static string GetText(int questionId)
        {
            switch (questionId)
            {
                case FirstSchool: return "نام اولین مدرسه‌ای که در آن درس خواندید چه بود؟";
                case FirstTeacher: return "نام اولین معلم دبستان شما چه بود؟";
                case FirstStoryBook: return "نام اولین کتاب داستانی که خواندید چه بود؟";
                default: return null;
            }
        }
    }
}
