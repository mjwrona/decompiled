// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackage
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class UpdatePackage
  {
    protected IMetadataProvisioningHelper m_metadata;
    protected XmlDocument m_doc;
    protected Hashtable m_constMap;
    private string m_globalGuid;
    private string m_instanceGuid;
    private MetaIDFactory m_metaIdFactory;
    private bool m_fillRulesWithNegativeProjectIds;

    public UpdatePackage(
      IMetadataProvisioningHelper metadata,
      MetaIDFactory metaIdFactory,
      bool fillRulesWithNegativeProjectIds = false)
    {
      this.m_metadata = metadata;
      this.m_metaIdFactory = metaIdFactory;
      this.m_constMap = new Hashtable(1024, (IEqualityComparer) metadata.ServerStringComparer);
      this.m_fillRulesWithNegativeProjectIds = fillRulesWithNegativeProjectIds;
      this.m_doc = new XmlDocument();
      XmlElement element = this.m_doc.CreateElement("Package");
      element.SetAttribute("Product", string.Empty);
      this.m_doc.AppendChild((XmlNode) element);
    }

    public UpdatePackage(UpdatePackageData updatePackageData)
      : this(updatePackageData.Metadata, updatePackageData.MetaIDFactory)
    {
    }

    public MetaID InsertConstant(string constant, MetaID listID) => this.InsertConstant((string) null, constant, listID);

    internal MetaID InsertConstant(string projectGuid, string constant, MetaID listID)
    {
      constant = constant.Trim();
      MetaID constID;
      if (constant.Length > 0)
      {
        constID = !this.IsUserName(constant) ? this.InsertConstantInternal(constant, false) : this.FindUserGroup(projectGuid, constant);
      }
      else
      {
        if (listID != null)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Get("WorkItemTypeListItemMustHaveValue"));
        constID = this.m_metaIdFactory.NewMetaID(-10000);
      }
      if (listID != null)
        this.SetListMember(listID, constID);
      return constID;
    }

    private MetaID InsertConstantInternal(string constant, bool lookupFlag)
    {
      MetaID metaId = (MetaID) this.m_constMap[(object) constant];
      if (metaId == null)
      {
        metaId = this.m_metaIdFactory.NewMetaID();
        this.m_constMap[(object) constant] = (object) metaId;
        XmlElement element1 = this.m_doc.CreateElement("InsertConstant");
        element1.SetAttribute("TempID", metaId.ToString());
        element1.SetAttribute("LookupAccount", lookupFlag ? "1" : "0");
        XmlElement element2 = this.m_doc.CreateElement("Name");
        element2.SetAttribute("xml:space", "preserve");
        element2.InnerText = constant;
        element1.AppendChild((XmlNode) element2);
        this.m_doc.DocumentElement.AppendChild((XmlNode) element1);
      }
      return metaId;
    }

    public void SetListMember(MetaID listID, MetaID constID)
    {
      XmlElement element = this.m_doc.CreateElement("InsertConstantSet");
      element.SetAttribute("TempID", this.m_metaIdFactory.NewMetaID().ToString());
      element.SetAttribute(listID.IsTemporary ? "TempParentID" : "ParentID", listID.ToString());
      element.SetAttribute(constID.IsTemporary ? "TempConstantID" : "ConstantID", constID.ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public void InsertComment(string formatString, params object[] args)
    {
    }

    public MetaID InsertWorkItemTypeCategory(
      int projectId,
      string refName,
      string name,
      int defaultTypeId)
    {
      MetaID metaId = this.m_metaIdFactory.NewMetaID();
      XmlElement element = this.m_doc.CreateElement(nameof (InsertWorkItemTypeCategory));
      element.SetAttribute("TempID", metaId.ToString());
      element.SetAttribute("ProjectID", XmlConvert.ToString(projectId));
      element.SetAttribute("Name", name);
      element.SetAttribute("ReferenceName", refName);
      element.SetAttribute("DefaultWorkItemTypeID", XmlConvert.ToString(defaultTypeId));
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
      return metaId;
    }

    public void DestroyWorkItemTypeCategory(int catId)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (DestroyWorkItemTypeCategory));
      element.SetAttribute("CategoryID", XmlConvert.ToString(catId));
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public void DeleteWorkItemTypeCategoryMember(int catMemberId)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (DeleteWorkItemTypeCategoryMember));
      element.SetAttribute("CategoryMemberID", XmlConvert.ToString(catMemberId));
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public void InsertWorkItemTypeCategoryMember(MetaID catId, int typeId)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (InsertWorkItemTypeCategoryMember));
      element.SetAttribute(catId.IsTemporary ? "TempCategoryID" : "CategoryID", catId.ToString());
      element.SetAttribute("WorkItemTypeID", XmlConvert.ToString(typeId));
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public void UpdateWorkItemTypeCategory(int catId, Hashtable props)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (UpdateWorkItemTypeCategory));
      element.SetAttribute("CategoryID", XmlConvert.ToString(catId));
      foreach (string key in (IEnumerable) props.Keys)
        element.SetAttribute(key, props[(object) key].ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public void UpdateFieldUsage(int usageId, Hashtable props)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (UpdateFieldUsage));
      element.SetAttribute("FldUsageID", XmlConvert.ToString(usageId));
      foreach (DictionaryEntry prop in props)
        element.SetAttribute((string) prop.Key, prop.Value.ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    private bool IsUserName(string name) => this.m_metadata.IsValidIdentityNameFormat(name);

    internal MetaID FindGroup(string projectGuid, string constant) => this.FindIdentity(projectGuid, constant, true);

    internal MetaID FindUserGroup(string projectGuid, string constant) => this.FindIdentity(projectGuid, constant, false);

    private MetaID FindIdentity(string projectGuid, string name, bool groupsOnlyFlag)
    {
      string str1 = (string) null;
      string str2 = (string) null;
      if (name.StartsWith(ProvisionValues.ConstScopeProject, StringComparison.OrdinalIgnoreCase))
      {
        if (projectGuid == ProvisionValues.ConstScopeProject)
          return this.m_metaIdFactory.NewMetaID(0);
        if (string.IsNullOrEmpty(projectGuid))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorMissingProjectScope", (object) name));
        str1 = ProvisionValues.ConstScopeProject;
        str2 = projectGuid;
      }
      else if (name.StartsWith(ProvisionValues.ConstScopeGlobal, StringComparison.OrdinalIgnoreCase))
      {
        str1 = ProvisionValues.ConstScopeGlobal;
        str2 = this.GlobalGuid;
      }
      else if (name.StartsWith(ProvisionValues.ConstScopeInstance, StringComparison.OrdinalIgnoreCase))
      {
        str1 = ProvisionValues.ConstScopeInstance;
        str2 = this.InstanceGuid;
      }
      string str3 = name;
      if (str1 != null)
      {
        string str4 = name.Remove(0, str1.Length);
        name = str2 + str4;
      }
      int id;
      if (this.m_metadata.FindConstByFullName(name, true, out id))
      {
        if (groupsOnlyFlag && this.m_metadata.IsValidGroup(id) || !groupsOnlyFlag && this.m_metadata.IsValidUserOrGroup(id))
          return this.m_metaIdFactory.NewMetaID(id);
        if (groupsOnlyFlag)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorGroupIdentityNotFound", (object) str3));
      }
      else if (projectGuid == ProvisionValues.ConstScopeProject)
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorIdentityNotFound", (object) str3));
      return this.InsertIdentity(name);
    }

    private MetaID InsertIdentity(string name)
    {
      this.m_metadata.RaiseImportEvent((Exception) null, InternalsResourceStrings.Format("WarningMissingIdentity", (object) name));
      return this.InsertConstantInternal(name, true);
    }

    public void InsertField(
      MetaID id,
      string name,
      string refName,
      int type,
      int report,
      int formula,
      string reportingName,
      string reportingRefName)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (InsertField));
      element.SetAttribute("TempID", id.ToString());
      element.SetAttribute("Name", name);
      element.SetAttribute("ReferenceName", refName);
      element.SetAttribute("Type", XmlConvert.ToString(type));
      element.SetAttribute("ReportingType", XmlConvert.ToString(report));
      element.SetAttribute("ReportingFormula", XmlConvert.ToString(formula));
      if (report != 0)
        element.SetAttribute("ReportingEnabled", XmlConvert.ToString(1));
      if (!string.IsNullOrEmpty(reportingName))
        element.SetAttribute("ReportingName", reportingName);
      if (!string.IsNullOrEmpty(reportingRefName))
        element.SetAttribute("ReportingReferenceName", reportingRefName);
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
      this.InsertFieldUsage(id);
    }

    public void UpdateField(int fieldId, Dictionary<string, object> props)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (UpdateField));
      element.SetAttribute("FieldID", XmlConvert.ToString(fieldId));
      foreach (KeyValuePair<string, object> prop in props)
        element.SetAttribute(prop.Key, prop.Value.ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public void InsertFieldUsage(MetaID fieldID)
    {
      if (!fieldID.IsTemporary)
        return;
      MetaID metaId = this.m_metaIdFactory.NewMetaID();
      XmlElement element = this.m_doc.CreateElement(nameof (InsertFieldUsage));
      element.SetAttribute("TempID", metaId.ToString());
      element.SetAttribute(fieldID.IsTemporary ? "TempFldID" : "FldID", fieldID.ToString());
      element.SetAttribute("ParentID", XmlConvert.ToString(-100));
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public void InsertWorkItemTypeUsage(MetaID typeId, MetaID fieldId)
    {
      MetaID metaId = this.m_metaIdFactory.NewMetaID();
      XmlElement element = this.m_doc.CreateElement(nameof (InsertWorkItemTypeUsage));
      element.SetAttribute("TempID", metaId.ToString());
      element.SetAttribute(typeId.IsTemporary ? "TempWorkItemTypeID" : "WorkItemTypeID", typeId.ToString());
      element.SetAttribute(fieldId.IsTemporary ? "TempFieldID" : "FieldID", fieldId.ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
    }

    public MetaID InsertRule(int projectId, UpdatePackageRuleProperties props)
    {
      MetaID metaId = this.m_metaIdFactory.NewMetaID();
      XmlElement element = this.m_doc.CreateElement(nameof (InsertRule));
      element.SetAttribute("TempID", metaId.ToString());
      if (this.m_fillRulesWithNegativeProjectIds)
        projectId *= -1;
      string str = XmlConvert.ToString(projectId);
      element.SetAttribute("FlowDownTree", "1");
      element.SetAttribute("AreaID", str);
      element.SetAttribute("RootTreeID", str);
      element.SetAttribute("ObjectTypeScopeID", XmlConvert.ToString(-100));
      props.SetProperties(element);
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
      return metaId;
    }

    public MetaID InsertForm(int projectId, MetaID typeConstID, string form)
    {
      this.InsertComment("Form property");
      MetaID metaId = this.InsertTreeProperty(projectId, Guid.NewGuid().ToString(), form);
      UpdatePackageRuleProperties props = new UpdatePackageRuleProperties();
      props[(object) "Default"] = (object) true;
      props[(object) "IfFldID"] = (object) 25;
      string key1 = typeConstID.IsTemporary ? "TempIfConstID" : "IfConstID";
      props[(object) key1] = (object) typeConstID.Value;
      props[(object) "ThenFldID"] = (object) -14;
      string key2 = metaId.IsTemporary ? "TempThenConstID" : "ThenConstID";
      props[(object) key2] = (object) metaId.Value;
      props[(object) "ThenConstLargetext"] = (object) true;
      this.InsertComment("Form rule");
      this.InsertRule(projectId, props);
      return metaId;
    }

    public MetaID InsertTreeProperty(int projectId, string name, string val)
    {
      MetaID metaId = this.m_metaIdFactory.NewMetaID();
      XmlElement element = this.m_doc.CreateElement(nameof (InsertTreeProperty));
      element.SetAttribute("AreaID", XmlConvert.ToString(projectId));
      element.SetAttribute("Name", name);
      element.SetAttribute("Value", val);
      element.SetAttribute("TempID", metaId.ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
      return metaId;
    }

    public MetaID InsertAction(MetaID typeID, MetaID fromID, MetaID toID, string action)
    {
      XmlElement element = this.m_doc.CreateElement(nameof (InsertAction));
      MetaID metaId = this.m_metaIdFactory.NewMetaID();
      element.SetAttribute("TempID", metaId.ToString());
      element.SetAttribute("Name", action);
      element.SetAttribute(typeID.IsTemporary ? "TempWorkItemTypeID" : "WorkItemTypeID", typeID.ToString());
      element.SetAttribute(fromID.IsTemporary ? "TempFromStateConstantID" : "FromStateConstantID", fromID.ToString());
      element.SetAttribute(toID.IsTemporary ? "TempToStateConstantID" : "ToStateConstantID", toID.ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
      return metaId;
    }

    public static void SetListRuleProperties(ListRuleType ruleType, MetaID listID, Hashtable props)
    {
      props[listID.IsTemporary ? (object) "TempThenConstID" : (object) "ThenConstID"] = (object) listID.Value;
      props[(object) "ThenLeaf"] = (object) true;
      props[(object) "ThenOneLevel"] = (object) true;
      switch (ruleType)
      {
        case ListRuleType.AllowedList:
        case ListRuleType.RequiredList:
          props[(object) "DenyWrite"] = (object) true;
          props[(object) "Unless"] = (object) true;
          if (ruleType == ListRuleType.RequiredList)
            break;
          props[(object) "ThenImplicitEmpty"] = (object) true;
          break;
        case ListRuleType.SuggestedList:
          props[(object) "Suggestion"] = (object) true;
          break;
        case ListRuleType.ProhibitedList:
          props[(object) "DenyWrite"] = (object) true;
          props[(object) "Unless"] = (object) true;
          props[(object) "ThenNot"] = (object) true;
          break;
      }
    }

    public MetaID InsertWorkItemType(int projectID, MetaID typeConstID, MetaID descrID)
    {
      MetaID metaId = this.m_metaIdFactory.NewMetaID();
      XmlElement element = this.m_doc.CreateElement(nameof (InsertWorkItemType));
      element.SetAttribute(typeConstID.IsTemporary ? "TempNameConstantID" : "NameConstantID", typeConstID.ToString());
      element.SetAttribute("ProjectID", XmlConvert.ToString(projectID));
      element.SetAttribute(descrID.IsTemporary ? "TempDescriptionID" : "DescriptionID", descrID.ToString());
      element.SetAttribute("TempID", metaId.ToString());
      this.m_doc.DocumentElement.AppendChild((XmlNode) element);
      return metaId;
    }

    public XmlDocument Xml => this.m_doc;

    internal string GlobalGuid
    {
      get
      {
        if (this.m_globalGuid == null)
          this.m_globalGuid = "488bb442-0beb-4c1e-98b6-4eddc604bd9e" + "\\";
        return this.m_globalGuid;
      }
    }

    internal string InstanceGuid
    {
      get
      {
        if (this.m_instanceGuid == null)
          this.m_instanceGuid = this.m_metadata.InstanceGuid;
        return this.m_instanceGuid;
      }
    }
  }
}
