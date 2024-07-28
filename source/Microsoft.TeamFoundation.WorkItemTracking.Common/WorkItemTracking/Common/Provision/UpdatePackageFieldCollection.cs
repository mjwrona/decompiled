// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageFieldCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UpdatePackageFieldCollection
  {
    private UpdatePackageData m_updatePackageData;
    private Dictionary<string, UpdatePackageField> m_fieldsMap;
    private List<UpdatePackageField> m_fieldsList;
    private Dictionary<string, UpdatePackageField> m_newFields;
    private Dictionary<string, UpdatePackageField> m_refnameMap;
    private Dictionary<string, UpdatePackageField> m_nameMap;
    private List<UpdatePackageField> m_coreFields;
    private HashSet<int> m_coreFieldIds;
    private HashSet<string> m_linkTypesRefName;
    internal const int c_maxFieldsCount = 1047;
    private int m_remainingFieldsCount;

    public UpdatePackageFieldCollection(UpdatePackageData sharedData)
    {
      this.m_updatePackageData = sharedData;
      List<int> fields = this.m_updatePackageData.Metadata.GetFields();
      this.m_coreFields = new List<UpdatePackageField>(20);
      this.m_refnameMap = new Dictionary<string, UpdatePackageField>(fields.Count, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      this.m_nameMap = new Dictionary<string, UpdatePackageField>(fields.Count, (IEqualityComparer<string>) this.m_updatePackageData.Metadata.ServerStringComparer);
      for (int index = 0; index < fields.Count; ++index)
      {
        UpdatePackageField updatePackageField = new UpdatePackageField(this.m_updatePackageData, this, fields[index]);
        this.m_refnameMap[updatePackageField.ReferenceName] = updatePackageField;
        this.m_nameMap[updatePackageField.Name] = updatePackageField;
        if (this.IsCoreField(fields[index]))
          this.m_coreFields.Add(updatePackageField);
      }
      if (!this.m_updatePackageData.Metadata.IsSupported("GuidFields"))
        this.m_remainingFieldsCount = 1047 - fields.Count;
      this.m_newFields = new Dictionary<string, UpdatePackageField>((IEqualityComparer<string>) this.m_updatePackageData.Metadata.ServerStringComparer);
      this.m_linkTypesRefName = new HashSet<string>((IEnumerable<string>) this.m_updatePackageData.Metadata.GetWorkItemLinkTypeReferenceNames(), (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
    }

    public void ProcessFieldDefinitions(
      UpdatePackageRuleContext context,
      XmlElement fieldsNode,
      UpdatePackage batch,
      bool provisionRules = true,
      bool provisionFields = true)
    {
      if (this.m_fieldsList != null)
      {
        foreach (UpdatePackageField fields in this.m_fieldsList)
        {
          if (!this.m_newFields.ContainsKey(fields.ReferenceName))
            this.m_newFields.Add(fields.ReferenceName, fields);
        }
      }
      this.m_fieldsMap = new Dictionary<string, UpdatePackageField>(200, (IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      this.m_fieldsList = new List<UpdatePackageField>(fieldsNode.ChildNodes.Count);
      Dictionary<string, string> dictionary = new Dictionary<string, string>(fieldsNode.ChildNodes.Count, (IEqualityComparer<string>) this.m_updatePackageData.Metadata.ServerStringComparer);
      int num = 0;
      foreach (XmlElement childNode in fieldsNode.ChildNodes)
      {
        string attribute1 = childNode.GetAttribute(ProvisionAttributes.FieldReferenceName);
        string attribute2 = childNode.GetAttribute(ProvisionAttributes.FieldName);
        if (this.m_fieldsMap.ContainsKey(attribute1))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateFieldDefinition", (object) attribute1));
        string str;
        if (dictionary.TryGetValue(attribute2, out str))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateFieldName", (object) attribute2, (object) str, (object) attribute1));
        dictionary[attribute2] = attribute1;
        if (this.m_linkTypesRefName.Contains(attribute1))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("FieldLinkTypeRefNameConflict", (object) attribute1));
        UpdatePackageField field;
        if (this.m_refnameMap.TryGetValue(attribute1, out field))
          field.Update(childNode);
        else if (this.m_newFields.TryGetValue(attribute1, out field))
        {
          field.UpdateRules(childNode);
        }
        else
        {
          ++num;
          field = new UpdatePackageField(this.m_updatePackageData, this, childNode);
          if (!ValidationMethods.IsValidReferenceFieldNameForImport(attribute1))
            this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidFieldReferenceName", (object) attribute1));
          this.CheckFieldNameUniqueness(field, this.m_updatePackageData.Metadata.UseStrictFieldNameCheck);
        }
        this.m_fieldsMap[attribute1] = field;
        this.m_fieldsList.Add(field);
      }
      if (!this.m_updatePackageData.Metadata.IsSupported("GuidFields"))
      {
        if (num > this.m_remainingFieldsCount)
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorNotEnoughFields", (object) num, (object) this.m_remainingFieldsCount));
        this.m_remainingFieldsCount -= num;
      }
      for (int index = 0; index < this.m_coreFields.Count; ++index)
      {
        UpdatePackageField coreField = this.m_coreFields[index];
        if (!this.m_fieldsMap.ContainsKey(coreField.ReferenceName))
          this.m_fieldsMap[coreField.ReferenceName] = coreField;
      }
      Dictionary<string, string> knownNames1 = new Dictionary<string, string>((IEqualityComparer<string>) this.m_updatePackageData.Metadata.ServerStringComparer);
      Dictionary<string, string> knownNames2 = new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      foreach (UpdatePackageField updatePackageField in this.m_fieldsMap.Values)
      {
        string name1 = updatePackageField.ReportingName;
        if (string.IsNullOrEmpty(name1))
          name1 = updatePackageField.Name;
        UpdatePackageFieldCollection.ValidateDuplicateReportingName(this.m_updatePackageData.Metadata, knownNames1, updatePackageField.ReferenceName, name1);
        string name2 = updatePackageField.ReportingReferenceName;
        if (string.IsNullOrEmpty(name2))
          name2 = updatePackageField.ReferenceName;
        UpdatePackageFieldCollection.ValidateDuplicateReportingName(this.m_updatePackageData.Metadata, knownNames2, updatePackageField.ReferenceName, name2);
      }
      this.AddToUpdatePackage(context, batch, provisionRules, provisionFields);
    }

    internal void CheckFieldNameUniqueness(UpdatePackageField field, bool useStrictFieldNameCheck)
    {
      UpdatePackageField updatePackageField1;
      if (this.m_nameMap.TryGetValue(field.Name, out updatePackageField1))
      {
        if (!useStrictFieldNameCheck && !string.IsNullOrWhiteSpace(this.m_updatePackageData.MethodologyName))
        {
          string backupName = UpdatePackageField.CreateBackupName(this.m_updatePackageData.MethodologyName, field.Name);
          UpdatePackageField updatePackageField2;
          if (this.m_nameMap.TryGetValue(backupName, out updatePackageField2))
          {
            this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateFieldNameBackup", (object) field.Name, (object) backupName, (object) field.ReferenceName, (object) updatePackageField1.ReferenceName, (object) updatePackageField2.ReferenceName));
          }
          else
          {
            string str = InternalsResourceStrings.Format("FieldNameChanged", (object) field.ReferenceName, (object) field.Name, (object) backupName, (object) updatePackageField1.ReferenceName);
            this.m_updatePackageData.Metadata.RaiseImportEvent((Exception) null, str);
            TeamFoundationTrace.Verbose(str);
            field.Name = backupName;
          }
        }
        else
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateFieldName", (object) field.Name, (object) updatePackageField1.ReferenceName, (object) field.ReferenceName));
      }
      else
        this.m_nameMap[field.Name] = field;
    }

    private static void ValidateDuplicateReportingName(
      IMetadataProvisioningHelper pro,
      Dictionary<string, string> knownNames,
      string refname,
      string name)
    {
      name = UpdatePackageFieldCollection.GetReportingFieldName(name);
      string str;
      if (knownNames.TryGetValue(name, out str))
      {
        string message = InternalsResourceStrings.Format("ErrorReportingFieldNamesCollision", (object) refname, (object) str, (object) name);
        pro.ThrowValidationException(message);
      }
      knownNames.Add(name, refname);
    }

    public UpdatePackageField this[string name]
    {
      get
      {
        UpdatePackageField updatePackageField;
        if (!this.m_fieldsMap.TryGetValue(name, out updatePackageField))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorFieldNotFound", (object) name));
        return updatePackageField;
      }
    }

    public bool Contains(string fieldName) => this.m_fieldsMap.ContainsKey(fieldName);

    public void StateAddReferencesToUpdatePackage(
      UpdatePackageRuleContext context,
      MetaID stateId,
      XmlElement containerElement,
      UpdatePackage batch)
    {
      FlagHelper flagHelper = FlagHelper.StateCreateFlagHelper(stateId);
      this.AddReferencesToUpdatePackage(context, flagHelper, containerElement, batch);
    }

    public void TransitionAddReferencesToUpdatePackage(
      UpdatePackageRuleContext context,
      MetaID fromStateId,
      MetaID toStateId,
      XmlElement containerElement,
      UpdatePackage batch)
    {
      FlagHelper flagHelper = FlagHelper.TransitionCreateFlagHelper(fromStateId, toStateId);
      this.AddReferencesToUpdatePackage(context, flagHelper, containerElement, batch);
    }

    public void ReasonAddReferencesToUpdatePackage(
      UpdatePackageRuleContext context,
      MetaID fromStateId,
      MetaID toStateId,
      XmlElement containerElement,
      UpdatePackage batch)
    {
      FlagHelper flagHelper = FlagHelper.ReasonCreateFlagHelper(fromStateId, toStateId);
      this.AddReferencesToUpdatePackage(context, flagHelper, containerElement, batch);
    }

    private void AddReferencesToUpdatePackage(
      UpdatePackageRuleContext context,
      FlagHelper flagHelper,
      XmlElement container,
      UpdatePackage batch)
    {
      XmlNodeList xmlNodeList = container.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}/{1}", (object) ProvisionTags.FieldReferences, (object) ProvisionTags.FieldReference));
      Dictionary<MetaID, bool> dictionary = new Dictionary<MetaID, bool>(xmlNodeList.Count);
      foreach (XmlElement xmlElement in xmlNodeList)
      {
        UpdatePackageField updatePackageField = this[xmlElement.GetAttribute(ProvisionAttributes.FieldReferenceName)];
        if (dictionary.ContainsKey(updatePackageField.ID))
          this.m_updatePackageData.Metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateFieldReference", (object) updatePackageField.ReferenceName));
        RuleFlags flags = flagHelper.GetFlags(updatePackageField.FlagManager, xmlElement);
        dictionary[updatePackageField.ID] = true;
        updatePackageField.AddRulesToUpdatePackage(context, (XmlNode) xmlElement, flags, batch);
      }
    }

    private void AddToUpdatePackage(
      UpdatePackageRuleContext context,
      UpdatePackage batch,
      bool provisionRules = true,
      bool provisionFields = true)
    {
      if (provisionFields)
      {
        foreach (UpdatePackageField fields in this.m_fieldsList)
          fields.AddToUpdatePackage(context, true, batch);
      }
      if (!provisionRules)
        return;
      foreach (UpdatePackageField fields in this.m_fieldsList)
        fields.AddToUpdatePackage(context, false, batch);
    }

    public bool IsCoreField(int fieldId)
    {
      if (this.m_coreFieldIds == null)
        this.m_coreFieldIds = new HashSet<int>(this.m_updatePackageData.Metadata.GetCoreFieldIds());
      return this.m_coreFieldIds.Contains(fieldId);
    }

    public static string GetReportingFieldName(string refName) => refName.Replace(".", "_");

    public UpdatePackageField GetField(string refName)
    {
      UpdatePackageField updatePackageField;
      return !this.m_refnameMap.TryGetValue(refName, out updatePackageField) ? (UpdatePackageField) null : updatePackageField;
    }
  }
}
