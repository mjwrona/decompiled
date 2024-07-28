// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VCTraceKeywordSets
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class VCTraceKeywordSets : TraceKeywordSets
  {
    public static readonly string[] Annotate = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.Annotate
    };
    public static readonly string[] BranchVisualization = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.BranchVisualization
    };
    public static readonly string[] Download = new string[1]
    {
      VCTraceKeywords.Download
    };
    public static readonly string[] Upload = new string[1]
    {
      VCTraceKeywords.Upload
    };
    public static readonly string[] LocalWorkspaces = new string[1]
    {
      VCTraceKeywords.LocalWorkspaces
    };
    public static readonly string[] FolderDiff = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.FolderDiff
    };
    public static readonly string[] GeneralVC = new string[1]
    {
      TraceKeywords.VersionControl
    };
    public static readonly string[] Get = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.Get
    };
    public static readonly string[] History = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.History
    };
    public static readonly string[] Label = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.Label
    };
    public static readonly string[] MyWork = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.MyWork
    };
    public static readonly string[] PendingChanges = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.PendingChanges
    };
    public static readonly string[] ProgressDialog = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.ProgressDialog
    };
    public static readonly string[] Resolve = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.Resolve
    };
    public static readonly string[] ShelvesetDetails = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.ShelvesetDetails
    };
    public static readonly string[] SolutionIntegration = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.SolutionIntegration
    };
    public static readonly string[] SCE = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.SCE
    };
    public static readonly string[] UserTools = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.UserTools
    };
    public static readonly string[] Vulcan = new string[1]
    {
      VCTraceKeywords.Vulcan
    };
    public static readonly string[] HandsOffHandsOn = new string[1]
    {
      VCTraceKeywords.HandsOffHandsOn
    };
    public static readonly string[] DiffMerge = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.DiffMerge
    };
    public static readonly string[] FindShelvesets = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.FindShelvesets
    };
    public static readonly string[] ChangesetDetails = new string[2]
    {
      TraceKeywords.VersionControl,
      VCTraceKeywords.ChangesetDetails
    };
    public static readonly string[] CodeReview = new string[1]
    {
      VCTraceKeywords.CodeReview
    };
  }
}
