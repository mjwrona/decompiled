// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UpdatePackageField
  {
    private UpdatePackageData m_updatePackageData;
    private string m_name;
    private string m_refName;
    private PsFieldDefinitionTypeEnum m_type;
    private string m_typeStr;
    private MetaID m_ID;
    private XmlElement m_fieldElement;
    private UpdatePackageFieldCollection m_fields;
    private int? m_fieldId;
    private bool m_reportingSpecified;
    private bool m_reportable;
    private int m_report;
    private int m_formula;
    private string m_reportingName;
    private string m_reportingRefName;
    private UpdatePackageFlagManager m_flagManager;
    private UpdatePackageField.FieldUpdateType m_updateType;
    private bool m_isAdded;

    public UpdatePackageField(
      UpdatePackageData sharedData,
      UpdatePackageFieldCollection fields,
      XmlElement fieldElement)
    {
      this.m_updatePackageData = sharedData;
      this.m_fields = fields;
      this.m_fieldElement = fieldElement;
      this.m_flagManager = new UpdatePackageFlagManager(fieldElement);
      this.m_name = fieldElement.GetAttribute(ProvisionAttributes.FieldName);
      this.m_refName = fieldElement.GetAttribute(ProvisionAttributes.FieldReferenceName);
      if (!ValidationMethods.IsValidFieldName(this.m_name))
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidFieldName", (object) this.m_name));
      if (!ValidationMethods.IsValidReferenceFieldName(this.m_refName))
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidFieldReferenceName", (object) this.m_refName));
      this.m_typeStr = fieldElement.GetAttribute(ProvisionAttributes.FieldType);
      this.m_type = UpdatePackageField.StringToFieldType(this.m_typeStr);
      if (fieldElement.HasAttribute(ProvisionAttributes.RenameSafe) && XmlConvert.ToBoolean(fieldElement.GetAttribute(ProvisionAttributes.RenameSafe)))
      {
        if (this.m_type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword)
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorRenameSafeNotValid", (object) this.m_refName, (object) ProvisionValues.FieldTypeString, (object) ProvisionAttributes.RenameSafe));
        else
          this.m_type = PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person;
      }
      UpdatePackageField.CheckTypeAvailability(sharedData.Metadata, this.m_type);
      this.m_ID = this.m_updatePackageData.MetaIDFactory.NewMetaID();
      if (this.m_type == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreePath)
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorCannotUseTreePathType", (object) this.m_typeStr, (object) "System.AreaPath", (object) "System.IterationPath"));
      else if (this.m_type == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_History)
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorCannotUseHistoryType", (object) this.m_typeStr, (object) "System.History"));
      this.InitReportability(this.m_fieldElement, out this.m_reportingSpecified, out this.m_report, out this.m_formula, out this.m_reportingName, out this.m_reportingRefName);
      this.m_reportable = this.m_reportingSpecified;
    }

    public UpdatePackageField(
      UpdatePackageData sharedData,
      UpdatePackageFieldCollection fields,
      int fieldId)
    {
      this.m_updatePackageData = sharedData;
      this.m_fields = fields;
      this.m_fieldId = new int?(fieldId);
      this.m_name = this.m_updatePackageData.Metadata.GetFieldName(fieldId);
      this.m_refName = this.m_updatePackageData.Metadata.GetFieldReferenceName(fieldId);
      this.m_ID = this.m_updatePackageData.MetaIDFactory.NewMetaID(fieldId);
      this.m_type = this.m_updatePackageData.Metadata.GetPsFieldType(fieldId);
      this.m_typeStr = UpdatePackageField.IsInternalField(fieldId, this.m_updatePackageData.Metadata) ? string.Empty : UpdatePackageField.FieldTypeToString(this.m_type);
      this.m_report = this.m_updatePackageData.Metadata.GetPsReportingType(fieldId);
      this.m_formula = this.m_updatePackageData.Metadata.GetPsReportingFormula(fieldId);
      this.m_reportable = this.m_updatePackageData.Metadata.IsReportable(fieldId);
    }

    public void AddToUpdatePackage(
      UpdatePackageRuleContext context,
      bool addDefinitionFlag,
      UpdatePackage batch)
    {
      IMetadataProvisioningHelper metadata = this.m_updatePackageData.Metadata;
      if (addDefinitionFlag)
      {
        if (!this.m_fieldId.HasValue)
        {
          if (!this.m_isAdded)
          {
            batch.InsertField(this.ID, this.m_name, this.m_refName, (int) this.m_type, this.m_report, this.m_formula, this.m_reportingName, this.m_reportingRefName);
            this.m_isAdded = true;
          }
        }
        else
        {
          Dictionary<string, object> props = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
          if ((this.m_updateType & UpdatePackageField.FieldUpdateType.DisplayName) == UpdatePackageField.FieldUpdateType.DisplayName && metadata.ServerStringComparer.Compare(this.m_name, metadata.GetFieldName(this.m_fieldId.Value)) != 0)
            props["Name"] = (object) this.m_name;
          if (this.m_reportingSpecified)
          {
            if (metadata.IsReportable(this.m_fieldId.Value) != this.m_reportable)
              props["ReportingEnabled"] = (object) this.m_reportable;
            if (this.m_report != metadata.GetPsReportingType(this.m_fieldId.Value))
              props["ReportingType"] = (object) this.m_report;
            if (this.m_formula != metadata.GetPsReportingFormula(this.m_fieldId.Value))
              props["ReportingFormula"] = (object) this.m_formula;
            if (!string.IsNullOrEmpty(this.m_reportingName) && metadata.ServerStringComparer.Compare(this.m_reportingName, metadata.GetReportingName(this.m_fieldId.Value)) != 0)
              props["ReportingName"] = (object) this.m_reportingName;
            if (!string.IsNullOrEmpty(this.m_reportingRefName) && !TFStringComparer.WorkItemFieldReferenceName.Equals(this.m_reportingRefName, metadata.GetReportingReferenceName(this.m_fieldId.Value)))
              props["ReportingReferenceName"] = (object) this.m_reportingRefName;
          }
          if (props.Count > 0)
            batch.UpdateField(this.m_ID.Value, props);
          if (!this.m_fields.IsCoreField(this.ID.Value))
            batch.InsertFieldUsage(this.ID);
        }
        if (this.m_updatePackageData.CurrentTypeId == null)
          return;
        batch.InsertWorkItemTypeUsage(this.m_updatePackageData.CurrentTypeId, this.m_ID);
      }
      else
      {
        if (this.m_fieldElement == null)
          return;
        this.AddRulesToUpdatePackage(context, (XmlNode) this.m_fieldElement, this.FlagManager.BaseFlags, batch);
      }
    }

    private bool IsReportingChanged
    {
      get
      {
        IMetadataProvisioningHelper metadata = this.m_updatePackageData.Metadata;
        if (!this.m_fieldId.HasValue || !this.m_reportingSpecified)
          return false;
        return metadata.IsReportable(this.m_fieldId.Value) != this.m_reportable || this.m_report != metadata.GetPsReportingType(this.m_fieldId.Value) || this.m_formula != metadata.GetPsReportingFormula(this.m_fieldId.Value) || !metadata.ServerStringComparer.Equals(this.m_reportingName, metadata.GetReportingName(this.m_fieldId.Value)) || !TFStringComparer.WorkItemFieldReferenceName.Equals(this.m_reportingRefName, metadata.GetReportingReferenceName(this.m_fieldId.Value));
      }
    }

    public void AddRulesToUpdatePackage(
      UpdatePackageRuleContext context,
      XmlNode parentNode,
      RuleFlags flags,
      UpdatePackage batch)
    {
      Dictionary<MatchKey, List<string>> matchLists = new Dictionary<MatchKey, List<string>>();
      foreach (XmlNode childNode in parentNode.ChildNodes)
      {
        if (childNode is XmlElement xmlElement)
        {
          string name = xmlElement.Name;
          if (VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenCondition) || VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenNotCondition) || VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenChangedCondition) || VssStringComparer.XmlElement.Equals(name, ProvisionTags.WhenNotChangedCondition))
          {
            context.Push(new UpdatePackageCondition(this.m_updatePackageData, xmlElement, this.m_fields, batch));
            try
            {
              RuleFlags flags1 = flags | UpdatePackageFlagManager.ExtractFlags(xmlElement);
              this.AddRulesToUpdatePackage(context, (XmlNode) xmlElement, flags1, batch);
            }
            finally
            {
              context.Pop();
            }
          }
          else
          {
            UpdatePackageRule importerRule = new UpdatePackageRule(this.m_updatePackageData, xmlElement);
            importerRule.CheckValidity(this);
            switch (importerRule.RuleType)
            {
              case RuleTypeEnum.HelpText:
                this.AddHelpText(context, xmlElement.InnerText, batch);
                continue;
              case RuleTypeEnum.Match:
                this.AddMatchPattern(xmlElement, matchLists);
                continue;
              case RuleTypeEnum.AllowedValues:
                this.AddListRule(context, ListRuleType.AllowedList, xmlElement, flags, batch);
                continue;
              case RuleTypeEnum.SuggestedValues:
                this.AddListRule(context, ListRuleType.SuggestedList, xmlElement, flags, batch);
                continue;
              case RuleTypeEnum.ProhibitedValues:
                this.AddListRule(context, ListRuleType.ProhibitedList, xmlElement, flags, batch);
                continue;
              case RuleTypeEnum.ServerDefault:
                this.AddServerDefaultRule(context, xmlElement, flags, batch);
                continue;
              default:
                this.AddRule(context, importerRule, flags, batch);
                continue;
            }
          }
        }
      }
      this.CreateMatchRules(context, matchLists, flags, batch);
    }

    public string Name
    {
      get => this.m_name;
      set
      {
        if (!ValidationMethods.IsValidFieldName(value))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidFieldName", (object) value));
        this.m_name = value;
      }
    }

    public string ReferenceName => this.m_refName;

    public string ReportingName => this.m_reportingName;

    public string ReportingReferenceName => this.m_reportingRefName;

    public MetaID ID => this.m_ID;

    public bool AcceptRules
    {
      get
      {
        if (this.m_ID.IsTemporary)
          return true;
        return !this.m_updatePackageData.Metadata.IsComputed(this.m_fieldId.Value) && !this.m_updatePackageData.Metadata.IsIgnored(this.m_fieldId.Value) && this.m_ID.Value != 54;
      }
    }

    public bool IsInternal => this.m_fieldId.HasValue && UpdatePackageField.IsInternalField(this.m_fieldId.Value, this.m_updatePackageData.Metadata);

    public static PsFieldDefinitionTypeEnum StringToFieldType(string stringType)
    {
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeString))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeInteger))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeDateTime))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDatetime;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypePlainText))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_PlainText;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeHtml))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_HTML;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeTreePath))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreePath;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeHistory))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_History;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeDouble))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDouble;
      if (VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeGuid))
        return PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedGuid;
      return VssStringComparer.XmlElement.Equals(stringType, ProvisionValues.FieldTypeBoolean) ? PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedBoolean : PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword;
    }

    internal static bool IsInternalFieldType(PsFieldDefinitionTypeEnum psFieldType)
    {
      psFieldType = UpdatePackageField.TranslatePicklistTypesIntoBasicTypes(psFieldType);
      if (psFieldType <= PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedGuid)
      {
        if (psFieldType <= PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger)
        {
          if (psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger)
            goto label_14;
        }
        else if (psFieldType <= PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_PlainText)
        {
          if (psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDatetime && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_PlainText)
            goto label_14;
        }
        else if (psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeTreeNode && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedGuid)
          goto label_14;
      }
      else if (psFieldType <= PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger_TreeID)
      {
        if (psFieldType <= PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDouble)
        {
          if (psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedBoolean && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDouble)
            goto label_14;
        }
        else if (psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreePath && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger_TreeID)
          goto label_14;
      }
      else if (psFieldType <= PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreeNodeName)
      {
        if (psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_History && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreeNodeName)
          goto label_14;
      }
      else if (psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_HTML && psFieldType != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreeNodeType)
        goto label_14;
      return false;
label_14:
      return true;
    }

    internal static bool IsInternalField(int fieldId, IMetadataProvisioningHelper provisioning) => UpdatePackageField.IsInternalFieldType(provisioning.GetPsFieldType(fieldId)) || provisioning.IsIgnored(fieldId);

    internal static string FieldTypeToString(PsFieldDefinitionTypeEnum fieldType)
    {
      fieldType = UpdatePackageField.TranslatePicklistTypesIntoBasicTypes(fieldType);
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

    private void AddHelpText(
      UpdatePackageRuleContext context,
      string helpText,
      UpdatePackage batch)
    {
      MetaID metaId = batch.InsertConstant(this.m_updatePackageData.ProjectGuid, helpText, (MetaID) null);
      if (!metaId.IsTemporary && metaId.Value == -10000)
        return;
      UpdatePackageRuleProperties rule = this.CreateRule(context, (RuleFlags) 0);
      rule[(object) "Helptext"] = (object) true;
      rule[metaId.IsTemporary ? (object) "TempThenConstID" : (object) "ThenConstID"] = (object) metaId.Value;
      batch.InsertComment("Help text rule for {0}", (object) this.m_name);
      batch.InsertRule(this.m_updatePackageData.ProjectId, rule);
    }

    private void AddRule(
      UpdatePackageRuleContext context,
      UpdatePackageRule importerRule,
      RuleFlags flags,
      UpdatePackage batch)
    {
      XmlElement ruleElement = importerRule.RuleElement;
      if (importerRule.RuleType == RuleTypeEnum.AllowExistingValue)
        return;
      context.AddRule(this, importerRule);
      UpdatePackageRuleProperties rule = this.CreateRule(context, flags);
      switch (importerRule.RuleType)
      {
        case RuleTypeEnum.Required:
          batch.InsertComment("Rule: required");
          rule[(object) "DenyWrite"] = (object) true;
          rule[(object) "Unless"] = (object) true;
          rule[(object) "ThenNot"] = (object) true;
          rule[(object) "ThenConstID"] = (object) -10000;
          break;
        case RuleTypeEnum.ReadOnly:
          batch.InsertComment("Rule: read only");
          rule[(object) "DenyWrite"] = (object) true;
          rule[(object) "Unless"] = (object) true;
          rule[(object) "ThenConstID"] = (object) -10001;
          break;
        case RuleTypeEnum.Empty:
          batch.InsertComment("Rule: empty");
          rule[(object) "DenyWrite"] = (object) true;
          rule[(object) "Unless"] = (object) true;
          rule[(object) "ThenConstID"] = (object) -10000;
          break;
        case RuleTypeEnum.Frozen:
          batch.InsertComment("Rule: frozen");
          rule[(object) "DenyWrite"] = (object) true;
          rule[(object) "Unless"] = (object) true;
          rule[(object) "If2Not"] = (object) true;
          rule[(object) "If2ConstID"] = (object) -10000;
          rule[(object) "ThenConstID"] = (object) -10022;
          rule[this.ID.IsTemporary ? (object) "TempIf2FldID" : (object) "If2FldID"] = (object) this.ID.Value;
          break;
        case RuleTypeEnum.CannotLoseValue:
          batch.InsertComment("Rule: cannotlosevalue");
          rule[(object) "DenyWrite"] = (object) true;
          rule[(object) "Unless"] = (object) true;
          string key = this.ID.IsTemporary ? "TempIf2FldID" : "If2FldID";
          rule[(object) key] = (object) this.ID.Value;
          rule[(object) "If2Not"] = (object) true;
          rule[(object) "If2ConstID"] = (object) -10006;
          rule[(object) "ThenNot"] = (object) true;
          rule[(object) "ThenConstID"] = (object) -10000;
          break;
        case RuleTypeEnum.NotSameAs:
          this.SetNotSameAsRule(ruleElement, (Hashtable) rule);
          break;
        case RuleTypeEnum.ValidUser:
          this.SetValidUserRule(ruleElement, (Hashtable) rule, batch);
          break;
        case RuleTypeEnum.Default:
        case RuleTypeEnum.Copy:
          this.SetCopyDefaultRule(ruleElement, context, rule, batch);
          break;
      }
      string attribute1 = ruleElement.GetAttribute(ProvisionAttributes.RuleApplyTo);
      string attribute2 = ruleElement.GetAttribute(ProvisionAttributes.RuleDontApplyTo);
      this.InsertRule(batch, rule, attribute1, attribute2);
    }

    private void SetNotSameAsRule(XmlElement rule, Hashtable props)
    {
      props[(object) "DenyWrite"] = (object) true;
      props[(object) "Unless"] = (object) true;
      props[(object) "ThenConstID"] = (object) -10025;
      props[(object) "ThenNot"] = (object) true;
      string attribute = rule.GetAttribute(ProvisionAttributes.FieldReference);
      UpdatePackageField field = this.m_fields[attribute];
      if (!this.TypesAreEquivalent(field.m_type, this.m_type))
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInconsistentFieldTypes", (object) rule.Name, (object) this.Name, (object) field.Name));
      if (field == this)
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorFieldSelfReference", (object) rule.Name, (object) this.Name));
      MetaID id = this.m_fields[attribute].ID;
      string key = id.IsTemporary ? "TempIf2FldID" : "If2FldID";
      props[(object) key] = (object) id.Value;
      props[(object) "If2ConstID"] = (object) -10025;
    }

    private void SetCopyDefaultRule(
      XmlElement rule,
      UpdatePackageRuleContext context,
      UpdatePackageRuleProperties props,
      UpdatePackage batch)
    {
      string attribute = rule.GetAttribute(ProvisionAttributes.RuleSource);
      MetaID metaId;
      if (VssStringComparer.XmlElement.Equals(attribute, ProvisionValues.SourceValue))
      {
        string constant = this.NormalizeValue(this.GetRequiredAttribute(rule, ProvisionAttributes.RuleValue));
        metaId = batch.InsertConstant(this.m_updatePackageData.ProjectGuid, constant, (MetaID) null);
      }
      else if (VssStringComparer.XmlElement.Equals(attribute, ProvisionValues.SourceField))
      {
        UpdatePackageField field = this.m_fields[this.GetRequiredAttribute(rule, ProvisionAttributes.FieldReference)];
        if (!this.TypesAreEquivalent(field.m_type, this.m_type) && (field.m_type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger_TreeID || this.m_type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInconsistentFieldTypes", (object) rule.Name, (object) this.Name, (object) field.Name));
        metaId = this.m_updatePackageData.MetaIDFactory.NewMetaID(-10012);
        props[field.ID.IsTemporary ? (object) "TempIf2FldID" : (object) "If2FldID"] = (object) field.ID.Value;
        props[(object) "If2ConstID"] = (object) -10012;
      }
      else if (VssStringComparer.XmlElement.Equals(attribute, ProvisionValues.SourceClock))
      {
        this.CheckAppropriateFieldType(rule.Name, PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDatetime);
        metaId = this.m_updatePackageData.MetaIDFactory.NewMetaID(-10028);
      }
      else
      {
        if (!VssStringComparer.XmlElement.Equals(attribute, ProvisionValues.SourceCurrentUser))
          return;
        this.CheckAppropriateFieldType(rule.Name, PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword, PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person);
        metaId = this.m_updatePackageData.MetaIDFactory.NewMetaID(-10002);
      }
      string key = metaId.IsTemporary ? "TempThenConstID" : "ThenConstID";
      props[(object) key] = (object) metaId.Value;
      props[(object) "Default"] = (object) true;
      if (VssStringComparer.XmlElement.Equals(rule.Name, ProvisionTags.DefaultRule))
      {
        context.AddDefaultConditionToEmptySlot(this.ID, props);
        batch.InsertComment("Rule: default (from {0})", (object) attribute);
      }
      else
        batch.InsertComment("Rule: copy (from {0})", (object) attribute);
    }

    private string GetRequiredAttribute(XmlElement element, string attribute)
    {
      if (!element.HasAttribute(attribute))
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorMissingRequiredAttribute", (object) element.Name, (object) attribute));
      return element.GetAttribute(attribute);
    }

    private void SetValidUserRule(XmlElement ruleElement, Hashtable props, UpdatePackage batch)
    {
      props[(object) "DenyWrite"] = (object) true;
      props[(object) "Unless"] = (object) true;
      string attribute = ruleElement.GetAttribute(ProvisionAttributes.GroupReference);
      MetaID metaId = attribute.Length == 0 ? this.m_updatePackageData.MetaIDFactory.NewMetaID(-2) : batch.FindGroup(this.m_updatePackageData.ProjectGuid, attribute);
      props[(object) "ThenImplicitEmpty"] = (object) true;
      props[(object) "ThenLeaf"] = (object) true;
      props[(object) "ThenInterior"] = (object) true;
      props[(object) "ThenOneLevel"] = (object) true;
      props[(object) "ThenTwoPlusLevels"] = (object) true;
      string key = metaId.IsTemporary ? "TempThenConstID" : "ThenConstID";
      props[(object) key] = (object) metaId.Value;
      batch.InsertComment("Rule: valid user");
    }

    private void AddList(
      XmlElement rule,
      bool expandFlag,
      UpdatePackage batch,
      out MetaID listId,
      out MetaID badNamesListId)
    {
      Dictionary<string, bool> dictionary1 = new Dictionary<string, bool>((IEqualityComparer<string>) this.m_updatePackageData.Metadata.ServerStringComparer);
      Dictionary<string, bool> dictionary2 = new Dictionary<string, bool>((IEqualityComparer<string>) this.m_updatePackageData.Metadata.ServerStringComparer);
      string constant1 = Guid.NewGuid().ToString();
      listId = batch.InsertConstant(constant1, (MetaID) null);
      badNamesListId = (MetaID) null;
      foreach (XmlNode childNode in rule.ChildNodes)
      {
        if (childNode is XmlElement xmlElement)
        {
          if (VssStringComparer.XmlElement.Equals(xmlElement.Name, ProvisionTags.GlobalList))
          {
            string attribute = xmlElement.GetAttribute(ProvisionAttributes.GlobalListName);
            if (dictionary1.ContainsKey(attribute))
              this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateGlobalListItem", (object) constant1));
            dictionary1[attribute] = true;
            if (expandFlag)
              this.m_updatePackageData.GlobalLists.CheckItemsValidity(attribute, this);
            string constant2 = "*" + attribute;
            batch.InsertConstant(constant2, listId);
            if (badNamesListId == null)
            {
              string constant3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "-{0}", (object) constant1);
              badNamesListId = batch.InsertConstant(constant3, (MetaID) null);
            }
            batch.InsertConstant(constant2, badNamesListId);
          }
          else
          {
            string str = this.NormalizeValue(xmlElement.GetAttribute(ProvisionAttributes.ListItemValue).Trim());
            if (dictionary2.ContainsKey(str))
              this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateListItem", (object) str));
            dictionary2[str] = true;
            batch.InsertConstant(this.m_updatePackageData.ProjectGuid, str, listId);
          }
        }
      }
    }

    private static PsFieldDefinitionTypeEnum TranslatePicklistTypesIntoBasicTypes(
      PsFieldDefinitionTypeEnum psFieldType)
    {
      return (psFieldType & (PsFieldDefinitionTypeEnum) 1280) != (PsFieldDefinitionTypeEnum) 1280 ? psFieldType : psFieldType & (PsFieldDefinitionTypeEnum) -1281;
    }

    private static int TranslateReportability(string value)
    {
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingDimension))
        return 2;
      return VssStringComparer.XmlElement.Equals(value, ProvisionValues.ReportingMeasure) ? 1 : 3;
    }

    private static int TranslateFormula(string value)
    {
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaSum))
        return 1;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaCount))
        return 2;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaDistinctCount))
        return 3;
      if (VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaAvg))
        return 4;
      return VssStringComparer.XmlElement.Equals(value, ProvisionValues.FormulaMin) ? 5 : 6;
    }

    internal void InitReportability(
      XmlElement fieldElement,
      out bool reportingSpecified,
      out int reportingType,
      out int reportingFormula,
      out string reportingName,
      out string reportingRefName)
    {
      bool flag1 = false;
      reportingName = (string) null;
      reportingRefName = (string) null;
      if (this.m_fieldId.HasValue)
      {
        reportingName = this.m_updatePackageData.Metadata.GetReportingName(this.m_fieldId.Value);
        reportingRefName = this.m_updatePackageData.Metadata.GetReportingReferenceName(this.m_fieldId.Value);
      }
      bool flag2;
      if (fieldElement.HasAttribute(ProvisionAttributes.Reportable))
      {
        if (!UpdatePackageField.IsSupportedReportingFieldType(this.m_type))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorUnsupportedReportingFieldType", (object) this.ReferenceName));
        reportingSpecified = true;
        reportingType = UpdatePackageField.TranslateReportability(fieldElement.GetAttribute(ProvisionAttributes.Reportable));
        if (reportingType == 1)
        {
          reportingFormula = !fieldElement.HasAttribute(ProvisionAttributes.Formula) ? 1 : UpdatePackageField.TranslateFormula(fieldElement.GetAttribute(ProvisionAttributes.Formula));
        }
        else
        {
          reportingFormula = 0;
          flag1 = fieldElement.HasAttribute(ProvisionAttributes.Formula);
        }
        flag2 = true;
      }
      else
      {
        reportingSpecified = false;
        reportingType = 0;
        reportingFormula = 0;
        flag1 = fieldElement.HasAttribute(ProvisionAttributes.Formula);
        flag2 = false;
      }
      if (flag1)
        this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorProhibitedMeasure", (object) ProvisionAttributes.Formula, (object) this.ReferenceName, (object) ProvisionAttributes.Reportable, (object) ProvisionValues.ReportingMeasure));
      if (!flag2)
        return;
      string resourceName = (string) null;
      if (reportingType == 1)
      {
        switch (this.m_type)
        {
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDouble:
            break;
          default:
            resourceName = "ErrorIncompatibleMeasure";
            break;
        }
      }
      else if (reportingType == 3)
      {
        switch (this.m_type)
        {
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDatetime:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDouble:
            break;
          default:
            resourceName = "ErrorIncompatibleDetail";
            break;
        }
      }
      else if (reportingType == 2)
      {
        switch (this.m_type)
        {
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedInteger:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDatetime:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeTreeNode:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedBoolean:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDouble:
          case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_TreePath:
            break;
          default:
            resourceName = "ErrorIncompatibleDimension";
            break;
        }
      }
      if (fieldElement.HasAttribute(ProvisionAttributes.FieldReportingName))
      {
        reportingName = fieldElement.GetAttribute(ProvisionAttributes.FieldReportingName);
        if (!ValidationMethods.IsValidFieldName(reportingName))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidFieldName", (object) reportingName));
      }
      if (fieldElement.HasAttribute(ProvisionAttributes.FieldReportingReferenceName))
      {
        reportingRefName = fieldElement.GetAttribute(ProvisionAttributes.FieldReportingReferenceName);
        if (!ValidationMethods.IsValidReferenceFieldName(reportingRefName))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidFieldReferenceName", (object) reportingRefName));
      }
      if (resourceName == null)
        return;
      this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format(resourceName, (object) this.ReferenceName));
    }

    private static bool IsSupportedReportingFieldType(PsFieldDefinitionTypeEnum type) => type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_PlainText && type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_History && type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_HTML && type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedGuid;

    private UpdatePackageRuleProperties CreateRule(
      UpdatePackageRuleContext context,
      RuleFlags flags)
    {
      UpdatePackageRuleProperties props = new UpdatePackageRuleProperties();
      props[this.ID.IsTemporary ? (object) "TempThenFldID" : (object) "ThenFldID"] = (object) this.ID.Value;
      context.SetProperties(props);
      if ((flags & RuleFlags.AllowExistingValue) != (RuleFlags) 0)
        props[(object) "ThenImplicitUnchanged"] = (object) true;
      return props;
    }

    private void CheckAppropriateFieldType(
      string ruleName,
      params PsFieldDefinitionTypeEnum[] types)
    {
      for (int index = 0; index < types.Length; ++index)
      {
        if (this.TypesAreEquivalent(this.m_type, types[index]))
          return;
      }
      this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorWrongRuleForFieldType", (object) this.Name, (object) ruleName));
    }

    internal string NormalizeValue(string value)
    {
      if (string.IsNullOrEmpty(value))
        return value;
      try
      {
        if (VssStringComparer.XmlElement.Equals(this.m_typeStr, ProvisionValues.FieldTypeInteger))
          return XmlConvert.ToString(XmlConvert.ToInt32(value));
        if (VssStringComparer.XmlElement.Equals(this.m_typeStr, ProvisionValues.FieldTypeDouble))
          return XmlConvert.ToString(XmlConvert.ToDouble(value));
        if (VssStringComparer.XmlElement.Equals(this.m_typeStr, ProvisionValues.FieldTypeDateTime))
          return XmlConvert.ToString(XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Unspecified), XmlDateTimeSerializationMode.Utc);
        if (VssStringComparer.XmlElement.Equals(this.m_typeStr, ProvisionValues.FieldTypeGuid))
          return XmlConvert.ToString(XmlConvert.ToGuid(value));
        return VssStringComparer.XmlElement.Equals(this.m_typeStr, ProvisionValues.FieldTypeBoolean) ? XmlConvert.ToString(XmlConvert.ToBoolean(value.ToLowerInvariant()) ? 1 : 0) : value;
      }
      catch (FormatException ex)
      {
      }
      catch (OverflowException ex)
      {
      }
      this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidListItem", (object) value, (object) this.Name));
      return (string) null;
    }

    private void AddServerDefaultRule(
      UpdatePackageRuleContext context,
      XmlElement rule,
      RuleFlags flags,
      UpdatePackage batch)
    {
      string attribute1 = rule.GetAttribute(ProvisionAttributes.RuleSource);
      string attribute2 = rule.GetAttribute(ProvisionAttributes.RuleApplyTo);
      string attribute3 = rule.GetAttribute(ProvisionAttributes.RuleDontApplyTo);
      int num;
      PsFieldDefinitionTypeEnum[] definitionTypeEnumArray;
      if (VssStringComparer.XmlElement.Equals(attribute1, ProvisionValues.SourceClock))
      {
        num = -10013;
        definitionTypeEnumArray = new PsFieldDefinitionTypeEnum[1]
        {
          PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedDatetime
        };
      }
      else if (VssStringComparer.XmlElement.Equals(attribute1, ProvisionValues.SourceGuid))
      {
        num = -10031;
        definitionTypeEnumArray = new PsFieldDefinitionTypeEnum[1]
        {
          PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedGuid
        };
        if (!this.m_updatePackageData.Metadata.IsSupported("GuidFields"))
          throw new NotSupportedException(InternalsResourceStrings.Get("ErrorGuidFieldsNotSupported"));
      }
      else
      {
        num = -10026;
        definitionTypeEnumArray = new PsFieldDefinitionTypeEnum[1]
        {
          PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword
        };
      }
      this.CheckAppropriateFieldType(rule.Name, definitionTypeEnumArray);
      UpdatePackageRuleProperties rule1 = this.CreateRule(context, flags);
      rule1[(object) "DenyWrite"] = (object) true;
      rule1[(object) "Unless"] = (object) true;
      rule1[(object) "ThenConstID"] = (object) num;
      batch.InsertComment("Rule: SERVERDEFAULT (deny write)");
      this.InsertRule(batch, rule1, attribute2, attribute3);
      rule1.Remove((object) "DenyWrite");
      rule1.Remove((object) "Unless");
      rule1[(object) "Default"] = (object) true;
      batch.InsertComment("Rule: SERVERDEFAULT (default)");
      this.InsertRule(batch, rule1, attribute2, attribute3);
    }

    public void Update(XmlElement fieldElement)
    {
      IMetadataProvisioningHelper metadata = this.m_updatePackageData.Metadata;
      if (this.IsInternal)
      {
        string message = InternalsResourceStrings.Format("ErrorInternalField", (object) this.ReferenceName);
        metadata.ThrowValidationException(message);
      }
      if (this.m_fieldElement != null)
        this.CheckDefinitionConsistency(fieldElement);
      string attribute1 = fieldElement.GetAttribute(ProvisionAttributes.FieldName);
      if (!metadata.ServerStringComparer.Equals(attribute1, this.Name) && string.IsNullOrWhiteSpace(this.m_updatePackageData.MethodologyName))
      {
        string message = InternalsResourceStrings.Format("ErrorCannotRenameField", (object) this.ReferenceName, (object) this.Name, (object) attribute1);
        metadata.ThrowValidationException(message);
      }
      string attribute2 = fieldElement.GetAttribute(ProvisionAttributes.FieldType);
      int fieldType = (int) UpdatePackageField.StringToFieldType(attribute2);
      if (attribute2 != UpdatePackageField.FieldTypeToString(this.m_type) && (!(attribute2 == ProvisionValues.FieldTypeHtml) || this.m_type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_PlainText) && (!(attribute2 == ProvisionValues.FieldTypePlainText) || this.m_type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedLargeText_HTML))
      {
        string message = InternalsResourceStrings.Format("ErrorFieldTypeMismatch", (object) this.ReferenceName, (object) this.StringFieldType, (object) attribute2);
        metadata.ThrowValidationException(message);
      }
      if (fieldElement.HasAttribute(ProvisionAttributes.RenameSafe))
      {
        bool boolean = XmlConvert.ToBoolean(fieldElement.GetAttribute(ProvisionAttributes.RenameSafe));
        if (boolean && this.m_type != PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person || !boolean && this.m_type == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person)
          this.RaiseRenameSafeValidationWarning();
      }
      if (!metadata.IgnoreReportabilityChange)
      {
        this.InitReportability(fieldElement, out this.m_reportingSpecified, out this.m_report, out this.m_formula, out this.m_reportingName, out this.m_reportingRefName);
        if (this.m_reportingSpecified)
        {
          if (metadata.GetPsReportingType(this.m_fieldId.Value) != 0 && (this.m_report != metadata.GetPsReportingType(this.m_fieldId.Value) || this.m_formula != metadata.GetPsReportingFormula(this.m_fieldId.Value) || !metadata.ServerStringComparer.Equals(metadata.GetFieldName(this.m_fieldId.Value), metadata.GetReportingName(this.m_fieldId.Value)) && !metadata.ServerStringComparer.Equals(this.m_reportingName, metadata.GetReportingName(this.m_fieldId.Value)) || !TFStringComparer.WorkItemFieldReferenceName.Equals(metadata.GetFieldReferenceName(this.m_fieldId.Value), metadata.GetReportingReferenceName(this.m_fieldId.Value)) && !TFStringComparer.WorkItemFieldReferenceName.Equals(this.m_reportingRefName, metadata.GetReportingReferenceName(this.m_fieldId.Value))))
          {
            string message = InternalsResourceStrings.Format("ErrorDifferentReportingSettings", (object) this.ReferenceName);
            metadata.ThrowValidationException(message);
          }
          this.m_reportable = this.m_report != 0;
          if (this.IsReportingChanged)
            this.m_updateType |= UpdatePackageField.FieldUpdateType.Reportibility;
        }
      }
      this.m_fieldElement = fieldElement;
      this.m_flagManager = new UpdatePackageFlagManager(fieldElement);
    }

    private void RaiseRenameSafeValidationWarning() => this.m_updatePackageData.Metadata.RaiseImportEvent((Exception) null, InternalsResourceStrings.Format("ErrorRenameSafeChanged", (object) this.m_refName, (object) ProvisionAttributes.RenameSafe));

    internal static string CreateBackupName(string methodologyName, string fieldName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} - {1}", (object) fieldName, (object) UpdatePackageData.GetNormalizedMethodologyName(methodologyName));

    private void CheckDefinitionConsistency(XmlElement fieldElement)
    {
      IMetadataProvisioningHelper metadata = this.m_updatePackageData.Metadata;
      string message1 = InternalsResourceStrings.Format("ErrorDifferentFieldDefinitions", (object) this.ReferenceName);
      if (!metadata.ServerStringComparer.Equals(fieldElement.GetAttribute(ProvisionAttributes.FieldName), this.m_fieldElement.GetAttribute(ProvisionAttributes.FieldName)))
        metadata.ThrowValidationException(message1);
      if (!VssStringComparer.FieldType.Equals(fieldElement.GetAttribute(ProvisionAttributes.FieldType), this.m_fieldElement.GetAttribute(ProvisionAttributes.FieldType)))
        metadata.ThrowValidationException(message1);
      if ((!fieldElement.HasAttribute(ProvisionAttributes.RenameSafe) ? 0 : (XmlConvert.ToBoolean(fieldElement.GetAttribute(ProvisionAttributes.RenameSafe)) ? 1 : 0)) != (!this.m_fieldElement.HasAttribute(ProvisionAttributes.RenameSafe) ? (false ? 1 : 0) : (XmlConvert.ToBoolean(this.m_fieldElement.GetAttribute(ProvisionAttributes.RenameSafe)) ? 1 : 0)))
        this.RaiseRenameSafeValidationWarning();
      if (metadata.IgnoreReportabilityChange)
        return;
      bool reportingSpecified;
      int reportingType;
      int reportingFormula;
      string reportingName;
      string reportingRefName;
      this.InitReportability(fieldElement, out reportingSpecified, out reportingType, out reportingFormula, out reportingName, out reportingRefName);
      if (reportingSpecified != this.m_reportingSpecified || reportingType == this.m_report && reportingFormula == this.m_formula && metadata.ServerStringComparer.Equals(reportingName, this.m_reportingName) && TFStringComparer.WorkItemFieldReferenceName.Equals(reportingRefName, this.m_reportingRefName))
        return;
      string message2 = InternalsResourceStrings.Format("ErrorReportabilityValidationFailed", (object) this.ReferenceName);
      metadata.ThrowValidationException(message2);
    }

    public void UpdateRules(XmlElement fieldElement)
    {
      this.CheckDefinitionConsistency(fieldElement);
      this.m_fieldElement = fieldElement;
      this.m_flagManager = new UpdatePackageFlagManager(fieldElement);
    }

    internal int FieldType => (int) this.m_type;

    internal string StringFieldType => this.m_typeStr;

    internal UpdatePackageFlagManager FlagManager
    {
      get
      {
        if (this.m_flagManager == null)
          this.m_flagManager = new UpdatePackageFlagManager();
        return this.m_flagManager;
      }
    }

    private void InsertRule(
      UpdatePackage batch,
      UpdatePackageRuleProperties props,
      string forGroup,
      string notGroup)
    {
      int num = string.IsNullOrEmpty(forGroup) || string.IsNullOrEmpty(notGroup) ? 1 : 2;
      for (int index = 0; index < num; ++index)
      {
        bool flag = false;
        string constant;
        if (index == 1)
        {
          constant = notGroup;
          flag = true;
        }
        else if (!string.IsNullOrEmpty(forGroup))
          constant = forGroup;
        else if (!string.IsNullOrEmpty(notGroup))
        {
          constant = notGroup;
          flag = true;
        }
        else
          constant = (string) null;
        if (constant != null)
        {
          MetaID userGroup = batch.FindUserGroup(this.m_updatePackageData.ProjectGuid, constant);
          props[userGroup.IsTemporary ? (object) "TempPersonID" : (object) "PersonID"] = (object) userGroup.Value;
          props[(object) "InversePersonID"] = (object) flag;
        }
        batch.InsertRule(this.m_updatePackageData.ProjectId, props);
      }
    }

    private void AddMatchPattern(
      XmlElement matchElement,
      Dictionary<MatchKey, List<string>> matchLists)
    {
      string attribute1 = matchElement.GetAttribute(ProvisionAttributes.RuleApplyTo);
      string attribute2 = matchElement.GetAttribute(ProvisionAttributes.RuleDontApplyTo);
      string attribute3 = matchElement.GetAttribute(ProvisionAttributes.MatchPattern);
      MatchKey key = new MatchKey(this.m_updatePackageData, attribute1, attribute2);
      List<string> stringList;
      if (matchLists.TryGetValue(key, out stringList))
      {
        for (int index = 0; index < stringList.Count; ++index)
        {
          if (this.m_updatePackageData.Metadata.ServerStringComparer.Compare(stringList[index], attribute3) == 0)
            this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicatePattern", (object) attribute3));
        }
      }
      else
      {
        stringList = new List<string>();
        matchLists[key] = stringList;
      }
      stringList.Add(attribute3);
    }

    private void CreateMatchRules(
      UpdatePackageRuleContext context,
      Dictionary<MatchKey, List<string>> matchLists,
      RuleFlags flags,
      UpdatePackage batch)
    {
      foreach (KeyValuePair<MatchKey, List<string>> matchList in matchLists)
      {
        MetaID listID = batch.InsertConstant(Guid.NewGuid().ToString(), (MetaID) null);
        for (int index = 0; index < matchList.Value.Count; ++index)
          batch.InsertConstant(matchList.Value[index], listID);
        MatchKey key = matchList.Key;
        UpdatePackageRuleProperties rule = this.CreateRule(context, flags);
        rule[(object) "DenyWrite"] = (object) true;
        rule[(object) "Unless"] = (object) true;
        rule[(object) "ThenLike"] = (object) true;
        rule[(object) "ThenOneLevel"] = (object) true;
        rule[(object) "ThenLeaf"] = (object) true;
        rule[(object) "ThenImplicitEmpty"] = (object) true;
        if ((flags & RuleFlags.AllowExistingValue) != (RuleFlags) 0)
          rule[(object) "ThenImplicitUnchanged"] = (object) true;
        rule[listID.IsTemporary ? (object) "TempThenConstID" : (object) "ThenConstID"] = (object) listID.Value;
        batch.InsertComment("Rule: match");
        this.InsertRule(batch, rule, key.For, key.Not);
      }
    }

    private void AddListRule(
      UpdatePackageRuleContext context,
      ListRuleType listType,
      XmlElement listElement,
      RuleFlags flags,
      UpdatePackage batch)
    {
      bool expandFlag = !listElement.HasAttribute(ProvisionAttributes.ExpandItems) || XmlConvert.ToBoolean(listElement.GetAttribute(ProvisionAttributes.ExpandItems));
      int num = listElement.HasAttribute(ProvisionAttributes.FilterItems) ? 1 : 0;
      MetaID listId;
      MetaID badNamesListId;
      this.AddList(listElement, expandFlag, batch, out listId, out badNamesListId);
      string attribute1 = listElement.GetAttribute(ProvisionAttributes.RuleApplyTo);
      string attribute2 = listElement.GetAttribute(ProvisionAttributes.RuleDontApplyTo);
      if (listType == ListRuleType.ProhibitedList)
        flags &= ~RuleFlags.AllowExistingValue;
      UpdatePackageRuleProperties rule1 = this.CreateRule(context, flags);
      UpdatePackage.SetListRuleProperties(listType, listId, (Hashtable) rule1);
      if (expandFlag)
        rule1[(object) "ThenTwoPlusLevels"] = (object) true;
      if (num == 0)
        rule1[(object) "ThenInterior"] = (object) true;
      batch.InsertComment("Rule: list");
      this.InsertRule(batch, rule1, attribute1, attribute2);
      if (badNamesListId == null)
        return;
      UpdatePackageRuleProperties rule2 = this.CreateRule(context, flags & ~RuleFlags.AllowExistingValue);
      UpdatePackage.SetListRuleProperties(ListRuleType.ProhibitedList, badNamesListId, (Hashtable) rule2);
      rule2[(object) "ThenInterior"] = (object) true;
      batch.InsertComment("Rule: no global list names in lists!");
      this.InsertRule(batch, rule2, attribute1, attribute2);
    }

    private static void CheckTypeAvailability(
      IMetadataProvisioningHelper pro,
      PsFieldDefinitionTypeEnum type)
    {
      switch (type)
      {
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedGuid:
          if (pro.IsSupported("GuidFields"))
            break;
          throw new NotSupportedException(InternalsResourceStrings.Get("ErrorGuidFieldsNotSupported"));
        case PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedBoolean:
          if (pro.IsSupported("BooleanFields"))
            break;
          throw new NotSupportedException(InternalsResourceStrings.Get("ErrorBoolFieldsNotSupported"));
      }
    }

    private bool TypesAreEquivalent(
      PsFieldDefinitionTypeEnum type1,
      PsFieldDefinitionTypeEnum type2)
    {
      if (type1 == type2 || type1 == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword && type2 == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person)
        return true;
      return type2 == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword && type1 == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person;
    }

    [Flags]
    private enum FieldUpdateType
    {
      None = 0,
      DisplayName = 1,
      Reportibility = 2,
    }
  }
}
