// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchServiceException
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [Serializable]
  public class SearchServiceException : Exception
  {
    private readonly bool m_exceptionUserMessageExists;

    public SearchServiceException()
    {
    }

    public SearchServiceException(string message)
      : this(message, (Exception) null)
    {
    }

    public SearchServiceException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.m_exceptionUserMessageExists = !string.IsNullOrWhiteSpace(message);
    }

    public SearchServiceException(string message, Exception innerException, string exceptionSource)
      : base(message, innerException)
    {
      this.m_exceptionUserMessageExists = !string.IsNullOrWhiteSpace(message);
      this.ExceptionSource = exceptionSource;
    }

    public SearchServiceException(
      string message,
      Exception innerException,
      string exceptionSource,
      SearchServiceErrorCode errorCode)
      : base(message, innerException)
    {
      this.m_exceptionUserMessageExists = !string.IsNullOrWhiteSpace(message);
      this.ExceptionSource = exceptionSource;
      this.ErrorCode = errorCode;
    }

    public SearchServiceException(
      string message,
      Exception innerException,
      SearchServiceErrorCode errorCode)
      : base(message, innerException)
    {
      this.m_exceptionUserMessageExists = !string.IsNullOrWhiteSpace(message);
      this.ErrorCode = errorCode;
    }

    public SearchServiceException(string message, SearchServiceErrorCode errorCode)
      : this(message, (Exception) null, errorCode)
    {
    }

    protected SearchServiceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.m_exceptionUserMessageExists = (bool) info.GetValue("ExceptionUserMessageExists", typeof (bool));
      this.ExceptionSource = (string) info.GetValue(nameof (ExceptionSource), typeof (string));
      this.Context = (string) info.GetValue(nameof (Context), typeof (string));
    }

    public string ExceptionSource { get; set; }

    public string BasicMessage => this.m_exceptionUserMessageExists ? base.Message : this.DefaultMessage;

    public override string Message
    {
      get
      {
        string message = this.BasicMessage;
        if (!string.IsNullOrEmpty(this.Context))
          message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, ExceptionMessage.AdditionalDetails, (object) message, (object) this.Context);
        return message;
      }
    }

    public SearchServiceErrorCode ErrorCode { get; set; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ExceptionUserMessageExists", this.m_exceptionUserMessageExists);
      info.AddValue("ExceptionSource", (object) this.ExceptionSource);
      info.AddValue("Context", (object) this.Context);
    }

    protected virtual string DefaultMessage => ExceptionMessage.SearchServiceException;

    internal string Context { get; set; }
  }
}
