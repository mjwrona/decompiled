// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.ProjectCreationUrlHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  public static class ProjectCreationUrlHelper
  {
    public static string GetCreateNewProjectUrl(
      TfsWebContext tfsWebContext,
      IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Application))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        ITeamFoundationHostManagementService service = vssRequestContext.GetService<ITeamFoundationHostManagementService>();
        TeamProjectCollectionProperties collectionProperties = requestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(requestContext.Elevate(), (IList<Guid>) null, ServiceHostFilterFlags.None).FirstOrDefault<TeamProjectCollectionProperties>();
        if (collectionProperties != null)
        {
          using (IVssRequestContext requestContext1 = service.BeginRequest(vssRequestContext, collectionProperties.Id, RequestContextType.UserContext, throwIfShutdown: false))
          {
            if (requestContext1 != null)
              return ProjectCreationUrlHelper.ConstructCreateProjectUrl(tfsWebContext, requestContext1);
          }
        }
      }
      else if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return ProjectCreationUrlHelper.ConstructCreateProjectUrl(tfsWebContext, requestContext);
      return string.Empty;
    }

    private static string ConstructCreateProjectUrl(
      TfsWebContext tfsWebContext,
      IVssRequestContext requestContext)
    {
      string collectionUrl = ProjectCreationUrlHelper.GetCollectionUrl(requestContext);
      string contributionId = "ms.vss-tfs-web.suite-me-page-route";
      string str = "newProject";
      string path2 = ProjectCreationUrlHelper.GetContributionRoute(requestContext, contributionId) + string.Format("?_a={0}", (object) str);
      return path2 != null ? Path.Combine(collectionUrl, path2) : (string) null;
    }

    private static string GetContributionRoute(
      IVssRequestContext requestContext,
      string contributionId)
    {
      Contribution contribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, contributionId);
      string[] strArray;
      return contribution != null && contribution.Properties.TryGetValue<string[]>("routeTemplates", out strArray) && strArray != null ? strArray[0] : string.Empty;
    }

    private static string GetCollectionUrl(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
  }
}
