// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IProvisioningService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [DefaultServiceImplementation(typeof (ProvisioningService))]
  public interface IProvisioningService : IVssFrameworkService
  {
    XmlDocument ExportWorkItemType(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string typeName,
      ExportMask exportMask = ExportMask.ExportGlobalLists);

    XmlDocument ExportGlobalWorkflow(
      IVssRequestContext requestContext,
      Guid projectGuid,
      ExportMask exportMask = ExportMask.ExportGlobalLists);

    void ImportWorkItemType(
      IVssRequestContext requestContext,
      int projectId,
      string methodologyName,
      XmlElement typeElement,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = true,
      ProvisioningImportEventsCallback importEventCallback = null,
      bool isPromote = false);

    void ImportWorkItemType(
      IVssRequestContext requestContext,
      int projectId,
      string methodologyName,
      string definition,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = true,
      ProvisioningImportEventsCallback importEventCallback = null,
      bool isPromote = false);

    void ImportWorkItemTypes(
      IVssRequestContext requestContext,
      int projectId,
      string methodologyName,
      IEnumerable<string> definitions,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = false,
      ProvisioningImportEventsCallback importEventCallback = null,
      bool isPromote = false);

    void DestroyWorkItemType(IVssRequestContext requestContext, string name, int projectId);

    void RenameWorkItemType(
      IVssRequestContext requestContext,
      string name,
      string newName,
      string projectName);

    void RenameWorkItemType(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string name,
      string newName);

    void ImportWorkItemLinkType(
      IVssRequestContext requestContext,
      string definition,
      bool insertsOnly,
      ProvisioningActionType action = ProvisioningActionType.Import);

    void ImportGlobalWorkflow(
      IVssRequestContext requestContext,
      int projectId,
      string definition,
      ProvisioningActionType actionType = ProvisioningActionType.Import,
      bool overwrite = true,
      ProvisioningImportEventsCallback importEventCallback = null);

    void ImportQueries(
      IVssRequestContext requestContext,
      IProcessTemplate template,
      XmlNode queriesNode,
      Uri projectUri,
      ProvisioningActionType action = ProvisioningActionType.Import);

    void InsertWorkItemTypeUsageWithRules(
      IVssRequestContext requestContext,
      Guid projectGuid,
      int projectid,
      string workItemTypeRefName,
      IDictionary<string, XElement> fieldRefNameToXMLMap);

    void RenameField(IVssRequestContext requestContext, string referenceName, string newName);

    void RenameFields(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<string, string>> fields);

    void RenameField(IVssRequestContext requestContext, int fieldId, string newName);

    void RenameFields(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<int, string>> fields);

    void DeleteField(IVssRequestContext requestContext, string referenceName);

    void DeleteFields(IVssRequestContext requestContext, IEnumerable<string> fields);

    void DeleteField(IVssRequestContext requestContext, int fieldId);

    void DeleteFields(IVssRequestContext requestContext, IEnumerable<int> fields);

    void ImportCategories(
      IVssRequestContext requestContext,
      int projectId,
      string definition,
      bool overwrite = true,
      ProvisioningActionType action = ProvisioningActionType.Import);

    void ImportCategories(
      IVssRequestContext requestContext,
      int projectId,
      XmlDocument doc,
      bool overwrite = true);
  }
}
