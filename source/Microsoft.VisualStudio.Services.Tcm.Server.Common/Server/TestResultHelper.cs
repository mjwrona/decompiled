// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestResultHelper
  {
    internal const int c_firstTestCaseResultId = 100000;

    public static void UpdateResultWithOutcome(TestCaseResult[] testResults, TestOutcome outcome)
    {
      foreach (TestCaseResult testResult in testResults)
      {
        testResult.Outcome = (byte) outcome;
        testResult.State = (byte) 5;
      }
    }

    public static void SaveTestResults(
      TestCaseResult[] testResults,
      TestManagementRequestContext testContext,
      string projectName)
    {
      ResultUpdateRequest[] array = ((IEnumerable<TestCaseResult>) testResults).Select<TestCaseResult, ResultUpdateRequest>((Func<TestCaseResult, ResultUpdateRequest>) (result => new ResultUpdateRequest()
      {
        TestCaseResult = result,
        TestResultId = result.TestResultId,
        TestRunId = result.TestRunId
      })).ToArray<ResultUpdateRequest>();
      TestCaseResult.Update(testContext, array, projectName);
    }

    public static bool TryJsonConvertWithRetry<T>(
      string jsonString,
      out T convertedObject,
      bool cultureInvariant)
    {
      bool flag = JsonUtilities.TryDeserialize<T>(jsonString, out convertedObject, cultureInvariant);
      if (!flag)
        flag = JsonUtilities.TryDeserialize<T>(jsonString.Replace("\\", "\\\\"), out convertedObject, cultureInvariant);
      return flag;
    }
  }
}
