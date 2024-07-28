// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.FeatureFlagConstants
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.ComponentModel;

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [GenerateAllConstants(null)]
  public sealed class FeatureFlagConstants
  {
    public const string EnableChecksScalabilityPhase1Warnings = "Pipelines.Checks.EnableChecksScalabilityPhase1Warnings";
    public const string EnableChecksScalabilityPhase2Errors = "Pipelines.Checks.EnableChecksScalabilityPhase2Errors";
    public const string EnableChecksScalabilityPhase3FailCheckRuns = "Pipelines.Checks.EnableChecksScalabilityPhase3FailCheckRuns";
    public const string EnableChecksScalabilityResourceIssues = "Pipelines.Checks.EnableChecksScalabilityResourceIssues";
    public const string EnableDisabledChecksFeature = "Pipelines.Checks.EnableDisabledChecksFeature";
    public const string ResourceListOptimizations = "Pipelines.Policy.ResourceListOptimizations";
    public const string EnableBypassTaskChecks = "Pipelines.Checks.EnableBypassTaskChecks";
    public const string EnableBypassApprovals = "Pipelines.Checks.EnableBypassApprovals";
    public const string EnableRerunChecks = "Pipelines.Checks.EnableRerunChecks";
  }
}
