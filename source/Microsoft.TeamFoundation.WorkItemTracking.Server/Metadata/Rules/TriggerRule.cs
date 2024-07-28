// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.TriggerRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("trigger")]
  public class TriggerRule : WorkItemRule
  {
    private int[] m_FieldIds;

    public TriggerRule() => this.Name = WorkItemRuleName.Trigger;

    [XmlElement("field-id", Type = typeof (int))]
    public int[] FieldIds
    {
      get => this.m_FieldIds;
      set
      {
        this.m_FieldIds = value;
        this.ReportRulesModification(nameof (FieldIds));
      }
    }

    internal override RuleEnginePhase Phase => RuleEnginePhase.CopyRules | RuleEnginePhase.DefaultRules | RuleEnginePhase.OtherRules;

    public override bool Equals(WorkItemRule other, bool deep)
    {
      if (!base.Equals(other, deep))
        return false;
      TriggerRule triggerRule = other as TriggerRule;
      return this.FieldIds != null && ((IEnumerable<int>) this.FieldIds).Any<int>() ? triggerRule.FieldIds != null && ((IEnumerable<int>) triggerRule.FieldIds).Any<int>() && ((IEnumerable<int>) this.FieldIds).SequenceEqual<int>((IEnumerable<int>) triggerRule.FieldIds) : triggerRule.FieldIds == null || !((IEnumerable<int>) triggerRule.FieldIds).Any<int>();
    }
  }
}
