// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.LibrarySharedStepsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class LibrarySharedStepsDataProvider : LibraryWorkItemsDataProvider
  {
    private static readonly string c_AllSharedSteps = "SELECT [System.Id]\r\n                                                        FROM WorkItems \r\n                                                        WHERE [System.TeamProject] = @project AND [System.WorkItemType] = 'Shared Steps'";
    private static readonly string c_UnreferencedSharedSteps = "SELECT [System.Links.LinkType], [System.Id]\r\n                                                                 FROM WorkItemLinks \r\n                                                                 WHERE System.Links.LinkType = 'Microsoft.VSTS.TestCase.SharedStepReferencedBy-Forward' \r\n                                                                 AND [Source].[System.WorkItemType] = 'Shared Steps'\r\n                                                                 AND [Target].[System.WorkItemType] IN GROUP 'Microsoft.TestCaseCategory'";

    public LibrarySharedStepsDataProvider()
    {
      this.m_queries = new List<LibraryWorkItemsDataProvider.WorkItemQueryAndType>();
      this.m_queries.Add(new LibraryWorkItemsDataProvider.WorkItemQueryAndType()
      {
        QueryType = TestPlansLibraryQuery.AllSharedSteps,
        Query = LibrarySharedStepsDataProvider.c_AllSharedSteps
      });
      this.m_queries.Add(new LibraryWorkItemsDataProvider.WorkItemQueryAndType()
      {
        QueryType = TestPlansLibraryQuery.SharedStepsNotLinkedToRequirement,
        Query = LibrarySharedStepsDataProvider.c_UnreferencedSharedSteps
      });
    }

    public virtual object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      return this.GetPaginatedWorkItemData(requestContext, providerContext);
    }

    protected override LibraryWorkItemsDataProvider.WorkItemQueryAndType AppendFilters(
      LibraryWorkItemsDataProvider.WorkItemQueryAndType query,
      List<TestPlansLibraryWorkItemFilter> filterValues)
    {
      if (filterValues.IsNullOrEmpty<TestPlansLibraryWorkItemFilter>())
        return query;
      string wiqlColumnPrefix = string.Empty;
      if (query.WitQueryType != QueryType.WorkItems)
        wiqlColumnPrefix = LibraryWorkItemsDataProvider.c_wiqlSourcePrefix;
      string empty = string.Empty;
      string str = this.BuildWorkItemFilters(filterValues, new List<string>()
      {
        WorkItemFieldRefNames.Title,
        WorkItemFieldRefNames.State,
        WorkItemFieldRefNames.AssignedTo,
        WorkItemFieldRefNames.AreaPath
      }, wiqlColumnPrefix);
      query.Query = string.Format("{0} {1}", (object) query.Query, (object) str);
      return query;
    }
  }
}
