// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GroupByHelperFactory
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class GroupByHelperFactory
  {
    public static IGroupByHelper GetHelper(
      List<string> groupByFields,
      IVssRequestContext requestContext,
      Guid projectId,
      Func<IQueryGroupedTestResultsColumns> getQueryGroupedTestResultsColumns)
    {
      if (groupByFields != null)
      {
        foreach (string groupByField in groupByFields)
        {
          if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            GroupByTestSuiteHelper helper = new GroupByTestSuiteHelper();
            helper.RequestContext = requestContext;
            helper.ProjectId = projectId;
            helper.GetGroupedQueryResultsColumns = getQueryGroupedTestResultsColumns;
            return (IGroupByHelper) helper;
          }
          if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            GroupByRequirementHelper helper = new GroupByRequirementHelper();
            helper.RequestContext = requestContext;
            helper.ProjectId = projectId;
            helper.GetGroupedQueryResultsColumns = getQueryGroupedTestResultsColumns;
            return (IGroupByHelper) helper;
          }
        }
      }
      GroupByTestResultFieldHelper helper1 = new GroupByTestResultFieldHelper();
      helper1.RequestContext = requestContext;
      helper1.ProjectId = projectId;
      helper1.GetGroupedQueryResultsColumns = getQueryGroupedTestResultsColumns;
      return (IGroupByHelper) helper1;
    }
  }
}
