// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VCTraceKeywords
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VCTraceKeywords : TraceKeywords
  {
    internal static readonly string Annotate = TraceKeywords.BuildKeyword(nameof (Annotate));
    internal static readonly string BranchVisualization = TraceKeywords.BuildKeyword(nameof (BranchVisualization));
    public static readonly string CodeReview = TraceKeywords.BuildKeyword(nameof (CodeReview));
    public static readonly string Download = TraceKeywords.BuildKeyword(nameof (Download));
    public static readonly string Upload = TraceKeywords.BuildKeyword(nameof (Upload));
    internal static readonly string FolderDiff = TraceKeywords.BuildKeyword("FolderDifference");
    internal static readonly string Get = TraceKeywords.BuildKeyword(nameof (Get));
    internal static readonly string History = TraceKeywords.BuildKeyword(nameof (History));
    internal static readonly string Label = TraceKeywords.BuildKeyword(nameof (Label));
    public static readonly string MyWork = TraceKeywords.BuildKeyword(nameof (MyWork));
    internal static readonly string PendingChanges = TraceKeywords.BuildKeyword(nameof (PendingChanges));
    internal static readonly string ProgressDialog = TraceKeywords.BuildKeyword(nameof (ProgressDialog));
    internal static readonly string Resolve = TraceKeywords.BuildKeyword(nameof (Resolve));
    internal static readonly string ShelvesetDetails = TraceKeywords.BuildKeyword(nameof (ShelvesetDetails));
    internal static readonly string SolutionIntegration = TraceKeywords.BuildKeyword(nameof (SolutionIntegration));
    public static readonly string SCE = TraceKeywords.BuildKeyword("SourceControlExplorer");
    internal static readonly string UserTools = TraceKeywords.BuildKeyword(nameof (UserTools));
    internal static readonly string Vulcan = TraceKeywords.BuildKeyword(nameof (Vulcan));
    internal static readonly string HandsOffHandsOn = TraceKeywords.BuildKeyword(nameof (HandsOffHandsOn));
    internal static readonly string DiffMerge = TraceKeywords.BuildKeyword(nameof (DiffMerge));
    internal static readonly string FindShelvesets = TraceKeywords.BuildKeyword(nameof (FindShelvesets));
    internal static readonly string ChangesetDetails = TraceKeywords.BuildKeyword(nameof (ChangesetDetails));
    internal static readonly string LocalWorkspaces = TraceKeywords.BuildKeyword(nameof (LocalWorkspaces));
  }
}
