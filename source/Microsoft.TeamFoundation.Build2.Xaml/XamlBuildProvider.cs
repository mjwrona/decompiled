// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Xaml.XamlBuildProvider
// Assembly: Microsoft.TeamFoundation.Build2.Xaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 48A241AC-D20F-49E0-A581-C219E1ED7760
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Xaml.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.Build2.Xaml
{
  public class XamlBuildProvider : IXamlBuildProvider, IVssFrameworkService
  {
    private const string ActivityLogRelativePath = "/ActivityLog.xml";
    private const string AgentScopeLogRelativePathFormat = "/ActivityLog.AgentScope.{0}.xml";

    public Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      bool includeDeleted = false)
    {
      return this.GetBuild(requestContext, projectInfo, buildId, true, includeDeleted);
    }

    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> GetBuilds(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int count,
      IList<int> definitionIds = null,
      IList<int> queueIds = null,
      string buildNumber = "*",
      DateTime? minFinishTime = null,
      DateTime? maxFinishTime = null,
      string requestedFor = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildReason reasonFilter = Microsoft.TeamFoundation.Build.WebApi.BuildReason.All,
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildResult? resultFilter = null,
      Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending,
      Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption queryDeletedOption = Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.ExcludeDeleted,
      int? maxBuildsPerDefinition = null)
    {
      Microsoft.TeamFoundation.Build.Server.BuildStatus xamlBuildStatus = XamlBuildExtensions.GetXamlBuildStatus(statusFilter, resultFilter);
      QueueStatus xamlQueueStatus = XamlBuildExtensions.GetXamlQueueStatus(statusFilter, resultFilter);
      Microsoft.TeamFoundation.Build.Server.BuildReason xamlBuildReason = reasonFilter.ToXamlBuildReason();
      Microsoft.TeamFoundation.Build.Server.QueryDeletedOption queryDeletedOption1 = XamlBuildExtensions.GetXamlQueryDeletedOption(queryDeletedOption);
      if (maxFinishTime.HasValue)
        maxFinishTime = new DateTime?(maxFinishTime.Value.EnsureUtc());
      Dictionary<int, Microsoft.TeamFoundation.Build.WebApi.Build> dictionary = new Dictionary<int, Microsoft.TeamFoundation.Build.WebApi.Build>();
      Dictionary<int, int> countsByDefinitionId = new Dictionary<int, int>();
      if (xamlBuildStatus.IsStarted())
      {
        Microsoft.TeamFoundation.Build.Server.BuildQueryOrder xamlBuildQueryOrder = queryOrder.ToXamlBuildQueryOrder();
        if (minFinishTime.HasValue)
          minFinishTime = new DateTime?(minFinishTime.Value.EnsureUtc());
        foreach (Microsoft.TeamFoundation.Build.WebApi.Build completedBuild in this.GetCompletedBuilds(requestContext, projectInfo, definitionIds, queueIds, buildNumber, minFinishTime, maxFinishTime, requestedFor, xamlBuildReason, xamlBuildStatus, xamlBuildQueryOrder, queryDeletedOption1, maxBuildsPerDefinition, count))
        {
          if (!dictionary.ContainsKey(completedBuild.Id))
          {
            dictionary[completedBuild.Id] = completedBuild;
            if (completedBuild.Definition != null)
              countsByDefinitionId[completedBuild.Definition.Id] = countsByDefinitionId.ContainsKey(completedBuild.Definition.Id) ? countsByDefinitionId[completedBuild.Definition.Id] + 1 : 1;
          }
        }
      }
      if (xamlBuildStatus.HasFlag((System.Enum) Microsoft.TeamFoundation.Build.Server.BuildStatus.NotStarted) && !this.IsBuildNumberFilterSpecified(buildNumber))
      {
        List<int> definitionIds1 = (List<int>) null;
        if (definitionIds != null && definitionIds.Count > 0)
          definitionIds1 = definitionIds.Where<int>((Func<int, bool>) (id =>
          {
            if (!countsByDefinitionId.ContainsKey(id))
              return true;
            return maxBuildsPerDefinition.HasValue && countsByDefinitionId[id] < maxBuildsPerDefinition.Value;
          })).ToList<int>();
        if (definitionIds1 == null || definitionIds1.Count > 0)
        {
          foreach (Microsoft.TeamFoundation.Build.WebApi.Build requestedBuild in this.GetRequestedBuilds(requestContext, projectInfo, count, (IList<int>) definitionIds1, queueIds, maxFinishTime, requestedFor, xamlQueueStatus, xamlBuildReason))
          {
            if (!dictionary.ContainsKey(requestedBuild.Id))
            {
              if (requestedBuild.Definition != null && maxBuildsPerDefinition.HasValue)
              {
                countsByDefinitionId[requestedBuild.Definition.Id] = countsByDefinitionId.ContainsKey(requestedBuild.Definition.Id) ? countsByDefinitionId[requestedBuild.Definition.Id] + 1 : 1;
                if (countsByDefinitionId[requestedBuild.Definition.Id] > maxBuildsPerDefinition.Value)
                  continue;
              }
              dictionary[requestedBuild.Id] = requestedBuild;
            }
          }
        }
      }
      return (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build>) dictionary.Values;
    }

    public Microsoft.TeamFoundation.Build.WebApi.Build QueueBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      string checkInTicket)
    {
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      string buildDefinitionUri = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Definition", build.Definition.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      string buildControllerUri1 = (string) null;
      if (build.Controller != null && build.Controller.Id != 0)
        buildControllerUri1 = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Controller", build.Controller.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      if (!System.Enum.IsDefined(typeof (Microsoft.TeamFoundation.Build.WebApi.QueuePriority), (object) build.Priority))
        throw new BuildRequestValidationFailedException(Resources.RequestPropertyInvalid((object) "Priority", (object) build.Priority.ToString()));
      if (!System.Enum.IsDefined(typeof (Microsoft.TeamFoundation.Build.WebApi.BuildReason), (object) build.Reason))
        throw new BuildRequestValidationFailedException(Resources.RequestPropertyInvalid((object) "Reason", (object) build.Reason.ToString()));
      string dropLocation = (string) null;
      string processParameters = (string) null;
      if (!string.IsNullOrEmpty(build.Parameters))
      {
        JObject container = JObject.Parse(build.Parameters);
        if (container != null)
        {
          container.TryGetValue<string>("dropLocation", out dropLocation);
          string msbuildArgs = (string) null;
          if (container.TryGetValue<string>("buildArgs", out msbuildArgs))
          {
            string str = (string) null;
            if (!container.TryGetValue<string>("originalBuildArgs", out str) || msbuildArgs != str)
              processParameters = MSBuildArgsHelper.CreateMSBuildArgsProcessParameters(msbuildArgs);
          }
        }
      }
      BuildRequest buildRequest = new BuildRequest(buildControllerUri1, buildDefinitionUri, dropLocation, (Microsoft.TeamFoundation.Build.Server.QueuePriority) build.Priority, processParameters, (Microsoft.TeamFoundation.Build.Server.BuildReason) build.Reason);
      if (buildRequest.Reason == Microsoft.TeamFoundation.Build.Server.BuildReason.CheckInShelveset || buildRequest.Reason == Microsoft.TeamFoundation.Build.Server.BuildReason.ValidateShelveset)
      {
        buildRequest.CheckInTicket = checkInTicket;
        buildRequest.ShelvesetName = build.SourceBranch;
      }
      BuildQueueQueryResult queueQueryResult;
      try
      {
        ITeamFoundationBuildService foundationBuildService = service;
        IVssRequestContext requestContext1 = requestContext;
        List<BuildRequest> requests = new List<BuildRequest>();
        requests.Add(buildRequest);
        Guid id = projectInfo.SafeGetId();
        queueQueryResult = foundationBuildService.QueueBuilds(requestContext1, (IList<BuildRequest>) requests, Microsoft.TeamFoundation.Build.Server.QueueOptions.None, id);
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
      catch (BuildDefinitionDoesNotExistException ex)
      {
        throw new DefinitionNotFoundException(Resources.DefinitionNotFound((object) build.Definition.Id));
      }
      catch (BuildDefinitionDisabledException ex)
      {
        string projectName = string.Empty;
        if (projectInfo != null)
          projectName = projectInfo.Name;
        else if (!requestContext.GetService<IProjectService>().TryGetProjectName(requestContext.Elevate(), build.Project.Id, out projectName))
          projectName = build.Project.Id.ToString();
        string str = build.Definition.Name;
        if (string.IsNullOrEmpty(str))
        {
          XamlBuildDefinition definition = requestContext.GetService<IXamlDefinitionProvider>().GetDefinition(requestContext, projectInfo, build.Definition.Id);
          str = definition == null ? build.Definition.Id.ToString() : definition.Name;
        }
        throw new DefinitionDisabledException(Resources.DefinitionDisabled((object) str, (object) projectName));
      }
      catch (Microsoft.TeamFoundation.Build.Server.ProcessTemplateDeletedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.ProcessTemplateDeletedException(ex.Message);
      }
      catch (Microsoft.TeamFoundation.Build.Server.ProcessTemplateNotFoundException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.ProcessTemplateNotFoundException(ex.Message);
      }
      List<QueuedBuild> list1 = queueQueryResult.QueuedBuilds.ToList<QueuedBuild>();
      List<BuildDetail> list2 = queueQueryResult.Builds.ToList<BuildDetail>();
      QueuedBuild queuedBuild = list1.ElementAt<QueuedBuild>(0);
      list2.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri));
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> dictionary = queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri));
      IdentityMap identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = this.ConvertQueuedBuildToBuild(requestContext, identityMap, queuedBuild);
      build1.Definition = build.Definition;
      string buildControllerUri2 = queuedBuild.BuildControllerUri;
      Microsoft.TeamFoundation.Build.Server.BuildController controller;
      ref Microsoft.TeamFoundation.Build.Server.BuildController local = ref controller;
      if (dictionary.TryGetValue(buildControllerUri2, out local))
        build1.Controller = controller.ToDataContract(requestContext);
      build1.AddLinks(requestContext);
      return build1;
    }

    public Microsoft.TeamFoundation.Build.WebApi.Build UpdateBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = this.GetBuild(requestContext, projectInfo, build.Id, false, false);
      if (build1 != null)
      {
        string str = Microsoft.TeamFoundation.Build2.Server.UriHelper.CreateBuildUri(build.Id).ToString();
        BuildUpdateOptions buildUpdateOptions1 = new BuildUpdateOptions()
        {
          Uri = str,
          Fields = BuildUpdate.None
        };
        bool flag1 = false;
        bool? keepForever = build.KeepForever;
        if (keepForever.HasValue)
        {
          keepForever = build1.KeepForever;
          bool flag2 = build.KeepForever.Value;
          if (!(keepForever.GetValueOrDefault() == flag2 & keepForever.HasValue))
          {
            BuildUpdateOptions buildUpdateOptions2 = buildUpdateOptions1;
            keepForever = build.KeepForever;
            int num = keepForever.Value ? 1 : 0;
            buildUpdateOptions2.KeepForever = num != 0;
            buildUpdateOptions1.Fields |= BuildUpdate.KeepForever;
            flag1 = true;
          }
        }
        if (build.Quality != null && build1.Quality != build.Quality)
        {
          buildUpdateOptions1.Quality = build.Quality;
          buildUpdateOptions1.Fields |= BuildUpdate.Quality;
          flag1 = true;
        }
        if (flag1)
        {
          BuildDetail buildDetail = requestContext.GetService<ITeamFoundationBuildService>().UpdateBuilds(requestContext, (IList<BuildUpdateOptions>) new BuildUpdateOptions[1]
          {
            buildUpdateOptions1
          }, projectInfo.SafeGetId()).FirstOrDefault<BuildDetail>();
          if (buildDetail != null)
            this.UpdateBuildContract(requestContext, build1, buildDetail);
        }
        Microsoft.TeamFoundation.Build.WebApi.BuildStatus? status = build.Status;
        Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling;
        if (status.GetValueOrDefault() == buildStatus & status.HasValue)
        {
          requestContext.GetService<ITeamFoundationBuildService>().StopBuilds(requestContext, (IList<string>) new string[1]
          {
            str
          }, projectInfo.SafeGetId());
          build1 = this.GetBuild(requestContext, projectInfo, build.Id, false);
        }
      }
      else
      {
        ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
        List<int> source1;
        if (!service.GetQueueIdsByBuildIds(requestContext, projectInfo.SafeGetId(), (IList<int>) new int[1]
        {
          build.Id
        }).TryGetValue(build.Id, out source1))
          throw new BuildNotFoundException(Resources.BuildNotFound((object) build.Id));
        BuildQueueQueryResult queueQueryResult = (BuildQueueQueryResult) null;
        try
        {
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus? status1 = build.Status;
          Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus1 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling;
          if (status1.GetValueOrDefault() == buildStatus1 & status1.HasValue)
          {
            service.CancelBuilds(requestContext, source1.ToArray(), projectInfo.SafeGetId());
          }
          else
          {
            status1 = build.Status;
            Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus2 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed;
            if (!(status1.GetValueOrDefault() == buildStatus2 & status1.HasValue))
            {
              status1 = build.Status;
              Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus3 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted;
              if (!(status1.GetValueOrDefault() == buildStatus3 & status1.HasValue))
              {
                status1 = build.Status;
                Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus4 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress;
                if (status1.GetValueOrDefault() == buildStatus4 & status1.HasValue)
                {
                  queueQueryResult = service.StartQueuedBuildsNow(requestContext, source1.ToArray(), projectInfo.SafeGetId());
                  goto label_24;
                }
                else
                  goto label_24;
              }
            }
            IEnumerable<QueuedBuildUpdateOptions> source2 = source1.Select<int, QueuedBuildUpdateOptions>((Func<int, QueuedBuildUpdateOptions>) (queueId =>
            {
              QueuedBuildUpdateOptions buildUpdateOptions = new QueuedBuildUpdateOptions();
              buildUpdateOptions.QueueId = queueId;
              Microsoft.TeamFoundation.Build.WebApi.BuildStatus? status2 = build.Status;
              Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus5 = Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed;
              buildUpdateOptions.Postponed = status2.GetValueOrDefault() == buildStatus5 & status2.HasValue;
              buildUpdateOptions.Fields = QueuedBuildUpdate.Postponed;
              return buildUpdateOptions;
            }));
            queueQueryResult = service.UpdateQueuedBuilds(requestContext, (IList<QueuedBuildUpdateOptions>) source2.ToList<QueuedBuildUpdateOptions>(), projectInfo.SafeGetId());
          }
        }
        catch (QueuedBuildUpdateException ex)
        {
          throw new BuildStatusInvalidChangeException(ex.Message);
        }
        catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
        {
          throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
        }
        catch (QueuedBuildDoesNotExistException ex)
        {
          throw new BuildNotFoundException(Resources.BuildNotFound((object) build.Id));
        }
label_24:
        if (queueQueryResult != null)
        {
          List<QueuedBuild> queuedBuilds = new List<QueuedBuild>();
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> dictionary1 = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
          Dictionary<string, BuildDetail> dictionary2 = new Dictionary<string, BuildDetail>();
          Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> dictionary3 = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
          this.MergeDictionaries<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>((IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) dictionary1, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) queueQueryResult.Definitions.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>) (x => x.Uri)));
          queuedBuilds.AddRange((IEnumerable<QueuedBuild>) queueQueryResult.QueuedBuilds);
          this.MergeDictionaries<string, BuildDetail>((IDictionary<string, BuildDetail>) dictionary2, (IDictionary<string, BuildDetail>) queueQueryResult.Builds.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri)));
          this.MergeDictionaries<string, Microsoft.TeamFoundation.Build.Server.BuildController>((IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) dictionary3, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri)));
          build1 = this.ConvertQueuedBuildsToBuilds(requestContext, (IList<QueuedBuild>) queuedBuilds, 1, (IDictionary<string, BuildDetail>) dictionary2, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) dictionary3, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) dictionary1).FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>();
        }
      }
      return build1;
    }

    public void DeleteBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId)
    {
      ArtifactId artifactId = new ArtifactId("Build", "Build", buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      try
      {
        ITeamFoundationBuildService foundationBuildService = service;
        IVssRequestContext requestContext1 = requestContext;
        List<string> uris = new List<string>();
        uris.Add(LinkingUtilities.EncodeUri(artifactId));
        Guid id = projectInfo.SafeGetId();
        foundationBuildService.DeleteBuilds(requestContext1, (IList<string>) uris, Microsoft.TeamFoundation.Build.Server.DeleteOptions.All, false, id);
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
    }

    public void DeleteBuildsForDefinition(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      XamlBuildDefinition definition)
    {
      IVssRequestContext requestContext1 = requestContext;
      ProjectInfo projectInfo1 = projectInfo;
      List<int> definitionIds1 = new List<int>();
      definitionIds1.Add(definition.Id);
      DateTime? minFinishTime = new DateTime?();
      DateTime? maxFinishTime1 = new DateTime?();
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus? statusFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?();
      Microsoft.TeamFoundation.Build.WebApi.BuildResult? resultFilter = new Microsoft.TeamFoundation.Build.WebApi.BuildResult?();
      int? maxBuildsPerDefinition = new int?();
      IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> builds = this.GetBuilds(requestContext1, projectInfo1, int.MaxValue, (IList<int>) definitionIds1, (IList<int>) null, "*", minFinishTime, maxFinishTime1, (string) null, Microsoft.TeamFoundation.Build.WebApi.BuildReason.All, statusFilter, resultFilter, Microsoft.TeamFoundation.Build.WebApi.BuildQueryOrder.FinishTimeDescending, Microsoft.TeamFoundation.Build.WebApi.QueryDeletedOption.ExcludeDeleted, maxBuildsPerDefinition);
      List<string> uris1 = new List<string>();
      foreach (Microsoft.TeamFoundation.Build.WebApi.Build build in builds)
      {
        ArtifactId artifactId = new ArtifactId("Build", "Build", build.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        uris1.Add(LinkingUtilities.EncodeUri(artifactId));
      }
      IXamlBuildProvider service1 = requestContext.GetService<IXamlBuildProvider>();
      QueueStatus queueStatus = QueueStatus.InProgress | QueueStatus.Retry | QueueStatus.Queued;
      IVssRequestContext requestContext2 = requestContext;
      ProjectInfo projectInfo2 = projectInfo;
      List<int> definitionIds2 = new List<int>();
      definitionIds2.Add(definition.Id);
      DateTime? maxFinishTime2 = new DateTime?();
      int xamlQueueStatus = (int) queueStatus;
      IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> requestedBuilds = service1.GetRequestedBuilds(requestContext2, projectInfo2, int.MaxValue, (IList<int>) definitionIds2, (IList<int>) null, maxFinishTime2, (string) null, (QueueStatus) xamlQueueStatus, Microsoft.TeamFoundation.Build.Server.BuildReason.All);
      List<string> uris2 = new List<string>();
      foreach (Microsoft.TeamFoundation.Build.WebApi.Build build in requestedBuilds)
      {
        ArtifactId artifactId = new ArtifactId("Build", "Build", build.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        uris2.Add(LinkingUtilities.EncodeUri(artifactId));
      }
      ITeamFoundationBuildService service2 = requestContext.GetService<ITeamFoundationBuildService>();
      try
      {
        service2.StopBuilds(requestContext, (IList<string>) uris2, projectInfo.SafeGetId());
        service2.DeleteBuilds(requestContext, (IList<string>) uris1, Microsoft.TeamFoundation.Build.Server.DeleteOptions.All, false, projectInfo.SafeGetId(), true);
      }
      catch (Microsoft.TeamFoundation.Build.Server.AccessDeniedException ex)
      {
        throw new Microsoft.TeamFoundation.Build.WebApi.AccessDeniedException(ex.Message);
      }
    }

    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> GetCompletedBuilds(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      IList<int> definitionIds,
      IList<int> queueIds,
      string buildNumber,
      DateTime? minFinishTime,
      DateTime? maxFinishTime,
      string requestedFor,
      Microsoft.TeamFoundation.Build.Server.BuildReason xamlBuildReason,
      Microsoft.TeamFoundation.Build.Server.BuildStatus xamlBuildStatus,
      Microsoft.TeamFoundation.Build.Server.BuildQueryOrder xamlQueryOrder,
      Microsoft.TeamFoundation.Build.Server.QueryDeletedOption queryDeletedOption,
      int? maxBuildsPerDefinition,
      int count)
    {
      BuildDetailSpec buildDetailSpec = new BuildDetailSpec()
      {
        DefinitionFilter = (object) new BuildDefinitionSpec()
        {
          FullPath = BuildPath.Root(projectInfo.SafeGetName("*"), "*")
        },
        BuildNumber = buildNumber,
        QueryOrder = xamlQueryOrder,
        Status = xamlBuildStatus,
        QueryOptions = QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.BatchedRequests,
        Reason = xamlBuildReason,
        QueryDeletedOption = queryDeletedOption,
        MaxBuilds = count
      };
      if (maxBuildsPerDefinition.HasValue)
        buildDetailSpec.MaxBuildsPerDefinition = maxBuildsPerDefinition.Value;
      if (!string.IsNullOrEmpty(requestedFor))
        buildDetailSpec.RequestedFor = requestedFor;
      if (definitionIds != null && definitionIds.Any<int>())
        buildDetailSpec.DefinitionFilter = (object) definitionIds.Select<int, string>((Func<int, string>) (definitionId => LinkingUtilities.EncodeUri(new ArtifactId("Build", "Definition", definitionId.ToString()))));
      if (minFinishTime.HasValue)
        buildDetailSpec.MinFinishTime = minFinishTime.Value;
      if (maxFinishTime.HasValue)
        buildDetailSpec.MaxFinishTime = maxFinishTime.Value;
      HashSet<string> requestedControllers = (HashSet<string>) null;
      if (queueIds != null && queueIds.Any<int>())
      {
        requestedControllers = new HashSet<string>();
        foreach (int num in queueIds.Distinct<int>())
        {
          ArtifactId artifactId = new ArtifactId("Build", "Controller", num.ToString());
          requestedControllers.Add(LinkingUtilities.EncodeUri(artifactId));
        }
      }
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      TeamFoundationDataReader foundationDataReader;
      try
      {
        foundationDataReader = service.QueryBuilds(requestContext, (IList<BuildDetailSpec>) new BuildDetailSpec[1]
        {
          buildDetailSpec
        }, projectInfo.SafeGetId());
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        return Enumerable.Empty<Microsoft.TeamFoundation.Build.WebApi.Build>();
      }
      using (foundationDataReader)
      {
        BuildQueryResult buildQueryResult = foundationDataReader.Current<StreamingCollection<BuildQueryResult>>().First<BuildQueryResult>();
        return (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build>) this.ReadBuilds(requestContext, buildQueryResult, requestedControllers).Where<Microsoft.TeamFoundation.Build.WebApi.Build>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, bool>) (b => b != null)).ToList<Microsoft.TeamFoundation.Build.WebApi.Build>();
      }
    }

    private IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> ReadBuilds(
      IVssRequestContext requestContext,
      BuildQueryResult buildQueryResult,
      HashSet<string> requestedControllers)
    {
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
      Dictionary<string, List<QueuedBuild>> urisToRequests = new Dictionary<string, List<QueuedBuild>>();
      IdentityMap identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      Dictionary<string, string> repositoryMap = new Dictionary<string, string>();
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in buildQueryResult.Controllers)
        urisToControllers.Add(controller.Uri, controller);
      foreach (Microsoft.TeamFoundation.Build.Server.BuildDefinition definition in buildQueryResult.Definitions)
        urisToDefinitions.Add(definition.Uri, definition);
      foreach (QueuedBuild queuedBuild in buildQueryResult.QueuedBuilds)
      {
        foreach (string buildUri in queuedBuild.BuildUris)
        {
          List<QueuedBuild> queuedBuildList = (List<QueuedBuild>) null;
          if (!urisToRequests.TryGetValue(buildUri, out queuedBuildList))
          {
            queuedBuildList = new List<QueuedBuild>();
            urisToRequests.Add(buildUri, queuedBuildList);
          }
          queuedBuildList.Add(queuedBuild);
        }
      }
      return buildQueryResult.Builds.Where<BuildDetail>((Func<BuildDetail, bool>) (b =>
      {
        if (b == null)
          return false;
        return requestedControllers == null || requestedControllers.Contains(b.BuildControllerUri);
      })).Select<BuildDetail, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<BuildDetail, Microsoft.TeamFoundation.Build.WebApi.Build>) (b => this.ConvertBuildDetailToDataContract(requestContext, b, identityMap, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) urisToControllers, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) urisToDefinitions, (IDictionary<string, List<QueuedBuild>>) urisToRequests, (IDictionary<string, string>) repositoryMap)));
    }

    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> GetRequestedBuilds(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int count,
      IList<int> definitionIds,
      IList<int> queueIds,
      DateTime? maxFinishTime,
      string requestedFor,
      QueueStatus xamlQueueStatus,
      Microsoft.TeamFoundation.Build.Server.BuildReason xamlBuildReason)
    {
      IList<string> controllerNames = this.GetControllerNames(requestContext, (IEnumerable<int>) queueIds);
      if (controllerNames.Count == 0)
        controllerNames.Add("*");
      IList<string> definitionUris = this.GetDefinitionUris(requestContext, (IEnumerable<int>) definitionIds);
      int num = int.MaxValue;
      if (maxFinishTime.HasValue)
        num = (int) DateTime.UtcNow.Subtract(maxFinishTime.Value).TotalSeconds;
      List<BuildQueueSpec> specs = new List<BuildQueueSpec>();
      foreach (string name in (IEnumerable<string>) controllerNames)
      {
        BuildQueueSpec buildQueueSpec = new BuildQueueSpec();
        buildQueueSpec.ControllerSpec = new BuildControllerSpec(name, "*", false);
        if (definitionUris.Count > 0)
          buildQueueSpec.DefinitionFilter = (object) definitionUris;
        else
          buildQueueSpec.DefinitionFilter = (object) new BuildDefinitionSpec()
          {
            FullPath = BuildPath.Root(projectInfo.SafeGetName("*"), "*")
          };
        buildQueueSpec.CompletedAge = num;
        buildQueueSpec.Reason = xamlBuildReason;
        buildQueueSpec.Status = xamlQueueStatus;
        if (!string.IsNullOrEmpty(requestedFor))
          buildQueueSpec.RequestedFor = requestedFor;
        buildQueueSpec.QueryOptions = QueryOptions.Definitions | QueryOptions.Controllers;
        specs.Add(buildQueueSpec);
      }
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      TeamFoundationDataReader foundationDataReader;
      try
      {
        foundationDataReader = service.QueryQueuedBuilds(requestContext, (IList<BuildQueueSpec>) specs, projectInfo.SafeGetId());
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        return Enumerable.Empty<Microsoft.TeamFoundation.Build.WebApi.Build>();
      }
      List<QueuedBuild> queuedBuilds = new List<QueuedBuild>();
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> dictionary1 = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>();
      Dictionary<string, BuildDetail> dictionary2 = new Dictionary<string, BuildDetail>();
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> dictionary3 = new Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>();
      using (foundationDataReader)
      {
        foreach (BuildQueueQueryResult queueQueryResult in foundationDataReader.Current<StreamingCollection<BuildQueueQueryResult>>())
        {
          this.MergeDictionaries<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>((IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) dictionary1, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) queueQueryResult.Definitions.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>) (x => x.Uri)));
          queuedBuilds.AddRange((IEnumerable<QueuedBuild>) queueQueryResult.QueuedBuilds);
          this.MergeDictionaries<string, BuildDetail>((IDictionary<string, BuildDetail>) dictionary2, (IDictionary<string, BuildDetail>) queueQueryResult.Builds.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri)));
          this.MergeDictionaries<string, Microsoft.TeamFoundation.Build.Server.BuildController>((IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) dictionary3, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri)));
        }
      }
      return this.ConvertQueuedBuildsToBuilds(requestContext, (IList<QueuedBuild>) queuedBuilds, count, (IDictionary<string, BuildDetail>) dictionary2, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) dictionary3, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) dictionary1);
    }

    private IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Build> ConvertQueuedBuildsToBuilds(
      IVssRequestContext requestContext,
      IList<QueuedBuild> queuedBuilds,
      int count,
      IDictionary<string, BuildDetail> uriToBuildDetailMap,
      IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> uriToControllerMap,
      IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> uriToDefinitionMap)
    {
      Dictionary<string, List<QueuedBuild>> uriToQueuedBuildMap = new Dictionary<string, List<QueuedBuild>>();
      foreach (QueuedBuild queuedBuild in (IEnumerable<QueuedBuild>) queuedBuilds)
      {
        foreach (string buildUri in queuedBuild.BuildUris)
        {
          List<QueuedBuild> queuedBuildList = (List<QueuedBuild>) null;
          if (!uriToQueuedBuildMap.TryGetValue(buildUri, out queuedBuildList))
          {
            queuedBuildList = new List<QueuedBuild>();
            uriToQueuedBuildMap[buildUri] = queuedBuildList;
          }
          queuedBuildList.Add(queuedBuild);
        }
      }
      IdentityMap identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      Dictionary<string, string> repositoryMap = new Dictionary<string, string>();
      return queuedBuilds.OrderBy<QueuedBuild, int>((Func<QueuedBuild, int>) (qb => qb.QueuePosition)).Take<QueuedBuild>(count).Select<QueuedBuild, Microsoft.TeamFoundation.Build.WebApi.Build>((Func<QueuedBuild, Microsoft.TeamFoundation.Build.WebApi.Build>) (buildRequest =>
      {
        string key = buildRequest.BuildUris.FirstOrDefault<string>((Func<string, bool>) (b => uriToBuildDetailMap.ContainsKey(b)));
        Microsoft.TeamFoundation.Build.WebApi.Build build;
        if (key != null)
        {
          build = this.ConvertBuildDetailToDataContract(requestContext, uriToBuildDetailMap[key], identityMap, uriToControllerMap, uriToDefinitionMap, (IDictionary<string, List<QueuedBuild>>) uriToQueuedBuildMap, (IDictionary<string, string>) repositoryMap);
        }
        else
        {
          build = this.ConvertQueuedBuildToBuild(requestContext, identityMap, buildRequest);
          Microsoft.TeamFoundation.Build.Server.BuildController controller;
          if (uriToControllerMap.TryGetValue(buildRequest.BuildControllerUri, out controller))
            build.Controller = controller.ToDataContract(requestContext);
          Microsoft.TeamFoundation.Build.Server.BuildDefinition definition;
          if (uriToDefinitionMap.TryGetValue(buildRequest.BuildDefinitionUri, out definition))
            build.Definition = definition.ToReference(requestContext);
          build.AddLinks(requestContext);
        }
        return build;
      }));
    }

    private Microsoft.TeamFoundation.Build.WebApi.Build ConvertQueuedBuildToBuild(
      IVssRequestContext requestContext,
      IdentityMap identityMap,
      QueuedBuild queuedBuild)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build build = new Microsoft.TeamFoundation.Build.WebApi.Build()
      {
        Id = queuedBuild.BuildId.GetValueOrDefault(),
        Priority = queuedBuild.Priority.ToQueuePriority(),
        Project = this.GetProjectReference(requestContext, queuedBuild.ProjectId),
        QueuePosition = new int?(queuedBuild.QueuePosition),
        QueueTime = new DateTime?(queuedBuild.QueueTime),
        Reason = queuedBuild.Reason.ToBuildReason(),
        RequestedBy = identityMap.GetIdentityRef(requestContext, queuedBuild.RequestedBy),
        RequestedFor = identityMap.GetIdentityRef(requestContext, queuedBuild.RequestedFor),
        Status = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(queuedBuild.Status.ToBuildStatus())
      };
      string msBuildArguments = MSBuildArgsHelper.GetMSBuildArguments(queuedBuild.ProcessParameters);
      if (!string.IsNullOrEmpty(msBuildArguments))
        build.Parameters = msBuildArguments;
      return build;
    }

    private TeamProjectReference GetProjectReference(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.GetService<IProjectService>().GetProject(requestContext.Elevate(), projectId).ToTeamProjectReference(requestContext);
    }

    private void MergeDictionaries<TKey, TValue>(
      IDictionary<TKey, TValue> target,
      IDictionary<TKey, TValue> source)
    {
      foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) source)
      {
        if (!target.ContainsKey(keyValuePair.Key))
          target.Add(keyValuePair.Key, keyValuePair.Value);
      }
    }

    private IList<string> GetControllerNames(
      IVssRequestContext requestContext,
      IEnumerable<int> queueIds)
    {
      if (queueIds == null)
        return (IList<string>) new List<string>();
      List<string> list = queueIds.Distinct<int>().Select<int, string>((Func<int, string>) (queueId => Microsoft.TeamFoundation.Build2.Server.UriHelper.CreateArtifactUri("Controller", queueId.ToString((IFormatProvider) CultureInfo.InvariantCulture)).ToString())).ToList<string>();
      return (IList<string>) requestContext.GetService<TeamFoundationBuildResourceService>().QueryBuildControllersByUri(requestContext, (IList<string>) list, (IList<string>) null, false).Controllers.Where<Microsoft.TeamFoundation.Build.Server.BuildController>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, bool>) (controller => controller != null)).Select<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (controller => controller.Name)).ToList<string>();
    }

    private IList<string> GetDefinitionUris(
      IVssRequestContext requestContext,
      IEnumerable<int> definitionIds)
    {
      return definitionIds == null ? (IList<string>) new List<string>() : (IList<string>) definitionIds.Distinct<int>().Select<int, string>((Func<int, string>) (definitionId => Microsoft.TeamFoundation.Build2.Server.UriHelper.CreateArtifactUri("Definition", definitionId.ToString((IFormatProvider) CultureInfo.InvariantCulture)).ToString())).ToList<string>();
    }

    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> GetArtifacts(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      ApiResourceVersion resourceVersion,
      string artifactName = null)
    {
      if (!string.IsNullOrEmpty(artifactName) && !string.Equals(artifactName, "drop", StringComparison.OrdinalIgnoreCase))
        return Enumerable.Empty<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>();
      BuildDetail singleBuild = this.GetSingleBuild(requestContext, projectInfo, buildId);
      if (singleBuild == null)
        return this.GetEmptyEnumerableForBuildRequest<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>(requestContext, projectInfo, buildId);
      if (projectInfo.SafeGetId() == Guid.Empty)
      {
        Guid projectId = singleBuild.ProjectId;
      }
      List<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact> artifacts = new List<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>();
      if (!string.IsNullOrEmpty(singleBuild.DropLocation))
      {
        Microsoft.TeamFoundation.Build.WebApi.BuildArtifact artifact = new Microsoft.TeamFoundation.Build.WebApi.BuildArtifact()
        {
          Name = "drop",
          Resource = new ArtifactResource()
          {
            Data = singleBuild.DropLocation
          }
        };
        artifact.Resource.TryInferType();
        artifacts.Add(artifact.UpdateReferences(requestContext, projectInfo.SafeGetId(), buildId, resourceVersion));
      }
      return (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.BuildArtifact>) artifacts;
    }

    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change> GetBuildChanges(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int top,
      int maxMessageLength)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      if (this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) new string[2]
      {
        InformationTypes.AssociatedCommit,
        InformationTypes.AssociatedChangeset
      }, out informationNodes) == null)
        return this.GetEmptyEnumerableForBuildRequest<Microsoft.TeamFoundation.Build.WebApi.Change>(requestContext, projectInfo, buildId);
      IEnumerable<Microsoft.TeamFoundation.Build.WebApi.Change> source = informationNodes.Select<BuildInformationNode, Microsoft.TeamFoundation.Build.WebApi.Change>((Func<BuildInformationNode, Microsoft.TeamFoundation.Build.WebApi.Change>) (node =>
      {
        if (string.Equals(node.Type, InformationTypes.AssociatedCommit, StringComparison.OrdinalIgnoreCase))
          return this.GetChangeFromAssociatedCommit(requestContext, node, maxMessageLength);
        return string.Equals(node.Type, InformationTypes.AssociatedChangeset, StringComparison.OrdinalIgnoreCase) ? this.GetChangeFromAssociatedChangeset(requestContext, node) : (Microsoft.TeamFoundation.Build.WebApi.Change) null;
      })).Where<Microsoft.TeamFoundation.Build.WebApi.Change>((Func<Microsoft.TeamFoundation.Build.WebApi.Change, bool>) (change => change != null)).Take<Microsoft.TeamFoundation.Build.WebApi.Change>(top);
      Microsoft.TeamFoundation.Build.WebApi.Change change1 = source.FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Change>();
      return change1 != null && change1.Type == "TfsVersionControl" ? source.Reverse<Microsoft.TeamFoundation.Build.WebApi.Change>() : source;
    }

    public IEnumerable<Issue> GetBuildIssues(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int top)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      return this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) new string[2]
      {
        InformationTypes.BuildError,
        InformationTypes.BuildWarning
      }, out informationNodes) == null ? this.GetEmptyEnumerableForBuildRequest<Issue>(requestContext, projectInfo, buildId) : informationNodes.Select<BuildInformationNode, Issue>((Func<BuildInformationNode, Issue>) (node =>
      {
        if (string.Equals(node.Type, InformationTypes.BuildError, StringComparison.OrdinalIgnoreCase))
          return this.GetIssueFromNode(requestContext, node, IssueType.Error);
        return string.Equals(node.Type, InformationTypes.BuildWarning, StringComparison.OrdinalIgnoreCase) ? this.GetIssueFromNode(requestContext, node, IssueType.Warning) : (Issue) null;
      })).Where<Issue>((Func<Issue, bool>) (issue => issue != null)).Take<Issue>(top);
    }

    public IEnumerable<ResourceRef> GetBuildWorkItemRefs(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int top)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      if (this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) new string[1]
      {
        InformationTypes.AssociatedWorkItem
      }, out informationNodes) == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      return informationNodes.Select<BuildInformationNode, ResourceRef>((Func<BuildInformationNode, ResourceRef>) (node => string.Equals(node.Type, InformationTypes.AssociatedWorkItem, StringComparison.OrdinalIgnoreCase) ? this.GetWorkItemRefFromInformationNode(requestContext, node) : (ResourceRef) null)).Where<ResourceRef>((Func<ResourceRef, bool>) (node => node != null)).Take<ResourceRef>(top);
    }

    public IEnumerable<Deployment> GetBuildDeployments(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      if (this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) new string[1]
      {
        InformationTypes.DeploymentInformation
      }, out informationNodes) == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      return informationNodes.Select<BuildInformationNode, Deployment>((Func<BuildInformationNode, Deployment>) (informationNode => string.Equals(informationNode.Type, InformationTypes.DeploymentInformation, StringComparison.OrdinalIgnoreCase) ? this.GetDeploymentDataForInformationNode(requestContext, informationNode) : (Deployment) null)).Where<Deployment>((Func<Deployment, bool>) (informationNode => informationNode != null));
    }

    private Deployment GetDeploymentDataForInformationNode(
      IVssRequestContext requestContext,
      BuildInformationNode node)
    {
      Dictionary<string, string> dictionary = node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (field => field.Name), (Func<InformationField, string>) (field => field.Value));
      string a = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.DeploymentInformationType);
      if (!string.IsNullOrEmpty(a))
      {
        if (string.Equals(a, DeploymentInformationTypes.Build, StringComparison.OrdinalIgnoreCase))
        {
          int num = int.Parse(LinkingUtilities.DecodeUri(CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Url)).ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture);
          DeploymentBuild forInformationNode = new DeploymentBuild();
          forInformationNode.Type = "Build";
          forInformationNode.BuildId = num;
          return (Deployment) forInformationNode;
        }
        if (string.Equals(a, DeploymentInformationTypes.Deploy, StringComparison.OrdinalIgnoreCase) || string.Equals(a, DeploymentInformationTypes.ConnectToSnapshot, StringComparison.OrdinalIgnoreCase))
        {
          DeploymentDeploy forInformationNode = new DeploymentDeploy();
          forInformationNode.Type = "Deploy";
          forInformationNode.Message = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Message) + CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Url);
          return (Deployment) forInformationNode;
        }
        if (string.Equals(a, DeploymentInformationTypes.Test, StringComparison.OrdinalIgnoreCase))
        {
          string s = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Url);
          DeploymentTest forInformationNode = new DeploymentTest();
          forInformationNode.Type = "Test";
          forInformationNode.RunId = int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture);
          return (Deployment) forInformationNode;
        }
      }
      return (Deployment) null;
    }

    public IEnumerable<BuildLog> GetBuildLogsMetadata(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      BuildDetail singleBuild = this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) new string[2]
      {
        InformationTypes.IntermediateLogInformation,
        InformationTypes.AgentScopeActivityTracking
      }, out informationNodes);
      if (singleBuild == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      if (!singleBuild.ContainerId.HasValue || string.IsNullOrEmpty(singleBuild.DropLocation) || !singleBuild.DropLocation.StartsWith("#"))
        throw new InvalidLogLocationException(Resources.RestApiInvalidLogLocation());
      List<BuildLog> buildLogsMetadata = new List<BuildLog>();
      BuildLog activityLog = this.GetActivityLog(requestContext, projectInfo, buildId, singleBuild);
      if (activityLog != null)
      {
        buildLogsMetadata.Add(activityLog);
        foreach (BuildInformationNode node in informationNodes)
        {
          if (string.Equals(node.Type, InformationTypes.IntermediateLogInformation, StringComparison.OrdinalIgnoreCase))
            buildLogsMetadata.Add(this.ConvertToBuildLog(requestContext, projectInfo, buildId, node));
          else if (string.Equals(node.Type, InformationTypes.AgentScopeActivityTracking, StringComparison.OrdinalIgnoreCase))
          {
            Dictionary<string, string> dictionary = node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (field => field.Name), (Func<InformationField, string>) (field => field.Value));
            string str = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.ReservedAgentUri);
            if (!string.IsNullOrEmpty(str) && str.StartsWith("vstfs:///Build/Agent/", StringComparison.OrdinalIgnoreCase))
              buildLogsMetadata.Add(this.ConvertToBuildLog(requestContext, projectInfo, buildId, node, (IDictionary<string, string>) dictionary));
          }
        }
      }
      return (IEnumerable<BuildLog>) buildLogsMetadata;
    }

    public PushStreamContent GetBuildLogsZip(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      BuildDetail build = this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) new string[1]
      {
        InformationTypes.IntermediateLogInformation
      }, out informationNodes);
      if (build == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      if (!build.ContainerId.HasValue || string.IsNullOrEmpty(build.LogLocation) || !build.LogLocation.StartsWith("#"))
        throw new InvalidLogLocationException(Resources.RestApiInvalidLogLocation());
      string path = build.LogLocation.Split(new char[1]
      {
        '/'
      }, 3)[2];
      return this.GetContainerItems(requestContext, projectInfo, build, path).Count == 0 ? (PushStreamContent) null : new PushStreamContent((Action<Stream, HttpContent, TransportContext>) ((stream, httpContent, transportContext) =>
      {
        try
        {
          requestContext.GetService<TeamFoundationFileContainerService>().WriteContents(requestContext, build.ContainerId.Value, path, stream, build.ProjectId, false, true);
        }
        finally
        {
          stream?.Dispose();
        }
      }), new MediaTypeHeaderValue("application/zip"));
    }

    public StreamContent GetBuildLog(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      int logId)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      BuildDetail singleBuild = this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) new string[2]
      {
        InformationTypes.IntermediateLogInformation,
        InformationTypes.AgentScopeActivityTracking
      }, out informationNodes);
      if (singleBuild == null)
        throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
      if (!singleBuild.ContainerId.HasValue || string.IsNullOrEmpty(singleBuild.LogLocation) || !singleBuild.LogLocation.StartsWith("#"))
        throw new InvalidLogLocationException(Resources.RestApiInvalidLogLocation());
      string[] strArray = singleBuild.LogLocation.Split(new char[1]
      {
        '/'
      }, 3);
      string path;
      if (logId == 0)
      {
        path = strArray[2] + "/ActivityLog.xml";
      }
      else
      {
        BuildInformationNode buildInformationNode = informationNodes.FirstOrDefault<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => node.NodeId == logId));
        if (buildInformationNode == null)
          return (StreamContent) null;
        InformationField informationField = buildInformationNode.Fields.FirstOrDefault<InformationField>((Func<InformationField, bool>) (f => f.Name == InformationFields.ReservedAgentUri));
        if (informationField == null || string.IsNullOrEmpty(informationField.Value))
          return (StreamContent) null;
        path = strArray[2] + string.Format("/ActivityLog.AgentScope.{0}.xml", (object) LinkingUtilities.DecodeUri(informationField.Value).ToolSpecificId);
      }
      FileContainerItem containerItem = this.GetContainerItem(requestContext, projectInfo, singleBuild, path);
      return containerItem == null ? (StreamContent) null : new StreamContent(this.GetFileStream(requestContext, containerItem.FileId));
    }

    private BuildLog GetActivityLog(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      BuildDetail build)
    {
      string path = build.LogLocation.Split(new char[1]
      {
        '/'
      }, 3)[2] + "/ActivityLog.xml";
      if (this.GetContainerItem(requestContext, projectInfo, build, path) == null)
        return (BuildLog) null;
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      BuildLog activityLog = new BuildLog();
      activityLog.Id = 0;
      activityLog.CreatedOn = new DateTime?(build.StartTime);
      activityLog.Type = "Container";
      activityLog.Url = service.GetBuildLogRestUrl(requestContext, projectInfo.SafeGetId(), buildId, 0);
      return activityLog;
    }

    private FileContainerItem GetContainerItem(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDetail build,
      string path)
    {
      List<FileContainerItem> containerItems = this.GetContainerItems(requestContext, projectInfo, build, path);
      if (containerItems.Count == 0)
      {
        containerItems = this.GetContainerItems(requestContext, projectInfo, build, Uri.UnescapeDataString(path));
        if (containerItems.Count == 0)
          return (FileContainerItem) null;
      }
      else if (containerItems.Count > 1)
        return (FileContainerItem) null;
      FileContainerItem fileContainerItem = containerItems[0];
      return fileContainerItem.ItemType == ContainerItemType.Folder ? (FileContainerItem) null : fileContainerItem;
    }

    private Stream GetFileStream(IVssRequestContext requestContext, int fileId) => requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _);

    private List<FileContainerItem> GetContainerItems(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      BuildDetail build,
      string path)
    {
      return requestContext.GetService<TeamFoundationFileContainerService>().QueryItems(requestContext, build.ContainerId.Value, path, projectInfo.SafeGetId(), false, false);
    }

    private BuildLog ConvertToBuildLog(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      BuildInformationNode node)
    {
      return this.ConvertToBuildLog(requestContext, projectInfo, buildId, node, (IDictionary<string, string>) node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (field => field.Name), (Func<InformationField, string>) (field => field.Value)));
    }

    private BuildLog ConvertToBuildLog(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      BuildInformationNode node,
      IDictionary<string, string> fields)
    {
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      BuildLog buildLog1 = new BuildLog();
      buildLog1.Id = node.NodeId;
      buildLog1.Url = service.GetBuildLogRestUrl(requestContext, projectInfo.SafeGetId(), buildId, node.NodeId);
      buildLog1.Type = "Container";
      BuildLog buildLog2 = buildLog1;
      DateTime dateTime = CommonInformationHelper.GetDateTime(fields, InformationFields.Timestamp);
      if (dateTime != DateTime.MinValue)
        buildLog2.CreatedOn = new DateTime?(dateTime);
      return buildLog2;
    }

    private Microsoft.TeamFoundation.Build.WebApi.Change GetChangeFromAssociatedCommit(
      IVssRequestContext requestContext,
      BuildInformationNode node,
      int maxMessageLength)
    {
      Dictionary<string, string> dictionary = node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (field => field.Name), (Func<InformationField, string>) (field => field.Value));
      string str = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.CommitId);
      Microsoft.TeamFoundation.Build.WebApi.Change associatedCommit = new Microsoft.TeamFoundation.Build.WebApi.Change()
      {
        Id = str,
        Author = new IdentityRef()
        {
          DisplayName = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Author)
        },
        Message = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Message),
        Type = "Git"
      };
      if (associatedCommit.Message.Length > maxMessageLength)
      {
        associatedCommit.Message = associatedCommit.Message.Substring(0, maxMessageLength);
        associatedCommit.MessageTruncated = true;
      }
      DateTime dateTime = CommonInformationHelper.GetDateTime((IDictionary<string, string>) dictionary, InformationFields.Timestamp);
      if (dateTime != DateTime.MinValue)
        associatedCommit.Timestamp = new DateTime?(dateTime);
      string uri = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Uri);
      if (!string.IsNullOrEmpty(uri))
      {
        string toolSpecificId = LinkingUtilities.DecodeUri(uri).ToolSpecificId;
        if (!string.IsNullOrEmpty(toolSpecificId))
        {
          string[] strArray = toolSpecificId.Split('/');
          Guid result;
          if (strArray.Length == 3 && Guid.TryParse(strArray[1], out result))
          {
            associatedCommit.Type = "TfsGit";
            ILocationService service = requestContext.GetService<ILocationService>();
            associatedCommit.Location = service.GetResourceUri(requestContext, "git", GitWebApiConstants.CommitsLocationId, (object) new
            {
              repositoryId = result,
              commitId = str
            });
          }
        }
      }
      return associatedCommit;
    }

    private Issue GetIssueFromNode(
      IVssRequestContext requestContext,
      BuildInformationNode node,
      IssueType type)
    {
      Dictionary<string, string> dictionary = node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (field => field.Name), (Func<InformationField, string>) (field => field.Value));
      string str1 = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Message);
      Issue issueFromNode = new Issue()
      {
        Message = str1,
        Type = type
      };
      string str2 = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.File);
      string str3 = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.LineNumber);
      if (!string.IsNullOrEmpty(str2) || !string.IsNullOrEmpty(str3))
      {
        issueFromNode.Category = "code";
        issueFromNode.Data.Add(new KeyValuePair<string, string>("sourcePath", str2));
        issueFromNode.Data.Add(new KeyValuePair<string, string>("lineNumber", str3));
        issueFromNode.Data.Add(new KeyValuePair<string, string>("message", str1));
      }
      return issueFromNode;
    }

    private ResourceRef GetWorkItemRefFromInformationNode(
      IVssRequestContext requestContext,
      BuildInformationNode node)
    {
      int id = CommonInformationHelper.GetInt((IDictionary<string, string>) node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (field => field.Name), (Func<InformationField, string>) (field => field.Value)), InformationFields.WorkItemId);
      ILocationService service = requestContext.GetService<ILocationService>();
      return new ResourceRef()
      {
        Id = id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
        Url = WITHelper.GetWorkItemUrlById(requestContext, id, service)
      };
    }

    private Microsoft.TeamFoundation.Build.WebApi.Change GetChangeFromAssociatedChangeset(
      IVssRequestContext requestContext,
      BuildInformationNode node)
    {
      Dictionary<string, string> dictionary = node.Fields.ToDictionary<InformationField, string, string>((Func<InformationField, string>) (field => field.Name), (Func<InformationField, string>) (field => field.Value));
      int num = CommonInformationHelper.GetInt((IDictionary<string, string>) dictionary, InformationFields.ChangesetId);
      return new Microsoft.TeamFoundation.Build.WebApi.Change()
      {
        Id = string.Format("C{0}", (object) num),
        Message = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.Comment),
        Author = new IdentityRef()
        {
          DisplayName = CommonInformationHelper.GetString((IDictionary<string, string>) dictionary, InformationFields.CheckedInBy)
        },
        Type = "TfsVersionControl",
        Location = requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "tfvc", TfvcConstants.TfvcChangesetsLocationId, (object) new
        {
          id = num
        })
      };
    }

    private BuildDetail GetSingleBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId)
    {
      List<BuildInformationNode> informationNodes = (List<BuildInformationNode>) null;
      return this.GetSingleBuild(requestContext, projectInfo, buildId, (IList<string>) null, out informationNodes);
    }

    private BuildDetail GetSingleBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      IList<string> informationTypes,
      out List<BuildInformationNode> informationNodes)
    {
      string str = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Build", buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      informationNodes = new List<BuildInformationNode>();
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      TeamFoundationDataReader foundationDataReader;
      try
      {
        foundationDataReader = service.QueryBuildsByUri(requestContext, (IList<string>) new string[1]
        {
          str
        }, informationTypes, QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.BatchedRequests, Microsoft.TeamFoundation.Build.Server.QueryDeletedOption.ExcludeDeleted, projectInfo.SafeGetId(), true);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        return (BuildDetail) null;
      }
      BuildDetail singleBuild = (BuildDetail) null;
      using (foundationDataReader)
      {
        singleBuild = foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>();
        if (singleBuild != null)
          informationNodes.AddRange((IEnumerable<BuildInformationNode>) singleBuild.Information);
      }
      return singleBuild;
    }

    private Microsoft.TeamFoundation.Build.WebApi.Build GetBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId,
      bool tryBuildQueue,
      bool includeDeleted)
    {
      string str = LinkingUtilities.EncodeUri(new ArtifactId("Build", "Build", buildId.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      TeamFoundationDataReader foundationDataReader;
      try
      {
        foundationDataReader = service.QueryBuildsByUri(requestContext, (IList<string>) new string[1]
        {
          str
        }, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.BatchedRequests, (Microsoft.TeamFoundation.Build.Server.QueryDeletedOption) (includeDeleted ? 1 : 0), projectInfo.SafeGetId(), true);
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      }
      Microsoft.TeamFoundation.Build.WebApi.Build build = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      using (foundationDataReader)
      {
        BuildQueryResult buildQueryResult = foundationDataReader.Current<BuildQueryResult>();
        build = this.ReadBuilds(requestContext, buildQueryResult, (HashSet<string>) null).FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>();
      }
      if (build == null & tryBuildQueue)
        build = this.GetQueuedBuild(requestContext, projectInfo, buildId);
      return build;
    }

    private Microsoft.TeamFoundation.Build.WebApi.Build GetQueuedBuild(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId)
    {
      Microsoft.TeamFoundation.Build.WebApi.Build queuedBuild = (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      List<int> intList;
      if (service.GetQueueIdsByBuildIds(requestContext, projectInfo.SafeGetId(), (IList<int>) new int[1]
      {
        buildId
      }).TryGetValue(buildId, out intList) && intList.Count > 0)
      {
        TeamFoundationDataReader foundationDataReader;
        try
        {
          foundationDataReader = service.QueryQueuedBuildsById(requestContext, (IList<int>) new int[1]
          {
            intList[0]
          }, (IList<string>) null, QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.BatchedRequests, false, projectInfo.SafeGetId());
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
        }
        List<QueuedBuild> list;
        Dictionary<string, BuildDetail> dictionary1;
        Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> dictionary2;
        Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> dictionary3;
        using (foundationDataReader)
        {
          BuildQueueQueryResult queueQueryResult = foundationDataReader.Current<BuildQueueQueryResult>();
          list = queueQueryResult.QueuedBuilds.Take<QueuedBuild>(1).ToList<QueuedBuild>();
          dictionary1 = queueQueryResult.Builds.ToDictionary<BuildDetail, string>((Func<BuildDetail, string>) (x => x.Uri));
          dictionary2 = queueQueryResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (x => x.Uri));
          dictionary3 = queueQueryResult.Definitions.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildDefinition, string>) (x => x.Uri));
        }
        queuedBuild = this.ConvertQueuedBuildsToBuilds(requestContext, (IList<QueuedBuild>) list, 1, (IDictionary<string, BuildDetail>) dictionary1, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController>) dictionary2, (IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition>) dictionary3).FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>();
      }
      return queuedBuild;
    }

    private Microsoft.TeamFoundation.Build.WebApi.Build ConvertBuildDetailToDataContract(
      IVssRequestContext requestContext,
      BuildDetail buildDetail,
      IdentityMap identityMap,
      IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> urisToControllers,
      IDictionary<string, Microsoft.TeamFoundation.Build.Server.BuildDefinition> urisToDefinitions,
      IDictionary<string, List<QueuedBuild>> urisToRequests,
      IDictionary<string, string> repositoryMap)
    {
      if (buildDetail == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      Microsoft.TeamFoundation.Build.WebApi.Build build1 = new Microsoft.TeamFoundation.Build.WebApi.Build();
      this.UpdateBuildContract(requestContext, build1, buildDetail, identityMap);
      Microsoft.TeamFoundation.Build.Server.BuildController controller;
      if (urisToControllers.TryGetValue(buildDetail.BuildControllerUri, out controller))
        build1.Controller = controller.ToDataContract(requestContext);
      Microsoft.TeamFoundation.Build.Server.BuildDefinition definition;
      if (urisToDefinitions.TryGetValue(buildDetail.BuildDefinitionUri, out definition))
      {
        build1.Definition = definition.ToReference(requestContext);
        Microsoft.TeamFoundation.Build.Server.BuildDefinitionSourceProvider sourceProvider = definition.SourceProviders.FirstOrDefault<Microsoft.TeamFoundation.Build.Server.BuildDefinitionSourceProvider>();
        if (sourceProvider != null)
        {
          IXamlSourceProvider service = requestContext.GetService<IXamlSourceProvider>();
          build1.Repository = service.ConvertXamlSourceProviderToBuildRepository(requestContext, definition, sourceProvider, repositoryMap);
        }
      }
      List<QueuedBuild> source;
      if (urisToRequests.TryGetValue(buildDetail.Uri, out source))
      {
        Microsoft.TeamFoundation.Build.WebApi.Build build2 = build1;
        QueuedBuild queuedBuild1 = source.Find((Predicate<QueuedBuild>) (qb => qb.RequestedByIdentity != null));
        IdentityRef identityRef1 = queuedBuild1 != null ? queuedBuild1.RequestedByIdentity.ToIdentityRef(requestContext) : (IdentityRef) null;
        build2.RequestedBy = identityRef1;
        Microsoft.TeamFoundation.Build.WebApi.Build build3 = build1;
        QueuedBuild queuedBuild2 = source.Find((Predicate<QueuedBuild>) (qb => qb.RequestedForIdentity != null));
        IdentityRef identityRef2 = queuedBuild2 != null ? queuedBuild2.RequestedForIdentity.ToIdentityRef(requestContext) : (IdentityRef) null;
        build3.RequestedFor = identityRef2;
        if (build1.RequestedBy == null)
          build1.RequestedBy = source.Where<QueuedBuild>((Func<QueuedBuild, bool>) (qb => !string.IsNullOrEmpty(qb.RequestedBy))).Select<QueuedBuild, string>((Func<QueuedBuild, string>) (qb => qb.RequestedBy)).Select<string, IdentityRef>((Func<string, IdentityRef>) (rb => identityMap.GetIdentityRef(requestContext, rb))).FirstOrDefault<IdentityRef>();
        if (build1.RequestedFor == null)
          build1.RequestedFor = source.Where<QueuedBuild>((Func<QueuedBuild, bool>) (qb => !string.IsNullOrEmpty(qb.RequestedFor))).Select<QueuedBuild, string>((Func<QueuedBuild, string>) (qb => qb.RequestedFor)).Select<string, IdentityRef>((Func<string, IdentityRef>) (rf => identityMap.GetIdentityRef(requestContext, rf))).FirstOrDefault<IdentityRef>();
        DateTime dateTime = source.Where<QueuedBuild>((Func<QueuedBuild, bool>) (qb => qb.QueueTime != DateTime.MinValue)).Select<QueuedBuild, DateTime>((Func<QueuedBuild, DateTime>) (qb => qb.QueueTime)).FirstOrDefault<DateTime>();
        if (dateTime != DateTime.MinValue)
          build1.QueueTime = new DateTime?(dateTime);
      }
      Uri result = (Uri) null;
      if (!string.IsNullOrEmpty(buildDetail.LogLocation))
      {
        IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
        if (buildDetail.LogLocation.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
          build1.Logs = new BuildLogReference()
          {
            Type = "VersionControl",
            Url = service.GetTfvcItemRestUrl(requestContext, buildDetail.LogLocation)
          };
        else if (buildDetail.LogLocation.StartsWith("#", StringComparison.OrdinalIgnoreCase))
          build1.Logs = new BuildLogReference()
          {
            Type = "Container",
            Url = service.GetBuildLogsRestUrl(requestContext, buildDetail.Definition.TeamProject.Id, build1.Id)
          };
        else if (Uri.TryCreate(buildDetail.LogLocation, UriKind.RelativeOrAbsolute, out result))
        {
          string str = string.Empty;
          if (result.IsUnc)
            str = this.GetLogUrl("file://///" + result.Authority + result.PathAndQuery);
          else if (result.IsFile)
            str = this.GetLogUrl("file:///" + result.Authority + result.PathAndQuery);
          build1.Logs = new BuildLogReference()
          {
            Type = "FilePath",
            Url = str
          };
        }
      }
      build1.AddLinks(requestContext);
      return build1;
    }

    private void UpdateBuildContract(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.Build build,
      BuildDetail buildDetail,
      IdentityMap identityMap = null)
    {
      if (identityMap == null)
        identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      int buildId = int.Parse(LinkingUtilities.DecodeUri(buildDetail.Uri).ToolSpecificId, (IFormatProvider) CultureInfo.InvariantCulture);
      Microsoft.TeamFoundation.Build.WebApi.BuildStatus buildStatus;
      Microsoft.TeamFoundation.Build.WebApi.BuildResult? buildResult;
      XamlBuildExtensions.DecodeXamlBuildStatus(buildDetail.Status, out buildStatus, out buildResult);
      Guid projectId = buildDetail.Definition?.TeamProject?.Id ?? Guid.Empty;
      build.Id = buildId;
      build.BuildNumber = buildDetail.BuildNumber;
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      build.Url = service.GetBuildRestUrl(requestContext, projectId, buildId);
      build.Uri = new Uri(buildDetail.Uri);
      build.KeepForever = new bool?(buildDetail.KeepForever);
      build.LastChangedBy = identityMap.GetIdentityRef(requestContext, buildDetail.LastChangedBy);
      build.LastChangedDate = buildDetail.LastChangedOn;
      build.Parameters = MSBuildArgsHelper.GetMSBuildArguments(buildDetail.ProcessParameters);
      build.Project = this.GetProjectReference(requestContext, projectId);
      build.Reason = buildDetail.Reason.ToBuildReason();
      build.Result = buildResult;
      build.SourceVersion = buildDetail.SourceGetVersion;
      build.Status = new Microsoft.TeamFoundation.Build.WebApi.BuildStatus?(buildStatus);
      build.Quality = buildDetail.Quality;
      build.Deleted = buildDetail.IsDeleted;
      if (buildDetail.StartTime != DateTime.MinValue)
        build.StartTime = new DateTime?(buildDetail.StartTime);
      if (!(buildDetail.FinishTime != DateTime.MinValue))
        return;
      build.FinishTime = new DateTime?(buildDetail.FinishTime);
    }

    private string GetLogUrl(string url)
    {
      string[] strArray = url.Split(new char[1]{ '?' }, 2);
      string logUrl = strArray[0].TrimEnd('/');
      if (strArray.Length > 1)
        logUrl = logUrl + "?" + strArray[1];
      return logUrl;
    }

    private IEnumerable<T> GetEmptyEnumerableForBuildRequest<T>(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo,
      int buildId)
    {
      ITeamFoundationBuildService service = requestContext.GetService<ITeamFoundationBuildService>();
      List<int> intList;
      if (service.GetQueueIdsByBuildIds(requestContext, projectInfo.SafeGetId(), (IList<int>) new int[1]
      {
        buildId
      }).TryGetValue(buildId, out intList) && intList.Count > 0)
      {
        TeamFoundationDataReader foundationDataReader;
        try
        {
          foundationDataReader = service.QueryQueuedBuildsById(requestContext, (IList<int>) new int[1]
          {
            intList[0]
          }, (IList<string>) null, QueryOptions.None, false, projectInfo.SafeGetId());
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
        }
        QueuedBuild queuedBuild = (QueuedBuild) null;
        using (foundationDataReader)
          queuedBuild = foundationDataReader.Current<BuildQueueQueryResult>().QueuedBuilds.FirstOrDefault<QueuedBuild>();
        if (queuedBuild != null)
          return Enumerable.Empty<T>();
      }
      throw new BuildNotFoundException(Resources.BuildNotFound((object) buildId));
    }

    private bool IsBuildNumberFilterSpecified(string buildNumber) => !string.IsNullOrEmpty(buildNumber) && buildNumber != "*";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
