// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResultsHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestResultsHelper
  {
    public static string GetTestResultId(int testRunId, int testResultId) => testRunId.ToString() + ";" + testResultId.ToString();

    public static TestCaseResultIdentifier GetTestCaseResultIdentifier(string resultId)
    {
      string[] strArray = resultId.Split(new char[1]{ ';' }, 2);
      int result1;
      int result2;
      if (strArray.Length == 2 && int.TryParse(strArray[0], out result1) && int.TryParse(strArray[1], out result2))
        return new TestCaseResultIdentifier(result1, result2);
      throw new TeamFoundationServerException(string.Format(TestManagementServerResources.ErrorInvalidTestResultIdFormat, (object) resultId));
    }
  }
}
