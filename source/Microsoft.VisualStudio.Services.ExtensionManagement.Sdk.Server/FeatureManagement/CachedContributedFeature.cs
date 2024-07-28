// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.CachedContributedFeature
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  internal class CachedContributedFeature
  {
    public CachedContributedFeature(
      IVssRequestContext requestContext,
      ContributedFeature feature,
      IDictionary<string, IContributedFeatureValuePlugin> registeredPlugins)
    {
      this.Feature = feature;
      if (feature.OverrideRules != null)
        this.OverrideRules = (IEnumerable<ParsedFeatureRule>) feature.OverrideRules.Select<ContributedFeatureValueRule, ParsedFeatureRule>((Func<ContributedFeatureValueRule, ParsedFeatureRule>) (r => new ParsedFeatureRule(requestContext, feature, r, registeredPlugins))).ToList<ParsedFeatureRule>();
      if (feature.DefaultValueRules == null)
        return;
      this.DefaultValueRules = (IEnumerable<ParsedFeatureRule>) feature.DefaultValueRules.Select<ContributedFeatureValueRule, ParsedFeatureRule>((Func<ContributedFeatureValueRule, ParsedFeatureRule>) (r => new ParsedFeatureRule(requestContext, feature, r, registeredPlugins))).ToList<ParsedFeatureRule>();
    }

    public ContributedFeature Feature { get; private set; }

    public IEnumerable<ParsedFeatureRule> OverrideRules { get; private set; }

    public IEnumerable<ParsedFeatureRule> DefaultValueRules { get; private set; }
  }
}
