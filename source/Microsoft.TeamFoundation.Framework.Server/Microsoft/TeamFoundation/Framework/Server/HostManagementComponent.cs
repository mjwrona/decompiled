// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostManagementComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class HostManagementComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[20]
    {
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(1, true),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(2),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(3),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(4),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(5),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(6),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(7),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(8),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(9),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(10),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(11),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(12),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(13),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(14),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(15),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(16),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(17),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(18),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(19),
      (IComponentCreator) new ComponentCreator<HostManagementComponent>(20)
    }, "HostManagement");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly string s_getUtcDateStmt = "SELECT GETUTCDATE()";

    static HostManagementComponent()
    {
      HostManagementComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      HostManagementComponent.s_sqlExceptionFactories.Add(800026, new SqlExceptionFactory(typeof (HostDoesNotExistException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800027, new SqlExceptionFactory(typeof (HostAlreadyExistsException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800028, new SqlExceptionFactory(typeof (HostMustBeTopLevelException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800029, new SqlExceptionFactory(typeof (HostStatusChangeException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800030, new SqlExceptionFactory(typeof (HostStatusChangeException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800031, new SqlExceptionFactory(typeof (HostCannotBeDeletedException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800039, new SqlExceptionFactory(typeof (HostProcessNotFoundException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800303, new SqlExceptionFactory(typeof (HostCannotBeDeletedBecauseItHasChildrenException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(1110006, new SqlExceptionFactory(typeof (ArgumentException)));
      HostManagementComponent.s_sqlExceptionFactories.Add(800309, new SqlExceptionFactory(typeof (CannotStartDeletingHostException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) HostManagementComponent.s_sqlExceptionFactories;

    internal void CreateServiceHost(
      Guid hostId,
      Guid parentHostId,
      string name,
      string description,
      TeamFoundationServiceHostStatus status,
      string statusReason,
      int databaseId,
      TeamFoundationHostType hostType,
      string serviceLevel,
      int storageAccountId,
      ServiceHostSubStatus subStatus)
    {
      this.PrepareStoredProcedure("prc_CreateServiceHost");
      this.BindNullableGuid("@parentHostId", parentHostId);
      this.BindGuid("@hostId", hostId);
      this.BindString("@name", name, 128, true, SqlDbType.NVarChar);
      this.BindString("@description", description, -1, true, SqlDbType.NVarChar);
      this.BindInt("@hostStatus", (int) status);
      this.BindString("@hostStatusReason", statusReason, 2048, true, SqlDbType.NVarChar);
      this.BindInt("@databaseId", databaseId);
      this.BindInt("@hostType", (int) hostType);
      if (this.Version >= 4)
        this.BindString("@serviceLevel", serviceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      if (this.Version >= 5)
        this.BindInt("@storageAccountId", storageAccountId);
      if (this.Version < 7)
      {
        this.BindString("@authority", (string) null, 260, true, SqlDbType.NVarChar);
        this.BindString("@virtualDirectory", (string) null, 260, true, SqlDbType.NVarChar);
        this.BindString("@resourceDirectory", (string) null, 260, true, SqlDbType.NVarChar);
      }
      if (this.Version >= 12)
        this.BindInt("@subStatus", (int) subStatus);
      this.ExecuteNonQuery();
    }

    internal void DeleteServiceHost(Guid hostId, HostDeletionReason deletionReason)
    {
      this.PrepareStoredProcedure("prc_DeleteServiceHost");
      this.BindGuid("@hostId", hostId);
      this.BindInt("@deletionReason", (int) deletionReason);
      this.ExecuteNonQuery();
    }

    internal void UpdateServiceHostBootstrapLegacy(
      Guid hostId,
      string name,
      string description,
      string connectionString)
    {
      if (this.Version >= 7)
        throw new ServiceVersionNotSupportedException("HostManagement", 1, 1);
      this.PrepareStoredProcedure("prc_UpdateServiceHost");
      this.BindGuid("@hostId", hostId);
      this.BindString("@name", name, 128, true, SqlDbType.NVarChar);
      this.BindString("@description", description, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@virtualDirectory", (string) null, 260, true, SqlDbType.NVarChar);
      this.BindString("@resourceDirectory", (string) null, 260, true, SqlDbType.NVarChar);
      this.BindString("@connectionString", connectionString, 520, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal void UpdateServiceHost(
      Guid hostId,
      Guid parentHostId,
      string name,
      string description,
      int databaseId,
      int storageAccountId,
      string serviceLevel,
      ServiceHostSubStatus subStatus)
    {
      this.PrepareStoredProcedure("prc_UpdateServiceHost2");
      this.BindGuid("@hostId", hostId);
      this.BindNullableGuid("@parentHostId", parentHostId);
      this.BindString("@name", name, 128, true, SqlDbType.NVarChar);
      this.BindString("@description", description, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindNullableInt("@databaseId", databaseId, DatabaseManagementConstants.InvalidDatabaseId);
      if (this.Version >= 4)
        this.BindString("@serviceLevel", serviceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      if (this.Version < 7)
      {
        this.BindString("@authority", (string) null, 260, true, SqlDbType.NVarChar);
        this.BindString("@virtualDirectory", (string) null, 260, true, SqlDbType.NVarChar);
        this.BindString("@resourceDirectory", (string) null, 260, true, SqlDbType.NVarChar);
      }
      if (this.Version >= 9)
        this.BindNullableInt("@storageAccountId", storageAccountId, -1);
      if (this.Version >= 11)
        this.BindInt("@subStatus", (int) subStatus);
      this.ExecuteNonQuery();
    }

    internal virtual DateTime UpdateServiceHostLastUserAccess(IEnumerable<Guid> hostsToUpdate)
    {
      try
      {
        this.TraceEnter(57516, nameof (UpdateServiceHostLastUserAccess));
        foreach (Guid guid in hostsToUpdate)
          this.Trace(57517, TraceLevel.Verbose, "UpdateServiceHostLastUserAccess: Updating host {0}", (object) guid);
        this.PrepareStoredProcedure("prc_UpdateServiceHostLastUserAccess");
        this.BindGuidTable("@hostUpdates", hostsToUpdate);
        return (DateTime) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        this.TraceException(57518, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(57519, nameof (UpdateServiceHostLastUserAccess));
      }
    }

    internal void DetectInactiveProcesses()
    {
      this.PrepareStoredProcedure("prc_DetectInactiveProcesses");
      this.ExecuteNonQuery();
    }

    internal ResultCollection QueryServiceHostProcesses(Guid machineId)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostProcesses");
      this.BindNullableGuid("@machineId", machineId);
      ResultCollection resultCollection;
      try
      {
        resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<TeamFoundationServiceHostProcess>((ObjectBinder<TeamFoundationServiceHostProcess>) new TeamFoundationServiceHostProcessBinder());
      }
      catch (DatabaseConfigurationException ex)
      {
        resultCollection = this.QueryServiceHostProperties(Guid.Empty, ServiceHostFilterFlags.IncludeProcessDetails);
        resultCollection.NextResult();
      }
      return resultCollection;
    }

    internal ResultCollection QueryServiceHosts(
      DateTime? lastUserAccessFrom,
      DateTime? lastUserAccessTo)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostPropertiesByLastUserAccess");
      this.BindNullableDateTime("@lastUserAccessFrom", lastUserAccessFrom);
      this.BindNullableDateTime("@lastUserAccessTo", lastUserAccessTo);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostPropertiesByLastUserAccess", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return resultCollection;
    }

    internal ResultCollection QueryServiceHosts(int databaseId, int maxResults)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostPropertiesByDatabaseId");
      this.BindInt("@databaseId", databaseId);
      this.BindInt("@maxResults", maxResults);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostPropertiesByDatabaseId", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return resultCollection;
    }

    internal ResultCollection QueryServiceHosts(int databaseId, int batchSize, Guid? minHostId)
    {
      if (this.Version < 18)
        throw new ServiceVersionNotSupportedException("HostManagement", this.Version, 18);
      this.PrepareStoredProcedure("prc_QueryServiceHostPropertiesByDatabaseIdBatched");
      this.BindInt("@databaseId", databaseId);
      this.BindGuid("@minHostId", minHostId ?? Guid.Empty);
      this.BindInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostPropertiesByDatabaseIdBatched", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return resultCollection;
    }

    internal ResultCollection QueryServiceHosts(
      TeamFoundationHostType hostType,
      Guid? minHostId,
      int? batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostPropertiesByHostType");
      this.BindInt("@hostType", (int) hostType);
      this.BindNullableGuid("@minHostId", minHostId);
      this.BindNullableInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostPropertiesByHostType", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return resultCollection;
    }

    internal int GetHostsCountByType(TeamFoundationHostType hostType)
    {
      this.PrepareStoredProcedure("prc_GetHostsCountByType");
      this.BindInt("@hostType", (int) hostType);
      return (int) this.ExecuteScalar();
    }

    internal ResultCollection QueryServiceHosts()
    {
      this.PrepareStoredProcedure("prc_QueryServiceHosts");
      ResultCollection resultCollection;
      try
      {
        resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      }
      catch (DatabaseConfigurationException ex)
      {
        string sqlStatement = "\n        SELECT  HostId,\n                ParentHostId,\n                Name,\n                Description,\n                [Status],\n                StatusReason,\n                SubStatus,\n                DatabaseId,\n                HostType,\n                LastUserAccess,\n                ServiceLevel,\n                StorageAccountId\n        FROM tbl_ServiceHost\n";
        if (this.Version < 5)
          sqlStatement = sqlStatement.Replace("StorageAccountId", "NULL as StorageAccountId");
        if (this.Version < 11)
          sqlStatement = sqlStatement.Replace("SubStatus", "NULL as SubStatus");
        this.PrepareSqlBatch(sqlStatement.Length);
        this.AddStatement(sqlStatement);
        resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      }
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return resultCollection;
    }

    internal ResultCollection QueryServiceHostHistory(int watermark)
    {
      if (this.Version < 6)
        return (ResultCollection) null;
      this.PrepareStoredProcedure("prc_QueryServiceHostHistory");
      this.BindInt("@watermark", watermark);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostHistory", this.RequestContext);
      resultCollection.AddBinder<ServiceHostHistoryEntry>(this.Version >= 7 ? (this.Version == 7 || this.Version == 8 ? (ObjectBinder<ServiceHostHistoryEntry>) new ServiceHostHistoryEntryBinder2() : (this.Version >= 15 ? (ObjectBinder<ServiceHostHistoryEntry>) new ServiceHostHistoryEntryBinder4() : (ObjectBinder<ServiceHostHistoryEntry>) new ServiceHostHistoryEntryBinder3())) : (ObjectBinder<ServiceHostHistoryEntry>) new ServiceHostHistoryEntryBinder());
      return resultCollection;
    }

    internal ResultCollection QueryServiceHostHistoryForTracing(int watermark)
    {
      if (this.Version < 8)
        return (ResultCollection) null;
      this.PrepareStoredProcedure("prc_QueryServiceHostHistoryForTracing");
      this.BindInt("@watermark", watermark);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostHistoryForTracing", this.RequestContext);
      resultCollection.AddBinder<ServiceHostHistoryEntry>(this.Version >= 15 ? (ObjectBinder<ServiceHostHistoryEntry>) new ServiceHostHistoryEntryBinder4() : (ObjectBinder<ServiceHostHistoryEntry>) new ServiceHostHistoryEntryBinder3());
      return resultCollection;
    }

    internal ResultCollection QueryServiceHostProperties(
      Guid hostId,
      ServiceHostFilterFlags filterFlags)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostPropertiesExtended");
      this.BindNullableGuid("@hostId", hostId);
      this.BindInt("@filterFlags", (int) filterFlags);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationServiceHostProperties>((ObjectBinder<TeamFoundationServiceHostProperties>) new TeamFoundationServiceHostPropertiesBinder());
      if ((filterFlags & ServiceHostFilterFlags.IncludeAllServicingDetails) != ServiceHostFilterFlags.None)
        resultCollection.AddBinder<TeamFoundationServiceHostProperties>((ObjectBinder<TeamFoundationServiceHostProperties>) new ScheduledServiceHostPropertiesBinder());
      if ((filterFlags & ServiceHostFilterFlags.IncludeProcessDetails) == ServiceHostFilterFlags.IncludeProcessDetails)
      {
        resultCollection.AddBinder<TeamFoundationServiceHostProcess>((ObjectBinder<TeamFoundationServiceHostProcess>) new TeamFoundationServiceHostProcessBinder());
        resultCollection.AddBinder<TeamFoundationServiceHostInstance>((ObjectBinder<TeamFoundationServiceHostInstance>) new TeamFoundationServiceHostInstanceBinder());
      }
      return resultCollection;
    }

    internal ResultCollection QueryServiceHostPropertiesByServiceLevel(
      string targetServiceLevel,
      TeamFoundationHostType hostType)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostPropertiesByServiceLevel", 900);
      this.BindString("@targetServiceLevel", targetServiceLevel, (int) byte.MaxValue, false, SqlDbType.VarChar);
      this.BindInt("@hostType", (int) hostType);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ServicingHostProperties>((ObjectBinder<ServicingHostProperties>) new ServicingHostPropertiesBinder());
      return resultCollection;
    }

    internal ResultCollection QueryServiceHostForExport(Guid afterHostIdMarker, int batchSize)
    {
      if (this.Version < 19)
        return (ResultCollection) null;
      this.PrepareStoredProcedure("prc_QueryServiceHostForExport");
      this.BindGuid("@afterHostIdMarker", afterHostIdMarker);
      this.BindInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostForExport", this.RequestContext);
      resultCollection.AddBinder<ServiceHostForExportEntry>((ObjectBinder<ServiceHostForExportEntry>) new ServiceHostForExportBinder());
      return resultCollection;
    }

    internal int NonVirtualHostCount()
    {
      if (this.Version < 20)
        return 0;
      this.PrepareStoredProcedure("prc_NonVirtualHostCount");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_NonVirtualHostCount", this.RequestContext);
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => reader.GetInt32(0))));
      return resultCollection.GetCurrent<int>().ToList<int>().FirstOrDefault<int>();
    }

    internal void BulkUpdateHostServiceLevel(
      int databaseId,
      TeamFoundationHostType hostType,
      string currentServiceLevel,
      string newServiceLevel)
    {
      this.PrepareStoredProcedure("prc_BulkUpdateHostServiceLevel", 1800);
      this.BindInt("@databaseId", databaseId);
      this.BindInt("@hostType", (int) hostType);
      this.BindString("@currentServiceLevel", currentServiceLevel, (int) byte.MaxValue, true, SqlDbType.VarChar);
      this.BindString("@newServiceLevel", newServiceLevel, (int) byte.MaxValue, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    internal ResultCollection QueryServiceHostPropertiesBootstrap(Guid hostId, bool includeChildren)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostProperties");
      this.BindNullableGuid("@hostId", hostId);
      this.BindBoolean("@includeChildren", includeChildren);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationServiceHostProperties>((ObjectBinder<TeamFoundationServiceHostProperties>) new ServiceHostPropertiesBootstrapBinder());
      return resultCollection;
    }

    internal ResultCollection QuerySqlConnectionInfoBootstrap(Guid hostId, bool includeChildren)
    {
      this.PrepareStoredProcedure("prc_QueryServiceHostProperties");
      this.BindNullableGuid("@hostId", hostId);
      this.BindBoolean("@includeChildren", includeChildren);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ISqlConnectionInfo>((ObjectBinder<ISqlConnectionInfo>) new SqlConnectionBootstrapBinder(this.ConnectionInfo));
      return resultCollection;
    }

    internal List<HostProperties> ResolveHost(Guid hostId)
    {
      string sqlStatement = "\n        DECLARE @parentHostId UNIQUEIDENTIFIER\n\n        SELECT  @parentHostId = sh.ParentHostId\n        FROM tbl_ServiceHost sh\n        WHERE sh.HostId = @hostId\n\n        SELECT  sh.HostId,\n                sh.ParentHostId,\n                sh.Name,\n                sh.Description,\n                sh.[Status],\n                sh.StatusReason,\n                sh.SubStatus,\n                sh.DatabaseId,\n                sh.HostType,\n                sh.LastUserAccess,\n                sh.ServiceLevel,\n                sh.StorageAccountId\n        FROM tbl_ServiceHost sh\n        WHERE sh.HostId = @hostId\n\n        UNION ALL\n\n        SELECT  sh.HostId,\n                sh.ParentHostId,\n                sh.Name,\n                sh.Description,\n                sh.[Status],\n                sh.StatusReason,\n                sh.SubStatus,\n                sh.DatabaseId,\n                sh.HostType,\n                sh.LastUserAccess,\n                sh.ServiceLevel,\n                sh.StorageAccountId\n        FROM tbl_ServiceHost sh\n        WHERE sh.HostId = @parentHostId\n\n        UNION ALL\n        \n        -- Add all the children as well\n        SELECT  sh.HostId,\n                sh.ParentHostId,\n                sh.Name,\n                sh.Description,\n                sh.[Status],\n                sh.StatusReason,\n                sh.SubStatus,\n                sh.DatabaseId,\n                sh.HostType,\n                sh.LastUserAccess,\n                sh.ServiceLevel,\n                sh.StorageAccountId\n        FROM tbl_ServiceHost sh\n        WHERE sh.ParentHostId = @hostId\n";
      if (this.Version < 4)
        sqlStatement = sqlStatement.Replace("sh.ServiceLevel", "NULL as ServiceLevel");
      if (this.Version < 5)
        sqlStatement = sqlStatement.Replace("sh.StorageAccountId", "NULL as StorageAccountId");
      if (this.Version < 11)
        sqlStatement = sqlStatement.Replace("sh.SubStatus", "NULL as SubStatus");
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindGuid("@hostId", hostId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostById", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return resultCollection.GetCurrent<HostProperties>().Items;
    }

    internal void StartServiceHost(Guid hostId, ServiceHostSubStatus? subStatus = null)
    {
      this.PrepareStoredProcedure("prc_StartServiceHost");
      this.BindGuid("@hostId", hostId);
      if (this.Version >= 11)
      {
        ServiceHostSubStatus? nullable = subStatus;
        this.BindNullableInt("@subStatus", nullable.HasValue ? new int?((int) nullable.GetValueOrDefault()) : new int?());
      }
      this.ExecuteNonQuery();
    }

    internal void StopServiceHost(Guid hostId, string reason, ServiceHostSubStatus subStatus)
    {
      this.PrepareStoredProcedure("prc_StopServiceHost");
      this.BindGuid("@hostId", hostId);
      this.BindString("@reason", reason, 2048, true, SqlDbType.NVarChar);
      if (this.Version >= 11)
        this.BindInt("@subStatus", (int) subStatus);
      this.ExecuteNonQuery();
    }

    internal void StartServiceHost(Guid[] hostIds)
    {
      this.PrepareStoredProcedure("prc_UpdateServiceHostStatusTVP");
      this.BindGuidTable(nameof (hostIds), (IEnumerable<Guid>) hostIds);
      this.BindInt("@fromState", 3);
      this.BindInt("@transitionState", 1);
      this.BindInt("@toState", 1);
      this.BindString("@reason", (string) null, 2048, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal void StopServiceHost(Guid[] hostIds, string reason)
    {
      this.PrepareStoredProcedure("prc_UpdateServiceHostStatusTVP");
      this.BindGuidTable(nameof (hostIds), (IEnumerable<Guid>) hostIds);
      this.BindInt("@fromState", 1);
      this.BindInt("@transitionState", 2);
      this.BindInt("@toState", 3);
      this.BindString("@reason", reason, 2048, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    internal TeamFoundationServiceHostStatus RegisterServiceHostInstance(
      Guid hostId,
      Guid machineId,
      string machineName,
      Guid processId,
      string processName,
      int osProcessId,
      string processIdentity,
      out DateTime startTime,
      out DateTime leaseExpiry)
    {
      this.PrepareStoredProcedure("prc_RegisterServiceHostInstance");
      this.BindGuid("@hostId", hostId);
      this.BindGuid("@processId", processId);
      this.BindString("@processName", processName, 128, false, SqlDbType.NVarChar);
      this.BindInt("@PID", osProcessId);
      this.BindString("@processIdentity", processIdentity, 256, false, SqlDbType.NVarChar);
      this.BindGuid("@machineId", machineId);
      this.BindString("@machineName", machineName, 128, false, SqlDbType.NVarChar);
      this.BindInt("@leaseDuration", 5);
      SqlParameter sqlParameter = this.BindDateTime("@startTime", DateTime.MinValue);
      sqlParameter.Direction = ParameterDirection.Output;
      leaseExpiry = DateTime.UtcNow.AddMinutes(5.0);
      int num = (int) this.ExecuteScalar();
      startTime = new DateTime(((DateTime) sqlParameter.Value).Ticks, DateTimeKind.Utc);
      return (TeamFoundationServiceHostStatus) num;
    }

    internal bool UpdateServiceHostInstance(
      Guid hostId,
      Guid processId,
      TeamFoundationServiceHostStatus status)
    {
      this.PrepareStoredProcedure("prc_UpdateServiceHostInstance");
      this.BindGuid("@hostId", hostId);
      this.BindGuid("@processId", processId);
      this.BindInt("@hostStatus", (int) status);
      return (int) this.ExecuteScalar() > 0;
    }

    internal void UnregisterServiceHostInstance(Guid processId, Guid hostId)
    {
      this.PrepareStoredProcedure("prc_UnregisterServiceHostInstance");
      this.BindGuid("@processId", processId);
      this.BindNullableGuid("@hostId", hostId);
      this.ExecuteNonQuery();
    }

    internal void UpdateMachineId(Guid processId, Guid machineId)
    {
      this.PrepareStoredProcedure("prc_UpdateMachineId");
      this.BindGuid("@processId", processId);
      this.BindGuid("@machineId", machineId);
      this.ExecuteNonQuery();
    }

    internal void RenewLease(Guid processId, int leaseDuration)
    {
      this.PrepareStoredProcedure("prc_RenewLease", 30);
      this.BindGuid("@processId", processId);
      this.BindInt("@leaseDuration", leaseDuration);
      this.ExecuteNonQuery();
    }

    internal virtual DateTime GetConfigDataTierTime()
    {
      this.PrepareSqlBatch(HostManagementComponent.s_getUtcDateStmt.Length);
      this.AddStatement(HostManagementComponent.s_getUtcDateStmt);
      return new DateTime(((DateTime) this.ExecuteScalar()).Ticks, DateTimeKind.Utc);
    }

    internal virtual void UpdateServiceHostInstances(
      DateTime dataTierTime,
      Guid processId,
      IEnumerable<Guid> registeredHosts)
    {
    }

    internal List<HostProperties> QueryStoppedServiceHosts()
    {
      if (this.Version < 10)
        return new List<HostProperties>(0);
      this.PrepareStoredProcedure("prc_QueryStoppedServiceHosts");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryStoppedServiceHosts", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return resultCollection.GetCurrent<HostProperties>().Items;
    }

    internal virtual IList<HostProperties> QueryChildrenServiceHostProperties(
      IList<Guid> parentHostIds)
    {
      if (parentHostIds.IsNullOrEmpty<Guid>())
        return (IList<HostProperties>) new List<HostProperties>();
      this.PrepareStoredProcedure("prc_QueryChildrenServiceHostProperties");
      this.BindGuidTable(nameof (parentHostIds), (IEnumerable<Guid>) parentHostIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryChildrenServiceHostProperties", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return (IList<HostProperties>) resultCollection.GetCurrent<HostProperties>().Items;
    }

    internal virtual IList<HostProperties> QueryServiceHostPropertiesBatch(IEnumerable<Guid> hostIds)
    {
      if (hostIds.IsNullOrEmpty<Guid>())
        return (IList<HostProperties>) new List<HostProperties>();
      this.PrepareStoredProcedure("prc_QueryServiceHostPropertiesBatch");
      this.BindGuidTable(nameof (hostIds), hostIds);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceHostPropertiesBatch", this.RequestContext);
      resultCollection.AddBinder<HostProperties>((ObjectBinder<HostProperties>) new HostPropertiesBinder());
      return (IList<HostProperties>) resultCollection.GetCurrent<HostProperties>().Items;
    }

    protected override string TraceArea => "HostManagement";
  }
}
