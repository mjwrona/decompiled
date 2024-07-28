// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.StreamLoggerException
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [Serializable]
  public class StreamLoggerException : SearchServiceException
  {
    public StreamLoggerException()
      : this(SearchServiceErrorCode.UnexpectedError, (string) null)
    {
    }

    public StreamLoggerException(string message)
      : base(message)
    {
    }

    public StreamLoggerException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public StreamLoggerException(SearchServiceErrorCode errorCode, string message)
      : this(errorCode, message, (Exception) null)
    {
    }

    public StreamLoggerException(
      SearchServiceErrorCode errorCode,
      string message,
      Exception innerException)
      : this(errorCode, message, innerException, (string) null)
    {
    }

    public StreamLoggerException(
      SearchServiceErrorCode errorCode,
      string message,
      Exception innerException,
      string exceptionSource)
      : base(message, innerException, exceptionSource, errorCode)
    {
    }

    protected StreamLoggerException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ErrorCode = (SearchServiceErrorCode) info.GetValue("ErrorCode", typeof (SearchServiceErrorCode));
    }

    protected override string DefaultMessage => ExceptionMessage.StreamLoggerException;

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Error code -----> {0}", (object) this.ErrorCode));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception message -----> {0}", (object) this.Message));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception type -----> {0}", (object) this.GetType()));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception source -----> {0}", (object) this.Source));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Stack trace,\r\n{0}", (object) this.StackTrace.Indent(3)));
      if (this.InnerException != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "INNER EXCEPTION,\r\n{0}", (object) this.InnerException.ToString().Indent(6)));
      }
      return stringBuilder.ToString();
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ErrorCode", (object) this.ErrorCode);
    }
  }
}
