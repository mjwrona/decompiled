// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SuiteEntryController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "SuiteEntry", ResourceVersion = 1)]
  public class SuiteEntryController : TestManagementController
  {
    [HttpGet]
    [ClientLocationId("BF8B7F78-0C1F-49CB-89E9-D1A17BCAAAD3")]
    [ClientExample("GET__test_suiteentry__suiteId_.json", null, null, null)]
    public IEnumerable<SuiteEntry> GetSuiteEntries(int suiteId) => this.SuitesHelper.GetSuiteEntriesForSuite(this.ProjectId, suiteId);

    [HttpPatch]
    [ClientLocationId("BF8B7F78-0C1F-49CB-89E9-D1A17BCAAAD3")]
    [ClientExample("PATCH__test_suiteentry__suiteId_.json", "Reorder test suites", null, null)]
    [ClientExample("PATCH__test_suiteentry__testcaseId_.json", "Reorder test cases", null, null)]
    [ClientExample("PATCH__test_suiteentry__suiteId_testcaseId_.json", "Reorder test cases and test suites", null, null)]
    [ClientExample("PATCH__test_suiteentry__suiteId_.json", null, null, null)]
    public IEnumerable<SuiteEntry> ReorderSuiteEntries(
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries)
    {
      return this.SuitesHelper.ReorderSuiteEntries(this.ProjectId, suiteId, suiteEntries);
    }
  }
}
