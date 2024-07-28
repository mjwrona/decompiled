// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Exceptions.TooManyRequestsException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.VisualStudio.Services.WebApi.Exceptions
{
  [Serializable]
  public class TooManyRequestsException : VssServiceException
  {
    public DateTime RetryAfterDateTime { get; set; }

    public TooManyRequestsException(string message, DateTime retryAfterDateTime)
      : base(message)
    {
      this.RetryAfterDateTime = retryAfterDateTime;
    }

    public TooManyRequestsException(
      string message,
      DateTime retryAfterDateTime,
      Exception innerException)
      : base(message, innerException)
    {
      this.RetryAfterDateTime = retryAfterDateTime;
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    protected TooManyRequestsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.RetryAfterDateTime = info.GetDateTime(nameof (RetryAfterDateTime));
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      info.AddValue("RetryAfterDateTime", this.RetryAfterDateTime);
      base.GetObjectData(info, context);
    }
  }
}
