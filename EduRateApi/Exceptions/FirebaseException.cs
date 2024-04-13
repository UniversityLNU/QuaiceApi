using FireSharp.Exceptions;

namespace EduRateApi.Exceptions
{
    public class FirebaseExceptions : FirebaseException
    {
        public FirebaseErrorReason Reason { get; }

        public FirebaseExceptions(string message, FirebaseErrorReason reason = FirebaseErrorReason.Undefined) : base(message)
        {
            Reason = reason;
        }

        public FirebaseExceptions(string message, Exception innerException, FirebaseErrorReason reason = FirebaseErrorReason.Undefined) : base(message, innerException)
        {
            Reason = reason;
        }
    }

    public enum FirebaseErrorReason
    {
        Undefined,
        NoTeachersFound,
        FirebaseConnectionFailed,
        DatabaseOperationFailed,
    }
}
