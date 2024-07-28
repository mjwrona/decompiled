// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationHostConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationHostConfiguration
  {
    private List<TeamFoundationDatabase> m_databases = new List<TeamFoundationDatabase>();
    private List<HostConfigurationValidationItem> m_validationItems = new List<HostConfigurationValidationItem>();
    private Dictionary<string, ConnectionStringUpdateHostConfigurationValidationItem> m_connectionStringUpdates = new Dictionary<string, ConnectionStringUpdateHostConfigurationValidationItem>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public TeamFoundationHostConfiguration(
      TeamFoundationServiceHostProperties hostProperties,
      ISqlConnectionInfo connectionInfo)
    {
      this.HostProperties = hostProperties;
      this.Status = TeamFoundationConfigurationStatus.NotValidated;
      this.PersistedConnectionInfo = connectionInfo;
    }

    public TeamFoundationServiceHostProperties HostProperties { get; private set; }

    public TeamFoundationConfigurationStatus Status { get; private set; }

    public ISqlConnectionInfo PersistedConnectionInfo { get; private set; }

    public List<HostConfigurationValidationItem> ValidationItems => this.m_validationItems;

    public List<TeamFoundationDatabase> DiscoveredDatabases => this.m_databases;

    public void Validate(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      Guid instanceId,
      bool autoFix,
      bool fullValidation,
      bool continueOnMissingCollectionDb,
      bool advancedConnectionCompare)
    {
      this.Status = TeamFoundationConfigurationStatus.Valid;
      this.m_validationItems.Clear();
      this.m_connectionStringUpdates.Clear();
      List<TeamFoundationDatabase> list = this.DiscoveredDatabases.Where<TeamFoundationDatabase>((Func<TeamFoundationDatabase, bool>) (db => !db.IsWarehouseDatabase)).ToList<TeamFoundationDatabase>();
      if (list.Count > 1)
      {
        TeamFoundationDatabase foundationDatabase = list[0];
        this.m_validationItems.Add(new HostConfigurationValidationItem(HostConfigurationValidationItemType.Error, FrameworkResources.DuplicateDatabasesFound((object) "Framework", (object) this.HostProperties.Name, (object) this.HostProperties.Id, (object) list[1].ConnectionInfo, (object) foundationDatabase.ConnectionInfo)));
        this.Status = TeamFoundationConfigurationStatus.Invalid;
      }
      else
      {
        SqlConnectionStringBuilder connectionStringBuilder1 = new SqlConnectionStringBuilder(this.PersistedConnectionInfo.ConnectionString);
        SqlConnectionStringBuilder connectionStringBuilder2 = new SqlConnectionStringBuilder(configurationDatabaseConnectionInfo.ConnectionString);
        string dataSource = connectionStringBuilder1.DataSource;
        if (this.DiscoveredDatabases.Count == 0)
        {
          if (continueOnMissingCollectionDb)
          {
            connectionStringBuilder1.DataSource = connectionStringBuilder2.DataSource;
            if (autoFix)
              this.FixConnectionString(configurationDatabaseConnectionInfo, connectionStringBuilder1);
            this.m_validationItems.Add(new HostConfigurationValidationItem(HostConfigurationValidationItemType.HostUnavailable, FrameworkResources.UsedConfigSqlInstanceForHostConnectionStringUpdateAction((object) this.HostProperties.Name, (object) this.HostProperties.Id)));
            this.Status = TeamFoundationConfigurationStatus.ValidAfterAutoFix;
          }
          else
          {
            this.m_validationItems.Add(new HostConfigurationValidationItem(HostConfigurationValidationItemType.Error, FrameworkResources.UnableToFindDatabase((object) this.HostProperties.Name, (object) this.HostProperties.Id)));
            this.Status = TeamFoundationConfigurationStatus.Invalid;
          }
        }
        else if (list.Count == 0)
        {
          this.m_validationItems.Add(new HostConfigurationValidationItem(HostConfigurationValidationItemType.Error, FrameworkResources.UnableToFindDatabaseWithSchema((object) "Framework", (object) this.HostProperties.Name, (object) this.HostProperties.Id)));
          this.Status = TeamFoundationConfigurationStatus.Invalid;
        }
        else
        {
          TeamFoundationDatabase frameworkDatabase = list.FirstOrDefault<TeamFoundationDatabase>();
          SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(frameworkDatabase.ConnectionInfo.ConnectionString);
          if (!DatabaseConnectionValidator.ConnectionsStringsAreEquivalent(builder.ConnectionString, connectionStringBuilder1.ConnectionString, advancedConnectionCompare))
          {
            connectionStringBuilder1.DataSource = builder.DataSource;
            connectionStringBuilder1.InitialCatalog = builder.InitialCatalog;
            connectionStringBuilder1.IntegratedSecurity = builder.IntegratedSecurity;
            connectionStringBuilder1.Encrypt = builder.Encrypt;
            connectionStringBuilder1.SetMultiSubnetFailover(builder.GetMultiSubnetFailover());
            this.AddConnectionStringUpdateValidationItem(connectionStringBuilder1, "Default");
            if (autoFix)
              this.FixConnectionString(configurationDatabaseConnectionInfo, connectionStringBuilder1);
            this.Status = TeamFoundationConfigurationStatus.ValidAfterAutoFix;
          }
          TeamFoundationHostConfiguration.GetConnectionStringsFromRegistry(frameworkDatabase, this.HostProperties.Id);
          DatabaseManagementComponent component;
          if (!fullValidation || !autoFix || this.HostProperties.HostType != TeamFoundationHostType.Deployment || !DatabaseManagementComponent.TryCreateComponent(configurationDatabaseConnectionInfo, out component))
            return;
          using (component)
          {
            foreach (InternalDatabaseProperties databaseProperties in component.QueryDatabases().GetCurrent<InternalDatabaseProperties>().Items)
            {
              SqlConnectionStringBuilder connectionStringBuilder3 = new SqlConnectionStringBuilder(databaseProperties.ConnectionInfoWrapper.ConnectionString);
              if (string.Equals(connectionStringBuilder3.DataSource, dataSource, StringComparison.OrdinalIgnoreCase) && (databaseProperties.Tenants == 0 || databaseProperties.Tenants - databaseProperties.TenantsPendingDelete == 0))
              {
                connectionStringBuilder3.DataSource = connectionStringBuilder2.DataSource;
                TeamFoundationDatabaseProperties editableProperties = databaseProperties.GetEditableProperties();
                editableProperties.UpdateConnectionString(connectionStringBuilder3.ConnectionString);
                component.UpdateDatabase(editableProperties);
              }
            }
          }
        }
      }
    }

    private void FixConnectionString(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      SqlConnectionStringBuilder persistedConnectionString)
    {
      if (this.HostProperties.DatabaseId == DatabaseManagementConstants.InvalidDatabaseId)
      {
        string str;
        using (ExtendedAttributeComponent componentRaw = configurationDatabaseConnectionInfo.CreateComponentRaw<ExtendedAttributeComponent>())
          str = componentRaw.ReadServiceLevelStamp();
        if (!string.IsNullOrEmpty(str))
        {
          if (!str.StartsWith("TFS2010", StringComparison.OrdinalIgnoreCase))
          {
            try
            {
              HostManagementComponent component;
              if (TeamFoundationResourceManagementService.TryCreateComponentRaw<HostManagementComponent>(configurationDatabaseConnectionInfo, 3600, 0, 10, out component))
              {
                using (component)
                  this.HostProperties = component.QueryServiceHostProperties(this.HostProperties.Id, ServiceHostFilterFlags.None).GetCurrent<TeamFoundationServiceHostProperties>().Items[0];
              }
            }
            catch (DatabaseConfigurationException ex)
            {
            }
          }
        }
      }
      DatabaseManagementComponent component1;
      if (this.HostProperties.DatabaseId != DatabaseManagementConstants.InvalidDatabaseId && DatabaseManagementComponent.TryCreateComponent(configurationDatabaseConnectionInfo, out component1))
      {
        using (component1)
        {
          TeamFoundationDatabaseProperties editableProperties = component1.GetDatabase(this.HostProperties.DatabaseId).GetEditableProperties();
          editableProperties.UpdateConnectionString(persistedConnectionString.ToString());
          component1.UpdateDatabase(editableProperties);
        }
      }
      else
      {
        using (HostManagementComponent componentRaw = configurationDatabaseConnectionInfo.CreateComponentRaw<HostManagementComponent>(3600, maxDeadlockRetries: 20, handleNoResourceManagementSchema: true))
          componentRaw.UpdateServiceHostBootstrapLegacy(this.HostProperties.Id, this.HostProperties.Name, this.HostProperties.Description, persistedConnectionString.ToString());
      }
    }

    private static Dictionary<string, RegistryEntry> GetConnectionStringsFromRegistry(
      TeamFoundationDatabase frameworkDatabase,
      Guid hostId)
    {
      Dictionary<string, RegistryEntry> stringsFromRegistry = new Dictionary<string, RegistryEntry>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry registryEntry in RegistryHelpers.GetValuesRaw(frameworkDatabase.ConnectionInfo, hostId, FrameworkServerConstants.DatabaseRoot))
      {
        if (registryEntry.Name.Equals(RegistryRelativePathConstants.ConnectionString, StringComparison.OrdinalIgnoreCase))
        {
          int startIndex = FrameworkServerConstants.DatabaseRoot.Length + 1;
          int num = RegistryRelativePathConstants.ConnectionString.Length + 1;
          string str = registryEntry.Path.Substring(startIndex, registryEntry.Path.Length - (startIndex + num));
          if (!str.Contains<char>('/'))
            stringsFromRegistry[str] = registryEntry;
        }
      }
      return stringsFromRegistry;
    }

    public string GetValidationMessageForConnectionStringUpdates()
    {
      if (this.m_connectionStringUpdates.Count == 0)
        return string.Empty;
      return this.m_connectionStringUpdates.Count == 1 ? FrameworkResources.UpdateConnectionStringAction((object) this.HostProperties.Name) : FrameworkResources.UpdatedConnectionStringsAction((object) this.m_connectionStringUpdates.Values.Count);
    }

    private void AddConnectionStringUpdateValidationItem(
      SqlConnectionStringBuilder targetConnectionString,
      string dataspaceCategory)
    {
      string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0};{1}", (object) targetConnectionString.DataSource, (object) targetConnectionString.InitialCatalog);
      ConnectionStringUpdateHostConfigurationValidationItem configurationValidationItem;
      if (!this.m_connectionStringUpdates.TryGetValue(key, out configurationValidationItem))
      {
        configurationValidationItem = new ConnectionStringUpdateHostConfigurationValidationItem(this.HostProperties.Name, targetConnectionString.ToString());
        this.m_connectionStringUpdates[key] = configurationValidationItem;
        this.m_validationItems.Add((HostConfigurationValidationItem) configurationValidationItem);
      }
      configurationValidationItem.DataspaceCategories[dataspaceCategory] = (object) null;
    }
  }
}
