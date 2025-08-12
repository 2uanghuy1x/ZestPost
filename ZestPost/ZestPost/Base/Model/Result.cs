namespace ZestPost.Base
{
    public class Result
    {
        public string Code { get; set; }

        public string Message { get; set; }

        public string ErrorContent { get; set; }

        public static Result OK = new Result();

        public const string ERROR_CODE = "98";

        public Result(string code, string message, string errorContent = null)
        {
            Code = code;

            Message = message;
            ErrorContent = errorContent;
        }

        public Result()
        {
            Code = "00";
            Message = string.Empty;
            ErrorContent = string.Empty;

        }

        public bool IsOk()
        {
            if (Code == "00")
            {
                return true;
            }

            return Code == "0" || Code == "200";
        }

        public bool IsError()
        {
            return !IsOk();
        }

        public bool IsException()
        {
            return Code == "99";
        }

        public bool IsValidate()
        {
            return Code == "100";
        }

        public static Result Ok()
        {
            return new Result("00", "Ok");
        }

        public static Result Error(string code, string message)
        {
            return new Result(code, message);
        }

        public static Result Error(string message)
        {
            return new Result("98", message);
        }

        public static Result ErrorWithContent(string message, string errrorContent)
        {
            return new Result("98", message, errrorContent);
        }

        public static Result Exception(string message, Exception ex)
        {
            return new Result("99", message, ex.ToString());
        }

        public static Result<TData> Ok<TData>(TData data, string message = null)
        {
            return new Result<TData>("00", message, data);
        }

        public static Result<TData> Error<TData>(string code, string message, TData data)
        {
            return new Result<TData>(code, message, data);
        }

        public static Result<TData> Error<TData>(string code, string message)
        {
            return new Result<TData>(code, message);
        }

        public static Result<TData> Error<TData>(string message)
        {
            return new Result<TData>("98", message);
        }

        public static Result<TData> ErrorWithContent<TData>(string message, string errorContent)
        {
            return new Result<TData>("98", message, errorContent);
        }

        public static Result<TData> ErrorWithData<TData>(string message, TData data)
        {
            return new Result<TData>("98", message, data);
        }

        public static Result<TData> ErrorData<TData>(string message, TData data)
        {
            string txtTrans = message;
            return new Result<TData>("98", txtTrans, data);
        }

        public static Result<TData> Exception<TData>(string message, Exception ex)
        {
            return new Result<TData>("99", message, ex.ToString());
        }
    }

    public class Result<TData> : Result
    {
        public TData Data { get; set; }

        public Result(Result result, TData data)
        {
            Data = data;
            Code = result.Code;
            Message = result.Message;
            ErrorContent = result.ErrorContent;

        }

        public Result(string code, string message, TData data = default(TData))
            : base(code, message)
        {
            Data = data;
        }

        public Result(string code, string message, string errorContent)
            : base(code, message, errorContent)
        {
        }

        public Result()
        {
        }
    }
}
