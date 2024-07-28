// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.AdministrationComponent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class AdministrationComponent : BuildSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<AdministrationComponent>(1),
      (IComponentCreator) new ComponentCreator<AdministrationComponent2>(2),
      (IComponentCreator) new ComponentCreator<AdministrationComponent3>(3),
      (IComponentCreator) new ComponentCreator<AdministrationComponent4>(4),
      (IComponentCreator) new ComponentCreator<AdministrationComponent5>(5),
      (IComponentCreator) new ComponentCreator<AdministrationComponent6>(6),
      (IComponentCreator) new ComponentCreator<AdministrationComponent7>(7)
    }, "BuildAdministration", "Build");

    protected override string TraceArea => "BuildAdministration";

    internal virtual BuildServiceHost AddBuildServiceHost(BuildServiceHost serviceHost)
    {
      this.TraceEnter(0, nameof (AddBuildServiceHost));
      this.PrepareStoredProcedure("prc_AddBuildServiceHost");
      this.BindItemName("@displayName", serviceHost.Name, 256, false);
      this.BindUri("@baseUrl", DBHelper.ServerUrlToDBUrl(serviceHost.BaseUrl), false);
      this.BindBoolean("@requireClientCertificates", serviceHost.RequireClientCertificates);
      BuildServiceHostBinder serviceHostBinder = new BuildServiceHostBinder(this.ExecuteReader(), this.ProcedureName);
      serviceHostBinder.MoveNext();
      this.TraceLeave(0, nameof (AddBuildServiceHost));
      return serviceHostBinder.Current;
    }

    internal ResultCollection AddBuildAgents(IList<BuildAgent> agents)
    {
      this.TraceEnter(0, nameof (AddBuildAgents));
      this.PrepareStoredProcedure("prc_AddBuildAgents");
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      List<KeyValuePair<int, string>> rows = new List<KeyValuePair<int, string>>();
      for (int i = 0; i < agents.Count; i++)
      {
        agents[i].Uri = DBHelper.CreateArtifactUri("Agent", i);
        this.Trace(0, TraceLevel.Info, "Created uri for agent '{0}'", (object) agents[i].Uri);
        if (agents[i].Tags.Count > 0)
          rows.AddRange((IEnumerable<KeyValuePair<int, string>>) agents[i].Tags.Select<string, KeyValuePair<int, string>>((System.Func<string, KeyValuePair<int, string>>) (x => new KeyValuePair<int, string>(i, x))).ToArray<KeyValuePair<int, string>>());
        properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateEmptyArtifactSpec(BuildPropertyKinds.BuildAgent, i), (IEnumerable<PropertyValue>) agents[i].Properties));
      }
      this.BindTable<BuildAgent>("@buildAgentTable", (TeamFoundationTableValueParameter<BuildAgent>) new BuildAgentTable((ICollection<BuildAgent>) agents));
      this.BindTable<KeyValuePair<int, string>>("@buildAgentTagTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) new KeyValuePairInt32StringTable((IEnumerable<KeyValuePair<int, string>>) rows));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildAgent);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      this.TraceLeave(0, nameof (AddBuildAgents));
      return resultCollection;
    }

    internal virtual ResultCollection AddBuildControllers(IList<BuildController> controllers)
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
      this.BindTable<BuildController>("@buildControllerTable", (TeamFoundationTableValueParameter<BuildController>) new BuildControllerTable((ICollection<BuildController>) controllers));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      this.TraceLeave(0, nameof (AddBuildControllers));
      return resultCollection;
    }

    internal void DeleteBuildAgents(IList<string> agentUris, ArtifactKind agentArtifactKind)
    {
      this.TraceEnter(0, nameof (DeleteBuildAgents));
      this.PrepareStoredProcedure("prc_DeleteBuildAgents");
      this.BindTable<int>("@agentIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) agentUris));
      this.BindInt("@compactKindId", agentArtifactKind.CompactKindId);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildAgents));
    }

    internal void DeleteBuildControllers(
      IList<string> controllerUris,
      ArtifactKind controllerArtifactKind)
    {
      this.TraceEnter(0, nameof (DeleteBuildControllers));
      this.PrepareStoredProcedure("prc_DeleteBuildControllers");
      this.BindTable<int>("@controllerIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) controllerUris));
      this.BindInt("@compactKindId", controllerArtifactKind.CompactKindId);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildControllers));
    }

    internal void DeleteBuildServiceHost(
      string serviceHostUri,
      ArtifactKind agentArtifactKind,
      ArtifactKind controllerArtifactKind)
    {
      this.TraceEnter(0, nameof (DeleteBuildServiceHost));
      this.PrepareStoredProcedure("prc_DeleteBuildServiceHost");
      this.BindItemUriToInt32("@serviceHostId", serviceHostUri);
      this.BindInt("@agentArtifactId", agentArtifactKind.CompactKindId);
      this.BindInt("@controllerArtifactId", controllerArtifactKind.CompactKindId);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (DeleteBuildServiceHost));
    }

    internal virtual ResultCollection QueryBuildAgents(BuildAgentSpec spec)
    {
      this.TraceEnter(0, nameof (QueryBuildAgents));
      this.PrepareStoredProcedure("prc_QueryBuildAgents");
      this.BindItemName("@agentName", spec.Name, 256, false);
      this.BindItemName("@serviceHostName", spec.ServiceHostName, 256, false);
      this.BindItemName("@controllerName", spec.ControllerName, 256, false);
      this.BindTable<string>("@tagFilterTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) spec.Tags, false, 256));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildAgents));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildAgentsByUri(IList<string> agentUris)
    {
      this.TraceEnter(0, nameof (QueryBuildAgentsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildAgentsByUri");
      this.BindTable<int>("agentIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) agentUris));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildAgentsByUri));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildControllers(BuildControllerSpec spec)
    {
      this.TraceEnter(0, nameof (QueryBuildControllers));
      this.PrepareStoredProcedure("prc_QueryBuildControllers");
      this.BindItemName("@controllerName", spec.Name, 256, false);
      this.BindItemName("@serviceHostName", spec.ServiceHostName, 256, false);
      this.BindBoolean("@includeAgents", spec.IncludeAgents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildControllers));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildControllersByUri(
      IList<string> controllerUris,
      bool includeAgents)
    {
      this.TraceEnter(0, nameof (QueryBuildControllersByUri));
      this.PrepareStoredProcedure("prc_QueryBuildControllersByUri");
      this.BindTable<int>("@controllerIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) controllerUris));
      this.BindBoolean("@includeAgents", includeAgents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      this.TraceLeave(0, nameof (QueryBuildControllersByUri));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildServiceHosts(string name)
    {
      this.TraceEnter(0, nameof (QueryBuildServiceHosts));
      this.PrepareStoredProcedure("prc_QueryBuildServiceHosts");
      this.BindItemName("@serviceHostName", name, 256, false);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      this.TraceLeave(0, nameof (QueryBuildServiceHosts));
      return resultCollection;
    }

    internal virtual ResultCollection QueryBuildServiceHostsByUri(IList<string> serviceHostUris)
    {
      this.TraceEnter(0, nameof (QueryBuildServiceHostsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildServiceHostsByUri");
      this.BindTable<int>("@serviceHostIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) serviceHostUris));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      this.TraceLeave(0, nameof (QueryBuildServiceHostsByUri));
      return resultCollection;
    }

    internal ResultCollection UpdateBuildAgents(IList<BuildAgentUpdateOptions> updates)
    {
      this.TraceEnter(0, nameof (UpdateBuildAgents));
      this.PrepareStoredProcedure("prc_UpdateBuildAgents");
      List<ArtifactPropertyValue> properties = new List<ArtifactPropertyValue>();
      List<KeyValuePair<int, string>> rows = new List<KeyValuePair<int, string>>();
      foreach (BuildAgentUpdateOptions update in (IEnumerable<BuildAgentUpdateOptions>) updates)
      {
        if ((update.Fields & BuildAgentUpdate.AttachedProperties) == BuildAgentUpdate.AttachedProperties)
          properties.Add(new ArtifactPropertyValue(ArtifactHelper.CreateArtifactSpec(update.Uri), (IEnumerable<PropertyValue>) update.AttachedProperties));
        if ((update.Fields & BuildAgentUpdate.Tags) == BuildAgentUpdate.Tags)
        {
          int agentId = int.Parse(DBHelper.ExtractDbId(update.Uri), (IFormatProvider) CultureInfo.InvariantCulture);
          rows.AddRange((IEnumerable<KeyValuePair<int, string>>) update.Tags.Select<string, KeyValuePair<int, string>>((System.Func<string, KeyValuePair<int, string>>) (x => new KeyValuePair<int, string>(agentId, x))).ToArray<KeyValuePair<int, string>>());
        }
      }
      this.BindTable<BuildAgentUpdateOptions>("@buildAgentTable", (TeamFoundationTableValueParameter<BuildAgentUpdateOptions>) new BuildAgentUpdateTable((ICollection<BuildAgentUpdateOptions>) updates));
      this.BindTable<KeyValuePair<int, string>>("@buildAgentTagTable", (TeamFoundationTableValueParameter<KeyValuePair<int, string>>) new KeyValuePairInt32StringTable((IEnumerable<KeyValuePair<int, string>>) rows));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildAgent);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildAgents));
      return resultCollection;
    }

    internal virtual ResultCollection UpdateBuildControllers(
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
      this.BindTable<BuildControllerUpdateOptions>("@buildControllerTable", (TeamFoundationTableValueParameter<BuildControllerUpdateOptions>) new BuildControllerUpdateTable((ICollection<BuildControllerUpdateOptions>) updates));
      this.BindArtifactPropertyValueList("@propertyValueList", (IEnumerable<ArtifactPropertyValue>) properties);
      this.BindGuid("@artifactKindId", BuildPropertyKinds.BuildController);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildControllers));
      return resultCollection;
    }

    internal virtual void UpdateBuildServiceHost(BuildServiceHostUpdateOptions update)
    {
      this.TraceEnter(0, nameof (UpdateBuildServiceHost));
      this.PrepareStoredProcedure("prc_UpdateBuildServiceHost");
      this.BindItemUriToInt32("@serviceHostId", update.Uri);
      if ((update.Fields & BuildServiceHostUpdate.BaseUrl) == BuildServiceHostUpdate.BaseUrl)
        this.BindUri("@baseUrl", DBHelper.ServerUrlToDBUrl(update.BaseUrl), false);
      else
        this.BindNullValue("@baseUrl", SqlDbType.VarChar);
      if ((update.Fields & BuildServiceHostUpdate.Name) == BuildServiceHostUpdate.Name)
        this.BindItemName("@displayName", update.Name, 256, false);
      else
        this.BindNullValue("@displayName", SqlDbType.NVarChar);
      if ((update.Fields & BuildServiceHostUpdate.RequireClientCertificates) == BuildServiceHostUpdate.RequireClientCertificates)
        this.BindBoolean("@requireClientCertificates", update.RequireClientCertificates);
      else
        this.BindNullValue("@requireClientCertificates", SqlDbType.Bit);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (UpdateBuildServiceHost));
    }

    internal virtual ResultCollection UpdateBuildServiceHostStatus(
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
      string str;
      if (!(queueAddress == (Uri) null))
        str = queueAddress.AbsoluteUri.TrimEnd('/');
      else
        str = (string) null;
      this.BindUri("@queueAddress", str, true);
      if (sequenceId.HasValue)
        this.BindInt("@sequenceId", sequenceId.Value);
      else
        this.BindNullValue("@sequenceId", SqlDbType.Int);
      this.BindByte("@newStatus", (byte) status);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      this.TraceLeave(0, nameof (UpdateBuildServiceHostStatus));
      return resultCollection;
    }

    internal void VerifyBuildServiceHostOwnership(int serviceHostId, Guid ownerId)
    {
      this.TraceEnter(800005, nameof (VerifyBuildServiceHostOwnership));
      this.PrepareStoredProcedure("prc_VerifyBuildServiceHostOwnership");
      this.BindInt("@serviceHostId", serviceHostId);
      this.BindGuid("@ownerId", ownerId);
      this.ExecuteNonQuery();
      this.TraceLeave(800006, nameof (VerifyBuildServiceHostOwnership));
    }

    internal virtual ResultCollection ReserveBuildAgent(
      string buildUri,
      Guid buildProjectId,
      string name,
      IList<string> requiredTags,
      TagComparison tagComparison)
    {
      this.TraceEnter(0, nameof (ReserveBuildAgent));
      this.PrepareStoredProcedure("prc_ReserveBuildAgent");
      this.BindItemUriToInt32("@buildId", buildUri);
      this.BindItemName("@agentName", name, 256, false);
      this.BindTable<string>("@requiredTagTable", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) requiredTags, false, 256));
      this.BindByte("@tagComparison", (byte) tagComparison);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<AgentReservation>((ObjectBinder<AgentReservation>) new AgentReservationBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      this.TraceLeave(0, nameof (ReserveBuildAgent));
      return resultCollection;
    }

    internal ResultCollection ReleaseBuildAgent(int reservationId)
    {
      this.TraceEnter(0, nameof (ReleaseBuildAgent));
      this.PrepareStoredProcedure("prc_ReleaseBuildAgent");
      this.BindInt("@reservationId", reservationId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      this.TraceLeave(0, nameof (ReleaseBuildAgent));
      return resultCollection;
    }

    internal ResultCollection QuerySharedResources(IList<string> resourceNames)
    {
      this.TraceEnter(0, nameof (QuerySharedResources));
      this.PrepareStoredProcedure("prc_QuerySharedResources");
      if (resourceNames.Count == 1 && BuildCommonUtil.IsStar(resourceNames[0]))
      {
        this.BindBoolean("@allResources", true);
      }
      else
      {
        this.BindTable<string>("@resourceNames", (TeamFoundationTableValueParameter<string>) new StringTable((IEnumerable<string>) resourceNames, false, 256));
        this.BindBoolean("@allResources", false);
      }
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Microsoft.TeamFoundation.Build.Server.SharedResource>((ObjectBinder<Microsoft.TeamFoundation.Build.Server.SharedResource>) new SharedResourceBinder());
      resultCollection.AddBinder<SharedResourceRequest>((ObjectBinder<SharedResourceRequest>) new SharedResourceRequestBinder());
      this.TraceLeave(0, nameof (QuerySharedResources));
      return resultCollection;
    }

    internal virtual ResultCollection RequestSharedResourceLock(
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
        this.BindString("@buildUri", DBHelper.ExtractDbId(buildUri), 256, false, SqlDbType.NVarChar);
      else
        this.BindNullValue("@buildUri", SqlDbType.NVarChar);
      if (!string.IsNullOrEmpty(requestBuildUri))
        this.BindString("@requestBuildUri", requestBuildUri, 256, false, SqlDbType.NVarChar);
      else
        this.BindNullValue("@requestBuildUri", SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<SharedResourceData>((ObjectBinder<SharedResourceData>) new SharedResourceDataBinder());
      this.TraceLeave(0, nameof (RequestSharedResourceLock));
      return resultCollection;
    }

    internal virtual bool TryRequestSharedResourceLock(
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
        this.BindString("@buildUri", DBHelper.ExtractDbId(buildUri), 256, false, SqlDbType.NVarChar);
      else
        this.BindNullValue("@buildUri", SqlDbType.NVarChar);
      this.TraceLeave(0, nameof (TryRequestSharedResourceLock));
      return (int) this.ExecuteScalar() == 0;
    }

    internal ResultCollection ReleaseSharedResourceLock(
      string resourceName,
      string instanceId,
      string requestedBy)
    {
      this.TraceEnter(0, nameof (ReleaseSharedResourceLock));
      this.PrepareStoredProcedure("prc_ReleaseSharedResourceLock");
      this.BindString("@resourceName", resourceName, 256, true, SqlDbType.NVarChar);
      this.BindString("@instanceId", instanceId, 64, true, SqlDbType.NVarChar);
      this.BindString("@requestedBy", requestedBy, 256, false, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<SharedResourceData>((ObjectBinder<SharedResourceData>) new SharedResourceDataBinder());
      this.TraceLeave(0, nameof (ReleaseSharedResourceLock));
      return resultCollection;
    }
  }
}
