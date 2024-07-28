// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TeamProjectSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.Catalog.Objects;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TeamProjectSettings
  {
    public Uri Guidance { get; private set; }

    public Uri Portal { get; private set; }

    public Uri ReportManagerFolderUrl { get; private set; }

    private TeamProjectSettings()
    {
    }

    public static TeamProjectSettings GetTeamProjectSettings(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext);
      TeamProjectSettings teamProjectSettings = (TeamProjectSettings) null;
      TeamProject projectById = new CatalogObjectContext(requestContext)
      {
        DefaultAccessMapping = accessMapping
      }.OrganizationalRoot.FindProjectById(projectId);
      if (projectById != null)
      {
        teamProjectSettings = new TeamProjectSettings();
        if (projectById.Guidance != null)
          teamProjectSettings.Guidance = projectById.Guidance.WellKnownGuidancePageUrl;
        if (projectById.Portal != null)
          teamProjectSettings.Portal = projectById.Portal.FullUrl;
        if (projectById.ReportFolder != null)
          teamProjectSettings.ReportManagerFolderUrl = projectById.ReportFolder.GetReportManagerFolderUrl();
      }
      return teamProjectSettings;
    }
  }
}
