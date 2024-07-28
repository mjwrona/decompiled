// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.TestCaseCache
// Assembly: Microsoft.TeamFoundation.TestImpact.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1ECF5BB1-1B8D-4502-95D9-1C6B9B1F7C03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestImpact.Server.Common.Properties;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestImpact.Server
{
  internal sealed class TestCaseCache : IVssFrameworkService
  {
    private Dictionary<int, TestCaseCache.TestCaseEntry> cacheData;
    private const int EXPIRATION_TIME_IN_MINUTES = 5;
    private const string TestIdFieldName = "Microsoft.VSTS.TCM.AutomatedTestId";
    private static readonly string[] m_testCasesFields = new string[2]
    {
      "System.Title",
      "Microsoft.VSTS.TCM.AutomatedTestId"
    };

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => this.cacheData = new Dictionary<int, TestCaseCache.TestCaseEntry>();

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => this.cacheData = (Dictionary<int, TestCaseCache.TestCaseEntry>) null;

    internal bool GetTestCaseInfo(
      IVssRequestContext requestContext,
      int testCaseId,
      out string title,
      out string type,
      out Guid automatedTestId)
    {
      title = (string) null;
      type = (string) null;
      automatedTestId = Guid.Empty;
      if (this.cacheData == null)
        return false;
      TestCaseCache.TestCaseEntry testCaseEntry;
      if (!this.cacheData.TryGetValue(testCaseId, out testCaseEntry) || testCaseEntry.HasExpired)
      {
        WorkItemFieldData workItemFieldData = requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemFieldValues(requestContext, (IEnumerable<int>) new int[1]
        {
          testCaseId
        }, (IEnumerable<string>) TestCaseCache.m_testCasesFields).FirstOrDefault<WorkItemFieldData>();
        if (workItemFieldData == null)
        {
          this.cacheData.Remove(testCaseId);
          return false;
        }
        testCaseEntry.Title = workItemFieldData.Title;
        string fieldValue = workItemFieldData.GetFieldValue<string>(requestContext, "Microsoft.VSTS.TCM.AutomatedTestId");
        if (string.IsNullOrEmpty(fieldValue))
        {
          testCaseEntry.TestType = Resources.ManualTestCase;
        }
        else
        {
          testCaseEntry.TestType = Resources.AutomatedTestCase;
          testCaseEntry.AutomatedTestId = new Guid(fieldValue);
        }
        testCaseEntry.ExpireTime = DateTime.Now.AddMinutes(5.0);
        this.cacheData[testCaseId] = testCaseEntry;
      }
      title = testCaseEntry.Title;
      type = testCaseEntry.TestType;
      automatedTestId = testCaseEntry.AutomatedTestId;
      return true;
    }

    private static string GetStringField(PayloadTable.PayloadRow testCase, string field)
    {
      try
      {
        object obj = testCase[field];
        return obj != null ? (string) obj : string.Empty;
      }
      catch
      {
        return string.Empty;
      }
    }

    private struct TestCaseEntry
    {
      public string Title { get; set; }

      public string TestType { get; set; }

      public Guid AutomatedTestId { get; set; }

      public DateTime ExpireTime { get; set; }

      public bool HasExpired => this.ExpireTime < DateTime.Now;
    }
  }
}
