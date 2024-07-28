// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsOnTaskBoardFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class BugsOnTaskBoardFeature : ProjectFeatureBase
  {
    private static readonly string fieldElementName = "FIELD";
    private static readonly string controlElementName = "Control";
    private static readonly string refnameAttributeName = "refname";
    private static readonly string fieldNameAttributeName = "FieldName";

    public BugsOnTaskBoardFeature()
      : base(Resources.BugsOnTaskBoardFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata)
    {
      ProjectProcessConfiguration processConfiguration = projectMetadata.GetProcessConfiguration();
      if (processConfiguration == null || !this.IsBugCategoryPresentInProject(projectMetadata, out CategoryConfiguration _))
        return ProjectFeatureState.NotConfigured;
      IEnumerable<string> commonWorkItemTypes;
      if (!this.IsCategoriesValid(projectMetadata, out commonWorkItemTypes))
        return ProjectFeatureState.PartiallyConfigured;
      return commonWorkItemTypes.Any<string>() || processConfiguration.ActivityField != null && processConfiguration.RemainingWorkField != null && (!this.FieldExistsOnCategoryWorkItems(projectMetadata, processConfiguration.RemainingWorkField.Name, processConfiguration.BugWorkItems.CategoryReferenceName) || !this.FieldExistsOnCategoryWorkItems(projectMetadata, processConfiguration.ActivityField.Name, processConfiguration.BugWorkItems.CategoryReferenceName)) ? ProjectFeatureState.NotConfigured : ProjectFeatureState.FullyConfigured;
    }

    public override void Process(IProjectProvisioningContext context)
    {
      this.EnsureProcessConfigurationSettings(context);
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      if (processConfiguration.BugWorkItems == null)
        throw new LegacyValidationException(Resources.ProcessSettingsInvalid);
      string categoryReferenceName = processConfiguration.BugWorkItems.CategoryReferenceName;
      this.EnsureCategory(context, categoryReferenceName, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists);
      IEnumerable<string> source1 = this.RemoveOverlappingWorkItemTypesInCategory(context, categoryReferenceName, processConfiguration.RequirementBacklog.CategoryReferenceName);
      IEnumerable<string> source2 = this.RemoveOverlappingWorkItemTypesInCategory(context, categoryReferenceName, processConfiguration.TaskBacklog.CategoryReferenceName);
      if (source1.Any<string>() && source2.Any<string>())
        context.ReportIssue(new ProjectProvisioningIssue(Resources.BugCategoryOverlapWithMultipleCategories_Error, IssueLevel.Error));
      else if (source1.Any<string>())
        this.EnsureBugsBehaviorProperty(context, BugsBehavior.AsRequirements);
      else if (source2.Any<string>())
        this.EnsureBugsBehaviorProperty(context, BugsBehavior.AsTasks);
      else
        this.EnsureBugsBehaviorProperty(context, BugsBehavior.Off);
      this.EnsureRemainingWorkAndActivityFieldsOnBugs(context);
    }

    private bool IsBugCategoryPresentInProject(
      IProjectMetadata projectMetaData,
      out CategoryConfiguration bugWorkItems)
    {
      ProjectProcessConfiguration processConfiguration = projectMetaData.GetProcessConfiguration();
      if (processConfiguration != null && processConfiguration.BugWorkItems != null)
      {
        bugWorkItems = processConfiguration.BugWorkItems;
        return true;
      }
      bugWorkItems = (CategoryConfiguration) null;
      return false;
    }

    private bool IsCategoriesValid(
      IProjectMetadata projectMetadata,
      out IEnumerable<string> commonWorkItemTypes)
    {
      ProjectProcessConfiguration processConfiguration = projectMetadata.GetProcessConfiguration();
      if (processConfiguration == null || processConfiguration.BugWorkItems == null)
      {
        commonWorkItemTypes = (IEnumerable<string>) new string[0];
        return false;
      }
      if (!this.IsCategorySubsetOrNotOverlapping(projectMetadata, processConfiguration.BugWorkItems.CategoryReferenceName, processConfiguration.RequirementBacklog.CategoryReferenceName, out commonWorkItemTypes))
        return false;
      IEnumerable<string> commonWorkItemTypes1;
      if (!this.IsCategorySubsetOrNotOverlapping(projectMetadata, processConfiguration.BugWorkItems.CategoryReferenceName, processConfiguration.TaskBacklog.CategoryReferenceName, out commonWorkItemTypes1))
      {
        commonWorkItemTypes = commonWorkItemTypes.Concat<string>(commonWorkItemTypes1);
        return false;
      }
      if (commonWorkItemTypes.Any<string>() && commonWorkItemTypes1.Any<string>())
      {
        commonWorkItemTypes = commonWorkItemTypes.Concat<string>(commonWorkItemTypes1);
        return false;
      }
      commonWorkItemTypes = commonWorkItemTypes.Concat<string>(commonWorkItemTypes1);
      return !this.IsCategoryOverlappingWithPortfolios(projectMetadata, processConfiguration.BugWorkItems.CategoryReferenceName);
    }

    private void EnsureBugsBehaviorProperty(
      IProjectProvisioningContext context,
      BugsBehavior overlapBehavior)
    {
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      Property property = new Property()
      {
        Name = "BugsBehavior",
        Value = "Off"
      };
      switch (overlapBehavior)
      {
        case BugsBehavior.AsRequirements:
        case BugsBehavior.AsTasks:
          property.Value = overlapBehavior.ToString();
          break;
      }
      if (processConfiguration.Properties == null)
        processConfiguration.Properties = new Property[1]
        {
          property
        };
      else if (!((IEnumerable<Property>) processConfiguration.Properties).Any<Property>((Func<Property, bool>) (p => VssStringComparer.PropertyName.Equals(p.Name, "BugsBehavior"))))
      {
        processConfiguration.Properties = ((IEnumerable<Property>) processConfiguration.Properties).Concat<Property>((IEnumerable<Property>) new Property[1]
        {
          property
        }).ToArray<Property>();
      }
      else
      {
        if (overlapBehavior == BugsBehavior.Off)
          return;
        IEnumerable<Property> first = ((IEnumerable<Property>) processConfiguration.Properties).Where<Property>((Func<Property, bool>) (p => !VssStringComparer.PropertyName.Equals(p.Name, "BugsBehavior")));
        processConfiguration.Properties = first.Concat<Property>((IEnumerable<Property>) new Property[1]
        {
          property
        }).ToArray<Property>();
      }
    }

    private void EnsureRemainingWorkAndActivityFieldsOnBugs(IProjectProvisioningContext context)
    {
      ProjectProcessConfiguration processConfiguration = context.GetProcessConfiguration();
      string categoryReferenceName = processConfiguration.BugWorkItems.CategoryReferenceName;
      IEnumerable<string> workItemTypeNames = context.GetWorkItemTypeCategory(categoryReferenceName).WorkItemTypeNames;
      string workItemTypeName = context.ProcessTemplate.GetWorkItemTypeCategory("Microsoft.BugCategory").DefaultWorkItemTypeName;
      XElement doc1 = (context.ProcessTemplate.GetWorkItemType(workItemTypeName) as ProcessTemplateWorkItemTypeMetadata).WorkItemTypeElement.ToXElement().Descendants((XName) "FIELDS").First<XElement>();
      if (!this.FieldExistsOnCategoryWorkItems((IProjectMetadata) context, processConfiguration.RemainingWorkField.Name, categoryReferenceName))
        this.AddFieldToCategoryWorkItemType(context, categoryReferenceName, processConfiguration.RemainingWorkField.Name, this.GetElement((XContainer) doc1, BugsOnTaskBoardFeature.fieldElementName, BugsOnTaskBoardFeature.refnameAttributeName, processConfiguration.RemainingWorkField.Name));
      if (!this.FieldExistsOnCategoryWorkItems((IProjectMetadata) context, processConfiguration.ActivityField.Name, categoryReferenceName))
        this.AddFieldToCategoryWorkItemType(context, categoryReferenceName, processConfiguration.ActivityField.Name, this.GetElement((XContainer) doc1, BugsOnTaskBoardFeature.fieldElementName, BugsOnTaskBoardFeature.refnameAttributeName, processConfiguration.ActivityField.Name));
      foreach (string name in workItemTypeNames)
      {
        XmlNode displayForm1 = context.GetWorkItemType(name).DisplayForm;
        XmlNode displayForm2 = context.ProcessTemplate.GetWorkItemType(workItemTypeName).DisplayForm;
        XDocument doc2 = XDocument.Load((XmlReader) new XmlNodeReader(displayForm1));
        XDocument doc3 = XDocument.Load((XmlReader) new XmlNodeReader(displayForm2));
        Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
        dictionary[processConfiguration.ActivityField.Name] = this.GetElement((XContainer) doc2, BugsOnTaskBoardFeature.controlElementName, BugsOnTaskBoardFeature.fieldNameAttributeName, processConfiguration.ActivityField.Name) == null;
        dictionary[processConfiguration.RemainingWorkField.Name] = this.GetElement((XContainer) doc2, BugsOnTaskBoardFeature.controlElementName, BugsOnTaskBoardFeature.fieldNameAttributeName, processConfiguration.RemainingWorkField.Name) == null;
        if (dictionary[processConfiguration.ActivityField.Name] || dictionary[processConfiguration.RemainingWorkField.Name])
        {
          XElement element1 = this.GetElement((XContainer) doc2, BugsOnTaskBoardFeature.controlElementName, BugsOnTaskBoardFeature.fieldNameAttributeName, processConfiguration.EffortField.Name);
          XElement element2 = this.GetElement((XContainer) doc3, BugsOnTaskBoardFeature.controlElementName, BugsOnTaskBoardFeature.fieldNameAttributeName, processConfiguration.RemainingWorkField.Name);
          XElement element3 = this.GetElement((XContainer) doc3, BugsOnTaskBoardFeature.controlElementName, BugsOnTaskBoardFeature.fieldNameAttributeName, processConfiguration.ActivityField.Name);
          if (element1 != null)
          {
            if (element3 != null && dictionary[processConfiguration.ActivityField.Name])
            {
              element1.AddAfterSelf((object) element3);
              dictionary[processConfiguration.ActivityField.Name] = false;
            }
            if (element2 != null && dictionary[processConfiguration.RemainingWorkField.Name])
            {
              element1.AddAfterSelf((object) element2);
              dictionary[processConfiguration.RemainingWorkField.Name] = false;
            }
            using (XmlReader reader = doc2.CreateReader())
            {
              XmlDocument xmlDocument = new XmlDocument();
              xmlDocument.Load(reader);
              context.GetWorkItemType(name).DisplayForm = (XmlNode) xmlDocument;
            }
          }
          foreach (string key in dictionary.Keys)
          {
            if (dictionary[key])
              context.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BugsOnTaskBoardFieldMissingWarning, (object) Resources.BugsOnTaskBoardFeatureName, (object) name, (object) key), IssueLevel.Warning));
          }
        }
      }
    }

    private XElement GetElement(XContainer doc, string xname, string attrname, string attrvalue)
    {
      foreach (XElement descendant in doc.Descendants((XName) xname))
      {
        XAttribute xattribute = descendant.Attribute((XName) attrname);
        if (xattribute != null && VssStringComparer.XmlAttributeValue.Equals(xattribute.Value, attrvalue))
          return descendant;
      }
      return (XElement) null;
    }
  }
}
