// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectFeatureBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal abstract class ProjectFeatureBase : IProjectFeature
  {
    private static Tuple<string, string>[] s_renamedWorkItemTypeNames = new Tuple<string, string>[1]
    {
      new Tuple<string, string>("使用者劇本", "使用者本文")
    };

    public ProjectFeatureBase(string name) => this.Name = name;

    public string Name { get; private set; }

    public abstract ProjectFeatureState GetState(IProjectMetadata projectMetadata);

    public abstract void Process(IProjectProvisioningContext context);

    public bool IsHidden { get; protected set; }

    protected void EnsureCategory(
      IProjectProvisioningContext context,
      string categoryReferenceName,
      ProjectFeatureBase.EnsureCategoryMode categoryMode,
      ProjectFeatureBase.EnsureWorkItemMode workitemMode)
    {
      WorkItemTypeCategory category = context.ProcessTemplate.GetWorkItemTypeCategory(categoryReferenceName);
      if (category == null)
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCategoryMissing, (object) categoryReferenceName));
      WorkItemTypeCategory itemTypeCategory = context.GetWorkItemTypeCategory(categoryReferenceName);
      if (itemTypeCategory == category)
        return;
      switch (categoryMode)
      {
        case ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite:
          if (itemTypeCategory != null)
            return;
          break;
        case ProjectFeatureBase.EnsureCategoryMode.Merge:
          if (itemTypeCategory != null)
          {
            category = new WorkItemTypeCategory(itemTypeCategory.Name, itemTypeCategory.ReferenceName, itemTypeCategory.WorkItemTypeNames.Union<string>(category.WorkItemTypeNames), itemTypeCategory.DefaultWorkItemTypeName);
            break;
          }
          break;
      }
      foreach (Tuple<string, string> workItemTypeName in ProjectFeatureBase.s_renamedWorkItemTypeNames)
      {
        string str = workItemTypeName.Item1;
        string name = workItemTypeName.Item2;
        if (category.WorkItemTypeNames.Contains<string>(str, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName) && category.WorkItemTypeNames.Contains<string>(name, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName) && context.GetWorkItemType(str) == null && context.GetWorkItemType(name) != null)
        {
          string defaultWorkItemTypeName = TFStringComparer.WorkItemTypeName.Equals(category.DefaultWorkItemTypeName, str) ? name : category.DefaultWorkItemTypeName;
          category = new WorkItemTypeCategory(category.Name, category.ReferenceName, category.WorkItemTypeNames.Except<string>((IEnumerable<string>) new string[1]
          {
            str
          }), defaultWorkItemTypeName);
        }
      }
      List<string> second = new List<string>();
      foreach (string workItemTypeName in category.WorkItemTypeNames)
      {
        WorkItemTypeMetadata workItemType = context.GetWorkItemType(workItemTypeName);
        switch (workitemMode)
        {
          case ProjectFeatureBase.EnsureWorkItemMode.FailIfMissingDoNotOverwriteIfExists:
            if (workItemType == null)
              throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorWorkItemCategoryTypeMissing, (object) categoryReferenceName, (object) workItemTypeName));
            continue;
          case ProjectFeatureBase.EnsureWorkItemMode.RemoveFromCategoryIfMissingDoNotOverwriteIfExists:
            if (workItemType == null)
            {
              second.Add(workItemTypeName);
              continue;
            }
            continue;
          case ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists:
            if (workItemType == null)
            {
              context.AddWorkItemType(context.ProcessTemplate.GetWorkItemType(workItemTypeName));
              continue;
            }
            context.ReportIssue(new ProjectProvisioningIssue(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionWarningWorkItemTypeExists, (object) workItemTypeName, (object) this.Name), IssueLevel.Warning));
            continue;
          default:
            continue;
        }
      }
      if (second.Count > 0)
      {
        IEnumerable<string> strings = category.WorkItemTypeNames.Except<string>((IEnumerable<string>) second);
        if (!strings.Any<string>())
          return;
        string defaultWorkItemTypeName = strings.Contains<string>(category.DefaultWorkItemTypeName, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName) ? category.DefaultWorkItemTypeName : strings.First<string>();
        category = new WorkItemTypeCategory(category.Name, category.ReferenceName, strings, defaultWorkItemTypeName);
      }
      if (itemTypeCategory != null && TFStringComparer.WorkItemTypeName.Equals(itemTypeCategory.DefaultWorkItemTypeName, category.DefaultWorkItemTypeName) && new HashSet<string>(itemTypeCategory.WorkItemTypeNames, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName).SetEquals(category.WorkItemTypeNames))
        return;
      context.AddWorkItemTypeCategory(category);
    }

    protected void EnsureProcessConfigurationSettings(IProjectProvisioningContext context)
    {
      ProjectProcessConfiguration processConfiguration = context.ProcessTemplate.GetProcessConfiguration();
      if (processConfiguration == null)
        throw new LegacyValidationException(Resources.ProcessSettingsMissing);
      if (context.GetProcessConfiguration() != null)
        return;
      context.AddProcessConfiguration(processConfiguration);
    }

    protected void EnsureProcessSettingsCategoryNode(
      IProjectProvisioningContext context,
      Func<ProjectProcessConfiguration, CategoryConfiguration> getCategoryNode,
      Action<ProjectProcessConfiguration, CategoryConfiguration> setNode,
      bool optional)
    {
      ProjectProcessConfiguration processConfiguration1 = context.GetProcessConfiguration();
      if (getCategoryNode(processConfiguration1) != null)
        return;
      ProjectProcessConfiguration processConfiguration2 = context.ProcessTemplate.GetProcessConfiguration();
      CategoryConfiguration categoryConfiguration = getCategoryNode(processConfiguration2);
      if (categoryConfiguration == null)
      {
        if (!optional)
          throw new LegacyValidationException(Resources.ProcessSettingsInvalid);
      }
      else
        setNode(processConfiguration1, categoryConfiguration);
    }

    protected void AddFieldToCategoryWorkItemType(
      IProjectProvisioningContext context,
      string categoryReferenceName,
      string fieldReferenceName,
      XElement field = null)
    {
      WorkItemTypeCategory itemTypeCategory = context.GetWorkItemTypeCategory(categoryReferenceName);
      if (itemTypeCategory == null)
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCategoryMissing, (object) categoryReferenceName));
      foreach (string workItemTypeName in itemTypeCategory.WorkItemTypeNames)
        this.AddFieldToWorkItemType(context, workItemTypeName, fieldReferenceName, field);
    }

    protected void AddFieldToWorkItemType(
      IProjectProvisioningContext context,
      string workItemTypeReferenceName,
      string fieldReferenceName,
      XElement field = null)
    {
      (context.GetWorkItemType(workItemTypeReferenceName) ?? throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorWorkItemTypeMissing, (object) workItemTypeReferenceName))).AddField(workItemTypeReferenceName, fieldReferenceName, field);
    }

    protected bool FieldExistsOnCategoryWorkItems(
      IProjectMetadata context,
      string fieldReferenceName,
      string categoryReferenceName)
    {
      bool flag = true;
      WorkItemTypeCategory itemTypeCategory = context.GetWorkItemTypeCategory(categoryReferenceName);
      if (itemTypeCategory == null)
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCategoryMissing, (object) categoryReferenceName));
      foreach (string workItemTypeName in itemTypeCategory.WorkItemTypeNames)
      {
        WorkItemTypeMetadata workItemType = context.GetWorkItemType(workItemTypeName);
        if (workItemType == null || workItemType.GetField(fieldReferenceName) == null)
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    protected IEnumerable<string> GetWorkItemTypeNamesForCategory(
      IProjectMetadata projectMetadata,
      string categoryReferenceName)
    {
      return (projectMetadata.GetWorkItemTypeCategory(categoryReferenceName) ?? throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ProvisionErrorCategoryMissing, (object) categoryReferenceName))).WorkItemTypeNames;
    }

    protected bool IsCategorySubsetOrNotOverlapping(
      IProjectMetadata projectMetadata,
      string sourceCategory,
      string targetCategory,
      out IEnumerable<string> commonWorkItemTypes)
    {
      IEnumerable<string> namesForCategory1 = this.GetWorkItemTypeNamesForCategory(projectMetadata, sourceCategory);
      IEnumerable<string> namesForCategory2 = this.GetWorkItemTypeNamesForCategory(projectMetadata, targetCategory);
      commonWorkItemTypes = namesForCategory1.Intersect<string>(namesForCategory2);
      return !commonWorkItemTypes.Any<string>() || !namesForCategory1.Except<string>(commonWorkItemTypes).Any<string>();
    }

    protected bool IsCategoryOverlappingWithPortfolios(
      IProjectMetadata projectMetadata,
      string categoryRefName)
    {
      foreach (CategoryConfiguration portfolioBacklog in projectMetadata.GetProcessConfiguration().PortfolioBacklogs)
      {
        IEnumerable<string> commonWorkItemTypes;
        this.IsCategorySubsetOrNotOverlapping(projectMetadata, categoryRefName, portfolioBacklog.CategoryReferenceName, out commonWorkItemTypes);
        if (commonWorkItemTypes.Any<string>())
          return true;
      }
      return false;
    }

    protected IEnumerable<string> RemoveOverlappingWorkItemTypesInCategory(
      IProjectProvisioningContext context,
      string sourceCategory,
      string targetCategory)
    {
      IEnumerable<string> commonWorkItemTypes;
      if (!this.IsCategorySubsetOrNotOverlapping((IProjectMetadata) context, sourceCategory, targetCategory, out commonWorkItemTypes))
        context.ReportIssue(new ProjectProvisioningIssue(Resources.WorkItemCategories_Configuration_Error, IssueLevel.Error));
      else if (commonWorkItemTypes.Any<string>())
      {
        WorkItemTypeCategory itemTypeCategory = context.GetWorkItemTypeCategory(targetCategory);
        WorkItemTypeCategory category = new WorkItemTypeCategory(itemTypeCategory.Name, itemTypeCategory.ReferenceName, itemTypeCategory.WorkItemTypeNames.Except<string>(commonWorkItemTypes), itemTypeCategory.DefaultWorkItemTypeName);
        context.AddWorkItemTypeCategory(category);
      }
      return commonWorkItemTypes;
    }

    protected enum EnsureCategoryMode
    {
      DoNotOverwrite,
      Merge,
    }

    protected enum EnsureWorkItemMode
    {
      FailIfMissingDoNotOverwriteIfExists,
      RemoveFromCategoryIfMissingDoNotOverwriteIfExists,
      AddIfMissingDoNotOverwriteButWarnIfExists,
    }
  }
}
