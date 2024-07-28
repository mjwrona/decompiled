// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Rest.BuildsController
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
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
  [VersionedApiControllerCustomName(Area = "Build", ResourceName = "Builds")]
  public class BuildsController : BuildApiController
  {
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static BuildsController()
    {
      BuildsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.Server.AccessDeniedException), HttpStatusCode.Forbidden);
      BuildsController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      BuildsController.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (ArgumentOutOfRangeException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (BuildNotFoundException), HttpStatusCode.NotFound);
      BuildsController.s_httpExceptions.Add(typeof (BuildServiceHostOwnershipException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (ConflictingBuildQueryParametersException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (FormatException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (InvalidBuildUriException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (IndexOutOfRangeException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      BuildsController.s_httpExceptions.Add(typeof (UriFormatException), HttpStatusCode.BadRequest);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) BuildsController.s_httpExceptions;

    [HttpGet]
    public Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(int buildId) => this.InternalGetBuild(buildId);

    [StreamingCollectionResponse]
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuilds(
      string projectName = "*",
      string requestedFor = null,
      string definition = null,
      int maxBuildsPerDefinition = 2147483647,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$top")] int top = 2147483647,
      string ids = null,
      DateTime? minFinishTime = null,
      string quality = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus status = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All)
    {
      return this.InternalGetBuilds(projectName, requestedFor, definition, maxBuildsPerDefinition, skip, top, ids, minFinishTime, quality, status);
    }

    private Microsoft.TeamFoundation.Build.WebApi.Build InternalGetBuild(int buildId)
    {
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      ArtifactId artifactId = new ArtifactId("Build", "Build", buildId.ToString());
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<string> uris = new List<string>();
      uris.Add(LinkingUtilities.EncodeUri(artifactId));
      Guid projectId = this.ProjectId;
      using (TeamFoundationDataReader foundationDataReader = service.QueryBuildsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Agents | QueryOptions.Controllers | QueryOptions.BatchedRequests, QueryDeletedOption.IncludeDeleted, projectId, false))
      {
        BuildQueryResult firstResult = foundationDataReader.Current<BuildQueryResult>();
        BuildDetail buildDetail = firstResult.Builds.ToList<BuildDetail>().ElementAt<BuildDetail>(0);
        if (buildDetail == null)
          throw new BuildNotFoundException(ResourceStrings.BuildNotFound((object) buildId.ToString()));
        Microsoft.TeamFoundation.Build.WebApi.Build build = new Microsoft.TeamFoundation.Build.WebApi.Build();
        Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
        Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
        Dictionary<string, List<QueuedBuild>> urisToRequests = new Dictionary<string, List<QueuedBuild>>();
        Dictionary<string, Guid> idCache = new Dictionary<string, Guid>();
        DataContractHelpers.WriteDictionaries(firstResult, urisToControllers, urisToDefinitions, urisToRequests);
        return DataContractHelpers.ConvertDetailToDataContract(this.TfsRequestContext, buildDetail, urisToControllers, urisToDefinitions, urisToRequests, idCache, this.ProjectId);
      }
    }

    private IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> InternalGetBuilds(
      string projectName = "*",
      string requestedFor = null,
      string definition = null,
      int maxBuildsPerDefinition = 2147483647,
      int skip = 0,
      int top = 2147483647,
      string ids = null,
      DateTime? minFinishTime = null,
      string quality = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus status = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All)
    {
      BuildsController buildsController1 = this;
      if (!string.IsNullOrEmpty(ids) && (!projectName.Equals("*") && buildsController1.ProjectId == Guid.Empty || !string.IsNullOrEmpty(requestedFor) || !string.IsNullOrEmpty(definition) || top != int.MaxValue || skip != 0 || minFinishTime.HasValue || !string.IsNullOrEmpty(quality) || status != Microsoft.TeamFoundation.Build.WebApi.BuildStatus.All))
        throw new ConflictingBuildQueryParametersException(ResourceStrings.ConflictingBuildQueryParameters());
      if (buildsController1.ProjectId != Guid.Empty)
        projectName = buildsController1.TfsRequestContext.GetService<IProjectService>().GetProjectName(buildsController1.TfsRequestContext, buildsController1.ProjectId);
      if (string.IsNullOrEmpty(projectName))
        ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      TeamFoundationBuildService service = buildsController1.TfsRequestContext.GetService<TeamFoundationBuildService>();
      TeamFoundationDataReader defReader;
      if (ids != null)
      {
        char[] chArray = new char[1]{ ',' };
        string[] strArray = ids.Split(chArray);
        List<string> uris = new List<string>();
        foreach (string str in strArray)
        {
          int result;
          if (!int.TryParse(str, out result))
            throw new ArgumentException(ResourceStrings.IllFormattedId((object) str));
          if (result <= 0)
            throw new ArgumentException(ResourceStrings.IllFormattedId((object) str));
          ArtifactId artifactId = new ArtifactId("Build", "Build", str);
          uris.Add(LinkingUtilities.EncodeUri(artifactId));
        }
        defReader = service.QueryBuildsByUri(buildsController1.TfsRequestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Agents | QueryOptions.Controllers | QueryOptions.BatchedRequests, QueryDeletedOption.IncludeDeleted, buildsController1.ProjectId, false);
        try
        {
          BuildsController buildsController = buildsController1;
          BuildQueryResult firstResult = defReader.Current<BuildQueryResult>();
          IEnumerable<BuildDetail> source = firstResult.Builds.Skip<BuildDetail>(skip).Take<BuildDetail>(top);
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
          Dictionary<string, List<QueuedBuild>> urisToRequests = new Dictionary<string, List<QueuedBuild>>();
          Dictionary<string, Guid> idCache = new Dictionary<string, Guid>();
          DataContractHelpers.WriteDictionaries(firstResult, urisToControllers, urisToDefinitions, urisToRequests);
          foreach (Microsoft.TeamFoundation.Build.WebApi.Build build in source.Where<BuildDetail>((Func<BuildDetail, bool>) (x => x != null)).Select<BuildDetail, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<BuildDetail, Microsoft.TeamFoundation.Build.WebApi.Build>) (x => DataContractHelpers.ConvertDetailToDataContract(buildsController.TfsRequestContext, x, urisToControllers, urisToDefinitions, urisToRequests, idCache, buildsController.ProjectId))))
            yield return build;
        }
        finally
        {
          defReader?.Dispose();
        }
      }
      else
      {
        BuildDetailSpec buildDetailSpec = new BuildDetailSpec(BuildPath.Root(projectName, "*"), "*");
        buildDetailSpec.QueryOrder = BuildQueryOrder.FinishTimeDescending;
        buildDetailSpec.Status = (Microsoft.TeamFoundation.Build.Server.BuildStatus) status;
        if (!string.IsNullOrEmpty(requestedFor))
          buildDetailSpec.RequestedFor = requestedFor;
        if (!string.IsNullOrEmpty(definition))
        {
          if (definition.StartsWith("vstfs:///", StringComparison.OrdinalIgnoreCase))
            buildDetailSpec.DefinitionFilter = (object) new string[1]
            {
              definition
            };
          else
            buildDetailSpec.DefinitionFilter = (object) new BuildDefinitionSpec(BuildPath.Root(projectName, definition));
        }
        if (minFinishTime.HasValue)
          buildDetailSpec.MinFinishTime = minFinishTime.Value.EnsureUtc();
        if (maxBuildsPerDefinition != int.MaxValue)
          buildDetailSpec.MaxBuildsPerDefinition = maxBuildsPerDefinition;
        buildDetailSpec.Quality = quality;
        buildDetailSpec.QueryOptions = QueryOptions.Definitions | QueryOptions.Agents | QueryOptions.Controllers | QueryOptions.BatchedRequests;
        TeamFoundationDataReader foundationDataReader;
        try
        {
          TeamFoundationBuildService foundationBuildService = service;
          IVssRequestContext tfsRequestContext = buildsController1.TfsRequestContext;
          List<BuildDetailSpec> specs = new List<BuildDetailSpec>();
          specs.Add(buildDetailSpec);
          Guid projectId = buildsController1.ProjectId;
          foundationDataReader = foundationBuildService.QueryBuilds(tfsRequestContext, (IList<BuildDetailSpec>) specs, projectId);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          yield break;
        }
        defReader = foundationDataReader;
        try
        {
          BuildsController buildsController = buildsController1;
          BuildQueryResult firstResult = foundationDataReader.Current<StreamingCollection<BuildQueryResult>>().ElementAt<BuildQueryResult>(0);
          IEnumerable<BuildDetail> source = firstResult.Builds.Skip<BuildDetail>(skip).Take<BuildDetail>(top);
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
          Dictionary<string, List<QueuedBuild>> urisToRequests = new Dictionary<string, List<QueuedBuild>>();
          Dictionary<string, Guid> idCache = new Dictionary<string, Guid>();
          DataContractHelpers.WriteDictionaries(firstResult, urisToControllers, urisToDefinitions, urisToRequests);
          foreach (Microsoft.TeamFoundation.Build.WebApi.Build build in source.Select<BuildDetail, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<BuildDetail, Microsoft.TeamFoundation.Build.WebApi.Build>) (x => DataContractHelpers.ConvertDetailToDataContract(buildsController.TfsRequestContext, x, urisToControllers, urisToDefinitions, urisToRequests, idCache, buildsController.ProjectId))))
            yield return build;
        }
        finally
        {
          defReader?.Dispose();
        }
        defReader = (TeamFoundationDataReader) null;
      }
    }

    [HttpPatch]
    public Microsoft.TeamFoundation.Build.WebApi.Build UpdateBuild(int buildId, Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      build.Id = buildId;
      Lazy<ArtifactId> lazy = new Lazy<ArtifactId>((Func<ArtifactId>) (() => new ArtifactId()
      {
        ToolSpecificId = buildId.ToString(),
        ArtifactType = "Build",
        Tool = "Build"
      }));
      if (build.Status == Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Stopped)
        service.StopBuilds(this.TfsRequestContext, (IList<string>) new string[1]
        {
          LinkingUtilities.EncodeUri(lazy.Value)
        }, this.ProjectId);
      else if (build.Quality != null || build.RetainIndefinitely.HasValue)
      {
        BuildUpdateOptions buildUpdateOptions = new BuildUpdateOptions()
        {
          Uri = LinkingUtilities.EncodeUri(lazy.Value)
        };
        if (build.Quality != null)
        {
          buildUpdateOptions.Quality = build.Quality;
          buildUpdateOptions.Fields |= BuildUpdate.Quality;
        }
        if (build.RetainIndefinitely.HasValue)
        {
          buildUpdateOptions.KeepForever = build.RetainIndefinitely.Value;
          buildUpdateOptions.Fields |= BuildUpdate.KeepForever;
        }
        service.UpdateBuilds(this.TfsRequestContext, (IList<BuildUpdateOptions>) new BuildUpdateOptions[1]
        {
          buildUpdateOptions
        }, this.ProjectId);
      }
      return this.InternalGetBuild(buildId);
    }

    [HttpDelete]
    public void DeleteBuild(int buildId)
    {
      ArtifactId artifactId = new ArtifactId("Build", "Build", buildId.ToString());
      TeamFoundationBuildService service = this.TfsRequestContext.GetService<TeamFoundationBuildService>();
      try
      {
        TeamFoundationBuildService foundationBuildService = service;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<string> uris = new List<string>();
        uris.Add(LinkingUtilities.EncodeUri(artifactId));
        Guid projectId = this.ProjectId;
        foundationBuildService.DeleteBuilds(tfsRequestContext, (IList<string>) uris, Microsoft.TeamFoundation.Build.Server.DeleteOptions.All, false, projectId, false);
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
    }
  }
}
