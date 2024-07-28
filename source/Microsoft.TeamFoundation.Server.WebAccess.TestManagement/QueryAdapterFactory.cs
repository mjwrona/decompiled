// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryAdapterFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryAdapterFactory
  {
    private Dictionary<string, Func<TestManagerRequestContext, QueryAdapter>> s_registeredAdapterCreators = new Dictionary<string, Func<TestManagerRequestContext, QueryAdapter>>();

    public QueryAdapterFactory(TestManagerRequestContext testContext) => this.TestContext = testContext;

    public TestManagerRequestContext TestContext { get; private set; }

    public virtual QueryAdapter GetAdapter(string queryableItemType)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "QueryAdapterFactory.GetAdapter");
        Func<TestManagerRequestContext, QueryAdapter> registeredAdapterCreator;
        if (!this.s_registeredAdapterCreators.TryGetValue(queryableItemType, out registeredAdapterCreator))
        {
          switch (queryableItemType)
          {
            case "TestResult":
              this.s_registeredAdapterCreators[queryableItemType] = (Func<TestManagerRequestContext, QueryAdapter>) (testContext => (QueryAdapter) new TestResultQueryAdapter(testContext));
              registeredAdapterCreator = this.s_registeredAdapterCreators[queryableItemType];
              break;
            case "TestRun":
              this.s_registeredAdapterCreators[queryableItemType] = (Func<TestManagerRequestContext, QueryAdapter>) (testContext => (QueryAdapter) new TestRunQueryAdapter(testContext));
              registeredAdapterCreator = this.s_registeredAdapterCreators[queryableItemType];
              break;
            default:
              this.TestContext.TraceError("BusinessLayer", "Unknown query type specified. Query Item: {0}", (object) queryableItemType);
              throw new TeamFoundationServerException(TestManagementServerResources.ErrorUnknownQueryTypeFormat);
          }
        }
        QueryAdapter adapter = registeredAdapterCreator(this.TestContext);
        if (adapter == null)
        {
          this.TestContext.TraceError("BusinessLayer", "Adapter not found for Query Item: {0}", (object) queryableItemType);
          throw new TeamFoundationServerException(TestManagementServerResources.ErrorUnknownQueryTypeFormat);
        }
        return adapter;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "QueryAdapterFactory.GetAdapter");
      }
    }
  }
}
