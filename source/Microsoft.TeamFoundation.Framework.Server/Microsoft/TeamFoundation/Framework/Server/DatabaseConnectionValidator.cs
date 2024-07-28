// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DatabaseConnectionValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DatabaseConnectionValidator
  {
    private TeamFoundationServiceHostProperties m_applicationHostProperties;
    private IEnumerable<TeamFoundationHostConfiguration> m_validationHostConfigurationDetails;
    private List<string> m_unreachableSqlInstances = new List<string>();
    private List<KeyValuePair<string, List<string>>> m_unreachableDatabases = new List<KeyValuePair<string, List<string>>>();
    private Func<string, string, bool> m_databaseFilter;
    private static readonly string s_area = "DataTierService";
    private static readonly string s_layer = nameof (DatabaseConnectionValidator);

    public DatabaseConnectionValidator()
      : this((Func<string, string, bool>) ((instance, db) => true))
    {
    }

    public DatabaseConnectionValidator(Func<string, string, bool> databaseFilter)
    {
      ArgumentUtility.CheckForNull<Func<string, string, bool>>(databaseFilter, nameof (databaseFilter));
      this.m_databaseFilter = databaseFilter;
    }

    public TeamFoundationServiceHostProperties ApplicationHostProperties => this.m_applicationHostProperties;

    public IEnumerable<TeamFoundationHostConfiguration> ValidationHostConfigurationDetails => this.m_validationHostConfigurationDetails;

    public TeamFoundationConfigurationStatus ValidateApplicationConfiguration(
      ISqlConnectionInfo configConnectionInfo,
      Guid configInstanceId,
      List<string> sqlInstances,
      bool autoFixConfiguration,
      bool fullValidation,
      bool continueOnMissingCollection,
      bool advancedConnectionCompare)
    {
      if (configInstanceId == Guid.Empty)
      {
        using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(configConnectionInfo))
        {
          DatabasePartition databasePartition = component.QueryPartition(DatabasePartitionConstants.DeploymentHostPartitionId);
          configInstanceId = databasePartition != null ? databasePartition.ServiceHostId : throw new DatabaseInstanceException(FrameworkResources.InstanceEmptyError());
          TeamFoundationTracingService.TraceRaw(9710, TraceLevel.Info, DatabaseConnectionValidator.s_area, DatabaseConnectionValidator.s_layer, "Loaded application id {0} from deployment partition ({1})", (object) configInstanceId, (object) databasePartition.PartitionId);
        }
      }
      else
        this.ValidateDatabaseInstanceStamp(configConnectionInfo, configInstanceId);
      List<TeamFoundationServiceHostProperties> items1;
      List<ISqlConnectionInfo> items2;
      using (HostManagementComponent componentRaw1 = configConnectionInfo.CreateComponentRaw<HostManagementComponent>(3600, handleNoResourceManagementSchema: true))
      {
        items1 = componentRaw1.QueryServiceHostPropertiesBootstrap(configInstanceId, fullValidation).GetCurrent<TeamFoundationServiceHostProperties>().Items;
        items2 = componentRaw1.QuerySqlConnectionInfoBootstrap(configInstanceId, fullValidation).GetCurrent<ISqlConnectionInfo>().Items;
        this.m_applicationHostProperties = items1.Count != 0 ? items1[0] : throw new DatabaseInstanceException(FrameworkResources.InstanceEmptyError());
        ISqlConnectionInfo sqlConnectionInfo = items2[0];
        if (configInstanceId != this.m_applicationHostProperties.Id)
          throw new DatabaseInstanceException(FrameworkResources.InstanceStampMismatch((object) configInstanceId, (object) this.m_applicationHostProperties.Id));
        bool flag;
        using (ExtendedAttributeComponent componentRaw2 = configConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
          flag = componentRaw2.ReadDeploymentTypeStamp() == DeploymentType.OnPremises;
        if (flag)
        {
          if (!DatabaseConnectionValidator.ConnectionsStringsAreEquivalent(configConnectionInfo.ConnectionString, sqlConnectionInfo.ConnectionString, advancedConnectionCompare))
          {
            if (!fullValidation)
            {
              items1 = componentRaw1.QueryServiceHostPropertiesBootstrap(configInstanceId, true).GetCurrent<TeamFoundationServiceHostProperties>().Items;
              items2 = componentRaw1.QuerySqlConnectionInfoBootstrap(configInstanceId, true).GetCurrent<ISqlConnectionInfo>().Items;
              this.m_applicationHostProperties = items1[0];
              fullValidation = true;
            }
          }
        }
      }
      Dictionary<Guid, TeamFoundationHostConfiguration> hostConfigurations = new Dictionary<Guid, TeamFoundationHostConfiguration>();
      for (int index = 0; index < items1.Count; ++index)
        hostConfigurations[items1[index].Id] = new TeamFoundationHostConfiguration(items1[index], items2[index]);
      if (fullValidation)
      {
        if (sqlInstances == null)
          sqlInstances = new List<string>();
        if (sqlInstances.Count == 0)
        {
          SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(configConnectionInfo.ConnectionString);
          sqlInstances.Add(connectionStringBuilder.DataSource);
        }
        this.m_unreachableDatabases.Clear();
        this.m_unreachableSqlInstances.Clear();
        foreach (TeamFoundationDatabase instancesForDatabase in this.ScanSqlInstancesForDatabases(configConnectionInfo, sqlInstances))
        {
          TeamFoundationHostConfiguration hostConfiguration;
          if (instancesForDatabase.InstanceIds.Length >= 1 && hostConfigurations.TryGetValue(instancesForDatabase.InstanceIds[0], out hostConfiguration))
            hostConfiguration.DiscoveredDatabases.Add(instancesForDatabase);
        }
      }
      TeamFoundationConfigurationStatus configurationStatus = DatabaseConnectionValidator.ValidateConfiguration(configConnectionInfo, configInstanceId, false, fullValidation, hostConfigurations, continueOnMissingCollection, advancedConnectionCompare);
      if (configurationStatus == TeamFoundationConfigurationStatus.ValidAfterAutoFix & autoFixConfiguration)
        configurationStatus = DatabaseConnectionValidator.ValidateConfiguration(configConnectionInfo, configInstanceId, true, fullValidation, hostConfigurations, continueOnMissingCollection, advancedConnectionCompare);
      this.m_validationHostConfigurationDetails = (IEnumerable<TeamFoundationHostConfiguration>) hostConfigurations.Values;
      return configurationStatus;
    }

    public string GetMessagesForInvalidHosts() => this.GetValidationMessagesForHosts(TeamFoundationConfigurationStatus.Invalid, HostConfigurationValidationItemType.Error);

    public string GetValidationMessagesForFixedHosts() => this.GetValidationMessagesForHosts(TeamFoundationConfigurationStatus.ValidAfterAutoFix, HostConfigurationValidationItemType.ConnectionStringUpdate | HostConfigurationValidationItemType.HostUnavailable);

    public string GetUnreachableDatabaseInformation()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.m_unreachableSqlInstances.Count != 0)
      {
        stringBuilder.AppendLine(FrameworkResources.UnreachableSqlInstances((object) this.m_unreachableSqlInstances.Count));
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(FrameworkResources.UnreachableSqlInstancesReason());
        stringBuilder.AppendLine();
      }
      if (this.m_unreachableDatabases.Count != 0)
      {
        foreach (KeyValuePair<string, List<string>> unreachableDatabase in this.m_unreachableDatabases)
          ;
        stringBuilder.AppendLine(FrameworkResources.UnreachableDatabases((object) this.m_unreachableDatabases.Count));
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(FrameworkResources.UnreachableDatabasesReason());
        stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }

    public List<TeamFoundationDatabase> ScanSqlInstancesForDatabases(
      ISqlConnectionInfo configConnectionInfo,
      List<string> sqlInstances)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(configConnectionInfo.ConnectionString);
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      List<TeamFoundationDatabase> foundationDatabaseList = new List<TeamFoundationDatabase>();
      foreach (string sqlInstance in sqlInstances)
      {
        if (!dictionary.ContainsKey(sqlInstance))
        {
          connectionStringBuilder.InitialCatalog = TeamFoundationSqlResourceComponent.Master;
          connectionStringBuilder.DataSource = sqlInstance;
          ISqlConnectionInfo masterConnectionInfo = configConnectionInfo.Create(connectionStringBuilder.ConnectionString);
          foundationDatabaseList.AddRange((IEnumerable<TeamFoundationDatabase>) this.ScanSqlInstance(masterConnectionInfo));
          dictionary[sqlInstance] = (object) null;
        }
      }
      return foundationDatabaseList;
    }

    internal static bool ConnectionsStringsAreEquivalent(
      string first,
      string second,
      bool advancedConnectionCompare)
    {
      if (StringComparer.OrdinalIgnoreCase.Equals(first, second))
        return true;
      SqlConnectionStringBuilder builder1 = new SqlConnectionStringBuilder(first);
      SqlConnectionStringBuilder builder2 = new SqlConnectionStringBuilder(second);
      if (!StringComparer.OrdinalIgnoreCase.Equals(builder1.InitialCatalog, builder2.InitialCatalog) || !advancedConnectionCompare && builder1.GetMultiSubnetFailover() != builder2.GetMultiSubnetFailover())
        return false;
      if (VssStringComparer.DataSourceIgnoreProtocol.Equals(builder1.DataSource, builder2.DataSource))
        return true;
      if (advancedConnectionCompare && builder1.IntegratedSecurity)
      {
        if (builder2.IntegratedSecurity)
        {
          try
          {
            Guid uniqueGuid = Guid.NewGuid();
            using (EquivalentDatabaseConnectionComponent componentRaw1 = SqlConnectionInfoFactory.Create(builder1.ConnectionString).CreateComponentRaw<EquivalentDatabaseConnectionComponent>())
            {
              if (!componentRaw1.GetAppLock(uniqueGuid))
                return false;
              using (EquivalentDatabaseConnectionComponent componentRaw2 = SqlConnectionInfoFactory.Create(builder2.ConnectionString).CreateComponentRaw<EquivalentDatabaseConnectionComponent>())
                return !componentRaw2.GetAppLock(uniqueGuid);
            }
          }
          catch
          {
            return false;
          }
        }
      }
      return false;
    }

    private string GetValidationMessagesForHosts(
      TeamFoundationConfigurationStatus hostStatus,
      HostConfigurationValidationItemType validationType)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (TeamFoundationHostConfiguration configurationDetail in this.ValidationHostConfigurationDetails)
      {
        if (configurationDetail.Status == hostStatus)
        {
          bool flag = false;
          foreach (HostConfigurationValidationItem validationItem in configurationDetail.ValidationItems)
          {
            if (validationItem.Type == HostConfigurationValidationItemType.ConnectionStringUpdate)
              flag = true;
            else if ((validationItem.Type & validationType) == validationItem.Type)
              stringBuilder.AppendLine(validationItem.Message);
          }
          if (flag)
            stringBuilder.AppendLine(configurationDetail.GetValidationMessageForConnectionStringUpdates());
        }
      }
      return stringBuilder.ToString();
    }

    private static TeamFoundationConfigurationStatus ValidateConfiguration(
      ISqlConnectionInfo configConnectionInfo,
      Guid configInstanceId,
      bool autoFixConfiguration,
      bool fullValidation,
      Dictionary<Guid, TeamFoundationHostConfiguration> hostConfigurations,
      bool continueOnMissingCollection,
      bool advancedConnectionCompare)
    {
      TeamFoundationConfigurationStatus configurationStatus = TeamFoundationConfigurationStatus.Valid;
      foreach (TeamFoundationHostConfiguration hostConfiguration in hostConfigurations.Values)
      {
        if (hostConfiguration.HostProperties.Id != configInstanceId)
        {
          hostConfiguration.Validate(configConnectionInfo, configInstanceId, autoFixConfiguration, fullValidation, continueOnMissingCollection, advancedConnectionCompare);
          configurationStatus |= hostConfiguration.Status;
        }
      }
      if (configurationStatus == TeamFoundationConfigurationStatus.Valid || configurationStatus == TeamFoundationConfigurationStatus.ValidAfterAutoFix)
      {
        TeamFoundationHostConfiguration hostConfiguration = hostConfigurations[configInstanceId];
        if (!fullValidation)
        {
          TeamFoundationDatabase foundationDatabase = new TeamFoundationDatabase(new Guid[1]
          {
            configInstanceId
          }, configConnectionInfo, string.Empty);
          hostConfiguration.DiscoveredDatabases.Add(foundationDatabase);
        }
        hostConfiguration.Validate(configConnectionInfo, configInstanceId, autoFixConfiguration, fullValidation, continueOnMissingCollection, advancedConnectionCompare);
        configurationStatus |= hostConfiguration.Status;
      }
      return configurationStatus;
    }

    private void ValidateDatabaseInstanceStamp(
      ISqlConnectionInfo configConnectionInfo,
      Guid configInstanceId)
    {
      using (DatabasePartitionComponent component = DatabasePartitionComponent.CreateComponent(configConnectionInfo))
      {
        if (component.QueryPartition(configInstanceId) == null)
          throw new DatabaseInstanceException(FrameworkResources.InstanceMismatchError((object) configInstanceId));
      }
    }

    private List<TeamFoundationDatabase> ScanSqlInstance(ISqlConnectionInfo masterConnectionInfo)
    {
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(masterConnectionInfo.ConnectionString);
      string sqlInstance = connectionStringBuilder.DataSource;
      List<TeamFoundationDatabase> foundationDatabaseList = new List<TeamFoundationDatabase>();
      List<DatabaseInformation> source = (List<DatabaseInformation>) null;
      using (TeamFoundationDataTierComponent componentRaw = masterConnectionInfo.CreateComponentRaw<TeamFoundationDataTierComponent>())
      {
        try
        {
          source = componentRaw.GetDatabases().Where<DatabaseInformation>((Func<DatabaseInformation, bool>) (db => !db.IsSystem && !db.ReadOnly && db.UserAccess == DatabaseUserAccess.Multiple && db.State == DatabaseState.Online)).ToList<DatabaseInformation>();
        }
        catch (DatabaseConnectionException ex)
        {
          this.m_unreachableSqlInstances.Add(sqlInstance);
          return new List<TeamFoundationDatabase>();
        }
      }
      List<string> stringList = new List<string>();
      foreach (DatabaseInformation databaseInformation in source.Where<DatabaseInformation>((Func<DatabaseInformation, bool>) (db => this.m_databaseFilter(sqlInstance, db.Name))))
      {
        List<Guid> guidList = new List<Guid>();
        connectionStringBuilder.InitialCatalog = databaseInformation.Name;
        string connectionString = connectionStringBuilder.ConnectionString;
        try
        {
          ISqlConnectionInfo connectionInfo = masterConnectionInfo.Create(connectionString);
          DatabasePartitionComponent component;
          if (DatabasePartitionComponent.TryCreateComponent(connectionInfo, out component))
          {
            using (component)
            {
              List<DatabasePartition> databasePartitionList = component.QueryPartitions();
              if (databasePartitionList.Count != 0)
              {
                foreach (DatabasePartition databasePartition in databasePartitionList)
                  guidList.Add(databasePartition.ServiceHostId);
              }
              else
                continue;
            }
          }
          using (ExtendedAttributeComponent componentRaw = connectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
          {
            string[] strArray = componentRaw.ReadDatabaseAttributes(TeamFoundationSqlResourceComponent.ExtendedPropertyInstanceStamp, TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelStamp, CollectionMoveConstants.SnapshotStateExtendedProperty, TeamFoundationSqlResourceComponent.ExtendedPropertyDatabaseType, TeamFoundationSqlResourceComponent.ExtendedPropertyDatabaseCategories);
            string input = strArray[0];
            string schemaLevel = strArray[1] ?? string.Empty;
            string str = strArray[2];
            string databaseType = strArray[3];
            string databaseCategories = strArray[4];
            if (guidList.Count == 0)
            {
              if (!string.IsNullOrEmpty(input))
              {
                Guid result;
                if (Guid.TryParse(input, out result))
                  guidList.Add(result);
              }
              else
                continue;
            }
            if (string.IsNullOrEmpty(str))
            {
              if (string.IsNullOrEmpty(databaseType))
              {
                if (string.IsNullOrEmpty(databaseCategories))
                  continue;
              }
              TeamFoundationDatabase foundationDatabase = new TeamFoundationDatabase(guidList.ToArray(), connectionInfo, schemaLevel);
              foundationDatabaseList.Add(foundationDatabase);
              foundationDatabase.IsWarehouseDatabase = DatabaseConnectionValidator.ComputeIsWarehouseDatabase(databaseType, databaseCategories);
            }
          }
        }
        catch (DatabaseConnectionException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(9700, TraceLevel.Warning, DatabaseConnectionValidator.s_area, DatabaseConnectionValidator.s_layer, (Exception) ex);
          stringList.Add(databaseInformation.Name);
        }
        catch (DatabaseConfigurationException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(9701, TraceLevel.Warning, DatabaseConnectionValidator.s_area, DatabaseConnectionValidator.s_layer, (Exception) ex);
          if (ex.InnerException is SqlException innerException && innerException.Number == 978)
          {
            TeamFoundationTracingService.TraceRaw(9703, TraceLevel.Warning, DatabaseConnectionValidator.s_area, DatabaseConnectionValidator.s_layer, "The '{0}' database which is part of the '{1}' availability group and requires ReadOnly ApplicationIntent or connecting to the high-availability listener.", (object) databaseInformation.Name, (object) databaseInformation.AvailabilityGroupName);
            stringList.Add(databaseInformation.Name);
          }
          else
            throw;
        }
        catch (SqlException ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(9702, TraceLevel.Warning, DatabaseConnectionValidator.s_area, DatabaseConnectionValidator.s_layer, (Exception) ex);
          if (ex.Class >= (byte) 20 && (databaseInformation.MirroringId != Guid.Empty || !string.IsNullOrEmpty(databaseInformation.AvailabilityGroupName)))
            stringList.Add(databaseInformation.Name);
          else
            throw;
        }
      }
      if (stringList.Count > 0)
        this.m_unreachableDatabases.Add(new KeyValuePair<string, List<string>>(sqlInstance, stringList));
      return foundationDatabaseList;
    }

    private static bool ComputeIsWarehouseDatabase(string databaseType, string databaseCategories) => !string.IsNullOrEmpty(databaseType) && string.Equals(databaseType, TeamFoundationSqlResourceComponent.DatabaseTypeWarehouse, StringComparison.Ordinal) || string.Equals(databaseCategories, TeamFoundationSqlResourceComponent.DatabaseCategoryWarehouse, StringComparison.Ordinal);

    internal static void ValidateOnPremConfigurationDatabase(
      IVssRequestContext deploymentRequestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentRequestContext, nameof (deploymentRequestContext));
      ISqlConnectionInfo frameworkConnectionInfo = deploymentRequestContext.FrameworkConnectionInfo;
      string str1;
      string str2;
      using (ExtendedAttributeComponent component = deploymentRequestContext.CreateComponent<ExtendedAttributeComponent>())
      {
        string[] strArray = component.ReadDatabaseAttributes(TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelStamp, TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelToStamp);
        str1 = strArray[0];
        str2 = strArray[1];
      }
      if (str2 != null)
        throw new DatabaseSchemaException(deploymentRequestContext, FrameworkResources.SchemaUpgradeInProgressError((object) str2));
      string str3 = ServicingUtils.GetServiceLevelFromReleaseManifest().ToString();
      if (string.IsNullOrEmpty(str1))
        throw new DatabaseSchemaException(deploymentRequestContext, FrameworkResources.ServiceLevelEmptyError((object) TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelStamp, (object) str3));
      if (string.Equals(str3, str1, StringComparison.OrdinalIgnoreCase))
        return;
      ServiceLevel serviceLevel1 = new ServiceLevel(str3);
      ServiceLevel serviceLevel2 = new ServiceLevel(str1);
      if (ServiceLevel.CompareMajorVersions(serviceLevel1.MajorVersion, serviceLevel2.MajorVersion) != 0 || ServiceLevel.CompareMilestones(serviceLevel1.Milestone, serviceLevel2.Milestone) != 0)
        throw new DatabaseSchemaException(deploymentRequestContext, FrameworkResources.ServiceLevelMismatchError((object) TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelStamp, (object) str3, (object) str1));
    }
  }
}
