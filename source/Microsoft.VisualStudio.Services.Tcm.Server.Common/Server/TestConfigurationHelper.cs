// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestConfigurationHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestConfigurationHelper : ITestConfigurationHelper
  {
    private const string activeConfigurationQuery = "SELECT * FROM TestConfiguration WHERE State = 'Active' ORDER BY Id";

    public List<int> FetchConfigurationIds(
      TestManagementRequestContext requestContext,
      string projectName)
    {
      List<int> intList = new List<int>();
      List<TestConfiguration> testConfigurationList = this.FetchConfigurations(requestContext, projectName);
      if (testConfigurationList != null)
      {
        foreach (TestConfiguration testConfiguration in testConfigurationList)
          intList.Add(testConfiguration.Id);
      }
      return intList;
    }

    public List<TestConfiguration> FetchConfigurations(
      TestManagementRequestContext requestContext,
      string projectName)
    {
      TimeZoneInfo utc = TimeZoneInfo.Utc;
      return TestConfiguration.Query(requestContext, new ResultsStoreQuery()
      {
        QueryText = string.Format("SELECT * FROM TestConfiguration ORDER BY Id"),
        TeamProjectName = projectName,
        TimeZone = utc.ToSerializedString()
      }, 0);
    }

    public List<TestConfiguration> FetchActiveConfigurations(
      TestManagementRequestContext requestContext,
      string projectName)
    {
      TimeZoneInfo utc = TimeZoneInfo.Utc;
      return TestConfiguration.Query(requestContext, new ResultsStoreQuery()
      {
        QueryText = string.Format("SELECT * FROM TestConfiguration WHERE State = 'Active' ORDER BY Id"),
        TeamProjectName = projectName,
        TimeZone = utc.ToSerializedString()
      }, 0);
    }
  }
}
