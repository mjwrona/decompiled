// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.OperationPluginWithSameIdException
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
  public class OperationPluginWithSameIdException : VssServiceException
  {
    public OperationPluginWithSameIdException()
    {
    }

    public OperationPluginWithSameIdException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public OperationPluginWithSameIdException(string message)
      : base(message)
    {
    }

    public OperationPluginWithSameIdException(Guid pluginId)
      : base(WebApiResources.OperationPluginWithSameIdException((object) pluginId))
    {
    }

    protected OperationPluginWithSameIdException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
