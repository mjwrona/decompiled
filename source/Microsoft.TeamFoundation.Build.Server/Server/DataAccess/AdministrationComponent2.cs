// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.AdministrationComponent2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal class AdministrationComponent2 : AdministrationComponent
  {
    public AdministrationComponent2() => this.ServiceVersion = ServiceVersion.V2;

    internal override BuildServiceHost AddBuildServiceHost(BuildServiceHost serviceHost)
    {
      this.TraceEnter(0, nameof (AddBuildServiceHost));
      this.PrepareStoredProcedure("prc_AddBuildServiceHost");
      this.BindItemName("@displayName", serviceHost.Name, 256, false);
      this.BindUri("@baseUrl", DBHelper.ServerUrlToDBUrl(serviceHost.BaseUrl), false);
      this.BindBoolean("@requireClientCertificates", serviceHost.RequireClientCertificates);
      this.BindBoolean("@isElastic", serviceHost.IsVirtual);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
        this.TraceLeave(0, nameof (AddBuildServiceHost));
        return resultCollection.GetCurrent<BuildServiceHost>().First<BuildServiceHost>();
      }
    }

    internal override ResultCollection QueryBuildAgents(BuildAgentSpec spec)
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
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      this.TraceLeave(0, nameof (QueryBuildAgents));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildAgentsByUri(IList<string> agentUris)
    {
      this.TraceEnter(0, nameof (QueryBuildAgentsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildAgentsByUri");
      this.BindTable<int>("@agentIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) agentUris));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
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
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
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
      this.BindTable<int>("@controllerIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) controllerUris));
      this.BindBoolean("@includeAgents", includeAgents);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
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
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      this.TraceLeave(0, nameof (QueryBuildServiceHosts));
      return resultCollection;
    }

    internal override ResultCollection QueryBuildServiceHostsByUri(IList<string> serviceHostUris)
    {
      this.TraceEnter(0, nameof (QueryBuildServiceHostsByUri));
      this.PrepareStoredProcedure("prc_QueryBuildServiceHostsByUri");
      this.BindTable<int>("@serviceHostIdTable", (TeamFoundationTableValueParameter<int>) BuildSqlResourceComponent.UrisToInt32Table((IEnumerable<string>) serviceHostUris));
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), int.MaxValue, this.ProcedureName, new SqlExceptionHandler(((TeamFoundationSqlResourceComponent) this).MapException), this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<BuildAgent>((ObjectBinder<BuildAgent>) new BuildAgentBinder());
      this.TraceLeave(0, nameof (QueryBuildServiceHostsByUri));
      return resultCollection;
    }

    internal override void UpdateBuildServiceHost(BuildServiceHostUpdateOptions update)
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
      this.BindNullValue("@isAllocationPending", SqlDbType.Bit);
      Guid? serviceIdentityId = update.ServiceIdentityId;
      if (serviceIdentityId.HasValue)
      {
        serviceIdentityId = update.ServiceIdentityId;
        this.BindGuid("@serviceIdentityId", serviceIdentityId.Value);
      }
      else
        this.BindNullValue("@serviceIdentityId", SqlDbType.UniqueIdentifier);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (UpdateBuildServiceHost));
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
      if (sequenceId.HasValue)
        this.BindInt("@sequenceId", sequenceId.Value);
      else
        this.BindNullValue("@sequenceId", SqlDbType.Int);
      this.BindByte("@newStatus", (byte) status);
      this.BindBoolean("@recursive", recursive);
      this.BindBoolean("@clearOwner", clearOwner);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<BuildServiceHost>((ObjectBinder<BuildServiceHost>) new BuildServiceHostBinder2());
      resultCollection.AddBinder<BuildController>((ObjectBinder<BuildController>) new BuildControllerBinder());
      resultCollection.AddBinder<KeyValuePair<Guid, Guid>>((ObjectBinder<KeyValuePair<Guid, Guid>>) new ServiceHostOwnershipBinder());
      resultCollection.AddBinder<StartBuildData>((ObjectBinder<StartBuildData>) new StartBuildDataBinder());
      resultCollection.AddBinder<AgentReservationData>((ObjectBinder<AgentReservationData>) new AgentReservationDataBinder());
      this.TraceLeave(0, nameof (UpdateBuildServiceHostStatus));
      return resultCollection;
    }
  }
}
