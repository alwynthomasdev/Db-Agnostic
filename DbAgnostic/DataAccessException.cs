using System;

namespace DbAgnostic
{
    public class DbAccessException : Exception
    {
        public string Statement
        {
            get
            {
                return Data["Statement"].ToString();
            }
        }
        public object Parameters
        {
            get
            {
                return Data["Parameters"];
            }
        }
        public DbAccessException(string message) : base(message) { }
        public DbAccessException(string message, Exception ex) : base(message, ex) { }
        public DbAccessException(string message, string statement, object @prams = null) : base(message) { Data["Statement"] = statement; Data["Parameters"] = @prams; }
        public DbAccessException(string message, Exception ex, string statement, object @prams = null) : base(message, ex) { Data["Statement"] = statement; Data["Parameters"] = @prams; }
    }
}
