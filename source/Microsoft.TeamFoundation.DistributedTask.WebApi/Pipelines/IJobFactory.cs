// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.IJobFactory
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal interface IJobFactory
  {
    string Name { get; }

    Job CreateJob(
      JobExecutionContext context,
      ExpressionValue<string> container,
      IDictionary<string, ExpressionValue<string>> sidecarContainers,
      bool continueOnError,
      int timeoutInMinutes,
      int cancelTimeoutInMinutes,
      string displayName = null);
  }
}
