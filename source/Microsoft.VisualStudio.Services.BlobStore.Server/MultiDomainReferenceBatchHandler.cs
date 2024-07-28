// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.MultiDomainReferenceBatchHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Common.Exceptions;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class MultiDomainReferenceBatchHandler
  {
    private IVssRequestContext requestContext;
    private HttpRequestMessage requestMessage;
    private IDomainId domainId;
    private const long MaxRequestContentLengthDefault = 20971520;

    public MultiDomainReferenceBatchHandler()
    {
    }

    public MultiDomainReferenceBatchHandler(
      IVssRequestContext requestContext,
      HttpRequestMessage requestMessage,
      IDomainId domainId)
    {
      this.requestContext = requestContext.AllowMultiDomainOperations(domainId) ? requestContext : throw new FeatureDisabledException("Multi-Domain");
      this.requestMessage = requestMessage;
      this.domainId = domainId;
    }

    public async Task<HttpResponseMessage> TryAddReferencesAsync()
    {
      if (this.requestMessage.Content == null)
        throw new ContentMissingException(Resources.ReferenceContentMissing());
      long? contentLength = this.requestMessage.Content.Headers.ContentLength;
      string json;
      using (this.requestContext.RequestTimer.CreateTimeToFirstPageExclusionBlock())
        json = await this.requestMessage.Content.ReadAsStringAsync().ConfigureAwait(true);
      if (!contentLength.HasValue)
        ContentLengthThrottleHelper.AssertMaxRequestContentLength(this.requestContext, ServiceRegistryConstants.MaxRequestContentLengthPath, (long) json.Length, false, 20971520L);
      ReferenceBatch request = JsonSerializer.Deserialize<ReferenceBatch>(json);
      IBlobStore service = this.requestContext.GetService<IBlobStore>();
      IDictionary<BlobIdentifier, IEnumerable<BlobReference>> refsGroupedByBlob;
      IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> groupedByBlobIdWithBlocks;
      this.CheckReferencesAndRegroupRequest(request, out groupedByBlobIdWithBlocks, out refsGroupedByBlob);
      ConfiguredTaskAwaitable<IDictionary<BlobIdentifier, IEnumerable<BlobReference>>> configuredTaskAwaitable;
      Dictionary<BlobIdentifier, IList<BlobReference>> dictionary1;
      if (groupedByBlobIdWithBlocks != null)
      {
        configuredTaskAwaitable = service.TryReferenceWithBlocksAsync(this.requestContext, this.domainId, groupedByBlobIdWithBlocks).ConfigureAwait(true);
        dictionary1 = (await configuredTaskAwaitable).ToDictionary<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, BlobIdentifier, IList<BlobReference>>((Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, BlobIdentifier>) (k => k.Key), (Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, IList<BlobReference>>) (v => (IList<BlobReference>) new List<BlobReference>(v.Value)));
      }
      else
        dictionary1 = new Dictionary<BlobIdentifier, IList<BlobReference>>();
      IDictionary<BlobIdentifier, IList<BlobReference>> refsWithBlocksGroupedByBlobResult = (IDictionary<BlobIdentifier, IList<BlobReference>>) dictionary1;
      Dictionary<BlobIdentifier, IList<BlobReference>> dictionary2;
      if (refsGroupedByBlob != null)
      {
        configuredTaskAwaitable = service.TryReferenceAsync(this.requestContext, this.domainId, refsGroupedByBlob).ConfigureAwait(true);
        dictionary2 = (await configuredTaskAwaitable).ToDictionary<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, BlobIdentifier, IList<BlobReference>>((Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, BlobIdentifier>) (k => k.Key), (Func<KeyValuePair<BlobIdentifier, IEnumerable<BlobReference>>, IList<BlobReference>>) (v => (IList<BlobReference>) new List<BlobReference>(v.Value)));
      }
      else
        dictionary2 = new Dictionary<BlobIdentifier, IList<BlobReference>>();
      IDictionary<BlobIdentifier, IList<BlobReference>> refsGroupedByBlobResult = (IDictionary<BlobIdentifier, IList<BlobReference>>) dictionary2;
      IEnumerable<Reference> source = this.PrepareResponse(request, refsWithBlocksGroupedByBlobResult, refsGroupedByBlobResult);
      ReferenceBatch dataContractObject = new ReferenceBatch()
      {
        References = source.ToList<Reference>()
      };
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = JsonSerializer.SerializeToContent<ReferenceBatch>(dataContractObject)
      };
      request = (ReferenceBatch) null;
      service = (IBlobStore) null;
      refsGroupedByBlob = (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) null;
      refsWithBlocksGroupedByBlobResult = (IDictionary<BlobIdentifier, IList<BlobReference>>) null;
      return httpResponseMessage;
    }

    public async Task<HttpResponseMessage> DeleteReferencesAsync()
    {
      if (this.requestMessage.Content == null)
        throw new ContentMissingException(Resources.ReferenceContentMissing());
      IDictionary<BlobIdentifier, IEnumerable<IdBlobReference>> dictionaryOfEnumerables = JsonSerializer.Deserialize<ReferenceBatch>(await this.requestMessage.Content.ReadAsStringAsync().ConfigureAwait(true)).References.Select<Reference, KeyValuePair<IdBlobReference, BlobIdentifier>>((Func<Reference, KeyValuePair<IdBlobReference, BlobIdentifier>>) (rwb =>
      {
        switch (rwb.Kind)
        {
          case ReferenceKind.IdReference:
            IdBlobReference key = rwb.BlobReference.Match<IdBlobReference>((Func<IdBlobReference, IdBlobReference>) (idRef => idRef), (Func<KeepUntilBlobReference, IdBlobReference>) (keepUntil =>
            {
              throw new NotSupportedException("expected id reference but got keep until reference");
            }));
            if (key.Scope != null)
              return new KeyValuePair<IdBlobReference, BlobIdentifier>(key, BlobIdentifier.Deserialize(rwb.Blob.Id));
            throw new ArgumentException("Id reference without scope. Blob Id: " + rwb.Blob.Id + ", Reference: " + key.ToString());
          case ReferenceKind.KeepUntilReference:
            throw new ArgumentException("keepUntil references can't be deleted.");
          default:
            throw new ArgumentException("unkown ReferenceKind");
        }
      })).GroupBy<KeyValuePair<IdBlobReference, BlobIdentifier>, BlobIdentifier, IdBlobReference>((Func<KeyValuePair<IdBlobReference, BlobIdentifier>, BlobIdentifier>) (kvp => kvp.Value), (Func<KeyValuePair<IdBlobReference, BlobIdentifier>, IdBlobReference>) (kvp => kvp.Key)).ToDictionaryOfEnumerables<BlobIdentifier, IdBlobReference>();
      await this.requestContext.GetService<IBlobStore>().RemoveReferencesAsync(this.requestContext, this.domainId, dictionaryOfEnumerables).ConfigureAwait(true);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    internal void CheckReferencesAndRegroupRequest(
      ReferenceBatch request,
      out IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>> groupedByBlobIdWithBlocks,
      out IDictionary<BlobIdentifier, IEnumerable<BlobReference>> groupedByBlobId)
    {
      List<Reference> source1 = new List<Reference>();
      List<Reference> source2 = new List<Reference>();
      HashSet<EquatableTuple<string, IdBlobReference>> source3 = new HashSet<EquatableTuple<string, IdBlobReference>>();
      HashSet<string> source4 = new HashSet<string>();
      foreach (Reference reference in request.References)
      {
        if (reference.Blob.HasBlockHashes())
          source1.Add(reference);
        else
          source2.Add(reference);
        switch (reference.Kind)
        {
          case ReferenceKind.IdReference:
            IdBlobReference t2 = reference.BlobReference.Match<IdBlobReference>((Func<IdBlobReference, IdBlobReference>) (x => x), (Func<KeepUntilBlobReference, IdBlobReference>) (keepUntil =>
            {
              throw new NotSupportedException("expected id reference but got keep until reference");
            }));
            if (t2.Scope == null)
              throw new ArgumentException("Id reference without scope. Blob Id: " + reference.Blob.Id + ", Reference: " + t2.ToString());
            if (!source3.Add(EquatableTuple.Create<string, IdBlobReference>(reference.Blob.Id, t2)))
              throw new ArgumentException("Duplicate blob/reference pair. Blob Id: " + reference.Blob.Id + ", Reference: " + t2.ToString());
            continue;
          case ReferenceKind.KeepUntilReference:
            KeepUntilBlobReference untilBlobReference = reference.BlobReference.Match<KeepUntilBlobReference>((Func<IdBlobReference, KeepUntilBlobReference>) (keepUntil =>
            {
              throw new NotSupportedException("expected keep until reference but got id reference");
            }), (Func<KeepUntilBlobReference, KeepUntilBlobReference>) (x => x));
            if (!source4.Add(reference.Blob.Id))
              throw new ArgumentException("Blob with more than one keepUntil reference. Blob Id: " + reference.Blob.Id + ", Reference: " + untilBlobReference.ToString());
            continue;
          default:
            throw new ArgumentException("unknown BlobReferenceKind");
        }
      }
      if (!source3.Any<EquatableTuple<string, IdBlobReference>>() && !source4.Any<string>())
        throw new ArgumentException("No reference is given in the input.");
      groupedByBlobIdWithBlocks = !source1.Any<Reference>() ? (IDictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>) new Dictionary<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, IEnumerable<BlobReference>>() : source1.GroupBy<Reference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, BlobReference>((Func<Reference, Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>) (reference => reference.Blob.ToBlobIdentifierWithBlocks()), (Func<Reference, BlobReference>) (reference => reference.BlobReference)).ToDictionaryOfEnumerables<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks, BlobReference>();
      if (source2.Any<Reference>())
        groupedByBlobId = source2.GroupBy<Reference, BlobIdentifier, BlobReference>((Func<Reference, BlobIdentifier>) (reference => reference.Blob.ToBlobIdentifier()), (Func<Reference, BlobReference>) (reference => reference.BlobReference)).ToDictionaryOfEnumerables<BlobIdentifier, BlobReference>();
      else
        groupedByBlobId = (IDictionary<BlobIdentifier, IEnumerable<BlobReference>>) new Dictionary<BlobIdentifier, IEnumerable<BlobReference>>();
    }

    internal IEnumerable<Reference> PrepareResponse(
      ReferenceBatch request,
      IDictionary<BlobIdentifier, IList<BlobReference>> refsWithBlocksGroupedByBlobResult,
      IDictionary<BlobIdentifier, IList<BlobReference>> refsGroupedByBlobResult)
    {
      ISet<Tuple<BlobIdentifier, BlobReference>> missingRefSet = (ISet<Tuple<BlobIdentifier, BlobReference>>) new HashSet<Tuple<BlobIdentifier, BlobReference>>();
      if (refsWithBlocksGroupedByBlobResult != null)
      {
        IEnumerable<Tuple<BlobIdentifier, BlobReference>> other = refsWithBlocksGroupedByBlobResult.SelectMany<KeyValuePair<BlobIdentifier, IList<BlobReference>>, Tuple<BlobIdentifier, BlobReference>>((Func<KeyValuePair<BlobIdentifier, IList<BlobReference>>, IEnumerable<Tuple<BlobIdentifier, BlobReference>>>) (kvp => kvp.Value.Select<BlobReference, Tuple<BlobIdentifier, BlobReference>>((Func<BlobReference, Tuple<BlobIdentifier, BlobReference>>) (refId => Tuple.Create<BlobIdentifier, BlobReference>(kvp.Key, refId)))));
        missingRefSet.UnionWith(other);
      }
      if (refsGroupedByBlobResult != null)
      {
        IEnumerable<Tuple<BlobIdentifier, BlobReference>> other = refsGroupedByBlobResult.SelectMany<KeyValuePair<BlobIdentifier, IList<BlobReference>>, Tuple<BlobIdentifier, BlobReference>>((Func<KeyValuePair<BlobIdentifier, IList<BlobReference>>, IEnumerable<Tuple<BlobIdentifier, BlobReference>>>) (kvp => kvp.Value.Select<BlobReference, Tuple<BlobIdentifier, BlobReference>>((Func<BlobReference, Tuple<BlobIdentifier, BlobReference>>) (refId => Tuple.Create<BlobIdentifier, BlobReference>(kvp.Key, refId)))));
        missingRefSet.UnionWith(other);
      }
      return request.References.Select<Reference, Reference>((Func<Reference, Reference>) (r =>
      {
        BlobReference blobReference = r.BlobReference;
        Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.Blob blob = r.Blob;
        return ReferenceFactory.MakeReference(blobReference, blob, new bool?(missingRefSet.Contains(Tuple.Create<BlobIdentifier, BlobReference>(blob.ToBlobIdentifier(), blobReference))));
      }));
    }
  }
}
