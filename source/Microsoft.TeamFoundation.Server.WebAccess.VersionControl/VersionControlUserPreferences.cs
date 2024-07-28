// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlUserPreferences
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Settings;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [DataContract]
  public class VersionControlUserPreferences : VersionControlSecuredObject
  {
    private const string c_userPreferencesSettingKey = "VersionControl/UserOptions";
    private const int WebAccessExceptionEaten = 599999;

    public VersionControlUserPreferences()
    {
      this.DiffViewerOrientation = DiffViewerOrientation.SideBySide;
      this.DiffViewerImageMode = DiffViewerImageMode.TwoUp;
      this.SummaryDiffOrientation = DiffViewerOrientation.Inline;
      this.ChangeExplorerGridDisplayMode = ChangeExplorerGridDisplayMode.FullTree;
      this.ChangeExplorerGridCommentsMode = ChangeExplorerGridCommentsMode.Default;
      this.CodeReviewCreateMode = CodeReviewCreateMode.Advanced;
      this.CodeReviewLandingPageMode = CodeReviewLandingPageMode.RequestedByMe;
      this.HistoryMode = GitHistoryMode.SimplifiedHistory;
      this.MergeOptionsDeleteSourceCheckboxMode = MergeOptionsDeleteSourceCheckboxMode.Checked;
      this.MergeOptionsMergeStrategy = GitPullRequestMergeStrategy.NoFastForward;
      this.MergeOptionsSquashMergeCheckboxMode = MergeOptionsSquashMergeCheckboxMode.Unchecked;
      this.MergeOptionsTransitionWorkItemsCheckboxMode = MergeOptionsTransitionWorkItemsCheckboxMode.Checked;
      this.PullRequestActivityOrder = PullRequestActivityOrder.Time_NewFirst;
      this.PullRequestActivityFilter = -1;
      this.PullRequestActivityDescriptionExpanded = PullRequestActivityDescriptionExpanded.Expanded;
      this.PullRequestListCustomCriteria = "";
      this.EditorPreferences = new EditorPreferences();
    }

    [DataMember(Name = "diffViewerOrientation")]
    public DiffViewerOrientation DiffViewerOrientation { get; set; }

    [DataMember(Name = "diffViewerImageMode")]
    public DiffViewerImageMode DiffViewerImageMode { get; set; }

    [DataMember(Name = "summaryDiffOrientation")]
    public DiffViewerOrientation SummaryDiffOrientation { get; set; }

    [DataMember(Name = "changeExplorerGridDisplayMode")]
    public ChangeExplorerGridDisplayMode ChangeExplorerGridDisplayMode { get; set; }

    [DataMember(Name = "changeExplorerGridCommentsMode")]
    public ChangeExplorerGridCommentsMode ChangeExplorerGridCommentsMode { get; set; }

    [DataMember(Name = "codeReviewCreateMode")]
    public CodeReviewCreateMode CodeReviewCreateMode { get; set; }

    [DataMember(Name = "codeReviewLandingPageMode", IsRequired = false)]
    public CodeReviewLandingPageMode CodeReviewLandingPageMode { get; set; }

    [DataMember(Name = "editorPreferences")]
    public EditorPreferences EditorPreferences { get; set; }

    [DataMember(Name = "historyMode")]
    public GitHistoryMode HistoryMode { get; set; }

    [DataMember(Name = "mergeOptionsDeleteSourceCheckboxMode", IsRequired = false)]
    public MergeOptionsDeleteSourceCheckboxMode MergeOptionsDeleteSourceCheckboxMode { get; set; }

    [DataMember(Name = "mergeOptionsMergeStrategy", IsRequired = false)]
    public GitPullRequestMergeStrategy MergeOptionsMergeStrategy { get; set; }

    [DataMember(Name = "mergeOptionsSquashMergeCheckboxMode", IsRequired = false)]
    public MergeOptionsSquashMergeCheckboxMode MergeOptionsSquashMergeCheckboxMode { get; set; }

    [DataMember(Name = "mergeOptionsTransitionWorkItemsCheckboxMode", IsRequired = false)]
    public MergeOptionsTransitionWorkItemsCheckboxMode MergeOptionsTransitionWorkItemsCheckboxMode { get; set; }

    [DataMember(Name = "pullRequestActivityOrder", IsRequired = false)]
    public PullRequestActivityOrder PullRequestActivityOrder { get; set; }

    [DataMember(Name = "pullRequestActivityFilter", IsRequired = false)]
    public int PullRequestActivityFilter { get; set; }

    [DataMember(Name = "pullRequestActivityDescriptionExpanded", IsRequired = false)]
    public PullRequestActivityDescriptionExpanded PullRequestActivityDescriptionExpanded { get; set; }

    [DataMember(Name = "pullRequestListCustomCriteria", IsRequired = false)]
    public string PullRequestListCustomCriteria { get; set; }

    public static VersionControlUserPreferences GetUserPreferences(TfsWebContext tfsWebContext) => VersionControlUserPreferences.GetUserPreferences(tfsWebContext.TfsRequestContext);

    public static VersionControlUserPreferences GetUserPreferences(IVssRequestContext requestContext) => requestContext.GetService<ISettingsService>().GetValue<VersionControlUserPreferences>(requestContext, SettingsUserScope.User, "VersionControl/UserOptions", (VersionControlUserPreferences) null, false) ?? new VersionControlUserPreferences();

    public static void SetUserPreferences(
      TfsWebContext tfsWebContext,
      VersionControlUserPreferences preferences)
    {
      tfsWebContext.TfsRequestContext.GetService<ISettingsService>().SetValue(tfsWebContext.TfsRequestContext, SettingsUserScope.User, "VersionControl/UserOptions", (object) preferences);
    }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      this.EditorPreferences?.SetSecuredObject(securedObject);
    }
  }
}
