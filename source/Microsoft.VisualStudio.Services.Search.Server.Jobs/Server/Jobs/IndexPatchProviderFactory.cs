// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.IndexPatchProviderFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal static class IndexPatchProviderFactory
  {
    internal static IIndexPatchProvider GetPatchProvider(
      IndexingExecutionContext indexingExecutionContext,
      Patch patch,
      TraceMetaData traceMetaData)
    {
      switch (patch)
      {
        case Patch.None:
          return (IIndexPatchProvider) null;
        case Patch.RepositoryHeal:
          return (IIndexPatchProvider) new RepositoryHealPatchProvider();
        case Patch.ReIndexFailedItems:
          return (IIndexPatchProvider) new FailedItemsPatchProvider(traceMetaData);
        case Patch.ReIndexCppFilesUsingFailedItems:
          return (IIndexPatchProvider) new ReIndexCppFilesPatchProvider(indexingExecutionContext, CodeFileContract.CreateCodeContract(indexingExecutionContext.ProvisioningContext.ContractType, indexingExecutionContext.ProvisioningContext.SearchPlatform));
        default:
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Could not find any mapping for the Patch : {0}", (object) patch)));
      }
    }
  }
}
