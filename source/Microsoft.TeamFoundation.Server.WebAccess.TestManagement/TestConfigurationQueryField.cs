// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestConfigurationQueryField
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestConfigurationQueryField : 
    TestArtifactQueryField<TestCaseResult, TestConfiguration>
  {
    public TestConfigurationQueryField(TestManagerRequestContext testContext)
      : base(testContext, "ConfigurationId", TestManagementServerResources.QueryColumnNameConfiguration, true, (Func<TestCaseResult, int>) (res => res.ConfigurationId))
    {
    }

    protected override IEnumerable<TestConfiguration> GetArtifacts()
    {
      List<TestConfiguration> artifacts = TestConfiguration.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.GetResultsStoreQuery(TestManagementConstants.Wiql_All_Configurations), 0);
      if (artifacts != null && !artifacts.Exists((Predicate<TestConfiguration>) (s => s.Id == 0)))
        artifacts.Insert(0, new TestConfiguration()
        {
          Id = 0,
          Name = TestManagementServerResources.ConfigurationNone
        });
      return (IEnumerable<TestConfiguration>) artifacts;
    }

    protected override string GetArtifactName(TestConfiguration artifact) => artifact.Name;

    protected override int GetArtifactId(TestConfiguration artifact) => artifact.Id;

    public override string ConvertRawValueToDisplayValue(object value)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestConfigurationQueryField.ConvertRawValueToDisplayValue");
        int int32 = Convert.ToInt32(value);
        this.EnsurePopulatedDictionaries();
        TestConfiguration testConfiguration;
        return this.ArtifactById.TryGetValue(int32, out testConfiguration) ? testConfiguration.Name : string.Empty;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestConfigurationQueryField.ConvertRawValueToDisplayValue");
      }
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestConfigurationQueryField.ConvertDisplayValueToRawValue");
        if (!string.IsNullOrEmpty(displayValue))
        {
          this.EnsurePopulatedDictionaries();
          TestConfiguration testConfiguration;
          if (this.ArtifactByName != null && this.ArtifactByName.TryGetValue(displayValue, out testConfiguration))
            return (object) testConfiguration.Id;
        }
        return (object) -1;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestConfigurationQueryField.ConvertDisplayValueToRawValue");
      }
    }
  }
}
