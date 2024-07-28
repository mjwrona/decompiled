// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.StoryboardIntegrationFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class StoryboardIntegrationFeature : ProjectFeatureBase
  {
    private static readonly string TagNameControl = "Control";
    private static readonly string TagNameTabGroup = "TabGroup";
    private static readonly string TagNameTab = "Tab";
    private static readonly string TagAttributeValueStoryboardsControl = "StoryboardsControl";
    private static readonly string TagAttributeName = "Name";

    public StoryboardIntegrationFeature()
      : base(Resources.StoryboardFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      WorkItemTypeCategory itemTypeCategory = projectMetadata.GetWorkItemTypeCategory("Microsoft.RequirementCategory");
      if (itemTypeCategory != null)
      {
        int num = itemTypeCategory.WorkItemTypeNames.Any<string>() ? 1 : 0;
        bool flag = this.IsControlPresentInAnyWorkItemType(projectMetadata, itemTypeCategory, StoryboardIntegrationFeature.TagAttributeValueStoryboardsControl);
        if (num == 0 || flag)
          return ProjectFeatureState.FullyConfigured;
        if (this.IsTabGroupMissingInAnyWorkItemType(projectMetadata, itemTypeCategory))
          return ProjectFeatureState.PartiallyConfigured;
      }
      return ProjectFeatureState.NotConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureCategory(context, "Microsoft.RequirementCategory", ProjectFeatureBase.EnsureCategoryMode.Merge, ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists);
      this.EnsureStoryboardTab(context);
    }

    private bool IsTabGroupMissingInAnyWorkItemType(
      IProjectMetadata projectMetadata,
      WorkItemTypeCategory category)
    {
      ArgumentUtility.CheckForNull<IProjectMetadata>(projectMetadata, nameof (projectMetadata));
      ArgumentUtility.CheckForNull<WorkItemTypeCategory>(category, nameof (category));
      bool flag = false;
      try
      {
        foreach (string workItemTypeName in category.WorkItemTypeNames)
        {
          if (this.FindFirstXmlNodeWithType(XDocument.Load((XmlReader) new XmlNodeReader(projectMetadata.GetWorkItemType(workItemTypeName).DisplayForm)), StoryboardIntegrationFeature.TagNameTabGroup) == null)
          {
            flag = true;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        flag = true;
      }
      return flag;
    }

    private void EnsureStoryboardTab(IProjectProvisioningContext context)
    {
      ArgumentUtility.CheckForNull<IProjectProvisioningContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<IProjectMetadata>(context.ProcessTemplate, "context.ProcessTemplate");
      WorkItemTypeCategory itemTypeCategory = context.ProcessTemplate.GetWorkItemTypeCategory("Microsoft.RequirementCategory");
      WorkItemTypeMetadata workItemType1 = context.ProcessTemplate.GetWorkItemType(itemTypeCategory.DefaultWorkItemTypeName);
      foreach (string workItemTypeName in context.GetWorkItemTypeCategory("Microsoft.RequirementCategory").WorkItemTypeNames)
      {
        try
        {
          WorkItemTypeMetadata workItemType2 = context.GetWorkItemType(workItemTypeName);
          this.AddTabControlToDisplayForm(workItemType1, workItemType2, StoryboardIntegrationFeature.TagAttributeValueStoryboardsControl);
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
          context.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.Warning_FailedToInsertTabForWorkItemType, (object) Resources.StoryboardFeatureName, (object) workItemTypeName, (object) "Microsoft.RequirementCategory"), IssueLevel.Warning));
        }
      }
    }

    private void AddTabControlToDisplayForm(
      WorkItemTypeMetadata templateType,
      WorkItemTypeMetadata contextType,
      string controlName)
    {
      ArgumentUtility.CheckForNull<WorkItemTypeMetadata>(templateType, nameof (templateType));
      ArgumentUtility.CheckForNull<WorkItemTypeMetadata>(contextType, nameof (contextType));
      ArgumentUtility.CheckStringForNullOrEmpty(controlName, nameof (controlName));
      XDocument xdocument = XDocument.Load((XmlReader) new XmlNodeReader(contextType.DisplayForm));
      if (this.GetControlNodeInDisplayForm(xdocument, controlName) != null)
        return;
      this.FindFirstXmlNodeWithType(xdocument, StoryboardIntegrationFeature.TagNameTabGroup).Add((object) this.GetControlNodeInDisplayForm(templateType, controlName).Parent);
      contextType.DisplayForm = (XmlNode) StoryboardIntegrationFeature.GetXmlDocument(xdocument);
    }

    private static XElement FindXmlNodeWithControl(XDocument docDisplayForm, string controlName)
    {
      ArgumentUtility.CheckForNull<XDocument>(docDisplayForm, nameof (docDisplayForm));
      ArgumentUtility.CheckStringForNullOrEmpty(controlName, nameof (controlName));
      return StoryboardIntegrationFeature.FindXmlNodeWithAttribute(docDisplayForm, StoryboardIntegrationFeature.TagNameControl, StoryboardIntegrationFeature.TagAttributeName, controlName);
    }

    private static XmlDocument GetXmlDocument(XDocument xDocument)
    {
      ArgumentUtility.CheckForNull<XDocument>(xDocument, nameof (xDocument));
      using (XmlReader reader = xDocument.CreateReader())
      {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.Load(reader);
        return xmlDocument;
      }
    }

    private XElement FindFirstXmlNodeWithType(XDocument xmlDocument, string tagType)
    {
      ArgumentUtility.CheckForNull<XDocument>(xmlDocument, nameof (xmlDocument));
      ArgumentUtility.CheckStringForNullOrEmpty(tagType, nameof (tagType));
      XElement firstXmlNodeWithType = (XElement) null;
      try
      {
        firstXmlNodeWithType = xmlDocument.Descendants((XName) tagType).FirstOrDefault<XElement>();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.Info("Xml node {0} not present in document {1}, Failed with exception {2}", (object) tagType, (object) xmlDocument.ToString(), (object) ex.StackTrace);
      }
      return firstXmlNodeWithType;
    }

    private static XElement FindXmlNodeWithAttribute(
      XDocument xmlDocument,
      string tagType,
      string attributeName,
      string attributeValue)
    {
      ArgumentUtility.CheckForNull<XDocument>(xmlDocument, nameof (xmlDocument));
      ArgumentUtility.CheckStringForNullOrEmpty(tagType, nameof (tagType));
      ArgumentUtility.CheckStringForNullOrEmpty(attributeName, nameof (attributeName));
      ArgumentUtility.CheckStringForNullOrEmpty(attributeValue, nameof (attributeValue));
      XElement nodeWithAttribute = (XElement) null;
      try
      {
        foreach (XElement descendant in xmlDocument.Descendants((XName) tagType))
        {
          XAttribute xattribute = descendant.Attribute((XName) attributeName);
          if (xattribute != null && VssStringComparer.XmlAttributeValue.Equals(xattribute.Value, attributeValue))
          {
            nodeWithAttribute = descendant;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.Info("Xml node {0} not present in document {1} with attribute name {2} and attribute value {3}, Failed with exception {4}", (object) tagType, (object) xmlDocument.ToString(), (object) attributeName, (object) attributeValue, (object) ex.StackTrace);
      }
      return nodeWithAttribute;
    }

    private bool IsControlPresentInAnyWorkItemType(
      IProjectMetadata projectMetadata,
      WorkItemTypeCategory category,
      string controlName)
    {
      ArgumentUtility.CheckForNull<IProjectMetadata>(projectMetadata, nameof (projectMetadata));
      ArgumentUtility.CheckForNull<WorkItemTypeCategory>(category, nameof (category));
      try
      {
        foreach (string workItemTypeName in category.WorkItemTypeNames)
        {
          WorkItemTypeMetadata workItemType = projectMetadata.GetWorkItemType(workItemTypeName);
          ArgumentUtility.CheckForNull<WorkItemTypeMetadata>(workItemType, "type");
          if (this.GetControlNodeInDisplayForm(workItemType, controlName) != null)
            return true;
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      return false;
    }

    private XElement GetControlNodeInDisplayForm(WorkItemTypeMetadata type, string controlName)
    {
      ArgumentUtility.CheckForNull<WorkItemTypeMetadata>(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(controlName, nameof (controlName));
      return this.GetControlNodeInDisplayForm(XDocument.Load((XmlReader) new XmlNodeReader(type.DisplayForm)), controlName);
    }

    private XElement GetControlNodeInDisplayForm(XDocument typeDocument, string controlName)
    {
      ArgumentUtility.CheckForNull<XDocument>(typeDocument, nameof (typeDocument));
      ArgumentUtility.CheckStringForNullOrEmpty(controlName, nameof (controlName));
      return StoryboardIntegrationFeature.FindXmlNodeWithControl(typeDocument, controlName);
    }
  }
}
