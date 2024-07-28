// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ServerMetadataProvisioningHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.Linking;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormExtensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ServerMetadataProvisioningHelper : IMetadataProvisioningHelper
  {
    private IVssRequestContext m_requestContext;
    private WorkItemTrackingFieldService m_fieldTypes;
    private WorkItemTrackingTreeService m_treeService;
    private LegacyWorkItemTypeDictionary m_workItemTypeDictionary;
    private WorkItemTrackingLinkService m_workItemLinkTypeDictionary;
    private IWorkItemTrackingConfigurationInfo m_workItemConfigurationInfo;
    private FieldEntry m_cachedField;
    private List<string> m_registeredLinkTypes;
    private ProvisioningImportEventsCallback m_provisioningImportEventsCallback;
    private HashSet<string> m_identitiesNotFound;
    private Dictionary<int, ServerMetadataProvisioningHelper.IdentityData> m_identityConstIdMapping;
    private Dictionary<string, ServerMetadataProvisioningHelper.IdentityData> m_identityNameMapping;
    private ConstantsSearchSession m_searchSession;

    internal ServerMetadataProvisioningHelper(IVssRequestContext requestContext)
      : this(requestContext, (ProvisioningImportEventsCallback) null)
    {
    }

    internal ServerMetadataProvisioningHelper(
      IVssRequestContext requestContext,
      ProvisioningImportEventsCallback provisioningImportEventsCallback)
    {
      this.m_requestContext = requestContext;
      this.m_fieldTypes = this.m_requestContext.GetService<WorkItemTrackingFieldService>();
      this.m_workItemTypeDictionary = this.m_requestContext.GetService<LegacyWorkItemTypeDictionary>();
      this.m_treeService = this.m_requestContext.GetService<WorkItemTrackingTreeService>();
      this.m_workItemLinkTypeDictionary = this.m_requestContext.GetService<WorkItemTrackingLinkService>();
      this.m_workItemConfigurationInfo = this.m_requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(this.m_requestContext);
      this.m_identitiesNotFound = new HashSet<string>((IEqualityComparer<string>) this.ServerStringComparer);
      this.m_identityNameMapping = new Dictionary<string, ServerMetadataProvisioningHelper.IdentityData>((IEqualityComparer<string>) this.ServerStringComparer);
      this.m_identityConstIdMapping = new Dictionary<int, ServerMetadataProvisioningHelper.IdentityData>();
      this.m_provisioningImportEventsCallback = provisioningImportEventsCallback;
      this.m_searchSession = new ConstantsSearchSession(requestContext);
    }

    public bool IsSupported(string feature) => true;

    public StringComparer ServerStringComparer => this.m_workItemConfigurationInfo.ServerStringComparer;

    public CultureInfo ServerCulture => this.m_workItemConfigurationInfo.ServerCulture;

    public string InstanceGuid => AccountHelper.GetInstanceId(this.m_requestContext);

    public List<int> GetFields()
    {
      List<int> fields = new List<int>();
      foreach (FieldEntry allField in this.m_fieldTypes.GetAllFields(this.m_requestContext))
        fields.Add(allField.FieldId);
      return fields;
    }

    public string GetFieldName(int fieldId) => this.GetFieldById(fieldId).Name;

    public string GetFieldReferenceName(int fieldId) => this.GetFieldById(fieldId).ReferenceName;

    public PsFieldDefinitionTypeEnum GetPsFieldType(int fieldId) => this.GetFieldById(fieldId).PsFieldType;

    public int GetPsReportingType(int fieldId) => this.GetFieldById(fieldId).ReportingType;

    public int GetPsReportingFormula(int fieldId) => this.GetFieldById(fieldId).ReportingFormula;

    public bool IsReportable(int fieldId) => this.GetFieldById(fieldId).IsReportable;

    public string GetReportingName(int fieldId) => this.GetFieldById(fieldId).ReportingName;

    public string GetReportingReferenceName(int fieldId) => this.GetFieldById(fieldId).ReportingReferenceName;

    public bool IsIgnored(int fieldId) => this.GetFieldById(fieldId).IsIgnored;

    public bool IsComputed(int fieldId) => this.GetFieldById(fieldId).IsComputed;

    public void RaiseImportEvent(Exception exception, string message)
    {
      if (this.m_provisioningImportEventsCallback == null)
        return;
      this.m_provisioningImportEventsCallback.RaiseEvent(message);
    }

    public void ThrowValidationException(string message) => throw new LegacyValidationException(message);

    public string GetNodeGuid(int nodeId) => this.m_treeService.LegacyGetTreeNode(this.m_requestContext, nodeId).CssNodeId.ToString();

    public string GetWorkItemLinkTypeForwardEndName(string workItemLinkTypeReferenceName) => this.m_workItemLinkTypeDictionary.GetLinkTypeByReferenceName(this.m_requestContext, workItemLinkTypeReferenceName).ForwardEndName;

    public string GetWorkItemLinkTypeReverseEndName(string workItemLinkTypeReferenceName) => this.m_workItemLinkTypeDictionary.GetLinkTypeByReferenceName(this.m_requestContext, workItemLinkTypeReferenceName).ReverseEndName;

    public int GetTopologyForWorkItemLinkTypeRefName(string workItemLinkTypeReferenceName) => (int) this.m_workItemLinkTypeDictionary.GetLinkTypeByReferenceName(this.m_requestContext, workItemLinkTypeReferenceName).Topology;

    public bool EditAllowForWorkItemLinkType(string workItemLinkTypeReferenceName) => this.m_workItemLinkTypeDictionary.GetLinkTypeByReferenceName(this.m_requestContext, workItemLinkTypeReferenceName).CanEdit;

    public bool ExistsWorkItemLinkRefName(string workItemLinkTypeReferenceName) => this.m_workItemLinkTypeDictionary.ContainsLinkTypeReferenceName(this.m_requestContext, workItemLinkTypeReferenceName);

    public string GetWorkItemReferenceNameByFriendlyName(string friendlyName)
    {
      MDWorkItemLinkType linkType;
      return !this.m_workItemLinkTypeDictionary.TryGetLinkTypeByName(this.m_requestContext, friendlyName, out linkType) ? (string) null : linkType.ReferenceName;
    }

    public List<string> GetWorkItemLinkTypeReferenceNames() => new List<string>(this.m_workItemLinkTypeDictionary.GetLinkTypeReferenceNames(this.m_requestContext));

    public List<string> GetRegisteredLinkTypes()
    {
      if (this.m_registeredLinkTypes == null)
      {
        this.m_registeredLinkTypes = new List<string>();
        foreach (RegistrationArtifactType artifactLinkType in (IEnumerable<RegistrationArtifactType>) this.m_requestContext.GetService<IArtifactLinkTypesService>().GetArtifactLinkTypes(this.m_requestContext, "WorkItemTracking"))
        {
          if (VssStringComparer.ArtifactType.Compare(artifactLinkType.Name, "WorkItem") == 0)
          {
            foreach (OutboundLinkType outboundLinkType in artifactLinkType.OutboundLinkTypes)
              this.m_registeredLinkTypes.Add(outboundLinkType.Name);
          }
        }
      }
      return this.m_registeredLinkTypes;
    }

    public IEnumerable<int> GetCoreFieldIds() => CoreField.All;

    public bool HasWorkItemType => this.m_workItemTypeDictionary.HasWorkItemType(this.m_requestContext);

    public int WorkItemLinkTypesCount => this.m_workItemLinkTypeDictionary.GetCount(this.m_requestContext);

    public bool IsLinkTypeDirectional(string linkTypeReferenceName)
    {
      if (string.IsNullOrEmpty(linkTypeReferenceName))
        throw new ArgumentNullException(nameof (linkTypeReferenceName));
      MDWorkItemLinkType linkType;
      return this.m_workItemLinkTypeDictionary.TryGetLinkTypeByReferenceName(this.m_requestContext, linkTypeReferenceName, out linkType) && linkType.IsDirectional;
    }

    public bool ContainsField(string fieldName) => this.m_fieldTypes.TryGetField(this.m_requestContext, fieldName, out FieldEntry _);

    public bool UseStrictFieldNameCheck => this.m_requestContext.ExecutionEnvironment.IsHostedDeployment || WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(this.m_requestContext);

    public bool IgnoreReportabilityChange => this.m_requestContext.ExecutionEnvironment.IsHostedDeployment || WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(this.m_requestContext);

    public IDictionary<int, IEnumerable<ListItem>> ExpandSetsOneLevel(IEnumerable<int> setIds)
    {
      IDictionary<ConstantSetReference, SetRecord[]> rawResult;
      using (WorkItemTrackingMetadataComponent component = this.m_requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        rawResult = component.GetConstantSets((IEnumerable<ConstantSetReference>) setIds.Select<int, ConstantSetReference>((Func<int, ConstantSetReference>) (id => new ConstantSetReference(id))).ToList<ConstantSetReference>());
      SetRecord[] source;
      return (IDictionary<int, IEnumerable<ListItem>>) setIds.ToDictionary<int, int, IEnumerable<ListItem>>((Func<int, int>) (id => id), (Func<int, IEnumerable<ListItem>>) (id => rawResult.TryGetValue(new ConstantSetReference(id), out source) ? (IEnumerable<ListItem>) ((IEnumerable<SetRecord>) source).Select<SetRecord, ListItem>((Func<SetRecord, ListItem>) (v => new ListItem()
      {
        ConstId = v.ItemId,
        DisplayName = v.Item
      })).ToList<ListItem>() : Enumerable.Empty<ListItem>()));
    }

    public bool FindConstByFullName(string name, bool isIdentity, out int id)
    {
      id = 0;
      if (string.IsNullOrEmpty(name))
        return false;
      if (isIdentity)
      {
        ServerMetadataProvisioningHelper.IdentityData identityData = this.GetIdentityData(name);
        if (identityData != null)
        {
          id = identityData.ConstId;
          return true;
        }
      }
      else
      {
        DalGetNonIdentityConstElement element;
        using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
        {
          element = DalSqlElement.GetElement<DalGetNonIdentityConstElement>(sqlBatch);
          element.JoinBatch(name);
          sqlBatch.ExecuteBatch();
        }
        if (element.TryGetNonIdentityConstant(out id))
          return true;
      }
      return false;
    }

    public bool IsValidGroup(int id)
    {
      ServerMetadataProvisioningHelper.IdentityData identityData;
      return this.m_identityConstIdMapping.TryGetValue(id, out identityData) && identityData.IsValidGroup;
    }

    public bool IsValidUserOrGroup(int id)
    {
      ServerMetadataProvisioningHelper.IdentityData identityData;
      if (!this.m_identityConstIdMapping.TryGetValue(id, out identityData))
        return false;
      return identityData.IsValidGroup || identityData.IsValidUser;
    }

    internal IVssRequestContext RequestContext => this.m_requestContext;

    private ServerMetadataProvisioningHelper.IdentityData GetIdentityData(string name)
    {
      if (!this.m_identitiesNotFound.Contains(name))
      {
        ServerMetadataProvisioningHelper.IdentityData identityData1;
        if (this.m_identityNameMapping.TryGetValue(name, out identityData1))
          return identityData1;
        DalGetIdentityElement element;
        using (SqlBatchBuilder sqlBatch = (SqlBatchBuilder) new WorkItemBatchBuilder(this.m_requestContext))
        {
          element = DalSqlElement.GetElement<DalGetIdentityElement>(sqlBatch);
          element.JoinBatch(name);
          sqlBatch.ExecuteBatch();
        }
        int id;
        bool isValidUser;
        bool isValidGroup;
        if (element.TryGetIdentity(out id, out isValidUser, out isValidGroup))
        {
          ServerMetadataProvisioningHelper.IdentityData identityData2 = new ServerMetadataProvisioningHelper.IdentityData(id, isValidUser, isValidGroup);
          this.m_identityNameMapping[name] = identityData2;
          this.m_identityConstIdMapping[id] = identityData2;
          return identityData2;
        }
        this.m_identitiesNotFound.Add(name);
      }
      return (ServerMetadataProvisioningHelper.IdentityData) null;
    }

    private FieldEntry GetFieldById(int id)
    {
      if (this.m_cachedField == null || id != this.m_cachedField.FieldId)
        this.m_cachedField = this.m_fieldTypes.GetFieldById(this.m_requestContext, id);
      return this.m_cachedField;
    }

    public bool IsValidIdentityNameFormat(string name) => name.Contains("\\");

    public void ValidateRequiredFieldsOnLayout(
      XmlElement fieldsElement,
      XmlElement workflowElement,
      XmlElement formElement,
      Action<string> requiredFieldNotInBothLayoutsErrorAction)
    {
      WebLayoutXmlHelper.ValidateRequiredFieldsOnLayout(fieldsElement, workflowElement, formElement, requiredFieldNotInBothLayoutsErrorAction);
    }

    public void ValidateWebLayoutControls(
      XmlElement webLayoutElement,
      Action<string> systemControlNotAllowedErrorAction,
      Action controlHeightNotAllowedErrorAction,
      Action<string> controlNotRecognizedWarningAction)
    {
      WebLayoutXmlHelper.ValidateWebLayoutControls(webLayoutElement, systemControlNotAllowedErrorAction, controlHeightNotAllowedErrorAction, controlNotRecognizedWarningAction);
    }

    public void ValidateWebLayoutSystemControls(
      XmlElement webLayoutElement,
      ValidateWebLayoutSystemControlErrorActions errorActions)
    {
      WebLayoutXmlHelper.ValidateWebLayoutSystemControls(webLayoutElement, errorActions);
    }

    public void GeneratePageAndGroupIds(
      string processName,
      string workItemTypeName,
      XmlElement webLayoutElement,
      Action<string> invalidOrDuplicatedPageLabelErrorAction,
      Action<string> invalidOrDuplicatedGroupLabelErrorAction,
      Action<string> invalidControlsInGroupErrorAction,
      Action<string, string> duplicateControlsInGroupErrorAction)
    {
      WebLayoutXmlHelper.GeneratePageAndGroupIds(processName, workItemTypeName, (string) null, webLayoutElement, invalidOrDuplicatedPageLabelErrorAction, invalidOrDuplicatedGroupLabelErrorAction, invalidControlsInGroupErrorAction, duplicateControlsInGroupErrorAction);
    }

    public void ValidateWebLayoutExtensions(
      XmlElement webLayoutElement,
      Func<string, string[], bool> doesFieldExist)
    {
      IEnumerable<InstalledExtension> formExtensions = FormExtensionsUtility.GetFormExtensions(this.m_requestContext);
      WebLayoutXmlHelper.ValidateWebLayoutExtensions(webLayoutElement, formExtensions, doesFieldExist);
    }

    public void ValidateLinksControls(XmlElement webLayoutElement) => WebLayoutXmlHelper.ValidateLinksControls((IMetadataProvisioningHelper) this, webLayoutElement);

    private class IdentityData
    {
      public IdentityData(int constId, bool isValidUser, bool isValidGroup)
      {
        this.ConstId = constId;
        this.IsValidUser = isValidUser;
        this.IsValidGroup = isValidGroup;
      }

      public int ConstId { get; private set; }

      public bool IsValidUser { get; private set; }

      public bool IsValidGroup { get; private set; }
    }
  }
}
