// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Monads.ExceptionWithStackTraceException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Monads
{
  internal sealed class ExceptionWithStackTraceException : Exception
  {
    private static readonly string EndOfInnerExceptionString = "--- End of inner exception stack trace ---";
    private readonly System.Diagnostics.StackTrace stackTrace;

    public ExceptionWithStackTraceException(System.Diagnostics.StackTrace stackTrace)
      : this((string) null, (Exception) null, stackTrace)
    {
    }

    public ExceptionWithStackTraceException(string message, System.Diagnostics.StackTrace stackTrace)
      : this(message, (Exception) null, stackTrace)
    {
    }

    public ExceptionWithStackTraceException(
      string message,
      Exception innerException,
      System.Diagnostics.StackTrace stackTrace)
      : base(message, innerException)
    {
      this.stackTrace = stackTrace != null ? stackTrace : throw new ArgumentNullException(nameof (stackTrace));
    }

    public override string StackTrace => this.stackTrace.ToString();

    public override string ToString()
    {
      string str = this.Message == null || this.Message.Length <= 0 ? this.GetClassName() : this.GetClassName() + ": " + this.Message;
      if (this.InnerException != null)
        str = str + " ---> " + this.InnerException.ToString() + Environment.NewLine + "   " + ExceptionWithStackTraceException.EndOfInnerExceptionString;
      return str + Environment.NewLine + this.StackTrace;
    }

    private string GetClassName() => this.GetType().ToString();

    public static Exception UnWrapMonadExcepion(Exception exception, ITrace trace)
    {
      switch (exception)
      {
        case ExceptionWithStackTraceException stackTraceException:
          return ExceptionWithStackTraceException.UnWrapMonadExcepion(stackTraceException.InnerException, trace);
        case OperationCanceledException originalException:
          return (Exception) new CosmosOperationCanceledException(originalException, trace);
        default:
          return exception;
      }
    }
  }
}
