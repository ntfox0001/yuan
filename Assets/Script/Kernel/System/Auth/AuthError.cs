using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthError : IError
{
    public long ErrorCode { get; protected set; }
    public long OriginalErrorCode { get; protected set; }
    public string Message { get; protected set; }

    public AuthError(long err, long oriErr, string msg)
    {
        ErrorCode = err;
        OriginalErrorCode = oriErr;
        Message = msg;
    }
}
