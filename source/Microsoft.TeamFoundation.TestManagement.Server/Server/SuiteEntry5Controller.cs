// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteEntry5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TestPlanning;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientGroupByResource("Test Suite Entry")]
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "SuiteEntry", ResourceVersion = 1)]
  public class SuiteEntry5Controller : TestManagementController
  {
    [HttpGet]
    [ClientLocationId("d6733edf-72f1-4252-925b-c560dfe9b75a")]
    [ClientExample("GetSuiteEntries.json", "Get a list of all suite Entries", null, null)]
    [ClientExample("GetChildSuiteEntries.json", "Get a list of child suites", null, null)]
    [ClientExample("GetTestCaseEntries.json", "Get a list of test cases in a suite", null, null)]
    public IEnumerable<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry> GetSuiteEntries(
      int suiteId,
      SuiteEntryTypes? suiteEntryType = null)
    {
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry> suiteEntriesForSuite = this.SuitesHelper.GetSuiteEntriesForSuite(this.ProjectId, suiteId);
      List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry> suiteEntries = new List<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry suiteEntry in suiteEntriesForSuite)
      {
        Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry webApiSuiteEntry = TestSuiteEntryAdapter.ToNewWebApiSuiteEntry(suiteEntry);
        if (suiteEntryType.HasValue)
        {
          SuiteEntryTypes? nullable = suiteEntryType;
          SuiteEntryTypes suiteEntryType1 = webApiSuiteEntry.SuiteEntryType;
          if (!(nullable.GetValueOrDefault() == suiteEntryType1 & nullable.HasValue))
            continue;
        }
        suiteEntries.Add(webApiSuiteEntry);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry>) suiteEntries;
    }

    [HttpPatch]
    [ClientLocationId("d6733edf-72f1-4252-925b-c560dfe9b75a")]
    [ClientExample("UpdateTestCaseEntryOrder.json", "Reorder test cases in a suite", null, null)]
    [ClientExample("UpdateChildSuiteEntryOrder.json", "Reorder child suites in a suite", null, null)]
    [ClientExample("UpdateTestCaseAndChildSuiteEntryOrder.json", "Reorder both test cases and child suites", null, null)]
    [ClientExample("UpdateTestCaseEntryOrderWithConflcitingSequenceNumber.json", "Reorder test cases with conflicting sequence Number", "If the user provided sequence numbers of 2 items are identical, they will be arranged based on their type and  IDs, with the lowest ID getting higher up in the order and suite type entries getting preference over test case entries.", null)]
    [ClientExample("UpdateTestCaseEntryOrderWithOverflowingSequenceOrder.json", "Reorder test cases with ovrflowing sequence number", null, null)]
    public IEnumerable<Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry> ReorderSuiteEntries(
      int suiteId,
      IEnumerable<SuiteEntryUpdateParams> suiteEntries)
    {
      IEnumerable<SuiteEntryUpdateModel> suiteEntries1 = suiteEntries.Select<SuiteEntryUpdateParams, SuiteEntryUpdateModel>((Func<SuiteEntryUpdateParams, SuiteEntryUpdateModel>) (suiteEntry => TestSuiteEntryAdapter.ToSuiteEntryUpdateModel(suiteEntry)));
      return this.SuitesHelper.ReorderSuiteEntries(this.ProjectId, suiteId, suiteEntries1).Select<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry, Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry>) (suiteEntry => TestSuiteEntryAdapter.ToNewWebApiSuiteEntry(suiteEntry)));
    }
  }
}
