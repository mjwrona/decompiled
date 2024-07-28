// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.GeoReplication.GeoReplicationConfigurationHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Sql;
using Azure.ResourceManager.Sql.Models;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.DatabaseReplication;
using Microsoft.TeamFoundation.Framework.Server.GeoReplication;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.GeoReplication
{
  public static class GeoReplicationConfigurationHelper
  {
    public static void ClonePrimaryDataspacePartitions(
      IVssRequestContext requestContext,
      Func<string, ISqlConnectionInfo> createAADConnection,
      string dataspaceCategory,
      ReplicationPartner primaryPartner,
      ITFLogger logger,
      bool waitForDatabasesToCopy = true)
    {
      requestContext.CheckHostedDeployment();
      Guid databaseSigningKey = requestContext.GetService<ITeamFoundationSigningService>().GetDatabaseSigningKey(requestContext);
      Guid databaseSigningKeyRaw = TeamFoundationSigningService.GetDatabaseSigningKeyRaw(primaryPartner.ConfigDbConnectionInfo);
      DataTierInfo dataTierInfo = requestContext.GetService<TeamFoundationDataTierService>().GetDataTiers(requestContext, false).FirstOrDefault<DataTierInfo>((Func<DataTierInfo, bool>) (dt => dt.State == DataTierState.Active));
      string str = TeamFoundationDataTierService.ManipulateDataSource(dataTierInfo.DataSource, DataSourceOptions.RemoveProtocolAndDomain);
      foreach (GeoReplicationConfigurationHelper.DatabaseItems databaseItems in (IEnumerable<GeoReplicationConfigurationHelper.DatabaseItems>) GeoReplicationConfigurationHelper.GetDatabasesToReplicate(primaryPartner).OrderBy<GeoReplicationConfigurationHelper.DatabaseItems, int>((Func<GeoReplicationConfigurationHelper.DatabaseItems, int>) (idp => idp.DatabaseProperties.DatabaseId)))
      {
        string loginName = SqlAzureLoginGenerator.CreateLoginName("GeoRep-");
        string loginPassword = SqlAzureLoginGenerator.CreateLoginPassword(loginName);
        GeoReplicationConfigurationHelper.CreateTemporarySqlLogin(requestContext, dataTierInfo.ConnectionInfo, (Func<TeamFoundationSqlSecurityComponent>) (() => createAADConnection(dataTierInfo.ConnectionInfo.ConnectionString).CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger)), loginName, loginPassword, logger);
        GeoReplicationConfigurationHelper.CreateTemporarySqlLogin(requestContext, primaryPartner.DataTierConnectionInfo, (Func<TeamFoundationSqlSecurityComponent>) (() => primaryPartner.DataTierConnectionInfo.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger)), loginName, loginPassword, logger);
        using (TeamFoundationSqlSecurityComponent componentRaw = primaryPartner.DataTierConnectionInfo.CloneReplaceInitialCatalog(GeoReplicationConfigurationHelper.GetNameOfDatabase(databaseItems.DatabaseProperties)).CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
        {
          string user = componentRaw.CreateUser(loginName);
          componentRaw.AddRoleMember("db_owner", user);
        }
        byte[] bytes = Encoding.UTF8.GetBytes(loginPassword);
        ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(primaryPartner.DataTierConnectionInfo.ConnectionString, loginName, bytes.ToSecureString(), bytes);
        logger.Info("Setting up replica for database " + databaseItems.DatabaseProperties.DatabaseName + " on " + str);
        ArmClient armClient = AzureRoleUtil.GetArmClient(requestContext);
        string configurationSetting1 = AzureRoleUtil.GetOverridableConfigurationSetting("AzureSubscriptionId");
        string configurationSetting2 = AzureRoleUtil.GetOverridableConfigurationSetting("HostedServiceName");
        string nameOfDatabase = GeoReplicationConfigurationHelper.GetNameOfDatabase(databaseItems.DatabaseProperties);
        ResourceIdentifier resourceIdentifier1 = SqlDatabaseResource.CreateResourceIdentifier(configurationSetting1, configurationSetting2, TeamFoundationDataTierService.ManipulateDataSource(databaseItems.DatabaseProperties.GetActualServerName(requestContext), DataSourceOptions.RemoveProtocolAndDomain), nameOfDatabase);
        SqlDatabaseResource databaseResource = Response<SqlDatabaseResource>.op_Implicit(SqlExtensions.GetSqlDatabaseResource(armClient, resourceIdentifier1).Get(new CancellationToken()));
        if (databaseResource.Data.Sku.Tier == "Basic" || databaseResource.Data.Sku.Tier == "Standard")
        {
          logger.Error("Unable to geo-replicate database " + nameOfDatabase + " since it is a " + databaseResource.Data.Sku.Tier + " database.");
          return;
        }
        ResourceIdentifier resourceIdentifier2 = SqlServerResource.CreateResourceIdentifier(configurationSetting1, configurationSetting2, str);
        SqlServerResource sqlServerResource = Response<SqlServerResource>.op_Implicit(SqlExtensions.GetSqlServerResource(armClient, resourceIdentifier2).Get((string) null, new CancellationToken()));
        SqlDatabaseData sqlDatabaseData = new SqlDatabaseData(((TrackedResourceData) sqlServerResource.Data).Location)
        {
          SourceDatabaseId = ((ArmResource) databaseResource).Id,
          CreateMode = new SqlDatabaseCreateMode?(SqlDatabaseCreateMode.Secondary),
          LicenseType = new DatabaseLicenseType?(DatabaseLicenseType.BasePrice),
          IsZoneRedundant = databaseResource.Data.IsZoneRedundant
        };
        try
        {
          sqlServerResource.GetSqlDatabases().CreateOrUpdate((WaitUntil) 0, nameOfDatabase, sqlDatabaseData, new CancellationToken());
          if (!databaseResource.Data.Sku.Tier.Equals("Hyperscale", StringComparison.OrdinalIgnoreCase))
          {
            logger.Info("Enabling ZR on non-HS database {0} in server {1} in resource group {2}", (object) nameOfDatabase, (object) str, (object) configurationSetting2);
            string configurationSetting3 = AzureRoleUtil.GetOverridableConfigurationSetting("ResourceManagerAadTenantId");
            string configurationSetting4 = AzureRoleUtil.GetOverridableConfigurationSetting("ResourceManagerEndpointUrl");
            new SqlManagementResourceHelper(Guid.Parse(configurationSetting1), configurationSetting3, configurationSetting4, logger).EnableDatabaseZoneRedundancy(configurationSetting2, str, nameOfDatabase);
          }
        }
        catch (Exception ex)
        {
          logger.Error("Stacktrace: {0}", (object) ex.ToReadableStackTrace());
          throw;
        }
        using (TeamFoundationDataTierComponent componentRaw = connectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
        {
          databaseItems.UserSid = componentRaw.GetDatatierLoginSid(databaseItems.DatabaseProperties.ConnectionInfoWrapper.UserId);
          databaseItems.DataTierSid = componentRaw.GetDatatierLoginSid(databaseItems.DatabaseProperties.DboConnectionInfoWrapper.UserId);
        }
        byte[] login1 = GeoReplicationConfigurationHelper.CreateLogin(requestContext, dataTierInfo.ConnectionInfo, primaryPartner.ConfigDbConnectionInfo, databaseItems.DatabaseProperties.ConnectionInfoWrapper.UserId, databaseItems.DatabaseProperties.ConnectionInfoWrapper.PasswordEncrypted, databaseItems.UserSid, databaseSigningKeyRaw, logger);
        byte[] login2 = GeoReplicationConfigurationHelper.CreateLogin(requestContext, dataTierInfo.ConnectionInfo, primaryPartner.ConfigDbConnectionInfo, databaseItems.DatabaseProperties.DboConnectionInfoWrapper.UserId, databaseItems.DatabaseProperties.DboConnectionInfoWrapper.PasswordEncrypted, databaseItems.DataTierSid, databaseSigningKeyRaw, logger);
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(databaseItems.DatabaseProperties.ConnectionInfoWrapper.ConnectionString);
        connectionStringBuilder.DataSource = dataTierInfo.DataSource;
        TeamFoundationDatabaseManagementService service1 = requestContext.GetService<TeamFoundationDatabaseManagementService>();
        ITeamFoundationDatabaseProperties properties = service1.RegisterDatabase(requestContext, connectionStringBuilder.ConnectionString, dataTierInfo.DataSource + ";" + GeoReplicationConfigurationHelper.GetNameOfDatabase(databaseItems.DatabaseProperties), databaseItems.DatabaseProperties.ServiceLevel, databaseItems.DatabaseProperties.PoolName, databaseItems.DatabaseProperties.Tenants, databaseItems.DatabaseProperties.MaxTenants, databaseItems.DatabaseProperties.Status, new DateTime?(databaseItems.DatabaseProperties.StatusChangedDate), databaseItems.DatabaseProperties.StatusReason, new DateTime?(databaseItems.DatabaseProperties.LastTenantAdded), true, databaseItems.DatabaseProperties.ConnectionInfoWrapper.UserId, login1, TeamFoundationDatabaseFlags.None);
        TeamFoundationDatabaseCredential credential;
        using (DatabaseCredentialsComponent component = requestContext.CreateComponent<DatabaseCredentialsComponent>())
          credential = component.RegisterDatabaseCredential(properties.DatabaseId, databaseItems.DatabaseProperties.DboConnectionInfoWrapper.UserId, login2, databaseSigningKey, false, DatabaseCredentialNames.DbOwnerCredential, (string) null);
        credential.CredentialStatus = TeamFoundationDatabaseCredentialStatus.InUse;
        service1.UpdateCredential(requestContext, credential);
        IDataspaceService service2 = requestContext.GetService<IDataspaceService>();
        foreach (Dataspace dataspace in databaseItems.Dataspaces)
          service2.CreateDataspace(requestContext, dataspaceCategory, dataspace.DataspaceIdentifier, properties.DatabaseId);
        if (waitForDatabasesToCopy)
          requestContext.GetService<IGeoReplicationService>().WaitForDatabaseCopy(requestContext, properties);
        GeoReplicationConfigurationHelper.RemoveTemporarySqlLogin(dataTierInfo.ConnectionInfo, loginName, logger);
        GeoReplicationConfigurationHelper.RemoveTemporarySqlLogin(primaryPartner.DataTierConnectionInfo, loginName, logger);
      }
      DataspacePartitionMap dataspacePartitionMap;
      using (DataspacePartitionComponent componentRaw = primaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DataspacePartitionComponent>())
      {
        componentRaw.PartitionId = DatabasePartitionConstants.DeploymentHostPartitionId;
        dataspacePartitionMap = componentRaw.GetDataspacePartitionMap(dataspaceCategory);
      }
      requestContext.GetService<IDataspacePartitionService>().SaveDataspacePartitionMap(requestContext, dataspacePartitionMap);
    }

    private static List<GeoReplicationConfigurationHelper.DatabaseItems> GetDatabasesToReplicate(
      ReplicationPartner primaryPartner)
    {
      List<GeoReplicationConfigurationHelper.DatabaseItems> list;
      using (DatabaseManagementComponent componentRaw = primaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DatabaseManagementComponent>())
        list = componentRaw.QueryDatabases(DatabaseManagementConstants.DefaultPartitionPoolName).Select<InternalDatabaseProperties, GeoReplicationConfigurationHelper.DatabaseItems>((Func<InternalDatabaseProperties, GeoReplicationConfigurationHelper.DatabaseItems>) (idp => new GeoReplicationConfigurationHelper.DatabaseItems()
        {
          DatabaseProperties = idp
        })).ToList<GeoReplicationConfigurationHelper.DatabaseItems>();
      using (DataspaceComponent componentRaw = primaryPartner.ConfigDbConnectionInfo.CreateComponentRaw<DataspaceComponent>())
      {
        componentRaw.PartitionId = DatabasePartitionConstants.DeploymentHostPartitionId;
        foreach (Dataspace queryDataspace in componentRaw.QueryDataspaces())
        {
          Dataspace ds = queryDataspace;
          list.FirstOrDefault<GeoReplicationConfigurationHelper.DatabaseItems>((Func<GeoReplicationConfigurationHelper.DatabaseItems, bool>) (di => di.DatabaseProperties.DatabaseId == ds.DatabaseId))?.Dataspaces.Add(ds);
        }
      }
      return list;
    }

    private static byte[] CreateLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo dataTierConnection,
      ISqlConnectionInfo primaryConfigurationDatabaseConnection,
      string userId,
      string passwordRemoteEncrypted,
      byte[] sid,
      Guid remoteSigningKey,
      ITFLogger logger)
    {
      byte[] numArray = TeamFoundationSigningService.DecryptRaw(primaryConfigurationDatabaseConnection, remoteSigningKey, Convert.FromBase64String(passwordRemoteEncrypted), SigningAlgorithm.SHA256);
      requestContext.GetService<TeamFoundationDatabaseManagementService>().CreateSqlLogin(requestContext, dataTierConnection, userId, Encoding.UTF8.GetString(numArray), true, logger, sid);
      ITeamFoundationSigningService service = requestContext.GetService<ITeamFoundationSigningService>();
      return service.Encrypt(requestContext, service.GetDatabaseSigningKey(requestContext), numArray, SigningAlgorithm.SHA256);
    }

    private static void CreateTemporarySqlLogin(
      IVssRequestContext requestContext,
      ISqlConnectionInfo connection,
      Func<TeamFoundationSqlSecurityComponent> createComponent,
      string userId,
      string rawPassword,
      ITFLogger logger)
    {
      string[] strArray = new string[2]
      {
        "LoginManager",
        "DBManager"
      };
      requestContext.GetService<TeamFoundationDatabaseManagementService>().CreateSqlLogin(requestContext, connection, userId, rawPassword, true, logger);
      using (TeamFoundationSqlSecurityComponent securityComponent = createComponent())
      {
        string user = securityComponent.CreateUser(userId);
        foreach (string roleName in strArray)
          securityComponent.AddRoleMember(roleName, user);
      }
    }

    private static void RemoveTemporarySqlLogin(
      ISqlConnectionInfo connection,
      string userId,
      ITFLogger logger)
    {
      using (TeamFoundationSqlSecurityComponent componentRaw = connection.CreateComponentRaw<TeamFoundationSqlSecurityComponent>(logger: logger))
        componentRaw.DropLogin(userId);
    }

    private static string GetNameOfDatabase(InternalDatabaseProperties dbProperties) => dbProperties.DatabaseName.Substring(dbProperties.DatabaseName.IndexOf(';') + 1);

    private class DatabaseItems
    {
      public DatabaseItems() => this.Dataspaces = new List<Dataspace>();

      public InternalDatabaseProperties DatabaseProperties { get; set; }

      public List<Dataspace> Dataspaces { get; set; }

      public byte[] UserSid { get; set; }

      public byte[] DataTierSid { get; set; }
    }
  }
}
