// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ExperimentFeature
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  public class ExperimentFeature
  {
    internal static readonly string DefaultValueRulePluginName = "ABTest";
    internal static readonly string PercentEnabledPropertyKey = "percentEnabled";
    internal static readonly string GroupByPropertyKey = "groupBy";

    public ExperimentFeature(ContributedFeature contributedFeature)
    {
      this.ContributedFeature = contributedFeature;
      ContributedFeatureValueRule featureValueRule = contributedFeature.DefaultValueRules.SingleOrDefault<ContributedFeatureValueRule>((Func<ContributedFeatureValueRule, bool>) (rule => string.Equals(rule.Name, ExperimentFeature.DefaultValueRulePluginName, StringComparison.InvariantCultureIgnoreCase)));
      this.PercentEnabled = featureValueRule != null ? this.ParsePercentEnabledProperty(featureValueRule.Properties) : throw new InvalidOperationException("Experiment features must use a single " + ExperimentFeature.DefaultValueRulePluginName + " default value plugin");
      this.GroupByStrategyName = this.ParseProperty<string>(featureValueRule.Properties, ExperimentFeature.GroupByPropertyKey);
    }

    public double PercentEnabled { get; internal set; }

    public string GroupByStrategyName { get; internal set; }

    public ContributedFeature ContributedFeature { get; internal set; }

    private T ParseProperty<T>(IDictionary<string, object> properties, string key)
    {
      object obj1;
      if (!properties.TryGetValue(key, out obj1))
        throw new InvalidOperationException("Experiment features must specify a '" + key + "' property.");
      return obj1 is T obj2 ? obj2 : throw new InvalidOperationException("Property '" + key + "' must be of type '" + typeof (T).ToString() + ".");
    }

    private double ParsePercentEnabledProperty(IDictionary<string, object> properties)
    {
      double property = this.ParseProperty<double>(properties, ExperimentFeature.PercentEnabledPropertyKey);
      return property >= 0.0 && property <= 100.0 ? property : throw new InvalidOperationException("Property '" + ExperimentFeature.PercentEnabledPropertyKey + "' must be between 0 and 100");
    }
  }
}
