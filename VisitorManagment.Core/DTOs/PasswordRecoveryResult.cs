namespace VisitorManagment.Core.DTOs
{
    public enum PasswordRecoveryStatus
    {
        Success = 0,
        InvalidAnswer = 1,
        Locked = 2,
        NotConfigured = 3,
        UserNotFound = 4,
        InvalidPassword = 5
    }

    public class PasswordRecoveryResult
    {
        public PasswordRecoveryStatus Status { get; set; }
        public int AttemptsRemaining { get; set; }
    }
}
