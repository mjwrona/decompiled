// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupTreeRestoreJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DedupTreeRestoreJob : VssAsyncJobExtension
  {
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const int Tracepoint = 5701996;
    private readonly string maxParallelismRegistryPath = "/Configuration/BlobStore/DedupTreeRestoreJob";

    private void RestoreDedupIdList(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupTreeRestoreJobParameter jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      AdminDedupStoreService dedupService = requestContext.GetService<AdminDedupStoreService>();
      requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) this.maxParallelismRegistryPath, true, 1);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      IEnumerable<NodeDedupIdentifier> dedupIdList = ((IEnumerable<string>) jobParameters.DedupIdList).Select<string, NodeDedupIdentifier>(DedupTreeRestoreJob.\u003C\u003EO.\u003C0\u003E__Parse ?? (DedupTreeRestoreJob.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, NodeDedupIdentifier>(NodeDedupIdentifier.Parse)));
      requestContext.Pump((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
      {
        foreach (NodeDedupIdentifier dedupId in dedupIdList)
        {
          long num = await dedupService.RestoreDedupTree(processor, (DedupIdentifier) dedupId, tracer).ConfigureAwait(false);
        }
      }));
    }

    private VssJobResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5701996, nameof (Run)))
      {
        if (jobDefinition.Data == null || !jobDefinition.Data.HasChildNodes)
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "No JobData provided");
        DedupTreeRestoreJobParameter jobParameters = TeamFoundationSerializationUtility.Deserialize<DedupTreeRestoreJobParameter>(jobDefinition.Data);
        if (jobParameters.DedupIdList != null)
        {
          if (((IEnumerable<string>) jobParameters.DedupIdList).Any<string>())
          {
            try
            {
              this.RestoreDedupIdList(requestContext, WellKnownDomainIds.DefaultDomainId, jobParameters, tracer);
              return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, "Restored dedup tree");
            }
            catch (Exception ex)
            {
              string exceptionMessage = JobHelper.GetNestedExceptionMessage(ex);
              return new VssJobResult(TeamFoundationJobExecutionResult.Failed, string.Format("Restoring dedup for '{0}' failed with {1}", (object) jobParameters.DedupIdList, (object) exceptionMessage));
            }
          }
        }
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "No DedupId provided for the Job");
      }
    }

    public override Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      return Task.FromResult<VssJobResult>(this.Run(requestContext, jobDefinition));
    }
  }
}
