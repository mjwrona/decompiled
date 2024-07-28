// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.InformationTypes
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  public static class InformationTypes
  {
    public static readonly string ActivityProperties = nameof (ActivityProperties);
    public static readonly string ActivityTracking = nameof (ActivityTracking);
    public static readonly string AgentScopeActivityTracking = nameof (AgentScopeActivityTracking);
    public static readonly string AssociatedChangeset = nameof (AssociatedChangeset);
    public static readonly string AssociatedCommit = nameof (AssociatedCommit);
    public static readonly string AssociatedWorkItem = nameof (AssociatedWorkItem);
    public static readonly string BuildError = nameof (BuildError);
    public static readonly string BuildMessage = nameof (BuildMessage);
    public static readonly string BuildProject = nameof (BuildProject);
    public static readonly string BuildStep = nameof (BuildStep);
    public static readonly string BuildWarning = nameof (BuildWarning);
    public static readonly string CheckInOutcome = nameof (CheckInOutcome);
    public static readonly string CompilationSummary = nameof (CompilationSummary);
    public static readonly string ConfigurationSummary = nameof (ConfigurationSummary);
    public static readonly string DeploymentInformation = nameof (DeploymentInformation);
    public static readonly string ExternalLink = nameof (ExternalLink);
    public static readonly string GetStatus = nameof (GetStatus);
    public static readonly string OpenedWorkItem = nameof (OpenedWorkItem);
    public static readonly string CustomSummaryInformation = nameof (CustomSummaryInformation);
    public static readonly string OtherBuildErrorsCount = nameof (OtherBuildErrorsCount);
    public static readonly string AutomatedTests = nameof (AutomatedTests);
    public static readonly string ReshelvedShelveset = nameof (ReshelvedShelveset);
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string RunOnceState = nameof (RunOnceState);
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string SymStoreTransaction = nameof (SymStoreTransaction);
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string IntermediateLogInformation = nameof (IntermediateLogInformation);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string CodeCoverageSummary = nameof (CodeCoverageSummary);
    [Obsolete("This property has been deprecated. Please remove all references.", false)]
    public static readonly string TestSummary = nameof (TestSummary);
  }
}
