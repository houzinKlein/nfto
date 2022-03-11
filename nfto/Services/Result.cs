namespace Nfto.Services
{
    public class Result:Result<Object>
    {
        public static Result BuildSuccessResult()
        {
            return new Result()
            {
                Success = true
            };
        }

        public static new Result BuildErrorResult(string error)
        {
            return new Result()
            {
                Success = false,
                Error = error
            };
        }

    }
    /// <summary>
    /// Get the result of the run
    /// </summary>
    public class Result<T>  where T : class
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public T Model { get; set; }

        public static Result<T> BuildErrorResult(string error)
        {
            return new Result<T>()
            {
                Success = false,
                Error = error
            };
        }

        public static  Result<T> BuildSuccessResult(T model)
        {
            return new Result<T>()
            {
                Model = model,
                Success = true
            };
        }
    }
}
