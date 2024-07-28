// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanCanceledException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [ExceptionMapping("0.0", "3.0", "TaskOrchestrationPlanCanceledException", "Microsoft.TeamFoundation.DistributedTask.WebApi.TaskOrchestrationPlanCanceledException, Microsoft.TeamFoundation.DistributedTask.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public sealed class TaskOrchestrationPlanCanceledException : DistributedTaskException
  {
    public TaskOrchestrationPlanCanceledException(string message)
      : base(message)
    {
    }

    public TaskOrchestrationPlanCanceledException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private TaskOrchestrationPlanCanceledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
