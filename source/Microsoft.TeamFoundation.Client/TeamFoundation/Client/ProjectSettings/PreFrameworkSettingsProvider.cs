// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProjectSettings.PreFrameworkSettingsProvider
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Common.Reporting;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Client.ProjectSettings
{
  internal class PreFrameworkSettingsProvider : ITpcSettingsAdapter
  {
    private TfsTeamProjectCollection m_tfs;

    public PreFrameworkSettingsProvider(TfsTeamProjectCollection teamProjectCollection) => this.m_tfs = teamProjectCollection;

    public void GetAllSettings(
      out ProjectCollectionSettings projectCollectionSettings,
      out ICollection<TeamProjectSettings> teamProjectSettings)
    {
      projectCollectionSettings = this.GetProjectCollectionSettings();
      teamProjectSettings = (ICollection<TeamProjectSettings>) this.InternalGetTeamProjectSettings();
    }

    private ProjectCollectionSettings GetProjectCollectionSettings()
    {
      string cubeDbName = (string) null;
      string cubeServerName = (string) null;
      string cubeConnectionString = (string) null;
      string warehouseDbName = (string) null;
      string warehouseServerName = (string) null;
      string warehouseConnectionString = (string) null;
      string reportFolder = (string) null;
      Uri uri1 = this.GetServiceLocation("Reports", "ReportsService");
      Uri serviceLocation = this.GetServiceLocation("Reports", "BaseReportsUrl");
      if (uri1 != (Uri) null && serviceLocation != (Uri) null)
      {
        uri1 = ReportingUtilities.RemoveKnownWebServicePaths(uri1);
        reportFolder = "/";
      }
      Uri uri2 = this.GetServiceLocation("Wss", "WssAdminService");
      if (uri2 != (Uri) null)
        uri2 = TFCommonUtil.StripEnd(uri2, SharePointConstants.KnownWebServicePaths);
      Database fromRegistration1 = this.GetDatabaseFromRegistration("BISANALYSIS DB");
      if (fromRegistration1 != null)
      {
        cubeDbName = fromRegistration1.DatabaseName;
        cubeServerName = fromRegistration1.SQLServerName;
        cubeConnectionString = fromRegistration1.ConnectionString;
      }
      Database fromRegistration2 = this.GetDatabaseFromRegistration("BISDW DB");
      if (fromRegistration2 != null)
      {
        cubeDbName = fromRegistration2.DatabaseName;
        cubeServerName = fromRegistration2.SQLServerName;
        cubeConnectionString = fromRegistration2.ConnectionString;
      }
      return new ProjectCollectionSettings(this.GetServiceLocation("Wss", "BaseSiteUrl"), uri2, reportFolder, uri1, serviceLocation, cubeDbName, cubeServerName, cubeConnectionString, warehouseDbName, warehouseServerName, warehouseConnectionString);
    }

    private Uri GetServiceLocation(string toolId, string serviceType)
    {
      try
      {
        return new Uri(RegistrationUtilities.GetServiceUrlForTool((TfsConnection) this.m_tfs, toolId, serviceType));
      }
      catch (TeamFoundationServerException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
      }
      return (Uri) null;
    }

    private Database GetDatabaseFromRegistration(string databaseType)
    {
      try
      {
        RegistrationEntry[] registrationEntries = ((IRegistration) this.m_tfs.GetService(typeof (IRegistration))).GetRegistrationEntries("vstfs");
        if (registrationEntries.Length != 1)
          return (Database) null;
        Database[] databases = registrationEntries[0].Databases;
        for (int index = 0; index < databases.Length; ++index)
        {
          if (VssStringComparer.DatabaseName.Equals(databaseType, databases[index].Name))
            return databases[index];
        }
      }
      catch (TeamFoundationServerException ex)
      {
        TeamFoundationTrace.TraceException((Exception) ex);
      }
      return (Database) null;
    }

    private List<TeamProjectSettings> InternalGetTeamProjectSettings()
    {
      ProjectInfo[] source = this.m_tfs.GetService<ICommonStructureService>().ListAllProjects();
      if (!((IEnumerable<ProjectInfo>) source).Any<ProjectInfo>())
        return new List<TeamProjectSettings>();
      Uri serviceLocation = this.GetServiceLocation("Wss", "BaseSiteUrl");
      List<TeamProjectSettings> teamProjectSettings = new List<TeamProjectSettings>();
      foreach (ProjectInfo projectInfo in source)
      {
        Guid id = Guid.Empty;
        ArtifactId artifactId = LinkingUtilities.DecodeUri(projectInfo.Uri);
        if (artifactId != null && artifactId.ToolSpecificId != null)
          id = new Guid(artifactId.ToolSpecificId);
        teamProjectSettings.Add(new TeamProjectSettings(id, projectInfo.Name, new Uri(projectInfo.Uri), projectInfo.Status, serviceLocation != (Uri) null ? UriUtility.Combine(serviceLocation, projectInfo.Name, true) : (Uri) null, true, true, serviceLocation != (Uri) null ? UriUtility.Combine(serviceLocation, UriUtility.CombinePath(projectInfo.Name, "Process Guidance"), true) : (Uri) null, (string) null, ProcessGuidanceType.WssDocumentLibrary, "/" + projectInfo.Name, (string) null, (string) null, (string) null, (string) null));
      }
      return teamProjectSettings;
    }
  }
}
