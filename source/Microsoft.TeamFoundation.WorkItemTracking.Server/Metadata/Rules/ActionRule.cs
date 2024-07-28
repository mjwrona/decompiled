// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.ActionRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public abstract class ActionRule : WorkItemRule
  {
    private string m_Value;
    private RuleValueFrom m_ValueFrom;

    public ActionRule() => this.ValueFrom = RuleValueFrom.Value;

    [XmlAttribute("value")]
    public string Value
    {
      get => this.m_Value;
      set
      {
        this.m_Value = value;
        this.ReportRulesModification(nameof (Value));
      }
    }

    [XmlAttribute("from")]
    [DefaultValue(RuleValueFrom.Value)]
    public RuleValueFrom ValueFrom
    {
      get => this.m_ValueFrom;
      set
      {
        this.m_ValueFrom = value;
        this.ReportRulesModification(nameof (ValueFrom));
      }
    }

    public override IEnumerable<RuleFieldDependency> GetDependencies()
    {
      IEnumerable<RuleFieldDependency> first = base.GetDependencies();
      switch (this.ValueFrom)
      {
        case RuleValueFrom.OtherFieldCurrentValue:
        case RuleValueFrom.OtherFieldOriginalValue:
          int result = 0;
          string str = (string) null;
          if (!int.TryParse(this.Value, out result))
            str = this.Value;
          first = first.Concat<RuleFieldDependency>((IEnumerable<RuleFieldDependency>) new RuleFieldDependency[1]
          {
            new RuleFieldDependency()
            {
              FieldId = result,
              FieldReferenceName = str
            }
          });
          break;
      }
      return first;
    }

    public override void FixFieldReferences(IRuleValidationContext validationHelper)
    {
      base.FixFieldReferences(validationHelper);
      switch (this.ValueFrom)
      {
        case RuleValueFrom.OtherFieldCurrentValue:
        case RuleValueFrom.OtherFieldOriginalValue:
          int result = 0;
          if (int.TryParse(this.Value, out result))
            break;
          string fieldReferenceName = this.Value;
          this.Value = validationHelper.GetFieldId(fieldReferenceName).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
          break;
      }
    }

    internal override IEnumerable<string> ExtractConstants()
    {
      IEnumerable<string>[] stringsArray = new IEnumerable<string>[2];
      string[] strArray;
      if (this.ValueFrom != RuleValueFrom.Value)
        strArray = (string[]) null;
      else
        strArray = new string[1]{ this.Value };
      stringsArray[0] = (IEnumerable<string>) strArray;
      stringsArray[1] = base.ExtractConstants();
      return this.GetNonEmptyConstants(stringsArray);
    }
  }
}
