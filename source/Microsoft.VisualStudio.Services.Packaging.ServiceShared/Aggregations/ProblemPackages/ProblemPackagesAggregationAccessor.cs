// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages.ProblemPackagesAggregationAccessor
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages
{
  public class ProblemPackagesAggregationAccessor : 
    IAggregationAccessor<ProblemPackagesAggregation>,
    IAggregationAccessor,
    IProblemPackagesReadService
  {
    private readonly IETaggedDocumentUpdater documentUpdater;
    private readonly IAggregationDocumentProvider<ProblemPackagesFile, NoSpecifier> documentProvider;

    public ProblemPackagesAggregationAccessor(
      IAggregation aggregation,
      IETaggedDocumentUpdater documentUpdater,
      IAggregationDocumentProvider<ProblemPackagesFile, NoSpecifier> documentProvider)
    {
      this.Aggregation = aggregation;
      this.documentUpdater = documentUpdater;
      this.documentProvider = documentProvider;
    }

    public IAggregation Aggregation { get; }

    public async Task ApplyCommitAsync(
      IFeedRequest feedRequest,
      IReadOnlyList<ICommitLogEntry> commitLogEntries)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<(AddProblemPackageOperationData, DateTime)> addProblemPackageOperations = commitLogEntries.SelectMany<ICommitLogEntry, ICommitLogEntry>(ProblemPackagesAggregationAccessor.\u003C\u003EO.\u003C0\u003E__FlattenCommitLogEntry ?? (ProblemPackagesAggregationAccessor.\u003C\u003EO.\u003C0\u003E__FlattenCommitLogEntry = new Func<ICommitLogEntry, IEnumerable<ICommitLogEntry>>(AggregationAccessorCommonUtils.FlattenCommitLogEntry))).Select<ICommitLogEntry, (AddProblemPackageOperationData, DateTime)>((Func<ICommitLogEntry, (AddProblemPackageOperationData, DateTime)>) (x => (x.CommitOperationData as AddProblemPackageOperationData, x.CreatedDate))).Where<(AddProblemPackageOperationData, DateTime)>((Func<(AddProblemPackageOperationData, DateTime), bool>) (x => x.OpData != null)).ToList<(AddProblemPackageOperationData, DateTime)>();
      if (!addProblemPackageOperations.Any<(AddProblemPackageOperationData, DateTime)>())
        return;
      (EtagValue<ProblemPackagesFile>, bool) valueTuple = await this.documentUpdater.UpdateETaggedDocumentAsync<ProblemPackagesFile, NoSpecifier>(new EtagValue<ProblemPackagesFile>?(), this.documentProvider, feedRequest, NoSpecifier.Null, (Func<ProblemPackagesFile, (ProblemPackagesFile, bool)>) (doc =>
      {
        foreach ((AddProblemPackageOperationData packageOperationData, DateTime timestamp) in addProblemPackageOperations)
          doc = doc.WithAddedOrUpdatedPackageVersion(packageOperationData.Identity.Name, packageOperationData.Identity.Version, new ProblemPackagesFilePackageVersion(timestamp, packageOperationData.UpstreamSource, packageOperationData.Reasons));
        return (doc, true);
      }), (Func<string>) (() => "failed to update problem packages aggregation"));
    }

    public async Task<ProblemPackagesFile> GetProblemPackagesAsync(IFeedRequest feedRequest)
    {
      ProblemPackagesFile problemPackagesAsync;
      (await this.documentProvider.GetDocumentAsync(feedRequest, NoSpecifier.Null)).Deconstruct<ProblemPackagesFile>(out problemPackagesAsync, out string _);
      return problemPackagesAsync;
    }
  }
}
