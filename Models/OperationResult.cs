using System;

namespace MidsApp.Models
{
    /// <summary>
    /// Defines a standard interface for operation results, providing a mechanism to capture the outcome of an operation.
    /// </summary>
    public interface IOperationResult
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        /// <value>
        /// True if the operation was successful; otherwise, false.
        /// </value>
        bool IsSuccessful { get; }

        /// <summary>
        /// Gets the message describing the operation's outcome. This message can provide additional details about the result,
        /// including error messages or success confirmations.
        /// </summary>
        /// <value>
        /// The message describing the operation's outcome.
        /// </value>
        string Message { get; }
    }

    /// <summary>
    /// Defines a standard interface for operation results that include data of a specified type.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the operation result if successful.</typeparam>
    public interface IOperationResult<T> : IOperationResult
    {
        /// <summary>
        /// Gets the data associated with a successful operation.
        /// </summary>
        /// <value>
        /// The data of type <typeparamref name="T"/>, which is returned if the operation is successful.
        /// If the operation is unsuccessful, the default value for type <typeparamref name="T"/> is returned.
        /// </value>
        T Data { get; }
    }

    /// <summary>
    /// Represents the outcome of an operation without data.
    /// </summary>
    public class OperationResult : IOperationResult
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Gets a message describing the operation's outcome.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="isSuccessful">Indicates whether the operation was successful.</param>
        /// <param name="message">The message describing the outcome of the operation.</param>
        protected OperationResult(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }

        /// <summary>
        /// Creates a success result with a descriptive message.
        /// </summary>
        /// <param name="message">The message describing the success.</param>
        /// <returns>An operation result indicating success.</returns>
        public static OperationResult Success(string message)
        {
            return new OperationResult(true, message);
        }

        /// <summary>
        /// Creates a failure result with a descriptive message.
        /// </summary>
        /// <param name="message">The message describing the failure.</param>
        /// <returns>An operation result indicating failure.</returns>
        public static OperationResult Failure(string message)
        {
            return new OperationResult(false, message);
        }
    }

    /// <summary>
    /// Represents the outcome of an operation with associated data.
    /// </summary>
    /// <typeparam name="T">The type of data returned in the operation result if successful.</typeparam>
    public class OperationResult<T> : OperationResult, IOperationResult<T>
    {
        /// <summary>
        /// Gets the data associated with the successful operation.
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{T}"/> class.
        /// </summary>
        /// <param name="isSuccessful">Indicates whether the operation was successful.</param>
        /// <param name="message">The message describing the outcome of the operation.</param>
        /// <param name="data">The data associated with the operation's success.</param>
        private OperationResult(bool isSuccessful, string message, T data) : base(isSuccessful, message)
        {
            if (isSuccessful && data == null) throw new ArgumentNullException(nameof(data), "Data must not be null for successful results.");
            Data = data;
        }

        /// <summary>
        /// Creates a success result with data and a descriptive message.
        /// </summary>
        /// <param name="message">The message describing the success.</param>
        /// <param name="data">The data associated with the success.</param>
        /// <returns>An operation result indicating success with data.</returns>
        public static OperationResult<T> Success(string message, T data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data), "Data provided for a successful result cannot be null.");
            return new OperationResult<T>(true, message, data);
        }
    }
}
