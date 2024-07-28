// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ParsedFeatureRule
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  internal class ParsedFeatureRule
  {
    private const string c_area = "FeatureManagement";
    private const string c_layer = "ContributedFeatureService";
    private ContributedFeature m_feature;
    private ContributedFeatureValueRule m_rule;
    private IContributedFeatureValuePlugin m_plugin;
    private object m_parsedProperties;
    private Exception m_parseError;
    private bool m_validRule;

    public ParsedFeatureRule(
      IVssRequestContext requestContext,
      ContributedFeature feature,
      ContributedFeatureValueRule rule,
      IDictionary<string, IContributedFeatureValuePlugin> registeredPlugins)
    {
      this.m_feature = feature;
      this.m_rule = rule;
      IContributedFeatureValuePlugin featureValuePlugin;
      if (!registeredPlugins.TryGetValue(rule.Name, out featureValuePlugin))
        return;
      this.m_plugin = featureValuePlugin;
      try
      {
        this.m_parsedProperties = this.m_plugin.ProcessProperties(rule.Properties);
        this.m_validRule = true;
      }
      catch (Exception ex)
      {
        this.m_parseError = ex;
      }
    }

    public ContributedFeatureEnabledValue GetEnabledValueFromPlugin(
      IVssRequestContext requestContext,
      IDictionary<string, string> scopeValues,
      out string reason)
    {
      reason = (string) null;
      if (this.m_validRule)
      {
        try
        {
          return this.m_plugin.ComputeEnabledValue(requestContext, this.m_feature, this.m_parsedProperties, scopeValues, out reason);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10026102, "FeatureManagement", "ContributedFeatureService", ex);
        }
      }
      else if (this.m_parseError != null)
        requestContext.Trace(10026103, TraceLevel.Info, "FeatureManagement", "ContributedFeatureService", "Failed to parse feature rule inputs for \"{0}\" rule: {1}.", (object) this.m_rule.Name, (object) this.m_parseError);
      else if (this.m_plugin == null)
        requestContext.Trace(10026104, TraceLevel.Info, "FeatureManagement", "ContributedFeatureService", "Could not find IContributedFeatureValuePlugin with name \"{0}\".", (object) this.m_rule.Name);
      return ContributedFeatureEnabledValue.Undefined;
    }
  }
}
