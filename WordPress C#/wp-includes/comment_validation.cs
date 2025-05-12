public class CommentValidator
{
    public static bool IsValid(Comment comment)
    {
        if (string.IsNullOrWhiteSpace(comment.Author)) return false;
        if (string.IsNullOrWhiteSpace(comment.Content)) return false;
        if (string.IsNullOrWhiteSpace(comment.AuthorEmail) || !IsValidEmail(comment.AuthorEmail)) return false;
        return true;
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}