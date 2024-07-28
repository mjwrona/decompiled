// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupManifestArtifactValidator
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class DedupManifestArtifactValidator
  {
    private static readonly HttpClient HttpClient = new HttpClient()
    {
      Timeout = TimeSpan.FromSeconds(7.0)
    };

    public static Manifest DownloadAndValidateManifest(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier manifestId,
      IEnumerable<DedupNode> proofNodes,
      DedupIdentifier rootId)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return DedupManifestArtifactValidator.DownloadAndValidateManifest(requestContext, domainId, manifestId, proofNodes, rootId, DedupManifestArtifactValidator.\u003C\u003EO.\u003C0\u003E__GetDownloadedManifest ?? (DedupManifestArtifactValidator.\u003C\u003EO.\u003C0\u003E__GetDownloadedManifest = new Func<IVssRequestContext, IDomainId, DedupIdentifier, (Manifest, ulong)>(DedupManifestArtifactValidator.GetDownloadedManifest)));
    }

    public static Manifest DownloadAndValidateManifest(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier manifestId,
      IEnumerable<DedupNode> proofNodes,
      DedupIdentifier rootId,
      HashType hashType)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return DedupManifestArtifactValidator.DownloadAndValidateManifest(requestContext, domainId, manifestId, proofNodes, rootId, hashType, DedupManifestArtifactValidator.\u003C\u003EO.\u003C0\u003E__GetDownloadedManifest ?? (DedupManifestArtifactValidator.\u003C\u003EO.\u003C0\u003E__GetDownloadedManifest = new Func<IVssRequestContext, IDomainId, DedupIdentifier, (Manifest, ulong)>(DedupManifestArtifactValidator.GetDownloadedManifest)));
    }

    public static Manifest DownloadAndValidateManifest(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier manifestId,
      IEnumerable<DedupNode> proofNodes,
      DedupIdentifier rootId,
      Func<IVssRequestContext, IDomainId, DedupIdentifier, (Manifest Manifest, ulong Size)> getManifestFunc)
    {
      return DedupManifestArtifactValidator.DownloadAndValidateManifest(requestContext, domainId, manifestId, proofNodes, rootId, ChunkerHelper.DefaultChunkHashType, getManifestFunc);
    }

    public static Manifest DownloadAndValidateManifest(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier manifestId,
      IEnumerable<DedupNode> proofNodes,
      DedupIdentifier rootId,
      HashType hashType,
      Func<IVssRequestContext, IDomainId, DedupIdentifier, (Manifest Manifest, ulong Size)> getManifestFunc)
    {
      (Manifest, ulong) valueTuple = getManifestFunc(requestContext, domainId, manifestId);
      ulong num1 = valueTuple.Item2;
      IList<ManifestItem> items = valueTuple.Item1.Items;
      IEnumerable<IGrouping<string, ManifestItem>> source1 = items.Where<ManifestItem>((Func<ManifestItem, bool>) (x => x.Blob != null)).GroupBy<ManifestItem, string>((Func<ManifestItem, string>) (x => x.Blob.Id)).Where<IGrouping<string, ManifestItem>>((Func<IGrouping<string, ManifestItem>, bool>) (g => g.Select<ManifestItem, ulong>((Func<ManifestItem, ulong>) (me => me.Blob.Size)).Distinct<ulong>().Count<ulong>() > 1)).Where<IGrouping<string, ManifestItem>>((Func<IGrouping<string, ManifestItem>, bool>) (g => g.Any<ManifestItem>((Func<ManifestItem, bool>) (me => (long) me.Blob.Size != (long) g.FirstOrDefault<ManifestItem>().Blob.Size))));
      if (source1.Count<IGrouping<string, ManifestItem>>() > 0)
      {
        IGrouping<string, ManifestItem> source2 = source1.FirstOrDefault<IGrouping<string, ManifestItem>>();
        throw new ArgumentException("The manifest items contain inconsistent sizing on the same ID: " + source2.Key + ". " + string.Format("{0} inconsistent entries: {1}", (object) source2.Count<ManifestItem>(), (object) string.Join<ulong>(" ,", source2.Select<ManifestItem, ulong>((Func<ManifestItem, ulong>) (ig => ig.Blob.Size)))));
      }
      Dictionary<DedupIdentifier, ulong> dictionary1 = new Dictionary<DedupIdentifier, ulong>();
      foreach (ManifestItem manifestItem in (IEnumerable<ManifestItem>) items)
      {
        DedupIdentifier key = (DedupIdentifier) null;
        if (manifestItem.Blob != null)
          key = DedupIdentifier.Create(manifestItem.Blob.Id);
        if (key != (DedupIdentifier) null && !dictionary1.ContainsKey(key))
          dictionary1.Add(key, manifestItem.Blob.Size);
      }
      if (!dictionary1.ContainsKey(manifestId))
        dictionary1.Add(manifestId, num1);
      Dictionary<NodeDedupIdentifier, DedupNode> dictionary2 = new Dictionary<NodeDedupIdentifier, DedupNode>();
      foreach (DedupNode proofNode in proofNodes)
      {
        NodeDedupIdentifier nodeDedupIdentifier = proofNode.CalculateNodeDedupIdentifier();
        if (!dictionary2.ContainsKey(nodeDedupIdentifier))
          dictionary2.Add(nodeDedupIdentifier, proofNode);
      }
      Dictionary<NodeDedupIdentifier, DedupNode> proofNodes1 = dictionary2;
      HashSet<NodeDedupIdentifier> roots = new HashSet<NodeDedupIdentifier>();
      roots.Add(rootId.CastToNodeDedupIdentifier());
      Dictionary<DedupIdentifier, ulong> idsToValidate = dictionary1;
      int num2 = (int) hashType;
      IEnumerable<DedupIdentifier> unvalidatedIds = ProofHelper.DetermineUnvalidatedIds((IDictionary<NodeDedupIdentifier, DedupNode>) proofNodes1, (ISet<NodeDedupIdentifier>) roots, (IDictionary<DedupIdentifier, ulong>) idsToValidate, (HashType) num2);
      if (unvalidatedIds.Any<DedupIdentifier>())
        throw new ArgumentException("The manifest items have unvalidated roots, (" + string.Format("Count: {0}, UnvalidatedFirst: {1}, ManifestSize: {2}", (object) unvalidatedIds.Count<DedupIdentifier>(), (object) unvalidatedIds.First<DedupIdentifier>(), (object) num1) + ")");
      return valueTuple.Item1;
    }

    public static (Manifest Manifest, ulong Size) GetDownloadedManifest(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier manifestId)
    {
      ulong size;
      using (Stream dedupDownloadStream = DedupManifestArtifactValidator.GetDedupDownloadStream(requestContext, domainId, manifestId, out size))
        return (JsonSerializer.Deserialize<Manifest>(dedupDownloadStream), size);
    }

    private static Stream GetDedupDownloadStream(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      out ulong size)
    {
      DedupDownloadInfo dedupDownloadInfo = requestContext.GetService<IDedupStore>().GetDownloadInfoAsync(requestContext, domainId, dedupId, true).SyncResult<DedupDownloadInfo>();
      if (dedupDownloadInfo == null)
        throw new DedupNotFoundException(string.Format("The requested manifest ID {0} does not exist.", (object) dedupId));
      IEnumerable<Uri> uris;
      if (dedupDownloadInfo.Chunks != null)
        uris = (IEnumerable<Uri>) ((IEnumerable<ChunkDedupDownloadInfo>) dedupDownloadInfo.Chunks).Select<ChunkDedupDownloadInfo, Uri>((Func<ChunkDedupDownloadInfo, Uri>) (c => c.Url)).ToArray<Uri>();
      else
        uris = (IEnumerable<Uri>) new Uri[1]
        {
          dedupDownloadInfo.Url
        };
      size = (ulong) dedupDownloadInfo.Size;
      return (Stream) Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.BlobStitcher.GetTransitiveContent(dedupDownloadInfo.Size, uris, DedupManifestArtifactValidator.HttpClient, (IAppTraceSource) NoopAppTraceSource.Instance, requestContext.CancellationToken);
    }
  }
}
