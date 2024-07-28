// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.ExceptionDetailNoStackTrace
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  [DataContract(Name = "ExceptionDetail", Namespace = "http://schemas.datacontract.org/2004/07/System.ServiceModel")]
  internal sealed class ExceptionDetailNoStackTrace
  {
    public ExceptionDetailNoStackTrace(ExceptionDetailNoStackTrace detail)
      : this(detail, false)
    {
    }

    public ExceptionDetailNoStackTrace(
      ExceptionDetailNoStackTrace detail,
      bool includeInnerException)
    {
      this.HelpLink = string.Empty;
      this.Message = detail.Message;
      this.Type = detail.Type;
      this.StackTrace = string.Empty;
      this.InnerException = (ExceptionDetailNoStackTrace) null;
      if (!includeInnerException || detail.InnerException == null)
        return;
      this.InnerException = new ExceptionDetailNoStackTrace(detail.InnerException, includeInnerException);
    }

    public ExceptionDetailNoStackTrace(ExceptionDetail detail)
      : this(detail, false)
    {
    }

    public ExceptionDetailNoStackTrace(ExceptionDetail detail, bool includeInnerException)
    {
      this.HelpLink = string.Empty;
      this.Message = detail.Message;
      this.Type = detail.Type;
      this.StackTrace = string.Empty;
      this.InnerException = (ExceptionDetailNoStackTrace) null;
      if (!includeInnerException || detail.InnerException == null)
        return;
      this.InnerException = new ExceptionDetailNoStackTrace(detail.InnerException, includeInnerException);
    }

    public ExceptionDetailNoStackTrace(Exception exception)
      : this(exception, false)
    {
    }

    public ExceptionDetailNoStackTrace(Exception exception, bool includeInnerException)
    {
      this.HelpLink = exception.HelpLink;
      this.Message = exception.Message;
      this.Type = exception.GetType().ToString();
      this.StackTrace = string.Empty;
      this.InnerException = (ExceptionDetailNoStackTrace) null;
      if (!includeInnerException || exception.InnerException == null)
        return;
      this.InnerException = new ExceptionDetailNoStackTrace(exception.InnerException, includeInnerException);
    }

    [DataMember]
    public string HelpLink { get; private set; }

    [DataMember]
    public ExceptionDetailNoStackTrace InnerException { get; private set; }

    [DataMember]
    public string Message { get; private set; }

    [DataMember]
    public string Type { get; private set; }

    [DataMember]
    public string StackTrace { get; private set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(this.GetType().Name);
      stringBuilder.Append(": ");
      stringBuilder.Append(this.Type);
      stringBuilder.Append(": ");
      stringBuilder.Append(this.Message);
      if (this.InnerException != null)
      {
        stringBuilder.Append("--->");
        stringBuilder.AppendLine(this.InnerException.ToString());
        stringBuilder.Append(SRCore.EndOfInnerExceptionStackTrace);
      }
      stringBuilder.AppendLine();
      stringBuilder.Append(this.StackTrace);
      return stringBuilder.ToString();
    }
  }
}
