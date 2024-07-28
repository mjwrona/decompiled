// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildDetailModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class BuildDetailModel
  {
    public string Uri { get; private set; }

    public string Name { get; private set; }

    public string Definition { get; private set; }

    public string DefinitionUri { get; private set; }

    public string Controller { get; private set; }

    public DateTime StartTime { get; private set; }

    public DateTime FinishTime { get; private set; }

    public string Quality { get; private set; }

    public string LastChangedBy { get; private set; }

    public DateTime LastChangedOn { get; private set; }

    public bool Finished { get; private set; }

    public bool KeepForever { get; private set; }

    public Microsoft.TeamFoundation.Build.Server.BuildStatus Status { get; private set; }

    public BuildReason Reason { get; private set; }

    public string Project { get; private set; }

    public int[] Requests { get; private set; }

    public string RequestedFor { get; private set; }

    public VersionControlDetailsModel VersionControlDetails { get; set; }

    public GitDetailsModel GitDetails { get; set; }

    public BuildInformationModel Information { get; set; }

    public int[] OpenedWorkItems { get; private set; }

    public AssociatedWorkItem[] AssociatedWorkItems { get; private set; }

    public BuildExternalLinkModel DropFolder { get; private set; }

    public CustomSummaryInformationSection[] CustomSummaryInformationSections { get; private set; }

    public bool HasDiagnostics { get; private set; }

    public BuildDetailModel(BuildQueryResult buildResult, string user)
      : this(buildResult, user, true, false)
    {
    }

    public BuildDetailModel(
      BuildQueryResult buildResult,
      string user,
      bool minimal,
      bool isHostedServer)
    {
      BuildDetail current = buildResult.Builds.Current;
      BuildInformationNode[] array1 = current.Information.ToArray<BuildInformationNode>();
      QueuedBuild[] array2 = buildResult.QueuedBuilds.ToArray<QueuedBuild>();
      Dictionary<string, Microsoft.TeamFoundation.Build.Server.BuildController> dictionary = buildResult.Controllers.ToDictionary<Microsoft.TeamFoundation.Build.Server.BuildController, string>((Func<Microsoft.TeamFoundation.Build.Server.BuildController, string>) (c => c.Uri));
      this.Uri = current.Uri;
      this.Name = current.BuildNumber;
      this.Definition = BuildDetailModel.GetDefinitionName(current.Definition);
      this.DefinitionUri = current.Definition != null ? current.Definition.Uri : (string) null;
      this.StartTime = current.StartTime;
      this.FinishTime = current.FinishTime;
      this.Finished = current.BuildFinished();
      this.Quality = current.Quality;
      this.KeepForever = current.KeepForever;
      this.LastChangedOn = current.LastChangedOn;
      this.LastChangedBy = current.LastChangedByDisplayName;
      this.Status = current.Status;
      this.Reason = current.Reason;
      this.Project = current.TeamProject;
      this.Requests = current.QueueIds.ToArray();
      this.DropFolder = new BuildExternalLinkModel(current.DropLocation, string.Empty);
      this.HasDiagnostics = current.ContainerId.HasValue;
      this.Controller = (string) null;
      string buildControllerUri = current.BuildControllerUri;
      Microsoft.TeamFoundation.Build.Server.BuildController buildController;
      ref Microsoft.TeamFoundation.Build.Server.BuildController local = ref buildController;
      if (dictionary.TryGetValue(buildControllerUri, out local))
        this.Controller = buildController.Name;
      this.RequestedFor = (string) null;
      if (array2.Length != 0)
      {
        IEnumerable<string> source = ((IEnumerable<QueuedBuild>) array2).Select<QueuedBuild, string>((Func<QueuedBuild, string>) (r => r.RequestedForDisplayName)).Distinct<string>();
        this.RequestedFor = source.Count<string>() > 1 ? BuildServerResources.BuildDetailViewMultiple : source.First<string>();
      }
      this.PopulateVersionControlFields(current, (IEnumerable<QueuedBuild>) array2, (IEnumerable<BuildInformationNode>) array1, user);
      this.PopulateGitFields(current, (IEnumerable<BuildInformationNode>) array1);
      this.PopulateCustomSummaryInformationSections(current, (IEnumerable<BuildInformationNode>) array1);
      if (this.Finished)
      {
        this.OpenedWorkItems = InformationNodeHelpers.GetOpenedWorkItemIds(array1);
        this.AssociatedWorkItems = InformationNodeHelpers.GetAssociatedWorkItems(array1);
      }
      if (minimal)
        return;
      this.Information = new BuildInformationModel((IEnumerable<BuildInformationNode>) array1);
    }

    public void PopulateTestRuns(IVssRequestContext tfsRequestContext)
    {
      if (!this.Information.Configurations.Any<KeyValuePair<string, BuildConfigurationModel>>())
        return;
      TestManagementRequestContext context = new TestManagementRequestContext(tfsRequestContext);
      foreach (TestRun testRun in TestRun.Query(context, 0, Guid.Empty, this.Uri, this.Project))
      {
        BuildConfigurationModel configurationModel;
        if (this.Information.Configurations.TryGetValue(testRun.BuildPlatform + "|" + testRun.BuildFlavor, out configurationModel))
        {
          BuildTestRunModel run = new BuildTestRunModel(testRun.TestRunId);
          configurationModel.AddRun(run);
          foreach (TestRunStatistic testRunStatistic in TestRunStatistic.Query(context, this.Project, testRun.TestRunId))
          {
            if (testRunStatistic.State == (byte) 5)
            {
              switch (testRunStatistic.Outcome)
              {
                case 2:
                  run.Passed += testRunStatistic.Count;
                  continue;
                case 3:
                case 5:
                case 6:
                case 10:
                  run.Failed += testRunStatistic.Count;
                  continue;
                case 4:
                case 7:
                case 8:
                case 9:
                  run.Inconclusive += testRunStatistic.Count;
                  continue;
                default:
                  continue;
              }
            }
          }
        }
      }
    }

    public void PopulateCoverageData(IVssRequestContext tfsRequestContext)
    {
      if (!this.Information.Configurations.Any<KeyValuePair<string, BuildConfigurationModel>>())
        return;
      foreach (BuildCoverage coverage in BuildCoverage.Query((TestManagementRequestContext) new TfsTestManagementRequestContext(tfsRequestContext), this.Project, this.Uri, CoverageQueryFlags.Modules))
      {
        BuildConfigurationModel configurationModel;
        if (this.Information.Configurations.TryGetValue(coverage.Configuration.BuildPlatform + "|" + coverage.Configuration.BuildFlavor, out configurationModel))
          configurationModel.SetCoverage(new BuildCoverageModel(coverage));
      }
    }

    public static string GetDefinitionName(BuildDefinition definition)
    {
      string definitionName = BuildResources.BuildDetailViewUnknownDefinition;
      if (definition != null && !string.IsNullOrEmpty(definition.Name))
        definitionName = definition.Name;
      return definitionName;
    }

    private void PopulateVersionControlFields(
      BuildDetail build,
      IEnumerable<QueuedBuild> queuedBuilds,
      IEnumerable<BuildInformationNode> informationNodes,
      string user)
    {
      this.VersionControlDetails = new VersionControlDetailsModel();
      this.VersionControlDetails.Version = build.SourceGetVersion;
      this.VersionControlDetails.VersionText = this.GetVersionText(build.SourceGetVersion, user);
      this.VersionControlDetails.ChangesetId = InformationNodeHelpers.GetChangesetId(informationNodes);
      this.VersionControlDetails.ShelvesetName = (string) null;
      if (queuedBuilds.Count<QueuedBuild>() > 0)
        this.VersionControlDetails.ShelvesetName = queuedBuilds.Count<QueuedBuild>() > 1 ? BuildServerResources.BuildDetailViewMultiple : queuedBuilds.First<QueuedBuild>().ShelvesetName;
      int successful;
      int failed;
      InformationNodeHelpers.GetChangesetsInfo(informationNodes, out successful, out failed);
      this.VersionControlDetails.SuccessChangesetCount = successful;
      this.VersionControlDetails.FailChangesetCount = failed;
      if (!this.Finished)
        return;
      this.VersionControlDetails.AssociatedChangesets = informationNodes.Where<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => string.Equals(node.Type, InformationTypes.AssociatedChangeset, StringComparison.OrdinalIgnoreCase))).Select<BuildInformationNode, AssociatedChangesetModel>((Func<BuildInformationNode, AssociatedChangesetModel>) (node => new AssociatedChangesetModel(node))).ToArray<AssociatedChangesetModel>();
    }

    private void PopulateGitFields(
      BuildDetail build,
      IEnumerable<BuildInformationNode> informationNodes)
    {
      this.GitDetails = new GitDetailsModel();
      if (!this.Finished)
        return;
      this.GitDetails.AssociatedCommits = informationNodes.Where<BuildInformationNode>((Func<BuildInformationNode, bool>) (node => string.Equals(node.Type, InformationTypes.AssociatedCommit, StringComparison.OrdinalIgnoreCase))).Select<BuildInformationNode, AssociatedCommitModel>((Func<BuildInformationNode, AssociatedCommitModel>) (node => new AssociatedCommitModel(node))).ToArray<AssociatedCommitModel>();
    }

    private string GetVersionText(string versionSpec, string user)
    {
      try
      {
        VersionSpec singleSpec = VersionSpec.ParseSingleSpec(versionSpec, user);
        switch (singleSpec)
        {
          case ChangesetVersionSpec changesetVersionSpec:
            return string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildResources.BuildDetailViewChangesetFormat, (object) changesetVersionSpec.ChangesetId);
          case LatestVersionSpec _:
            return BuildServerResources.BuildDetailViewLatestSources;
          case LabelVersionSpec _:
            string branch;
            string commit;
            if (BuildSourceVersion.TryParseGit(versionSpec, out branch, out commit))
            {
              if (!string.IsNullOrEmpty(branch) && !string.IsNullOrEmpty(commit) && commit.Length >= 7)
                return string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDetailViewGitCommitFormatWithCommit, (object) branch, (object) commit.Substring(0, 7));
              if (!string.IsNullOrEmpty(branch))
                return string.Format((IFormatProvider) CultureInfo.CurrentCulture, BuildServerResources.BuildDetailViewGitCommitFormat, (object) branch);
              break;
            }
            break;
        }
        return singleSpec.ToString();
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (BuildDetailModel), ex);
        return versionSpec;
      }
    }

    private void PopulateCustomSummaryInformationSections(
      BuildDetail build,
      IEnumerable<BuildInformationNode> informationNodes)
    {
      Dictionary<string, CustomSummaryInformationSection> dictionary = new Dictionary<string, CustomSummaryInformationSection>();
      foreach (CustomSummaryInformation summaryInformation in InformationNodeHelpers.GetCustomSummaryInformation(informationNodes))
      {
        if (!string.IsNullOrEmpty(summaryInformation.SectionName))
        {
          CustomSummaryInformationSection informationSection;
          if (dictionary.TryGetValue(summaryInformation.SectionName, out informationSection))
          {
            informationSection.Messages.Add(summaryInformation.Message);
            if (summaryInformation.SectionPriority < informationSection.Priority && summaryInformation.SectionPriority > -1)
              informationSection.Priority = summaryInformation.SectionPriority;
          }
          else
          {
            informationSection = new CustomSummaryInformationSection()
            {
              Header = summaryInformation.SectionHeader,
              Name = summaryInformation.SectionName,
              Priority = summaryInformation.SectionPriority
            };
            informationSection.Messages.Add(summaryInformation.Message);
            dictionary.Add(informationSection.Name, informationSection);
          }
        }
      }
      List<CustomSummaryInformationSection> list = dictionary.Values.ToList<CustomSummaryInformationSection>();
      list.Sort((Comparison<CustomSummaryInformationSection>) ((s1, s2) => s1.Priority - s2.Priority));
      this.CustomSummaryInformationSections = list.ToArray();
    }

    public void FixIntermediateLogNodes(string logsFolderUrl)
    {
      List<IntermediateLogNodeModel> intermediateLogNodeModels = this.Information.GetIntermediateLogNodeModels();
      if (!intermediateLogNodeModels.Any<IntermediateLogNodeModel>())
        return;
      foreach (IntermediateLogNodeModel intermediateLogNodeModel in intermediateLogNodeModels)
        intermediateLogNodeModel.MakeLocationRelative(logsFolderUrl);
    }

    public JsObject ToJson()
    {
      JsObject jsObject = new JsObject();
      jsObject.Add("uri", (object) this.Uri);
      jsObject.Add("name", (object) this.Name);
      jsObject.Add("definition", (object) this.Definition);
      jsObject.Add("definitionUri", (object) this.DefinitionUri);
      jsObject.Add("controller", (object) this.Controller);
      jsObject.Add("startTime", (object) this.StartTime);
      jsObject.Add("finishTime", (object) this.FinishTime);
      jsObject.Add("finished", (object) this.Finished);
      jsObject.Add("quality", (object) this.Quality);
      jsObject.Add("retain", (object) this.KeepForever);
      jsObject.Add("lastChangedBy", (object) this.LastChangedBy);
      jsObject.Add("lastChangedOn", (object) this.LastChangedOn);
      jsObject.Add("status", (object) this.Status);
      jsObject.Add("reason", (object) this.Reason);
      jsObject.Add("dropFolder", (object) this.DropFolder.ToJson());
      jsObject.Add("requestedFor", (object) this.RequestedFor);
      jsObject.Add("project", (object) this.Project);
      jsObject.Add("requests", (object) this.Requests);
      jsObject.Add("vc", (object) this.VersionControlDetails.ToJson());
      jsObject.Add("git", (object) this.GitDetails.ToJson());
      jsObject.Add("hasDiagnostics", (object) this.HasDiagnostics);
      jsObject.Add("customSummarySections", (object) ((IEnumerable<CustomSummaryInformationSection>) this.CustomSummaryInformationSections).Select<CustomSummaryInformationSection, JsObject>((Func<CustomSummaryInformationSection, JsObject>) (c => c.ToJson())));
      JsObject json = jsObject;
      if (this.Information != null)
        json["information"] = this.Information.ToJson();
      if (this.OpenedWorkItems != null)
        json["openedWorkItems"] = (object) this.OpenedWorkItems;
      if (this.AssociatedWorkItems != null)
        json["associatedWorkItems"] = (object) ((IEnumerable<AssociatedWorkItem>) this.AssociatedWorkItems).Select<AssociatedWorkItem, JsObject>((Func<AssociatedWorkItem, JsObject>) (awi => awi.ToJson()));
      return json;
    }
  }
}
