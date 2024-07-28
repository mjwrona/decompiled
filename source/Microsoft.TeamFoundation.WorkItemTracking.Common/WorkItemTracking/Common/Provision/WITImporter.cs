// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.WITImporter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WITImporter
  {
    public string MethodologyName;
    private IMetadataProvisioningHelper m_metadata;
    private UpdatePackageData m_updatePackageData;
    private MetaID m_currentTypeNameID;
    private int m_projectId;
    private string m_processName;
    private bool m_provisionRules;
    private bool m_provisionFields;
    private bool m_provisionActions;
    private bool m_provisionWorkItemTypes;
    private UpdatePackageFieldCollection m_fieldDefinitions;
    private Dictionary<MetaID, UpdatePackageCondition> m_stateConditions;
    private Dictionary<MetaID, Dictionary<string, string>> m_actionsMap;
    private List<WorkItemTypeDefinition> m_workItemTypes;
    private List<XmlElement> m_globalLists;
    private WorkItemTypeDefinition m_currentWitd;
    private MetaID m_validWorkItemTypes;

    public int ProjectId
    {
      get => this.m_projectId;
      set => this.m_projectId = value;
    }

    public IEnumerable<WorkItemTypeDefinition> WorkItemTypes => (IEnumerable<WorkItemTypeDefinition>) this.m_workItemTypes;

    public bool ProvisionRules => this.m_provisionRules;

    public WITImporter(
      IMetadataProvisioningHelper metadata,
      int projectId = 0,
      string methodologyName = null,
      string processName = null,
      bool isProvisionRulesEnabled = true,
      bool isProvisionFieldsEnabled = true,
      bool isProvisionActions = true,
      bool isProvisionWorkItemTypes = true)
    {
      this.m_metadata = metadata;
      this.m_projectId = projectId;
      this.m_processName = processName;
      this.m_provisionRules = isProvisionRulesEnabled;
      this.m_provisionFields = isProvisionFieldsEnabled;
      this.m_provisionActions = isProvisionActions;
      this.m_provisionWorkItemTypes = isProvisionWorkItemTypes;
      this.MethodologyName = methodologyName;
      this.m_workItemTypes = new List<WorkItemTypeDefinition>();
      this.m_globalLists = new List<XmlElement>();
    }

    public void AddWorkItemTypeDefinition(XmlElement typeElement) => this.m_workItemTypes.Add(new WorkItemTypeDefinition()
    {
      TypeName = typeElement.GetAttribute(ProvisionAttributes.WorkItemTypeName),
      TypeRefName = typeElement.GetAttribute(ProvisionAttributes.ReferenceName),
      Description = (XmlElement) typeElement.SelectSingleNode(ProvisionTags.Description),
      FieldDefinitions = (XmlElement) typeElement.SelectSingleNode(ProvisionTags.FieldDefinitions),
      Workflow = (XmlElement) typeElement.SelectSingleNode(ProvisionTags.Workflow),
      Form = (XmlElement) typeElement.SelectSingleNode(ProvisionTags.Form)
    });

    public void AddGlobalLists(XmlElement listsElement)
    {
      if (listsElement == null)
        return;
      foreach (XmlNode xmlNode in (XmlNode) listsElement)
      {
        if (xmlNode is XmlElement xmlElement)
          this.m_globalLists.Add(xmlElement);
      }
    }

    public XmlDocument Translate()
    {
      this.m_updatePackageData = new UpdatePackageData(this.m_metadata, new MetaIDFactory(), this.ProjectId, this.MethodologyName);
      this.m_fieldDefinitions = new UpdatePackageFieldCollection(this.m_updatePackageData);
      UpdatePackage batch = new UpdatePackage(this.m_updatePackageData);
      if (this.m_globalLists.Count > 0)
        this.ProcessGlobalLists(batch);
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) this.m_updatePackageData.Metadata.ServerStringComparer);
      HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
      foreach (WorkItemTypeDefinition workItemType in this.WorkItemTypes)
      {
        this.m_currentWitd = workItemType;
        UpdatePackageRuleContext context = new UpdatePackageRuleContext(this.m_metadata);
        if (!string.IsNullOrEmpty(this.m_currentWitd.TypeName))
        {
          if (stringSet1.Contains(this.m_currentWitd.TypeName))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorWorkItemTypeNameNotUnique", (object) this.m_currentWitd.TypeName));
          stringSet1.Add(this.m_currentWitd.TypeName);
          if (!string.IsNullOrEmpty(this.m_currentWitd.TypeRefName))
          {
            if (!ValidationMethods.IsValidReferenceFieldName(this.m_currentWitd.TypeRefName))
              this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidWorkItemTypeReferenceName", (object) this.m_currentWitd.TypeRefName));
            if (stringSet2.Contains(this.m_currentWitd.TypeRefName))
              this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorWorkItemTypeReferenceNameNotUnique", (object) this.m_currentWitd.TypeRefName));
            stringSet2.Add(this.m_currentWitd.TypeRefName);
          }
          this.m_currentTypeNameID = batch.InsertConstant(this.m_currentWitd.TypeName, (MetaID) null);
          if (this.m_provisionWorkItemTypes)
          {
            MetaID descrID = batch.InsertTreeProperty(this.ProjectId, Guid.NewGuid().ToString(), this.m_currentWitd.Description == null ? string.Empty : this.m_currentWitd.Description.InnerText);
            this.m_updatePackageData.CurrentTypeId = batch.InsertWorkItemType(this.ProjectId, this.m_currentTypeNameID, descrID);
          }
          MetaID fieldId = this.m_updatePackageData.MetaIDFactory.NewMetaID(25);
          context.Push(new UpdatePackageCondition(this.m_updatePackageData, fieldId, this.m_currentTypeNameID));
        }
        else
        {
          MetaID metaId = this.m_updatePackageData.MetaIDFactory.NewMetaID(0);
          batch.InsertWorkItemType(this.ProjectId, metaId, metaId);
          this.m_updatePackageData.CurrentTypeId = this.m_updatePackageData.MetaIDFactory.NewMetaID(-this.ProjectId);
          context.Push(new UpdatePackageCondition());
        }
        if (this.m_currentWitd.FieldDefinitions != null && (this.m_provisionRules || this.m_provisionFields))
          this.m_fieldDefinitions.ProcessFieldDefinitions(context, this.m_currentWitd.FieldDefinitions, batch, this.m_provisionRules, this.m_provisionFields);
        if (this.m_currentWitd.Workflow != null)
          this.ProcessWorkflow(context, batch);
        if (this.m_provisionRules && this.m_currentWitd.Form != null)
          this.ProcessForm(batch);
        if (!string.IsNullOrEmpty(this.m_currentWitd.TypeName))
          this.ProcessWorkItemType(batch);
      }
      this.m_updatePackageData.GlobalLists.CheckPendingItemsValidity();
      return batch.Xml;
    }

    private void ProcessGlobalLists(UpdatePackage batch)
    {
      foreach (XmlElement globalList in this.m_globalLists)
        this.m_updatePackageData.GlobalLists.Add(globalList, this.m_updatePackageData.MethodologyName == null);
      this.m_updatePackageData.GlobalLists.SubmitChanges(batch);
    }

    private void ProcessForm(UpdatePackage batch)
    {
      this.ProcessFieldNames();
      this.ProcessLegacyFormLayout();
      this.ProcessWebLayout();
      this.ProcessLabelControls();
      this.ProcessLinkElements();
      this.ProcessWebpageControlElements();
      batch.InsertForm(this.ProjectId, this.m_currentTypeNameID, this.m_currentWitd.Form.OuterXml);
    }

    private void ProcessFieldNames()
    {
      foreach (XmlNode selectNode in this.m_currentWitd.Form.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//@{0}", (object) ProvisionAttributes.ControlFieldName)))
      {
        string fieldName = selectNode.Value;
        if (!this.m_fieldDefinitions.Contains(fieldName))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorUnknownFormField", (object) fieldName));
      }
    }

    private void ProcessLegacyFormLayout()
    {
      foreach (XmlElement selectNode in this.m_currentWitd.Form.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}//{1}", (object) ProvisionTags.Layout, (object) ProvisionTags.Column)))
      {
        string trimmedAttribute1 = WITImporter.GetTrimmedAttribute(selectNode, ProvisionAttributes.ColumnFixedWidth);
        string trimmedAttribute2 = WITImporter.GetTrimmedAttribute(selectNode, ProvisionAttributes.ColumnPercentWidth);
        if (!string.IsNullOrEmpty(trimmedAttribute1) && !string.IsNullOrEmpty(trimmedAttribute2))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorTwoWidthElements", (object) ProvisionTags.Column, (object) ProvisionAttributes.ColumnFixedWidth, (object) ProvisionAttributes.ColumnPercentWidth));
        else if (string.IsNullOrEmpty(trimmedAttribute1) && string.IsNullOrEmpty(trimmedAttribute2))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorNoWidthElements", (object) ProvisionTags.Column, (object) ProvisionAttributes.ColumnFixedWidth, (object) ProvisionAttributes.ColumnPercentWidth));
      }
      this.ProcessLinksControls();
    }

    private void ThrowValidationException(string resourceName, params string[] args) => this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format(resourceName, (object[]) args));

    private void ThrowValidationWarning(string resourceName, params string[] args) => this.m_metadata.RaiseImportEvent((Exception) null, InternalsResourceStrings.Format(resourceName, (object[]) args));

    private void ProcessWebLayout()
    {
      if (!(this.m_currentWitd.Form.SelectSingleNode("WebLayout") is XmlElement webLayoutElement))
        return;
      this.m_metadata.ValidateRequiredFieldsOnLayout(this.m_currentWitd.FieldDefinitions, this.m_currentWitd.Workflow, this.m_currentWitd.Form, (Action<string>) (fieldName => this.ThrowValidationWarning("WebLayout_RequiredFieldsNotInBothLayouts", fieldName)));
      this.m_metadata.ValidateWebLayoutControls(webLayoutElement, (Action<string>) (controlType => this.ThrowValidationException("WebLayout_ControlTypeNotAllowed", controlType)), (Action) (() => this.ThrowValidationException("WebLayout_ControlHeightNotAllowed")), (Action<string>) (controlType => this.ThrowValidationWarning("WebLayout_ControlNotRecognized", controlType)));
      this.m_metadata.ValidateWebLayoutSystemControls(webLayoutElement, new ValidateWebLayoutSystemControlErrorActions()
      {
        SystemControlTypeErrorAction = (Action<string>) (controlType => this.ThrowValidationException("WebLayout_SystemControlTypeNotAllowed", controlType)),
        SystemControlFieldErrorAction = (Action<string>) (fieldName => this.ThrowValidationException("WebLayout_SystemControlFieldNameNotAllowed", fieldName)),
        SystemControlDuplicateErrorAction = (Action<string>) (controlName => this.ThrowValidationException("WebLayout_SystemControlNameDuplicated", controlName)),
        SystemControlReplacesFieldErrorAction = (Action<string>) (fieldNames => this.ThrowValidationException("WebLayout_SystemControlReplacesFieldNotAllowed", fieldNames)),
        SystemControlReplacesDuplicateFieldErrorAction = (Action<string>) (fieldName => this.ThrowValidationException("WebLayout_SystemControlReplacesFieldDuplicated", fieldName)),
        SystemControlHiddenFieldErrorAction = (Action<string>) (fieldNames => this.ThrowValidationException("WebLayout_SystemControlHiddenFieldNotAllowed", fieldNames))
      });
      try
      {
        this.m_metadata.ValidateWebLayoutExtensions(webLayoutElement, (Func<string, string[], bool>) ((fieldReferenceName, fieldTypes) =>
        {
          if (!this.m_fieldDefinitions.Contains(fieldReferenceName))
            return false;
          return fieldTypes == null || fieldTypes.Length == 0 || ((IEnumerable<string>) fieldTypes).Contains<string>(this.m_fieldDefinitions[fieldReferenceName].StringFieldType, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }));
      }
      catch (InvalidContributionTypeException ex)
      {
        this.ThrowValidationException("WebLayout_InvalidContributionType", ex.ContributionId, ex.ExpectedContributionType);
      }
      catch (InvalidContributionInputIdException ex)
      {
        this.ThrowValidationException("WebLayout_InvalidContributionInputId", ex.ContributionInputId, ex.ContributionId);
      }
      catch (InvalidContributionInputValueException ex)
      {
        this.ThrowValidationException("WebLayout_InvalidContributionInputValue", ex.ContributionInputValue, ex.ContributionInputId, ex.ContributionId);
      }
      catch (ContributionInputNotSpecifiedException ex)
      {
        this.ThrowValidationException("WebLayout_ContributionInputNotSpecified", ex.ContributionInputId, ex.ContributionId);
      }
      catch (InvalidContributionInputTypeException ex)
      {
        this.ThrowValidationException("WebLayout_InvalidContributionInputType", ex.ContributionInputType, ex.ContributionInputId, ex.ContributionId);
      }
      catch (FormExtensionNotFoundOrNoFormContribution ex)
      {
        this.ThrowValidationWarning("WebLayout_FormExtensionNotFoundOrNoFormContribution", ex.ExtensionId);
      }
      catch (ExtensionDoesNotContainValidFormContribution ex)
      {
        this.ThrowValidationWarning("WebLayout_FormExtensionDoesNotHavePageGroupOrControlContribution", ex.ExtensionId);
      }
      catch (ContributionNotFoundOrNotInListedExtensionsElement ex)
      {
        this.ThrowValidationWarning("WebLayout_ContributionNotFoundOrNotInListedExtensionsElement", ex.ContributionId);
      }
      catch (DuplicateGroupContributionsException ex)
      {
        this.ThrowValidationException("WebLayout_DuplicateGroupContributions", ex.GroupContributionId, ex.PageLabel);
      }
      catch (DuplicatePageContributionsException ex)
      {
        this.ThrowValidationException("WebLayout_DuplicatePageContributions", ex.PageContributionId);
      }
      catch (FormExtensionNotDeclaredException ex)
      {
        this.ThrowValidationException("WebLayout_FormExtensionNotDeclared");
      }
      this.m_metadata.ValidateLinksControls(webLayoutElement);
      this.m_metadata.GeneratePageAndGroupIds(this.m_processName, this.m_currentWitd.TypeName, webLayoutElement, (Action<string>) (pageLabel => this.ThrowValidationException("WebLayout_InvalidOrDuplicatedPageLabel", pageLabel)), (Action<string>) (groupLabel => this.ThrowValidationException("WebLayout_InvalidOrDuplicatedGroupLabel", groupLabel)), (Action<string>) (groupLabel => this.ThrowValidationException("WebLayout_InvalidControlsInGroup", groupLabel)), (Action<string, string>) ((fieldRefName, groupLabel) => this.ThrowValidationException("WebLayout_DuplicateControlsInGroup", fieldRefName, groupLabel)));
    }

    private void ProcessLinksControls()
    {
      bool flag = this.m_metadata.WorkItemLinkTypesCount > 0;
      foreach (XmlNode selectNode in this.m_currentWitd.Form.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}", (object) ProvisionTags.Layout)))
      {
        XmlNodeList xmlNodeList1 = selectNode.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Control));
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        for (int i = 0; i < xmlNodeList1.Count; ++i)
        {
          XmlElement xmlElement = (XmlElement) xmlNodeList1[i];
          string attribute1 = xmlElement.GetAttribute(ProvisionAttributes.ControlType);
          if (!string.IsNullOrEmpty(attribute1) && VssStringComparer.XmlAttributeValue.Equals(WellKnownControlNames.LinksControl, attribute1) && !xmlElement.ParentNode.Name.Equals(ProvisionTags.SystemControls))
          {
            string attribute2 = xmlElement.GetAttribute(ProvisionAttributes.ControlName);
            string key = attribute2 == null ? string.Empty : attribute2;
            if (dictionary.ContainsKey(key))
              this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlDuplicateName", (object) key));
            dictionary[key] = (object) null;
            if (!flag)
            {
              string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinksControlOptions);
              if (xmlElement.SelectNodes(xpath).Count > 0)
                this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlCannotContainLinksOptions", (object) ProvisionTags.LinksControlOptions));
            }
            string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WorkItemLinkFilters);
            XmlNodeList xmlNodeList2 = xmlElement.SelectNodes(xpath1);
            bool isExcludeAll = false;
            if (xmlNodeList2.Count == 1)
              this.ValidateLinksControlFilters(xmlNodeList2[0], false, out isExcludeAll);
            string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.ExternalLinkFilters);
            XmlNodeList xmlNodeList3 = xmlElement.SelectNodes(xpath2);
            if (xmlNodeList3.Count == 1)
              this.ValidateLinksControlFilters(xmlNodeList3[0], true, out bool _);
            string xpath3 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinkColumns);
            XmlNodeList xmlNodeList4 = xmlElement.SelectNodes(xpath3);
            if (xmlNodeList4.Count == 1)
              this.ValidateLinkColumns(xmlNodeList4[0]);
            string xpath4 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WorkItemTypeFilters);
            XmlNodeList xmlNodeList5 = xmlElement.SelectNodes(xpath4);
            if (xmlNodeList5.Count == 1)
            {
              if (isExcludeAll)
                this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlExcludeAllLinkTypesFiltersCannotContainWorkItemTypeFilters", (object) ProvisionTags.WorkItemTypeFilters, (object) ProvisionTags.WorkItemLinkFilters, (object) ProvisionAttributes.FilterType, (object) ProvisionValues.ExcludeAll));
              this.ValidateLinksControlWorkItemTypeFilters(xmlNodeList5[0]);
            }
          }
        }
      }
    }

    private void ValidateLinksControlWorkItemTypeFilters(XmlNode node)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Filter);
      XmlNodeList xmlNodeList = node.SelectNodes(xpath);
      string x = node.Attributes[ProvisionAttributes.FilterType].Value;
      if (VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.IncludeAll) && xmlNodeList.Count != 0)
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlAllFiltersCannotContainChildren", (object) node.Name, (object) ProvisionAttributes.FilterType, (object) x, (object) ProvisionTags.Filter, (object) ProvisionValues.Include));
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        string lowerInvariant = xmlNode.Attributes[ProvisionAttributes.WorkItemType].Value.Trim().ToLowerInvariant();
        if (dictionary.ContainsKey(lowerInvariant))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlDuplicateWITFilterName", (object) lowerInvariant));
        dictionary[lowerInvariant] = (object) null;
      }
    }

    private void ValidateLinksControlFilters(XmlNode node, bool external, out bool isExcludeAll)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Filter);
      XmlNodeList xmlNodeList = node.SelectNodes(xpath);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      string x = node.Attributes[ProvisionAttributes.FilterType].Value;
      isExcludeAll = VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.ExcludeAll);
      if ((VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.ExcludeAll) || VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.IncludeAll)) && xmlNodeList.Count != 0)
      {
        string str = isExcludeAll ? ProvisionValues.Exclude : ProvisionValues.Include;
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlAllFiltersCannotContainChildren", (object) node.Name, (object) ProvisionAttributes.FilterType, (object) x, (object) ProvisionTags.Filter, (object) str));
      }
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        string str = xmlNode.Attributes[ProvisionAttributes.LinkType].Value;
        if (dictionary.ContainsKey(str))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlDuplicateFilterName", (object) str));
        dictionary[str] = (object) null;
        bool flag = false;
        if (external)
        {
          foreach (string registeredLinkType in this.m_metadata.GetRegisteredLinkTypes())
          {
            if (VssStringComparer.LinkName.Equals(registeredLinkType, str))
            {
              flag = true;
              break;
            }
          }
        }
        else if (this.m_metadata.GetWorkItemLinkTypeReferenceNames().Contains(str))
        {
          flag = true;
          if (!this.m_metadata.IsLinkTypeDirectional(str) && xmlNode.Attributes[ProvisionAttributes.FilterOn] != null)
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlNonDirectionalLinkWithFilterOn", (object) str, (object) ProvisionAttributes.FilterOn));
        }
        else
          flag = true;
        if (!flag)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlUnknownFilterName", (object) str));
      }
    }

    private void ValidateLinkColumns(XmlNode node)
    {
      string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinkColumn);
      XmlNodeList xmlNodeList = node.SelectNodes(xpath);
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      foreach (XmlNode xmlNode in xmlNodeList)
      {
        if (xmlNode.Attributes.Count != 1)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Get("LinksControlLinkColumnInvalidAttributes"));
        string key = xmlNode.Attributes[0].Value;
        if (dictionary.ContainsKey(key))
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlLinkColumnDuplicateColumn", (object) key));
        dictionary[key] = (object) null;
        if (VssStringComparer.LinkName.Equals(xmlNode.Attributes[0].Name, ProvisionAttributes.LinkAttribute))
        {
          string x = xmlNode.Attributes[0].Value;
          if (!VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.ColumnLinkComment) && !VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.ColumnLinkType) && !VssStringComparer.XmlAttributeValue.Equals(x, ProvisionValues.ColumnTargetDescription))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("LinksControlLinkAttributeInvalid", (object) x, (object) ProvisionValues.ColumnLinkComment, (object) ProvisionValues.ColumnLinkType, (object) ProvisionValues.ColumnTargetDescription));
        }
      }
    }

    private void ProcessLabelControls()
    {
      XmlNodeList xmlNodeList1 = this.m_currentWitd.Form.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.Control));
      for (int i = 0; i < xmlNodeList1.Count; ++i)
      {
        XmlElement xmlElement = (XmlElement) xmlNodeList1[i];
        string xpath1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}", (object) ProvisionTags.LinkElement);
        XmlNodeList xmlNodeList2 = xmlElement.SelectNodes(xpath1);
        int num1 = xmlNodeList2 == null ? 0 : (xmlNodeList2.Count > 0 ? 1 : 0);
        string xpath2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}", (object) ProvisionTags.LabelElement);
        XmlNodeList xmlNodeList3 = xmlElement.SelectNodes(xpath2);
        int num2 = xmlNodeList3 == null ? 0 : (xmlNodeList3.Count > 0 ? 1 : 0);
        bool flag = xmlElement.Attributes[ProvisionAttributes.Label] != null;
        if (num1 != 0 && !flag)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorControlLabelAttrMissing", (object) xmlElement.OuterXml));
      }
    }

    private void ProcessWebpageControlElements()
    {
      XmlNodeList xmlNodeList1 = this.m_currentWitd.Form.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.WebpageControlOptions));
      for (int i = 0; i < xmlNodeList1.Count; ++i)
      {
        XmlElement xmlElement = (XmlElement) xmlNodeList1[i];
        string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}", (object) ProvisionTags.ContentElement);
        XmlNodeList xmlNodeList2 = xmlElement.SelectNodes(xpath);
        bool flag1 = xmlElement.HasAttribute(ProvisionAttributes.AllowScript);
        bool flag2 = xmlElement.HasAttribute(ProvisionAttributes.ReloadOnParamChange);
        if (xmlNodeList2 != null && xmlNodeList2.Count > 0 && flag1 | flag2)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("WebpageControlInvalidAttributes", (object) xmlElement.OuterXml));
      }
    }

    private void ProcessLinkElements()
    {
      XmlNodeList xmlNodeList1 = this.m_currentWitd.Form.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".//{0}", (object) ProvisionTags.LinkElement));
      for (int i = 0; i < xmlNodeList1.Count; ++i)
      {
        XmlElement linkElement = (XmlElement) xmlNodeList1[i];
        string attribute1 = linkElement.GetAttribute(ProvisionAttributes.UrlRoot);
        string attribute2 = linkElement.GetAttribute(ProvisionAttributes.UrlPath);
        bool isParentWebBrowser = this.IsParentWebBrowser(linkElement);
        this.ValidateUrlRoot(attribute1, isParentWebBrowser, linkElement.OuterXml);
        string xpath = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "./{0}", (object) ProvisionTags.ParamElement);
        XmlNodeList xmlNodeList2 = linkElement.SelectNodes(xpath);
        int count = xmlNodeList2 != null ? xmlNodeList2.Count : 0;
        string[] strArray = new string[count];
        try
        {
          string.Format((IFormatProvider) CultureInfo.InvariantCulture, attribute2, (object[]) strArray);
        }
        catch (FormatException ex)
        {
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorLinkParamsTooFew", (object) linkElement.OuterXml));
        }
        bool[] flagArray = new bool[count];
        foreach (XmlNode xmlNode in xmlNodeList2)
        {
          string str = xmlNode.Attributes[ProvisionAttributes.ParamValue].Value;
          int int32 = Convert.ToInt32(xmlNode.Attributes[ProvisionAttributes.Index].Value, (IFormatProvider) CultureInfo.CurrentCulture);
          if (int32 >= count)
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("InvalidParamIndexError", (object) int32, (object) count, (object) linkElement.OuterXml));
          if (flagArray[int32])
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ParamIndexDuplicateError", (object) int32, (object) linkElement.OuterXml));
          flagArray[int32] = true;
          if (!this.m_fieldDefinitions.Contains(str) && !this.m_metadata.ContainsField(str) && !this.IsValidUrlPathMacro(str))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidParam", (object) str, (object) linkElement.OuterXml));
        }
      }
    }

    private bool IsValidUrlPathMacro(string macro) => macro.Equals(ProvisionValues.ParamMacroMe, StringComparison.OrdinalIgnoreCase);

    private bool IsValidUrlRootMacro(string macro) => macro.Equals(ProvisionValues.ParamMacroProcessGuidanceUrl, StringComparison.OrdinalIgnoreCase) || macro.Equals(ProvisionValues.ParamMacroPortal, StringComparison.OrdinalIgnoreCase) || macro.Equals(ProvisionValues.ParamMacroReportServiceSiteUrl, StringComparison.OrdinalIgnoreCase) || macro.Equals(ProvisionValues.ParamMacroReportManagerUrl, StringComparison.OrdinalIgnoreCase);

    private void ValidateUrlRoot(string url, bool isParentWebBrowser, string errorXml)
    {
      if (this.IsValidUrlRootMacro(url))
        return;
      Uri uri = (Uri) null;
      try
      {
        uri = new Uri(url, UriKind.Absolute);
      }
      catch (UriFormatException ex)
      {
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("InvalidUrlRootFormat", (object) url, (object) ex.Message, (object) errorXml));
      }
      if (!isParentWebBrowser && uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeMailto)
      {
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("UnsupportedUriScheme", (object) uri.Scheme, (object) errorXml));
      }
      else
      {
        if (!isParentWebBrowser || !(uri.Scheme != Uri.UriSchemeHttp) || !(uri.Scheme != Uri.UriSchemeHttps))
          return;
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("UnsupportedUriSchemeWebpageControl", (object) uri.Scheme, (object) errorXml));
      }
    }

    private bool IsParentWebBrowser(XmlElement linkElement)
    {
      XmlNode parentNode = linkElement.ParentNode;
      return parentNode != null && parentNode.Name.Equals(ProvisionTags.WebpageControlOptions);
    }

    private void ProcessWorkItemType(UpdatePackage batch)
    {
      if (this.m_validWorkItemTypes == null)
      {
        string constant = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "__Valid work item types for ({0})", (object) this.ProjectId);
        MetaID listID = batch.InsertConstant(constant, (MetaID) null);
        if (this.m_provisionRules)
        {
          UpdatePackageRuleProperties props = new UpdatePackageRuleProperties();
          props[(object) "ThenFldID"] = (object) 25;
          UpdatePackage.SetListRuleProperties(ListRuleType.RequiredList, listID, (Hashtable) props);
          batch.InsertComment("Rule: basic list (work item type)");
          batch.InsertRule(this.ProjectId, props);
        }
        this.m_validWorkItemTypes = listID;
      }
      batch.InsertConstant(this.m_currentWitd.TypeName, this.m_validWorkItemTypes);
    }

    private void ProcessWorkflow(UpdatePackageRuleContext context, UpdatePackage batch)
    {
      this.m_actionsMap = new Dictionary<MetaID, Dictionary<string, string>>();
      this.m_stateConditions = new Dictionary<MetaID, UpdatePackageCondition>(10);
      Hashtable states = (Hashtable) null;
      if (this.m_provisionRules)
        this.ProcessStatesRules(context, batch, out states);
      MetaID defaultStateID = (MetaID) null;
      this.ProcessTransitions(states, context, batch, out defaultStateID);
      if (!this.m_provisionRules)
        return;
      if (defaultStateID == null)
        this.m_metadata.ThrowValidationException(InternalsResourceStrings.Get("ErrorNoDefaultState"));
      this.ProcessDefaultState(defaultStateID, context, batch);
    }

    private void ProcessStatesRules(
      UpdatePackageRuleContext context,
      UpdatePackage batch,
      out Hashtable states)
    {
      XmlNodeList xmlNodeList = this.m_currentWitd.Workflow.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ProvisionTags.States, (object) ProvisionTags.State));
      states = new Hashtable(xmlNodeList.Count + 1, (IEqualityComparer) this.m_metadata.ServerStringComparer);
      MetaID fieldId = this.m_updatePackageData.MetaIDFactory.NewMetaID(2);
      MetaID listID = batch.InsertConstant(Guid.NewGuid().ToString(), (MetaID) null);
      foreach (XmlElement xmlElement in xmlNodeList)
      {
        string trimmedAttribute = WITImporter.GetTrimmedAttribute(xmlElement, ProvisionAttributes.ListItemValue);
        if (states[(object) trimmedAttribute] != null)
          this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorManyStates", (object) trimmedAttribute));
        states[(object) trimmedAttribute] = (object) states.Count;
        MetaID metaId = batch.InsertConstant(trimmedAttribute, listID);
        UpdatePackageCondition condition = new UpdatePackageCondition(this.m_updatePackageData, fieldId, metaId);
        this.m_stateConditions[metaId] = condition;
        context.Push(condition);
        try
        {
          this.m_fieldDefinitions.StateAddReferencesToUpdatePackage(context, metaId, xmlElement, batch);
        }
        finally
        {
          context.Pop();
        }
      }
      UpdatePackageRuleProperties props = new UpdatePackageRuleProperties();
      props[(object) "ThenFldID"] = (object) 2;
      context.SetProperties(props);
      UpdatePackage.SetListRuleProperties(ListRuleType.RequiredList, listID, (Hashtable) props);
      batch.InsertComment("Rule: basic list (state)");
      batch.InsertRule(this.ProjectId, props);
    }

    private void ProcessTransitions(
      Hashtable states,
      UpdatePackageRuleContext context,
      UpdatePackage batch,
      out MetaID defaultStateID)
    {
      XmlNodeList xmlNodeList = this.m_currentWitd.Workflow.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ProvisionTags.Transitions, (object) ProvisionTags.Transition));
      defaultStateID = (MetaID) null;
      if (!this.m_provisionRules)
      {
        if (!this.m_provisionActions)
          return;
        foreach (XmlElement element in xmlNodeList)
        {
          string trimmedAttribute1 = WITImporter.GetTrimmedAttribute(element, ProvisionAttributes.OriginalState);
          string trimmedAttribute2 = WITImporter.GetTrimmedAttribute(element, ProvisionAttributes.NewState);
          MetaID toID = batch.InsertConstant(trimmedAttribute2, (MetaID) null);
          MetaID fromID = batch.InsertConstant(trimmedAttribute1, (MetaID) null);
          XmlElement actionsElement = (XmlElement) element.SelectSingleNode(ProvisionTags.Actions);
          if (actionsElement != null)
          {
            if (trimmedAttribute1.Length == 0)
              this.m_metadata.ThrowValidationException(InternalsResourceStrings.Get("ErrorActionsForInitialTransition"));
            this.ProcessActions(actionsElement, trimmedAttribute1, fromID, toID, batch);
          }
        }
      }
      else
      {
        MetaID fieldId = this.m_updatePackageData.MetaIDFactory.NewMetaID(2);
        if (states[(object) ""] == null)
          states[(object) ""] = (object) states.Count;
        bool[,] flagArray = new bool[states.Count, states.Count];
        for (int index1 = 0; index1 < states.Count; ++index1)
        {
          for (int index2 = 0; index2 < states.Count; ++index2)
            flagArray[index1, index2] = index1 == index2;
        }
        List<UpdatePackageTransition> packageTransitionList = new List<UpdatePackageTransition>(xmlNodeList.Count);
        foreach (XmlElement xmlElement in xmlNodeList)
        {
          string trimmedAttribute3 = WITImporter.GetTrimmedAttribute(xmlElement, ProvisionAttributes.OriginalState);
          string trimmedAttribute4 = WITImporter.GetTrimmedAttribute(xmlElement, ProvisionAttributes.NewState);
          string trimmedAttribute5 = WITImporter.GetTrimmedAttribute(xmlElement, ProvisionAttributes.TransitionAllowedFor);
          string trimmedAttribute6 = WITImporter.GetTrimmedAttribute(xmlElement, ProvisionAttributes.TransitionProhibitedFor);
          batch.InsertComment("Transition from '{0}' to '{1}'", (object) trimmedAttribute3, (object) trimmedAttribute4);
          MetaID metaId1 = batch.InsertConstant(trimmedAttribute4, (MetaID) null);
          MetaID metaId2 = batch.InsertConstant(trimmedAttribute3, (MetaID) null);
          if (metaId1 == metaId2)
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateStateInTransition", (object) trimmedAttribute3));
          if (trimmedAttribute3.Length > 0 && !this.m_stateConditions.ContainsKey(metaId2))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidState", (object) trimmedAttribute3));
          UpdatePackageCondition condition;
          if (!this.m_stateConditions.TryGetValue(metaId1, out condition))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorInvalidState", (object) trimmedAttribute4));
          flagArray[(int) states[(object) trimmedAttribute3], (int) states[(object) trimmedAttribute4]] = true;
          if (trimmedAttribute3.Length == 0)
          {
            if (defaultStateID != null)
              this.m_metadata.ThrowValidationException(InternalsResourceStrings.Get("ErrorManyDefaultStates"));
            defaultStateID = metaId1;
          }
          MetaID userGroup1 = trimmedAttribute5.Length > 0 ? batch.FindUserGroup(this.m_updatePackageData.ProjectGuid, trimmedAttribute5) : (MetaID) null;
          MetaID userGroup2 = trimmedAttribute6.Length > 0 ? batch.FindUserGroup(this.m_updatePackageData.ProjectGuid, trimmedAttribute6) : (MetaID) null;
          UpdatePackageTransition packageTransition = new UpdatePackageTransition(metaId2, metaId1, userGroup1, userGroup2);
          for (int index = 0; index < packageTransitionList.Count; ++index)
          {
            if (packageTransitionList[index].Equals((object) packageTransition))
              this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateTransition", (object) trimmedAttribute3, (object) trimmedAttribute4, (object) trimmedAttribute5, (object) trimmedAttribute6));
          }
          packageTransitionList.Add(packageTransition);
          if (userGroup1 != null)
          {
            batch.InsertComment("Allow transition for {0}", (object) trimmedAttribute5);
            this.RestrictTransition(metaId2, metaId1, userGroup1, true, batch);
          }
          if (userGroup2 != null)
          {
            batch.InsertComment("Prohibit transition for {0}", (object) trimmedAttribute6);
            this.RestrictTransition(metaId2, metaId1, userGroup2, false, batch);
          }
          if (this.m_provisionActions)
          {
            XmlElement actionsElement = (XmlElement) xmlElement.SelectSingleNode(ProvisionTags.Actions);
            if (actionsElement != null)
            {
              if (trimmedAttribute3.Length == 0)
                this.m_metadata.ThrowValidationException(InternalsResourceStrings.Get("ErrorActionsForInitialTransition"));
              this.ProcessActions(actionsElement, trimmedAttribute3, metaId2, metaId1, batch);
            }
          }
          condition.MakeTransparent();
          context.Push(condition);
          context.Push(new UpdatePackageCondition(fieldId, metaId2, metaId1));
          try
          {
            this.m_fieldDefinitions.TransitionAddReferencesToUpdatePackage(context, metaId2, metaId1, xmlElement, batch);
            this.ProcessReasons(metaId2, metaId1, xmlElement, context, batch);
          }
          finally
          {
            context.Pop();
            context.Pop();
          }
        }
        states.Remove((object) "");
        foreach (string key1 in (IEnumerable) states.Keys)
        {
          int state1 = (int) states[(object) key1];
          MetaID fromId = batch.InsertConstant(key1, (MetaID) null);
          foreach (string key2 in (IEnumerable) states.Keys)
          {
            int state2 = (int) states[(object) key2];
            if (!flagArray[state1, state2])
            {
              MetaID toId = batch.InsertConstant(key2, (MetaID) null);
              batch.InsertComment("Prohibit: '{0}' -> '{1}'", (object) key1, (object) key2);
              this.RestrictTransition(fromId, toId, this.m_updatePackageData.MetaIDFactory.NewMetaID(-1), false, batch);
            }
          }
        }
      }
    }

    private void ProcessDefaultState(
      MetaID defaultStateID,
      UpdatePackageRuleContext context,
      UpdatePackage batch)
    {
      UpdatePackageRuleProperties props1 = new UpdatePackageRuleProperties();
      string key = defaultStateID.IsTemporary ? "TempThenConstID" : "ThenConstID";
      props1[(object) "Default"] = (object) true;
      props1[(object) "ThenFldID"] = (object) 2;
      props1[(object) key] = (object) defaultStateID.Value;
      props1[(object) "IfFldID"] = (object) 2;
      props1[(object) "IfConstID"] = (object) -10000;
      context.SetProperties(props1);
      batch.InsertComment("Rule: default (initial state)");
      batch.InsertRule(this.ProjectId, props1);
      UpdatePackageRuleProperties props2 = new UpdatePackageRuleProperties();
      props2[(object) "DenyWrite"] = (object) true;
      props2[(object) "Unless"] = (object) true;
      props2[(object) "ThenFldID"] = (object) 2;
      props2[(object) key] = (object) defaultStateID.Value;
      props2[(object) "IfFldID"] = (object) 2;
      props2[(object) "IfConstID"] = (object) -10006;
      context.SetProperties(props2);
      batch.InsertComment("Rule: allowed value (State)");
      batch.InsertRule(this.ProjectId, props2);
    }

    private void ProcessActions(
      XmlElement actionsElement,
      string transitionFrom,
      MetaID fromID,
      MetaID toID,
      UpdatePackage batch)
    {
      Dictionary<string, string> dictionary;
      if (this.m_actionsMap.ContainsKey(fromID))
      {
        dictionary = this.m_actionsMap[fromID];
      }
      else
      {
        dictionary = new Dictionary<string, string>((IEqualityComparer<string>) this.m_metadata.ServerStringComparer);
        this.m_actionsMap[fromID] = dictionary;
      }
      for (XmlNode xmlNode = actionsElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
      {
        if (xmlNode is XmlElement element)
        {
          string trimmedAttribute = WITImporter.GetTrimmedAttribute(element, ProvisionAttributes.ListItemValue);
          if (dictionary.ContainsKey(trimmedAttribute))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateActions", (object) trimmedAttribute, (object) transitionFrom));
          dictionary[trimmedAttribute] = trimmedAttribute;
          batch.InsertAction(this.m_updatePackageData.CurrentTypeId, fromID, toID, trimmedAttribute);
        }
      }
    }

    private void ProcessReasons(
      MetaID fromStateId,
      MetaID toStateId,
      XmlElement transitionElement,
      UpdatePackageRuleContext context,
      UpdatePackage batch)
    {
      MetaID fieldId = this.m_updatePackageData.MetaIDFactory.NewMetaID(22);
      MetaID listID = batch.InsertConstant(Guid.NewGuid().ToString(), (MetaID) null);
      MetaID metaId = (MetaID) null;
      XmlNode xmlNode = transitionElement.SelectSingleNode(ProvisionTags.Reasons);
      Dictionary<string, bool> dictionary = new Dictionary<string, bool>(xmlNode.ChildNodes.Count, (IEqualityComparer<string>) this.m_metadata.ServerStringComparer);
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        if (xmlNode.ChildNodes[i] is XmlElement childNode)
        {
          string trimmedAttribute = WITImporter.GetTrimmedAttribute(childNode, ProvisionAttributes.ListItemValue);
          if (dictionary.ContainsKey(trimmedAttribute))
            this.m_metadata.ThrowValidationException(InternalsResourceStrings.Format("ErrorDuplicateReason", (object) WITImporter.GetTrimmedAttribute(transitionElement, ProvisionAttributes.OriginalState), (object) WITImporter.GetTrimmedAttribute(transitionElement, ProvisionAttributes.NewState), (object) trimmedAttribute));
          MetaID valueId = batch.InsertConstant(trimmedAttribute, listID);
          context.Push(new UpdatePackageCondition(this.m_updatePackageData, fieldId, valueId));
          this.m_fieldDefinitions.ReasonAddReferencesToUpdatePackage(context, fromStateId, toStateId, childNode, batch);
          context.Pop();
          if (string.CompareOrdinal(childNode.Name, ProvisionTags.DefaultReason) == 0)
            metaId = valueId;
          dictionary[trimmedAttribute] = true;
        }
      }
      UpdatePackageRuleProperties props1 = new UpdatePackageRuleProperties();
      props1[(object) "ThenFldID"] = (object) 22;
      context.SetProperties(props1);
      UpdatePackage.SetListRuleProperties(ListRuleType.RequiredList, listID, (Hashtable) props1);
      batch.InsertComment("Rule: basic list for reason");
      batch.InsertRule(this.ProjectId, props1);
      string key = metaId.IsTemporary ? "TempThenConstID" : "ThenConstID";
      UpdatePackageRuleProperties props2 = new UpdatePackageRuleProperties();
      context.SetProperties(props2);
      props2[(object) "ThenFldID"] = (object) 22;
      props2[(object) key] = (object) metaId.Value;
      props2[(object) "Default"] = (object) true;
      batch.InsertComment("Rule: default reason");
      batch.InsertRule(this.ProjectId, props2);
    }

    private void RestrictTransition(
      MetaID fromId,
      MetaID toId,
      MetaID personId,
      bool inverseFlag,
      UpdatePackage batch)
    {
      UpdatePackageRuleProperties props = new UpdatePackageRuleProperties();
      props[(object) "DenyWrite"] = (object) true;
      props[(object) "Fld1ID"] = (object) 25;
      if (this.m_currentTypeNameID.IsTemporary)
        props[(object) "TempFld1IsConstID"] = (object) this.m_currentTypeNameID.Value;
      else
        props[(object) "Fld1IsConstID"] = (object) this.m_currentTypeNameID.Value;
      props[(object) "ThenFldID"] = (object) 2;
      props[(object) "Fld2ID"] = (object) 2;
      if (fromId.IsTemporary)
        props[(object) "TempFld2WasConstID"] = (object) fromId.Value;
      else
        props[(object) "Fld2WasConstID"] = (object) fromId.Value;
      if (toId.IsTemporary)
        props[(object) "TempThenConstID"] = (object) toId.Value;
      else
        props[(object) "ThenConstID"] = (object) toId.Value;
      if (personId.IsTemporary)
        props[(object) "TempPersonID"] = (object) personId.Value;
      else
        props[(object) "PersonID"] = (object) personId.Value;
      props[(object) "InversePersonID"] = (object) inverseFlag;
      batch.InsertRule(this.ProjectId, props);
    }

    private static string GetTrimmedAttribute(XmlElement element, string attribute) => element.GetAttribute(attribute).Trim();
  }
}
