// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.MapRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("map")]
  public class MapRule : DependentRule
  {
    private bool m_Inverse;

    public MapRule() => this.Name = WorkItemRuleName.Map;

    [XmlAttribute("inverse")]
    public bool Inverse
    {
      get => this.m_Inverse;
      set
      {
        this.m_Inverse = value;
        this.ReportRulesModification(nameof (Inverse));
      }
    }

    internal override RuleEnginePhase Phase => RuleEnginePhase.CopyRules;

    [XmlElement("case", typeof (MapCase))]
    public MapCase[] Cases { get; set; }

    [XmlElement("else", typeof (MapValues))]
    public MapValues Else { get; set; }

    public override bool Equals(WorkItemRule other, bool deep)
    {
      if (base.Equals(other, deep))
      {
        MapRule mapRule = (MapRule) other;
        if (this.Inverse != mapRule.Inverse)
          return false;
        if (this.Else != null)
        {
          if (!this.Else.Equals(mapRule.Else))
            return false;
        }
        else if (mapRule.Else != null)
          return false;
        if (this.Cases != null && ((IEnumerable<MapCase>) this.Cases).Any<MapCase>())
          return mapRule.Cases != null && ((IEnumerable<MapCase>) mapRule.Cases).Any<MapCase>() && ((IEnumerable<MapCase>) this.Cases).OrderBy<MapCase, string>((Func<MapCase, string>) (c => c.Value), (IComparer<string>) StringComparer.OrdinalIgnoreCase).SequenceEqual<MapCase>((IEnumerable<MapCase>) ((IEnumerable<MapCase>) mapRule.Cases).OrderBy<MapCase, string>((Func<MapCase, string>) (c => c.Value), (IComparer<string>) StringComparer.OrdinalIgnoreCase));
        if (mapRule.Cases != null)
        {
          ((IEnumerable<MapCase>) mapRule.Cases).Any<MapCase>();
          return false;
        }
      }
      return false;
    }

    public override void Validate(IRuleValidationContext validationHelper)
    {
      base.Validate(validationHelper);
      if (this.Cases != null)
      {
        foreach (MapValues mapValues in this.Cases)
          mapValues.Validate(validationHelper);
        if (((IEnumerable<MapCase>) this.Cases).Select<MapCase, string>((Func<MapCase, string>) (c => c.Value)).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count<string>() != this.Cases.Length)
          throw new InvalidRuleException("Duplicate case values!");
      }
      if (this.Else == null)
        return;
      this.Else.Validate(validationHelper);
    }

    internal override IEnumerable<string> ExtractConstants()
    {
      IEnumerable<string>[] stringsArray = new IEnumerable<string>[6];
      MapCase[] cases1 = this.Cases;
      stringsArray[0] = cases1 != null ? ((IEnumerable<MapCase>) cases1).Select<MapCase, string>((Func<MapCase, string>) (c => c?.Value)) : (IEnumerable<string>) null;
      MapCase[] cases2 = this.Cases;
      stringsArray[1] = cases2 != null ? ((IEnumerable<MapCase>) cases2).Where<MapCase>((Func<MapCase, bool>) (c => c?.Values != null)).SelectMany<MapCase, string>((Func<MapCase, IEnumerable<string>>) (c => c == null ? (IEnumerable<string>) null : (IEnumerable<string>) c.Values)) : (IEnumerable<string>) null;
      MapCase[] cases3 = this.Cases;
      stringsArray[2] = cases3 != null ? ((IEnumerable<MapCase>) cases3).Select<MapCase, string>((Func<MapCase, string>) (c => c?.Default)) : (IEnumerable<string>) null;
      stringsArray[3] = (IEnumerable<string>) this.Else?.Values;
      string[] strArray;
      if (this.Else?.Default == null)
        strArray = (string[]) null;
      else
        strArray = new string[1]{ this.Else.Default };
      stringsArray[4] = (IEnumerable<string>) strArray;
      stringsArray[5] = base.ExtractConstants();
      return this.GetNonEmptyConstants(stringsArray);
    }
  }
}
