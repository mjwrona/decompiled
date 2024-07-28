// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlDefinitionProvider
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public class XamlDefinitionProvider : IXamlDefinitionProvider, IVssFrameworkService
  {
    public XamlBuildDefinition GetDefinition(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int definitionId)
    {
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      ArtifactId artifactId = new ArtifactId("Build", "Definition", definitionId.ToString());
      IVssRequestContext requestContext1 = requestContext;
      List<string> uris = new List<string>();
      uris.Add(LinkingUtilities.EncodeUri(artifactId));
      Guid id = projectInfo.Id;
      BuildDefinitionQueryResult definitionQueryResult = service.QueryBuildDefinitionsByUri(requestContext1, (IList<string>) uris, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers, id);
      List<Microsoft.TeamFoundation.Build.Server.BuildDefinition> list = definitionQueryResult.Definitions.ToList<Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
      if (list.Count > 0 && list[0] == null)
        return (XamlBuildDefinition) null;
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in definitionQueryResult.Controllers)
        urisToControllers.Add(controller.Uri, controller);
      return this.ConvertDefinitionToDataContract(requestContext, list.ElementAt<Microsoft.TeamFoundation.Build.Server.BuildDefinition>(0), (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) urisToControllers, projectInfo);
    }

    public IEnumerable<XamlBuildDefinition> GetDefinitions(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      string name)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(projectInfo, nameof (projectInfo));
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      BuildDefinitionSpec spec = new BuildDefinitionSpec()
      {
        FullPath = BuildPath.Root(projectInfo.Name, name),
        Options = QueryOptions.Definitions | QueryOptions.Controllers
      };
      BuildDefinitionQueryResult definitionQueryResult;
      try
      {
        definitionQueryResult = service.QueryBuildDefinitions(requestContext, spec, projectId: projectInfo.Id);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        yield break;
      }
      List<Microsoft.TeamFoundation.Build.Server.BuildDefinition> list = definitionQueryResult.Definitions.ToList<Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in definitionQueryResult.Controllers)
        urisToControllers.Add(controller.Uri, controller);
      Dictionary<string, string> repositoryMap = new Dictionary<string, string>();
      foreach (XamlBuildDefinition definition in list.Select<Microsoft.TeamFoundation.Build.Server.BuildDefinition, XamlBuildDefinition>((Func<Microsoft.TeamFoundation.Build.Server.BuildDefinition, XamlBuildDefinition>) (x => this.ConvertDefinitionToDataContract(requestContext, x, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) urisToControllers, projectInfo, repositoryMap))))
        yield return definition;
    }

    public void DeleteDefinition(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      XamlBuildDefinition definition)
    {
      TeamFoundationBuildService service = requestContext.GetService<TeamFoundationBuildService>();
      try
      {
        requestContext.GetService<IXamlBuildProvider>().DeleteBuildsForDefinition(requestContext, projectInfo, definition);
        service.DeleteBuildDefinitions(requestContext, (IList<string>) new List<string>()
        {
          definition.Uri.ToString()
        });
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
    }

    private XamlBuildDefinition ConvertDefinitionToDataContract(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.Server.BuildDefinition definition,
      IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      ProjectInfo projectInfo)
    {
      return this.ConvertDefinitionToDataContract(requestContext, definition, urisToControllers, projectInfo, new Dictionary<string, string>());
    }

    private XamlBuildDefinition ConvertDefinitionToDataContract(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.Server.BuildDefinition definition,
      IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      ProjectInfo projectInfo,
      Dictionary<string, string> repositoryMap)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(definition.Uri);
      Microsoft.TeamFoundation.Build.WebApi.BuildController buildController = (Microsoft.TeamFoundation.Build.WebApi.BuildController) null;
      Microsoft.TeamFoundation.Build.Server.BuildController controller;
      if (!string.IsNullOrEmpty(definition.BuildControllerUri) && urisToControllers.TryGetValue(definition.BuildControllerUri, out controller))
        buildController = controller.ToDataContract(requestContext);
      int definitionId = int.Parse(artifactId.ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture);
      IBuildRouteService service1 = requestContext.GetService<IBuildRouteService>();
      XamlBuildDefinition xamlBuildDefinition = new XamlBuildDefinition();
      xamlBuildDefinition.Id = definitionId;
      xamlBuildDefinition.Name = definition.Name;
      xamlBuildDefinition.Url = service1.GetDefinitionRestUrl(requestContext, projectInfo.Id, definitionId);
      xamlBuildDefinition.Uri = new Uri(definition.Uri);
      xamlBuildDefinition.Controller = buildController;
      xamlBuildDefinition.Project = projectInfo.ToTeamProjectReference(requestContext);
      xamlBuildDefinition.Description = definition.Description;
      xamlBuildDefinition.CreatedOn = definition.DateCreated;
      xamlBuildDefinition.BatchSize = definition.BatchSize;
      xamlBuildDefinition.BuildArgs = MSBuildArgsHelper.GetMSBuildArguments(definition.ProcessParameters);
      xamlBuildDefinition.ContinuousIntegrationQuietPeriod = definition.ContinuousIntegrationQuietPeriod;
      xamlBuildDefinition.DefaultDropLocation = definition.DefaultDropLocation;
      xamlBuildDefinition.QueueStatus = definition.QueueStatus.ToQueueStatus();
      xamlBuildDefinition.TriggerType = definition.TriggerType.ToTriggerType();
      XamlBuildDefinition result = xamlBuildDefinition;
      result.Links.TryAddLink("self", (ISecuredObject) result, result.Url);
      result.Links.TryAddLink("web", (ISecuredObject) result, (Func<string>) (() => result.GetWebUrl(requestContext)));
      if (!definition.LastBuildUri.IsNullOrEmpty<char>())
      {
        int buildId = int.Parse(LinkingUtilities.DecodeUri(definition.LastBuildUri).ToolSpecificId);
        result.LastBuildRef = XamlBuildExtensions.GetReference(requestContext, projectInfo.Id, buildId);
      }
      result.SupportedReasons = definition.Process == null ? Microsoft.TeamFoundation.Build.WebApi.BuildReason.None : definition.Process.SupportedReasons.ToBuildReason();
      Microsoft.TeamFoundation.Build.Server.BuildDefinitionSourceProvider sourceProvider = definition.SourceProviders.FirstOrDefault<Microsoft.TeamFoundation.Build.Server.BuildDefinitionSourceProvider>();
      if (sourceProvider != null)
      {
        IXamlSourceProvider service2 = requestContext.GetService<IXamlSourceProvider>();
        result.Repository = service2.ConvertXamlSourceProviderToBuildRepository(requestContext, definition, sourceProvider, (IDictionary<string, string>) repositoryMap, true);
      }
      return result;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
