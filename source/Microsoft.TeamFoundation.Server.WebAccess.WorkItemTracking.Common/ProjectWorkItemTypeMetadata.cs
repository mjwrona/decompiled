// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectWorkItemTypeMetadata
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class ProjectWorkItemTypeMetadata : WorkItemTypeMetadata
  {
    internal ProjectWorkItemTypeMetadata(
      IVssRequestContext requestContext,
      int projectId,
      string name,
      string form,
      IEnumerable<WorkItemTypeAction> actions,
      IEnumerable<WorkItemTypeState> states,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition> transitions)
    {
      requestContext.TraceEnter(1004026, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ProjectWorkItemTypeMetadata.Ctor");
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      List<WorkItemField> fields = new List<WorkItemField>();
      WorkItemTrackingFieldService service = requestContext.GetService<WorkItemTrackingFieldService>();
      LegacyWorkItemType wit;
      requestContext.GetService<LegacyWorkItemTypeDictionary>().GetWorkItemTypesForProject(requestContext, projectId).TryGetByName(name, out wit);
      foreach (int fieldId in wit.FieldIds)
      {
        FieldEntry fieldById = service.GetFieldById(requestContext, fieldId);
        if (fieldById != null)
          fields.Add(new WorkItemField(fieldById.ReferenceName, fieldById.Name, fieldById.FieldType));
      }
      XmlDocument document = XmlUtility.GetDocument(form);
      this.Init(name, states.Select<WorkItemTypeState, string>((Func<WorkItemTypeState, string>) (item => item.State)), (IEnumerable<WorkItemField>) fields, actions, (XmlNode) document, transitions);
      requestContext.TraceEnter(1004027, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "ProjectWorkItemTypeMetadata.Ctor");
    }

    internal override void Save(IVssRequestContext requestContext, int projectId)
    {
      if (!this.IsDirty)
        return;
      requestContext.Trace(1004028, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, "importing over exiting workitem {0}", (object) this.Name);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = requestContext.GetService<WorkItemTrackingTreeService>().LegacyGetTreeNode(requestContext, projectId);
      string name = treeNode.GetName(requestContext);
      Guid cssNodeId = treeNode.CssNodeId;
      DataAccessLayerImpl dataAccessLayerImpl = new DataAccessLayerImpl(requestContext);
      if (this.AddedActions.Count<WorkItemTypeAction>() > 0)
        dataAccessLayerImpl.AddWorkItemTypeActions(name, this.AddedActions);
      if (this.AddedFieldRefNameToXMLMap.Count > 0)
        requestContext.GetService<IProvisioningService>().InsertWorkItemTypeUsageWithRules(requestContext, cssNodeId, projectId, this.Name, this.AddedFieldRefNameToXMLMap);
      if (!this.DisplayFormIsDirty)
        return;
      dataAccessLayerImpl.SetDisplayForm(name, this.Name, this.DisplayForm);
    }

    internal bool IsDirty => this.AddedActions.Any<WorkItemTypeAction>() || this.DisplayFormIsDirty || this.AddedFieldRefNameToXMLMap.Count > 0;

    internal override void Validate(
      IVssRequestContext requestContext,
      IProjectProvisioningContext provisioningContext)
    {
      base.Validate(requestContext, provisioningContext);
    }
  }
}
