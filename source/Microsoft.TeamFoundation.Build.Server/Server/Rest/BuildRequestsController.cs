// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.BuildRequestsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.WebApis;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build.Server.Rest
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "Requests")]
  public class BuildRequestsController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static BuildRequestsController()
    {
      BuildRequestsController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (BuildRequestNotFoundException), HttpStatusCode.NotFound);
      BuildRequestsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionDoesNotExistException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionDisabledException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (BuildRequestUpdateException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (BuildRequestPropertyInvalidException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (BuildProcessTemplateNotFoundException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (ArgumentOutOfRangeException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (BuildProcessTemplateDeletionException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (IndexOutOfRangeException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (FormatException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (BuildControllerDoesNotExistException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      BuildRequestsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BuildRequestsController.s_httpExceptions;

    [StreamingCollectionResponse]
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildRequest> GetRequests(
      string projectName = "*",
      string requestedFor = null,
      int? queueId = null,
      string controllerName = "*",
      int? definitionId = null,
      string definitionName = "*",
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      string ids = null,
      int maxCompletedAge = 2147483647,
      Microsoft.TeamFoundation.Build.Server.QueueStatus status = Microsoft.TeamFoundation.Build.Server.QueueStatus.All)
    {
      if (queueId.HasValue)
      {
        string str = LinkingUtilities.EncodeUri(new ArtifactId()
        {
          ToolSpecificId = queueId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          ArtifactType = "Controller",
          Tool = "Build"
        });
        Microsoft.TeamFoundation.Build.Server.BuildController buildController = this.TfsRequestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildControllersByUri(this.TfsRequestContext, (IList<string>) new string[1]
        {
          str
        }, (IList<string>) null, false).Controllers.FirstOrDefault<Microsoft.TeamFoundation.Build.Server.BuildController>();
        if (buildController == null)
          return Enumerable.Empty<Microsoft.TeamFoundation.Build.WebApi.BuildRequest>();
        controllerName = buildController.Name;
      }
      if (definitionId.HasValue)
        definitionName = LinkingUtilities.EncodeUri(new ArtifactId()
        {
          ToolSpecificId = definitionId.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          ArtifactType = "Definition",
          Tool = "Build"
        });
      return this.InternalGetRequests(projectName, requestedFor, controllerName, definitionName, skip, top, ids, maxCompletedAge, status);
    }

    internal IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildRequest> InternalGetRequests(
      string projectName = "*",
      string requestedFor = null,
      string controllerName = "*",
      string definitionName = "*",
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      string ids = null,
      int maxCompletedAge = 2147483647,
      Microsoft.TeamFoundation.Build.Server.QueueStatus status = Microsoft.TeamFoundation.Build.Server.QueueStatus.All)
    {
      BuildRequestsController requestsController1 = this;
      if (!string.IsNullOrEmpty(ids) && (!projectName.Equals("*") && requestsController1.ProjectId == Guid.Empty || !string.IsNullOrEmpty(requestedFor) || !controllerName.Equals("*") || !definitionName.Equals("*") || top != int.MaxValue || skip != 0 || maxCompletedAge != int.MaxValue || status != Microsoft.TeamFoundation.Build.Server.QueueStatus.All))
        throw new ConflictingBuildQueryParametersException(ResourceStrings.ConflictingBuildQueryParameters());
      if (requestsController1.ProjectId != Guid.Empty)
        projectName = requestsController1.TfsRequestContext.GetService<IProjectService>().GetProjectName(requestsController1.TfsRequestContext, requestsController1.ProjectId);
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(controllerName, nameof (controllerName));
      ArgumentUtility.CheckStringForNullOrEmpty(definitionName, nameof (definitionName));
      TeamFoundationBuildService buildService = requestsController1.TfsRequestContext.GetService<TeamFoundationBuildService>();
      TeamFoundationDataReader defReader;
      if (ids != null)
      {
        char[] chArray = new char[1]{ ',' };
        string[] strArray = ids.Split(chArray);
        List<int> ids1 = new List<int>();
        foreach (string s in strArray)
        {
          int result;
          if (!int.TryParse(s, out result))
            throw new ArgumentException(ResourceStrings.IllFormattedId((object) s));
          if (result <= 0)
            throw new ArgumentException(ResourceStrings.IllFormattedId((object) s));
          ids1.Add(result);
        }
        defReader = buildService.QueryQueuedBuildsById(requestsController1.TfsRequestContext, (IList<int>) ids1, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Agents | QueryOptions.Controllers | QueryOptions.BatchedRequests, requestsController1.ProjectId);
        try
        {
          BuildRequestsController requestsController = requestsController1;
          BuildQueueQueryResult queueQueryResult = defReader.Current<BuildQueueQueryResult>();
          IEnumerable<QueuedBuild> source = queueQueryResult.QueuedBuilds.Skip<QueuedBuild>(skip).Take<QueuedBuild>(top);
          Dictionary<string, BuildDetail> uriToBuildDetailMap = queueQueryResult.Builds.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri));
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> uriToControllerMap = queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri));
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> uriToDefinitionMap = queueQueryResult.Definitions.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>) (x => x.Uri));
          Dictionary<string, TeamProjectReference> projectNameToProjectReferenceMap = new Dictionary<string, TeamProjectReference>();
          foreach (Microsoft.TeamFoundation.Build.WebApi.BuildRequest request in source.Select<QueuedBuild, Microsoft.TeamFoundation.Build.WebApi.BuildRequest>((Func<QueuedBuild, Microsoft.TeamFoundation.Build.WebApi.BuildRequest>) (x => requestsController.ConvertBuildRequestToContract(x, uriToBuildDetailMap, uriToControllerMap, requestsController.ProjectId, uriToDefinitionMap, projectNameToProjectReferenceMap))))
            yield return request;
        }
        finally
        {
          defReader?.Dispose();
        }
        defReader = (TeamFoundationDataReader) null;
      }
      BuildQueueSpec buildQueueSpec = new BuildQueueSpec();
      buildQueueSpec.ControllerSpec = new BuildControllerSpec(controllerName, "*", false);
      if (!string.IsNullOrEmpty(definitionName))
      {
        if (definitionName.StartsWith("vstfs:///", StringComparison.OrdinalIgnoreCase))
          buildQueueSpec.DefinitionFilter = (object) new string[1]
          {
            definitionName
          };
        else
          buildQueueSpec.DefinitionFilter = (object) new BuildDefinitionSpec(BuildPath.Root(projectName, definitionName));
      }
      buildQueueSpec.CompletedAge = maxCompletedAge;
      buildQueueSpec.Status = status;
      if (!string.IsNullOrEmpty(requestedFor))
        buildQueueSpec.RequestedFor = requestedFor;
      buildQueueSpec.QueryOptions = QueryOptions.Definitions | QueryOptions.Controllers;
      TeamFoundationDataReader foundationDataReader;
      try
      {
        TeamFoundationBuildService foundationBuildService = buildService;
        IVssRequestContext tfsRequestContext = requestsController1.TfsRequestContext;
        List<BuildQueueSpec> specs = new List<BuildQueueSpec>();
        specs.Add(buildQueueSpec);
        Guid projectId = requestsController1.ProjectId;
        foundationDataReader = foundationBuildService.QueryQueuedBuilds(tfsRequestContext, (IList<BuildQueueSpec>) specs, projectId);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        yield break;
      }
      defReader = foundationDataReader;
      try
      {
        BuildRequestsController requestsController = requestsController1;
        BuildQueueQueryResult queueQueryResult = foundationDataReader.Current<StreamingCollection<BuildQueueQueryResult>>().ElementAt<BuildQueueQueryResult>(0);
        IEnumerable<QueuedBuild> source = queueQueryResult.QueuedBuilds.Skip<QueuedBuild>(skip).Take<QueuedBuild>(top);
        Dictionary<string, BuildDetail> uriToBuildDetailMap = queueQueryResult.Builds.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri));
        Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> uriToControllerMap = queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri));
        Dictionary<string, TeamProjectReference> projectNameToProjectReferenceMap = new Dictionary<string, TeamProjectReference>();
        foreach (Microsoft.TeamFoundation.Build.WebApi.BuildRequest request in source.Select<QueuedBuild, Microsoft.TeamFoundation.Build.WebApi.BuildRequest>((Func<QueuedBuild, Microsoft.TeamFoundation.Build.WebApi.BuildRequest>) (x => requestsController.ConvertBuildRequestToContract(x, uriToBuildDetailMap, uriToControllerMap, requestsController.ProjectId, projectNameToProjectReferenceMap: projectNameToProjectReferenceMap))))
          yield return request;
      }
      finally
      {
        defReader?.Dispose();
      }
      defReader = (TeamFoundationDataReader) null;
    }

    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.BuildRequest GetRequest(int requestId)
    {
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<int> ids = new List<int>();
      ids.Add(requestId);
      Guid projectId = this.ProjectId;
      using (TeamFoundationDataReader foundationDataReader = service.QueryQueuedBuildsById(tfsRequestContext, (IList<int>) ids, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers, projectId))
      {
        BuildQueueQueryResult queueQueryResult = foundationDataReader.Current<BuildQueueQueryResult>();
        QueuedBuild queuedBuild = queueQueryResult.QueuedBuilds.ToList<QueuedBuild>().ElementAt<QueuedBuild>(0);
        if (queuedBuild == null)
          throw new BuildRequestNotFoundException(ResourceStrings.BuildRequestNotFound((object) requestId));
        Dictionary<string, BuildDetail> dictionary1 = queueQueryResult.Builds.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri));
        Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> dictionary2 = queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri));
        Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> dictionary3 = queueQueryResult.Definitions.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>) (x => x.Uri));
        return this.ConvertBuildRequestToContract(queuedBuild, dictionary1, dictionary2, this.ProjectId, dictionary3);
      }
    }

    [HttpPost]
    public Microsoft.TeamFoundation.Build.WebApi.BuildRequest CreateRequest(
      Microsoft.TeamFoundation.Build.WebApi.BuildRequest postContract)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.BuildRequest>(postContract, nameof (postContract));
      ArgumentUtility.CheckForNull<ShallowReference>(postContract.Definition, "definition");
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      string buildDefinitionUri = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Definition", postContract.Definition.Id.ToString()));
      string buildControllerUri = (string) null;
      if (postContract.Queue != null && postContract.Queue.Id != 0)
        buildControllerUri = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Controller", postContract.Queue.Id.ToString()));
      if (!System.Enum.IsDefined(typeof (Microsoft.TeamFoundation.Build.WebApi.QueuePriority), (object) postContract.Priority))
        throw new BuildRequestPropertyInvalidException(ResourceStrings.BuildRequestPropertyInvalid((object) "Priority", (object) postContract.Priority.ToString()));
      if (!System.Enum.IsDefined(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildReason), (object) postContract.Reason))
        throw new BuildRequestPropertyInvalidException(ResourceStrings.BuildRequestPropertyInvalid((object) "Reason", (object) postContract.Reason.ToString()));
      Microsoft.TeamFoundation.Build.Server.BuildRequest buildRequest = new Microsoft.TeamFoundation.Build.Server.BuildRequest(buildControllerUri, buildDefinitionUri, postContract.RequestDropLocation, (Microsoft.TeamFoundation.Build.Server.QueuePriority) postContract.Priority, (string) null, (Microsoft.TeamFoundation.Build.Server.BuildReason) postContract.Reason);
      BuildQueueQueryResult queueQueryResult;
      try
      {
        TeamFoundationBuildService foundationBuildService = service;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<Microsoft.TeamFoundation.Build.Server.BuildRequest> requests = new List<Microsoft.TeamFoundation.Build.Server.BuildRequest>();
        requests.Add(buildRequest);
        Guid projectId = this.ProjectId;
        queueQueryResult = foundationBuildService.QueueBuilds(tfsRequestContext, (IList<Microsoft.TeamFoundation.Build.Server.BuildRequest>) requests, QueueOptions.None, projectId);
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
      catch (Microsoft.TeamFoundation.Build.Server.BuildDefinitionDoesNotExistException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionDoesNotExistException(ResourceStrings.BuildDefinitionDoesNotExist((object) postContract.Definition.Id));
      }
      catch (Microsoft.TeamFoundation.Build.Server.BuildDefinitionDisabledException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.BuildDefinitionDisabledException(ResourceStrings.BuildDefinitionDisabled((object) postContract.Definition.Id));
      }
      catch (ProcessTemplateDeletedException ex)
      {
        throw new BuildProcessTemplateDeletionException(ex.Message);
      }
      catch (ProcessTemplateNotFoundException ex)
      {
        throw new BuildProcessTemplateNotFoundException(ex.Message);
      }
      List<QueuedBuild> list1 = queueQueryResult.QueuedBuilds.ToList<QueuedBuild>();
      List<BuildDetail> list2 = queueQueryResult.Builds.ToList<BuildDetail>();
      return this.ConvertBuildRequestToContract(list1.ElementAt<QueuedBuild>(0), list2.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri)), queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri)), this.ProjectId);
    }

    [HttpPatch]
    public void UpdateRequestStatus(int requestId, Microsoft.TeamFoundation.Build.WebApi.BuildRequest request)
    {
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      request.Id = requestId;
      if (request.Status == Microsoft.TeamFoundation.Build.WebApi.QueueStatus.Postponed || request.Status == Microsoft.TeamFoundation.Build.WebApi.QueueStatus.Queued)
      {
        QueuedBuildUpdateOptions buildUpdateOptions = new QueuedBuildUpdateOptions()
        {
          QueueId = requestId,
          Postponed = request.Status == Microsoft.TeamFoundation.Build.WebApi.QueueStatus.Postponed,
          Fields = QueuedBuildUpdate.Postponed
        };
        service.UpdateQueuedBuilds(this.TfsRequestContext, (IList<QueuedBuildUpdateOptions>) new QueuedBuildUpdateOptions[1]
        {
          buildUpdateOptions
        }, this.ProjectId);
      }
      else
      {
        if (request.Status != Microsoft.TeamFoundation.Build.WebApi.QueueStatus.InProgress)
          return;
        service.StartQueuedBuildsNow(this.TfsRequestContext, new int[1]
        {
          requestId
        }, this.ProjectId);
      }
    }

    [HttpDelete]
    public void DeleteRequest(int requestId)
    {
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      try
      {
        service.CancelBuilds(this.TfsRequestContext, new int[1]
        {
          requestId
        }, this.ProjectId);
      }
      catch (QueuedBuildUpdateException ex)
      {
        throw new BuildRequestUpdateException(ex.Message);
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
      catch (QueuedBuildDoesNotExistException ex)
      {
      }
    }

    private Microsoft.TeamFoundation.Build.WebApi.BuildRequest ConvertBuildRequestToContract(
      QueuedBuild queuedBuild,
      Dictionary<string, BuildDetail> uriToBuildDetailMap,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> uriToControllerMap,
      Guid projectId,
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> uriToDefinitionMap = null,
      Dictionary<string, TeamProjectReference> projectNameToProjectReferenceMap = null)
    {
      if (queuedBuild == null)
        return (Microsoft.TeamFoundation.Build.WebApi.BuildRequest) null;
      Microsoft.TeamFoundation.Build.WebApi.BuildRequest contract = new Microsoft.TeamFoundation.Build.WebApi.BuildRequest();
      contract.Id = queuedBuild.Id;
      ArtifactId artifactId = new ArtifactId("Build", "Request", queuedBuild.Id.ToString());
      contract.Uri = LinkingUtilities.EncodeUri(artifactId);
      contract.Url = DataContractHelpers.GetResourceUrl(this.TfsRequestContext, BuildResourceIds.Requests, projectId, (object) new
      {
        requestId = long.Parse(artifactId.ToolSpecificId)
      });
      Microsoft.TeamFoundation.Build.WebApi.BuildRequest buildRequest = contract;
      QueueReference queueReference = new QueueReference();
      queueReference.Id = int.Parse(LinkingUtilities.DecodeUri(queuedBuild.BuildControllerUri).ToolSpecificId);
      queueReference.Url = DataContractHelpers.GetResourceUrl(this.TfsRequestContext, BuildResourceIds.Queues, Guid.Empty, (object) new
      {
        controllerId = long.Parse(LinkingUtilities.DecodeUri(queuedBuild.BuildControllerUri).ToolSpecificId)
      });
      queueReference.QueueType = QueueType.BuildController;
      queueReference.Name = (string) null;
      buildRequest.Queue = queueReference;
      Microsoft.TeamFoundation.Build.Server.BuildController buildController = (Microsoft.TeamFoundation.Build.Server.BuildController) null;
      if (uriToControllerMap.TryGetValue(queuedBuild.BuildControllerUri, out buildController))
        contract.Queue.Name = buildController.Name;
      DefinitionReference definitionReference1 = new DefinitionReference();
      definitionReference1.Id = int.Parse(LinkingUtilities.DecodeUri(queuedBuild.BuildDefinitionUri).ToolSpecificId);
      definitionReference1.Url = DataContractHelpers.GetResourceUrl(this.TfsRequestContext, BuildResourceIds.Definitions, projectId, (object) new
      {
        definitionId = long.Parse(LinkingUtilities.DecodeUri(queuedBuild.BuildDefinitionUri).ToolSpecificId)
      });
      DefinitionReference definitionReference2 = definitionReference1;
      definitionReference2.Name = (string) null;
      Microsoft.TeamFoundation.Build.Server.BuildDefinition buildDefinition = (Microsoft.TeamFoundation.Build.Server.BuildDefinition) null;
      if (uriToDefinitionMap != null && uriToDefinitionMap.TryGetValue(queuedBuild.BuildDefinitionUri, out buildDefinition))
      {
        definitionReference2.Name = buildDefinition.Name;
        definitionReference2.Id = buildDefinition.Id;
        definitionReference2.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) buildDefinition.QueueStatus;
      }
      else if (queuedBuild.Definition != null)
      {
        definitionReference2.Name = queuedBuild.Definition.Name;
        definitionReference2.QueueStatus = (Microsoft.TeamFoundation.Build.WebApi.DefinitionQueueStatus) queuedBuild.Definition.QueueStatus;
      }
      contract.Definition = (ShallowReference) definitionReference2;
      if (uriToBuildDetailMap.Any<KeyValuePair<string, BuildDetail>>())
      {
        foreach (string buildUri in queuedBuild.BuildUris)
        {
          ShallowReference shallowReference = new ShallowReference();
          shallowReference.Id = int.Parse(LinkingUtilities.DecodeUri(buildUri).ToolSpecificId);
          BuildDetail buildDetail = (BuildDetail) null;
          shallowReference.Name = !uriToBuildDetailMap.TryGetValue(buildUri, out buildDetail) ? (string) null : buildDetail.BuildNumber;
          shallowReference.Url = DataContractHelpers.GetResourceUrl(this.TfsRequestContext, BuildResourceIds.Builds, projectId, (object) new
          {
            buildId = long.Parse(LinkingUtilities.DecodeUri(buildUri).ToolSpecificId)
          });
          contract.Builds.Add(shallowReference);
        }
      }
      else
        contract.Builds = new List<ShallowReference>();
      contract.CustomGetVersion = queuedBuild.CustomGetVersion;
      contract.RequestDropLocation = queuedBuild.DropLocation;
      contract.Priority = (Microsoft.TeamFoundation.Build.WebApi.QueuePriority) queuedBuild.Priority;
      contract.QueuePosition = queuedBuild.QueuePosition;
      contract.QueueTime = queuedBuild.QueueTime;
      contract.Reason = (Microsoft.TeamFoundation.Build.WebApi.BuildReason) queuedBuild.Reason;
      contract.RequestedBy = queuedBuild.RequestedByIdentity.ToIdentityRef(this.TfsRequestContext);
      contract.RequestedFor = queuedBuild.RequestedForIdentity.ToIdentityRef(this.TfsRequestContext);
      contract.ShelvesetName = queuedBuild.ShelvesetName;
      contract.Status = (Microsoft.TeamFoundation.Build.WebApi.QueueStatus) queuedBuild.Status;
      contract.Project = DataContractHelpers.GetValueFromMap<string, TeamProjectReference>(queuedBuild.TeamProject, (IDictionary<string, TeamProjectReference>) projectNameToProjectReferenceMap, (Func<string, TeamProjectReference>) (key =>
      {
        try
        {
          return BuildRequestsController.ToProjectReference(this.TfsRequestContext, this.TfsRequestContext.GetService<CommonStructureService>().GetProjectFromName(this.TfsRequestContext, key));
        }
        catch (Exception ex)
        {
          return (TeamProjectReference) null;
        }
      }));
      return contract;
    }

    private static TeamProjectReference ToProjectReference(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo projectInfo)
    {
      Guid id;
      CommonStructureUtils.TryParseUri(projectInfo.Uri, out id, true);
      return new TeamProjectReference()
      {
        Id = id,
        Name = projectInfo.Name,
        Url = ProjectApiExtensions.GetProjectResourceUrl(requestContext, id),
        State = CommonStructureProjectInfo.ConvertToProjectState(projectInfo.Status)
      };
    }
  }
}
