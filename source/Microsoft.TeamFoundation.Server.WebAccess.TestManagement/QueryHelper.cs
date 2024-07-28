// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class QueryHelper : TestHelperBase
  {
    private Dictionary<string, List<string>> m_mapOfCategoryToNames = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);

    public QueryHelper(TestManagerRequestContext context)
      : base(context)
    {
      WebAccessWorkItemService service = this.TestContext.TfsRequestContext.GetService<WebAccessWorkItemService>();
      foreach (Project project in service.GetProjects(this.TestContext.TfsRequestContext))
      {
        foreach (WorkItemTypeCategory itemTypeCategory in service.GetWorkItemTypeCategories(this.TestContext.TfsRequestContext, project.Name))
        {
          WorkItemTypeCategory category = itemTypeCategory;
          if (!this.m_mapOfCategoryToNames.ContainsKey(category.ReferenceName))
            this.m_mapOfCategoryToNames[category.ReferenceName] = new List<string>();
          if (!this.m_mapOfCategoryToNames[category.ReferenceName].Exists((Predicate<string>) (name => string.Equals(category.Name, name, StringComparison.CurrentCultureIgnoreCase))))
            this.m_mapOfCategoryToNames[category.ReferenceName].Add(category.Name);
        }
      }
    }

    internal void CheckTestCaseCategoryCondition(string queryText, string categoryRefName)
    {
      NodeSelect syntax;
      try
      {
        syntax = Parser.ParseSyntax(queryText);
      }
      catch (SyntaxException ex)
      {
        throw new LegacyValidationException(ex.Details);
      }
      if (Microsoft.TeamFoundation.WorkItemTracking.Internals.LinkQueryMode.WorkItems == WiqlAdapter.GetQueryMode(syntax))
      {
        if (syntax.Where != null)
        {
          if (syntax.Where.NodeType == NodeType.FieldCondition && this.HasCategoryCondition((NodeCondition) syntax.Where, categoryRefName))
            return;
          if (syntax.Where.NodeType == NodeType.And)
          {
            foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node condition in syntax.Where)
            {
              if (condition.NodeType == NodeType.FieldCondition && this.HasCategoryCondition((NodeCondition) condition, categoryRefName))
                return;
            }
          }
        }
        throw new LegacyValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TestManagementServerResources.QueryNeedsCategoryCondition, (object) categoryRefName));
      }
    }

    internal virtual List<int> GetUserStoriesHavingAssociatedTestCases(int[] userStoryIds) => TestCaseHelper.QueryTestCasesForUserStories((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName, userStoryIds).Keys.ToList<int>();

    private bool HasCategoryCondition(NodeCondition condition, string categoryRefName)
    {
      if (condition.Condition != Condition.Group || !condition.Left.Value.Contains(WorkItemFieldNames.WorkItemType))
        return false;
      return string.Equals(condition.Right.ConstStringValue, categoryRefName) || this.hasCategoryName(categoryRefName, condition.Right.ConstStringValue);
    }

    private bool hasCategoryName(string categoryRefName, string stringValue) => this.m_mapOfCategoryToNames.ContainsKey(categoryRefName) && this.m_mapOfCategoryToNames[categoryRefName].Exists((Predicate<string>) (name => string.Equals(stringValue, name, StringComparison.CurrentCultureIgnoreCase)));
  }
}
