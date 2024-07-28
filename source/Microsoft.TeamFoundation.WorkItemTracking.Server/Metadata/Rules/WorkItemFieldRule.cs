// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.WorkItemFieldRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  [XmlType("field-rules")]
  public class WorkItemFieldRule : RuleBlock
  {
    private string m_Field;
    private int m_FieldInt;

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

    [XmlAttribute("field-id")]
    [DefaultValue(0)]
    public int FieldId
    {
      get => this.m_FieldInt;
      set
      {
        this.m_FieldInt = value;
        this.ReportRulesModification(nameof (FieldId));
      }
    }

    public override void FixFieldReferences(IRuleValidationContext validationHelper)
    {
      base.FixFieldReferences(validationHelper);
      if (string.IsNullOrEmpty(this.Field))
        return;
      int fieldId = validationHelper.GetFieldId(this.Field);
      if (fieldId == 0)
        return;
      this.FieldId = fieldId;
    }

    public override void Validate(IRuleValidationContext validationHelper)
    {
      if (!string.IsNullOrEmpty(this.Field))
      {
        if (!validationHelper.IsValidField(this.Field))
          throw new InvalidRuleException();
      }
      else
      {
        if (this.FieldId == 0)
          throw new InvalidRuleException();
        if (!validationHelper.IsValidField(this.FieldId))
          throw new InvalidRuleException();
      }
      base.Validate(validationHelper);
    }
  }
}
