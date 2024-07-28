// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCases5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "TestCases", ResourceVersion = 1)]
  public class TestCases5Controller : TestManagementController
  {
    [HttpDelete]
    [ClientLocationId("29006fb5-816b-4ff7-a329-599943569229")]
    [ClientExample("DeleteTestCases.json", "Delete a test case", null, null)]
    public void DeleteTestCase(int testCaseId)
    {
      if (!TfsRestApiHelper.DoesWorkItemExistInExpectedCategory(this.TestManagementRequestContext, testCaseId, this.ProjectInfo.Name, "Microsoft.TestCaseCategory"))
        throw new TestManagementInvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServerResources.InvalidWorkItemPassed, (object) testCaseId, (object) "Microsoft.TestCaseCategory"));
      TestCaseHelper.DeleteTestCase(this.TestManagementRequestContext, this.ProjectInfo.Name, testCaseId);
    }
  }
}
