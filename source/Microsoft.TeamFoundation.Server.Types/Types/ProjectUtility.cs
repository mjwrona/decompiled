// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectUtility
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Types
{
  public class ProjectUtility
  {
    public static void SetupProjectContext(IVssRequestContext requestContext, ProjectInfo project)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProjectInfo>(project, nameof (project));
      requestContext.RootContext.Items["RequestProject"] = (object) project;
      if (project.Visibility != ProjectVisibility.Public)
        return;
      requestContext.SetLicenseForPublicResource();
    }

    public static ProjectInfo GetProjectFromRoute(
      IVssRequestContext requestContext,
      IDictionary<string, object> routeValues)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(routeValues, nameof (routeValues));
      ProjectInfo project = (ProjectInfo) null;
      string projFilter;
      if (routeValues.TryGetValue<string>("project", out projFilter) && !string.IsNullOrEmpty(projFilter))
      {
        project = requestContext.GetProjectInfo(projFilter);
        if (project != null)
          ProjectUtility.SetupProjectContext(requestContext, project);
      }
      return project;
    }

    public static void CheckOrganizationInReadOnlyMode(IVssRequestContext requestContext)
    {
      if (requestContext.IsOrganizationInReadOnlyMode())
        throw new InvalidOperationException(ServerTypesResources.ProjectOperationNotAllowedIfOrganizationInReadOnlyMode());
    }
  }
}
