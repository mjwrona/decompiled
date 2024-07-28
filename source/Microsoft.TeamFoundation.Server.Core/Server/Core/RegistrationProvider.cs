// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.RegistrationProvider
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.Catalog.Objects;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class RegistrationProvider : 
    ITeamFoundationArtifactRegistrationService,
    IVssFrameworkService
  {
    private long m_collectionLocationLastChangeId = -1;
    private long m_appLocationLastChangeId = -1;
    private IVssRegistryService m_serviceSettings;
    private Dictionary<string, object> m_registryToolIds;
    private Dictionary<string, List<RegistrationExtendedAttribute2>> m_extendedAttributes;
    private bool m_registryEntriesFresh;
    private Dictionary<string, object> m_registrationToolIds;
    private Dictionary<string, List<RegistrationDatabase>> m_databases;
    private Dictionary<string, List<RegistrationArtifactType>> m_artifactTypes;
    private bool m_registrationEntriesFresh;
    private Dictionary<string, object> m_serviceToolIds;
    private Dictionary<string, List<RegistrationServiceInterface>> m_serviceInterfaces;
    private bool m_catalogServiceInterfacesFresh;
    private Dictionary<string, FrameworkRegistrationEntry> m_compiledRegistrationEntries;
    private Dictionary<string, FrameworkRegistrationEntry> m_compiledRegistrationEntriesForClientOM;
    private bool m_compiledRegistrationEntriesFresh;
    private Guid m_instanceId;
    private ReaderWriterLock m_accessLock = new ReaderWriterLock();
    private static readonly string[] s_notificationFilters = new string[2]
    {
      string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/...", (object) FrameworkServerConstants.DatabaseRoot),
      string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/...", (object) FrameworkServerConstants.RegistrationRoot)
    };
    private static readonly string s_databaseCategoryFormat = FrameworkServerConstants.RegistrationRoot + "/Database/{0}/{1}/Category";
    private static readonly string s_databaseServerFormat = FrameworkServerConstants.RegistrationRoot + "/Database/{0}/{1}/Server";
    private static readonly string s_databaseNameFormat = FrameworkServerConstants.RegistrationRoot + "/Database/{0}/{1}/DatabaseName";
    private static readonly string s_databaseConnectionStringFormat = FrameworkServerConstants.RegistrationRoot + "/Database/{0}/{1}/ConnectionString";
    private static readonly string s_databaseExcludeFromBackupFormat = FrameworkServerConstants.RegistrationRoot + "/Database/{0}/{1}/ExcludeFromBackup";
    private static readonly string s_databaseQueryString = FrameworkServerConstants.RegistrationRoot + "/Database/{0}/{1}/...";
    private static readonly string s_registrationEntriesQueryString = FrameworkServerConstants.RegistrationRoot + "/...";
    private static readonly string s_witOmExpirationSettingQueryString = "/Service/WorkItemTracking/Settings/WitOMExpirationDate";
    private static readonly string s_registrationExtendedAttribute = "RegistrationExtendedAttribute";
    private const string c_workingItemTackingPath = "/WorkItemTracking/v1.0/AttachFileHandler.ashx";
    private const string c_artifactDisplayUrlKey = "ArtifactDisplayUrl";
    private const string c_attachmentServerUrlKey = "AttachmentServerUrl";
    private const string c_witOmExpirationDateKey = "WitOMExpirationDate";
    private static readonly string s_buildCategory = "Build";
    private static readonly string s_integrationCategory = "Integration";
    private static readonly string s_versionControlCategory = "VersionControl";
    private static readonly string s_workItemCategory = "WorkItem";
    private static readonly string s_workItemAttachmentCategory = "WorkItemAttachment";
    private static readonly IReadOnlyCollection<string> s_toolsExcludedForClientOM = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "GitHub"
    };
    internal const string c_isClientOmKey = "IsClientOm";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      this.m_instanceId = systemRequestContext.ServiceHost.InstanceId;
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Application);
      this.SetServiceSettings(systemRequestContext);
      vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(vssRequestContext, "Default", SqlNotificationEventClasses.CatalogDataChanged, new SqlNotificationCallback(this.CatalogSettingsChanged), false);
    }

    private void SetServiceSettings(IVssRequestContext systemRequestContext)
    {
      if (!this.IsDeploymentLevelRegistrationEntriesEnabled(systemRequestContext))
        this.m_serviceSettings = systemRequestContext.GetService<IVssRegistryService>();
      else
        this.m_serviceSettings = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      IVssRequestContext vssRequestContext = systemRequestContext.To(TeamFoundationHostType.Application);
      vssRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(vssRequestContext, "Default", SqlNotificationEventClasses.CatalogDataChanged, new SqlNotificationCallback(this.CatalogSettingsChanged), false);
    }

    public FrameworkRegistrationEntry[] GetRegistrationEntries(
      IVssRequestContext requestContext,
      string toolId)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      AccessMapping accessMapping = service.DetermineAccessMapping(requestContext) ?? service.GetServerAccessMapping(requestContext);
      return this.GetRegistrationEntries(requestContext, toolId, accessMapping);
    }

    private FrameworkRegistrationEntry[] GetRegistrationEntries(
      IVssRequestContext requestContext,
      string toolId,
      AccessMapping accessMapping)
    {
      this.EnsureEntriesCompiled(requestContext, accessMapping);
      try
      {
        this.m_accessLock.AcquireReaderLock(-1);
        Dictionary<string, FrameworkRegistrationEntry> dictionary = requestContext.Items.GetCastedValueOrDefault<string, bool>("IsClientOm") ? this.m_compiledRegistrationEntriesForClientOM : this.m_compiledRegistrationEntries;
        FrameworkRegistrationEntry[] registrationEntries;
        if (string.IsNullOrEmpty(toolId))
        {
          int index = 0;
          registrationEntries = new FrameworkRegistrationEntry[dictionary.Values.Count];
          foreach (FrameworkRegistrationEntry registrationEntry in dictionary.Values)
          {
            registrationEntries[index] = registrationEntry.Clone();
            ++index;
          }
        }
        else
        {
          FrameworkRegistrationEntry registrationEntry;
          if (dictionary.TryGetValue(toolId, out registrationEntry))
            registrationEntries = new FrameworkRegistrationEntry[1]
            {
              registrationEntry.Clone()
            };
          else
            registrationEntries = Array.Empty<FrameworkRegistrationEntry>();
        }
        return registrationEntries;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    internal List<RegistrationServiceInterface> GetServiceInterfaces(
      IVssRequestContext requestContext,
      string toolId,
      AccessMapping accessMapping)
    {
      try
      {
        this.EnsureServicesLoaded(requestContext, accessMapping);
        this.m_accessLock.AcquireReaderLock(-1);
        List<RegistrationServiceInterface> serviceInterfaces;
        if (string.IsNullOrEmpty(toolId))
        {
          serviceInterfaces = new List<RegistrationServiceInterface>();
          foreach (List<RegistrationServiceInterface> collection in this.m_serviceInterfaces.Values)
            serviceInterfaces.AddRange((IEnumerable<RegistrationServiceInterface>) collection);
        }
        else if (!this.m_serviceInterfaces.TryGetValue(toolId, out serviceInterfaces))
          serviceInterfaces = new List<RegistrationServiceInterface>();
        return serviceInterfaces;
      }
      finally
      {
        if (this.m_accessLock.IsReaderLockHeld)
          this.m_accessLock.ReleaseReaderLock();
      }
    }

    private void LoadArtifactTypes(IVssRequestContext requestContext)
    {
      this.m_artifactTypes = requestContext.GetService<IArtifactLinkTypesService>().GetArtifactLinkTypes(requestContext);
      foreach (string key in this.m_artifactTypes.Keys)
        this.m_registrationToolIds[key] = (object) null;
    }

    private void CatalogSettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      this.m_catalogServiceInterfacesFresh = false;
      this.m_registryEntriesFresh = false;
      this.m_compiledRegistrationEntriesFresh = false;
    }

    private int CompareRegistryEntriesByPath(RegistryEntry entry1, RegistryEntry entry2) => VssStringComparer.RegistryPath.Compare(entry1.Path, entry2.Path);

    private string[] GetEntryInformation(
      RegistryEntry registryEntry,
      out string toolId,
      out string entryType)
    {
      string[] entryInformation = registryEntry.Path.Split('/');
      entryType = entryInformation[3];
      toolId = entryInformation[4];
      return entryInformation;
    }

    private void CompileVirtualizedRegistrationEntries(IVssRequestContext requestContext)
    {
      this.m_registryToolIds = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.ToolId);
      this.m_extendedAttributes = new Dictionary<string, List<RegistrationExtendedAttribute2>>((IEqualityComparer<string>) VssStringComparer.ToolId);
      string str1 = requestContext.ServiceHost.GetCulture(requestContext).LCID.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      string str2 = "/WorkItemTracking/v1.0/AttachFileHandler.ashx";
      RegistrationExtendedAttribute2 extendedAttribute2_1 = new RegistrationExtendedAttribute2("ArtifactDisplayUrl", "");
      RegistrationExtendedAttribute2 extendedAttribute2_2 = new RegistrationExtendedAttribute2("AttachmentServerUrl", str2);
      this.m_extendedAttributes.Add("vstfs", new List<RegistrationExtendedAttribute2>()
      {
        new RegistrationExtendedAttribute2(ServicingTokenConstants.InstalledUICulture, str1)
      });
      this.m_extendedAttributes.Add("WorkItemTracking", new List<RegistrationExtendedAttribute2>()
      {
        extendedAttribute2_2,
        extendedAttribute2_1
      });
      this.m_extendedAttributes.Add("VersionControl", new List<RegistrationExtendedAttribute2>()
      {
        extendedAttribute2_1
      });
      this.m_extendedAttributes.Add("Requirements", new List<RegistrationExtendedAttribute2>()
      {
        extendedAttribute2_1
      });
      this.m_extendedAttributes.Add("Build", new List<RegistrationExtendedAttribute2>()
      {
        extendedAttribute2_1
      });
      this.m_registryToolIds["Build"] = (object) null;
      this.m_registryToolIds["vstfs"] = (object) null;
      this.m_registryToolIds["VersionControl"] = (object) null;
      this.m_registryToolIds["Requirements"] = (object) null;
      this.m_registryToolIds["WorkItemTracking"] = (object) null;
      this.m_databases = new Dictionary<string, List<RegistrationDatabase>>((IEqualityComparer<string>) VssStringComparer.ToolId);
    }

    private void EnsureRegistryLoaded(IVssRequestContext requestContext)
    {
      if (this.m_registryEntriesFresh)
        return;
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (this.m_registryEntriesFresh)
          return;
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnableRegistrationEntriesVirtualization"))
        {
          this.CompileVirtualizedRegistrationEntries(requestContext);
          this.AddWitOMExpirationDate(requestContext.Elevate());
        }
        else
        {
          this.m_compiledRegistrationEntriesFresh = false;
          this.m_registryToolIds = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.ToolId);
          this.m_databases = new Dictionary<string, List<RegistrationDatabase>>((IEqualityComparer<string>) VssStringComparer.ToolId);
          this.m_extendedAttributes = new Dictionary<string, List<RegistrationExtendedAttribute2>>((IEqualityComparer<string>) VssStringComparer.ToolId);
          this.m_databases["Build"] = new List<RegistrationDatabase>()
          {
            this.BuildRegistrationDatabase(requestContext, RegistrationProvider.s_buildCategory, "TeamBuild DB")
          };
          this.m_registryToolIds["Build"] = (object) null;
          this.m_databases["VersionControl"] = new List<RegistrationDatabase>()
          {
            this.BuildRegistrationDatabase(requestContext, RegistrationProvider.s_versionControlCategory, "VersionControl DB")
          };
          this.m_registryToolIds["VersionControl"] = (object) null;
          this.m_databases["vstfs"] = new List<RegistrationDatabase>()
          {
            this.BuildRegistrationDatabase(requestContext, RegistrationProvider.s_integrationCategory, "BIS DB")
          };
          this.m_registryToolIds["vstfs"] = (object) null;
          this.m_databases["WorkItemTracking"] = new List<RegistrationDatabase>()
          {
            this.BuildRegistrationDatabase(requestContext, RegistrationProvider.s_workItemCategory, "WIT DB"),
            this.BuildRegistrationDatabase(requestContext, RegistrationProvider.s_workItemAttachmentCategory, "WITAttachments DB")
          };
          this.m_registryToolIds["WorkItemTracking"] = (object) null;
          List<RegistryEntry> registryEntryList = new List<RegistryEntry>();
          foreach (RegistryEntry registryEntry in this.IsDeploymentLevelRegistrationEntriesEnabled(requestContext) ? this.m_serviceSettings.ReadEntries(requestContext.To(TeamFoundationHostType.Deployment).Elevate(), (RegistryQuery) RegistrationProvider.s_registrationEntriesQueryString) : this.m_serviceSettings.ReadEntries(requestContext.Elevate(), (RegistryQuery) RegistrationProvider.s_registrationEntriesQueryString))
            registryEntryList.Add(registryEntry);
          registryEntryList.Sort(new Comparison<RegistryEntry>(this.CompareRegistryEntriesByPath));
          for (int index = 0; index < registryEntryList.Count; ++index)
          {
            string toolId;
            string entryType;
            this.GetEntryInformation(registryEntryList[index], out toolId, out entryType);
            this.m_registryToolIds[toolId] = (object) null;
            if (VssStringComparer.RegistryPath.Equals(entryType, RegistrationProvider.s_registrationExtendedAttribute))
            {
              RegistrationExtendedAttribute2 extendedAttribute2 = new RegistrationExtendedAttribute2();
              extendedAttribute2.Name = registryEntryList[index].Name;
              extendedAttribute2.Value = registryEntryList[index].Value;
              extendedAttribute2.SourceRegistryPath = registryEntryList[index].Path;
              List<RegistrationExtendedAttribute2> extendedAttribute2List;
              if (!this.m_extendedAttributes.TryGetValue(toolId, out extendedAttribute2List))
              {
                extendedAttribute2List = new List<RegistrationExtendedAttribute2>();
                this.m_extendedAttributes[toolId] = extendedAttribute2List;
              }
              extendedAttribute2List.Add(extendedAttribute2);
            }
          }
          RegistrationExtendedAttribute2 extendedAttribute2_1 = new RegistrationExtendedAttribute2();
          extendedAttribute2_1.Name = "InstanceId";
          extendedAttribute2_1.Value = this.m_instanceId.ToString();
          extendedAttribute2_1.SourceRegistryPath = (string) null;
          List<RegistrationExtendedAttribute2> extendedAttribute2List1;
          if (!this.m_extendedAttributes.TryGetValue("vstfs", out extendedAttribute2List1))
          {
            extendedAttribute2List1 = new List<RegistrationExtendedAttribute2>();
            this.m_extendedAttributes["vstfs"] = extendedAttribute2List1;
          }
          extendedAttribute2List1.Add(extendedAttribute2_1);
          this.AddWitOMExpirationDate(requestContext.Elevate());
          this.m_registryEntriesFresh = true;
        }
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    private void AddWitOMExpirationDate(IVssRequestContext elevatedContext)
    {
      string str = this.m_serviceSettings.GetValue(elevatedContext, (RegistryQuery) RegistrationProvider.s_witOmExpirationSettingQueryString, true, (string) null);
      List<RegistrationExtendedAttribute2> extendedAttribute2List;
      if (string.IsNullOrEmpty(str) || this.m_extendedAttributes == null || !this.m_extendedAttributes.TryGetValue("WorkItemTracking", out extendedAttribute2List))
        return;
      extendedAttribute2List.Add(new RegistrationExtendedAttribute2()
      {
        Name = "WitOMExpirationDate",
        Value = str,
        SourceRegistryPath = (string) null
      });
    }

    private RegistrationDatabase BuildRegistrationDatabase(
      string registrationName,
      string connectionString)
    {
      RegistrationDatabase registrationDatabase = new RegistrationDatabase();
      registrationDatabase.Name = registrationName;
      registrationDatabase.SourceRegistryPath = (string) null;
      registrationDatabase.ConnectionString = connectionString;
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
      registrationDatabase.DatabaseName = connectionStringBuilder.InitialCatalog;
      registrationDatabase.SQLServerName = connectionStringBuilder.DataSource;
      registrationDatabase.ExcludeFromBackup = false;
      return registrationDatabase;
    }

    private RegistrationDatabase BuildRegistrationDatabase(
      IVssRequestContext requestContext,
      string category,
      string registrationName)
    {
      RegistrationDatabase registrationDatabase = new RegistrationDatabase();
      registrationDatabase.Name = registrationName;
      ISqlConnectionInfo frameworkConnectionInfo = requestContext.FrameworkConnectionInfo;
      registrationDatabase.ConnectionString = frameworkConnectionInfo == null ? (string) null : frameworkConnectionInfo.ConnectionString;
      SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(registrationDatabase.ConnectionString);
      registrationDatabase.DatabaseName = connectionStringBuilder.InitialCatalog;
      registrationDatabase.SQLServerName = connectionStringBuilder.DataSource;
      registrationDatabase.ExcludeFromBackup = false;
      return registrationDatabase;
    }

    private void EnsureRegistrationLoaded(IVssRequestContext requestContext)
    {
      if (this.m_registrationEntriesFresh)
        return;
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (this.m_registrationEntriesFresh)
          return;
        this.m_compiledRegistrationEntriesFresh = false;
        this.m_registrationToolIds = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.ToolId);
        this.LoadArtifactTypes(requestContext);
        this.m_registrationEntriesFresh = true;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    private void EnsureServicesLoaded(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      requestContext = requestContext.Elevate();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      ILocationService service1 = vssRequestContext.GetService<ILocationService>();
      ILocationService service2 = requestContext.GetService<ILocationService>();
      long lastChangeId1 = service2.GetLastChangeId(requestContext);
      long lastChangeId2 = service1.GetLastChangeId(vssRequestContext);
      if (lastChangeId1 == this.m_collectionLocationLastChangeId && lastChangeId2 == this.m_appLocationLastChangeId && this.m_catalogServiceInterfacesFresh)
        return;
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (lastChangeId1 == this.m_collectionLocationLastChangeId && lastChangeId2 == this.m_appLocationLastChangeId && this.m_catalogServiceInterfacesFresh)
          return;
        this.m_compiledRegistrationEntriesFresh = false;
        this.m_serviceInterfaces = new Dictionary<string, List<RegistrationServiceInterface>>((IEqualityComparer<string>) VssStringComparer.ToolId);
        this.m_serviceToolIds = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.ToolId);
        this.m_collectionLocationLastChangeId = lastChangeId1;
        this.m_appLocationLastChangeId = lastChangeId2;
        IEnumerable<ServiceDefinition> definitionsByToolId1 = service2.FindServiceDefinitionsByToolId(requestContext, (string) null);
        this.BuildServiceInterfaces(requestContext, accessMapping, definitionsByToolId1);
        IEnumerable<ServiceDefinition> definitionsByToolId2 = service1.FindServiceDefinitionsByToolId(vssRequestContext, "TSWebAccess");
        this.BuildServiceInterfaces(vssRequestContext, accessMapping, definitionsByToolId2);
        this.LoadServiceInterfacesFromCatalog(requestContext, accessMapping);
        this.m_catalogServiceInterfacesFresh = true;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    private void BuildServiceInterfaces(
      IVssRequestContext requestContext,
      AccessMapping accessMapping,
      IEnumerable<ServiceDefinition> services)
    {
      ILocationService service1 = requestContext.GetService<ILocationService>();
      foreach (ServiceDefinition service2 in services)
      {
        this.m_serviceToolIds[service2.ToolId] = (object) null;
        string serviceType = service2.ServiceType;
        string url = service2.RelativeToSetting == RelativeToSetting.Context ? service2.RelativePath : service1.LocationForAccessMapping(requestContext, service2, accessMapping);
        if (url != null)
        {
          RegistrationServiceInterface serviceInterface = new RegistrationServiceInterface(serviceType, url);
          List<RegistrationServiceInterface> serviceInterfaceList;
          if (!this.m_serviceInterfaces.TryGetValue(service2.ToolId, out serviceInterfaceList))
          {
            serviceInterfaceList = new List<RegistrationServiceInterface>();
            this.m_serviceInterfaces[service2.ToolId] = serviceInterfaceList;
          }
          serviceInterfaceList.Add(serviceInterface);
        }
      }
    }

    private void LoadServiceInterfacesFromCatalog(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Application).Elevate();
      if (requestContext.ServiceHost.DeploymentServiceHost.IsHosted)
      {
        this.m_serviceToolIds["Reports"] = (object) null;
        this.m_serviceToolIds["Wss"] = (object) null;
        this.m_serviceToolIds["TeamProjects"] = (object) null;
      }
      else
      {
        ProjectCollection projectCollectionById = new CatalogObjectContext(requestContext1)
        {
          DefaultAccessMapping = accessMapping
        }.OrganizationalRoot.FindProjectCollectionById(this.m_instanceId);
        if (projectCollectionById == null)
          throw new TeamFoundationServerException(TFCommonResources.EntityModel_CollectionOrganizationalNodeNotFound());
        projectCollectionById.Preload();
        this.LoadSharePointInterfaces(projectCollectionById);
        this.LoadReportingInterfaces(projectCollectionById);
        this.LoadTeamProjectsInterfaces(projectCollectionById);
      }
    }

    private void LoadSharePointInterfaces(ProjectCollection projectCollection)
    {
      this.m_serviceToolIds["Wss"] = (object) null;
      string url1 = string.Empty;
      string url2 = string.Empty;
      string url3 = string.Empty;
      string url4 = string.Empty;
      if (projectCollection.LocationForNewWssSites != null)
      {
        SharePointSiteCreationLocation locationForNewWssSites = projectCollection.LocationForNewWssSites;
        SharePointWebApplication webApplication = locationForNewWssSites.WebApplication;
        if (webApplication != null)
        {
          url1 = webApplication.RootUrlServiceLocation.AbsoluteUri.Trim('/');
          url2 = locationForNewWssSites.FullUrl.AbsoluteUri;
          url3 = locationForNewWssSites.FullyQualifiedUncPath;
          if (webApplication.AdminUrlServiceLocation != (Uri) null)
            url4 = UriUtility.Combine(webApplication.AdminUrlServiceLocation.AbsoluteUri, "_vti_adm/Admin.asmx", false).AbsoluteUri;
        }
      }
      this.m_serviceInterfaces["Wss"] = new List<RegistrationServiceInterface>()
      {
        new RegistrationServiceInterface("BaseServerUrl", url1),
        new RegistrationServiceInterface("BaseSiteUrl", url2),
        new RegistrationServiceInterface("BaseSiteUnc", url3),
        new RegistrationServiceInterface("WssAdminService", url4)
      };
    }

    private void LoadReportingInterfaces(ProjectCollection projectCollection)
    {
      this.m_serviceToolIds["Reports"] = (object) null;
      string url1 = string.Empty;
      string url2 = string.Empty;
      ReportingServer reportingServer = (ReportingServer) null;
      if (projectCollection.ReportFolder != null)
        reportingServer = projectCollection.ReportFolder.GetReportServer();
      if (reportingServer != null && reportingServer.ReportsManagerServiceLocation != (Uri) null && reportingServer.ReportServerServiceLocation != (Uri) null)
      {
        url1 = reportingServer.ReportsManagerServiceLocation.AbsoluteUri;
        url2 = UriUtility.Combine(reportingServer.ReportServerServiceLocation.AbsoluteUri, "ReportService2005.asmx", false).AbsoluteUri;
      }
      this.m_serviceInterfaces["Reports"] = new List<RegistrationServiceInterface>()
      {
        new RegistrationServiceInterface("BaseReportsUrl", url1),
        new RegistrationServiceInterface("ReportWebServiceUrl", url2)
      };
    }

    private void LoadTeamProjectsInterfaces(ProjectCollection projectCollection)
    {
      this.m_serviceToolIds["TeamProjects"] = (object) null;
      List<RegistrationServiceInterface> interfaces = new List<RegistrationServiceInterface>();
      foreach (TeamProject project in (IEnumerable<TeamProject>) projectCollection.Projects)
      {
        if (project.Portal != null)
        {
          this.AddTeamProjectServiceInterface(interfaces, project, "{0}:Portal", project.Portal.FullUrl.AbsoluteUri);
          this.AddTeamProjectServiceInterface(interfaces, project, "{0}:PortalType", project.Portal.ResourceSubType.ToString());
        }
        if (project.Guidance != null)
        {
          this.AddTeamProjectServiceInterface(interfaces, project, "{0}:ProcessGuidance", project.Guidance.FullUrl.AbsoluteUri);
          this.AddTeamProjectServiceInterface(interfaces, project, "{0}:ProcessGuidanceType", project.Guidance.ResourceSubType.ToString());
        }
        if (project.ReportFolder != null)
          this.AddTeamProjectServiceInterface(interfaces, project, "{0}:ReportFolder", project.ReportFolder.FullPath);
      }
      this.m_serviceInterfaces["TeamProjects"] = interfaces;
    }

    private void AddTeamProjectServiceInterface(
      List<RegistrationServiceInterface> interfaces,
      TeamProject project,
      string interfaceNameFormatString,
      string interfaceValue)
    {
      interfaces.Add(new RegistrationServiceInterface(string.Format((IFormatProvider) CultureInfo.InvariantCulture, interfaceNameFormatString, (object) project.ProjectName), interfaceValue)
      {
        ProjectUri = project.ProjectUri.AbsoluteUri
      });
    }

    private void EnsureEntriesCompiled(
      IVssRequestContext requestContext,
      AccessMapping accessMapping)
    {
      this.EnsureRegistryLoaded(requestContext);
      this.EnsureServicesLoaded(requestContext, accessMapping);
      this.EnsureRegistrationLoaded(requestContext);
      if (this.m_compiledRegistrationEntriesFresh)
        return;
      try
      {
        this.m_accessLock.AcquireWriterLock(-1);
        if (this.m_compiledRegistrationEntriesFresh)
          return;
        this.m_compiledRegistrationEntries = new Dictionary<string, FrameworkRegistrationEntry>((IEqualityComparer<string>) VssStringComparer.ToolId);
        this.m_compiledRegistrationEntriesForClientOM = new Dictionary<string, FrameworkRegistrationEntry>((IEqualityComparer<string>) VssStringComparer.ToolId);
        HashSet<string> collection = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.ToolId);
        collection.AddRange<string, HashSet<string>>((IEnumerable<string>) this.m_registryToolIds.Keys);
        collection.AddRange<string, HashSet<string>>((IEnumerable<string>) this.m_serviceToolIds.Keys);
        collection.AddRange<string, HashSet<string>>((IEnumerable<string>) this.m_registrationToolIds.Keys);
        foreach (string key in collection)
        {
          FrameworkRegistrationEntry entry = new FrameworkRegistrationEntry();
          entry.Type = key;
          List<RegistrationDatabase> registrationDatabaseList;
          entry.Databases = !this.m_databases.TryGetValue(key, out registrationDatabaseList) ? Array.Empty<RegistrationDatabase>() : registrationDatabaseList.ToArray();
          List<RegistrationArtifactType> registrationArtifactTypeList;
          entry.ArtifactTypes = !this.m_artifactTypes.TryGetValue(key, out registrationArtifactTypeList) ? Array.Empty<RegistrationArtifactType>() : registrationArtifactTypeList.ToArray();
          List<RegistrationExtendedAttribute2> extendedAttribute2List;
          entry.RegistrationExtendedAttributes = !this.m_extendedAttributes.TryGetValue(key, out extendedAttribute2List) ? Array.Empty<RegistrationExtendedAttribute2>() : extendedAttribute2List.ToArray();
          List<RegistrationServiceInterface> serviceInterfaceList;
          entry.ServiceInterfaces = !this.m_serviceInterfaces.TryGetValue(key, out serviceInterfaceList) ? Array.Empty<RegistrationServiceInterface>() : serviceInterfaceList.ToArray();
          this.m_compiledRegistrationEntries[key] = entry;
          this.m_compiledRegistrationEntriesForClientOM[key] = RegistrationProvider.FilterLinkTypesNotVisibleToClientOM(entry);
        }
        this.m_compiledRegistrationEntriesFresh = true;
      }
      finally
      {
        if (this.m_accessLock.IsWriterLockHeld)
          this.m_accessLock.ReleaseWriterLock();
      }
    }

    private static FrameworkRegistrationEntry FilterLinkTypesNotVisibleToClientOM(
      FrameworkRegistrationEntry entry)
    {
      entry = entry.DeepCloneForArtifactTypes();
      foreach (RegistrationArtifactType artifactType in entry.ArtifactTypes)
        artifactType.OutboundLinkTypes = ((IEnumerable<OutboundLinkType>) artifactType.OutboundLinkTypes).Where<OutboundLinkType>((Func<OutboundLinkType, bool>) (t => !RegistrationProvider.s_toolsExcludedForClientOM.Contains<string>(t.TargetArtifactTypeTool))).ToArray<OutboundLinkType>();
      return entry;
    }

    private bool IsDeploymentLevelRegistrationEntriesEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Framework.EnableDeploymentRegistrationEntries") && requestContext.ServiceHost.DeploymentServiceHost.IsHosted;

    internal IVssRegistryService GetServiceSettings() => this.m_serviceSettings;
  }
}
