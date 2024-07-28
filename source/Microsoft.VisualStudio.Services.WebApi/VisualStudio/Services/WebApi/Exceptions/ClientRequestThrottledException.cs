// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Exceptions.ClientRequestThrottledException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.VisualStudio.Services.WebApi.Exceptions
{
  [Serializable]
  public class ClientRequestThrottledException : TooManyRequestsException
  {
    public ClientRequestThrottledException(string message, DateTime retryAfterDateTime)
      : base(message, retryAfterDateTime)
    {
    }

    public ClientRequestThrottledException(
      string message,
      DateTime retryAfterDateTime,
      Exception innerException)
      : base(message, retryAfterDateTime, innerException)
    {
    }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    protected ClientRequestThrottledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
