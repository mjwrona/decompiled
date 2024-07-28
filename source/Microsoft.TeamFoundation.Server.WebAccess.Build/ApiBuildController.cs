// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.ApiBuildController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Mail;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.TeamFoundation.TestImpact.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Xsl;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  [SupportedRouteArea("Api", NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiBuildController : BuildAreaController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514000, 514010)]
    public ActionResult Index() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514015, 515019)]
    public ActionResult QueueBuildDialog()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.View();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514020, 514030)]
    [ValidateInput(false)]
    public ActionResult QueueBuild(
      string definitionUri,
      string controllerUri,
      string whatToBuild,
      string shelveset,
      bool? checkin,
      string priority,
      string dropFolder,
      string msbuildArgs,
      string msbuildArgsOriginal)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      string str = string.Empty;
      msbuildArgs = msbuildArgs == null ? string.Empty : msbuildArgs.Trim();
      msbuildArgsOriginal = msbuildArgsOriginal == null ? string.Empty : msbuildArgsOriginal.Trim();
      if (msbuildArgs != msbuildArgsOriginal)
        str = MSBuildArgsHelper.CreateMSBuildArgsProcessParameters(msbuildArgs);
      BuildRequest buildRequest = new BuildRequest()
      {
        BuildDefinitionUri = definitionUri,
        BuildControllerUri = controllerUri,
        DropLocation = dropFolder,
        Priority = this.GetQueuePriority(priority),
        Reason = this.GetBuildReason(whatToBuild, checkin),
        ShelvesetName = shelveset,
        ProcessParameters = str
      };
      this.TfsRequestContext.Trace(514020, TraceLevel.Info, "WebAccess.Build", TfsTraceLayers.Controller, "Queuing new build Definition Uri: {0} ControllerUri: {1} whatToBuild: {2} shelveset: {3} priority: {4} dropFolder: {5} msbuildArgs: {6}", (object) definitionUri, (object) controllerUri, (object) whatToBuild, (object) shelveset, (object) priority, (object) dropFolder, (object) msbuildArgs);
      this.XamlBuildService.QueueBuilds(this.TfsRequestContext, (IList<BuildRequest>) new BuildRequest[1]
      {
        buildRequest
      }, QueueOptions.None, new Guid());
      JsObject data = new JsObject();
      data.Add("success", (object) true);
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    private BuildReason GetBuildReason(string whatToBuild, bool? checkin)
    {
      BuildReason buildReason = BuildReason.Manual;
      if (string.Equals("latest-with-shelveset", whatToBuild, StringComparison.OrdinalIgnoreCase))
        buildReason = !checkin.HasValue || !checkin.Value ? BuildReason.ValidateShelveset : BuildReason.CheckInShelveset;
      return buildReason;
    }

    private QueuePriority GetQueuePriority(string priority)
    {
      QueuePriority result = QueuePriority.Normal;
      System.Enum.TryParse<QueuePriority>(priority, true, out result);
      return result;
    }

    private DateTime GetMinFinishTime(string dateFilter)
    {
      TimeZoneInfo timeZone = this.TfsRequestContext.GetTimeZone();
      DateTime minFinishTime = BuildHelpers.GetMinFinishTime(dateFilter, timeZone.ConvertToLocal(DateTime.UtcNow));
      return minFinishTime != DateTime.MinValue ? timeZone.ConvertToUtc(minFinishTime) : minFinishTime;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514040, 514050)]
    [ValidateInput(false)]
    public ActionResult CompletedBuilds(
      string definitionUri,
      string quality,
      string date,
      bool? my,
      string informationTypes)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      this.TfsRequestContext.Trace(514035, TraceLevel.Info, "WebAccess.Build", TfsTraceLayers.Controller, "Fetching Completed Builds with filters. definitionUri: {0} quality: {1} date: {2} my: {3} informationTypes: {4}", (object) definitionUri, (object) quality, (object) date, (object) (my.HasValue ? my : new bool?(false)), (object) informationTypes);
      List<JsObject> jsObjectList1 = new List<JsObject>();
      informationTypes = informationTypes ?? InformationTypes.CheckInOutcome;
      string str = this.ConstructBuildDefinitionPath(definitionUri);
      if (string.IsNullOrEmpty(str))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildDefinitionNotFound, (object) definitionUri));
      BuildDetailSpec buildDetailSpec = new BuildDetailSpec()
      {
        DefinitionFilter = (object) new BuildDefinitionSpec()
        {
          FullPath = str
        },
        DefinitionFilterType = DefinitionFilterType.DefinitionSpec,
        Quality = quality,
        RequestedFor = !my.HasValue || !my.Value ? (string) null : this.TfsRequestContext.AuthenticatedUserName,
        QueryOrder = BuildQueryOrder.FinishTimeDescending,
        QueryOptions = QueryOptions.Definitions | QueryOptions.Controllers | QueryOptions.BatchedRequests,
        Status = Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed | Microsoft.TeamFoundation.Build.Server.BuildStatus.Stopped,
        MaxBuildsPerDefinition = 1000,
        MinFinishTime = this.GetMinFinishTime(date)
      };
      buildDetailSpec.BuildNumber = "*";
      buildDetailSpec.InformationTypes.AddRange((IEnumerable<string>) informationTypes.Split(','));
      using (TeamFoundationDataReader foundationDataReader = this.XamlBuildService.QueryBuilds(this.TfsRequestContext, (IList<BuildDetailSpec>) new BuildDetailSpec[1]
      {
        buildDetailSpec
      }, new Guid()))
      {
        foreach (BuildQueryResult current in foundationDataReader.CurrentEnumerable<BuildQueryResult>())
        {
          Dictionary<int, QueuedBuild> dictionary = current.QueuedBuilds.ToDictionary<QueuedBuild, int>((Func<QueuedBuild, int>) (qb => qb.Id));
          foreach (BuildDetail build in current.Builds)
          {
            List<JsObject> jsObjectList2 = new List<JsObject>();
            foreach (int queueId in build.QueueIds)
            {
              QueuedBuild queuedBuild;
              if (dictionary.TryGetValue(queueId, out queuedBuild))
              {
                List<JsObject> jsObjectList3 = jsObjectList2;
                JsObject jsObject = new JsObject();
                jsObject.Add("id", (object) queuedBuild.Id);
                jsObject.Add("requestedBy", (object) queuedBuild.RequestedForDisplayName);
                jsObjectList3.Add(jsObject);
              }
            }
            List<JsObject> jsObjectList4 = jsObjectList1;
            JsObject jsObject1 = new JsObject();
            jsObject1.Add("name", (object) build.BuildNumber);
            jsObject1.Add("uri", (object) build.Uri);
            jsObject1.Add("definition", (object) BuildDetailModel.GetDefinitionName(build.Definition));
            jsObject1.Add("dropFolder", (object) new BuildExternalLinkModel(build.DropLocation, string.Empty).ToJson());
            jsObject1.Add(nameof (quality), (object) build.Quality);
            jsObject1.Add("status", (object) build.Status);
            jsObject1.Add("finished", (object) build.BuildFinished());
            jsObject1.Add(nameof (date), (object) build.FinishTime);
            jsObject1.Add("reason", (object) build.Reason);
            jsObject1.Add("retain", (object) build.KeepForever);
            jsObject1.Add("logLocation", (object) build.LogLocation);
            jsObject1.Add("requests", (object) jsObjectList2);
            jsObject1.Add("hasDiagnostics", (object) build.ContainerId.HasValue);
            jsObjectList4.Add(jsObject1);
          }
        }
        SecureJsonResult secureJsonResult = new SecureJsonResult();
        secureJsonResult.Data = (object) jsObjectList1;
        secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) secureJsonResult;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    public ActionResult DeployedBuilds(string definitionUri, string date, bool? my)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      this.TfsRequestContext.Trace(514035, TraceLevel.Info, "WebAccess.Build", TfsTraceLayers.Controller, "Fetching Deployed Builds with filters. definitionUri: {0} date: {1} my: {2}", (object) definitionUri, (object) date, (object) (my.HasValue ? my : new bool?(false)));
      string str = this.ConstructBuildDefinitionPath(definitionUri);
      if (string.IsNullOrEmpty(str))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildDefinitionNotFound, (object) definitionUri));
      List<BuildDeployment> source1 = this.TfsRequestContext.GetService<TeamFoundationDeploymentService>().QueryDeployments(this.TfsRequestContext, new BuildDeploymentSpec()
      {
        MaxDeployments = 1000,
        MinFinishTime = this.GetMinFinishTime(date),
        QueryOrder = BuildQueryOrder.FinishTimeDescending,
        RequestedFor = !my.HasValue || !my.Value ? (string) null : this.TfsRequestContext.AuthenticatedUserName,
        DefinitionPath = str
      });
      Guid[] array = source1.SelectMany<BuildDeployment, RequestedForDisplayInformation>((Func<BuildDeployment, IEnumerable<RequestedForDisplayInformation>>) (x => (IEnumerable<RequestedForDisplayInformation>) x.Deployment.RequestedFor)).Select<RequestedForDisplayInformation, Guid>((Func<RequestedForDisplayInformation, Guid>) (x => x.TeamFoundationId)).Distinct<Guid>().ToArray<Guid>();
      TeamFoundationIdentity[] source2 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.TfsRequestContext, array);
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      JsObject jsObject = new JsObject();
      jsObject.Add("builds", (object) source1.Select<BuildDeployment, JsObject>((Func<BuildDeployment, JsObject>) (x => x.ToJson())));
      jsObject.Add("identities", (object) ((IEnumerable<TeamFoundationIdentity>) source2).ToDictionary<TeamFoundationIdentity, string, string>((Func<TeamFoundationIdentity, string>) (x => x.TeamFoundationId.ToString()), (Func<TeamFoundationIdentity, string>) (y => y.DisplayName)));
      secureJsonResult.Data = (object) jsObject;
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514060, 514070)]
    public JsonResult BuildData(bool? allProjects)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      List<JsObject> jsObjectList1 = new List<JsObject>();
      Dictionary<string, JsObject> dictionary = new Dictionary<string, JsObject>();
      List<JsObject> jsObjectList2 = new List<JsObject>();
      BuildDefinitionSpec spec = new BuildDefinitionSpec();
      string str = !allProjects.HasValue || !allProjects.Value ? BuildPath.Combine(this.TfsWebContext.ProjectContext == null ? (string) null : this.TfsWebContext.ProjectContext.Name, "*") : "*/*";
      spec.FullPath = str;
      spec.Options = QueryOptions.Process;
      int num = 0;
      foreach (BuildDefinition buildDefinition in (IEnumerable<BuildDefinition>) this.XamlBuildService.QueryBuildDefinitions(this.TfsRequestContext, spec).Definitions.OrderBy<BuildDefinition, string>((Func<BuildDefinition, string>) (def => def.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase))
      {
        string msBuildArguments = this.GetMSBuildArguments(buildDefinition.ProcessParameters);
        BuildDefinitionSourceProvider definitionSourceProvider = buildDefinition.SourceProviders.FirstOrDefault<BuildDefinitionSourceProvider>();
        ProcessTemplate process = buildDefinition.Process;
        JsObject jsObject1 = new JsObject();
        jsObject1.Add("name", (object) buildDefinition.Name);
        jsObject1.Add("uri", (object) buildDefinition.Uri);
        jsObject1.Add("status", (object) buildDefinition.QueueStatus.ToString());
        jsObject1.Add("dropLocation", (object) buildDefinition.DefaultDropLocation);
        jsObject1.Add("controllerUri", (object) buildDefinition.BuildControllerUri);
        jsObject1.Add("sourceProviderName", definitionSourceProvider != null ? (object) definitionSourceProvider.Name : (object) string.Empty);
        jsObject1.Add("supportedReasons", (object) (process != null ? (int) process.SupportedReasons : 511));
        jsObject1.Add("fullPath", (object) buildDefinition.FullPath);
        jsObject1.Add("msbuildArgs", (object) msBuildArguments);
        jsObject1.Add("msbuildArgsOriginal", (object) msBuildArguments);
        JsObject jsObject2 = jsObject1;
        jsObjectList1.Add(jsObject2);
        dictionary[buildDefinition.Uri] = jsObject2;
        if (++num > 10000)
          break;
      }
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController controller in this.XamlBuildResourceService.QueryBuildControllers(this.TfsRequestContext, new BuildControllerSpec()
      {
        IncludeAgents = false,
        Name = "*",
        ServiceHostName = "*"
      }).Controllers)
      {
        List<JsObject> jsObjectList3 = jsObjectList2;
        JsObject jsObject = new JsObject();
        jsObject.Add("name", (object) controller.Name);
        jsObject.Add("uri", (object) controller.Uri);
        jsObjectList3.Add(jsObject);
      }
      using (TeamFoundationDataReader foundationDataReader = this.XamlBuildService.QueryBuilds(this.TfsRequestContext, (IList<BuildDetailSpec>) new BuildDetailSpec[1]
      {
        new BuildDetailSpec()
        {
          DefinitionFilter = (object) new BuildDefinitionSpec()
          {
            FullPath = str
          },
          DefinitionFilterType = DefinitionFilterType.DefinitionSpec,
          QueryOrder = BuildQueryOrder.FinishTimeDescending,
          QueryOptions = QueryOptions.Definitions,
          Status = Microsoft.TeamFoundation.Build.Server.BuildStatus.All,
          MaxBuildsPerDefinition = 3,
          MinFinishTime = DateTime.MinValue,
          BuildNumber = "*"
        }
      }, new Guid()))
      {
        foreach (BuildQueryResult current in foundationDataReader.CurrentEnumerable<BuildQueryResult>())
        {
          foreach (BuildDetail build in current.Builds)
          {
            JsObject jsObject3;
            if ((!build.BuildFinished() || !(build.FinishTime == DateTime.MinValue)) && (build.BuildFinished() || !(build.StartTime == DateTime.MinValue)) && dictionary.TryGetValue(build.BuildDefinitionUri, out jsObject3) && !jsObject3.TryGetValue("lastBuild", out object _))
            {
              JsObject jsObject4 = jsObject3;
              JsObject jsObject5 = new JsObject();
              jsObject5.Add("status", (object) (int) build.Status);
              jsObject5.Add("finished", (object) build.BuildFinished());
              jsObject5.Add("startTime", (object) build.StartTime);
              jsObject5.Add("finishTime", (object) build.FinishTime);
              jsObject4["lastBuild"] = (object) jsObject5;
            }
          }
        }
      }
      JsObject data = new JsObject();
      data.Add("definitions", (object) jsObjectList1);
      data.Add("controllers", (object) jsObjectList2);
      return this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    private string GetMSBuildArguments(string processParameters)
    {
      if (string.IsNullOrEmpty(processParameters))
        return string.Empty;
      string empty = string.Empty;
      bool flag = false;
      MatchCollection matchCollection1 = new Regex(string.Format("{0}(.*?){1}", (object) "<mtbc:BuildParameter x:Key=\"AdvancedBuildSettings\">", (object) "</mtbc:BuildParameter>")).Matches(processParameters);
      if (matchCollection1.Count == 1 && matchCollection1[0].Groups.Count == 2)
      {
        string json = matchCollection1[0].Groups[1].Value;
        if (!string.IsNullOrEmpty(json))
        {
          try
          {
            empty = new BuildParameter(json).GetValue<string>("MSBuildArguments");
            flag = true;
          }
          catch (BuildParameterException ex)
          {
          }
        }
      }
      if (!flag)
      {
        MatchCollection matchCollection2 = new Regex(string.Format("{0}(.*?){1}", (object) "<x:String x:Key=\"MSBuildArguments\".*?>", (object) "</x:String>")).Matches(processParameters);
        if (matchCollection2.Count == 1 && matchCollection2[0].Groups.Count == 2)
          empty = matchCollection2[0].Groups[1].Value;
      }
      return empty;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514080, 514090)]
    public ActionResult QueuedBuilds(
      string definitionUri,
      QueueStatus status,
      string buildController,
      bool? my)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      this.TfsRequestContext.Trace(514060, TraceLevel.Info, "WebAccess.Build", TfsTraceLayers.Controller, "Fetching Queued Builds with filters. definitionUri: {0} status: {1} buildController: {2} my: {3}", (object) definitionUri, (object) status, (object) buildController, (object) (my.HasValue ? my : new bool?(false)));
      string str = this.ConstructBuildDefinitionPath(definitionUri);
      if (string.IsNullOrEmpty(str))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildDefinitionNotFound, (object) definitionUri));
      buildController = string.IsNullOrEmpty(buildController) ? "*" : buildController;
      using (TeamFoundationDataReader foundationDataReader = this.XamlBuildService.QueryQueuedBuilds(this.TfsRequestContext, (IList<BuildQueueSpec>) new List<BuildQueueSpec>()
      {
        new BuildQueueSpec()
        {
          DefinitionFilter = (object) new BuildDefinitionSpec()
          {
            FullPath = str
          },
          DefinitionFilterType = DefinitionFilterType.DefinitionSpec,
          Status = status,
          RequestedFor = !my.HasValue || !my.Value ? (string) null : this.TfsRequestContext.AuthenticatedUserName,
          ControllerSpec = new BuildControllerSpec(buildController, "*", true)
        }
      }, new Guid()))
      {
        List<QueuedBuildModel> data = new List<QueuedBuildModel>();
        foreach (BuildQueueQueryResult current in foundationDataReader.CurrentEnumerable<BuildQueueQueryResult>())
        {
          foreach (QueuedBuild queuedBuild in current.QueuedBuilds)
            data.Add(new QueuedBuildModel(queuedBuild, this.GetControllerForBuild(current.Controllers, queuedBuild.BuildControllerUri)));
        }
        DataContractJsonResult contractJsonResult = new DataContractJsonResult((object) data);
        contractJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) contractJsonResult;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514100, 514110)]
    public ActionResult TestImpact(string buildUri)
    {
      this.Trace(514101, TraceLevel.Info, "Fetching TestImpact data. buildUri: {0}", (object) buildUri);
      return (ActionResult) this.Json((object) new BuildTestImpactModel(TestImpactServer.QueryImpactedTests(this.TfsRequestContext, this.TfsWebContext.ProjectContext.Name, buildUri)).ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514120, 514130)]
    [ValidateInput(false)]
    public ActionResult UpdateBuildQualities(List<string> itemsAdded, List<string> itemsRemoved)
    {
      if (itemsAdded != null && itemsAdded.Count > 0)
        this.XamlBuildService.AddBuildQualities(this.TfsRequestContext, this.TfsWebContext.ProjectContext.Name, (IList<string>) itemsAdded);
      if (itemsRemoved != null && itemsRemoved.Count > 0)
        this.XamlBuildService.DeleteBuildQualities(this.TfsRequestContext, this.TfsWebContext.ProjectContext.Name, (IList<string>) itemsRemoved);
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514140, 514150)]
    public ActionResult Build(string uri, bool? includeTestRuns, bool? includeCoverage)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      this.Trace(514141, TraceLevel.Info, "Fetching Build data. buildUri: {0}", (object) uri);
      try
      {
        BuildDetailModel buildDetailModel = (BuildDetailModel) null;
        BuildDetail build = (BuildDetail) null;
        TeamFoundationBuildService xamlBuildService = this.XamlBuildService;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<string> uris = new List<string>();
        uris.Add(uri);
        string[] informationTypes = Microsoft.TeamFoundation.Build.Common.BuildConstants.AllInformationTypes;
        Guid projectId = new Guid();
        using (TeamFoundationDataReader foundationDataReader = xamlBuildService.QueryBuildsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) informationTypes, QueryOptions.All, QueryDeletedOption.ExcludeDeleted, projectId, false))
        {
          BuildQueryResult buildResult = foundationDataReader.Current<BuildQueryResult>();
          if (buildResult.Builds.FirstOrDefault<BuildDetail>() == null)
            throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildResources.BuildNotFound, (object) uri));
          buildDetailModel = new BuildDetailModel(buildResult, this.TfsWebContext.CurrentUserDisplayName, false, this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment);
          if (includeTestRuns.HasValue && includeTestRuns.Value)
            buildDetailModel.PopulateTestRuns(this.TfsRequestContext);
          if (includeCoverage.HasValue && includeCoverage.Value)
            buildDetailModel.PopulateCoverageData(this.TfsRequestContext);
          build = buildResult.Builds.Current;
          string logsFolderUrl = string.Empty;
          if (BuildContainerPath.IsServerPath(build.LogLocation))
            logsFolderUrl = this.ConvertToHttpTarget(build.LogLocation);
          buildDetailModel.FixIntermediateLogNodes(logsFolderUrl);
        }
        JsObject jsonObject = buildDetailModel.ToJson();
        RetryableJsonResult retryableJsonResult = new RetryableJsonResult((Func<object>) (() =>
        {
          jsonObject.Remove("information");
          jsonObject["hasDiagnostics"] = (object) false;
          jsonObject["jsonLimitExceeded"] = (object) true;
          jsonObject["hasLogs"] = (object) (this.GetContainerItems(build, "/logs").Count > 0);
          return (object) jsonObject;
        }));
        retryableJsonResult.Data = (object) jsonObject;
        retryableJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        return (ActionResult) retryableJsonResult;
      }
      catch (SqlException ex)
      {
        this.TfsRequestContext.TraceException(514142, "WebAccess.Build", TfsTraceLayers.Controller, (Exception) ex);
        return (ActionResult) new EmptyResult();
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514160, 514170)]
    public ActionResult BuildHistory(string definitionUri, string buildUri, int? count)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      this.Trace(514161, TraceLevel.Info, "Fetching BuildHistory. definitionUri: {0} buildUri: {1}", (object) definitionUri, (object) buildUri);
      List<BuildDetail> source1 = new List<BuildDetail>();
      List<BuildDetail> source2 = (List<BuildDetail>) null;
      List<BuildDetail> source3 = (List<BuildDetail>) null;
      int num1 = 10;
      if (count.HasValue)
        num1 = count.Value;
      if (!string.IsNullOrWhiteSpace(buildUri))
      {
        TeamFoundationBuildService xamlBuildService = this.XamlBuildService;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        List<string> uris = new List<string>();
        uris.Add(buildUri);
        Guid projectId = new Guid();
        using (TeamFoundationDataReader foundationDataReader = xamlBuildService.QueryBuildsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.None, QueryDeletedOption.ExcludeDeleted, projectId, false))
        {
          BuildQueryResult buildQueryResult = foundationDataReader.Current<BuildQueryResult>();
          if (buildQueryResult.Builds.FirstOrDefault<BuildDetail>() != null)
            source1.Add(buildQueryResult.Builds.Current);
        }
      }
      DateTime dateTime = DateTime.UtcNow;
      if (source1.Any<BuildDetail>())
      {
        BuildDetail buildDetail = source1.First<BuildDetail>();
        if (buildDetail.FinishTime > DateTime.MinValue)
          dateTime = buildDetail.FinishTime;
      }
      string str = this.ConstructBuildDefinitionPath(definitionUri);
      if (string.IsNullOrEmpty(str))
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildDefinitionNotFound, (object) definitionUri));
      BuildDetailSpec buildDetailSpec1 = new BuildDetailSpec()
      {
        BuildNumber = "*",
        DefinitionFilter = (object) new BuildDefinitionSpec()
        {
          FullPath = str
        },
        DefinitionFilterType = DefinitionFilterType.DefinitionSpec,
        QueryOrder = BuildQueryOrder.FinishTimeDescending,
        QueryOptions = QueryOptions.None,
        Status = Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed,
        MaxBuildsPerDefinition = num1,
        MaxFinishTime = dateTime.AddSeconds(-1.0)
      };
      BuildDetailSpec buildDetailSpec2 = new BuildDetailSpec()
      {
        BuildNumber = "*",
        DefinitionFilter = (object) new BuildDefinitionSpec()
        {
          FullPath = str
        },
        DefinitionFilterType = DefinitionFilterType.DefinitionSpec,
        QueryOrder = BuildQueryOrder.FinishTimeAscending,
        QueryOptions = QueryOptions.None,
        Status = Microsoft.TeamFoundation.Build.Server.BuildStatus.Succeeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.PartiallySucceeded | Microsoft.TeamFoundation.Build.Server.BuildStatus.Failed,
        MaxBuildsPerDefinition = num1,
        MinFinishTime = dateTime.AddSeconds(1.0)
      };
      using (TeamFoundationDataReader foundationDataReader = this.XamlBuildService.QueryBuilds(this.TfsRequestContext, (IList<BuildDetailSpec>) new BuildDetailSpec[2]
      {
        buildDetailSpec1,
        buildDetailSpec2
      }, new Guid()))
      {
        foreach (BuildQueryResult current in foundationDataReader.CurrentEnumerable<BuildQueryResult>())
        {
          if (source2 == null)
            source2 = new List<BuildDetail>((IEnumerable<BuildDetail>) current.Builds.ToArray<BuildDetail>());
          else
            source3 = new List<BuildDetail>((IEnumerable<BuildDetail>) current.Builds.ToArray<BuildDetail>());
        }
      }
      int num2 = num1 / 2;
      int count1 = source2.Count;
      int count2 = source1.Count;
      int count3 = source3.Count;
      while (count1 + count2 + count3 > num1)
      {
        if (count3 > num2)
          --count3;
        else
          --count1;
      }
      List<BuildDetail> buildDetailList = new List<BuildDetail>();
      buildDetailList.AddRange(source2.Take<BuildDetail>(count1).Reverse<BuildDetail>());
      buildDetailList.AddRange(source1.Take<BuildDetail>(count2));
      buildDetailList.AddRange(source3.Take<BuildDetail>(count3));
      return (ActionResult) this.Json((object) new BuildHistogramModel((IEnumerable<BuildDetail>) buildDetailList.ToArray()).ToJson(), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514180, 514190)]
    public ActionResult Cancel(int id)
    {
      this.Trace(514125, TraceLevel.Info, "Canceling Build. id: {0}", (object) id);
      this.XamlBuildService.CancelBuilds(this.TfsRequestContext, new int[1]
      {
        id
      }, new Guid());
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514200, 514210)]
    public ActionResult Postpone(int id)
    {
      this.Trace(514201, TraceLevel.Info, "Postpone Build. id: {0}", (object) id);
      this.XamlBuildService.UpdateQueuedBuilds(this.TfsRequestContext, (IList<QueuedBuildUpdateOptions>) new QueuedBuildUpdateOptions[1]
      {
        new QueuedBuildUpdateOptions()
        {
          QueueId = id,
          Postponed = true,
          Fields = QueuedBuildUpdate.Postponed
        }
      }, new Guid());
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514220, 514230)]
    public ActionResult Resume(int id)
    {
      this.Trace(514221, TraceLevel.Info, "Resume Build. id: {0}", (object) id);
      this.XamlBuildService.UpdateQueuedBuilds(this.TfsRequestContext, (IList<QueuedBuildUpdateOptions>) new QueuedBuildUpdateOptions[1]
      {
        new QueuedBuildUpdateOptions()
        {
          QueueId = id,
          Postponed = false,
          Fields = QueuedBuildUpdate.Postponed
        }
      }, new Guid());
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514240, 514250)]
    public ActionResult Stop(string uri)
    {
      this.Trace(514241, TraceLevel.Info, "Stop Build. uri: {0}", (object) uri);
      this.XamlBuildService.StopBuilds(this.TfsRequestContext, (IList<string>) new string[1]
      {
        uri
      }, new Guid());
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514260, 514270)]
    public ActionResult StartNow(int id)
    {
      this.Trace(514261, TraceLevel.Info, "StartNow Build. id: {0}", (object) id);
      this.XamlBuildService.StartQueuedBuildsNow(this.TfsRequestContext, new int[1]
      {
        id
      }, new Guid());
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514280, 514290)]
    public ActionResult Retain(string uri, bool? retain)
    {
      this.Trace(514281, TraceLevel.Info, "Retain Build. uri: {0}  retain: {1}", (object) uri, (object) retain.GetValueOrDefault());
      this.XamlBuildService.UpdateBuilds(this.TfsRequestContext, (IList<BuildUpdateOptions>) new BuildUpdateOptions[1]
      {
        new BuildUpdateOptions()
        {
          Uri = uri,
          KeepForever = ((int) retain ?? 1) != 0,
          Fields = BuildUpdate.KeepForever
        }
      }, new Guid());
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514300, 514310)]
    public ActionResult Delete(string uri, int? deleteOptions)
    {
      if (!deleteOptions.HasValue)
        deleteOptions = new int?(0);
      this.Trace(514301, TraceLevel.Info, "Delete Build. uri: {0}  deleteOptions: {1}", (object) uri, (object) deleteOptions);
      this.XamlBuildService.DeleteBuilds(this.TfsRequestContext, (IList<string>) new string[1]
      {
        uri
      }, (DeleteOptions) deleteOptions.Value, true, new Guid(), false);
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [ValidateInput(false)]
    [TfsTraceFilter(514320, 514330)]
    public ActionResult BuildQuality(string uri, string quality)
    {
      this.Trace(514321, TraceLevel.Info, "Set Build Quality.  uri: {0}  quality: {1}", (object) uri, (object) quality);
      this.XamlBuildService.UpdateBuilds(this.TfsRequestContext, (IList<BuildUpdateOptions>) new BuildUpdateOptions[1]
      {
        new BuildUpdateOptions()
        {
          Uri = uri,
          Quality = quality ?? string.Empty,
          Fields = BuildUpdate.Quality
        }
      }, new Guid());
      return (ActionResult) new EmptyResult();
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514340, 514350)]
    public ActionResult RequeueBuild(string uri)
    {
      this.Trace(514341, TraceLevel.Info, "Redeploy Build. uri: {0}", (object) uri);
      this.TfsRequestContext.GetService<TeamFoundationDeploymentService>().Redeploy(this.TfsRequestContext, uri);
      return (ActionResult) new EmptyResult();
    }

    private string GetControllerForBuild(List<Microsoft.TeamFoundation.Build.Server.BuildController> buildControllers, string buildUri)
    {
      foreach (Microsoft.TeamFoundation.Build.Server.BuildController buildController in buildControllers)
      {
        if (buildController.Uri.Equals(buildUri))
          return buildController.Name;
      }
      return string.Empty;
    }

    private string ConstructBuildDefinitionPath(string definitionUri)
    {
      string str = string.Empty;
      if (!string.IsNullOrWhiteSpace(definitionUri))
      {
        TeamFoundationBuildService xamlBuildService = this.XamlBuildService;
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string[] uris = new string[1]{ definitionUri };
        Guid projectId = new Guid();
        foreach (BuildDefinition definition in xamlBuildService.QueryBuildDefinitionsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.Definitions, projectId).Definitions)
        {
          if (definition != null)
            str = definition.FullPath;
        }
      }
      else
        str = BuildPath.Combine(this.TfsWebContext.ProjectContext == null ? (string) null : this.TfsWebContext.ProjectContext.Name, "*");
      return str;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514140, 514150)]
    public ActionResult LogXml()
    {
      string str;
      try
      {
        string buildUri = this.Request.Params["buildUri"];
        string path = this.Request.Params["logLocation"];
        string physicalLocation = StaticResources.Versioned.Content.GetPhysicalLocation("BuildLog.xsl", this.TfsRequestContext);
        XslCompiledTransform compiledTransform = new XslCompiledTransform();
        compiledTransform.Load(physicalLocation, XsltSettings.Default, (XmlResolver) new XmlUrlResolver());
        StringBuilder stringBuilder = new StringBuilder();
        string buildNumber;
        Stream singleItemStream = this.GetSingleItemStream(buildUri, path, out buildNumber);
        if (singleItemStream == null)
          return (ActionResult) this.HttpStatusCodeWithMessage(HttpStatusCode.NotFound, string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildItemNotFound, (object) buildNumber, (object) buildUri, (object) path));
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StreamReader input1 = new StreamReader(singleItemStream))
        {
          using (XmlReader input2 = XmlReader.Create((TextReader) input1, settings))
          {
            StringBuilder output = stringBuilder;
            using (XmlWriter results = XmlWriter.Create(output, new XmlWriterSettings()
            {
              ConformanceLevel = ConformanceLevel.Auto
            }))
              compiledTransform.Transform(input2, results);
          }
        }
        str = stringBuilder.ToString();
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(514360, "WebAccess.Build", TfsTraceLayers.Controller, ex);
        throw;
      }
      if (str.Length + 100 > SecureJsonResult.DefaultMaxJsonLength)
        throw new TeamFoundationServiceException(BuildServerResources.BuildJsonLimitExceeded);
      JsObject data = new JsObject();
      data.Add("transformedLogXml", (object) str);
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Project | NavigationContextLevels.Team)]
    [TfsTraceFilter(514360, 514370)]
    public FileResult ItemContent()
    {
      try
      {
        this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(60.0);
        string buildUri = this.Request.Params["buildUri"];
        string path = this.Request.Params["path"];
        this.Response.BufferOutput = false;
        return this.GetFileResult(buildUri, path);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(514360, "WebAccess.Build", TfsTraceLayers.Controller, ex);
        throw;
      }
    }

    [ValidateInput(false)]
    [AcceptVerbs(HttpVerbs.Post)]
    [TfsTraceFilter(514370, 514380)]
    public void SendMailAsync([ModelBinder(typeof (JsonModelBinder))] MailMessage message, int buildId)
    {
      ArgumentUtility.CheckForNonnegativeInt(buildId, nameof (buildId));
      string str1 = "";
      if (message.Body != null)
      {
        int length = message.Body.Length;
      }
      string str2 = str1 + this.ConstructNotesText(message.Body) + this.BuildService.GetReportGenerator(this.TfsRequestContext).GetBuildReport(this.TfsRequestContext, this.TfsWebContext.Project.Id, buildId);
      message.Body = str2;
      MailSender.BeginSendMail(message, this.TfsRequestContext, this.Request.RequestContext.TfsWebContext().IsHosted, this.AsyncManager);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [TfsTraceFilter(514380, 514382)]
    public ActionResult SendMailCompleted() => (ActionResult) this.Json((object) MailSender.SendMailCompleted(this.AsyncManager));

    private string ConstructNotesText(string body)
    {
      string str1 = SafeHtmlWrapper.MakeSafeWithHtmlEncode(body, true);
      int num = str1 == null ? 0 : (str1.Trim().Length > 0 ? 1 : 0);
      string emailWithNote = num != 0 ? BuildServerResources.EmailWithNote : "";
      string str2 = "";
      if (this.TfsWebContext.User != null)
        str2 += string.Format(BuildServerResources.EmailBuildPrependFormat, (object) this.TfsWebContext.User.Name, (object) this.TfsWebContext.User.Email, (object) emailWithNote);
      if (num != 0)
        str2 = str2 + "<br />" + string.Format(BuildServerResources.EmailNotes, (object) BuildServerResources.EmailBuildPrependNote, (object) str1);
      return str2;
    }

    private Stream GetSingleItemStream(string buildUri, string path, out string buildNumber)
    {
      Guid projectId;
      List<FileContainerItem> containerItems = this.GetContainerItems(buildUri, path, out buildNumber, out projectId);
      if (containerItems.Count == 0)
      {
        containerItems = this.GetContainerItems(buildUri, Uri.UnescapeDataString(path), out buildNumber, out projectId);
        if (containerItems.Count == 0)
        {
          this.TfsRequestContext.Trace(514383, TraceLevel.Error, "WebAccess.Build", TfsTraceLayers.Controller, "Build {0} ({1}) did not provide '{2}'.", (object) buildNumber, (object) buildUri, (object) path);
          return (Stream) null;
        }
      }
      else if (containerItems.Count > 1)
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildItemNotUnique, (object) path, (object) buildUri));
      FileContainerItem fileContainerItem = containerItems[0];
      return fileContainerItem.ItemType != ContainerItemType.Folder ? this.GetFileStream(fileContainerItem.FileId) : throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildItemNotAFile, (object) path, (object) buildUri));
    }

    private FileResult GetFileResult(string buildUri, string path)
    {
      if (string.IsNullOrEmpty(path))
        path = string.Empty;
      string buildName;
      Guid projectId;
      List<FileContainerItem> containerItems = this.GetContainerItems(buildUri, path, out buildName, out projectId);
      if (containerItems.Count == 0)
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildItemNotFound, (object) buildName, (object) buildUri, (object) path));
      if (containerItems.Count == 1 && containerItems[0].ItemType == ContainerItemType.File)
      {
        string contentType = "application/octet-stream";
        return (FileResult) this.File(this.GetFileStream(containerItems[0].FileId), contentType, Path.GetFileName(path.Replace("/", "\\")));
      }
      string existingZipString = string.Empty;
      if (path.Equals("/drop", StringComparison.OrdinalIgnoreCase))
        existingZipString = "/drop/" + buildName + ".zip";
      else if (path.Equals("/logs", StringComparison.OrdinalIgnoreCase))
        existingZipString = "/logs/" + buildName + "_logs.zip";
      FileContainerItem fileContainerItem = containerItems.FirstOrDefault<FileContainerItem>((Func<FileContainerItem, bool>) (x => x.Path.Equals(existingZipString, StringComparison.OrdinalIgnoreCase)));
      if (fileContainerItem != null)
      {
        string contentType = "application/zip";
        return (FileResult) this.File(this.GetFileStream(fileContainerItem.FileId), contentType, Path.GetFileName(fileContainerItem.Path.Replace("/", "\\")));
      }
      string str = path.Trim('/');
      int num = str.LastIndexOf('/');
      if (num > 0)
        str = str.Substring(num + 1);
      if (!string.IsNullOrEmpty(str))
        str = "_" + str;
      ApiBuildController.ZipFileContainerStreamResult fileResult = new ApiBuildController.ZipFileContainerStreamResult(this.TfsRequestContext, containerItems[0].ContainerId, path, projectId);
      fileResult.FileDownloadName = buildName + str + ".zip";
      return (FileResult) fileResult;
    }

    private List<FileContainerItem> GetContainerItems(
      string buildUri,
      string path,
      out string buildName,
      out Guid projectId)
    {
      BuildDetail build = (BuildDetail) null;
      TeamFoundationBuildService xamlBuildService = this.XamlBuildService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<string> uris = new List<string>();
      uris.Add(buildUri);
      Guid projectId1 = new Guid();
      using (TeamFoundationDataReader foundationDataReader = xamlBuildService.QueryBuildsByUri(tfsRequestContext, (IList<string>) uris, (IList<string>) null, QueryOptions.None, QueryDeletedOption.ExcludeDeleted, projectId1, false))
      {
        build = foundationDataReader.Current<BuildQueryResult>().Builds.FirstOrDefault<BuildDetail>();
        if (build == null)
          throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildResources.BuildNotFound, (object) buildUri));
      }
      projectId = this.ProjectService.GetProjectId(this.TfsRequestContext, build.TeamProject);
      buildName = build.BuildNumber;
      return this.GetContainerItems(build, path);
    }

    private List<FileContainerItem> GetContainerItems(BuildDetail build, string path)
    {
      if (!build.ContainerId.HasValue)
        throw new TeamFoundationServiceException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, BuildServerResources.BuildContainerNotFound, (object) build.Uri));
      long containerId = build.ContainerId.Value;
      Guid projectId = this.ProjectService.GetProjectId(this.TfsRequestContext, build.TeamProject);
      return this.TfsRequestContext.GetService<TeamFoundationFileContainerService>().QueryItems(this.TfsRequestContext, containerId, path, projectId, false, false);
    }

    private Stream GetFileStream(int fileId) => this.TfsRequestContext.GetService<TeamFoundationFileService>().RetrieveFile(this.TfsRequestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _);

    private string ConvertToHttpTarget(string targetDirectory)
    {
      ILocationService service = this.TfsRequestContext.GetService<ILocationService>();
      string str = service.LocationForAccessMapping(this.TfsRequestContext, "FileContainersResource", FrameworkServiceIdentifiers.FileContainers, service.DetermineAccessMapping(this.TfsRequestContext));
      long containerId;
      string itemPath;
      BuildContainerPath.GetContainerIdAndPath(targetDirectory, out containerId, out itemPath);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}?itemPath={2}", (object) str, (object) containerId, (object) itemPath.TrimStart('/'));
    }

    private class ZipFileContainerStreamResult : FileResult
    {
      private IVssRequestContext m_requestContext;
      private long m_containerId;
      private string m_path;
      private Guid m_DataspaceIdentifier;

      public ZipFileContainerStreamResult(
        IVssRequestContext requestContext,
        long containerId,
        string path,
        Guid dataspaceIdentifier)
        : this()
      {
        this.m_requestContext = requestContext;
        this.m_containerId = containerId;
        this.m_path = path;
        this.m_DataspaceIdentifier = dataspaceIdentifier;
      }

      protected ZipFileContainerStreamResult()
        : base("application/zip")
      {
      }

      protected override void WriteFile(HttpResponseBase response)
      {
        response.BufferOutput = false;
        this.m_requestContext.GetService<TeamFoundationFileContainerService>().WriteContents(this.m_requestContext, this.m_containerId, this.m_path, response.OutputStream, this.m_DataspaceIdentifier, false, true);
      }
    }
  }
}
