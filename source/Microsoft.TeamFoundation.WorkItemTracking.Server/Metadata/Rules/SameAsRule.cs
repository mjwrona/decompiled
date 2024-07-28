// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.SameAsRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("same-as")]
  public class SameAsRule : DependentRule
  {
    private bool m_Inverse;
    private bool m_CheckOriginalValue;

    public SameAsRule()
    {
      this.Name = WorkItemRuleName.OtherField;
      this.Inverse = false;
      this.CheckOriginalValue = false;
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

    [XmlAttribute("original-value")]
    [DefaultValue(false)]
    public bool CheckOriginalValue
    {
      get => this.m_CheckOriginalValue;
      set
      {
        this.m_CheckOriginalValue = value;
        this.ReportRulesModification(nameof (CheckOriginalValue));
      }
    }

    public override bool Equals(WorkItemRule other, bool deep)
    {
      if (!base.Equals(other, deep))
        return false;
      SameAsRule sameAsRule = other as SameAsRule;
      return this.CheckOriginalValue == sameAsRule.CheckOriginalValue && this.Inverse == sameAsRule.Inverse;
    }
  }
}
