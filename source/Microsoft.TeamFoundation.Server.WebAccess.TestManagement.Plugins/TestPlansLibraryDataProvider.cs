// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.TestPlansLibraryDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 53130500-4E07-459F-A593-E61E658993AF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement.Plugins
{
  public class TestPlansLibraryDataProvider : IExtensionDataProvider
  {
    private const string TestCasesPivotName = "testcases";
    private const string SharedStepsPivotName = "sharedsteps";
    private const string SharedParametersPivotName = "sharedparameters";
    private const string ConfigurationsPivotName = "configurations";

    public string Name => "TestManagement.Provider.TestPlansLibraryDataProvider";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      Guid empty1 = Guid.Empty;
      Guid empty2 = Guid.Empty;
      string empty3 = string.Empty;
      string pageUrl = string.Empty;
      using (PerformanceTimer.StartMeasure(requestContext, "TestPlansLibraryDataProvider.GetProjectAndTeamData"))
        pageUrl = this.GetProjectAndTeamData(requestContext).pageUrl;
      switch (this.GetLibraryPivot(pageUrl))
      {
        case TestPlansLibraryDataProvider.LibraryPivots.TestCases:
          return new LibraryTestCasesDataProvider().GetData(requestContext, providerContext, contribution);
        case TestPlansLibraryDataProvider.LibraryPivots.SharedSteps:
          return new LibrarySharedStepsDataProvider().GetData(requestContext, providerContext, contribution);
        case TestPlansLibraryDataProvider.LibraryPivots.SharedParameters:
        case TestPlansLibraryDataProvider.LibraryPivots.Configurations:
          return (object) null;
        default:
          requestContext.Trace(1015694, TraceLevel.Warning, "TestManagement", "WebService", "Returning null from TestPlansLibraryDataProvider.GetData");
          return (object) null;
      }
    }

    protected internal virtual (Guid projectId, string projectName, Guid teamId, string pageUrl) GetProjectAndTeamData(
      IVssRequestContext requestContext)
    {
      return Utils.GetProjectAndTeamData(requestContext);
    }

    private TestPlansLibraryDataProvider.LibraryPivots GetLibraryPivot(string pageUrl)
    {
      string[] segments = new Uri(pageUrl).Segments;
      if (segments != null && ((IEnumerable<string>) segments).Count<string>() > 0)
      {
        string str = segments[((IEnumerable<string>) segments).Count<string>() - 1];
        if (str.Equals("testcases", StringComparison.OrdinalIgnoreCase))
          return TestPlansLibraryDataProvider.LibraryPivots.TestCases;
        if (str.Equals("sharedsteps", StringComparison.OrdinalIgnoreCase))
          return TestPlansLibraryDataProvider.LibraryPivots.SharedSteps;
        if (str.Equals("sharedparameters", StringComparison.OrdinalIgnoreCase))
          return TestPlansLibraryDataProvider.LibraryPivots.SharedParameters;
        if (str.Equals("configurations", StringComparison.OrdinalIgnoreCase))
          return TestPlansLibraryDataProvider.LibraryPivots.Configurations;
      }
      return TestPlansLibraryDataProvider.LibraryPivots.TestCases;
    }

    private enum LibraryPivots
    {
      TestCases,
      SharedSteps,
      SharedParameters,
      Configurations,
    }
  }
}
