namespace V4H.Application.Common.Exceptions;

public class DocumentValidationException : Exception
{
    public decimal Score { get; }

    public DocumentValidationException(decimal score)
        : base($"Document validation failed. Score: {score:F2}")
    {
        Score = score;
    }
}
