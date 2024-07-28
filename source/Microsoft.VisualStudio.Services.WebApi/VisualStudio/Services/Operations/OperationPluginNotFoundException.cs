// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.OperationPluginNotFoundException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Operations
{
  [Serializable]
  public class OperationPluginNotFoundException : VssServiceException
  {
    public OperationPluginNotFoundException()
    {
    }

    public OperationPluginNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public OperationPluginNotFoundException(string message)
      : base(message)
    {
    }

    public OperationPluginNotFoundException(Guid pluginId)
      : base(WebApiResources.OperationPluginNotFoundException((object) pluginId))
    {
    }

    protected OperationPluginNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
