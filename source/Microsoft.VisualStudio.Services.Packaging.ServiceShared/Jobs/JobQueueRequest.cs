// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.JobQueueRequest
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs
{
  public class JobQueueRequest
  {
    public JobQueueRequest(JobId jobId) => this.JobId = jobId;

    public JobId JobId { get; }

    public JobQueuingOptions Options { get; private set; }

    public TimeSpan QueueDelay { get; private set; }

    public static JobQueueRequest QueueAfter(JobId jobId, TimeSpan delay) => new JobQueueRequest(jobId)
    {
      Options = JobQueuingOptions.QueueAfter,
      QueueDelay = delay
    };
  }
}
