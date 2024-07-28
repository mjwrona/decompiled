// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.QueryHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class QueryHelper
  {
    private Dictionary<string, List<string>> m_mapOfCategoryToNames = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);

    public QueryHelper(IVssRequestContext requestContext)
    {
      using (PerfManager.Measure(requestContext, "CrossService", TraceUtils.GetActionName(nameof (QueryHelper), "WorkItem")))
      {
        foreach (ProjectInfo project in requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed))
        {
          IWorkItemTypeCategoryService service = requestContext.GetService<IWorkItemTypeCategoryService>();
          try
          {
            foreach (WorkItemTypeCategory itemTypeCategory in service.GetWorkItemTypeCategories(requestContext, project.Name))
            {
              WorkItemTypeCategory category = itemTypeCategory;
              if (!this.m_mapOfCategoryToNames.ContainsKey(category.ReferenceName))
                this.m_mapOfCategoryToNames[category.ReferenceName] = new List<string>();
              if (!this.m_mapOfCategoryToNames[category.ReferenceName].Exists((Predicate<string>) (name => string.Equals(category.Name, name, StringComparison.CurrentCultureIgnoreCase))))
                this.m_mapOfCategoryToNames[category.ReferenceName].Add(category.Name);
            }
          }
          catch (WorkItemTrackingProjectNotFoundException ex)
          {
            requestContext.TraceInfo("RestLayer", ex.Message);
          }
        }
      }
    }

    internal void CheckTestCaseCategoryCondition(string queryString, string categoryRefName)
    {
      NodeSelect syntax;
      try
      {
        syntax = Parser.ParseSyntax(queryString);
      }
      catch (SyntaxException ex)
      {
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidPropertyMessage, (object) "QueryString"), (Exception) ex);
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
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidPropertyMessage, (object) "QueryString"), new Exception());
      }
    }

    private bool HasCategoryCondition(NodeCondition condition, string categoryRefName)
    {
      if (condition.Condition != Condition.Group || !condition.Left.Value.Contains(WorkItemFieldNames.WorkItemType))
        return false;
      return string.Equals(condition.Right.ConstStringValue, categoryRefName) || this.hasCategoryName(categoryRefName, condition.Right.ConstStringValue);
    }

    private bool hasCategoryName(string categoryRefName, string stringValue) => this.m_mapOfCategoryToNames.ContainsKey(categoryRefName) && this.m_mapOfCategoryToNames[categoryRefName].Exists((Predicate<string>) (name => string.Equals(stringValue, name, StringComparison.CurrentCultureIgnoreCase)));
  }
}
