// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.IMetadataProvisioningHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public interface IMetadataProvisioningHelper
  {
    bool IsSupported(string feature);

    StringComparer ServerStringComparer { get; }

    CultureInfo ServerCulture { get; }

    string InstanceGuid { get; }

    List<int> GetFields();

    string GetFieldName(int fieldId);

    string GetFieldReferenceName(int fieldId);

    PsFieldDefinitionTypeEnum GetPsFieldType(int fieldId);

    int GetPsReportingType(int fieldId);

    int GetPsReportingFormula(int fieldId);

    bool IsReportable(int fieldId);

    string GetReportingName(int fieldId);

    string GetReportingReferenceName(int fieldId);

    bool IsIgnored(int fieldId);

    bool IsComputed(int fieldId);

    void RaiseImportEvent(Exception exception, string message);

    void ThrowValidationException(string message);

    string GetNodeGuid(int nodeId);

    string GetWorkItemLinkTypeForwardEndName(string workItemLinkTypeReferenceName);

    string GetWorkItemLinkTypeReverseEndName(string workItemLinkTypeReferenceName);

    string GetWorkItemReferenceNameByFriendlyName(string friendlyName);

    int GetTopologyForWorkItemLinkTypeRefName(string workItemLinkTypeReferenceName);

    bool EditAllowForWorkItemLinkType(string workItemLinkTypeReferenceName);

    bool ExistsWorkItemLinkRefName(string workItemLinkTypeReferenceName);

    List<string> GetWorkItemLinkTypeReferenceNames();

    List<string> GetRegisteredLinkTypes();

    IEnumerable<int> GetCoreFieldIds();

    bool HasWorkItemType { get; }

    int WorkItemLinkTypesCount { get; }

    bool IsLinkTypeDirectional(string ltr);

    bool ContainsField(string field);

    bool UseStrictFieldNameCheck { get; }

    bool IgnoreReportabilityChange { get; }

    IDictionary<int, IEnumerable<ListItem>> ExpandSetsOneLevel(IEnumerable<int> setIds);

    bool FindConstByFullName(string name, bool isIdentity, out int id);

    bool IsValidGroup(int id);

    bool IsValidUserOrGroup(int id);

    bool IsValidIdentityNameFormat(string name);

    void ValidateRequiredFieldsOnLayout(
      XmlElement fieldsElement,
      XmlElement workflowElement,
      XmlElement formElement,
      Action<string> requiredFieldNotInBothLayoutsErrorAction);

    void ValidateWebLayoutControls(
      XmlElement webLayoutElement,
      Action<string> systemControlNotAllowedErrorAction,
      Action controlHeightNotAllowedErrorAction,
      Action<string> controlNotRecognizedWarningAction);

    void ValidateWebLayoutSystemControls(
      XmlElement webLayoutElement,
      ValidateWebLayoutSystemControlErrorActions errorActions);

    void GeneratePageAndGroupIds(
      string processName,
      string workItemTypeName,
      XmlElement webLayoutElement,
      Action<string> invalidOrDuplicatedPageLabelErrorAction,
      Action<string> invalidOrDuplicatedGroupLabelErrorAction,
      Action<string> invalidControlsInGroupErrorAction,
      Action<string, string> duplicateControlsInGroupErrorAction);

    void ValidateWebLayoutExtensions(
      XmlElement webLayoutElement,
      Func<string, string[], bool> doesFieldExist);

    void ValidateLinksControls(XmlElement webLayoutElement);
  }
}
