// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentJobStillRunningException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ExceptionMapping("0.0", "3.0", "TaskAgentJobStillRunningException", "Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentJobStillRunningException, Microsoft.TeamFoundation.DistributedTask.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class TaskAgentJobStillRunningException : DistributedTaskException
  {
    public TaskAgentJobStillRunningException(string message)
      : base(message)
    {
    }

    public TaskAgentJobStillRunningException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private TaskAgentJobStillRunningException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
