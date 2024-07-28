// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.DecorateCacheWithCommitDetailsHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class DecorateCacheWithCommitDetailsHandler : 
    IAsyncHandler<IReadOnlyCollection<ICommitLogEntry>>,
    IAsyncHandler<IReadOnlyCollection<ICommitLogEntry>, NullResult>,
    IHaveInputType<IReadOnlyCollection<ICommitLogEntry>>,
    IHaveOutputType<NullResult>
  {
    private readonly ICache<string, object> requestItemsCache;

    public DecorateCacheWithCommitDetailsHandler(ICache<string, object> requestItemsCache) => this.requestItemsCache = requestItemsCache;

    public Task<NullResult> Handle(IReadOnlyCollection<ICommitLogEntry> commits)
    {
      ICommitLogEntry entry = commits.Last<ICommitLogEntry>();
      this.requestItemsCache.Set("Packaging.Properties.SubmittedCommitBookmark", (object) entry.GetCommitLogBookmark());
      if (entry.CommitOperationData is BatchCommitOperationData commitOperationData1)
      {
        this.requestItemsCache.Set("Packaging.Properties.CommitOperationType", (object) commitOperationData1.Operations.FirstOrDefault<ICommitOperationData>()?.GetType().Name);
        this.requestItemsCache.Set("Packaging.Properties.CommitBatchSize", (object) commitOperationData1.Operations.Count<ICommitOperationData>());
        this.requestItemsCache.Set("Packaging.Properties.BatchCommitVersionedPackageNames", (object) commitOperationData1.Operations.Select<ICommitOperationData, IPackageVersionOperationData>((Func<ICommitOperationData, IPackageVersionOperationData>) (o => o as IPackageVersionOperationData)).Where<IPackageVersionOperationData>((Func<IPackageVersionOperationData, bool>) (o => o != null)).Select<IPackageVersionOperationData, DecorateCacheWithCommitDetailsHandler.MinimalVersionedPackageName>((Func<IPackageVersionOperationData, DecorateCacheWithCommitDetailsHandler.MinimalVersionedPackageName>) (o => new DecorateCacheWithCommitDetailsHandler.MinimalVersionedPackageName(o.PackageName.NormalizedName, o.Identity.Version.NormalizedVersion))).ToList<DecorateCacheWithCommitDetailsHandler.MinimalVersionedPackageName>());
      }
      else
        this.requestItemsCache.Set("Packaging.Properties.CommitOperationType", (object) entry.CommitOperationData.GetType().Name);
      if (commits.Count > 1)
        this.requestItemsCache.Set("Packaging.Properties.CommitCount", (object) commits.Count);
      int val = commits.Select<ICommitLogEntry, int>((Func<ICommitLogEntry, int>) (c => !(c.CommitOperationData is IBatchCommitOperationData commitOperationData2) ? 1 : commitOperationData2.Operations.Count<ICommitOperationData>())).Sum();
      if (val > 1)
        this.requestItemsCache.Set("Packaging.Properties.OperationCount", (object) val);
      return Task.FromResult<NullResult>((NullResult) null);
    }

    public class MinimalVersionedPackageName
    {
      public string Name { get; }

      public string Ver { get; }

      public MinimalVersionedPackageName(string name, string ver)
      {
        this.Name = name;
        this.Ver = ver;
      }
    }
  }
}
