// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.VstsInfoHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Core.CollectionManagement.WebServer;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Location.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class VstsInfoHandler : GitHttpHandler
  {
    private const string c_jsonContentType = "application/json";

    public VstsInfoHandler()
    {
    }

    public VstsInfoHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (VstsInfoHandler);

    internal override void Execute()
    {
      try
      {
        HttpRequestBase request = this.HandlerHttpContext.Request;
        string routeValue1 = request.RequestContext.RouteData.GetRouteValue<string>("project");
        string routeValue2 = request.RequestContext.RouteData.GetRouteValue<string>("GitRepositoryName");
        MethodInformation methodInformation = new MethodInformation(this.Layer, MethodType.LightWeight, EstimatedMethodCost.Low);
        methodInformation.AddParameter("RepositoryName", (object) routeValue2);
        this.EnterMethod(methodInformation);
        using (ITfsGitRepository repositoryByName = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().FindRepositoryByName(this.RequestContext, routeValue1, routeValue2))
        {
          TeamProjectReference projectReference = this.RequestContext.GetService<IProjectService>().GetProject(this.RequestContext, routeValue1).ToTeamProjectReference(this.RequestContext);
          IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
          TeamProjectCollectionReference collectionReference = vssRequestContext.GetService<ITeamProjectCollectionPropertiesService>().GetCollectionProperties(vssRequestContext, this.RequestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.None).ToTeamProjectCollectionReference(this.RequestContext);
          string str1 = vssRequestContext.RequestUri().ToString();
          string str2 = vssRequestContext.RelativeUrl();
          string str3 = str1.Substring(0, str1.Length - str2.Length);
          GitRepository gitRepository = new GitRepository()
          {
            Id = repositoryByName.Key.RepoId,
            Name = routeValue2,
            Url = this.RequestContext.GetService<ILocationService>().GetResourceUri(this.RequestContext, "git", GitWebApiConstants.RepositoriesLocationId, (object) new
            {
              repositoryId = repositoryByName.Key.RepoId
            }).AbsoluteUri,
            ProjectReference = projectReference,
            RemoteUrl = repositoryByName.GetRepositoryFullUri(),
            IsFork = repositoryByName.IsFork,
            Size = new long?(repositoryByName.Size)
          };
          VstsInfo info = new VstsInfo()
          {
            ServerUrl = str3,
            Collection = collectionReference,
            Repository = gitRepository
          };
          this.HandlerHttpContext.Response.ContentType = "application/json";
          this.HandlerHttpContext.Response.Write(this.getJson(info));
        }
      }
      catch (Exception ex)
      {
        if (this.HandleException(ex, false))
          return;
        throw;
      }
    }

    internal string getJson(VstsInfo info)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      };
      return JsonConvert.SerializeObject((object) info, settings);
    }
  }
}
