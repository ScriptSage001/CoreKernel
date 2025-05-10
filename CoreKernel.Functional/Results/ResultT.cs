namespace CoreKernel.Functional.Results;
        
        /// <summary>
        /// Class for defining the generic Result type, representing the outcome of an operation with a value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value associated with the result.</typeparam>
        public class Result<TValue> : Results.Result
        {
            private readonly TValue? _value;
        
            /// <summary>
            /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
            /// </summary>
            /// <param name="value">The value associated with the result.</param>
            /// <param name="isSuccess">Indicates whether the result represents a success.</param>
            /// <param name="error">The error associated with the result, if any.</param>
            protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
            {
                _value = value;
            }
        
            /// <summary>
            /// Gets the value associated with the result.
            /// </summary>
            /// <exception cref="InvalidOperationException">Thrown if the result represents a failure.</exception>
            public TValue Value => IsSuccess
                ? _value!
                : throw new InvalidOperationException("The Value of the Failure Result cannot be accessed.");
            
            /// <summary>
            /// Implicitly converts a value of type <typeparamref name="TValue"/> to a <see cref="Result{TValue}"/> instance.
            /// </summary>
            /// <param name="value">The value to convert.</param>
            public static implicit operator Result<TValue>(TValue? value) => Create(value);
        
            /// <summary>
            /// Executes the specified action if the result represents success.
            /// </summary>
            /// <param name="action">The action to execute, which receives the associated value.</param>
            /// <returns>The current <see cref="Result{TValue}"/> instance.</returns>
            public Result<TValue> OnSuccess(Action<TValue> action)
            {
                if (IsSuccess)
                {
                    action(Value);
                }
                return this;
            }
        
            /// <summary>
            /// Executes the specified action if the result represents failure.
            /// </summary>
            /// <param name="action">The action to execute, which receives the associated error.</param>
            /// <returns>The current <see cref="Result{TValue}"/> instance.</returns>
            public new Result<TValue> OnFailure(Action<Error> action)
            {
                if (IsFailure)
                {
                    action(Error);
                }
                return this;
            }
        
            /// <summary>
            /// Transforms the value associated with the result using the specified mapping function.
            /// </summary>
            /// <typeparam name="TResult">The type of the transformed value.</typeparam>
            /// <param name="mapper">The mapping function to apply.</param>
            /// <returns>
            /// A new <see cref="Result{TResult}"/> instance containing the transformed value if the result represents success;
            /// otherwise, a failure result with the same error.
            /// </returns>
            public Result<TResult> Map<TResult>(Func<TValue, TResult> mapper)
            {
                return IsSuccess ? Success(mapper(Value)) : Failure<TResult>(Error);
            }
        
            /// <summary>
            /// Transforms the result using the specified binding function.
            /// </summary>
            /// <typeparam name="TResult">The type of the transformed result.</typeparam>
            /// <param name="binder">The binding function to apply.</param>
            /// <returns>
            /// A new <see cref="Result{TResult}"/> instance returned by the binding function if the result represents success;
            /// otherwise, a failure result with the same error.
            /// </returns>
            public Result<TResult> Bind<TResult>(Func<TValue, Result<TResult>> binder)
            {
                return IsSuccess ? binder(Value) : Failure<TResult>(Error);
            }
        
            /// <summary>
            /// Returns a string representation of the result.
            /// </summary>
            /// <returns>
            /// "Success: {Value}" if the result represents success; otherwise, "Failure: {Error}" with the associated error.
            /// </returns>
            public override string ToString()
            {
                return IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
            }
        }