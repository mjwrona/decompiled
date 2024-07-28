// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ReleaseJobInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ReleaseJobInfo
  {
    public ReleaseJobInfo(
      Guid jobSource,
      Guid jobId,
      TeamFoundationJobResult jobResult,
      string resultMessage,
      bool jobDefinitionExists,
      JobPriorityClass priorityClass,
      int scheduleSeconds,
      int ignoreDormancySeconds,
      bool isTemplateJob)
    {
      this.JobSource = jobSource;
      this.JobId = jobId;
      this.JobResult = jobResult;
      this.ResultMessage = resultMessage;
      this.JobDefinitionExists = jobDefinitionExists;
      this.PriorityClass = priorityClass;
      this.ScheduleSeconds = scheduleSeconds;
      this.IgnoreDormancySeconds = ignoreDormancySeconds;
      this.IsTemplateJob = isTemplateJob;
    }

    public Guid JobSource { get; }

    public Guid JobId { get; }

    public TeamFoundationJobResult JobResult { get; }

    public string ResultMessage { get; }

    public bool JobDefinitionExists { get; }

    public JobPriorityClass PriorityClass { get; }

    public int ScheduleSeconds { get; }

    public int IgnoreDormancySeconds { get; }

    public bool IsTemplateJob { get; }
  }
}
