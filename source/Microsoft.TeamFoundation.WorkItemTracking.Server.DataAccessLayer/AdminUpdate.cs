// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AdminUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Events;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class AdminUpdate : BaseUpdate
  {
    private bool m_verbose;
    protected bool m_linkTypeUpdate;
    private List<AdminUpdateEventData> m_operations = new List<AdminUpdateEventData>();

    public AdminUpdate(
      SqlBatchBuilder sqlBatch,
      XmlNode actionNode,
      Update parentUpdate,
      XmlNode outputNode,
      IVssIdentity user,
      bool verbose,
      IVssRequestContext requestContext)
      : base(sqlBatch, actionNode, parentUpdate, outputNode, user)
    {
      this.m_verbose = verbose;
    }

    public void InsertField()
    {
      string stringValue1;
      this.GetAttributeString("Name", true, (string) null, out stringValue1);
      string stringValue2;
      this.GetAttributeString("ReferenceName", true, (string) null, out stringValue2);
      int intValue1;
      this.GetAttributeInt("Type", true, 0, out intValue1);
      int intValue2;
      this.GetAttributeInt("ReportingType", false, 0, out intValue2);
      int intValue3;
      this.GetAttributeInt("ReportingFormula", false, 0, out intValue3);
      bool outValue;
      this.GetAttributeBool("ReportingEnabled", false, false, out outValue);
      string stringValue3;
      this.GetAttributeString("ReportingName", false, (string) null, out stringValue3);
      string stringValue4;
      this.GetAttributeString("ReportingReferenceName", false, (string) null, out stringValue4);
      this.CheckForValidReportingType(intValue2);
      this.CheckForValidFormula(intValue3);
      this.CheckForValidFieldName(stringValue1);
      this.CheckForValidFieldReferenceName(stringValue2);
      if (!string.IsNullOrEmpty(stringValue3))
        this.CheckForValidFieldName(stringValue3);
      if (!string.IsNullOrEmpty(stringValue4))
        this.CheckForValidFieldReferenceName(stringValue4);
      int actionId;
      this.Initialize(true, "AuthorizeFieldChanges", "Rule", this.m_parentUpdate.ChangeFieldGroup, this.m_parentUpdate.CheckFieldGroup, out actionId);
      DalChangeFieldElement element = DalSqlElement.GetElement<DalChangeFieldElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeFieldGroup, actionId, stringValue1, true, intValue1, true, intValue2, true, intValue3, true, outValue, true, stringValue3, stringValue4, stringValue2, true, (string) null, true);
      this.m_updateSqlElement = (DalSqlElement) element;
    }

    private void CheckForValidFieldName(string name)
    {
      if (!ValidationMethods.IsValidFieldName(name))
      {
        this.m_sqlBatch.RequestContext.Trace(900000, TraceLevel.Error, "Update", nameof (AdminUpdate), "Invalid Field Name: {0}", (object) name);
        throw new ArgumentException(DalResourceStrings.Get("InsertFieldInvalidFieldName"), "updateElement");
      }
    }

    private void CheckForValidFieldReferenceName(string referenceName)
    {
      if (!ValidationMethods.IsValidReferenceFieldNameForImport(referenceName))
      {
        this.m_sqlBatch.RequestContext.Trace(900001, TraceLevel.Error, "Update", nameof (AdminUpdate), "Invalid Reference Name: {0}", (object) referenceName);
        throw new ArgumentException(DalResourceStrings.Get("InsertFieldInvalidReferenceName"), "updateElement");
      }
    }

    private void CheckForValidReportingType(int reportingType)
    {
      if (reportingType != 0 && reportingType != 1 && reportingType != 2 && reportingType != 3)
      {
        this.m_sqlBatch.RequestContext.Trace(900002, TraceLevel.Error, "Update", nameof (AdminUpdate), "Invalid reporting type: {0}", (object) reportingType.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        throw new ArgumentException(DalResourceStrings.Get("InsertUpdateFieldInvalidReportingType"), "updateElement");
      }
    }

    private void CheckForValidFormula(int formula)
    {
      if (formula != 0 && formula != 1 && formula != 2 && formula != 3 && formula != 4 && formula != 5 && formula != 6)
      {
        this.m_sqlBatch.RequestContext.Trace(900003, TraceLevel.Error, "Update", nameof (AdminUpdate), "Invalid formula: {0}", (object) formula.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        throw new ArgumentException(DalResourceStrings.Get("InsertUpdateFieldInvalidReportingFormula"), "updateElement");
      }
    }

    public void UpdateField()
    {
      string stringValue1 = string.Empty;
      int intValue1;
      this.GetAttributeInt("FieldID", true, 0, out intValue1);
      this.m_inputId = intValue1;
      string stringValue2;
      bool attributeString = this.GetAttributeString("Name", false, (string) null, out stringValue2);
      int intValue2;
      bool attributeInt1 = this.GetAttributeInt("Type", false, 0, out intValue2);
      this.GetAttributeString("Cachestamp", false, (string) null, out stringValue1);
      int intValue3;
      bool attributeInt2 = this.GetAttributeInt("ReportingType", false, 0, out intValue3);
      int intValue4;
      bool attributeInt3 = this.GetAttributeInt("ReportingFormula", false, 0, out intValue4);
      bool outValue;
      bool attributeBool = this.GetAttributeBool("ReportingEnabled", false, false, out outValue);
      string stringValue3;
      this.GetAttributeString("ReportingName", false, (string) null, out stringValue3);
      string stringValue4;
      this.GetAttributeString("ReportingReferenceName", false, (string) null, out stringValue4);
      if (attributeInt2)
        this.CheckForValidReportingType(intValue3);
      if (attributeInt3)
        this.CheckForValidFormula(intValue4);
      if (!string.IsNullOrEmpty(stringValue2))
        this.CheckForValidFieldName(stringValue2);
      if (!string.IsNullOrEmpty(stringValue3))
        this.CheckForValidFieldName(stringValue3);
      if (!string.IsNullOrEmpty(stringValue4))
        this.CheckForValidFieldReferenceName(stringValue4);
      int actionId;
      this.Initialize(false, "AuthorizeFieldChanges", "Rule", this.m_parentUpdate.ChangeFieldGroup, this.m_parentUpdate.CheckFieldGroup, out actionId);
      DalChangeFieldElement element = DalSqlElement.GetElement<DalChangeFieldElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeFieldGroup, actionId, stringValue2, attributeString, intValue2, attributeInt1, intValue3, attributeInt2, intValue4, attributeInt3, outValue, attributeBool, stringValue3, stringValue4, (string) null, false, stringValue1, false);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddField(this.m_inputId);
      if (!attributeInt1)
        return;
      FieldEntry fieldById = this.RequestContext.GetService<WorkItemTrackingFieldService>().GetFieldById(this.RequestContext, intValue1);
      if (fieldById == null || fieldById.FieldDataType == intValue2)
        return;
      if (intValue2 == 16 && fieldById.FieldDataType == 24 || intValue2 == 24 && fieldById.FieldDataType == 16 && (fieldById.IsCore || WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(this.m_sqlBatch.RequestContext)))
        throw new LegacyValidationException(InternalsResourceStrings.Get("ErrorObseleteFeature"));
      int num = -513;
      if ((intValue2 & num) != (fieldById.FieldDataType & num) && (intValue2 != 24 || fieldById.FieldDataType != 16))
        throw new LegacyValidationException(DalResourceStrings.Get("BadAdminDataTypeCannotChangeAfterFieldHasBeenUsed"), 600069);
      this.m_operations.Add((AdminUpdateEventData) new FieldTypeUpdateEventData()
      {
        FieldId = intValue1,
        ReferenceName = fieldById.ReferenceName,
        OldType = new int?(fieldById.FieldDataType),
        NewType = new int?(intValue2)
      });
    }

    public void DeleteField()
    {
      this.GetAttributeInt("FieldID", true, 0, out this.m_inputId);
      int actionId;
      this.Initialize(false, "AuthorizeFieldChanges", "Rule", this.m_parentUpdate.ChangeFieldGroup, this.m_parentUpdate.CheckFieldGroup, out actionId);
      DalDeleteFieldElement element = DalSqlElement.GetElement<DalDeleteFieldElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.DeleteFieldGroup, actionId);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.HasDeleteField = true;
    }

    public void InsertFieldUsage()
    {
      bool oftenQueried = false;
      int intValue1;
      this.GetAttributeInt("FldID", false, 0, out intValue1);
      int intValue2;
      this.GetAttributeTempId("TempFldID", false, 0, out intValue2);
      if (intValue1 == 0 && intValue2 == 0)
        throw new ArgumentException(DalResourceStrings.Get("InsertFieldUsageMissingFieldId"), "updateElement");
      int actionId;
      this.Initialize(true, "AuthorizeFieldChanges", "Rule", this.m_parentUpdate.ChangeFieldGroup, this.m_parentUpdate.CheckFieldGroup, out actionId);
      DalChangeFieldUsageElement element = DalSqlElement.GetElement<DalChangeFieldUsageElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeFieldGroup, actionId, false, intValue1, intValue2, oftenQueried, (string) null, true);
      this.m_updateSqlElement = (DalSqlElement) element;
    }

    public void UpdateFieldUsage()
    {
      this.GetAttributeInt("FldUsageID", true, 0, out this.m_inputId);
      bool outValue1;
      this.GetAttributeBool("Deleted", false, false, out outValue1);
      bool outValue2;
      this.GetAttributeBool("OftenQueried", false, false, out outValue2);
      if (outValue2)
        throw new LegacyValidationException(InternalsResourceStrings.Get("ErrorObseleteFeature"));
      string stringValue;
      this.GetAttributeString("Cachestamp", false, (string) null, out stringValue);
      int actionId;
      this.Initialize(false, "AuthorizeFieldChanges", "Rule", this.m_parentUpdate.ChangeFieldGroup, this.m_parentUpdate.CheckFieldGroup, out actionId);
      DalChangeFieldUsageElement element = DalSqlElement.GetElement<DalChangeFieldUsageElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeFieldGroup, actionId, outValue1, 0, 0, outValue2, stringValue, false);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddFieldUsage(this.m_inputId);
    }

    public void InsertTreeProperty()
    {
      int intValue1;
      this.GetAttributeInt("AreaID", false, 0, out intValue1);
      int intValue2;
      this.GetAttributeTempId("TempAreaID", false, 0, out intValue2);
      string stringValue1;
      this.GetAttributeString("Name", false, (string) null, out stringValue1);
      string stringValue2;
      this.GetAttributeString("Value", false, (string) null, out stringValue2);
      if (stringValue1 != null && stringValue1.Length > 120)
      {
        this.m_sqlBatch.RequestContext.Trace(900004, TraceLevel.Error, "Update", nameof (AdminUpdate), "Tree Property name too long: {0}", (object) stringValue1);
        throw new ArgumentException(DalResourceStrings.Get("InsertTreePropertyNameTooLong"), "updateElement");
      }
      int actionId;
      this.Initialize(true, (string) null, "Tree", this.m_parentUpdate.ChangeTreeGroup, this.m_parentUpdate.CheckTreeGroup, out actionId);
      DalChangeTreePropertyElement element = DalSqlElement.GetElement<DalChangeTreePropertyElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeTreeGroup, actionId, intValue1, intValue2, stringValue1, true, stringValue2, true, false, (string) null, true);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddProject(intValue1);
    }

    public void UpdateTreeProperty()
    {
      this.GetAttributeInt("TreePropID", true, 0, out this.m_inputId);
      bool outValue;
      this.GetAttributeBool("Deleted", false, false, out outValue);
      int intValue1;
      this.GetAttributeInt("AreaID", false, 0, out intValue1);
      int intValue2;
      this.GetAttributeTempId("TempAreaID", false, 0, out intValue2);
      string stringValue1;
      bool attributeString1 = this.GetAttributeString("Name", false, (string) null, out stringValue1);
      string stringValue2;
      bool attributeString2 = this.GetAttributeString("Value", false, (string) null, out stringValue2);
      if (stringValue1 != null && stringValue1.Length > 120)
      {
        this.m_sqlBatch.RequestContext.Trace(900005, TraceLevel.Error, "Update", nameof (AdminUpdate), "Tree Property name too long: {0}", (object) stringValue1);
        throw new ArgumentException(DalResourceStrings.Get("InsertTreePropertyNameTooLong"), "updateElement");
      }
      int actionId;
      this.Initialize(false, (string) null, "Tree", this.m_parentUpdate.ChangeTreeGroup, this.m_parentUpdate.CheckTreeGroup, out actionId);
      DalChangeTreePropertyElement element = DalSqlElement.GetElement<DalChangeTreePropertyElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeTreeGroup, actionId, intValue1, intValue2, stringValue1, attributeString1, stringValue2, attributeString2, outValue, (string) null, false);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddTreeProperty(this.m_inputId);
    }

    public void InsertConstant()
    {
      bool outValue;
      this.GetAttributeBool("LookupAccount", false, false, out outValue);
      XmlNodeList elementsByTagName = ((XmlElement) this.m_actionNode).GetElementsByTagName("Name");
      string stringValue;
      switch (elementsByTagName.Count)
      {
        case 0:
          if (!this.GetAttributeString("Name", false, (string) null, out stringValue))
            throw new ArgumentException(DalResourceStrings.Format("MissingElementInAction", (object) "Name", (object) nameof (InsertConstant)), "updateElement");
          break;
        case 1:
          stringValue = elementsByTagName[0].InnerText;
          break;
        default:
          throw new ArgumentException(DalResourceStrings.Format("RepeatedElementInAction", (object) "Name", (object) nameof (InsertConstant)), "updateElement");
      }
      if (stringValue.Length > 256)
      {
        this.m_sqlBatch.RequestContext.Trace(900006, TraceLevel.Error, "Update", nameof (AdminUpdate), "Constant name too long: {0}", (object) stringValue);
        throw new ArgumentException(DalResourceStrings.Format("InsertConstantNameTooLong", (object) 256, (object) stringValue), "updateElement");
      }
      if (stringValue.IndexOf('\\') >= 0 && !outValue)
      {
        this.m_sqlBatch.RequestContext.Trace(900007, TraceLevel.Error, "Update", nameof (AdminUpdate), "Constant contains a backslash and is not an identity: {0}", (object) stringValue);
        throw new ArgumentException(DalResourceStrings.Format("InsertConstantWithBackslash", (object) stringValue), "updateElement");
      }
      string str = stringValue.Trim();
      if (str.Length == 0)
        throw new LegacyValidationException(DalResourceStrings.Get("UnexpectedReturnedDataSetException"), 602001);
      if (outValue && str.IndexOf('\\') < 0)
        throw new LegacyValidationException("ErrorAddConstantNotValidDomainAccount", 600019);
      int num = this.InitializeActionId();
      ServerConstant o = new ServerConstant();
      o.ConstId = -num;
      o.LookupAccount = outValue;
      if (outValue)
      {
        int length = str.IndexOf('\\');
        if (length > 0)
          o.DomainPart = str.Substring(0, length);
        if (length + 1 < str.Length)
          o.NamePart = str.Substring(length + 1, str.Length - (length + 1));
      }
      else
        o.DisplayPart = str;
      this.m_parentUpdate.ChangeConstantElement.AddObject(o);
    }

    public void InsertRule()
    {
      InsertRuleAttributes insertRuleAttributes = this.CreateInsertRuleAttributes(this.InitializeActionId());
      this.m_parentUpdate.SaveRuleElement.AddObject(insertRuleAttributes);
      if (this.WitRequestContext.IsAadBackedAccount && AdminUpdate.IsAllowedValueRule(insertRuleAttributes) && !AdminUpdate.IsValidUserRule(insertRuleAttributes))
        this.m_parentUpdate.SaveIdentityRuleElement.AddObject(this.CreateSuggestedValueInsertRuleAttributes());
      this.m_parentUpdate.ProvisionHelper.AddProject(insertRuleAttributes.areaId);
    }

    public void UpdateRule()
    {
      string stringValue = string.Empty;
      this.GetAttributeInt("RuleID", true, 0, out this.m_inputId);
      this.GetAttributeString("Cachestamp", false, (string) null, out stringValue);
      bool outValue1;
      bool attributeBool1 = this.GetAttributeBool("GrantRead", false, false, out outValue1);
      bool outValue2;
      bool attributeBool2 = this.GetAttributeBool("DenyRead", false, false, out outValue2);
      bool outValue3;
      bool attributeBool3 = this.GetAttributeBool("GrantWrite", false, false, out outValue3);
      bool outValue4;
      bool attributeBool4 = this.GetAttributeBool("DenyWrite", false, false, out outValue4);
      bool outValue5;
      bool attributeBool5 = this.GetAttributeBool("GrantAdmin", false, false, out outValue5);
      bool outValue6;
      bool attributeBool6 = this.GetAttributeBool("DenyAdmin", false, false, out outValue6);
      bool outValue7;
      bool attributeBool7 = this.GetAttributeBool("Default", false, false, out outValue7);
      bool outValue8;
      bool attributeBool8 = this.GetAttributeBool("Suggestion", false, false, out outValue8);
      bool outValue9;
      bool attributeBool9 = this.GetAttributeBool("Helptext", false, false, out outValue9);
      if (!(attributeBool1 | attributeBool2 | attributeBool3 | attributeBool4 | attributeBool5 | attributeBool6 | attributeBool7 | attributeBool8 | attributeBool9))
        return;
      int actionId;
      this.Initialize(false, "AuthorizeRuleChanges", "Rule", this.m_parentUpdate.ChangeRuleGroup, this.m_parentUpdate.CheckRuleGroup, out actionId);
      DalSqlElement.GetElement<DalChangeRuleElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.ChangeRuleGroup, actionId, outValue1, outValue2, outValue3, outValue4, outValue5, outValue6, outValue7, outValue8, outValue9, attributeBool9, attributeBool1, attributeBool2, attributeBool3, attributeBool4, attributeBool5, attributeBool6, attributeBool7, attributeBool8, stringValue);
      this.m_parentUpdate.ProvisionHelper.AddRule(this.m_inputId);
    }

    public void InsertConstantSet(bool provisionRules)
    {
      int num = this.InitializeActionId();
      if (provisionRules)
        this.InitializeRelatedGroups("AuthorizeRuleChanges", (string) null, (ElementGroup) null, this.m_parentUpdate.CheckRuleGroup, (ElementGroup) null);
      ServerConstantSet o = new ServerConstantSet();
      o.SetId = -num;
      this.GetAttributeInt("ParentID", false, 0, out o.ParentId);
      this.GetAttributeTempId("TempParentID", false, 0, out o.TempParentId);
      this.GetAttributeInt("ConstantID", false, 0, out o.ConstId);
      this.GetAttributeTempId("TempConstantID", false, 0, out o.TempConstId);
      this.GetAttributeBool("Deleted", false, false, out o.IsDeleted);
      this.GetAttributeCacheStamp("Cachestamp", false, out o.CacheStamp);
      if (o.ConstId == 0 && o.TempConstId == 0)
        throw new ArgumentException(DalResourceStrings.Get("InsertConstantSetMissingConsantId"), "updateElement");
      this.m_parentUpdate.SaveConstantSetElement.AddObject(o);
      if (o.TempParentId != 0)
        return;
      this.m_parentUpdate.ProvisionHelper.AddConstant(o.ParentId);
    }

    public void UpdateConstantSet(bool provisionRules)
    {
      this.GetAttributeInt("SetID", true, 0, out this.m_inputId);
      bool outValue;
      bool attributeBool = this.GetAttributeBool("Deleted", true, false, out outValue);
      string stringValue;
      this.GetAttributeString("Cachestamp", false, (string) null, out stringValue);
      int actionId;
      this.Initialize(false, provisionRules ? "AuthorizeRuleChanges" : (string) null, "Rule", this.m_parentUpdate.ChangeConstantSetGroup, this.m_parentUpdate.CheckRuleGroup, out actionId);
      DalSqlElement.GetElement<DalChangeConstantSetElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.ChangeConstantSetGroup, actionId, 0, 0, 0, 0, outValue, attributeBool, stringValue, false, this.m_parentUpdate.Overwrite);
      this.m_parentUpdate.ProvisionHelper.AddConstantSet(this.m_inputId);
    }

    public void InsertWorkItemType()
    {
      int intValue1;
      this.GetAttributeInt("NameConstantID", false, 0, out intValue1);
      int intValue2;
      this.GetAttributeTempId("TempNameConstantID", false, 0, out intValue2);
      int intValue3;
      this.GetAttributeInt("ProjectID", true, 0, out intValue3);
      if (intValue1 == 0 && intValue2 == 0 && intValue3 >= 0)
      {
        this.m_parentUpdate.ProvisionHelper.AddProject(intValue3);
        this.m_parentUpdate.WorkItemTypeTemplateUpdateType = intValue3 == 0 ? WorkItemTypeTemplateUpdateType.CollectionGlobalWorkflow : WorkItemTypeTemplateUpdateType.ProjectGlobalWorkflow;
      }
      else
      {
        if (intValue3 < 0)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("ProjectIdNotPositive"), (object) intValue3.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
        int intValue4;
        this.GetAttributeInt("DescriptionID", false, -1, out intValue4);
        int intValue5;
        this.GetAttributeTempId("TempDescriptionID", false, 0, out intValue5);
        if (intValue1 == 0 && intValue2 == 0)
          throw new ArgumentException(DalResourceStrings.Get("InsertWorkItemTypeMissingNameConsantId"), "updateElement");
        this.m_parentUpdate.WorkItemTypeTemplateUpdateType = WorkItemTypeTemplateUpdateType.WorkItemType;
        int actionId;
        this.Initialize(true, "AuthorizeWorkItemTypeChanges", "Rule", this.m_parentUpdate.ChangeWorkItemTypeGroup, this.m_parentUpdate.CheckWorkItemTypeGroup, out actionId);
        DalChangeWorkItemTypeElement element = DalSqlElement.GetElement<DalChangeWorkItemTypeElement>(this.m_sqlBatch);
        element.JoinBatch(this.m_parentUpdate.ChangeWorkItemTypeGroup, actionId, intValue1, intValue2, true, intValue3, true, intValue4, intValue5, false, (string) null, true, this.m_parentUpdate.Overwrite);
        this.m_updateSqlElement = (DalSqlElement) element;
        this.m_parentUpdate.ProvisionHelper.AddProject(intValue3);
      }
    }

    public void UpdateWorkItemType()
    {
      this.GetAttributeInt("ID", true, 0, out this.m_inputId);
      int intValue1;
      bool nameChanged = this.GetAttributeInt("NameConstantID", false, 0, out intValue1);
      int intValue2;
      this.GetAttributeTempId("TempNameConstantID", false, 0, out intValue2);
      int intValue3;
      bool attributeInt = this.GetAttributeInt("ProjectID", false, 0, out intValue3);
      int intValue4;
      this.GetAttributeInt("DescriptionID", false, -1, out intValue4);
      int intValue5;
      this.GetAttributeTempId("TempDescriptionID", false, 0, out intValue5);
      bool outValue;
      this.GetAttributeBool("Deleted", false, false, out outValue);
      string stringValue;
      this.GetAttributeString("Cachestamp", false, (string) null, out stringValue);
      if (intValue2 > 0)
        nameChanged = true;
      if (!(nameChanged | attributeInt | outValue) && intValue4 < 0 && intValue5 <= 0)
        throw new ArgumentException(DalResourceStrings.Get("UpdateWorkItemTypeNothingToUpdate"), "updateElement");
      int actionId;
      this.Initialize(false, "AuthorizeWorkItemTypeChanges", "Rule", this.m_parentUpdate.ChangeWorkItemTypeGroup, this.m_parentUpdate.CheckWorkItemTypeGroup, out actionId);
      DalChangeWorkItemTypeElement element = DalSqlElement.GetElement<DalChangeWorkItemTypeElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeWorkItemTypeGroup, actionId, intValue1, intValue2, nameChanged, intValue3, attributeInt, intValue4, intValue5, outValue, stringValue, false, this.m_parentUpdate.Overwrite);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddWorkItemType(this.m_inputId);
    }

    public void DestroyWorkItemType()
    {
      string stringValue1;
      this.GetAttributeString("ProjectName", true, string.Empty, out stringValue1);
      string stringValue2;
      this.GetAttributeString("WorkItemTypeName", true, string.Empty, out stringValue2);
      DalSqlElement.GetElement<DalDestroyWorkItemTypeElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.ChangeWorkItemTypeGroup, stringValue2, stringValue1);
      this.m_parentUpdate.ProvisionHelper.AddProject(stringValue1);
    }

    public void DestroyGlobalList()
    {
      string stringValue;
      this.GetAttributeString("ListName", true, string.Empty, out stringValue);
      bool outValue;
      this.GetAttributeBool("ForceDelete", false, false, out outValue);
      DalSqlElement.GetElement<DalDestroyGlobalListElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.ChangeConstantSetGroup, stringValue, outValue);
      this.m_parentUpdate.ProvisionHelper.AddProject(0);
    }

    public void InsertWorkItemTypeUsage()
    {
      int intValue1;
      int num1 = this.GetAttributeInt("FieldID", false, 0, out intValue1) ? 1 : 0;
      int intValue2;
      bool attributeTempId1 = this.GetAttributeTempId("TempFieldID", false, 0, out intValue2);
      int intValue3;
      bool attributeInt = this.GetAttributeInt("WorkItemTypeID", false, 0, out intValue3);
      int intValue4;
      bool attributeTempId2 = this.GetAttributeTempId("TempWorkItemTypeID", false, 0, out intValue4);
      bool outValue;
      this.GetAttributeBool("GreyOut", false, false, out outValue);
      int num2 = attributeTempId1 ? 1 : 0;
      if ((num1 | num2) == 0)
        throw new ArgumentException(DalResourceStrings.Get("InsertWorkItemTypeUsageMissingFieldID"), "updateElement");
      if (!(attributeInt | attributeTempId2))
        throw new ArgumentException(DalResourceStrings.Get("InsertWorkItemTypeUsageMissingWorkItemTypeID"), "updateElement");
      int actionId;
      this.Initialize(true, "AuthorizeWorkItemTypeChanges", "Rule", this.m_parentUpdate.ChangeWorkItemTypeUsageGroup, this.m_parentUpdate.CheckWorkItemTypeGroup, out actionId);
      DalChangeWorkItemTypeUsageElement element = DalSqlElement.GetElement<DalChangeWorkItemTypeUsageElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeWorkItemTypeUsageGroup, actionId, intValue1, intValue2, intValue3, intValue4, outValue, true, false, (string) null, true, this.m_parentUpdate.Overwrite);
      this.m_updateSqlElement = (DalSqlElement) element;
      if (intValue4 != 0)
        return;
      this.m_parentUpdate.ProvisionHelper.AddWorkItemType(intValue3);
    }

    public void UpdateWorkItemTypeUsage()
    {
      this.GetAttributeInt("ID", true, 0, out this.m_inputId);
      bool outValue1;
      bool attributeBool = this.GetAttributeBool("GreyOut", false, false, out outValue1);
      bool outValue2;
      this.GetAttributeBool("Deleted", false, false, out outValue2);
      string stringValue;
      this.GetAttributeString("Cachestamp", false, (string) null, out stringValue);
      if (!(attributeBool | outValue2))
        throw new ArgumentException(DalResourceStrings.Get("UpdateWorkItemTypeUsageNothingToUpdate"), "updateElement");
      int actionId;
      this.Initialize(false, "AuthorizeWorkItemTypeChanges", "Rule", this.m_parentUpdate.ChangeWorkItemTypeUsageGroup, this.m_parentUpdate.CheckWorkItemTypeGroup, out actionId);
      DalChangeWorkItemTypeUsageElement element = DalSqlElement.GetElement<DalChangeWorkItemTypeUsageElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeWorkItemTypeUsageGroup, actionId, 0, 0, 0, 0, outValue1, attributeBool, outValue2, stringValue, false, this.m_parentUpdate.Overwrite);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddWorkItemTypeUsage(this.m_inputId);
    }

    public void InsertAction()
    {
      string stringValue;
      this.GetAttributeString("Name", true, (string) null, out stringValue);
      int intValue1;
      int num1 = this.GetAttributeInt("WorkItemTypeID", false, 0, out intValue1) ? 1 : 0;
      int intValue2;
      bool attributeTempId1 = this.GetAttributeTempId("TempWorkItemTypeID", false, 0, out intValue2);
      int intValue3;
      bool attributeInt1 = this.GetAttributeInt("FromStateConstantID", false, 0, out intValue3);
      int intValue4;
      bool attributeTempId2 = this.GetAttributeTempId("TempFromStateConstantID", false, 0, out intValue4);
      int intValue5;
      bool attributeInt2 = this.GetAttributeInt("ToStateConstantID", false, 0, out intValue5);
      int intValue6;
      bool attributeTempId3 = this.GetAttributeTempId("TempToStateConstantID", false, 0, out intValue6);
      if (stringValue.Length > (int) byte.MaxValue)
        throw new ArgumentException(DalResourceStrings.Get("InsertActionNameTooLong"), "updateElement");
      int num2 = attributeTempId1 ? 1 : 0;
      if ((num1 | num2) == 0)
        throw new ArgumentException(DalResourceStrings.Get("InsertActionMissingWorkItemTypeId"), "updateElement");
      if (!(attributeInt1 | attributeTempId2))
        throw new ArgumentException(DalResourceStrings.Get("InsertActionMissingFromConstantId"), "updateElement");
      if (!(attributeInt2 | attributeTempId3))
        throw new ArgumentException(DalResourceStrings.Get("InsertActionMissingToConstantId"), "updateElement");
      int actionId;
      this.Initialize(true, "AuthorizeActionChanges", "Rule", this.m_parentUpdate.ChangeActionGroup, this.m_parentUpdate.CheckActionGroup, out actionId);
      DalChangeActionElement element = DalSqlElement.GetElement<DalChangeActionElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeActionGroup, actionId, stringValue, intValue1, intValue2, intValue3, intValue4, intValue5, intValue6, false, (string) null, true, this.m_parentUpdate.Overwrite);
      this.m_updateSqlElement = (DalSqlElement) element;
      if (intValue2 != 0)
        return;
      this.m_parentUpdate.ProvisionHelper.AddWorkItemType(intValue1);
    }

    public void UpdateAction()
    {
      this.GetAttributeInt("ID", true, 0, out this.m_inputId);
      bool outValue;
      this.GetAttributeBool("Deleted", false, false, out outValue);
      string stringValue;
      this.GetAttributeString("Cachestamp", false, (string) null, out stringValue);
      if (!outValue)
        throw new ArgumentException(DalResourceStrings.Get("UpdateActionNothingToUpdate"), "updateElement");
      int actionId;
      this.Initialize(false, "AuthorizeActionChanges", "Rule", this.m_parentUpdate.ChangeActionGroup, this.m_parentUpdate.CheckActionGroup, out actionId);
      DalChangeActionElement element = DalSqlElement.GetElement<DalChangeActionElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeActionGroup, actionId, (string) null, 0, 0, 0, 0, 0, 0, outValue, stringValue, false, this.m_parentUpdate.Overwrite);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddAction(this.m_inputId);
    }

    public void InsertLinkType()
    {
      this.InitializeLinkTypes();
      string stringValue1;
      this.GetAttributeString("ReferenceName", true, (string) null, out stringValue1);
      string stringValue2;
      this.GetAttributeString("ForwardName", true, (string) null, out stringValue2);
      string stringValue3;
      this.GetAttributeString("ReverseName", false, (string) null, out stringValue3);
      int intValue;
      this.GetAttributeInt("Rules", false, 0, out intValue);
      if (!ValidationMethods.IsValidLinkTypeReferenceNameForImport(stringValue1))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkTypeInvalidReferenceName"), (object) stringValue1));
      if (!ValidationMethods.IsValidLinkTypeName(stringValue2))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkTypeInvalidFriendlyName"), (object) stringValue2));
      if (!string.IsNullOrEmpty(stringValue3) && !ValidationMethods.IsValidFieldName(stringValue3))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkTypeInvalidFriendlyName"), (object) stringValue3));
      DalAddLinkTypeElement element = DalSqlElement.GetElement<DalAddLinkTypeElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.AddLinkTypeGroup, stringValue1, stringValue2, stringValue3, intValue);
      this.m_updateSqlElement = (DalSqlElement) element;
    }

    public void UpdateLinkType()
    {
      this.InitializeLinkTypes();
      string stringValue1;
      this.GetAttributeString("ReferenceName", true, (string) null, out stringValue1);
      string stringValue2;
      this.GetAttributeString("ForwardName", false, (string) null, out stringValue2);
      string stringValue3;
      this.GetAttributeString("ReverseName", false, (string) null, out stringValue3);
      int intValue;
      this.GetAttributeInt("Rules", false, -1, out intValue);
      if (!ValidationMethods.IsValidLinkTypeReferenceName(stringValue1))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkTypeInvalidReferenceName"), (object) stringValue1));
      if (!string.IsNullOrEmpty(stringValue2) && !ValidationMethods.IsValidLinkTypeName(stringValue2))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkTypeInvalidFriendlyName"), (object) stringValue2));
      if (!string.IsNullOrEmpty(stringValue3) && !ValidationMethods.IsValidFieldName(stringValue3))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkTypeInvalidFriendlyName"), (object) stringValue3));
      DalUpdateLinkTypeElement element = DalSqlElement.GetElement<DalUpdateLinkTypeElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.UpdateLinkTypeGroup, stringValue1, stringValue2, stringValue3, intValue);
      this.m_updateSqlElement = (DalSqlElement) element;
    }

    public void DeleteLinkType()
    {
      this.InitializeLinkTypes();
      string stringValue;
      this.GetAttributeString("ReferenceName", true, (string) null, out stringValue);
      DalDeleteLinkTypeElement element = DalSqlElement.GetElement<DalDeleteLinkTypeElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.DeleteLinkTypeGroup, stringValue);
      if (!ValidationMethods.IsValidLinkTypeReferenceName(stringValue))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertLinkTypeInvalidReferenceName"), (object) stringValue));
      this.m_updateSqlElement = (DalSqlElement) element;
    }

    public void InsertWorkItemTypeCategory()
    {
      int categoryID = -1;
      int intValue1;
      this.GetAttributeTempId("TempID", false, -1, out intValue1);
      int intValue2;
      this.GetAttributeInt("ProjectID", true, 0, out intValue2);
      string stringValue1;
      this.GetAttributeString("Name", true, string.Empty, out stringValue1);
      string stringValue2;
      this.GetAttributeString("ReferenceName", true, string.Empty, out stringValue2);
      int intValue3;
      this.GetAttributeInt("DefaultWorkItemTypeID", false, -1, out intValue3);
      int intValue4;
      this.GetAttributeTempId("TempDefaultWorkItemTypeID", false, -1, out intValue4);
      if (!ValidationMethods.IsValidWorkItemTypeCategoryName(stringValue1))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertWorkItemTypeCategoryInvalidFriendlyName"), (object) stringValue1));
      if (!ValidationMethods.IsValidWorkItemTypeCategoryReferenceName(stringValue2))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertWorkItemTypeCategoryInvalidReferenceName"), (object) stringValue2));
      if (intValue3 == -1 && intValue4 == -1)
        throw new ArgumentException(DalResourceStrings.Get("InsertWorkItemTypeCategoryMissingWorkItemType"));
      if (intValue1 == -1)
        intValue1 = this.GetNextActionId();
      DalChangeWorkItemTypeCategoryElement element = DalSqlElement.GetElement<DalChangeWorkItemTypeCategoryElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeCategoryGroup, categoryID, intValue1, intValue2, stringValue1, stringValue2, intValue3, intValue4, this.m_parentUpdate.Overwrite);
      if (this.m_parentUpdate.CheckCategoryGroup.ElementCount == 0)
        DalSqlElement.GetElement<DalCheckWorkItemTypeCategoryElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.CheckCategoryGroup);
      this.m_updateSqlElement = (DalSqlElement) element;
      this.m_parentUpdate.ProvisionHelper.AddProject(intValue2);
      this.m_operations.Add((AdminUpdateEventData) new WorkItemTypeCategoryUpdateEventData()
      {
        ChangeType = WorkItemTypeCategoryUpdateType.InsertCategory,
        TempCategoryID = intValue1,
        DefaultWorkItemTypeId = intValue3,
        TempDefaultWorkItemTypeId = intValue4,
        ProjectId = intValue2,
        Name = stringValue1,
        ReferenceName = stringValue2
      });
    }

    public void UpdateWorkItemTypeCategory()
    {
      int tempCategoryID = -1;
      int projectID = -1;
      string refName = (string) null;
      int intValue1;
      this.GetAttributeInt("CategoryID", true, 0, out intValue1);
      string stringValue;
      this.GetAttributeString("Name", true, (string) null, out stringValue);
      int intValue2;
      this.GetAttributeInt("DefaultWorkItemTypeID", false, -1, out intValue2);
      int intValue3;
      this.GetAttributeTempId("TempDefaultWorkItemTypeID", false, -1, out intValue3);
      if (!ValidationMethods.IsValidWorkItemTypeCategoryName(stringValue))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, DalResourceStrings.Get("InsertWorkItemTypeCategoryInvalidFriendlyName"), (object) stringValue));
      if (intValue2 == -1 && intValue3 == -1)
        throw new ArgumentException(DalResourceStrings.Get("InsertWorkItemTypeCategoryMissingWorkItemType"));
      DalSqlElement.GetElement<DalChangeWorkItemTypeCategoryElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.ChangeCategoryGroup, intValue1, tempCategoryID, projectID, stringValue, refName, intValue2, intValue3, this.m_parentUpdate.Overwrite);
      if (this.m_parentUpdate.CheckCategoryGroup.ElementCount == 0)
        DalSqlElement.GetElement<DalCheckWorkItemTypeCategoryElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.CheckCategoryGroup);
      this.m_parentUpdate.ProvisionHelper.AddWorkItemTypeCategory(intValue1);
      this.m_operations.Add((AdminUpdateEventData) new WorkItemTypeCategoryUpdateEventData()
      {
        ChangeType = WorkItemTypeCategoryUpdateType.UpdateCategory,
        CategoryId = intValue1,
        ProjectId = projectID,
        Name = stringValue,
        ReferenceName = refName,
        DefaultWorkItemTypeId = intValue2,
        TempDefaultWorkItemTypeId = intValue3
      });
    }

    public void DestroyWorkItemTypeCategory()
    {
      int intValue;
      this.GetAttributeInt("CategoryID", true, 0, out intValue);
      DalSqlElement.GetElement<DalDestroyWorkItemTypeCategoryElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.DestroyCategoryGroup, intValue);
      this.m_parentUpdate.ProvisionHelper.AddWorkItemTypeCategory(intValue);
      this.m_operations.Add((AdminUpdateEventData) new WorkItemTypeCategoryUpdateEventData()
      {
        ChangeType = WorkItemTypeCategoryUpdateType.DestroyCategory,
        CategoryId = intValue
      });
    }

    public void InsertWorkItemTypeCategoryMember()
    {
      int intValue1;
      this.GetAttributeInt("CategoryID", false, -1, out intValue1);
      int intValue2;
      this.GetAttributeTempId("TempCategoryID", false, -1, out intValue2);
      int intValue3;
      this.GetAttributeInt("WorkItemTypeID", false, -1, out intValue3);
      int intValue4;
      this.GetAttributeTempId("TempWorkItemTypeID", false, -1, out intValue4);
      if (intValue1 == -1 && intValue2 == -1)
        throw new ArgumentException(DalResourceStrings.Get("InsertWorkItemTypeCategoryMemberMissingCategory"));
      if (intValue3 == -1 && intValue4 == -1)
        throw new ArgumentException(DalResourceStrings.Get("InsertWorkItemTypeCategoryMemberMissingWorkItemType"));
      int nextActionId = this.GetNextActionId();
      DalInsertWorkItemTypeCategoryMemberElement element = DalSqlElement.GetElement<DalInsertWorkItemTypeCategoryMemberElement>(this.m_sqlBatch);
      element.JoinBatch(this.m_parentUpdate.ChangeCategoryMemberGroup, nextActionId, intValue1, intValue2, intValue3, intValue4, this.m_parentUpdate.Overwrite);
      this.m_updateSqlElement = (DalSqlElement) element;
      if (intValue2 <= 0)
        this.m_parentUpdate.ProvisionHelper.AddWorkItemTypeCategory(intValue1);
      this.m_operations.Add((AdminUpdateEventData) new WorkItemTypeCategoryUpdateEventData()
      {
        ChangeType = WorkItemTypeCategoryUpdateType.AddCategoryMember,
        CategoryId = intValue1,
        TempCategoryID = intValue2,
        WorkItemTypeId = intValue3,
        TempWorkItemTypeId = intValue4
      });
    }

    public void DeleteWorkItemTypeCategoryMember()
    {
      int intValue;
      this.GetAttributeInt("CategoryMemberID", true, 0, out intValue);
      DalSqlElement.GetElement<DalDeleteWorkItemTypeCategoryMemberElement>(this.m_sqlBatch).JoinBatch(this.m_parentUpdate.ChangeCategoryMemberGroup, intValue);
      this.m_parentUpdate.ProvisionHelper.AddWorkItemTypeCategoryMember(intValue);
      this.m_operations.Add((AdminUpdateEventData) new WorkItemTypeCategoryUpdateEventData()
      {
        ChangeType = WorkItemTypeCategoryUpdateType.DeleteCategoryMember,
        CategoryMemberId = intValue
      });
    }

    private void Initialize(
      bool newItem,
      string authorizeStoredProcedureName,
      string objectName,
      ElementGroup changeGroup,
      ElementGroup checkGroup,
      out int actionId)
    {
      actionId = this.InitializeActionId();
      DalSqlElement.GetElement<DalDeclareIssueIdVariableElement>(this.m_sqlBatch).JoinBatch(changeGroup, actionId);
      DalSqlElement.GetElement<DalSetIssueIdVariableElement>(this.m_sqlBatch).JoinBatch(changeGroup, actionId, this.m_inputId, newItem);
      if (checkGroup.ElementCount != 0 || string.IsNullOrEmpty(authorizeStoredProcedureName))
        return;
      DalSqlElement.GetElement<DalAdminChangeCheckElement>(this.m_sqlBatch).JoinBatch(checkGroup, authorizeStoredProcedureName, this.m_verbose);
    }

    private void InitializeRelatedGroups(
      string authorizeStoredProcedureName,
      string applyStoredProcedureName,
      ElementGroup changeGroup,
      ElementGroup checkGroup,
      ElementGroup applyGroup)
    {
      if (checkGroup.ElementCount == 0 && !string.IsNullOrEmpty(authorizeStoredProcedureName))
        DalSqlElement.GetElement<DalAdminChangeCheckElement>(this.m_sqlBatch).JoinBatch(checkGroup, authorizeStoredProcedureName, this.m_verbose);
      if (applyGroup == null || applyGroup.ElementCount != 0)
        return;
      DalSqlElement.GetElement<DalApplyChangesElement>(this.m_sqlBatch).JoinBatch(applyGroup, applyStoredProcedureName);
    }

    private void InitializeLinkTypes() => this.m_linkTypeUpdate = true;

    protected override int GetOutputId() => this.m_linkTypeUpdate ? this.m_parentUpdate.GetOutputId(-this.m_actionId) : base.GetOutputId();

    internal override void GenerateOutput() => this.RequestContext.TraceBlock(900743, 900744, "Update", "BaseUpdate", "AdminUpdate.GenerateOutput", (Action) (() =>
    {
      if (!this.m_linkTypeUpdate)
      {
        base.GenerateOutput();
      }
      else
      {
        XmlDocument ownerDocument = this.m_outputNode.OwnerDocument;
        PayloadTable.PayloadRow row = this.m_sqlBatch.ResultPayload.Tables[this.m_updateSqlElement.GetOutputRowSetIndex()].Rows[0];
        XmlAttribute attribute1 = ownerDocument.CreateAttribute("ReferenceName");
        attribute1.Value = ((string) row["ReferenceName"]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute1);
        XmlAttribute attribute2 = ownerDocument.CreateAttribute("ForwardName");
        attribute2.Value = ((string) row["ForwardName"]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute2);
        XmlAttribute attribute3 = ownerDocument.CreateAttribute("ForwardID");
        int num1 = (int) (short) row["ForwardID"];
        attribute3.Value = num1.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute3);
        XmlAttribute attribute4 = ownerDocument.CreateAttribute("ReverseName");
        attribute4.Value = ((string) row["ReverseName"]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute4);
        XmlAttribute attribute5 = ownerDocument.CreateAttribute("ReverseID");
        int num2 = (int) (short) row["ReverseID"];
        attribute5.Value = num2.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute5);
        XmlAttribute attribute6 = ownerDocument.CreateAttribute("Rules");
        int num3 = (int) row["Rules"];
        attribute6.Value = num3.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute6);
        XmlAttribute attribute7 = ownerDocument.CreateAttribute("fDeleted");
        bool flag = (bool) row["fDeleted"];
        attribute7.Value = flag.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute7);
        XmlAttribute attribute8 = ownerDocument.CreateAttribute("ChangedBy");
        attribute8.Value = ((int) row["ChangedBy"]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute8);
        XmlAttribute attribute9 = ownerDocument.CreateAttribute("ChangedDate");
        attribute9.Value = ((DateTime) row["ChangedDate"]).ToString("yyyy-MM-ddTHH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture);
        this.m_outputNode.Attributes.Append(attribute9);
      }
    }));

    public List<AdminUpdateEventData> Operations => this.m_operations;

    private InsertRuleAttributes CreateInsertRuleAttributes(int actionId)
    {
      this.InitializeRelatedGroups("AuthorizeRuleChanges", (string) null, (ElementGroup) null, this.m_parentUpdate.CheckRuleGroup, (ElementGroup) null);
      InsertRuleAttributes insertRuleAttributes = new InsertRuleAttributes();
      insertRuleAttributes.Id = -actionId;
      this.GetAttributeCacheStamp("Cachestamp", false, out insertRuleAttributes.cacheStamp);
      this.GetAttributeInt("AreaID", false, 0, out insertRuleAttributes.areaId);
      this.GetAttributeTempId("TempAreaID", false, 0, out insertRuleAttributes.tempAreaId);
      this.GetAttributeInt("RootTreeID", false, 0, out insertRuleAttributes.rootTreeId);
      this.GetAttributeTempId("TempRootTreeID", false, 0, out insertRuleAttributes.tempRootTreeId);
      this.GetAttributeInt("PersonID", false, -1, out insertRuleAttributes.personId);
      this.GetAttributeTempId("TempPersonID", false, 0, out insertRuleAttributes.tempPersonId);
      this.GetAttributeBool("InversePersonID", false, false, out insertRuleAttributes.inversePersonId);
      this.GetAttributeInt("ObjectTypeScopeID", false, 0, out insertRuleAttributes.objectTypeScopeId);
      this.GetAttributeTempId("TempObjectTypeScopeID", false, 0, out insertRuleAttributes.tempObjectTypeScopeId);
      this.GetAttributeBool("Unless", false, false, out insertRuleAttributes.unless);
      this.GetAttributeInt("IfFldID", false, 0, out insertRuleAttributes.ifFieldId);
      this.GetAttributeTempId("TempIfFldID", false, 0, out insertRuleAttributes.tempIfFieldId);
      this.GetAttributeBool("IfNot", false, false, out insertRuleAttributes.ifNot);
      this.GetAttributeInt("IfConstID", false, 0, out insertRuleAttributes.ifConstId);
      this.GetAttributeTempId("TempIfConstID", false, 0, out insertRuleAttributes.tempIfConstId);
      this.GetAttributeInt("If2FldID", false, 0, out insertRuleAttributes.if2FieldId);
      this.GetAttributeTempId("TempIf2FldID", false, 0, out insertRuleAttributes.tempIf2FieldId);
      this.GetAttributeBool("If2Not", false, false, out insertRuleAttributes.if2Not);
      this.GetAttributeInt("If2ConstID", false, 0, out insertRuleAttributes.if2ConstId);
      this.GetAttributeTempId("TempIf2ConstID", false, 0, out insertRuleAttributes.tempIf2ConstId);
      this.GetAttributeInt("ThenFldID", false, 0, out insertRuleAttributes.thenFieldId);
      this.GetAttributeTempId("TempThenFldID", false, 0, out insertRuleAttributes.tempThenFieldId);
      this.GetAttributeBool("ThenNot", false, false, out insertRuleAttributes.thenNot);
      this.GetAttributeBool("ThenLike", false, false, out insertRuleAttributes.thenLike);
      this.GetAttributeInt("ThenConstID", false, 0, out insertRuleAttributes.thenConstId);
      this.GetAttributeTempId("TempThenConstID", false, 0, out insertRuleAttributes.tempThenConstId);
      this.GetAttributeBool("ThenLeaf", false, false, out insertRuleAttributes.thenLeaf);
      this.GetAttributeBool("ThenInterior", false, false, out insertRuleAttributes.thenInterior);
      this.GetAttributeBool("ThenOneLevel", false, false, out insertRuleAttributes.thenOneLevel);
      this.GetAttributeBool("ThenTwoPlusLevels", false, false, out insertRuleAttributes.thenTwoPlusLevels);
      this.GetAttributeBool("ThenConstLargetext", false, false, out insertRuleAttributes.thenConstLargeText);
      this.GetAttributeBool("ThenImplicitEmpty", false, false, out insertRuleAttributes.thenImplicitEmpty);
      this.GetAttributeBool("ThenImplicitUnchanged", false, false, out insertRuleAttributes.thenImplicitUnchanged);
      this.GetAttributeBool("FlowDownTree", false, true, out insertRuleAttributes.flowDownTree);
      this.GetAttributeBool("FlowAroundTree", false, false, out insertRuleAttributes.flowAroundTree);
      this.GetAttributeBool("Reverse", false, false, out insertRuleAttributes.reverse);
      this.GetAttributeInt("Fld1ID", false, 0, out insertRuleAttributes.field1Id);
      this.GetAttributeTempId("TempFld1ID", false, 0, out insertRuleAttributes.tempField1Id);
      this.GetAttributeInt("Fld1IsConstID", false, 0, out insertRuleAttributes.field1IsConstId);
      this.GetAttributeTempId("TempFld1IsConstID", false, 0, out insertRuleAttributes.tempField1IsConstId);
      this.GetAttributeInt("Fld1WasConstID", false, 0, out insertRuleAttributes.field1WasConstId);
      this.GetAttributeTempId("TempFld1WasConstID", false, 0, out insertRuleAttributes.tempField1WasConstId);
      this.GetAttributeInt("Fld2ID", false, 0, out insertRuleAttributes.field2Id);
      this.GetAttributeTempId("TempFld2ID", false, 0, out insertRuleAttributes.tempField2Id);
      this.GetAttributeInt("Fld2IsConstID", false, 0, out insertRuleAttributes.field2IsConstId);
      this.GetAttributeTempId("TempFld2IsConstID", false, 0, out insertRuleAttributes.tempField2IsConstId);
      this.GetAttributeInt("Fld2WasConstID", false, 0, out insertRuleAttributes.field2WasConstId);
      this.GetAttributeTempId("TempFld2WasConstID", false, 0, out insertRuleAttributes.tempField2WasConstId);
      this.GetAttributeInt("Fld3ID", false, 0, out insertRuleAttributes.field3Id);
      this.GetAttributeTempId("TempFld3ID", false, 0, out insertRuleAttributes.tempField3Id);
      this.GetAttributeInt("Fld3IsConstID", false, 0, out insertRuleAttributes.field3IsConstId);
      this.GetAttributeTempId("TempFld3IsConstID", false, 0, out insertRuleAttributes.tempField3IsConstId);
      this.GetAttributeInt("Fld3WasConstID", false, 0, out insertRuleAttributes.field3WasConstId);
      this.GetAttributeTempId("TempFld3WasConstID", false, 0, out insertRuleAttributes.tempField3WasConstId);
      this.GetAttributeInt("Fld4ID", false, 0, out insertRuleAttributes.field4Id);
      this.GetAttributeTempId("TempFld4ID", false, 0, out insertRuleAttributes.tempField4Id);
      this.GetAttributeInt("Fld4IsConstID", false, 0, out insertRuleAttributes.field4IsConstId);
      this.GetAttributeTempId("TempFld4IsConstID", false, 0, out insertRuleAttributes.tempField4IsConstId);
      this.GetAttributeInt("Fld4WasConstID", false, 0, out insertRuleAttributes.field4WasConstId);
      this.GetAttributeTempId("TempFld4WasConstID", false, 0, out insertRuleAttributes.tempField4WasConstId);
      this.GetAttributeBool("fAcl", false, false, out insertRuleAttributes.fAcl);
      this.GetAttributeBool("GrantRead", false, out insertRuleAttributes.grantRead);
      this.GetAttributeBool("DenyRead", false, out insertRuleAttributes.denyRead);
      this.GetAttributeBool("GrantWrite", false, out insertRuleAttributes.grantWrite);
      this.GetAttributeBool("DenyWrite", false, out insertRuleAttributes.denyWrite);
      this.GetAttributeBool("GrantAdmin", false, out insertRuleAttributes.grantAdmin);
      this.GetAttributeBool("DenyAdmin", false, out insertRuleAttributes.denyAdmin);
      this.GetAttributeBool("Default", false, out insertRuleAttributes.defaultAttrib);
      this.GetAttributeBool("Suggestion", false, out insertRuleAttributes.suggestion);
      this.GetAttributeBool("Helptext", false, out insertRuleAttributes.helpText);
      if (insertRuleAttributes.thenFieldId == 0 && insertRuleAttributes.tempThenFieldId == 0 && (insertRuleAttributes.field1Id != 0 || insertRuleAttributes.tempField1Id != 0))
      {
        if (insertRuleAttributes.field4Id != 0 || insertRuleAttributes.tempField4Id != 0)
        {
          insertRuleAttributes.thenFieldId = insertRuleAttributes.field4Id;
          insertRuleAttributes.tempThenFieldId = insertRuleAttributes.tempField4Id;
          insertRuleAttributes.thenConstId = insertRuleAttributes.field4IsConstId;
          insertRuleAttributes.tempThenConstId = insertRuleAttributes.tempField4IsConstId;
          insertRuleAttributes.field4IsConstId = 0;
          if (insertRuleAttributes.field4WasConstId == 0)
          {
            insertRuleAttributes.field4Id = 0;
            insertRuleAttributes.tempField4Id = 0;
          }
        }
        else if (insertRuleAttributes.field3Id != 0 || insertRuleAttributes.tempField3Id != 0)
        {
          insertRuleAttributes.thenFieldId = insertRuleAttributes.field3Id;
          insertRuleAttributes.tempThenFieldId = insertRuleAttributes.tempField3Id;
          insertRuleAttributes.thenConstId = insertRuleAttributes.field3IsConstId;
          insertRuleAttributes.tempThenConstId = insertRuleAttributes.tempField3IsConstId;
          insertRuleAttributes.field3IsConstId = 0;
          if (insertRuleAttributes.field3WasConstId == 0)
          {
            insertRuleAttributes.field3Id = 0;
            insertRuleAttributes.tempField3Id = 0;
          }
        }
        else if (insertRuleAttributes.field2Id != 0)
        {
          insertRuleAttributes.thenFieldId = insertRuleAttributes.field2Id;
          insertRuleAttributes.tempThenFieldId = insertRuleAttributes.tempField2Id;
          insertRuleAttributes.thenConstId = insertRuleAttributes.field2IsConstId;
          insertRuleAttributes.tempThenConstId = insertRuleAttributes.tempField2IsConstId;
          insertRuleAttributes.field2IsConstId = 0;
          if (insertRuleAttributes.field2WasConstId == 0)
          {
            insertRuleAttributes.field2Id = 0;
            insertRuleAttributes.tempField2Id = 0;
          }
        }
        else if (insertRuleAttributes.field1Id != 0)
        {
          insertRuleAttributes.thenFieldId = insertRuleAttributes.field1Id;
          insertRuleAttributes.tempThenFieldId = insertRuleAttributes.tempField1Id;
          insertRuleAttributes.thenConstId = insertRuleAttributes.field1IsConstId;
          insertRuleAttributes.tempThenConstId = insertRuleAttributes.tempField1IsConstId;
          insertRuleAttributes.field1IsConstId = 0;
          if (insertRuleAttributes.field1WasConstId == 0)
          {
            insertRuleAttributes.field1Id = 0;
            insertRuleAttributes.tempField1Id = 0;
          }
        }
      }
      if (insertRuleAttributes.objectTypeScopeId == -100 && (insertRuleAttributes.ifFieldId == 25 || insertRuleAttributes.if2FieldId == 25 || insertRuleAttributes.field1Id == 25 || insertRuleAttributes.field2Id == 25 || insertRuleAttributes.field3Id == 25 || insertRuleAttributes.field4Id == 25) && insertRuleAttributes.thenFieldId == -14 && insertRuleAttributes.thenConstLargeText)
        insertRuleAttributes.personId = -1;
      return insertRuleAttributes;
    }

    private InsertRuleAttributes CreateSuggestedValueInsertRuleAttributes()
    {
      InsertRuleAttributes insertRuleAttributes = this.CreateInsertRuleAttributes(this.GetNextActionId());
      insertRuleAttributes.suggestion = new bool?(true);
      insertRuleAttributes.denyWrite = new bool?(false);
      insertRuleAttributes.unless = false;
      insertRuleAttributes.thenConstId = -1;
      insertRuleAttributes.field2Id = 0;
      insertRuleAttributes.field2IsConstId = 0;
      insertRuleAttributes.field2WasConstId = 0;
      insertRuleAttributes.tempField2Id = 0;
      insertRuleAttributes.tempField2IsConstId = 0;
      insertRuleAttributes.tempField2WasConstId = 0;
      insertRuleAttributes.tempThenConstId = 0;
      return insertRuleAttributes;
    }

    private static bool IsAllowedValueRule(InsertRuleAttributes rule) => rule.denyWrite.HasValue && rule.denyWrite.Value && rule.unless && rule.thenLeaf && rule.thenOneLevel && !rule.thenNot && !rule.thenLike && rule.thenImplicitEmpty && (rule.thenConstId != 0 || rule.tempThenConstId != 0) && rule.if2FieldId == 0 && rule.tempIf2FieldId == 0;

    private static bool IsValidUserRule(InsertRuleAttributes rule) => rule.if2FieldId == 0 && rule.tempIf2FieldId == 0 && rule.thenConstId == -2 && rule.denyWrite.HasValue && rule.denyWrite.Value && rule.unless && rule.thenLeaf && rule.thenOneLevel && rule.thenTwoPlusLevels && !rule.thenNot && !rule.thenLike;
  }
}
