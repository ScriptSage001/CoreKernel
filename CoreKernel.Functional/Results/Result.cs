using System;

namespace CoreKernel.Functional.Results;
    
    /// <summary>
    /// Class for defining the Result type, representing the outcome of an operation.
    /// </summary>
    public class Result
    {
        #region Constructor
    
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the result represents a success.</param>
        /// <param name="error">The error associated with the result, if any.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the combination of <paramref name="isSuccess"/> and <paramref name="error"/> is invalid.
        /// </exception>
        protected Result(bool isSuccess, Error error)
        {
            if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            {
                throw new InvalidOperationException("Invalid Result Scenario.");
            }
    
            IsSuccess = isSuccess;
            Error = error;
        }
    
        #endregion
    
        #region Properties
    
        /// <summary>
        /// Gets a value indicating whether the result represents a success.
        /// </summary>
        public bool IsSuccess { get; }
    
        /// <summary>
        /// Gets a value indicating whether the result represents a failure.
        /// </summary>
        public bool IsFailure => !IsSuccess;
    
        /// <summary>
        /// Gets the error associated with the result, if any.
        /// </summary>
        public Error Error { get; }
    
        #endregion
    
        #region Methods
    
        /// <summary>
        /// Creates a success result.
        /// </summary>
        /// <returns>A new <see cref="Result"/> instance representing success.</returns>
        public static Result Success() => new(true, Error.None);
        
        /// <summary>
        /// Creates a success result with a value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to associate with the success result.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance representing success.</returns>
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
    
        /// <summary>
        /// Creates a failure result.
        /// </summary>
        /// <param name="error">The error to associate with the failure result.</param>
        /// <returns>A new <see cref="Result"/> instance representing failure.</returns>
        public static Result Failure(Error error) => new(false, error);
            
        /// <summary>
        /// Creates a failure result with a value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="error">The error to associate with the failure result.</param>
        /// <returns>A new <see cref="Result{TValue}"/> instance representing failure.</returns>
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    
        /// <summary>
        /// Creates a success result if the specified condition is true; otherwise, creates a failure result.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <returns>
        /// A success result if <paramref name="condition"/> is true; otherwise, a failure result with the <see cref="Error.ConditionNotMet"/> error.
        /// </returns>
        public static Result Create(bool condition) => condition ? Success() : Failure(Error.ConditionNotMet);
            
        /// <summary>
        /// Creates a success result with a value if the value is not null; otherwise, creates a failure result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value to evaluate.</param>
        /// <returns>
        /// A success result with the value if <paramref name="value"/> is not null; otherwise, a failure result with the <see cref="Error.NullValue"/> error.
        /// </returns>
        public static Result<TValue> Create<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    
        /// <summary>
        /// Executes the specified action if the result represents success.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>The current <see cref="Result"/> instance.</returns>
        public Result OnSuccess(Action action)
        {
            if (IsSuccess)
            {
                action();
            }
            return this;
        }
    
        /// <summary>
        /// Executes the specified action if the result represents failure.
        /// </summary>
        /// <param name="action">The action to execute, which receives the associated error.</param>
        /// <returns>The current <see cref="Result"/> instance.</returns>
        public Result OnFailure(Action<Error> action)
        {
            if (IsFailure)
            {
                action(Error);
            }
            return this;
        }
    
        /// <summary>
        /// Returns a string representation of the result.
        /// </summary>
        /// <returns>
        /// "Success" if the result represents success; otherwise, "Failure: {Error}" with the associated error.
        /// </returns>
        public override string ToString()
        {
            return IsSuccess ? "Success" : $"Failure: {Error}";
        }
    
        /// <summary>
        /// Implicitly converts an <see cref="Error"/> to a failure result.
        /// </summary>
        /// <param name="error">The error to convert.</param>
        public static implicit operator Result(Error error) => Failure(error);
    
        #endregion
    }