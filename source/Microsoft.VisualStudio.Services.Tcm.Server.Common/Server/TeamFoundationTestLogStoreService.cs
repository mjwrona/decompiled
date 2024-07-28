// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestLogStoreService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestLogStoreService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestLogStoreService,
    IVssFrameworkService
  {
    private TeamFoundationTestLogStoreService m_deploymentService;
    private ITestLogStorageConnection[] m_testLogStorageProviders;
    private Dictionary<int, ITestLogStorageConnection> m_omegaTestLogStorageProvidersCache;
    private long logStoreConnectionCounter;

    public TeamFoundationTestLogStoreService()
    {
    }

    public TeamFoundationTestLogStoreService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<IVssRegistryService>();
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        this.m_deploymentService = this;
        this.RegisterForStrongBoxItemChanges(systemRequestContext);
        this.InitializeTestLogStorageProviders(systemRequestContext);
      }
      else
        this.m_deploymentService = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<TeamFoundationTestLogStoreService>();
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      systemRequestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged));
    }

    public ITestLogStorageConnection GetTestLogStorageConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails scopeDetails,
      bool createIfNotExist = true)
    {
      requestContext.TraceEnter("TraceLayer.RestLayer", nameof (GetTestLogStorageConnection));
      try
      {
        if (this.m_deploymentService?.m_testLogStorageProviders == null)
          return (ITestLogStorageConnection) null;
        int connectionIndexOfArtifact1 = LogStoreStorageAccountMap.GetStorageAccountConnectionIndexOfArtifact(requestContext, projectId, scopeDetails);
        requestContext.TraceVerbose("LogStorage", "GetTestLogStorageConnection: existingStorageAccountConnectionIndexOfArtifact - {0}", (object) connectionIndexOfArtifact1);
        if (connectionIndexOfArtifact1 >= 0)
          return this.m_deploymentService.m_testLogStorageProviders[connectionIndexOfArtifact1];
        if (!createIfNotExist)
        {
          requestContext.TraceInfo("LogStorage", "Not creating any connection as there's no mapping exists for the Artifact.");
          return (ITestLogStorageConnection) null;
        }
        int connectionIndexOfArtifact2 = LogStoreStorageAccountMap.CreateNewStorageAccountConnectionIndexOfArtifact(requestContext, projectId, scopeDetails, this.m_deploymentService.logStoreConnectionCounter);
        ++this.m_deploymentService.logStoreConnectionCounter;
        requestContext.TraceVerbose("LogStorage", "GetTestLogStorageConnection: newStorageAccountConnectionIndexOfArtifact - {0}", (object) connectionIndexOfArtifact2);
        return this.m_deploymentService.m_testLogStorageProviders[connectionIndexOfArtifact2];
      }
      finally
      {
        requestContext.TraceLeave("TraceLayer.RestLayer", nameof (GetTestLogStorageConnection));
      }
    }

    public IList<string> GetAllStorageAccountsName(IVssRequestContext requestContext)
    {
      List<string> storageAccountList = new List<string>();
      if (this.m_deploymentService == null)
        return (IList<string>) storageAccountList;
      if (this.m_deploymentService.m_testLogStorageProviders == null)
        return (IList<string>) storageAccountList;
      foreach (ITestLogStorageConnection logStorageProvider in this.m_deploymentService.m_testLogStorageProviders)
        TeamFoundationTestLogStoreService.PopulateStorageAccountList(storageAccountList, logStorageProvider);
      if (this.m_deploymentService.m_omegaTestLogStorageProvidersCache == null || this.m_deploymentService.m_omegaTestLogStorageProvidersCache.Count == 0)
      {
        requestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "No Omega / Secondary storage accounts have been configured for the LoghStore accounts. Check if strong box items are populated.");
        return (IList<string>) storageAccountList;
      }
      foreach (ITestLogStorageConnection storageAccount in this.m_deploymentService.m_omegaTestLogStorageProvidersCache.Values.ToList<ITestLogStorageConnection>())
        TeamFoundationTestLogStoreService.PopulateStorageAccountList(storageAccountList, storageAccount);
      return (IList<string>) storageAccountList;
    }

    private static void PopulateStorageAccountList(
      List<string> storageAccountList,
      ITestLogStorageConnection storageAccount)
    {
      string accountName = storageAccount?.GetLogStoreConnectionEndpoint()?.GetAccountName();
      if (accountName == null)
        return;
      storageAccountList.Add(accountName);
    }

    public IList<ITestLogStorageConnection> GetTestLogStorageConnections(
      IVssRequestContext requestContext)
    {
      TeamFoundationTestLogStoreService deploymentService = this.m_deploymentService;
      return deploymentService == null ? (IList<ITestLogStorageConnection>) null : (IList<ITestLogStorageConnection>) deploymentService.m_testLogStorageProviders;
    }

    public IList<ITestLogStorageConnection> GetOmegaTestLogStorageConnections(
      IVssRequestContext requestContext)
    {
      TeamFoundationTestLogStoreService deploymentService = this.m_deploymentService;
      return deploymentService == null ? (IList<ITestLogStorageConnection>) null : (IList<ITestLogStorageConnection>) deploymentService.m_omegaTestLogStorageProvidersCache.Values.ToList<ITestLogStorageConnection>();
    }

    public bool EnableCorsForStorageAccounts(
      IVssRequestContext requestContext,
      string corsDomainName)
    {
      if (this.m_deploymentService == null || this.m_deploymentService.m_testLogStorageProviders == null || !this.VerifyDomain(corsDomainName))
        return false;
      foreach (ITestLogStorageConnection logStorageProvider in this.m_deploymentService.m_testLogStorageProviders)
      {
        if (!TeamFoundationTestLogStoreService.EnableCors(corsDomainName, logStorageProvider))
          return false;
      }
      if (this.m_deploymentService.m_omegaTestLogStorageProvidersCache == null || this.m_deploymentService.m_omegaTestLogStorageProvidersCache.Count == 0)
      {
        requestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "No Omega / Secondary storage accounts have been configured for the LoghStore accounts. Check if strong box items are populated.");
        return true;
      }
      foreach (ITestLogStorageConnection storageAccount in this.m_deploymentService.m_omegaTestLogStorageProvidersCache.Values.ToList<ITestLogStorageConnection>())
      {
        if (!TeamFoundationTestLogStoreService.EnableCors(corsDomainName, storageAccount))
          return false;
      }
      return true;
    }

    private static bool EnableCors(string corsDomainName, ITestLogStorageConnection storageAccount)
    {
      bool? nullable1 = storageAccount?.GetLogStoreConnectionEndpoint()?.EnableCors(corsDomainName);
      if (nullable1.HasValue)
      {
        if (nullable1.HasValue)
        {
          bool? nullable2 = nullable1;
          bool flag = false;
          if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
            goto label_3;
        }
        return true;
      }
label_3:
      return false;
    }

    public bool DisableCorsForStorageAccounts(IVssRequestContext requestContext)
    {
      if (this.m_deploymentService == null || this.m_deploymentService.m_testLogStorageProviders == null)
        return false;
      foreach (ITestLogStorageConnection logStorageProvider in this.m_deploymentService.m_testLogStorageProviders)
      {
        if (!TeamFoundationTestLogStoreService.EnableCors((string) null, logStorageProvider))
          return false;
      }
      if (this.m_deploymentService.m_omegaTestLogStorageProvidersCache == null || this.m_deploymentService.m_omegaTestLogStorageProvidersCache.Count == 0)
      {
        requestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "No Omega / Secondary storage accounts have been configured for the LogStore accounts. Check if strong box items are populated.");
        return true;
      }
      foreach (ITestLogStorageConnection storageAccount in this.m_deploymentService.m_omegaTestLogStorageProvidersCache.Values.ToList<ITestLogStorageConnection>())
      {
        if (!TeamFoundationTestLogStoreService.EnableCors((string) null, storageAccount))
          return false;
      }
      return true;
    }

    public Dictionary<string, IList<string>> GetCorsAllowedHostListForStorageAccounts(
      IVssRequestContext requestContext)
    {
      Dictionary<string, IList<string>> forStorageAccounts = new Dictionary<string, IList<string>>();
      if (this.m_deploymentService == null || this.m_deploymentService.m_testLogStorageProviders == null)
        return forStorageAccounts;
      int num = 0;
      foreach (ITestLogStorageConnection logStorageProvider in this.m_deploymentService.m_testLogStorageProviders)
      {
        ++num;
        string accountName = logStorageProvider?.GetLogStoreConnectionEndpoint()?.GetAccountName();
        forStorageAccounts.Add(accountName == null ? string.Format("devstoreaccount{0}", (object) num) : accountName, logStorageProvider?.GetLogStoreConnectionEndpoint()?.GetCorsAllowedHostList());
      }
      return forStorageAccounts;
    }

    private void RegisterForStrongBoxItemChanges(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationStrongBoxService service = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
      service.RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
      {
        "TcmLogStoreConnectionString*"
      });
      try
      {
        service.RegisterNotification(systemRequestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxItemChanged), "ConfigurationSecrets", (IEnumerable<string>) new string[1]
        {
          "OmegaTcmLogStoreConnectionString*"
        });
      }
      catch (Exception ex)
      {
        systemRequestContext.Trace(1015679, TraceLevel.Error, "TestManagement", "LogStorage", "Unable to register to strong box item change notification. Error Details: " + ex.Message);
      }
    }

    private void OnStrongBoxItemChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      if (!itemNames.Any<StrongBoxItemName>((Func<StrongBoxItemName, bool>) (i => !i.LookupKey.EndsWith("-previous"))))
        return;
      this.InitializeTestLogStorageProviders(requestContext);
    }

    private void InitializeTestLogStorageProviders(IVssRequestContext systemContext)
    {
      List<string> connectionStrings = this.GetStorageConnectionStrings(systemContext, "TcmLogStoreConnectionString");
      this.m_testLogStorageProviders = this.GetTestLogStorageProviders(systemContext, connectionStrings);
      LogStoreStorageAccount.CreateTcmLogStoreStorageAccounts(systemContext, connectionStrings.Count);
      this.InitializeOmegaTestLogStorageProviders(systemContext);
    }

    private void InitializeOmegaTestLogStorageProviders(IVssRequestContext systemContext)
    {
      try
      {
        Dictionary<int, string> connectionStrings = this.GetOmegaStorageConnectionStrings(systemContext, "OmegaTcmLogStoreConnectionString");
        this.m_omegaTestLogStorageProvidersCache = this.GetOmegaTestLogStorageProviders(systemContext, connectionStrings);
      }
      catch (Exception ex)
      {
        systemContext.Trace(1015679, TraceLevel.Error, "TestManagement", "LogStorage", "No omega storage account has been configured, check the strong box items. Error Details: " + ex.Message);
      }
    }

    private ITestLogStorageConnection[] GetTestLogStorageProviders(
      IVssRequestContext systemContext,
      List<string> storageConnectionStrings)
    {
      ITestLogStorageConnection[] storageProviders = new ITestLogStorageConnection[storageConnectionStrings.Count];
      for (int index = 0; index < storageConnectionStrings.Count; ++index)
      {
        Dictionary<string, string> connectionSettings = new Dictionary<string, string>()
        {
          ["TestLogStorageAccountConnectionString"] = storageConnectionStrings[index]
        };
        ITestLogStorageConnection storageConnection = (ITestLogStorageConnection) new TestLogBlogStorageConnection(systemContext, connectionSettings);
        storageProviders[index] = storageConnection;
      }
      return storageProviders;
    }

    private Dictionary<int, ITestLogStorageConnection> GetOmegaTestLogStorageProviders(
      IVssRequestContext systemContext,
      Dictionary<int, string> storageConnectionStringsMap)
    {
      Dictionary<int, ITestLogStorageConnection> storageProviders = new Dictionary<int, ITestLogStorageConnection>();
      foreach (int key in storageConnectionStringsMap.Keys)
      {
        Dictionary<string, string> connectionSettings = new Dictionary<string, string>()
        {
          ["TestLogStorageAccountConnectionString"] = storageConnectionStringsMap[key]
        };
        ITestLogStorageConnection storageConnection = (ITestLogStorageConnection) new TestLogBlogStorageConnection(systemContext, connectionSettings);
        storageProviders[key] = storageConnection;
      }
      return storageProviders;
    }

    private List<string> GetStorageConnectionStrings(
      IVssRequestContext systemContext,
      string lookUpKeyPrefix)
    {
      List<string> stringList = new List<string>();
      int storageAccountIndex = 0;
      while (true)
      {
        string connectionString = this.GetStorageAccountConnectionString(systemContext, storageAccountIndex, lookUpKeyPrefix, false);
        if (connectionString != null)
        {
          stringList.Add(connectionString);
          ++storageAccountIndex;
        }
        else
          break;
      }
      return stringList.Count != 0 ? stringList : throw new TestManagementServiceException("No storage accounts configured. Check TcmLogStoreConnectionString keyvault secret.");
    }

    private Dictionary<int, string> GetOmegaStorageConnectionStrings(
      IVssRequestContext systemContext,
      string lookUpKeyPrefix)
    {
      List<string> stringList = new List<string>();
      Dictionary<int, string> connectionStrings = new Dictionary<int, string>();
      int num = 0;
      while (num < this.m_testLogStorageProviders.Length)
      {
        string connectionString = this.GetStorageAccountConnectionString(systemContext, num, lookUpKeyPrefix, false);
        if (connectionString == null)
        {
          ++num;
        }
        else
        {
          stringList.Add(connectionString);
          connectionStrings[num] = connectionString;
          ++num;
        }
      }
      return connectionStrings;
    }

    private string GetStorageAccountConnectionString(
      IVssRequestContext deploymentContext,
      int storageAccountIndex,
      string lookUpKeyPrefix,
      bool throwIfNotFound = true)
    {
      if (storageAccountIndex < 0)
        throw new ArgumentException("Invalid StorageAccountId! Should be greater or equal than 0");
      if (!deploymentContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        deploymentContext = deploymentContext.To(TeamFoundationHostType.Deployment);
      string connectionString = (string) null;
      ITeamFoundationStrongBoxService service = deploymentContext.GetService<ITeamFoundationStrongBoxService>();
      string lookupKey = string.Format("{0}{1}", (object) lookUpKeyPrefix, (object) storageAccountIndex);
      StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentContext, "ConfigurationSecrets", lookupKey, false);
      if (itemInfo == null)
      {
        if (throwIfNotFound)
          throw new StrongBoxItemNotFoundException(lookupKey);
      }
      else
        connectionString = service.GetString(deploymentContext, itemInfo);
      return connectionString;
    }

    private bool VerifyDomain(string corsDomainName)
    {
      if (corsDomainName.Equals("*"))
        return true;
      return new List<string>()
      {
        "https://dev.azure.com",
        "https://codeapp.ms",
        "https://codedev.ms"
      }.Contains<string>(corsDomainName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public ITestLogStorageConnection GetOmegaTestLogStoreConnection(
      IVssRequestContext requestContext,
      Guid projectId,
      ContainerScopeDetails scopeDetails)
    {
      try
      {
        if (this.m_deploymentService.m_omegaTestLogStorageProvidersCache == null || this.m_deploymentService.m_omegaTestLogStorageProvidersCache.Count == 0)
        {
          requestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "No Omega / Secondary storage accounts have been configured for the LoghStore accounts. Check if strong box items are populated.");
          return (ITestLogStorageConnection) null;
        }
        int connectionIndexOfArtifact = LogStoreStorageAccountMap.GetStorageAccountConnectionIndexOfArtifact(requestContext, projectId, scopeDetails);
        if (connectionIndexOfArtifact >= 0)
          return this.m_deploymentService.m_omegaTestLogStorageProvidersCache[connectionIndexOfArtifact];
        requestContext.Trace(1015679, TraceLevel.Warning, "TestManagement", "LogStorage", "No mapping found for the older artifacts.");
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015679, TraceLevel.Error, "TestManagement", "LogStorage", "No omega storage account has been configured, check the strong box items. Error Details: " + ex.Message);
      }
      return (ITestLogStorageConnection) null;
    }

    public List<GeoRedundantStorageAccountSettings> GetGeoRedundantStorageAccountSettings(
      IVssRequestContext requestContext)
    {
      List<GeoRedundantStorageAccountSettings> storageAccountSettings1 = new List<GeoRedundantStorageAccountSettings>();
      try
      {
        if (this.m_testLogStorageProviders == null || this.m_omegaTestLogStorageProvidersCache == null || this.m_testLogStorageProviders.Length == 0 || this.m_omegaTestLogStorageProvidersCache.Count <= 0)
          return storageAccountSettings1;
        for (int key = 0; key < this.m_testLogStorageProviders.Length; ++key)
        {
          if (this.m_omegaTestLogStorageProvidersCache.ContainsKey(key))
          {
            GeoRedundantStorageAccountSettings storageAccountSettings2 = new GeoRedundantStorageAccountSettings()
            {
              DrawerName = "ConfigurationSecrets",
              PrimaryLookupKey = "TcmLogStoreConnectionString" + key.ToString(),
              SecondaryLookupKey = "OmegaTcmLogStoreConnectionString" + key.ToString()
            };
            storageAccountSettings1.Add(storageAccountSettings2);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1015795, "TestManagement", "LogStorage", ex);
      }
      return storageAccountSettings1;
    }
  }
}
