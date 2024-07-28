// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ListRuleResolverState
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ListRuleResolverState
  {
    private readonly Dictionary<ListRuleResolverType, List<ListRule>> m_rulesNeedMoreResolving = new Dictionary<ListRuleResolverType, List<ListRule>>();

    public IEnumerable<WorkItemFieldRule> FieldRules { get; private set; }

    public ListRuleResolverState(IEnumerable<WorkItemFieldRule> fieldRules) => this.FieldRules = fieldRules != null ? fieldRules : throw new ArgumentNullException(nameof (fieldRules));

    public IEnumerable<ListRuleResolverState.FieldListRule> IdentifyAllFieldListRules(
      IVssRequestContext context)
    {
      IFieldTypeDictionary fieldTypeDictionary = context.WitContext().FieldDictionary;
      return this.FieldRules.Select(fieldRule => new
      {
        fieldRule = fieldRule,
        field = fieldTypeDictionary.GetFieldOrNull(fieldRule.FieldId)
      }).SelectMany(_param1 => _param1.fieldRule.SelectRules<ListRule>(), (_param1, listRule) => new
      {
        \u003C\u003Eh__TransparentIdentifier0 = _param1,
        listRule = listRule
      }).Where(_param1 => _param1.listRule.Sets != null && _param1.listRule.Sets.Length != 0).Select(_param1 => new ListRuleResolverState.FieldListRule()
      {
        Rule = _param1.listRule,
        ForIdentityField = _param1.\u003C\u003Eh__TransparentIdentifier0.field != null && _param1.\u003C\u003Eh__TransparentIdentifier0.field.IsIdentity,
        FieldId = _param1.\u003C\u003Eh__TransparentIdentifier0.field.FieldId
      });
    }

    public void MarkRuleForFutureProcessing(ListRuleResolverType type, ListRule rule)
    {
      if (rule == null)
        return;
      if (!this.m_rulesNeedMoreResolving.ContainsKey(type))
        this.m_rulesNeedMoreResolving.Add(type, new List<ListRule>());
      this.m_rulesNeedMoreResolving[type].Add(rule);
    }

    public bool TryGetRules(ListRuleResolverType resolvingType, out List<ListRule> rule)
    {
      if (this.m_rulesNeedMoreResolving.ContainsKey(resolvingType))
      {
        rule = this.m_rulesNeedMoreResolving[resolvingType];
        return rule != null;
      }
      rule = (List<ListRule>) null;
      return false;
    }

    public struct FieldListRule
    {
      public ListRule Rule;
      public bool ForIdentityField;
      public int FieldId;
    }
  }
}
