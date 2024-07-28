// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentGroupNotFoundException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [Serializable]
  public sealed class DeploymentGroupNotFoundException : DistributedTaskException
  {
    public DeploymentGroupNotFoundException(string message)
      : base(message)
    {
    }

    public DeploymentGroupNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private DeploymentGroupNotFoundException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
