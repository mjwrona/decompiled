// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultTestCasePropertiesSanitizer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class DefaultTestCasePropertiesSanitizer : ITestCasePropertiesSanitizer
  {
    private const string c_unsanitizedTestNamePattern = "^(([A-Z]:\\\\)|/).*\\.(dll|exe)\\..*";
    private const string c_unsanitizedTestStoragePattern = "^(([A-Z]:\\\\)|/).*\\.(dll|exe)";
    private const string c_testContainerExtensionFilter = ".dll";
    private const string c_testContainerExeExtensionFilter = ".exe";
    private const string c_testPathSeparator = "\\";
    private const string c_testPathSeparatorNonWindows = "/";
    private static readonly string[] s_testPathSeparators = new string[2]
    {
      "\\",
      "/"
    };
    private static Regex c_unsanitizedTestNamePatternRegex = new Regex("^(([A-Z]:\\\\)|/).*\\.(dll|exe)\\..*", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds((double) TestResultsConstants.RegexTimeOutInMilliSeconds));
    private static Regex c_unsanitizedTestStoragePatternRegex = new Regex("^(([A-Z]:\\\\)|/).*\\.(dll|exe)", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds((double) TestResultsConstants.RegexTimeOutInMilliSeconds));

    public void Sanitize(TestManagementRequestContext context, TestCaseResult result)
    {
      result.AutomatedTestStorage = this.SanitizedTestStorage(context, result.AutomatedTestStorage);
      result.AutomatedTestName = this.SanitizedTestName(context, result.AutomatedTestName);
    }

    private string SanitizedTestName(TestManagementRequestContext context, string testName)
    {
      if (string.IsNullOrEmpty(testName))
        return string.Empty;
      bool flag;
      try
      {
        flag = this.UnsanitizedTestNamePatternRegex.IsMatch(testName);
      }
      catch (Exception ex)
      {
        flag = false;
        if (ex is RegexMatchTimeoutException)
          context.RequestContext.Trace(1015791, TraceLevel.Error, "TestManagement", "RestLayer", string.Format("sanitizing of string: '{0}' has failed with exception: {1}", (object) testName, (object) ex));
      }
      if (!flag)
        return testName;
      string str = ((IEnumerable<string>) testName.Split(new string[2]
      {
        "\\",
        "/"
      }, StringSplitOptions.RemoveEmptyEntries)).Last<string>();
      return !str.Contains(".dll") && !str.Contains(".exe") ? testName : str;
    }

    private string SanitizedTestStorage(TestManagementRequestContext context, string testStorage)
    {
      if (string.IsNullOrEmpty(testStorage))
        return string.Empty;
      bool flag;
      try
      {
        flag = this.UnsanitizedTestStoragePatternRegex.IsMatch(testStorage);
      }
      catch (Exception ex)
      {
        flag = false;
        if (ex is RegexMatchTimeoutException)
          context.RequestContext.Trace(1015792, TraceLevel.Error, "TestManagement", "RestLayer", string.Format("sanitizing of string: '{0}' has failed with exception: {1}", (object) testStorage, (object) ex));
      }
      return flag ? ((IEnumerable<string>) testStorage.Split(DefaultTestCasePropertiesSanitizer.s_testPathSeparators, StringSplitOptions.RemoveEmptyEntries)).Last<string>() : testStorage;
    }

    internal IRegexWrapper UnsanitizedTestNamePatternRegex { get; set; } = (IRegexWrapper) new RegexWrapper(DefaultTestCasePropertiesSanitizer.c_unsanitizedTestNamePatternRegex);

    internal IRegexWrapper UnsanitizedTestStoragePatternRegex { get; set; } = (IRegexWrapper) new RegexWrapper(DefaultTestCasePropertiesSanitizer.c_unsanitizedTestStoragePatternRegex);
  }
}
