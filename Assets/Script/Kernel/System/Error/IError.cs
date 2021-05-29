using System;
public interface IError
{
    long ErrorCode { get; }
    string Message { get;  }
}