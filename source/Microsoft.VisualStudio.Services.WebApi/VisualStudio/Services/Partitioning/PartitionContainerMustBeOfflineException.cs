// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.PartitionContainerMustBeOfflineException
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Partitioning
{
  [Serializable]
  public class PartitionContainerMustBeOfflineException : VssServiceException
  {
    public PartitionContainerMustBeOfflineException(string message)
      : base(message)
    {
    }

    public PartitionContainerMustBeOfflineException(string message, Exception ex)
      : base(message, ex)
    {
    }

    public PartitionContainerMustBeOfflineException(Guid containerId)
      : base(PartitioningResources.PartitionContainerMustBeOfflineError((object) containerId))
    {
    }

    protected PartitionContainerMustBeOfflineException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
