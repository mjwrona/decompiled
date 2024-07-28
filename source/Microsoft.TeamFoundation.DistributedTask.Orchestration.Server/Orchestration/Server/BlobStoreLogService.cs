// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.BlobStoreLogService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public class BlobStoreLogService : IBlobStoreLogService, IVssFrameworkService
  {
    public const string BuildLogsScopeName = "buildlogs";

    public async Task<BlobIdentifier> AddLogReferenceAsync(
      IVssRequestContext requestContext,
      Guid planId,
      int logId,
      int pageId,
      string serializedBlobId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      BlobReference blobReference = new BlobReference(this.GetBlobReference(planId, logId, pageId));
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdentifier = Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks.Deserialize(serializedBlobId);
      Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> dictionary1 = new Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>();
      Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks key = blobIdentifier;
      dictionary1[key] = (IEnumerable<BlobReference>) new BlobReference[1]
      {
        blobReference
      };
      Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> referencesGroupedByBlobIds = dictionary1;
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> dictionary2 = await vssRequestContext.GetService<IBlobStore>().TryReferenceWithBlocksAsync(vssRequestContext, WellKnownDomainIds.DefaultDomainId, (IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>) referencesGroupedByBlobIds);
      BlobIdentifier blobId = blobIdentifier.BlobId;
      blobIdentifier = (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null;
      return blobId;
    }

    public async Task<Stream> GetLogStreamAsync(
      IVssRequestContext requestContext,
      string blobFileId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      BlobIdentifier blobId = BlobIdentifier.Deserialize(blobFileId);
      return await vssRequestContext.GetService<IBlobStore>().GetBlobAsync(vssRequestContext, WellKnownDomainIds.DefaultDomainId, blobId);
    }

    public async Task DeleteLogReferencesAsync(
      IVssRequestContext requestContext,
      IEnumerable<LogBlobIdentifier> logBlobs)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      Dictionary<BlobIdentifier, IList<IdBlobReference>> source = new Dictionary<BlobIdentifier, IList<IdBlobReference>>();
      foreach (LogBlobIdentifier logBlob in logBlobs)
      {
        BlobIdentifier key = BlobIdentifier.Deserialize(logBlob.BlobId);
        IList<IdBlobReference> idBlobReferenceList;
        if (!source.TryGetValue(key, out idBlobReferenceList))
        {
          idBlobReferenceList = (IList<IdBlobReference>) new List<IdBlobReference>();
          source.Add(key, idBlobReferenceList);
        }
        idBlobReferenceList.Add(this.GetBlobReference(logBlob.PlanId, logBlob.LogId, logBlob.PageId));
      }
      await vssRequestContext.GetService<IBlobStore>().RemoveReferencesAsync(vssRequestContext, WellKnownDomainIds.DefaultDomainId, (IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>>) source.ToDictionary<KeyValuePair<BlobIdentifier, IList<IdBlobReference>>, BlobIdentifier, IEnumerable<IdBlobReference>>((Func<KeyValuePair<BlobIdentifier, IList<IdBlobReference>>, BlobIdentifier>) (k => k.Key), (Func<KeyValuePair<BlobIdentifier, IList<IdBlobReference>>, IEnumerable<IdBlobReference>>) (v => v.Value.AsEnumerable<IdBlobReference>())));
    }

    public async Task<string> UploadLogAsync(
      IVssRequestContext requestContext,
      Guid planId,
      int logId,
      int pageId,
      Stream stream)
    {
      IVssRequestContext context = requestContext.Elevate();
      IBlobStore service = context.GetService<IBlobStore>();
      BlobIdentifier blobId = stream.CalculateBlobIdentifierWithBlocks((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance).BlobId;
      BlobReference blobReference = new BlobReference(this.GetBlobReference(planId, logId, pageId));
      IVssRequestContext requestContext1 = context;
      IDomainId defaultDomainId = WellKnownDomainIds.DefaultDomainId;
      BlobIdentifier blobId1 = blobId;
      Stream blob = stream;
      BlobReference reference = blobReference;
      await service.PutBlobAndReferenceAsync(requestContext1, defaultDomainId, blobId1, blob, reference);
      string valueString = blobId.ValueString;
      blobId = (BlobIdentifier) null;
      return valueString;
    }

    private IdBlobReference GetBlobReference(Guid planId, int logId, int pageId) => new IdBlobReference(string.Format("{0}/{1}/{2}", (object) planId, (object) logId, (object) pageId), "buildlogs");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
