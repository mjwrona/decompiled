// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming.MultiFeedRefreshListTrimmingJobQueuer
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Jobs.RefreshListTrimming
{
  public class MultiFeedRefreshListTrimmingJobQueuer : IMultiFeedRefreshListTrimmingJobQueuer
  {
    private readonly IJobQueuer jobService;
    private readonly ITracerService tracer;
    private readonly string jobExtensionFullName;
    private readonly string jobName;

    public MultiFeedRefreshListTrimmingJobQueuer(
      IJobQueuer jobServiceFacade,
      ITracerService tracer,
      string jobExtensionFullName,
      string jobName)
    {
      this.jobService = jobServiceFacade;
      this.tracer = tracer;
      this.jobExtensionFullName = jobExtensionFullName;
      this.jobName = jobName;
    }

    public void QueueMultiFeedJob(IEnumerable<Guid> feedsToPrune, TimeSpan delay)
    {
      using (ITracerBlock tracerBlock = this.tracer.Enter((object) this, nameof (QueueMultiFeedJob)))
      {
        RefreshListTrimmingJobData objectToSerialize = new RefreshListTrimmingJobData(feedsToPrune.ToList<Guid>());
        tracerBlock.TraceInfo(string.Format("Queue Job {0} with delay {1} for {2} feeds", (object) this.jobExtensionFullName, (object) delay, (object) objectToSerialize.FeedIdsToPrune.Count));
        this.jobService.QueueOneTimeJob(this.jobName, this.jobExtensionFullName, TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize), maxDelay: delay);
      }
    }
  }
}
