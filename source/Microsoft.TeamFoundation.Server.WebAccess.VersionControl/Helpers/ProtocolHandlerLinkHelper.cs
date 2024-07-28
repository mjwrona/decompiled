// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Helpers.ProtocolHandlerLinkHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl.Helpers
{
  public class ProtocolHandlerLinkHelper
  {
    private static readonly string[] c_allPlatforms = new string[3]
    {
      "Win",
      "Mac",
      "Linux"
    };
    private static readonly string[] c_onlyMacAndWin = new string[2]
    {
      "Mac",
      "Win"
    };
    private static readonly string[] c_onlyMac = new string[1]
    {
      "Mac"
    };
    private static readonly string[] c_onlyWin = new string[1]
    {
      "Win"
    };

    private static string GetTfsEncodedLink(
      IVssRequestContext requestContext,
      Guid currentProjectId,
      Guid? currentRepositoryGuid = null)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      string str1;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        str1 = service.GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
      }
      else
      {
        Guid id = requestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(requestContext.Elevate(), ServiceHostFilterFlags.None).First<TeamProjectCollectionProperties>().Id;
        str1 = service.LocationForAccessMapping(requestContext, "LocationService2", id, service.DetermineAccessMapping(requestContext));
      }
      string str2 = currentProjectId.ToString();
      string s = "vstfs:///Framework/TeamProject/" + str2 + "?url=" + str1 + "&project=" + str2;
      if (currentRepositoryGuid.HasValue)
        s = s + "&repo=" + currentRepositoryGuid.ToString();
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
    }

    private static string GetTfsGitRepositoryUrl(
      IVssRequestContext requestContext,
      Guid currentRepositoryGuid,
      out string name)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      using (ITfsGitRepository repositoryById = requestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryById(requestContext, currentRepositoryGuid))
      {
        name = repositoryById.Name;
        return repositoryById.GetRepositoryFullUri();
      }
    }

    private static string GetTfsEncodedLinkForEclipse(
      IVssRequestContext requestContext,
      Guid currentProjectId,
      string repoUrl,
      string repoName)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.ClientAccessMappingMoniker);
      string name = requestContext.GetService<IProjectService>().GetProject(requestContext, currentProjectId).Name;
      return Convert.ToBase64String(Encoding.UTF8.GetBytes("collectionUrl=" + locationServiceUrl + "&cloneUrl=" + repoUrl + "&project=" + name + "&repository=" + repoName));
    }

    public static SupportedIde[] GetSupportedIdes(
      IVssRequestContext requestContext,
      Guid currentProjectId,
      Guid? currentRepositoryGuid = null)
    {
      List<SupportedIde> supportedIdeList = new List<SupportedIde>();
      try
      {
        SupportedIde supportedIde = new SupportedIde(SupportedIdeType.VisualStudio, VCServerResources.VSProductName, "vsweb://vs/?Product=Visual_Studio&EncFormat=UTF8&tfslink=" + ProtocolHandlerLinkHelper.GetTfsEncodedLink(requestContext, currentProjectId, currentRepositoryGuid), ProtocolHandlerLinkHelper.c_onlyWin, VCServerResources.VSDownloadLink);
        if (currentRepositoryGuid.HasValue)
        {
          string name;
          string gitRepositoryUrl = ProtocolHandlerLinkHelper.GetTfsGitRepositoryUrl(requestContext, currentRepositoryGuid.Value, out name);
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.AndroidStudio, VCResources.AndroidStudioName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=Android%20Studio&IdeExe=studio", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.AndroidStudioDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.AppCode, VCServerResources.AppCode, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=AppCode&IdeExe=appcode", ProtocolHandlerLinkHelper.c_onlyMac, VCServerResources.AppCodeDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.CLion, VCServerResources.ClionName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=CLion&IdeExe=clion", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.CLionDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.DataGrip, VCServerResources.DataGripName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=DataGrip&IdeExe=datagrip", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.DataGripDownloadLink));
          string encodedLinkForEclipse = ProtocolHandlerLinkHelper.GetTfsEncodedLinkForEclipse(requestContext, currentProjectId, gitRepositoryUrl, name);
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.Eclipse, VCServerResources.EclipseName, "vsoeclipse://checkout/?EncFormat=UTF8&tfslink=" + encodedLinkForEclipse, ProtocolHandlerLinkHelper.c_onlyWin, VCServerResources.EclipseName));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.IntelliJ, VCResources.IntelliJName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=IntelliJ&IdeExe=idea", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.IntelliJDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.MPS, VCServerResources.MPSName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=MPS&IdeExe=mps", ProtocolHandlerLinkHelper.c_onlyMac, VCServerResources.MPSDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.PhpStorm, VCServerResources.PhpStormName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=PhpStorm&IdeExe=PhpStorm", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.PhpStormDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.PyCharm, VCServerResources.PyCharmName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=PyCharm&IdeExe=pycharm", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.PyCharmDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.RubyMine, VCServerResources.RubyMineName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=RubyMine&IdeExe=rubymine", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.RubyMineDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.Tower, VCServerResources.TowerName, "gittower://openRepo/" + gitRepositoryUrl, ProtocolHandlerLinkHelper.c_onlyMacAndWin, VCServerResources.TowerDownloadLink));
          supportedIdeList.Add(supportedIde);
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.VSCode, VCServerResources.VSCodeName, "vscode://vscode.git/clone?url=" + gitRepositoryUrl, ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.VSCodeDownloadLink));
          supportedIdeList.Add(new SupportedIde(SupportedIdeType.WebStorm, VCServerResources.WebStormName, "vsoi://checkout/?url=" + gitRepositoryUrl + "&EncFormat=UTF8&IdeType=WebStorm&IdeExe=WebStorm", ProtocolHandlerLinkHelper.c_allPlatforms, VCServerResources.WebStormDownloadLink));
        }
        else
          supportedIdeList.Add(supportedIde);
        return supportedIdeList.ToArray();
      }
      catch (Exception ex)
      {
        return supportedIdeList.ToArray();
      }
    }
  }
}
