// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.WorkItemRelevanceRulesProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions.Relevance;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.RulesProviders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance
{
  public class WorkItemRelevanceRulesProvider : RelevanceRuleProviderBase
  {
    public override IEnumerable<RelevanceRule> GetRelevanceRules(IVssRequestContext requestContext)
    {
      string str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemContract.PlatformFieldNames.AssignedTo, (object) "lower"));
      if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields"))
      {
        if ((bool) requestContext.Items["isUserAnonymousKey"])
          str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemContract.PlatformFieldNames.AssignedToName, (object) "lower"));
        else
          str = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) WorkItemContract.PlatformFieldNames.AssignedToIdentity, (object) "lower"));
      }
      List<RelevanceRule> relevanceRuleList = new List<RelevanceRule>();
      TermBoostRule termBoostRule1 = new TermBoostRule();
      termBoostRule1.Description = "Boost workitem search results assigned to current user";
      termBoostRule1.Target = Target.RootNode;
      termBoostRule1.TermsDescriptor = new List<TermBoostExpression.TermExpression>()
      {
        new TermBoostExpression.TermExpression()
        {
          Terms = new List<string>() { "@current_user" },
          Boost = 2.0,
          FieldName = str
        }
      };
      relevanceRuleList.Add((RelevanceRule) termBoostRule1);
      FunctionDecayRule<DateTime, string> functionDecayRule = new FunctionDecayRule<DateTime, string>();
      functionDecayRule.Description = "Boost newer Workitem search results over the older ones";
      functionDecayRule.Target = Target.RootNode;
      functionDecayRule.Scale = "56d";
      functionDecayRule.Function = DecayFunction.BellCurve;
      functionDecayRule.Field = "fields.date|changeddate|system$";
      relevanceRuleList.Add((RelevanceRule) functionDecayRule);
      List<RelevanceRule> relevanceRules = relevanceRuleList;
      TermBoostRule termBoostRule2 = this.TryAddTermBoostRuleForRecentWorkItemIds(requestContext);
      if (termBoostRule2 != null)
        relevanceRules.Add((RelevanceRule) termBoostRule2);
      TermBoostRule termBoostRule3 = this.TryAddTermBoostRuleForRecentWorkItemAreaIds(requestContext);
      if (termBoostRule3 != null)
        relevanceRules.Add((RelevanceRule) termBoostRule3);
      return (IEnumerable<RelevanceRule>) relevanceRules;
    }

    public TermBoostRule TryAddTermBoostRuleForRecentWorkItemIds(IVssRequestContext requestContext)
    {
      int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/WorkItemSearchBoostOnWorkItemIds", 3);
      List<string> values;
      bool flag = requestContext.RootContext.TryGetItem<List<string>>(Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.RecentWorkItemIds, out values);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081315, "Data Provider", "WorkItemRecentActivityDataProviderService", "Recent Work Items Found:" + flag.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ", List of Work Item Ids:[" + (values == null ? "" : string.Join(",", (IEnumerable<string>) values)) + "], Boost Factor:" + configValueOrDefault.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!flag || values.Count <= 0 || configValueOrDefault <= 0)
        return (TermBoostRule) null;
      TermBoostRule termBoostRule = new TermBoostRule();
      termBoostRule.Description = "Boost search results which were recently accessed by the user based on ID";
      termBoostRule.Target = Target.RootNode;
      termBoostRule.TermsDescriptor = new List<TermBoostExpression.TermExpression>()
      {
        new TermBoostExpression.TermExpression()
        {
          Terms = values,
          Boost = (double) configValueOrDefault,
          FieldName = "fields.int|id|system$"
        }
      };
      return termBoostRule;
    }

    public TermBoostRule TryAddTermBoostRuleForRecentWorkItemAreaIds(
      IVssRequestContext requestContext)
    {
      int configValueOrDefault = requestContext.GetConfigValueOrDefault("/Service/ALMSearch/Settings/WorkItemSearchBoostOnAreaIds", 2);
      List<string> values;
      bool flag = requestContext.RootContext.TryGetItem<List<string>>(Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Constants.WorkItemRecencyData.RecentAreaIds, out values);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081315, "Data Provider", "WorkItemRecentActivityDataProviderService", "Recent Area Ids Found:" + flag.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ", List of Area Ids:[" + (values == null ? "" : string.Join(",", (IEnumerable<string>) values)) + "], Boost Factor:" + configValueOrDefault.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (!flag || values.Count <= 0 || configValueOrDefault <= 0)
        return (TermBoostRule) null;
      TermBoostRule termBoostRule = new TermBoostRule();
      termBoostRule.Description = "Boost search results which were recently accessed by the user based on Area ID";
      termBoostRule.Target = Target.RootNode;
      termBoostRule.TermsDescriptor = new List<TermBoostExpression.TermExpression>()
      {
        new TermBoostExpression.TermExpression()
        {
          Terms = values,
          Boost = (double) configValueOrDefault,
          FieldName = "fields.int|areaid|system$"
        }
      };
      return termBoostRule;
    }
  }
}
