// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DRCleanupJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DRCleanupJob : VssAsyncJobExtension
  {
    protected readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const string LastSyncNodeName = "LastSyncTime";
    private const string StartingBlobIdNodeName = "StartingBlobId";
    private const string SkipDeletionNodeName = "SkipDeletion";

    public override async Task<VssJobResult> RunAsync(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime)
    {
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, this.traceData, 5701220, nameof (RunAsync)))
      {
        try
        {
          XmlNode xmlNode1 = jobDefinition.Data.SelectSingleNode("LastSyncTime");
          XmlNode xmlNode2 = jobDefinition.Data.SelectSingleNode("StartingBlobId");
          XmlNode xmlNode3 = jobDefinition.Data.SelectSingleNode("SkipDeletion");
          DateTime result;
          if (!DateTime.TryParse(xmlNode1?.FirstChild.Value, out result))
            throw new ArgumentException("LastSyncTime data is not provided properly!");
          BlobIdentifier startingBlobId = (BlobIdentifier) null;
          if (xmlNode2 != null)
            startingBlobId = BlobIdentifier.Deserialize(xmlNode2.FirstChild.Value);
          bool skipDeletion = false;
          if (xmlNode3 != null)
            skipDeletion = bool.Parse(xmlNode3.FirstChild.Value);
          FixMetadataAfterDisasterResults dataContractObject = await requestContext.GetService<AdminPlatformBlobStore>().FixMetadataAfterDisasterAsync(requestContext, WellKnownDomainIds.DefaultDomainId, result, startingBlobId, skipDeletion).ConfigureAwait(true);
          return dataContractObject.FailedToRemoveMetadata > 0L || dataContractObject.ExceptionsThrown > 0L ? new VssJobResult(TeamFoundationJobExecutionResult.PartiallySucceeded, JsonSerializer.Serialize<FixMetadataAfterDisasterResults>(dataContractObject)) : new VssJobResult(TeamFoundationJobExecutionResult.Succeeded, JsonSerializer.Serialize<FixMetadataAfterDisasterResults>(dataContractObject));
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          return new VssJobResult(TeamFoundationJobExecutionResult.Failed, string.Format("Error: {0}", (object) ex));
        }
      }
    }
  }
}
