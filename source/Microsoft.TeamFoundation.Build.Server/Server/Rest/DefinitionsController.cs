// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.DefinitionsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "Definitions")]
  public class DefinitionsController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static DefinitionsController()
    {
      DefinitionsController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      DefinitionsController.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      DefinitionsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.CannotDeleteDefinitionBuildExistsException), HttpStatusCode.BadRequest);
      DefinitionsController.s_httpExceptions.Add(typeof (IndexOutOfRangeException), HttpStatusCode.BadRequest);
      DefinitionsController.s_httpExceptions.Add(typeof (DefinitionNotFoundException), HttpStatusCode.NotFound);
      DefinitionsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      DefinitionsController.s_httpExceptions.Add(typeof (ArgumentOutOfRangeException), HttpStatusCode.BadRequest);
      DefinitionsController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      DefinitionsController.s_httpExceptions.Add(typeof (FormatException), HttpStatusCode.BadRequest);
      DefinitionsController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) DefinitionsController.s_httpExceptions;

    [StreamingCollectionResponse]
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildDefinition> GetDefinitions(
      string projectName = "*")
    {
      return this.InternalGetDefinitions(projectName);
    }

    internal IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildDefinition> InternalGetDefinitions(
      string projectName = "*")
    {
      DefinitionsController definitionsController = this;
      if (definitionsController.ProjectId != Guid.Empty)
        projectName = definitionsController.TfsRequestContext.GetService<IProjectService>().GetProjectName(definitionsController.TfsRequestContext, definitionsController.ProjectId);
      if (string.IsNullOrEmpty(projectName))
        ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      TeamFoundationBuildService service = definitionsController.TfsRequestContext.GetService<TeamFoundationBuildService>();
      BuildDefinitionSpec spec = new BuildDefinitionSpec(BuildPath.Root(projectName, "*"));
      spec.Options = QueryOptions.Definitions | QueryOptions.Controllers;
      BuildDefinitionQueryResult definitionQueryResult;
      try
      {
        definitionQueryResult = service.QueryBuildDefinitions(definitionsController.TfsRequestContext, spec, projectId: definitionsController.ProjectId);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        yield break;
      }
      List<Microsoft.TeamFoundation.Build.Server.BuildDefinition> list = definitionQueryResult.Definitions.ToList<Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
      List<Microsoft.TeamFoundation.Build.WebApi.BuildDefinition> buildDefinitionList = new List<Microsoft.TeamFoundation.Build.WebApi.BuildDefinition>();
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in definitionQueryResult.Controllers)
        urisToControllers.Add(controller.Uri, controller);
      foreach (Microsoft.TeamFoundation.Build.WebApi.BuildDefinition definition in list.Select<Microsoft.TeamFoundation.Build.Server.BuildDefinition, Microsoft.TeamFoundation.Build.WebApi.BuildDefinition>((Func<Microsoft.TeamFoundation.Build.Server.BuildDefinition, Microsoft.TeamFoundation.Build.WebApi.BuildDefinition>) (x => this.ConvertDefinitionToDataContract(x, urisToControllers, false, this.ProjectId))))
        yield return definition;
    }

    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.BuildDefinition GetDefinition(int definitionId)
    {
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      ArtifactId artifactId = new ArtifactId("Build", "Definition", definitionId.ToString());
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<string> uris = new List<string>();
      uris.Add(LinkingUtilities.EncodeUri(artifactId));
      Guid projectId = this.ProjectId;
      BuildDefinitionQueryResult definitionQueryResult = service.QueryBuildDefinitionsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers, projectId);
      List<Microsoft.TeamFoundation.Build.Server.BuildDefinition> definitions = definitionQueryResult.Definitions;
      if (definitions[0] == null)
        throw new DefinitionNotFoundException(ResourceStrings.DefinitionNotFound((object) definitionId));
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in definitionQueryResult.Controllers)
        urisToControllers.Add(controller.Uri, controller);
      return this.ConvertDefinitionToDataContract(definitions[0], urisToControllers, true, this.ProjectId);
    }

    private Microsoft.TeamFoundation.Build.WebApi.BuildDefinition ConvertDefinitionToDataContract(
      Microsoft.TeamFoundation.Build.Server.BuildDefinition definition,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      bool isDeep,
      Guid projectId)
    {
      Microsoft.TeamFoundation.Build.WebApi.BuildDefinition dataContract = new Microsoft.TeamFoundation.Build.WebApi.BuildDefinition();
      dataContract.Id = definition.Id;
      dataContract.BatchSize = definition.BatchSize;
      dataContract.ContinuousIntegrationQuietPeriod = definition.ContinuousIntegrationQuietPeriod;
      dataContract.DateCreated = definition.DateCreated;
      dataContract.DefaultDropLocation = definition.DefaultDropLocation;
      dataContract.Description = definition.Description;
      dataContract.BuildArgs = MSBuildArgsHelper.GetMSBuildArguments(definition.ProcessParameters);
      dataContract.Name = definition.Name;
      dataContract.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) definition.QueueStatus;
      dataContract.TriggerType = (Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType) definition.TriggerType;
      dataContract.Uri = definition.Uri;
      dataContract.Url = DataContractHelpers.GetResourceUrl(this.TfsRequestContext, BuildResourceIds.Definitions, projectId, (object) new
      {
        definitionId = int.Parse(LinkingUtilities.DecodeUri(definition.Uri).ToolSpecificId)
      });
      if (!definition.LastBuildUri.IsNullOrEmpty<char>())
      {
        int num = int.Parse(LinkingUtilities.DecodeUri(definition.LastBuildUri).ToolSpecificId);
        ShallowReference shallowReference = new ShallowReference()
        {
          Id = num,
          Url = DataContractHelpers.GetResourceUrl(this.TfsRequestContext, BuildResourceIds.Builds, projectId, (object) new
          {
            buildId = num
          })
        };
        dataContract.LastBuild = shallowReference;
      }
      QueueReference queueReference = new QueueReference();
      Microsoft.TeamFoundation.Build.Server.BuildController buildController = new Microsoft.TeamFoundation.Build.Server.BuildController();
      if (urisToControllers.TryGetValue(definition.BuildControllerUri, out buildController))
      {
        queueReference.Id = int.Parse(LinkingUtilities.DecodeUri(buildController.Uri).ToolSpecificId);
        queueReference.Name = buildController.Name;
        queueReference.Url = DataContractHelpers.GetResourceUrl(this.TfsRequestContext, BuildResourceIds.Queues, Guid.Empty, (object) new
        {
          controllerId = LinkingUtilities.DecodeUri(buildController.Uri).ToolSpecificId
        });
        queueReference.QueueType = QueueType.BuildController;
        dataContract.Queue = queueReference;
      }
      else
        dataContract.Queue = (QueueReference) null;
      dataContract.SupportedReasons = definition.Process == null ? Microsoft.TeamFoundation.Build.WebApi.BuildReason.None : (Microsoft.TeamFoundation.Build.WebApi.BuildReason) definition.Process.SupportedReasons;
      return dataContract;
    }
  }
}
