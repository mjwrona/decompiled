// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.WellKnownControlNames
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateAllConstants(null)]
  public static class WellKnownControlNames
  {
    public static readonly string HtmlControl = "HtmlFieldControl";
    public static readonly string PlainTextControl = nameof (PlainTextControl);
    public static readonly string FreshnessIndicatorControl = nameof (FreshnessIndicatorControl);
    public static readonly string FieldControl = nameof (FieldControl);
    public static readonly string DateControl = "DateTimeControl";
    public static readonly string ClassificationControl = "WorkItemClassificationControl";
    public static readonly string WorkItemLogControl = nameof (WorkItemLogControl);
    public static readonly string WorkItemHistoryControl = nameof (WorkItemHistoryControl);
    public static readonly string WorkItemStateGraphControl = nameof (WorkItemStateGraphControl);
    public static readonly string LinksControl = nameof (LinksControl);
    public static readonly string AttachmentsControl = nameof (AttachmentsControl);
    public static readonly string WebpageControl = nameof (WebpageControl);
    public static readonly string LabelControl = nameof (LabelControl);
    public static readonly string TagFieldControl = nameof (TagFieldControl);
    public static readonly string TestStepsControl = nameof (TestStepsControl);
    public static readonly string AssociatedAutomationControl = nameof (AssociatedAutomationControl);
    public static readonly string StageControl = nameof (StageControl);
    public static readonly string StageBuildControl = nameof (StageBuildControl);
    public static readonly string ReleaseBuildControl = nameof (ReleaseBuildControl);
    public static readonly string AcceptanceCriteriaControl = nameof (AcceptanceCriteriaControl);
    public static readonly string DevelopmentControl = nameof (DevelopmentControl);
    public static readonly string WorkItemDiscussionControl = nameof (WorkItemDiscussionControl);
    public static readonly string ParameterSetControl = nameof (ParameterSetControl);
    public static readonly string DeploymentsControl = nameof (DeploymentsControl);
  }
}
