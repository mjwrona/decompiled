// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Copy.DedupContentCopyJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Copy
{
  public class DedupContentCopyJob : VssAsyncJobExtension
  {
    public const string JobName = "Dedup content copy job";
    public const string JobExtensionName = "Microsoft.VisualStudio.Services.BlobStore.Server.Copy.DedupContentCopyJob";

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      DedupContentCopyRequest copyRequest = JsonConvert.DeserializeObject<DedupContentCopyRequest>(JsonConvert.SerializeXmlNode(jobDefinition.Data, Formatting.Indented, true));
      try
      {
        string dedupIdsStr = string.Join<DedupIdentifier>(",", (IEnumerable<DedupIdentifier>) copyRequest.DedupIds);
        requestContext.TraceAlways(5701920, TraceLevel.Info, "BlobStore", "Copy", string.Format("Copying {0} from ({1}, {2})", (object) dedupIdsStr, (object) copyRequest.CopyFromHostId, (object) copyRequest.CopyFromDomainId) + string.Format(" to ({0}, {1})", (object) requestContext.ServiceHost.InstanceId, (object) copyRequest.CopyToDomainId));
        Dictionary<DedupIdentifier, KeepUntilReceipt> dictionary = await DedupContentCopier.Create(requestContext, copyRequest.CopyFromHostId, copyRequest.CopyFromDomainId, copyRequest.CopyToDomainId).CopyAsync(requestContext, copyRequest.DedupIds, requestContext.CancellationToken);
        return new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, string.Format("Copied {0} successfully from ({1}, {2})", (object) dedupIdsStr, (object) copyRequest.CopyFromHostId, (object) copyRequest.CopyFromDomainId) + string.Format(" to ({0}, {1}).", (object) requestContext.ServiceHost.InstanceId, (object) copyRequest.CopyToDomainId));
      }
      catch (Exception ex)
      {
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, ex.ToString());
      }
    }
  }
}
