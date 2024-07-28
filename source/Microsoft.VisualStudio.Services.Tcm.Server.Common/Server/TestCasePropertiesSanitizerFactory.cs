// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCasePropertiesSanitizerFactory
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestCasePropertiesSanitizerFactory
  {
    public static ITestCasePropertiesSanitizer Create(IVssRequestContext tfsRequestContext)
    {
      if (!Enum.TryParse<TestCasePropertiesSanitizerFactory.SanitizationScheme>(TestCasePropertiesSanitizerFactory.GetSanitizationSchemeSetting(tfsRequestContext), true, out TestCasePropertiesSanitizerFactory.SanitizationScheme _))
        return (ITestCasePropertiesSanitizer) new DefaultTestCasePropertiesSanitizer();
      return (ITestCasePropertiesSanitizer) new DefaultTestCasePropertiesSanitizer();
    }

    private static string GetSanitizationSchemeSetting(IVssRequestContext tfsRequestContext)
    {
      try
      {
        return tfsRequestContext.GetService<IVssRegistryService>().GetValue<string>(tfsRequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TestCasePropertiesSanitizationScheme", false, (string) null);
      }
      catch (Exception ex)
      {
        VssRequestContextExtensions.Trace(tfsRequestContext, 1015641, TraceLevel.Error, "TestManagement", "Exceptions", "Error while fetching sanitization scheme: {0}", new object[1]
        {
          (object) ex
        });
        return string.Empty;
      }
    }

    internal enum SanitizationScheme
    {
      Default,
    }
  }
}
