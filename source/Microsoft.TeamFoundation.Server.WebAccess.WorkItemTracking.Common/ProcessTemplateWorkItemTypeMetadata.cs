// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessTemplateWorkItemTypeMetadata
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.DataStore;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class ProcessTemplateWorkItemTypeMetadata : WorkItemTypeMetadata
  {
    private XmlElement m_workItemTypeElement;
    private string m_methodologyName;

    public ProcessTemplateWorkItemTypeMetadata(XmlElement typeElement, string methodologyName)
    {
      ArgumentUtility.CheckForNull<XmlElement>(typeElement, nameof (typeElement));
      this.m_workItemTypeElement = typeElement;
      typeElement = (XmlElement) typeElement.SelectSingleNode(ProvisionTags.WorkItemType);
      string attribute = typeElement.GetAttribute(ProvisionAttributes.WorkItemTypeName);
      XmlNode displayForm = typeElement.SelectSingleNode(ProvisionTags.Form);
      this.m_methodologyName = methodologyName;
      IEnumerable<WorkItemField> fields;
      this.ProcessFieldsFromXml((XmlElement) typeElement.SelectSingleNode(ProvisionTags.FieldDefinitions), out fields);
      IEnumerable<string> states;
      IEnumerable<WorkItemTypeAction> actions;
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition> transitions;
      this.ProcessWorkflowFromXml(attribute, (XmlElement) typeElement.SelectSingleNode(ProvisionTags.Workflow), out states, out actions, out transitions);
      this.Init(attribute, states, fields, actions, displayForm, transitions);
    }

    private void ProcessWorkflowFromXml(
      string workItemType,
      XmlElement workflowElement,
      out IEnumerable<string> states,
      out IEnumerable<WorkItemTypeAction> actions,
      out IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition> transitions)
    {
      XmlNodeList xmlNodeList1 = workflowElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ProvisionTags.States, (object) ProvisionTags.State));
      List<string> stringList = new List<string>(xmlNodeList1.Count);
      foreach (XmlElement xmlElement in xmlNodeList1)
        stringList.Add(xmlElement.GetAttribute(ProvisionAttributes.ListItemValue).Trim());
      states = (IEnumerable<string>) stringList;
      XmlNodeList xmlNodeList2 = workflowElement.SelectNodes(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) ProvisionTags.Transitions, (object) ProvisionTags.Transition));
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition> itemTypeTransitionList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>(xmlNodeList2.Count);
      List<WorkItemTypeAction> workItemTypeActionList = new List<WorkItemTypeAction>();
      foreach (XmlElement xmlElement1 in xmlNodeList2)
      {
        string fromState = xmlElement1.GetAttribute(ProvisionAttributes.OriginalState).Trim();
        string toState = xmlElement1.GetAttribute(ProvisionAttributes.NewState).Trim();
        XmlElement xmlElement2 = (XmlElement) xmlElement1.SelectSingleNode(ProvisionTags.Actions);
        if (xmlElement2 != null)
        {
          for (XmlNode xmlNode = xmlElement2.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
          {
            if (xmlNode is XmlElement xmlElement3)
            {
              string name = xmlElement3.GetAttribute(ProvisionAttributes.ListItemValue).Trim();
              workItemTypeActionList.Add(new WorkItemTypeAction(workItemType, name, fromState, toState));
            }
          }
        }
        itemTypeTransitionList.Add(new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition(workItemType, fromState, toState));
      }
      actions = (IEnumerable<WorkItemTypeAction>) workItemTypeActionList;
      transitions = (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>) itemTypeTransitionList;
    }

    private void ProcessFieldsFromXml(
      XmlElement fieldsElement,
      out IEnumerable<WorkItemField> fields)
    {
      List<WorkItemField> workItemFieldList = new List<WorkItemField>(fieldsElement.ChildNodes.Count);
      foreach (XmlElement childNode in fieldsElement.ChildNodes)
      {
        string attribute1 = childNode.GetAttribute(ProvisionAttributes.FieldReferenceName);
        string attribute2 = childNode.GetAttribute(ProvisionAttributes.FieldName);
        string attribute3 = childNode.GetAttribute(ProvisionAttributes.FieldType);
        string name = attribute2;
        string type = attribute3;
        WorkItemField workItemField = new WorkItemField(attribute1, name, type);
        if (childNode.HasAttribute(ProvisionAttributes.Reportable))
          workItemField.ReportingType = WorkItemField.TranslateReportability(childNode.GetAttribute(ProvisionAttributes.Reportable));
        if (childNode.HasAttribute(ProvisionAttributes.RenameSafe))
          workItemField.IsRenameSafe = XmlConvert.ToBoolean(childNode.GetAttribute(ProvisionAttributes.RenameSafe));
        workItemFieldList.Add(workItemField);
      }
      fields = (IEnumerable<WorkItemField>) workItemFieldList;
    }

    public override void AddAction(WorkItemTypeAction action) => throw new NotSupportedException();

    public override void AddField(string workItemTypeRefName, string fieldRefName, XElement field) => throw new NotSupportedException();

    public override XmlNode DisplayForm
    {
      get => base.DisplayForm;
      set => throw new NotSupportedException();
    }

    public XmlElement WorkItemTypeElement => this.m_workItemTypeElement;

    internal override void Save(IVssRequestContext requestContext, int projectId)
    {
      requestContext.TraceEnter(1004013, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Save));
      requestContext.GetService<IProvisioningService>().ImportWorkItemType(requestContext, projectId, this.m_methodologyName, this.m_workItemTypeElement, overwrite: false);
      requestContext.TraceLeave(1004014, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Save));
    }

    internal override void Validate(
      IVssRequestContext requestContext,
      IProjectProvisioningContext provisioningContext)
    {
      requestContext.TraceEnter(1004015, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Validate));
      try
      {
        base.Validate(requestContext, provisioningContext);
        WorkItemTrackingFieldService service1 = requestContext.GetService<WorkItemTrackingFieldService>();
        WorkItemTrackingLinkService service2 = requestContext.GetService<WorkItemTrackingLinkService>();
        foreach (WorkItemField field1 in this.Fields)
        {
          FieldEntry field2;
          if (service1.TryGetField(requestContext, field1.ReferenceName, out field2))
          {
            if (field1.Type != field2.FieldType)
              provisioningContext.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCannotChangeFieldType, (object) field1.ReferenceName, (object) this.Name), IssueLevel.Error));
            if (field2.IsReportable && field1.IsReportable && field1.ReportingType != field2.ReportingType)
              provisioningContext.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCannotChangeReportingType, (object) field1.ReferenceName, (object) this.Name), IssueLevel.Error));
            if (field2.PsFieldType == PsFieldDefinitionTypeEnum.FieldDefinitionTypeSingleValuedKeyword_Person != field1.IsRenameSafe)
              provisioningContext.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCannotChangeSyncNameChanges, (object) field1.ReferenceName, (object) this.Name), IssueLevel.Error));
          }
          else
          {
            if (service1.TryGetField(requestContext, field1.Name, out FieldEntry _))
              provisioningContext.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCannotHaveSameFriendlyName, (object) field1.ReferenceName, (object) this.Name), IssueLevel.Error));
            if (service2.ContainsLinkTypeReferenceName(requestContext, field1.ReferenceName))
              provisioningContext.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCannotHaveSameLinkReferenceName, (object) field1.ReferenceName, (object) this.Name), IssueLevel.Error));
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(1004016, "ProjectFeatureProvisioning", TfsTraceLayers.Framework, nameof (Validate));
      }
    }
  }
}
