// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationBlockedException
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [Serializable]
  public class ServicingOrchestrationBlockedException : ServicingOrchestrationException
  {
    public DateTime BlockingSince { get; private set; }

    public ServicingOrchestrationBlockedException(string message)
      : base(message)
    {
      this.MarkAsBlockedServicingOrchestrationException();
    }

    public ServicingOrchestrationBlockedException(string message, Exception innerException)
      : base(message, innerException)
    {
      this.MarkAsBlockedServicingOrchestrationException();
    }

    public ServicingOrchestrationBlockedException(
      DateTime blockingSince,
      string message,
      Exception innerException = null)
      : this(message, innerException)
    {
      this.BlockingSince = blockingSince;
    }

    protected ServicingOrchestrationBlockedException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
      this.MarkAsBlockedServicingOrchestrationException();
    }
  }
}
