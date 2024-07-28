// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueriesStore
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueriesStore
  {
    public QueriesStore(
      TestManagerRequestContext testContext,
      QueryAdapterFactory queryAdapterFactory)
    {
      this.TestContext = testContext;
      this.QueryAdapterFactory = queryAdapterFactory;
    }

    public TestManagerRequestContext TestContext { get; private set; }

    public QueryAdapterFactory QueryAdapterFactory { get; private set; }

    public virtual QueryHierarchyModel GetQueryHierarchy(
      IEnumerable<string> itemTypes,
      TestManagerRequestContext context)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryStore.GetQueryHierarchy");
        QueryHierarchyModel queryHierarchy = new QueryHierarchyModel();
        List<QueryModel> queryModelList = new List<QueryModel>();
        if (itemTypes.Contains<string>("TestRun"))
        {
          QueryAdapter adapter = this.QueryAdapterFactory.GetAdapter("TestRun");
          QueryModel queryModel1 = new QueryModel()
          {
            Id = "8AC4BE34-5582-43AF-8830-6797C47F5870",
            Name = TestManagementResources.RecentRuns,
            ItemType = "TestRun",
            IsFolder = false,
            Filter = adapter.GetDefaultFilter(),
            Columns = adapter.GetDefaultColumns()
          };
          queryModelList.Add(queryModel1);
          QueryModel queryModel2 = new QueryModel()
          {
            Id = "3018B3CE-1579-4538-9556-A6092E4C1E1A",
            Name = TestManagementServerResources.RunTitleQueryText,
            ItemType = "TestRun",
            IsFolder = false,
            Filter = adapter.GetTestRunTitleFilter(),
            Columns = adapter.GetTitleColumn()
          };
          queryModelList.Add(queryModel2);
        }
        if (itemTypes.Contains<string>("TestResult"))
        {
          QueryAdapter adapter = this.QueryAdapterFactory.GetAdapter("TestResult");
          QueryModel queryModel = new QueryModel()
          {
            Id = "6618FEF0-354F-4CDF-960A-D01E5E00B173",
            Name = TestManagementServerResources.TestRunsTabTitle,
            ItemType = "TestResult",
            IsFolder = true,
            Filter = adapter.GetDefaultFilter(),
            Columns = adapter.GetDefaultColumns()
          };
          queryModelList.Add(queryModel);
        }
        queryHierarchy.Queries = (IEnumerable<QueryModel>) queryModelList;
        return queryHierarchy;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryStore.GetQueryHierarchy");
      }
    }
  }
}
