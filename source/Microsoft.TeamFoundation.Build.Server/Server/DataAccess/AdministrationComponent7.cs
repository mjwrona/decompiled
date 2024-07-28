// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.AdministrationComponent7
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class AdministrationComponent7 : AdministrationComponent6
  {
    protected static SqlMetaData[] typ_BuildControllerTableV2 = new SqlMetaData[9]
    {
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("ServiceHostId", SqlDbType.Int),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("MaxConcurrentBuilds", SqlDbType.Int),
      new SqlMetaData("CustomAssemblyPath", SqlDbType.NVarChar, 400L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Enabled", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L)
    };
    protected static SqlMetaData[] typ_BuildControllerUpdateTableV2 = new SqlMetaData[9]
    {
      new SqlMetaData("ControllerId", SqlDbType.Int),
      new SqlMetaData("Fields", SqlDbType.Int),
      new SqlMetaData("DisplayName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("MaxConcurrentBuilds", SqlDbType.Int),
      new SqlMetaData("CustomAssemblyPath", SqlDbType.NVarChar, 400L),
      new SqlMetaData("Status", SqlDbType.TinyInt),
      new SqlMetaData("StatusMessage", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Enabled", SqlDbType.Bit),
      new SqlMetaData("Description", SqlDbType.NVarChar, 2048L)
    };

    public AdministrationComponent7()
    {
      this.ServiceVersion = ServiceVersion.V7;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal override ResultCollection AddBuildControllers(IList<BuildController> controllers)
    {
      this.TraceEnter(0, nameof (AddBuildControllers));
      this.PrepareStoredProcedure("prc_AddBuildControllers");
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      for (int index = 0; index < controllers.Count; ++index)
      {
        controllers[index].Uri = DBHelper.CreateArtifactUri("Controller", index);
        this.Trace(0, TraceLevel.Info, "Created uri for controller '{0}'", (object) controllers[index].Uri);
        properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateEmptyArtifactSpec(BuildPropertyKinds.BuildController, index), (IEnumerable<PropertyValue>) controllers[index].Properties));
      }
      this.BindBuildControllerTable("@buildControllerTable", (IEnumerable<BuildController>) controllers);
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      this.TraceLeave(0, nameof (AddBuildControllers));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildAgents(BuildAgentSpec spec)
    {
      this.TraceEnter(0, nameof (QueryBuildAgents));
      this.PrepareStoredProcedure("prc_QueryBuildAgents");
      this.BindItemName("@agentName", spec.Name, 256, false);
      this.BindItemName("@serviceHostName", spec.ServiceHostName, 256, false);
      this.BindItemName("@controllerName", spec.ControllerName, 256, false);
      this.BindStringTable("@tagFilterTable", (IEnumerable<string>) spec.Tags, maxLength: 256);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildAgents));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildAgentsByUri(IList<string> agentUris)
    {
      this.TraceEnter(0, nameof (QueryBuildAgentsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildAgentsByUri");
      this.BindUrisToInt32Table("@agentIdTable", (IEnumerable<string>) agentUris);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildAgentsByUri));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildControllers(BuildControllerSpec spec)
    {
      this.TraceEnter(0, nameof (QueryBuildControllers));
      this.PrepareStoredProcedure("prc_QueryBuildControllers");
      this.BindItemName("@controllerName", spec.Name, 256, false);
      this.BindItemName("@serviceHostName", spec.ServiceHostName, 256, false);
      this.BindBoolean("@includeAgents", spec.IncludeAgents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildControllers));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildControllersByUri(
      IList<string> controllerUris,
      bool includeAgents)
    {
      this.TraceEnter(0, nameof (QueryBuildControllersByUri));
      this.PrepareStoredProcedure("prc_QueryBuildControllersByUri");
      this.BindUrisToInt32Table("@controllerIdTable", (IEnumerable<string>) controllerUris);
      this.BindBoolean("@includeAgents", includeAgents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildControllersByUri));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildServiceHosts(string name)
    {
      this.TraceEnter(0, nameof (QueryBuildServiceHosts));
      this.PrepareStoredProcedure("prc_QueryBuildServiceHosts");
      this.BindItemName("@serviceHostName", name, 256, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      this.TraceLeave(0, nameof (QueryBuildServiceHosts));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildServiceHostsByUri(IList<string> serviceHostUris)
    {
      this.TraceEnter(0, nameof (QueryBuildServiceHostsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildServiceHostsByUri");
      this.BindUrisToInt32Table("@serviceHostIdTable", (IEnumerable<string>) serviceHostUris);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      this.TraceLeave(0, nameof (QueryBuildServiceHostsByUri));
      return resultCollection;
    }

    internal override ResultCollection RequestSharedResourceLock(
      string resourceName,
      string instanceId,
      string requestedBy,
      string buildUri,
      Guid buildProjectId,
      string requestBuildUri,
      Guid requestBuildProjectId)
    {
      this.TraceEnter(0, nameof (RequestSharedResourceLock));
      this.PrepareStoredProcedure("prc_RequestSharedResourceLock");
      this.BindString("@resourceName", resourceName, 256, false, SqlDbType.NVarChar);
      this.BindString("@instanceId", instanceId, 64, false, SqlDbType.NVarChar);
      this.BindString("@requestedBy", requestedBy, 256, false, SqlDbType.NVarChar);
      if (!string.IsNullOrEmpty(buildUri))
      {
        this.BindString("@buildUri", DBHelper.ExtractDbId(buildUri), 256, false, SqlDbType.NVarChar);
        this.BindInt("@buildDataspaceId", this.GetDataspaceId(buildProjectId));
      }
      else
      {
        this.BindNullValue("@buildUri", SqlDbType.NVarChar);
        this.BindNullValue("@buildDataspaceId", SqlDbType.Int);
      }
      if (!string.IsNullOrEmpty(requestBuildUri))
      {
        this.BindString("@requestBuildUri", requestBuildUri, 256, false, SqlDbType.NVarChar);
        this.BindInt("@requestBuildDataspaceId", this.GetDataspaceId(requestBuildProjectId));
      }
      else
      {
        this.BindNullValue("@requestBuildUri", SqlDbType.NVarChar);
        this.BindNullValue("@requestBuildDataspaceId", SqlDbType.Int);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<SharedResourceData>((ObjectBinder<SharedResourceData>) new SharedResourceDataBinder());
      this.TraceLeave(0, nameof (RequestSharedResourceLock));
      return resultCollection;
    }

    internal override ResultCollection ReserveBuildAgent(
      string buildUri,
      Guid buildProjectId,
      string name,
      IList<string> requiredTags,
      TagComparison tagComparison)
    {
      this.TraceEnter(0, nameof (ReserveBuildAgent));
      this.PrepareStoredProcedure("prc_ReserveBuildAgent");
      this.BindItemUriToInt32("@buildId", buildUri);
      this.BindInt("@buildDataspaceId", this.GetDataspaceId(buildProjectId));
      this.BindItemName("@agentName", name, 256, false);
      this.BindStringTable("@requiredTagTable", (IEnumerable<string>) requiredTags, maxLength: 256);
      this.BindByte("@tagComparison", (byte) tagComparison);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<AgentReservation>((ObjectBinder<AgentReservation>) new AgentReservationBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      this.TraceLeave(0, nameof (ReserveBuildAgent));
      return resultCollection;
    }

    internal override bool TryRequestSharedResourceLock(
      string resourceName,
      string instanceId,
      string requestedBy,
      string buildUri,
      Guid buildProjectId)
    {
      this.TraceEnter(0, nameof (TryRequestSharedResourceLock));
      this.PrepareStoredProcedure("prc_TryRequestSharedResourceLock");
      this.BindString("@resourceName", resourceName, 256, false, SqlDbType.NVarChar);
      this.BindString("@instanceId", instanceId, 64, false, SqlDbType.NVarChar);
      this.BindString("@requestedBy", requestedBy, 256, false, SqlDbType.NVarChar);
      if (!string.IsNullOrEmpty(buildUri))
      {
        this.BindString("@buildUri", DBHelper.ExtractDbId(buildUri), 256, false, SqlDbType.NVarChar);
        this.BindInt("@buildDataspaceId", this.GetDataspaceId(buildProjectId));
      }
      else
      {
        this.BindNullValue("@buildUri", SqlDbType.NVarChar);
        this.BindNullValue("@buildDataspaceId", SqlDbType.Int);
      }
      this.TraceLeave(0, nameof (TryRequestSharedResourceLock));
      return (int) this.ExecuteScalar() == 0;
    }

    internal override ResultCollection UpdateBuildControllers(
      IList<BuildControllerUpdateOptions> updates)
    {
      this.TraceEnter(0, nameof (UpdateBuildControllers));
      this.PrepareStoredProcedure("prc_UpdateBuildControllers");
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      foreach (BuildControllerUpdateOptions update in (IEnumerable<BuildControllerUpdateOptions>) updates)
      {
        if ((update.Fields & BuildControllerUpdate.AttachedProperties) == BuildControllerUpdate.AttachedProperties)
          properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateArtifactSpec(update.Uri), (IEnumerable<PropertyValue>) update.AttachedProperties));
      }
      this.BindBuildControllerUpdateTable("@buildControllerTable", (IEnumerable<BuildControllerUpdateOptions>) updates);
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildControllers));
      return resultCollection;
    }

    internal override ResultCollection UpdateBuildServiceHostStatus(
      string uri,
      ServiceHostStatus status,
      Guid? ownerId,
      string ownerName,
      int? sequenceId,
      bool recursive,
      bool clearOwner = false,
      Uri queueAddress = null)
    {
      this.TraceEnter(0, nameof (UpdateBuildServiceHostStatus));
      this.PrepareStoredProcedure("prc_UpdateBuildServiceHostStatus");
      this.BindItemUriToInt32("@serviceHostId", uri);
      if (ownerId.HasValue)
        this.BindGuid("@ownerId", ownerId.Value);
      else
        this.BindNullValue("@ownerId", SqlDbType.UniqueIdentifier);
      this.BindString("@ownerName", ownerName, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      if (sequenceId.HasValue)
        this.BindInt("@sequenceId", sequenceId.Value);
      else
        this.BindNullValue("@sequenceId", SqlDbType.Int);
      this.BindByte("@newStatus", (byte) status);
      this.BindBoolean("@recursive", recursive);
      this.BindBoolean("@clearOwner", clearOwner);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder2((BuildSqlResourceComponent) this));
      resultCollection.AddBinder<KeyValuePair<Guid, Guid>>((ObjectBinder<KeyValuePair<Guid, Guid>>) new ServiceHostOwnershipBinder());
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      resultCollection.AddBinder<WorkflowCancellationData>((ObjectBinder<WorkflowCancellationData>) new WorkflowCancellationDataBinder());
      resultCollection.AddBinder<WorkflowCancellationData>((ObjectBinder<WorkflowCancellationData>) new WorkflowCancellationDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildServiceHostStatus));
      return resultCollection;
    }

    protected virtual SqlParameter BindBuildControllerTable(
      string parameterName,
      IEnumerable<BuildController> rows)
    {
      return this.BindTable(parameterName, "typ_BuildControllerTableV2", (rows ?? Enumerable.Empty<BuildController>()).Select<BuildController, SqlDataRecord>(new System.Func<BuildController, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindBuildControllerUpdateTable(
      string parameterName,
      IEnumerable<BuildControllerUpdateOptions> rows)
    {
      return this.BindTable(parameterName, "typ_BuildControllerUpdateTableV2", (rows ?? Enumerable.Empty<BuildControllerUpdateOptions>()).Select<BuildControllerUpdateOptions, SqlDataRecord>(new System.Func<BuildControllerUpdateOptions, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(BuildController row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(AdministrationComponent7.typ_BuildControllerTableV2);
      sqlDataRecord.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.Uri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetInt32(1, int.Parse(DBHelper.ExtractDbId(row.ServiceHostUri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetString(2, DBHelper.ServerPathToDBPath(row.Name));
      sqlDataRecord.SetInt32(3, row.MaxConcurrentBuilds);
      if (!string.IsNullOrEmpty(row.CustomAssemblyPath))
        sqlDataRecord.SetString(4, this.VersionControlPathToDataspaceDBPath(row.CustomAssemblyPath));
      else
        sqlDataRecord.SetDBNull(4);
      sqlDataRecord.SetByte(5, (byte) row.Status);
      sqlDataRecord.SetString(6, row.StatusMessage ?? string.Empty);
      sqlDataRecord.SetBoolean(7, row.Enabled);
      if (!string.IsNullOrEmpty(row.Description))
        sqlDataRecord.SetString(8, row.Description);
      else
        sqlDataRecord.SetDBNull(8);
      return sqlDataRecord;
    }

    protected SqlDataRecord ConvertToSqlDataRecord(BuildControllerUpdateOptions row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(AdministrationComponent7.typ_BuildControllerUpdateTableV2);
      sqlDataRecord.SetInt32(0, int.Parse(DBHelper.ExtractDbId(row.Uri), (IFormatProvider) CultureInfo.InvariantCulture));
      sqlDataRecord.SetInt32(1, (int) row.Fields);
      if ((row.Fields & BuildControllerUpdate.Name) == BuildControllerUpdate.Name)
        sqlDataRecord.SetString(2, DBHelper.ServerPathToDBPath(row.Name));
      else
        sqlDataRecord.SetDBNull(2);
      if ((row.Fields & BuildControllerUpdate.MaxConcurrentBuilds) == BuildControllerUpdate.MaxConcurrentBuilds)
        sqlDataRecord.SetInt32(3, row.MaxConcurrentBuilds);
      else
        sqlDataRecord.SetDBNull(3);
      if ((row.Fields & BuildControllerUpdate.CustomAssemblyPath) == BuildControllerUpdate.CustomAssemblyPath && !string.IsNullOrEmpty(row.CustomAssemblyPath))
        sqlDataRecord.SetString(4, this.VersionControlPathToDataspaceDBPath(row.CustomAssemblyPath));
      else
        sqlDataRecord.SetDBNull(4);
      if ((row.Fields & BuildControllerUpdate.Status) == BuildControllerUpdate.Status)
        sqlDataRecord.SetByte(5, (byte) row.Status);
      else
        sqlDataRecord.SetDBNull(5);
      if ((row.Fields & BuildControllerUpdate.StatusMessage) == BuildControllerUpdate.StatusMessage)
        sqlDataRecord.SetString(6, row.StatusMessage ?? string.Empty);
      else
        sqlDataRecord.SetDBNull(6);
      if ((row.Fields & BuildControllerUpdate.Enabled) == BuildControllerUpdate.Enabled)
        sqlDataRecord.SetBoolean(7, row.Enabled);
      else
        sqlDataRecord.SetDBNull(7);
      if ((row.Fields & BuildControllerUpdate.Description) == BuildControllerUpdate.Description && !string.IsNullOrEmpty(row.Description))
        sqlDataRecord.SetString(8, row.Description);
      else
        sqlDataRecord.SetDBNull(8);
      return sqlDataRecord;
    }
  }
}
