// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GeoReplication.GeoReplicationContextProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.DatabaseReplication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server.GeoReplication
{
  public class GeoReplicationContextProvider : IDatabaseReplicationContextProvider
  {
    private DatabaseReplicationConfiguration m_replicationConfiguration;

    public void Initialize(
      IVssRequestContext requestContext,
      DatabaseReplicationConfiguration configuration)
    {
      this.m_replicationConfiguration = configuration;
    }

    public DatabaseReplicationContext GetDatabaseReplicationContext(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      ISqlConnectionInfo dataTierConnectionInfo,
      ITFLogger logger)
    {
      List<GeoReplica> source = requestContext.GetService<IGeoReplicationService>().QueryReplicas(requestContext, databaseProperties);
      if (source.Count == 0)
        return DatabaseReplicationContext.Default;
      if (source.SingleOrDefault<GeoReplica>((Func<GeoReplica, bool>) (x => !x.IsPrimary)) != null)
        return new DatabaseReplicationContext(false);
      DatabaseReplicationContext replicationContext = (DatabaseReplicationContext) new GeoReplicationContextProvider.GeoDatabaseReplicationContext(true, databaseProperties);
      foreach (GeoReplica geoReplica in source)
      {
        ReplicationPartner partnerForServer = this.GetSecondaryPartnerForServer(geoReplica.PartnerServer);
        ITeamFoundationDatabaseProperties propertiesOnSecondary = this.GetDatabasePropertiesOnSecondary(requestContext, partnerForServer, geoReplica.PartnerServer, geoReplica.PartnerDatabase);
        replicationContext.Replicas.Add((IDatabaseReplicaInfo) new GeoReplicationContextProvider.GeoReplicaDbInfo()
        {
          DatabaseName = geoReplica.PartnerDatabase,
          DataTierConnectionInfo = partnerForServer.DataTierConnectionInfo,
          SecondaryDatabaseId = propertiesOnSecondary.DatabaseId,
          SecondaryPartner = partnerForServer,
          Provider = this,
          ReplicationState = geoReplica.ReplicationState,
          ReplicationLagSeconds = geoReplica.ReplicationLagSeconds
        });
      }
      return replicationContext;
    }

    private ReplicationPartner GetSecondaryPartnerForServer(string server) => ((IEnumerable<ReplicationPartner>) this.m_replicationConfiguration.Partners).Single<ReplicationPartner>((Func<ReplicationPartner, bool>) (x => !x.IsLocal));

    private ITeamFoundationDatabaseProperties GetDatabasePropertiesOnSecondary(
      IVssRequestContext requestContext,
      ReplicationPartner secondaryPartner,
      string datasource,
      string initialCatalog)
    {
      List<InternalDatabaseProperties> items;
      using (DatabaseManagementComponent componentRaw = secondaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
      {
        using (ResultCollection resultCollection = componentRaw.QueryDatabases())
          items = resultCollection.GetCurrent<InternalDatabaseProperties>().Items;
      }
      foreach (InternalDatabaseProperties propertiesOnSecondary in items)
      {
        if (propertiesOnSecondary.ConnectionInfoWrapper != null)
        {
          SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(propertiesOnSecondary.ConnectionInfoWrapper.ConnectionString);
          if (VssStringComparer.DataSource.Equals(this.FormatDataSource(connectionStringBuilder.DataSource), this.FormatDataSource(datasource)) && VssStringComparer.DatabaseName.Equals(connectionStringBuilder.InitialCatalog, initialCatalog))
            return (ITeamFoundationDatabaseProperties) propertiesOnSecondary;
        }
      }
      throw new DatabaseNotFoundException("Could not resolve database on secondary ConfigDb. Server: " + datasource + ". Database: " + initialCatalog + ".");
    }

    private string FormatDataSource(string dataSource)
    {
      DataSourceOptions options = DataSourceOptions.RemoveProtocol | DataSourceOptions.RemoveDefaultSqlPort;
      return TeamFoundationDataTierService.ManipulateDataSource(dataSource, options);
    }

    private TeamFoundationDatabaseCredential RegisterCredentialOnSecondary(
      IVssRequestContext requestContext,
      ReplicationPartner secondaryPartner,
      int secondaryDatabaseId,
      string loginName,
      string loginPassword,
      string credentialName,
      ITFLogger logger)
    {
      Guid databaseSigningKeyRaw = TeamFoundationSigningService.GetDatabaseSigningKeyRaw(secondaryPartner.ConfigDbConnectionInfo);
      byte[] passwordEncrypted = TeamFoundationSigningService.EncryptRaw(secondaryPartner.ConfigDbConnectionInfo, databaseSigningKeyRaw, Encoding.UTF8.GetBytes(loginPassword), SigningAlgorithm.SHA256);
      using (DatabaseCredentialsComponent componentRaw = secondaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DatabaseCredentialsComponent>())
        return componentRaw.RegisterDatabaseCredential(secondaryDatabaseId, loginName, passwordEncrypted, databaseSigningKeyRaw, true, credentialName, (string) null);
    }

    private bool UpdateCredentialOnSecondary(
      IVssRequestContext requestContext,
      ReplicationPartner secondaryPartner,
      TeamFoundationDatabaseCredential secondaryCredential,
      ITFLogger logger)
    {
      using (DatabaseCredentialsComponent componentRaw = secondaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DatabaseCredentialsComponent>())
        ((DatabaseCredentialsComponent5) componentRaw).UpdateDatabaseCredential(secondaryCredential);
      return true;
    }

    private bool UnregisterCredentialOnSecondary(
      IVssRequestContext requestContext,
      ReplicationPartner secondaryPartner,
      int secondaryDatabaseId,
      TeamFoundationDatabaseCredential primaryCredential,
      ITFLogger logger)
    {
      using (DatabaseCredentialsComponent componentRaw = secondaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DatabaseCredentialsComponent>())
      {
        TeamFoundationDatabaseCredential databaseCredential = componentRaw.QueryDatabaseCredentials(secondaryDatabaseId).FirstOrDefault<TeamFoundationDatabaseCredential>((Func<TeamFoundationDatabaseCredential, bool>) (cred => cred.UserId.Equals(primaryCredential.UserId, StringComparison.OrdinalIgnoreCase) && cred.Name.Equals(primaryCredential.Name, StringComparison.OrdinalIgnoreCase)));
        if (databaseCredential != null)
        {
          if (databaseCredential.CredentialStatus != TeamFoundationDatabaseCredentialStatus.PendingDelete)
          {
            logger.Error("The secondary credential is not in the PendingDelete state which is unexpected. Cannot delete! Id: {0}. UserId: {1}. Status: {0}", (object) databaseCredential.Id, (object) databaseCredential.UserId, (object) databaseCredential.CredentialStatus);
            return false;
          }
          componentRaw.RemoveDatabaseCredentials((IEnumerable<int>) new int[1]
          {
            databaseCredential.Id
          });
        }
        return true;
      }
    }

    private void UpdateDatabasePropertiesOnSecondary(
      IVssRequestContext requestContext,
      ReplicationPartner secondaryPartner,
      int secondaryDatabaseId,
      ITFLogger logger,
      Action<TeamFoundationDatabaseProperties> action)
    {
      InternalDatabaseProperties database;
      using (DatabaseManagementComponent componentRaw = secondaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
        database = componentRaw.GetDatabase(secondaryDatabaseId);
      TeamFoundationDatabaseProperties editableProperties = database.GetEditableProperties();
      action(editableProperties);
      using (DatabaseManagementComponent componentRaw = secondaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
        componentRaw.UpdateDatabase(editableProperties);
    }

    private class GeoDatabaseReplicationContext : DatabaseReplicationContext
    {
      private ITeamFoundationDatabaseProperties m_properties;

      public GeoDatabaseReplicationContext(
        bool isPrimary,
        ITeamFoundationDatabaseProperties properties)
        : base(isPrimary)
      {
        this.m_properties = properties;
      }

      public override void WaitForDatabaseCopy(IVssRequestContext requestContext)
      {
        if (!this.IsPrimary || this.Replicas.Count <= 0)
          return;
        requestContext.GetService<IGeoReplicationService>().WaitForDatabaseCopy(requestContext, this.m_properties);
      }
    }

    private class GeoReplicaDbInfo : IDatabaseReplicaInfo
    {
      public string DatabaseName { get; set; }

      public ISqlConnectionInfo DataTierConnectionInfo { get; set; }

      public TeamFoundationDatabaseCredential NewCredential { get; set; }

      public int SecondaryDatabaseId { get; set; }

      public ReplicationPartner SecondaryPartner { get; set; }

      public GeoReplicationContextProvider Provider { get; set; }

      public string ReplicationState { get; set; }

      public int ReplicationLagSeconds { get; set; }

      public bool RegisterCredential(
        IVssRequestContext requestContext,
        string loginName,
        string loginPassword,
        string credentialName,
        ITFLogger logger)
      {
        this.NewCredential = this.Provider.RegisterCredentialOnSecondary(requestContext, this.SecondaryPartner, this.SecondaryDatabaseId, loginName, loginPassword, credentialName, logger);
        return true;
      }

      public bool UpdateCredential(IVssRequestContext requestContext, ITFLogger logger) => this.Provider.UpdateCredentialOnSecondary(requestContext, this.SecondaryPartner, this.NewCredential, logger);

      public bool UnregisterCredential(
        IVssRequestContext requestContext,
        TeamFoundationDatabaseCredential credential,
        ITFLogger logger)
      {
        return this.Provider.UnregisterCredentialOnSecondary(requestContext, this.SecondaryPartner, this.SecondaryDatabaseId, credential, logger);
      }

      public void UpdateDatabaseProperties(
        IVssRequestContext requestContext,
        ITFLogger logger,
        Action<TeamFoundationDatabaseProperties> action)
      {
        this.Provider.UpdateDatabasePropertiesOnSecondary(requestContext, this.SecondaryPartner, this.SecondaryDatabaseId, logger, action);
      }
    }
  }
}
