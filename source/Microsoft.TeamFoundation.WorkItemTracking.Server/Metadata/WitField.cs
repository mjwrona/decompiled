// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WitField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using System;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WitField : IComparable<WitField>
  {
    private int m_id;
    private PsFieldDefinitionTypeEnum m_type;
    private string m_referenceName;
    private XmlElement m_x;

    public WitField(XmlDocument doc, FieldEntry f)
    {
      this.m_id = f.FieldId;
      this.m_type = f.PsFieldType;
      if (f.IsPicklist)
        this.m_type &= (PsFieldDefinitionTypeEnum) 255;
      this.m_referenceName = f.ReferenceName;
      string name = f.Name;
      string reportingName = f.ReportingName;
      string reportingReferenceName = f.ReportingReferenceName;
      string str1 = WitField.TranslateFieldType(this.m_type);
      if (f.IsIgnored || string.IsNullOrEmpty(str1))
        return;
      this.m_x = doc.CreateElement(ProvisionTags.FieldDefinition);
      this.m_x.SetAttribute(ProvisionAttributes.FieldName, name);
      this.m_x.SetAttribute(ProvisionAttributes.FieldReferenceName, this.m_referenceName);
      this.m_x.SetAttribute(ProvisionAttributes.FieldType, str1);
      if (this.m_type == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person)
        this.m_x.SetAttribute(ProvisionAttributes.RenameSafe, XmlConvert.ToString(true));
      if (!f.Flags.HasFlag((Enum) FieldFlags.Reportable) || f.ReportingType == 0)
        return;
      string str2 = WitField.TranslateReportingType(f.ReportingType);
      string.IsNullOrEmpty(str2);
      this.m_x.SetAttribute(ProvisionAttributes.Reportable, str2);
      if (f.ReportingType == 1)
      {
        string str3 = WitField.TranslateReportingFormula(f.ReportingFormula);
        string.IsNullOrEmpty(str3);
        this.m_x.SetAttribute(ProvisionAttributes.Formula, str3);
      }
      if (!string.IsNullOrEmpty(reportingName) && !string.Equals(name, reportingName, StringComparison.Ordinal))
        this.m_x.SetAttribute(ProvisionAttributes.FieldReportingName, reportingName);
      if (string.IsNullOrEmpty(reportingReferenceName) || string.Equals(this.m_referenceName, reportingReferenceName, StringComparison.Ordinal))
        return;
      this.m_x.SetAttribute(ProvisionAttributes.FieldReportingReferenceName, reportingReferenceName);
    }

    public int Id => this.m_id;

    public bool IsIgnored => this.m_x == null;

    public PsFieldDefinitionTypeEnum Type => this.m_type;

    public XmlElement Element => this.m_x;

    public string ReferenceName => this.m_referenceName;

    private static string TranslateFieldType(PsFieldDefinitionTypeEnum fieldType)
    {
      switch (fieldType)
      {
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword:
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person:
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeTreeNode:
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreeNodeName:
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreeNodeType:
          return ProvisionValues.FieldTypeString;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger:
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger_TreeID:
          return ProvisionValues.FieldTypeInteger;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDatetime:
          return ProvisionValues.FieldTypeDateTime;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_PlainText:
          return ProvisionValues.FieldTypePlainText;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedGuid:
          return ProvisionValues.FieldTypeGuid;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedBoolean:
          return ProvisionValues.FieldTypeBoolean;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDouble:
          return ProvisionValues.FieldTypeDouble;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreePath:
          return ProvisionValues.FieldTypeTreePath;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_History:
          return ProvisionValues.FieldTypeHistory;
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_HTML:
          return ProvisionValues.FieldTypeHtml;
        default:
          return string.Empty;
      }
    }

    private static string TranslateReportingType(int reportingType)
    {
      switch (reportingType)
      {
        case 1:
          return ProvisionValues.ReportingMeasure;
        case 2:
          return ProvisionValues.ReportingDimension;
        case 3:
          return ProvisionValues.ReportingDetail;
        default:
          return string.Empty;
      }
    }

    private static string TranslateReportingFormula(int formula)
    {
      switch (formula)
      {
        case 1:
          return ProvisionValues.FormulaSum;
        case 2:
          return ProvisionValues.FormulaCount;
        case 3:
          return ProvisionValues.FormulaDistinctCount;
        case 4:
          return ProvisionValues.FormulaAvg;
        case 5:
          return ProvisionValues.FormulaMin;
        case 6:
          return ProvisionValues.FormulaMax;
        default:
          return string.Empty;
      }
    }

    public int CompareTo(WitField other) => this.Id - other.Id;
  }
}
