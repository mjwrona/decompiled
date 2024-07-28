// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageRule
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Constants;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UpdatePackageRule
  {
    private UpdatePackageData m_updatePackageData;
    private string m_name;
    private string m_for;
    private RuleTypeEnum m_type;
    private XmlElement m_ruleElement;

    public UpdatePackageRule(UpdatePackageData sharedData, XmlElement ruleElement)
    {
      this.m_updatePackageData = sharedData;
      this.m_ruleElement = ruleElement;
      string name = ruleElement.Name;
      this.m_for = ruleElement.GetAttribute(ProvisionAttributes.RuleApplyTo);
      if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.HelpTextRule))
        this.m_type = RuleTypeEnum.HelpText;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.RequiredRule))
        this.m_type = RuleTypeEnum.Required;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.ReadOnlyRule))
        this.m_type = RuleTypeEnum.ReadOnly;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.EmptyRule))
        this.m_type = RuleTypeEnum.Empty;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.FrozenRule))
        this.m_type = RuleTypeEnum.Frozen;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.CannotLoseValueRule))
        this.m_type = RuleTypeEnum.CannotLoseValue;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.NotSameAsRule))
        this.m_type = RuleTypeEnum.NotSameAs;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.ValidUserRule))
        this.m_type = RuleTypeEnum.ValidUser;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.MatchRule))
        this.m_type = RuleTypeEnum.Match;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.AllowedValuesRule))
        this.m_type = RuleTypeEnum.AllowedValues;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.SuggestedValuesRule))
        this.m_type = RuleTypeEnum.SuggestedValues;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.ProhibitedValuesRule))
        this.m_type = RuleTypeEnum.ProhibitedValues;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.DefaultRule))
        this.m_type = RuleTypeEnum.Default;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.CopyRule))
        this.m_type = RuleTypeEnum.Copy;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.ServerDefaultRule))
        this.m_type = RuleTypeEnum.ServerDefault;
      else if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.AllowExistingValueRule))
        this.m_type = RuleTypeEnum.AllowExistingValue;
      this.m_name = ruleElement.CloneNode(false).OuterXml;
    }

    internal string Name => this.m_name;

    internal RuleTypeEnum RuleType => this.m_type;

    internal XmlElement RuleElement => this.m_ruleElement;

    internal static bool AreRulesConsistent(
      StringComparer comparer,
      UpdatePackageRule rule1,
      UpdatePackageRule rule2)
    {
      return (rule1.m_for.Length <= 0 || rule2.m_for.Length <= 0 || comparer.Compare(rule1.m_for, rule2.m_for) == 0) && (rule1.m_type != RuleTypeEnum.Required || rule2.m_type != RuleTypeEnum.Empty) && (rule2.m_type != RuleTypeEnum.Required || rule1.m_type != RuleTypeEnum.Empty) && (!rule1.ChangesValue || !rule2.LocksValue) && (!rule1.LocksValue || !rule2.ChangesValue);
    }

    internal void CheckValidity(UpdatePackageField field)
    {
      if (this.m_type == RuleTypeEnum.HelpText)
        return;
      if (!this.IsValidForFieldType(field.FieldType))
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidRuleForFieldType", (object) field.ReferenceName, (object) this.Name, (object) field.StringFieldType));
      if (this.IsValidForField(field))
        return;
      this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidRuleForField", (object) this.Name, (object) field.ReferenceName));
    }

    private bool IsValidForFieldType(int fieldType)
    {
      if (this.m_type == RuleTypeEnum.NotSameAs)
      {
        if (fieldType == 64 || fieldType == 576)
          return false;
      }
      else if (this.m_type == RuleTypeEnum.ValidUser)
      {
        if (fieldType != 16 && fieldType != 24)
          return false;
      }
      else if (this.m_type == RuleTypeEnum.Match)
      {
        if (fieldType != 16)
          return false;
      }
      else if (this.m_type == RuleTypeEnum.AllowedValues || this.m_type == RuleTypeEnum.ProhibitedValues)
      {
        switch (fieldType & 240)
        {
          case 16:
          case 32:
          case 208:
          case 240:
            break;
          default:
            return false;
        }
      }
      else if (this.m_type == RuleTypeEnum.SuggestedValues)
      {
        switch (fieldType & 240)
        {
          case 16:
          case 32:
          case 64:
          case 208:
          case 240:
            break;
          default:
            return false;
        }
      }
      else if (this.m_type == RuleTypeEnum.Copy || this.m_type == RuleTypeEnum.Default || this.m_type == RuleTypeEnum.ServerDefault)
      {
        string attribute = this.m_ruleElement.GetAttribute(ProvisionAttributes.RuleSource);
        if (VssStringComparer.XmlElement.Equals(attribute, ProvisionValues.SourceClock))
        {
          if (fieldType != 48)
            return false;
        }
        else if (VssStringComparer.XmlElement.Equals(attribute, ProvisionValues.SourceCurrentUser) && fieldType != 16 && fieldType != 24)
          return false;
      }
      return true;
    }

    private bool IsValidForField(UpdatePackageField field)
    {
      if (field.ID.IsTemporary)
        return true;
      switch (field.ID.Value)
      {
        case 1:
        case 9:
        case 24:
        case 52:
          return true;
        case 2:
        case 22:
          if (this.m_type != RuleTypeEnum.ReadOnly)
            return false;
          break;
        default:
          using (IEnumerator<string> enumerator = CoreFieldRefNames.All.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              if (this.m_updatePackageData.Metadata.ServerStringComparer.Equals(enumerator.Current, field.ReferenceName))
                return false;
            }
            break;
          }
      }
      return true;
    }

    internal bool ChangesValue => this.m_type == RuleTypeEnum.ServerDefault;

    internal bool LocksValue => this.m_type == RuleTypeEnum.ReadOnly || this.m_type == RuleTypeEnum.Empty;
  }
}
