// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Staging.IStageService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Analytics.Staging
{
  [DefaultServiceImplementation(typeof (StageService))]
  public interface IStageService : IVssFrameworkService
  {
    IngestResult StageRecords(
      IVssRequestContext requestContext,
      string table,
      int providerShard,
      int stream,
      Stream content);

    StageProviderShardInfo GetShard(
      IVssRequestContext requestContext,
      string table,
      int providerShard);

    StageTableInfo GetTable(IVssRequestContext requestContext, string table);

    StageProviderShardInfo CreateShard(
      IVssRequestContext requestContext,
      string table,
      int providerShard);

    void DeleteShard(IVssRequestContext requestContext, string table, int providerShard);

    void InvalidateProviderShard(
      IVssRequestContext requestContext,
      string table,
      int providerShardId,
      IList<string> fieldNames = null,
      bool disableCurrentStream = false,
      bool keysOnly = false);

    void InvalidateTable(IVssRequestContext requestContext, string table);

    void InvalidateAllTables(IVssRequestContext requestContext);

    void SetStagingTableMaintenance(
      IVssRequestContext requestContext,
      string table,
      string maintenanceReason,
      ITeamFoundationDatabaseProperties database = null);

    void ClearStagingTableMaintenance(
      IVssRequestContext requestContext,
      string table,
      ITeamFoundationDatabaseProperties database = null);

    CleanupStreamResult CleanupStreams(IVssRequestContext requestContext, bool forceBatchCleanup = false);

    CleanupStreamResult CleanupOrphanedStreamData(
      IVssRequestContext requestContext,
      int commandTimeout,
      ITeamFoundationDatabaseProperties database = null);

    string ExportRecords(IVssRequestContext requestContext, string table, long? batchId = null);
  }
}
