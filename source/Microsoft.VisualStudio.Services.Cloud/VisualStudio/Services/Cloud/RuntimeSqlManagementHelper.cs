// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RuntimeSqlManagementHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure;
using Azure.ResourceManager.Models;
using Azure.ResourceManager.Sql;
using Azure.ResourceManager.Sql.Models;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class RuntimeSqlManagementHelper : IRuntimeSqlManagementHelper
  {
    private const string c_area = "DatabaseManagement";
    private const string c_layer = "FailoverGroup";

    public void AddDatabaseToFailoverGroupAndEnableAZ(
      IVssRequestContext requestContext,
      string dataSource,
      string databaseName,
      bool enableAz,
      ITFLogger logger)
    {
      if (logger == null)
        logger = (ITFLogger) new TraceLogger(requestContext, "DatabaseManagement", "FailoverGroup");
      if (!requestContext.ExecutionEnvironment.IsCloudDeployment)
        return;
      bool flag = requestContext.IsFeatureEnabled(FrameworkServerConstants.SqlFailoverGroupEnabled);
      bool allowNull = !flag;
      string resourceGroupName;
      SqlManagementResourceHelper managementResourceHelper = this.GetSqlManagementResourceHelper(requestContext, logger, out resourceGroupName, allowNull);
      try
      {
        if (flag)
          managementResourceHelper.EnableDatabaseFailoverGroup(resourceGroupName, dataSource, databaseName);
        else
          logger.Info("Skip enabling failover group for database " + databaseName);
      }
      catch (RequestFailedException ex)
      {
        logger.Error(string.Format("Fail to add database to failover group with exception: {0}", (object) ex));
        throw;
      }
      try
      {
        managementResourceHelper?.EnableDatabaseZoneRedundancy(resourceGroupName, dataSource, databaseName);
      }
      catch (RequestFailedException ex)
      {
        logger.Error(string.Format("Fail to enable database zone redundancy with exception: {0}", (object) ex));
        throw;
      }
    }

    public void AddDatabaseToFailoverGroupAndEnableAZ(
      Guid subscriptionId,
      string resourceManagerAadTenantId,
      string resourceManagementUrl,
      string resourceGroupName,
      string dataSource,
      string databaseName,
      ITFLogger logger,
      bool enableDatabaseFailoverGroup = true)
    {
      SqlManagementResourceHelper managementResourceHelper = new SqlManagementResourceHelper(subscriptionId, resourceManagerAadTenantId, resourceManagementUrl, logger);
      if (enableDatabaseFailoverGroup)
      {
        try
        {
          managementResourceHelper.EnableDatabaseFailoverGroup(resourceGroupName, dataSource, databaseName);
        }
        catch (RequestFailedException ex)
        {
          logger.Error(string.Format("Fail to add database to failover group with exception: {0}", (object) ex));
        }
      }
      try
      {
        managementResourceHelper.EnableDatabaseZoneRedundancy(resourceGroupName, dataSource, databaseName);
      }
      catch (RequestFailedException ex)
      {
        logger.Error(string.Format("Fail to enable database zone redundancy with exception: {0}", (object) ex));
        throw;
      }
    }

    public void DropUserDatabasesFromPartnerServer(
      Guid subscriptionId,
      string resourceManagerAadTenantId,
      string resourceManagementUrl,
      string resourceGroupName,
      string dataSource,
      ITFLogger logger)
    {
      SqlManagementResourceHelper helper = new SqlManagementResourceHelper(subscriptionId, resourceManagerAadTenantId, resourceManagementUrl, logger);
      try
      {
        logger.Info("Find failover group by data source " + dataSource + " of resource group " + resourceGroupName + ".");
        FailoverGroupResource failoverGroup = helper.FindFailoverGroup(resourceGroupName, dataSource);
        string partnerServer = failoverGroup != null ? failoverGroup.Data.PartnerServers.FirstOrDefault<PartnerServerInfo>()?.Id.Name : (string) null;
        logger.Info("Found failover group " + (((ResourceData) failoverGroup?.Data).Name ?? "NULL") + ". Partner server: " + (partnerServer ?? "NULL") + ".");
        if (partnerServer == null)
          return;
        List<string> second = new List<string>()
        {
          "master",
          "tempdb",
          "model",
          "msdb"
        };
        IEnumerable<string> strings = helper.ListDatabaseByServer(resourceGroupName, partnerServer).Select<SqlDatabaseResource, string>((Func<SqlDatabaseResource, string>) (db => ((ResourceData) db.Data).Name)).Except<string>((IEnumerable<string>) second, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        RetryManager retryManager = new RetryManager(6, TimeSpan.FromSeconds(15.0), (Action<Exception>) (exception => logger.Warning(exception)));
        foreach (string str in strings)
        {
          string database = str;
          retryManager.Invoke((Action) (() =>
          {
            logger.Info("Deleting database " + database + ".");
            helper.DeleteDatabase(resourceGroupName, partnerServer, database);
          }));
        }
      }
      catch (RequestFailedException ex)
      {
        logger.Error(string.Format("Fail to remove database from failover group with exception: {0}", (object) ex));
        throw;
      }
    }

    public void EnsureZoneRedundancyEnabled(IVssRequestContext requestContext, ITFLogger logger)
    {
      if (!AzureRoleUtil.IsAvailable || !requestContext.ExecutionEnvironment.IsCloudDeployment)
        return;
      SqlManagementResourceHelper managementResourceHelper = this.GetSqlManagementResourceHelper(requestContext, logger, out string _);
      try
      {
        managementResourceHelper.SetDatabaseZoneRedundancyForAll(requestContext, true, logger, (string) null);
      }
      catch (RequestFailedException ex)
      {
        logger.Error(string.Format("Fail to enable database zone redundancy with exception: {0}", (object) ex));
      }
    }

    public void SetDatabaseZoneRedundancy(
      IVssRequestContext requestContext,
      string datasource,
      string databaseName,
      bool isDatabaseVcore,
      bool enable,
      ITFLogger logger)
    {
      string str = TeamFoundationDataTierService.ManipulateDataSource(datasource, DataSourceOptions.RemoveProtocolAndDomain);
      this.GetSqlManagementResourceHelper(requestContext, logger, out string _).SetDatabasesZoneRedundancy(str, databaseName, isDatabaseVcore, enable);
    }

    public bool GetDatabaseZoneRedundancy(
      IVssRequestContext requestContext,
      string datasource,
      string databaseName,
      ITFLogger logger)
    {
      string str = TeamFoundationDataTierService.ManipulateDataSource(datasource, DataSourceOptions.RemoveProtocolAndDomain);
      return this.GetSqlManagementResourceHelper(requestContext, logger, out string _).GetDatabasesZoneRedundancy(str, databaseName);
    }

    private SqlManagementResourceHelper GetSqlManagementResourceHelper(
      IVssRequestContext requestContext,
      ITFLogger logger,
      out string resourceGroupName,
      bool allowNull = false)
    {
      SqlManagementResourceHelper managementResourceHelper;
      try
      {
        if (AzureRoleUtil.IsAvailable)
        {
          managementResourceHelper = RuntimeSqlManagementResourceHelper.Create(requestContext, logger);
          resourceGroupName = AzureRoleUtil.GetOverridableConfigurationSetting("HostedServiceName");
        }
        else
          managementResourceHelper = this.GetDeploymentSqlManagementResourceHelper(requestContext, logger, out resourceGroupName);
      }
      catch (AzureRoleEnvironmentUnavailableException ex)
      {
        managementResourceHelper = this.GetDeploymentSqlManagementResourceHelper(requestContext, logger, out resourceGroupName);
      }
      return managementResourceHelper != null || allowNull ? managementResourceHelper : throw new InvalidOperationException("Fail to create Sql management client! Either call InitializeManagementSettingsForDeploymentContext for deployment context or make sure it is running on role instance");
    }

    private SqlManagementResourceHelper GetDeploymentSqlManagementResourceHelper(
      IVssRequestContext requestContext,
      ITFLogger logger,
      out string resourceGroupName)
    {
      Guid guid;
      string str1;
      string str2;
      if (requestContext.TryGetItem<Guid>(RequestContextItemsKeys.AzureSubscriptionId, out guid) && requestContext.TryGetItem<string>(RequestContextItemsKeys.ResourceManagerAadTenantId, out str1) && requestContext.TryGetItem<string>(RequestContextItemsKeys.ResourceManagementUrl, out str2) && requestContext.TryGetItem<string>(RequestContextItemsKeys.ResourceGroupName, out resourceGroupName))
        return new SqlManagementResourceHelper(guid, str1, str2, logger);
      logger.Warning("Fail to get sql management helper");
      resourceGroupName = (string) null;
      return (SqlManagementResourceHelper) null;
    }

    public void UpgradeToHyperscale(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties databaseProperties,
      string currentServiceObjective,
      ITFLogger logger)
    {
      if (logger == null)
        logger = (ITFLogger) new TraceLogger(requestContext, "DatabaseManagement", "FailoverGroup");
      if (currentServiceObjective.StartsWith("HS_"))
        logger.Info("Current service objective is already Hyperscale: {0}", (object) currentServiceObjective);
      else if (requestContext.IsFeatureEnabled(FrameworkServerConstants.UpgradeToHyperscaleOnCreation))
      {
        IAzureDataTierComponentHelper tierComponentHelper = (IAzureDataTierComponentHelper) new AzureDataTierComponentHelper(requestContext, databaseProperties.SqlConnectionInfo, ChangeRecordEventConstants.SloChangeDuringCreation, logger);
        DatabaseServiceObjective hyperscaleObjective = this.GetEquivalentHyperscaleObjective(currentServiceObjective, logger);
        try
        {
          ISqlConnectionInfo sqlConnectionInfo;
          if (!tierComponentHelper.PrepareConnectionInfo(requestContext, ref sqlConnectionInfo))
            return;
          logger.Info("Changing service objective to {0} on database: {1}", (object) hyperscaleObjective.ToString(), (object) databaseProperties.DatabaseName);
          if (tierComponentHelper.SetServiceObjective(requestContext, databaseProperties, sqlConnectionInfo, hyperscaleObjective, true, (ServiceObjectiveChangeAllowed) 3, new int?()))
            logger.Info("Successfully changed the objective to {0} on database: {1}", (object) hyperscaleObjective.ToString(), (object) databaseProperties.DatabaseName);
          else
            logger.Warning("Failed to upgrade the database to {0} on database: {1}", (object) hyperscaleObjective.ToString(), (object) databaseProperties.DatabaseName);
        }
        catch (Exception ex)
        {
          logger.Warning("Failed to upgrade the database to {0} on database: {1}", (object) hyperscaleObjective.ToString(), (object) databaseProperties.DatabaseName);
          logger.Warning(ex);
        }
      }
      else
        logger.Info("FF {0} is disabled. Skipping upgrade on database: {1}", (object) FrameworkServerConstants.UpgradeToHyperscaleOnCreation, (object) databaseProperties.DatabaseName);
    }

    private DatabaseServiceObjective GetEquivalentHyperscaleObjective(
      string currentServiceObjective,
      ITFLogger logger)
    {
      DatabaseServiceObjective result = DatabaseServiceObjective.HS_Gen5_6;
      if (currentServiceObjective.StartsWith("HS_"))
      {
        logger.Info("The Service Objective is provided is already Hyperscale, converting string to DatabaseServiceObjective object");
        Enum.TryParse<DatabaseServiceObjective>(currentServiceObjective, true, out result);
      }
      else if (currentServiceObjective.StartsWith("GP_"))
        Enum.TryParse<DatabaseServiceObjective>(currentServiceObjective.Replace("GP_", "HS_"), true, out result);
      else if (currentServiceObjective.StartsWith("BC_"))
      {
        Enum.TryParse<DatabaseServiceObjective>(currentServiceObjective.Replace("BC_", "HS_"), true, out result);
      }
      else
      {
        if (currentServiceObjective != null)
        {
          switch (currentServiceObjective.Length)
          {
            case 2:
              switch (currentServiceObjective[1])
              {
                case '0':
                  if (currentServiceObjective == "S0")
                    break;
                  goto label_28;
                case '1':
                  if (currentServiceObjective == "S1" || currentServiceObjective == "P1")
                    break;
                  goto label_28;
                case '2':
                  switch (currentServiceObjective)
                  {
                    case "S2":
                      break;
                    case "P2":
                      goto label_22;
                    default:
                      goto label_28;
                  }
                  break;
                case '3':
                  if (currentServiceObjective == "S3")
                    break;
                  goto label_28;
                case '4':
                  switch (currentServiceObjective)
                  {
                    case "S4":
                      goto label_22;
                    case "P4":
                      goto label_23;
                    default:
                      goto label_28;
                  }
                case '6':
                  switch (currentServiceObjective)
                  {
                    case "S6":
                      goto label_23;
                    case "P6":
                      goto label_24;
                    default:
                      goto label_28;
                  }
                case '7':
                  if (currentServiceObjective == "S7")
                    goto label_24;
                  else
                    goto label_28;
                case '9':
                  if (currentServiceObjective == "S9")
                    goto label_25;
                  else
                    goto label_28;
                default:
                  goto label_28;
              }
              return DatabaseServiceObjective.HS_Gen5_2;
label_22:
              return DatabaseServiceObjective.HS_Gen5_4;
label_23:
              return DatabaseServiceObjective.HS_Gen5_6;
label_24:
              return DatabaseServiceObjective.HS_Gen5_12;
            case 3:
              switch (currentServiceObjective[2])
              {
                case '1':
                  if (currentServiceObjective == "P11")
                    break;
                  goto label_28;
                case '2':
                  if (currentServiceObjective == "S12")
                    return DatabaseServiceObjective.HS_Gen5_32;
                  goto label_28;
                case '5':
                  if (currentServiceObjective == "P15")
                    return DatabaseServiceObjective.HS_Gen5_40;
                  goto label_28;
                default:
                  goto label_28;
              }
              break;
            default:
              goto label_28;
          }
label_25:
          return DatabaseServiceObjective.HS_Gen5_18;
        }
label_28:
        return result;
      }
      return result;
    }
  }
}
