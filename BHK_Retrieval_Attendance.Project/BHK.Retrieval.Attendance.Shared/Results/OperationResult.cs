using System;

namespace BHK.Retrieval.Attendance.Shared.Results
{
    /// <summary>
    /// Represents the result of an operation with success/failure status and optional data
    /// </summary>
    /// <typeparam name="T">The type of data returned on success</typeparam>
    public class OperationResult<T>
    {
        /// <summary>
        /// Indicates whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The data returned on successful operation
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// A user-friendly message describing the result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Detailed error information for debugging purposes
        /// </summary>
        public string ErrorDetails { get; set; }

        /// <summary>
        /// Exception that occurred during the operation (if any)
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Creates a successful operation result
        /// </summary>
        /// <param name="data">The data to return</param>
        /// <param name="message">Optional success message</param>
        /// <returns>A successful OperationResult</returns>
        public static OperationResult<T> Success(T data, string message = "")
        {
            return new OperationResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed operation result
        /// </summary>
        /// <param name="message">User-friendly error message</param>
        /// <param name="errorDetails">Detailed error information</param>
        /// <param name="exception">Exception that caused the failure</param>
        /// <returns>A failed OperationResult</returns>
        public static OperationResult<T> Failure(string message, string errorDetails = "", Exception exception = null)
        {
            return new OperationResult<T>
            {
                IsSuccess = false,
                Message = message,
                ErrorDetails = errorDetails,
                Exception = exception
            };
        }

        /// <summary>
        /// Implicit operator to check if operation was successful
        /// </summary>
        /// <param name="result">The operation result</param>
        public static implicit operator bool(OperationResult<T> result)
        {
            return result?.IsSuccess == true;
        }
    }

    /// <summary>
    /// Non-generic version of OperationResult for operations that don't return data
    /// </summary>
    public class OperationResult : OperationResult<object>
    {
        /// <summary>
        /// Creates a successful operation result without data
        /// </summary>
        /// <param name="message">Optional success message</param>
        /// <returns>A successful OperationResult</returns>
        public static OperationResult Success(string message = "")
        {
            return new OperationResult
            {
                IsSuccess = true,
                Message = message
            };
        }

        /// <summary>
        /// Creates a failed operation result without data
        /// </summary>
        /// <param name="message">User-friendly error message</param>
        /// <param name="errorDetails">Detailed error information</param>
        /// <param name="exception">Exception that caused the failure</param>
        /// <returns>A failed OperationResult</returns>
        public static new OperationResult Failure(string message, string errorDetails = "", Exception exception = null)
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = message,
                ErrorDetails = errorDetails,
                Exception = exception
            };
        }
    }
}