using API.Configurations.InitialUsers.Interfaces;
using API.Enums.Identity;
using System.Text.RegularExpressions;

namespace API.Validations
{
    public static class InitialUserValidator
    {
        public static bool Validate(IInitialUser user)
        {
            // Validación de Username
            if (string.IsNullOrWhiteSpace(user.Username))
                return false;

            // Validación de Email (debe tener el formato correcto: hola@domain.com)
            if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return false;

            // Validación de FullName
            if (string.IsNullOrWhiteSpace(user.FullName))
                return false;

            // Validación de Password (al menos 8 caracteres, 1 letra mayúscula, 1 letra minúscula, 1 número)
            if (user.Password.Length < 8 ||
                !Regex.IsMatch(user.Password, @"[A-Z]") ||
                !Regex.IsMatch(user.Password, @"[a-z]") ||
                !Regex.IsMatch(user.Password, @"\d"))
                return false;

            // Validación de Role (debe ser uno de los valores del enum UserRole)
            if (string.IsNullOrWhiteSpace(user.Role))
                return false;

            if (!Enum.TryParse<UserRole>(user.Role.Trim(), out _))
                return false;

            return true;
        }
    }
}