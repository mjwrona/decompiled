// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.CoverageStatusBadgeController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "status", ResourceVersion = 1)]
  public class CoverageStatusBadgeController : TfsApiController
  {
    internal CodeCoverageHelper m_codeCoverageHelper;
    private TestManagementRequestContext m_testManagementRequestContext;
    private string _statusBadgeNoBuilds = TcmResources.CoverageBadgeNoBuilds();
    private string _statusBadgeNoCoverageText = TcmResources.CoverageBadgeNoCoverage();
    private string _statusBadgeNoDefinition = TcmResources.CoverageBadgeNoDefinition();

    [HttpGet]
    [ActionName("GetCoverageStatusBadge")]
    [ClientResponseType(typeof (string), null, null)]
    [RequestRestrictions(RequiredAuthentication.Anonymous, false, true, AuthenticationMechanisms.All, "", UserAgentFilterType.None, null)]
    [ClientLocationId("73B7C9D8-DEFB-4B60-B3D6-2162D60D6B13")]
    [FeatureEnabled("TestManagement.Server.CoverageStatusBadge")]
    public HttpResponseMessage GetCoverageStatusBadge(
      string definition,
      string branchName = null,
      string label = null)
    {
      ArgumentUtility.CheckForNull<string>(definition, nameof (definition));
      Guid projectId;
      if (!this.TryGetProjectId(out projectId) || projectId == new Guid())
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, TcmResources.CoverageBadgeNoDefinition());
      if (string.IsNullOrWhiteSpace(label))
        label = TcmResources.CoverageBadgeLeftText();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      bool flag = this.CodeCovHelper.IsAnonymousAccessAllowed(this.TfsRequestContext, projectId);
      if (!flag)
        return this.Request.CreateErrorResponse(HttpStatusCode.NotFound, TcmResources.CoverageBadgeNoDefinition());
      IVssRequestContext context = flag ? vssRequestContext : this.TfsRequestContext;
      string str1 = (string) null;
      int result1 = 0;
      BuildDefinition buildDefinition = (BuildDefinition) null;
      BuildHttpClient client = context.GetClient<BuildHttpClient>();
      try
      {
        if (int.TryParse(definition, out result1))
        {
          buildDefinition = client.GetDefinitionAsync(projectId, result1, new int?(), new DateTime?(), (IEnumerable<string>) null, new bool?(), (object) null, new CancellationToken()).Result;
          str1 = buildDefinition.Repository.DefaultBranch;
        }
        else
        {
          buildDefinition = client.GetFullDefinitionsAsync(projectId, definition, (string) null, (string) null, new DefinitionQueryOrder?(), new int?(), (string) null, new DateTime?(), (IEnumerable<int>) null, (string) null, new DateTime?(), new DateTime?(), new bool?(), new Guid?(), new int?(), (string) null, (object) null, new CancellationToken()).Result.FirstOrDefault<BuildDefinition>();
          result1 = buildDefinition.Id;
          str1 = buildDefinition.Repository.DefaultBranch;
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "Error occurred in getting build definition; Definition = {0}", (object) definition, (object) ex);
        if (buildDefinition == null)
          return CoverageStatusBadgeHelper.GenerateSvg(this.TfsRequestContext, CoverageResult.NoDefinition, label, this._statusBadgeNoDefinition, (object) buildDefinition);
      }
      if (string.IsNullOrWhiteSpace(branchName))
        branchName = str1;
      else if (!branchName.Contains("refs/heads/") && CoverageStatusBadgeHelper.GitRepoTypes.Contains(buildDefinition.Repository.Type))
        branchName = "refs/heads/" + branchName;
      List<int> intList = new List<int>() { result1 };
      BuildHttpClient buildHttpClient = client;
      Guid project = projectId;
      List<int> definitions = intList;
      BuildResult? nullable = new BuildResult?(BuildResult.Succeeded);
      string str2 = branchName;
      DateTime? minTime = new DateTime?();
      DateTime? maxTime = new DateTime?();
      BuildReason? reasonFilter = new BuildReason?();
      BuildStatus? statusFilter = new BuildStatus?();
      BuildResult? resultFilter = nullable;
      int? top = new int?();
      int? maxBuildsPerDefinition = new int?();
      QueryDeletedOption? deletedFilter = new QueryDeletedOption?();
      BuildQueryOrder? queryOrder = new BuildQueryOrder?();
      string branchName1 = str2;
      CancellationToken cancellationToken = new CancellationToken();
      List<Microsoft.TeamFoundation.Build.WebApi.Build> result2 = buildHttpClient.GetBuildsAsync(project, (IEnumerable<int>) definitions, (IEnumerable<int>) null, (string) null, minTime, maxTime, (string) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, (IEnumerable<string>) null, top, (string) null, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName1, (IEnumerable<int>) null, (string) null, (string) null, (object) null, cancellationToken).Result;
      Microsoft.TeamFoundation.Build.WebApi.Build build = result2 != null ? result2.FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>() : (Microsoft.TeamFoundation.Build.WebApi.Build) null;
      if (build == null)
        return CoverageStatusBadgeHelper.GenerateSvg(this.TfsRequestContext, CoverageResult.NoBuild, label, this._statusBadgeNoBuilds, (object) build);
      Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary = this.CodeCovHelper.GetCodeCoverageSummary(projectId.ToString(), build.Id, -1);
      CodeCoverageStatistics coverageStatistics1;
      if (codeCoverageSummary == null)
      {
        coverageStatistics1 = (CodeCoverageStatistics) null;
      }
      else
      {
        IList<CodeCoverageData> coverageData = codeCoverageSummary.CoverageData;
        if (coverageData == null)
        {
          coverageStatistics1 = (CodeCoverageStatistics) null;
        }
        else
        {
          CodeCoverageData codeCoverageData = coverageData.FirstOrDefault<CodeCoverageData>();
          if (codeCoverageData == null)
          {
            coverageStatistics1 = (CodeCoverageStatistics) null;
          }
          else
          {
            IList<CodeCoverageStatistics> coverageStats = codeCoverageData.CoverageStats;
            coverageStatistics1 = coverageStats != null ? coverageStats.Where<CodeCoverageStatistics>((Func<CodeCoverageStatistics, bool>) (x => x.Label.Equals("Lines", StringComparison.OrdinalIgnoreCase))).First<CodeCoverageStatistics>() : (CodeCoverageStatistics) null;
          }
        }
      }
      CodeCoverageStatistics coverageStatistics2 = coverageStatistics1;
      double coveragePercentage = -1.0;
      string rightText = this._statusBadgeNoCoverageText;
      if (coverageStatistics2 != null && coverageStatistics2.Total > 0)
      {
        coveragePercentage = (double) coverageStatistics2.Covered / (double) coverageStatistics2.Total * 100.0;
        rightText = coveragePercentage.ToString("#.##") + "%";
      }
      return CoverageStatusBadgeHelper.GenerateSvg(this.TfsRequestContext, this.GetCodeCoverageResult(this.TfsRequestContext, coveragePercentage), label, rightText, (object) build);
    }

    private CoverageResult GetCodeCoverageResult(
      IVssRequestContext requestContext,
      double coveragePercentage)
    {
      CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
      double statusLowerThreshold = coverageConfiguration.GetCoverageStatusLowerThreshold(requestContext);
      double statusUpperThreshold = coverageConfiguration.GetCoverageStatusUpperThreshold(requestContext);
      if (coveragePercentage < 0.0)
        return CoverageResult.NoData;
      if (coveragePercentage >= 0.0 && coveragePercentage < statusLowerThreshold)
        return CoverageResult.Failed;
      if (coveragePercentage >= statusLowerThreshold && coveragePercentage < statusUpperThreshold)
        return CoverageResult.PartiallySucceeded;
      return coveragePercentage >= statusUpperThreshold ? CoverageResult.Succeeded : CoverageResult.NoData;
    }

    protected virtual bool TryGetProjectId(out Guid projectId)
    {
      projectId = new Guid();
      string str;
      if (!this.ControllerContext.RouteData.Values.TryGetValue<string>("project", out str))
        return false;
      if (!Guid.TryParse(str, out projectId))
      {
        IProjectService service = this.TfsRequestContext.GetService<IProjectService>();
        try
        {
          projectId = service.GetProjectId(this.TfsRequestContext.Elevate(), str, true);
        }
        catch (ProjectDoesNotExistWithNameException ex)
        {
          this.TfsRequestContext.TraceException(0, TraceLevel.Info, "TestManagement", nameof (CoverageStatusBadgeController), (Exception) ex);
          return false;
        }
      }
      return true;
    }

    internal CodeCoverageHelper CodeCovHelper
    {
      get
      {
        if (this.m_codeCoverageHelper == null)
          this.m_codeCoverageHelper = new CodeCoverageHelper(this.TestManagementRequestContext);
        return this.m_codeCoverageHelper;
      }
    }

    protected TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = new TestManagementRequestContext(this.TfsRequestContext.Elevate());
        return this.m_testManagementRequestContext;
      }
    }
  }
}
