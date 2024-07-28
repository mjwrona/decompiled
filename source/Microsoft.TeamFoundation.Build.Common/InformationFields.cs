// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.InformationFields
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class InformationFields
  {
    public static readonly string AssemblySignature = nameof (AssemblySignature);
    public static readonly string ActivityInstanceId = nameof (ActivityInstanceId);
    public static readonly string ActivityType = nameof (ActivityType);
    public static readonly string AssignedTo = nameof (AssignedTo);
    public static readonly string Author = nameof (Author);
    public static readonly string ChangesetId = nameof (ChangesetId);
    public static readonly string ChangesetUri = nameof (ChangesetUri);
    public static readonly string CheckedInBy = nameof (CheckedInBy);
    public static readonly string CheckInCommitted = nameof (CheckInCommitted);
    public static readonly string Code = nameof (Code);
    public static readonly string Comment = nameof (Comment);
    public static readonly string CommitId = nameof (CommitId);
    public static readonly string Committer = nameof (Committer);
    public static readonly string CompilationErrors = nameof (CompilationErrors);
    public static readonly string CompilationWarnings = nameof (CompilationWarnings);
    public static readonly string ContextId = nameof (ContextId);
    public static readonly string DisplayText = nameof (DisplayText);
    public static readonly string EndLineNumber = nameof (EndLineNumber);
    public static readonly string ErrorType = nameof (ErrorType);
    public static readonly string File = nameof (File);
    public static readonly string FinishTime = nameof (FinishTime);
    internal static readonly string FirstAgentName = nameof (FirstAgentName);
    internal static readonly string FirstAgentUri = nameof (FirstAgentUri);
    public static readonly string Flavor = nameof (Flavor);
    public static readonly string GitUri = nameof (GitUri);
    public static readonly string Importance = nameof (Importance);
    public static readonly string LineNumber = nameof (LineNumber);
    public static readonly string LocalPath = nameof (LocalPath);
    public static readonly string LogFile = nameof (LogFile);
    public static readonly string Message = nameof (Message);
    public static readonly string Name = nameof (Name);
    public static readonly string NumBytes = nameof (NumBytes);
    public static readonly string NumFiles = nameof (NumFiles);
    public static readonly string OriginalName = nameof (OriginalName);
    public static readonly string Owner = nameof (Owner);
    public static readonly string OriginalOwner = nameof (OriginalOwner);
    public static readonly string Platform = nameof (Platform);
    public static readonly string PossibleAgents = nameof (PossibleAgents);
    public static readonly string ProjectFile = nameof (ProjectFile);
    public static readonly string Properties = nameof (Properties);
    public static readonly string QualifiedName = nameof (QualifiedName);
    public static readonly string RelativeServerLogDirectory = nameof (RelativeServerLogDirectory);
    public static readonly string RequestId = nameof (RequestId);
    public static readonly string RequestedBy = nameof (RequestedBy);
    public static readonly string ReservationStatus = nameof (ReservationStatus);
    public static readonly string ReservedAgentName = nameof (ReservedAgentName);
    public static readonly string ReservedAgentUri = nameof (ReservedAgentUri);
    public static readonly string SectionHeader = nameof (SectionHeader);
    public static readonly string SectionName = nameof (SectionName);
    public static readonly string SectionPriority = nameof (SectionPriority);
    public static readonly string ServerPath = nameof (ServerPath);
    public static readonly string StartTime = nameof (StartTime);
    public static readonly string State = nameof (State);
    public static readonly string StaticAnalysisErrors = nameof (StaticAnalysisErrors);
    public static readonly string StaticAnalysisWarnings = nameof (StaticAnalysisWarnings);
    public static readonly string Status = nameof (Status);
    internal static readonly string StorePath = nameof (StorePath);
    public static readonly string TargetNames = nameof (TargetNames);
    public static readonly string Timestamp = nameof (Timestamp);
    public static readonly string TotalCompilationErrors = nameof (TotalCompilationErrors);
    public static readonly string TotalCompilationWarnings = nameof (TotalCompilationWarnings);
    public static readonly string TotalOtherErrorsCount = nameof (TotalOtherErrorsCount);
    public static readonly string TotalStaticAnalysisErrors = nameof (TotalStaticAnalysisErrors);
    public static readonly string TotalStaticAnalysisWarnings = nameof (TotalStaticAnalysisWarnings);
    public static readonly string Title = nameof (Title);
    internal static readonly string TransactionId = nameof (TransactionId);
    public static readonly string Uri = nameof (Uri);
    public static readonly string Url = nameof (Url);
    public static readonly string WarningType = nameof (WarningType);
    public static readonly string WorkItemId = nameof (WorkItemId);
    public static readonly string WorkItemUri = nameof (WorkItemUri);
    public static readonly string WorkItemType = nameof (WorkItemType);
    public static readonly string DeploymentInformationType = nameof (DeploymentInformationType);
    public static readonly string TotalTests = nameof (TotalTests);
    public static readonly string PassedTests = nameof (PassedTests);
    public static readonly string FailedTests = nameof (FailedTests);
    public static readonly string NotImpactedTests = nameof (NotImpactedTests);
    public static readonly string OtherTests = nameof (OtherTests);
    public static readonly string TestRunDuration = nameof (TestRunDuration);
    public static readonly string TestResultPassPercent = nameof (TestResultPassPercent);
    public static readonly string WebAccessUri = nameof (WebAccessUri);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string BlocksCovered = nameof (BlocksCovered);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string BlocksNotCovered = nameof (BlocksNotCovered);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string LinesCovered = nameof (LinesCovered);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string LinesNotCovered = nameof (LinesNotCovered);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string LinesPartiallyCovered = nameof (LinesPartiallyCovered);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string IsBuildCoverageProcessing = nameof (IsBuildCoverageProcessing);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string RunId = nameof (RunId);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string RunPassed = nameof (RunPassed);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string RunUser = nameof (RunUser);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string TestsFailed = nameof (TestsFailed);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string TestsPassed = nameof (TestsPassed);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string TestsTotal = nameof (TestsTotal);
  }
}
