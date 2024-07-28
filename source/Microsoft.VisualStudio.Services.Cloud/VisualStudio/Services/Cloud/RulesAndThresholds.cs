// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RulesAndThresholds
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class RulesAndThresholds
  {
    public int Version;
    private Dictionary<string, IEnumerable<string>> Macros;
    private List<RURule> RawRules;
    private List<RUThreshold> RawThresholds;
    public Dictionary<Guid, ProcessedRURule> ProcessedRules;
    internal static readonly Guid s_ruleIdNamespace = new Guid("{1203A0A8-803F-4C35-932C-E62EC5F79811}");

    public RulesAndThresholds(IVssRequestContext requestContext, int version)
    {
      this.Version = version;
      if (this.Version < 0)
      {
        this.ProcessedRules = new Dictionary<Guid, ProcessedRURule>();
      }
      else
      {
        List<RUMacro> macros;
        using (ResourceUtilizationComponent component = requestContext.CreateComponent<ResourceUtilizationComponent>())
          this.RawRules = component.ReadRURulesAndThresholds(out macros, out this.RawThresholds);
        this.Macros = macros.ToDictionary<RUMacro, string, IEnumerable<string>>((Func<RUMacro, string>) (ruMacro => ruMacro.MacroName), (Func<RUMacro, IEnumerable<string>>) (ruMacro => ((IEnumerable<string>) ruMacro.MacroDefinition.Split(';')).Where<string>((Func<string, bool>) (x => true))));
      }
    }

    public RulesAndThresholds(RulesAndThresholds other)
    {
      this.Version = other.Version;
      this.Macros = other.Macros;
      this.RawRules = other.RawRules;
      this.RawThresholds = other.RawThresholds;
    }

    internal void Compile(
      IVssRequestContext requestContext,
      RulesAndThresholds parentRulesAndThresholds,
      ResourceUtilization2Service.ServiceSettings ruSettings)
    {
      this.ProcessedRules = parentRulesAndThresholds == null ? new Dictionary<Guid, ProcessedRURule>() : parentRulesAndThresholds.ProcessedRules.ToDictionary<KeyValuePair<Guid, ProcessedRURule>, Guid, ProcessedRURule>((Func<KeyValuePair<Guid, ProcessedRURule>, Guid>) (entry => entry.Key), (Func<KeyValuePair<Guid, ProcessedRURule>, ProcessedRURule>) (entry => new ProcessedRURule(entry.Value)));
      foreach (RURule rawRule in this.RawRules)
      {
        ProcessedRURule processedRuRule = new ProcessedRURule();
        processedRuRule.Compile(requestContext, this.Macros, rawRule);
        if (processedRuRule.Resource != null)
          this.ProcessedRules[GuidUtils.GenerateGuidFromString(RulesAndThresholds.s_ruleIdNamespace, rawRule.RuleName.ToLowerInvariant())] = processedRuRule;
        else
          requestContext.Trace(522304009, TraceLevel.Error, "ResourceUtilizationService", "Service", "Couldn't parse rule: " + rawRule.RuleName + " (" + rawRule.RuleDefinition + ")");
      }
      this.LoopOverThresholds(requestContext, this.RawThresholds.Where<RUThreshold>((Func<RUThreshold, bool>) (t => string.IsNullOrEmpty(t.Entity))), (Action<ProcessedRURule, RUThreshold, ProcessedRUThreshold>) ((rule, newRawThreshold, preexistingProcessedThreshold) => preexistingProcessedThreshold.OverrideThresholds(newRawThreshold, ruSettings.DPSignificanceThresholdFraction)), ruSettings.DPSignificanceThresholdFraction);
      this.LoopOverThresholds(requestContext, this.RawThresholds.Where<RUThreshold>((Func<RUThreshold, bool>) (t => !string.IsNullOrEmpty(t.Entity))), (Action<ProcessedRURule, RUThreshold, ProcessedRUThreshold>) ((rule, newRawThreshold, preexistingProcessedThreshold) =>
      {
        rule.Thresholds[newRawThreshold.Entity] = new ProcessedRUThreshold(preexistingProcessedThreshold);
        rule.Thresholds[newRawThreshold.Entity].OverrideThresholds(newRawThreshold, ruSettings.DPSignificanceThresholdFraction);
      }), ruSettings.DPSignificanceThresholdFraction);
    }

    private void LoopOverThresholds(
      IVssRequestContext requestContext,
      IEnumerable<RUThreshold> rawThresholds,
      Action<ProcessedRURule, RUThreshold, ProcessedRUThreshold> overrideThreshold,
      double dPSignificanceThresholdFraction)
    {
      foreach (RUThreshold rawThreshold in rawThresholds)
      {
        ProcessedRURule processedRuRule;
        if (!this.ProcessedRules.TryGetValue(GuidUtils.GenerateGuidFromString(RulesAndThresholds.s_ruleIdNamespace, rawThreshold.RuleName.ToLowerInvariant()), out processedRuRule))
        {
          requestContext.Trace(522304006, TraceLevel.Error, "ResourceUtilizationService", "Service", "Threshold specified with no corresponding rule (" + rawThreshold.RuleName + ")");
        }
        else
        {
          ProcessedRUThreshold processedRuThreshold;
          if (processedRuRule.Thresholds.TryGetValue(string.Empty, out processedRuThreshold))
            overrideThreshold(processedRuRule, rawThreshold, processedRuThreshold);
          else
            processedRuRule.Thresholds[rawThreshold.Entity ?? string.Empty] = new ProcessedRUThreshold(rawThreshold, dPSignificanceThresholdFraction);
        }
      }
    }
  }
}
