// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.DataQuality.DataQualityDefinition
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics.DataQuality
{
  public class DataQualityDefinition
  {
    public int MinServiceVersion { get; }

    public int? MaxServiceVersion { get; }

    public string Name { get; }

    public string SprocName { get; }

    public int MinimumIntervalSeconds { get; }

    public string KpiName { get; }

    public string KpiDisplayName { get; }

    public string KpiDescription { get; }

    public DataQualityCriterias Criterias { get; }

    public string FeatureFlagToRun { get; }

    public string FeatureFlagForModelReady { get; }

    public bool ShouldExposeWarning { get; }

    public Func<DataQualityResult, string> WarningMessage { get; }

    public DataQualityDefinition(
      int minServiceVersion,
      int? maxServiceVersion,
      string name,
      string sprocName,
      int minimumIntervalSeconds,
      DataQualityCriterias criterias = DataQualityCriterias.None,
      string kpiName = null,
      string kpiDisplayName = null,
      bool shouldExposeWarning = false,
      Func<DataQualityResult, string> warningMessage = null)
    {
      this.MinServiceVersion = minServiceVersion;
      this.MaxServiceVersion = maxServiceVersion;
      this.Name = name;
      this.SprocName = sprocName;
      this.MinimumIntervalSeconds = minimumIntervalSeconds;
      this.Criterias = criterias;
      this.ShouldExposeWarning = shouldExposeWarning;
      this.WarningMessage = warningMessage;
    }

    public DataQualityDefinition(
      int minServiceVersion,
      int? maxServiceVersion,
      string name,
      string sprocName,
      int minimumIntervalSeconds,
      string kpiName,
      string kpiDisplayName,
      string kpiDescription,
      DataQualityCriterias criterias = DataQualityCriterias.None,
      string featureFlagToRun = null,
      string featureFlagForModelReady = null,
      bool shouldExposeWarning = false,
      Func<DataQualityResult, string> warningMessage = null)
    {
      this.MinServiceVersion = minServiceVersion;
      this.MaxServiceVersion = maxServiceVersion;
      this.Name = name;
      this.SprocName = sprocName;
      this.MinimumIntervalSeconds = minimumIntervalSeconds;
      this.KpiName = kpiName;
      this.KpiDisplayName = kpiDisplayName;
      this.KpiDescription = kpiDescription;
      this.Criterias = criterias;
      this.FeatureFlagToRun = featureFlagToRun;
      this.FeatureFlagForModelReady = featureFlagForModelReady;
      this.ShouldExposeWarning = shouldExposeWarning;
      this.WarningMessage = warningMessage;
    }
  }
}
