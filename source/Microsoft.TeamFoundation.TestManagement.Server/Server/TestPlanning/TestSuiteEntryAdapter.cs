// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.TestSuiteEntryAdapter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  public class TestSuiteEntryAdapter
  {
    public static Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry ToNewWebApiSuiteEntry(
      Microsoft.TeamFoundation.TestManagement.WebApi.SuiteEntry suiteEntry)
    {
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry suiteEntry1 = new Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry();
      suiteEntry1.SuiteId = suiteEntry.SuiteId;
      suiteEntry1.SequenceNumber = suiteEntry.SequenceNumber;
      suiteEntry1.Id = 0;
      Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.SuiteEntry webApiSuiteEntry = suiteEntry1;
      if (suiteEntry.TestCaseId != 0)
      {
        webApiSuiteEntry.Id = suiteEntry.TestCaseId;
        webApiSuiteEntry.SuiteEntryType = SuiteEntryTypes.TestCase;
      }
      else if (suiteEntry.ChildSuiteId != 0)
      {
        webApiSuiteEntry.Id = suiteEntry.ChildSuiteId;
        webApiSuiteEntry.SuiteEntryType = SuiteEntryTypes.Suite;
      }
      return webApiSuiteEntry;
    }

    public static SuiteEntryUpdateModel ToSuiteEntryUpdateModel(SuiteEntryUpdateParams updateParams)
    {
      int testCaseId = 0;
      int childSuiteId = 0;
      ArgumentUtility.CheckForNull<SuiteEntryUpdateParams>(updateParams, nameof (updateParams));
      if (updateParams.SuiteEntryType == SuiteEntryTypes.TestCase)
        testCaseId = updateParams.Id;
      else if (updateParams.SuiteEntryType == SuiteEntryTypes.Suite)
        childSuiteId = updateParams.Id;
      return new SuiteEntryUpdateModel(updateParams.SequenceNumber, testCaseId, childSuiteId);
    }
  }
}
