// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskHubExistsException
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [Serializable]
  public class TaskHubExistsException : TeamFoundationServiceException
  {
    public TaskHubExistsException(string message)
      : base(message)
    {
    }

    public TaskHubExistsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected TaskHubExistsException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
