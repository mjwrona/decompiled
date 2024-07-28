// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProjectSettings.FrameworkSettingsProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Client.Catalog.Objects;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Client.ProjectSettings
{
  internal class FrameworkSettingsProvider : ITpcSettingsAdapter
  {
    private TfsTeamProjectCollection m_tfs;
    private ICatalogService m_catalogService;

    public FrameworkSettingsProvider(TfsTeamProjectCollection teamProjectCollection)
    {
      this.m_tfs = teamProjectCollection;
      this.m_catalogService = teamProjectCollection.ConfigurationServer.GetService<ICatalogService>();
    }

    public void GetAllSettings(
      out ProjectCollectionSettings projectCollectionSettings,
      out ICollection<TeamProjectSettings> teamProjectSettings)
    {
      CatalogBulkData catalogBulkData = this.GetCatalogBulkData();
      CatalogObjectContext catalogObjectContext = new CatalogObjectContext(this.m_catalogService);
      OrganizationalRoot catalogObject1 = catalogObjectContext.CreateCatalogObject<OrganizationalRoot>(catalogBulkData, CatalogRoots.OrganizationalPath);
      ProjectCollection catalogObject2 = catalogObjectContext.CreateCatalogObject<ProjectCollection>(catalogBulkData, this.m_tfs.CatalogNode.FullPath);
      projectCollectionSettings = catalogObject2 == null ? (ProjectCollectionSettings) null : this.GetProjectCollectionSettings(catalogObject2, catalogObject1);
      ICollection<TeamProject> projects = catalogObject2.Projects;
      teamProjectSettings = (ICollection<TeamProjectSettings>) projects.Select<TeamProject, TeamProjectSettings>((Func<TeamProject, TeamProjectSettings>) (t => this.CreateTeamProjectSettings(t))).ToList<TeamProjectSettings>();
    }

    private TeamProjectSettings CreateTeamProjectSettings(TeamProject teamProject) => new TeamProjectSettings(teamProject.ProjectId, teamProject.ProjectName, teamProject.ProjectUri, teamProject.ProjectState, teamProject.Portal == null ? (Uri) null : teamProject.Portal.FullUrl, teamProject.PortalIsSharePoint, teamProject.IsOwnerOfSharePointPortal, teamProject.Guidance == null ? (Uri) null : teamProject.Guidance.FullUrl, teamProject.Guidance == null ? (string) null : teamProject.Guidance.GuidanceFileName, teamProject.Guidance == null ? ProcessGuidanceType.WssDocumentLibrary : teamProject.Guidance.ResourceSubType, teamProject.ReportFolder == null ? (string) null : teamProject.ReportFolder.FullPath, teamProject.CatalogNodeFullPath, teamProject.SourceControlCapabilityFlags, teamProject.SourceControlGitEnabled, teamProject.SourceControlTfvcEnabled);

    private ProjectCollectionSettings GetProjectCollectionSettings(
      ProjectCollection projectCollection,
      OrganizationalRoot organizationalRoot)
    {
      Uri siteCreationLocation = (Uri) null;
      Uri adminUri = (Uri) null;
      if (projectCollection.LocationForNewWssSites != null)
      {
        siteCreationLocation = projectCollection.LocationForNewWssSites.FullyQualifiedUrl;
        adminUri = projectCollection.LocationForNewWssSites.ReferencedResource.As<SharePointWebApplication>().AdminUrlServiceLocation;
      }
      string reportFolder = (string) null;
      Uri reportServerUri = (Uri) null;
      Uri reportManagerUri = (Uri) null;
      if (projectCollection.ReportFolder != null)
      {
        reportFolder = projectCollection.ReportFolder.FullPath;
        ReportingServer reportServer = projectCollection.ReportFolder.GetReportServer();
        if (reportServer != null)
        {
          reportServerUri = reportServer.ReportServerServiceLocation;
          reportManagerUri = reportServer.ReportsManagerServiceLocation;
        }
      }
      string cubeDbName = string.Empty;
      string cubeServerName = string.Empty;
      string cubeConnectionString = string.Empty;
      string warehouseDbName = string.Empty;
      string warehouseServerName = string.Empty;
      string warehouseConnectionString = string.Empty;
      ReportingConfiguration reportingConfiguration = organizationalRoot.ReportingConfiguration;
      if (reportingConfiguration != null)
      {
        SqlAnalysisDatabase reportingCube = reportingConfiguration.ReportingCube;
        if (reportingCube != null)
        {
          cubeDbName = reportingCube.InitialCatalog;
          SqlAnalysisInstance databaseInstance = reportingCube.DatabaseInstance;
          if (databaseInstance != null)
            cubeServerName = databaseInstance.ServerName;
          cubeConnectionString = reportingCube.GetConnectionString();
        }
        WarehouseDatabase reportingWarehouse = reportingConfiguration.ReportingWarehouse;
        if (reportingWarehouse != null)
        {
          warehouseDbName = reportingWarehouse.InitialCatalog;
          SqlReportingInstance databaseInstance = reportingWarehouse.DatabaseInstance;
          if (databaseInstance != null)
            warehouseServerName = databaseInstance.ServerName;
          warehouseConnectionString = reportingWarehouse.GetConnectionString();
        }
      }
      return new ProjectCollectionSettings(siteCreationLocation, adminUri, reportFolder, reportServerUri, reportManagerUri, cubeDbName, cubeServerName, cubeConnectionString, warehouseDbName, warehouseServerName, warehouseConnectionString);
    }

    private CatalogBulkData GetCatalogBulkData()
    {
      List<Guid> settingsResourceTypes = ProjectSettingsCatalogHelper.GetProjectSettingsResourceTypes<TeamProject>(!this.m_tfs.IsHostedServer);
      ReadOnlyCollection<CatalogNode> nodes = this.m_catalogService.QueryNodes((IEnumerable<string>) ProjectSettingsCatalogHelper.GetProjectCollectionPathSpecs(this.m_tfs.CatalogNode.FullPath), (IEnumerable<Guid>) settingsResourceTypes, CatalogQueryOptions.ExpandDependencies | CatalogQueryOptions.IncludeParents);
      settingsResourceTypes.Add(CatalogResourceTypes.OrganizationalRoot);
      List<Guid> queriedResourceTypes = settingsResourceTypes;
      return new CatalogBulkData((ICollection<CatalogNode>) nodes, (ICollection<Guid>) queriedResourceTypes);
    }
  }
}
