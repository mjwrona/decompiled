// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageCondition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UpdatePackageCondition
  {
    private UpdatePackageData m_sharedData;
    private MetaID m_fieldId;
    private MetaID m_fromId;
    private MetaID m_toId;
    private bool m_invertedFlag;
    private Dictionary<MetaID, List<UpdatePackageRule>> m_fieldRules = new Dictionary<MetaID, List<UpdatePackageRule>>();
    private ConditionType m_type;

    public UpdatePackageCondition(
      UpdatePackageData sharedData,
      XmlElement conditionElement,
      UpdatePackageFieldCollection fields,
      UpdatePackage batch)
    {
      this.m_sharedData = sharedData;
      string attribute1 = conditionElement.GetAttribute(ProvisionAttributes.FieldReference);
      UpdatePackageField field = fields[attribute1];
      this.m_fieldId = field.ID;
      string name = conditionElement.Name;
      this.m_invertedFlag = VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenNotCondition) || VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenChangedCondition);
      if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenChangedCondition) || VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenNotChangedCondition))
      {
        this.m_toId = this.m_sharedData.MetaIDFactory.NewMetaID(-10001);
      }
      else
      {
        string attribute2 = conditionElement.GetAttribute(ProvisionAttributes.RuleValue);
        string constant = field.NormalizeValue(attribute2);
        this.m_toId = batch.InsertConstant(this.m_sharedData.ProjectGuid, constant, (MetaID) null);
      }
      this.m_fromId = this.m_toId;
      this.m_type = ConditionType.Condition;
    }

    public UpdatePackageCondition(UpdatePackageData sharedData, MetaID fieldId, MetaID valueId)
    {
      this.m_sharedData = sharedData;
      this.m_fieldId = fieldId;
      this.m_fromId = valueId;
      this.m_toId = valueId;
      this.m_type = ConditionType.Context;
    }

    public UpdatePackageCondition(MetaID fieldId, MetaID fromId, MetaID toId)
    {
      this.m_fieldId = fieldId;
      this.m_fromId = fromId;
      this.m_toId = toId;
      this.m_type = ConditionType.Context;
    }

    public UpdatePackageCondition() => this.m_type = ConditionType.Transparent;

    public void SetProperties(int depth, UpdatePackageRuleProperties props)
    {
      if (this.m_type == ConditionType.Context)
      {
        string key1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_fieldId.IsTemporary ? "TempFld{0}ID" : "Fld{0}ID", (object) depth);
        props[(object) key1] = (object) this.m_fieldId.Value;
        if (this.m_fromId != this.m_toId)
        {
          string key2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_fromId.IsTemporary ? "TempFld{0}WasConstID" : "Fld{0}WasConstID", (object) depth);
          props[(object) key2] = (object) this.m_fromId.Value;
        }
        string key3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, this.m_toId.IsTemporary ? "TempFld{0}IsConstID" : "Fld{0}IsConstID", (object) depth);
        props[(object) key3] = (object) this.m_toId.Value;
      }
      else
      {
        string key4 = this.m_fieldId.IsTemporary ? "TempIfFldID" : "IfFldID";
        string key5 = this.m_toId.IsTemporary ? "TempIfConstID" : "IfConstID";
        props[(object) key4] = (object) this.m_fieldId.Value;
        props[(object) key5] = (object) this.m_toId.Value;
        if (!this.m_invertedFlag)
          return;
        props[(object) "IfNot"] = (object) true;
      }
    }

    public ConditionType ConditionType => this.m_type;

    public void MakeTransparent() => this.m_type = ConditionType.Transparent;

    public Dictionary<MetaID, List<UpdatePackageRule>> FieldRules => this.m_fieldRules;
  }
}
