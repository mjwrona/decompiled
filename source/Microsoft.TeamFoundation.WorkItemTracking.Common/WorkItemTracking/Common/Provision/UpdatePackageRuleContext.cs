// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageRuleContext
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UpdatePackageRuleContext
  {
    private IMetadataProvisioningHelper m_metadata;
    private List<UpdatePackageCondition> m_items = new List<UpdatePackageCondition>(5);
    private int m_contextDepth;

    public UpdatePackageRuleContext(IMetadataProvisioningHelper metadata) => this.m_metadata = metadata;

    public void Push(UpdatePackageCondition condition)
    {
      this.m_items.Add(condition);
      if (condition.ConditionType != ConditionType.Context)
        return;
      ++this.m_contextDepth;
    }

    public UpdatePackageCondition Pop()
    {
      int index = this.m_items.Count - 1;
      UpdatePackageCondition packageCondition = this.m_items[index];
      this.m_items.RemoveAt(index);
      if (packageCondition.ConditionType != ConditionType.Context)
        return packageCondition;
      --this.m_contextDepth;
      return packageCondition;
    }

    public void SetProperties(UpdatePackageRuleProperties props)
    {
      int depth = 1;
      for (int index = 0; index < this.m_items.Count; ++index)
      {
        UpdatePackageCondition packageCondition = this.m_items[index];
        if (packageCondition.ConditionType != ConditionType.Transparent)
        {
          packageCondition.SetProperties(depth, props);
          if (packageCondition.ConditionType == ConditionType.Context)
            ++depth;
        }
      }
    }

    public void AddDefaultConditionToEmptySlot(MetaID fieldId, UpdatePackageRuleProperties props)
    {
      string key1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, fieldId.IsTemporary ? "TempFld{0}ID" : "Fld{0}ID", (object) (this.m_contextDepth + 1));
      string key2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Fld{0}IsConstID", (object) (this.m_contextDepth + 1));
      props[(object) key1] = (object) fieldId.Value;
      props[(object) key2] = (object) -10000;
    }

    public int Count => this.m_items.Count;

    public void AddRule(UpdatePackageField field, UpdatePackageRule rule)
    {
      List<UpdatePackageRule> updatePackageRuleList;
      for (int index1 = this.m_items.Count - 1; index1 >= 0; --index1)
      {
        if (this.m_items[index1].FieldRules.TryGetValue(field.ID, out updatePackageRuleList))
        {
          for (int index2 = 0; index2 < updatePackageRuleList.Count; ++index2)
          {
            UpdatePackageRule rule2 = updatePackageRuleList[index2];
            if (!UpdatePackageRule.AreRulesConsistent(this.m_metadata.ServerStringComparer, rule, rule2))
              this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInconsistentRules", (object) field.Name, (object) rule.Name, (object) rule2.Name));
          }
        }
      }
      Dictionary<MetaID, List<UpdatePackageRule>> fieldRules = this.m_items[this.m_items.Count - 1].FieldRules;
      if (!fieldRules.TryGetValue(field.ID, out updatePackageRuleList))
      {
        updatePackageRuleList = new List<UpdatePackageRule>();
        fieldRules[field.ID] = updatePackageRuleList;
      }
      updatePackageRuleList.Add(rule);
    }
  }
}
