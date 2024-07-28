// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestMethodNameSanitizer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Utility;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestMethodNameSanitizer : ITestCasePropertiesSanitizer
  {
    public const string FullyQualifiedNameRegexString = "\\s*(\\(((\"((\\\\\")|[^\"])*\")|[^\\)])*\\))[\\)]*";
    private static readonly Regex NormalizedFullyQualifiedNameReplacementRegex = new Regex("\\s*(\\(((\"((\\\\\")|[^\"])*\")|[^\\)])*\\))[\\)]*", RegexOptions.Compiled, TimeSpan.FromMilliseconds((double) TestResultsConstants.RegexTimeOutInMilliSeconds));
    private IRegexWrapper regexWrapper;

    public TestMethodNameSanitizer()
      : this((IRegexWrapper) new RegexWrapper(TestMethodNameSanitizer.NormalizedFullyQualifiedNameReplacementRegex))
    {
    }

    public TestMethodNameSanitizer(IRegexWrapper regexWrapper) => this.regexWrapper = regexWrapper;

    public void Sanitize(TestManagementRequestContext context, TestCaseResult result)
    {
      result.AutomatedTestName = this.Sanitize(context, result.AutomatedTestName);
      result.TestCaseTitle = this.Sanitize(context, result.TestCaseTitle);
    }

    public string Sanitize(TestManagementRequestContext context, string testMethodName)
    {
      try
      {
        return context.RequestContext.IsFeatureEnabled("TestManagement.Server.SanitizeTestMethodNameRemoveAllAfterBracket") ? this.TrimNameFromFirstBracket(testMethodName) : this.regexWrapper.Replace(testMethodName, string.Empty);
      }
      catch (RegexMatchTimeoutException ex)
      {
        context.RequestContext.Trace(1015098, TraceLevel.Error, "TestManagement", "RestLayer", string.Format("sanitizing of string: '{0}' has failed with exception: {1}", (object) testMethodName, (object) ex));
      }
      return testMethodName;
    }

    private string TrimNameFromFirstBracket(string name)
    {
      if (!string.IsNullOrEmpty(name))
      {
        for (int index = 0; index < name.Length; ++index)
        {
          switch (name[index])
          {
            case '(':
            case '<':
            case '[':
            case '{':
              return name.Substring(0, index);
            default:
              continue;
          }
        }
      }
      return name;
    }
  }
}
