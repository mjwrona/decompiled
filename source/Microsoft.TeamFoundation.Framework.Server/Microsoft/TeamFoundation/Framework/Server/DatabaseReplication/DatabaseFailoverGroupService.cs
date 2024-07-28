// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseReplication.DatabaseFailoverGroupService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.DatabaseReplication
{
  public class DatabaseFailoverGroupService : IDatabaseFailoverGroupService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public DatabaseReplicationContext GetDatabaseReplicationContext(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      ISqlConnectionInfo dataTierConnectionInfo = null,
      ITFLogger logger = null)
    {
      logger = logger ?? (ITFLogger) new TraceLogger();
      if (requestContext.GetService<IGeoReplicationService>().GetGeoReplicationMode(requestContext) == GeoReplicationMode.PartitionDb)
      {
        logger.Warning("Partition db only geo replication feature is on, we can't use failover group service");
        return DatabaseReplicationContext.Default;
      }
      ArgumentUtility.CheckForNull<ITeamFoundationDatabaseProperties>(databaseProperties, nameof (databaseProperties));
      if (dataTierConnectionInfo == null)
        dataTierConnectionInfo = requestContext.GetService<TeamFoundationDataTierService>().FindAssociatedDataTier(requestContext, databaseProperties.SqlConnectionInfo.ConnectionString).ConnectionInfo;
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(databaseProperties.ConnectionInfoWrapper.ConnectionString);
      List<GeoReplica> replicas;
      using (GeoReplicationRawComponent componentRaw = dataTierConnectionInfo.CloneReplaceInitialCatalog(connectionStringBuilder.InitialCatalog).CreateComponentRaw<GeoReplicationRawComponent>())
        replicas = componentRaw.QueryReplicas();
      return this.GenerateDatabaseReplicationContext(requestContext, replicas, databaseProperties, dataTierConnectionInfo, logger);
    }

    private DatabaseReplicationContext GenerateDatabaseReplicationContext(
      IVssRequestContext requestContext,
      List<GeoReplica> replicas,
      ITeamFoundationDatabaseProperties databaseProperties,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITFLogger logger)
    {
      if (replicas.Count == 0)
        return DatabaseReplicationContext.Default;
      if (replicas.SingleOrDefault<GeoReplica>((Func<GeoReplica, bool>) (x => !x.IsPrimary)) != null)
        return new DatabaseReplicationContext(false);
      DatabaseFailoverGroupService.FailoverGroupReplicationContext replicationContext = new DatabaseFailoverGroupService.FailoverGroupReplicationContext(true, databaseProperties);
      using (TeamFoundationDataTierComponent componentRaw = dataTierConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        replicationContext.PrimaryServerName = componentRaw.GetServerName();
      string dataSource = TeamFoundationDataTierService.GetDataSource(dataTierConnectionInfo.ConnectionString, DataSourceOptions.RemoveProtocolAndDomain) + AzureDomainConstants.SecondaryDatabaseWindowsNet;
      ISqlConnectionInfo connectionInfo1 = dataTierConnectionInfo.CloneReplaceDataSource(dataSource);
      ISqlConnectionInfo connectionInfo2 = connectionInfo1.CloneReplaceInitialCatalog(databaseProperties.SqlConnectionInfo.InitialCatalog);
      string secondaryServerName = (string) null;
      try
      {
        using (TeamFoundationDataTierComponent componentRaw = connectionInfo2.CreateComponentRaw<TeamFoundationDataTierComponent>(30))
          secondaryServerName = componentRaw.GetServerName();
      }
      catch
      {
        logger.Warning("Cannot connect to secondary listener " + dataSource + ", Most likely this is not set up as a failover group");
        return DatabaseReplicationContext.Default;
      }
      GeoReplica geoReplica = replicas.Where<GeoReplica>((Func<GeoReplica, bool>) (r => r.PartnerServer.Equals(secondaryServerName, StringComparison.InvariantCultureIgnoreCase))).SingleOrDefault<GeoReplica>((Func<GeoReplica, bool>) (r => r.IsPrimary));
      if (geoReplica == null)
      {
        string str = string.Join(",", replicas.Select<GeoReplica, string>((Func<GeoReplica, string>) (r => r.PartnerServer)));
        logger.Error("There is no replica match name of secondary server name " + secondaryServerName + ". Replicas from the system view : " + str);
      }
      replicationContext.Replicas.Add((IDatabaseReplicaInfo) new DatabaseFailoverGroupService.FailoverGroupReplicaDbInfo()
      {
        DatabaseName = databaseProperties.SqlConnectionInfo.InitialCatalog,
        ServerName = (secondaryServerName + AzureDomainConstants.DatabaseWindowsNet),
        DataTierConnectionInfo = connectionInfo1,
        ReplicationState = (geoReplica?.ReplicationState ?? string.Empty),
        ReplicationLagSeconds = (geoReplica != null ? geoReplica.ReplicationLagSeconds : -1)
      });
      return (DatabaseReplicationContext) replicationContext;
    }

    private class FailoverGroupReplicationContext : DatabaseReplicationContext
    {
      private ITeamFoundationDatabaseProperties m_properties;

      public FailoverGroupReplicationContext(
        bool isPrimary,
        ITeamFoundationDatabaseProperties properties)
        : base(isPrimary)
      {
        this.m_properties = properties;
      }

      public override void WaitForDatabaseCopy(IVssRequestContext requestContext)
      {
        if (!this.IsPrimary || this.Replicas.Count <= 0 || this.m_properties.Status == TeamFoundationDatabaseStatus.Creating)
          return;
        requestContext.GetService<IGeoReplicationService>().WaitForDatabaseCopy(requestContext, this.m_properties);
      }
    }

    public class FailoverGroupReplicaDbInfo : IDatabaseReplicaInfo
    {
      public string DatabaseName { get; set; }

      public string ServerName { get; set; }

      public ISqlConnectionInfo DataTierConnectionInfo { get; set; }

      public TeamFoundationDatabaseCredential NewCredential { get; }

      public string ReplicationState { get; set; }

      public int ReplicationLagSeconds { get; set; }

      public bool RegisterCredential(
        IVssRequestContext requestContext,
        string loginName,
        string loginPassword,
        string credentialName,
        ITFLogger logger)
      {
        return true;
      }

      public bool UpdateCredential(IVssRequestContext requestContext, ITFLogger logger) => true;

      public bool UnregisterCredential(
        IVssRequestContext requestContext,
        TeamFoundationDatabaseCredential credential,
        ITFLogger logger)
      {
        return true;
      }

      public void UpdateDatabaseProperties(
        IVssRequestContext requestContext,
        ITFLogger logger,
        Action<TeamFoundationDatabaseProperties> action)
      {
      }
    }
  }
}
