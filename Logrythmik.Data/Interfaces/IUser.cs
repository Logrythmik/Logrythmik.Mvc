namespace Logrythmik.Data
{
    public interface IUser
    {
        string DisplayName { get; }
        string Email { get; }
        bool IsAdmin { get; }
    }
}