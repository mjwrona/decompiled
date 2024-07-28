// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.ProjectHelpers
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  internal static class ProjectHelpers
  {
    private const string CacheKey = "Build2.WebApiConverters.GetTeamProjectReference";
    private const string TraceLayer = "ProjectHelpers";

    public static TeamProjectReference GetTeamProjectReference(
      this IVssRequestContext requestContext,
      Guid projectId,
      string projectName = null)
    {
      if (projectId != Guid.Empty && requestContext.UserContext != (IdentityDescriptor) null)
      {
        using (PerformanceTimer.StartMeasure(requestContext, "ProjectHelpers.GetTeamProjectReference"))
        {
          IDictionary<Guid, TeamProjectReference> projectReferenceCache = ProjectHelpers.GetProjectReferenceCache(requestContext);
          TeamProjectReference projectReference;
          if (!projectReferenceCache.TryGetValue(projectId, out projectReference))
          {
            ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
            if (project != null)
            {
              projectReference = ProjectHelpers.ToTeamProjectReference(requestContext, project);
              projectReferenceCache[projectId] = projectReference;
            }
          }
          if (projectReference != null)
            return projectReference;
        }
      }
      return new TeamProjectReference()
      {
        Id = projectId,
        Name = projectName ?? string.Empty
      };
    }

    private static IDictionary<Guid, TeamProjectReference> GetProjectReferenceCache(
      IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope(nameof (ProjectHelpers), nameof (GetProjectReferenceCache)))
      {
        IDictionary<Guid, TeamProjectReference> projectReferenceCache = (IDictionary<Guid, TeamProjectReference>) null;
        if (!requestContext.TryGetItem<IDictionary<Guid, TeamProjectReference>>("Build2.WebApiConverters.GetTeamProjectReference", out projectReferenceCache))
        {
          requestContext.TraceInfo(nameof (ProjectHelpers), "Adding TeamProjectReference dictionary to the request context");
          projectReferenceCache = (IDictionary<Guid, TeamProjectReference>) new Dictionary<Guid, TeamProjectReference>();
          requestContext.Items["Build2.WebApiConverters.GetTeamProjectReference"] = (object) projectReferenceCache;
        }
        return projectReferenceCache;
      }
    }

    private static TeamProjectReference ToTeamProjectReference(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      var routeValues = new{ projectId = projectInfo.Id };
      ILocationService service = requestContext.GetService<ILocationService>();
      Uri resourceUri;
      try
      {
        resourceUri = service.GetResourceUri(requestContext, "core", CoreConstants.ProjectsLocationId, (object) routeValues);
      }
      catch (VssResourceNotFoundException ex)
      {
        resourceUri = service.GetLocationData(requestContext, Guid.Parse("79134C72-4A58-4B42-976C-04E7115F32BF")).GetResourceUri(requestContext, "core", CoreConstants.ProjectsLocationId, (object) routeValues);
      }
      return new TeamProjectReference()
      {
        Id = projectInfo.Id,
        Abbreviation = projectInfo.Abbreviation,
        Name = projectInfo.Name,
        Url = resourceUri?.ToString(),
        State = projectInfo.State,
        Description = projectInfo.Description,
        Revision = projectInfo.Revision,
        Visibility = projectInfo.Visibility,
        LastUpdateTime = projectInfo.LastUpdateTime
      };
    }
  }
}
