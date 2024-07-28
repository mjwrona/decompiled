// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git.TempAndFileMetaDataStoreDeleter
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.SQL;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git
{
  public class TempAndFileMetaDataStoreDeleter
  {
    internal bool DeleteTempAndFileMetaDataStoreRecords(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      List<string> branches)
    {
      bool flag1 = true;
      bool flag2 = false;
      try
      {
        this.GetFileMetadataStoreDataAccess(indexingUnit).DeleteBranchInfoInRecords(requestContext, branches);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1083049, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Completed deleting records for {0} in FileMetaDataStore.", (object) string.Join(",", (IEnumerable<string>) branches))));
        TempFileMetadataStoreDataAccess metadataStoreDataAccess = this.GetTempFileMetadataStoreDataAccess(indexingUnit);
        foreach (string branch in branches)
        {
          metadataStoreDataAccess.DeleteBranchInfoInRecords(requestContext, branch);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1083049, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Completed deleting records for {0} in TempData store.", (object) branch)));
        }
        flag2 = true;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083049, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Deletion of sql metadata store records for indexingunit {0} for branches {1} failed with exception {2}", (object) indexingUnit.IndexingUnitId, (object) string.Join(",", (IEnumerable<string>) branches), (object) ex)));
        flag1 = false;
      }
      finally
      {
        if (flag2)
        {
          ScopedGitRepositoryIndexingProperties repoIndexingProperties = indexingUnit.Properties as ScopedGitRepositoryIndexingProperties;
          if (this.IsFeatureFlagEnabled(requestContext))
          {
            if (repoIndexingProperties.BranchesToClean == null)
              repoIndexingProperties.BranchesToClean = new List<string>();
            branches.ForEach((Action<string>) (b =>
            {
              repoIndexingProperties.BranchIndexInfo.Remove(b);
              if (repoIndexingProperties.BranchesToClean.Contains(b))
                return;
              repoIndexingProperties.BranchesToClean.Add(b);
            }));
          }
          else
            branches.ForEach((Action<string>) (b => repoIndexingProperties.BranchIndexInfo.Remove(b)));
          indexingUnitDataAccess.UpdateIndexingUnit(requestContext, indexingUnit);
        }
      }
      return flag1;
    }

    private bool IsFeatureFlagEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Search.Server.Code.DisableESCleanUp");

    internal virtual TempFileMetadataStoreDataAccess GetTempFileMetadataStoreDataAccess(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit)
    {
      return new TempFileMetadataStoreDataAccess(scopedIndexingUnit);
    }

    internal virtual FileMetadataStoreDataAccess GetFileMetadataStoreDataAccess(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit scopedIndexingUnit)
    {
      return new FileMetadataStoreDataAccess(scopedIndexingUnit);
    }
  }
}
