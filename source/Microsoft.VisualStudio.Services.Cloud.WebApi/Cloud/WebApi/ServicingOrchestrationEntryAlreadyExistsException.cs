// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationEntryAlreadyExistsException
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [ExceptionMapping("0.0", "3.0", "ServicingOrchestrationEntryAlreadyExistsException", "Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationEntryAlreadyExistsException, Microsoft.VisualStudio.Services.Cloud.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class ServicingOrchestrationEntryAlreadyExistsException : ServicingOrchestrationException
  {
    public ServicingOrchestrationEntryAlreadyExistsException(Guid requestId)
      : base(ServicingOrchestrationResources.ServicingOrchestrationAlreadyExistsException((object) requestId))
    {
    }

    public ServicingOrchestrationEntryAlreadyExistsException(
      string message,
      Exception innerException)
      : base(message, innerException)
    {
    }

    protected ServicingOrchestrationEntryAlreadyExistsException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
