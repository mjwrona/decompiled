// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.RequestContextExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class RequestContextExtensions
  {
    public static bool IsTeamFieldAreaPath(
      this IVssRequestContext requestContext,
      ProjectInfo project)
    {
      return requestContext.GetProjectProcessConfiguration(project.Uri, false).IsTeamFieldAreaPath();
    }

    public static ProjectProcessConfiguration GetProjectProcessSettings(
      this IVssRequestContext requestContext,
      ProjectInfo project)
    {
      return requestContext.GetProjectProcessSettings(project, true);
    }

    public static ProjectProcessConfiguration GetProjectProcessSettings(
      this IVssRequestContext requestContext,
      ProjectInfo project,
      bool validateSettings)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      return requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, project.Uri, validateSettings);
    }

    public static ProjectProcessConfiguration GetProjectProcessConfiguration(
      this IVssRequestContext requestContext,
      string projectUri)
    {
      return requestContext.GetProjectProcessConfiguration(projectUri, true);
    }

    public static ProjectProcessConfiguration GetProjectProcessConfiguration(
      this IVssRequestContext requestContext,
      string projectUri,
      bool validateSettings)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectUri, nameof (projectUri));
      return requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, projectUri, validateSettings);
    }
  }
}
