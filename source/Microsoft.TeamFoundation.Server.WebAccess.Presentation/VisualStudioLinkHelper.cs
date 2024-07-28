// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Presentation.VisualStudioLinkHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Presentation, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4ED0029A-5609-48A8-995C-ADAB0E762821
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Presentation.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Presentation
{
  public class VisualStudioLinkHelper
  {
    public static string GetOpenInVisualStudioUri(
      TfsWebContext webContext,
      NavigationContextLevels navigationContextLevel = NavigationContextLevels.Application,
      Guid? currentRepositoryGuid = null)
    {
      return VisualStudioLinkHelper.GetOpenInVisualStudioUri(webContext.TfsRequestContext, navigationContextLevel, currentRepositoryGuid);
    }

    public static string GetOpenInVisualStudioUri(
      IVssRequestContext requestContext,
      NavigationContextLevels navigationContextLevel = NavigationContextLevels.Application,
      Guid? currentRepositoryGuid = null)
    {
      try
      {
        ILocationService service1 = requestContext.GetService<ILocationService>();
        IRequestProjectService service2 = requestContext.GetService<IRequestProjectService>();
        string str1;
        if (requestContext.ServiceHost.HostType == TeamFoundationHostType.ProjectCollection)
        {
          str1 = service1.GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
        }
        else
        {
          Guid id = requestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(requestContext.Elevate(), ServiceHostFilterFlags.None).First<TeamProjectCollectionProperties>().Id;
          str1 = service1.LocationForAccessMapping(requestContext, "LocationService2", id, service1.DetermineAccessMapping(requestContext));
        }
        IVssRequestContext requestContext1 = requestContext;
        string str2 = service2.GetProject(requestContext1)?.Id.ToString() ?? "00000000-0000-0000-0000-000000000000";
        string s = "vstfs:///Framework/TeamProject/" + str2 + "?url=" + str1 + "&project=" + str2;
        if (currentRepositoryGuid.HasValue)
          s = s + "&repo=" + currentRepositoryGuid.ToString();
        return "vsweb://vs/?Product=Visual_Studio&EncFormat=UTF8&tfslink=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }
  }
}
