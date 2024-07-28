// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.DependentRule
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
  public abstract class DependentRule : WorkItemRule
  {
    private int m_FieldId;
    private string m_Field;

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
      DependentRule dependentRule = (DependentRule) other;
      return this.FieldId == dependentRule.FieldId && StringComparer.OrdinalIgnoreCase.Equals(this.Field, dependentRule.Field);
    }

    public override void FixFieldReferences(IRuleValidationContext validationHelper)
    {
      base.FixFieldReferences(validationHelper);
      if (string.IsNullOrEmpty(this.Field))
        return;
      this.FieldId = validationHelper.GetFieldId(this.Field);
    }
  }
}
