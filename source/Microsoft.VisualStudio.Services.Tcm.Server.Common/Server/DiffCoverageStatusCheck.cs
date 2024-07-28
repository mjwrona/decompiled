// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DiffCoverageStatusCheck
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class DiffCoverageStatusCheck : ICoverageStatusCheck
  {
    public string Source { get; private set; }

    public string Name { get; private set; }

    public CoverageStatusCheckResult Evaluate(
      TestManagementRequestContext requestContext,
      string source,
      CoverageMetrics coverageMetrics,
      CoverageStatusCheckConfiguration statusCheckConfiguration)
    {
      this.Source = source;
      this.Name = nameof (DiffCoverageStatusCheck);
      CoverageStatusCheckResult statusCheckResult = new CoverageStatusCheckResult()
      {
        Source = this.Source,
        Name = this.Name,
        Properties = new Dictionary<string, object>()
      };
      if (coverageMetrics == null)
      {
        statusCheckResult.State = CoverageStatusCheckState.InProgress;
        statusCheckResult.Properties.Add("State", (object) statusCheckResult.State);
        return statusCheckResult;
      }
      if (!coverageMetrics.AggregatedDiffCoverage.Coverage.HasValue)
      {
        statusCheckResult.Properties.Add("DiffCoverage", (object) null);
        statusCheckResult.State = !requestContext.IsFeatureEnabled("TestManagement.Server.FailCodeCoverageStatusIfNotTestsFound") || !coverageMetrics.AggregatedDiffCoverage.CoverageDataNotFound ? CoverageStatusCheckState.Succeeded : CoverageStatusCheckState.Failed;
        return statusCheckResult;
      }
      double? nullable1 = string.IsNullOrEmpty(statusCheckConfiguration.ExceptionMessage) ? statusCheckConfiguration.DiffCoverageThreshold : throw new InvalidStatusCheckConfigurationException(string.Format(CoverageResources.CoverageConfigurationError, (object) statusCheckConfiguration.ExceptionMessage));
      if (!nullable1.HasValue)
      {
        nullable1 = new CoverageConfiguration().GetCoverageStatusCheckConfiguration(requestContext.RequestContext).DiffCoverageThreshold;
        statusCheckResult.Properties.Add("ThresholdType", (object) "Default");
      }
      else
        statusCheckResult.Properties.Add("ThresholdType", (object) "UserSpecified");
      statusCheckResult.Properties.Add("Threshold", (object) nullable1);
      double? nullable2 = nullable1;
      double num = 100.0;
      double? nullable3 = nullable2.HasValue ? new double?(nullable2.GetValueOrDefault() / num) : new double?();
      statusCheckResult.Description = "Diff coverage target is " + nullable3?.ToString("P", (IFormatProvider) CultureInfo.InvariantCulture);
      statusCheckResult.Properties.Add("DiffCoverage", (object) coverageMetrics.AggregatedDiffCoverage.Coverage);
      nullable2 = coverageMetrics.AggregatedDiffCoverage.Coverage;
      double? nullable4 = nullable3;
      if (nullable2.GetValueOrDefault() < nullable4.GetValueOrDefault() & nullable2.HasValue & nullable4.HasValue)
      {
        statusCheckResult.State = CoverageStatusCheckState.Failed;
        statusCheckResult.Properties.Add("State", (object) statusCheckResult.State);
        return statusCheckResult;
      }
      statusCheckResult.State = CoverageStatusCheckState.Succeeded;
      statusCheckResult.Properties.Add("State", (object) statusCheckResult.State);
      return statusCheckResult;
    }
  }
}
