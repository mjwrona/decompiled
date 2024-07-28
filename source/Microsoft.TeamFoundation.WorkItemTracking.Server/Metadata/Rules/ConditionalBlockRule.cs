// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.ConditionalBlockRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public abstract class ConditionalBlockRule : RuleBlock
  {
    private bool m_Inverse;
    private int m_FieldId;
    private string m_Field;
    private string m_Value;
    private RuleValueFrom m_ValueFrom;

    public ConditionalBlockRule()
    {
      this.Inverse = false;
      this.ValueFrom = RuleValueFrom.Value;
    }

    [XmlAttribute("inverse")]
    [DefaultValue(false)]
    public bool Inverse
    {
      get => this.m_Inverse;
      set
      {
        this.m_Inverse = value;
        this.ReportRulesModification(nameof (Inverse));
      }
    }

    [XmlAttribute("field-id")]
    [DefaultValue(0)]
    public int FieldId
    {
      get => this.m_FieldId;
      set
      {
        this.m_FieldId = value;
        this.ReportRulesModification(nameof (FieldId));
      }
    }

    [XmlAttribute("field")]
    public string Field
    {
      get => this.m_Field;
      set
      {
        this.m_Field = value;
        this.ReportRulesModification(nameof (Field));
      }
    }

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

    public override IEnumerable<RuleFieldDependency> GetDependencies() => base.GetDependencies().Concat<RuleFieldDependency>((IEnumerable<RuleFieldDependency>) new RuleFieldDependency[1]
    {
      new RuleFieldDependency()
      {
        FieldId = this.FieldId,
        FieldReferenceName = this.Field
      }
    });

    public override bool Equals(WorkItemRule other, bool deep)
    {
      if (!base.Equals(other, deep))
        return false;
      ConditionalBlockRule conditionalBlockRule = other as ConditionalBlockRule;
      return this.Inverse == conditionalBlockRule.Inverse && this.FieldId == conditionalBlockRule.FieldId && StringComparer.OrdinalIgnoreCase.Equals(this.Field, conditionalBlockRule.Field) && this.ValueFrom == conditionalBlockRule.ValueFrom && StringComparer.OrdinalIgnoreCase.Equals(this.Value, conditionalBlockRule.Value);
    }

    public override void FixFieldReferences(IRuleValidationContext validationHelper)
    {
      base.FixFieldReferences(validationHelper);
      if (string.IsNullOrEmpty(this.Field))
        return;
      this.FieldId = validationHelper.GetFieldId(this.Field);
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
