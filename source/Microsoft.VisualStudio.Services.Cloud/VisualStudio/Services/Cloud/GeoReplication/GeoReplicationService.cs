// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.GeoReplication.GeoReplicationService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Cloud.GeoReplication
{
  public class GeoReplicationService : IGeoReplicationService, IVssFrameworkService
  {
    private string m_currentInstanceName;
    private static readonly RegistryQuery s_secondaryAzureInstanceHost = (RegistryQuery) "/Configuration/GeoReplication/SecondaryAzureInstanceHostName";
    private VssRefreshCache<string> m_primaryInstanceName;
    private const string c_synchronousGeoDr = "SynchronousGeoDrEnabled";
    private const string c_area = "GeoReplication";
    private const string c_layer = "GeoReplicationService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_primaryInstanceName = new VssRefreshCache<string>(TimeSpan.FromSeconds(10.0), (Func<IVssRequestContext, string>) (rc => rc.GetService<ISqlRegistryService>().GetValue(rc, (RegistryQuery) "/Configuration/GeoReplication/PrimaryInstance", (string) null)), true);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<GeoReplica> QueryReplicas(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ITeamFoundationDatabaseProperties>(properties, nameof (properties));
      List<GeoReplica> geoReplicaList = (List<GeoReplica>) null;
      if (!requestContext.ServiceHost.IsProduction && requestContext.Items.ContainsKey("MockReplica"))
      {
        geoReplicaList = new List<GeoReplica>((IEnumerable<GeoReplica>) new GeoReplica[1]
        {
          new GeoReplica()
          {
            IsPrimary = true,
            LastReplication = DateTimeOffset.UtcNow,
            Link = Guid.NewGuid(),
            PartnerDatabase = properties.SqlConnectionInfo.InitialCatalog,
            PartnerServer = properties.SqlConnectionInfo.DataSource,
            ReplicationLagSeconds = 0,
            ReplicationState = "CATCH_UP",
            SecondaryAllowConnections = true
          }
        });
      }
      else
      {
        using (GeoReplicationComponent databaseComponent = requestContext.GetService<TeamFoundationResourceManagementService>().CreateDatabaseComponent<GeoReplicationComponent>(requestContext, properties.DatabaseId, "Default"))
          geoReplicaList = databaseComponent.QueryReplicas();
      }
      if (requestContext.ExecutionEnvironment.IsCloudDeployment)
      {
        foreach (GeoReplica geoReplica in geoReplicaList)
        {
          if (!geoReplica.PartnerServer.EndsWith(AzureDomainConstants.DatabaseWindowsNet, StringComparison.OrdinalIgnoreCase))
            geoReplica.PartnerServer += AzureDomainConstants.DatabaseWindowsNet;
        }
      }
      return geoReplicaList;
    }

    public void SetGeoReplicationState(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      bool synchronous)
    {
      if (synchronous)
      {
        List<GeoReplica> source = this.QueryReplicas(requestContext, properties);
        GeoReplica geoReplica = source.Count == 1 ? source.FirstOrDefault<GeoReplica>() : throw new InvalidOperationException(string.Format("Found {0} replicas", (object) source.Count));
        if (!geoReplica.IsPrimary)
          throw new InvalidOperationException("This command must be run against the primary database.");
        if (!string.Equals(geoReplica.ReplicationState, "CATCH_UP", StringComparison.OrdinalIgnoreCase))
          throw new InvalidOperationException("The replica is in state " + geoReplica.ReplicationState);
        if (geoReplica.ReplicationLagSeconds > 30)
          throw new InvalidOperationException(string.Format("The replica is lagging by {0}", (object) geoReplica.ReplicationLagSeconds));
      }
      using (ResourceManagementComponent2 databaseComponent = requestContext.GetService<TeamFoundationResourceManagementService>().CreateDatabaseComponent<ResourceManagementComponent2>(requestContext, properties.DatabaseId, "Default"))
      {
        if (synchronous)
          databaseComponent.SetResourceManagementSetting("SynchronousGeoDrEnabled", synchronous.ToString());
        else
          databaseComponent.DeleteResourceManagementSetting("SynchronousGeoDrEnabled");
      }
    }

    public string FailoverDatabase(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties,
      bool controlledFailOver,
      bool allowDataLoss)
    {
      List<GeoReplica> source = this.QueryReplicas(requestContext, properties);
      StringBuilder info = new StringBuilder();
      if (!source.Any<GeoReplica>((Func<GeoReplica, bool>) (gr => gr.IsPrimary)))
      {
        string str = source.First<GeoReplica>().PartnerServer + ";" + source.First<GeoReplica>().PartnerDatabase;
        DataTierInfo associatedDataTier = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, properties.ConnectionInfoWrapper.ConnectionString);
        if (allowDataLoss || !controlledFailOver && !this.IsGeoReplicationSychronous(requestContext, properties))
          throw new InvalidOperationException("Cannot failover database " + str + " because we are not doing synchronous writes but we are also not allowing data loss.");
        this.LogInfo(requestContext, info, "Failover " + str + " to instance " + associatedDataTier.DataSource + " starting.");
        using (TeamFoundationDataTierComponent componentRaw = associatedDataTier.ConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
          componentRaw.FailoverGeoReplicatedSqlAzureDatabase(properties.SqlConnectionInfo.InitialCatalog, !controlledFailOver);
        this.LogInfo(requestContext, info, "Failover " + str + " to instance " + associatedDataTier.DataSource + " completed.");
      }
      else
        this.LogInfo(requestContext, info, "Database " + properties.DatabaseName + " is already the primary.");
      if (!controlledFailOver)
        new RetryManager(3, TimeSpan.FromSeconds(30.0), (Action<Exception>) (exception => this.LogInfo(requestContext, info, exception.ToReadableStackTrace()))).Invoke((Action) (() => this.SetGeoReplicationState(requestContext, properties, false)));
      return info.ToString();
    }

    private bool IsGeoReplicationSychronous(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      using (ResourceManagementComponent2 databaseComponent = requestContext.GetService<TeamFoundationResourceManagementService>().CreateDatabaseComponent<ResourceManagementComponent2>(requestContext, properties.DatabaseId, "Default"))
      {
        ResourceManagementSetting managementSetting = databaseComponent.GetResourceManagementSetting("SynchronousGeoDrEnabled");
        bool result;
        return ((managementSetting == null ? 0 : (bool.TryParse(managementSetting.Value, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0;
      }
    }

    private void LogInfo(IVssRequestContext requestContext, StringBuilder info, string message)
    {
      info.AppendLine(string.Format("Timestamp: {0}, {1}", (object) DateTime.Now, (object) message));
      requestContext.TraceAlways(960673, TraceLevel.Warning, "GeoReplication", nameof (GeoReplicationService), message);
    }

    public void WaitForDatabaseCopy(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties properties)
    {
      using (GeoReplicationComponent databaseComponent = requestContext.GetService<TeamFoundationResourceManagementService>().CreateDatabaseComponent<GeoReplicationComponent>(requestContext, properties.DatabaseId, "Default"))
        databaseComponent.WaitForDatabaseCopy();
    }

    public bool IsPrimaryInstance(IVssRequestContext requestContext)
    {
      if (this.GetGeoReplicationMode(requestContext) == GeoReplicationMode.All)
      {
        string b = this.m_primaryInstanceName.Get(requestContext);
        if (string.IsNullOrEmpty(b))
          requestContext.Trace(922722, TraceLevel.Error, "GeoReplication", nameof (GeoReplicationService), "The required 'PrimaryGeoReplicationInstance' configuration setting is missing.");
        return b != null && string.Equals(this.CurrentInstanceName, b, StringComparison.OrdinalIgnoreCase);
      }
      if (this.GetGeoReplicationMode(requestContext) != GeoReplicationMode.PartitionDb)
        return true;
      bool? nullable = requestContext.GetService<IVssRegistryService>().GetValue<bool?>(requestContext, (RegistryQuery) "/Configuration/GeoReplication/IsPrimary", new bool?());
      if (!nullable.HasValue)
        requestContext.Trace(922722, TraceLevel.Error, "GeoReplication", nameof (GeoReplicationService), "The required 'IsPrimaryGeoReplicationInstance' configuration setting is missing.");
      return nullable.GetValueOrDefault();
    }

    public virtual GeoReplicationMode GetGeoReplicationMode(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled(FrameworkServerConstants.ConfigDbReplicationEnabled))
        return GeoReplicationMode.All;
      return requestContext.IsFeatureEnabled(FrameworkServerConstants.DatabaseReplication) ? GeoReplicationMode.PartitionDb : GeoReplicationMode.None;
    }

    public string GetSecondaryAzureInstanceHost(IVssRequestContext requestContext) => this.GetGeoReplicationMode(requestContext) == GeoReplicationMode.All ? requestContext.GetService<IVssRegistryService>().GetValue(requestContext, in GeoReplicationService.s_secondaryAzureInstanceHost) : (string) null;

    private string CurrentInstanceName
    {
      get
      {
        if (this.m_currentInstanceName == null)
          this.m_currentInstanceName = AzureRoleUtil.GetOverridableConfigurationSetting("HostedServiceName");
        return this.m_currentInstanceName;
      }
    }
  }
}
