// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestSettingsTypeQueryField
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestSettingsTypeQueryField : TestArtifactQueryField<TestRun, TestSettings>
  {
    public TestSettingsTypeQueryField(TestManagerRequestContext testContext)
      : base(testContext, "TestSettingsId", TestManagementServerResources.QueryColumnNameTestSettings, true, (Func<TestRun, int>) (run => run.PublicTestSettingsId))
    {
    }

    protected override IEnumerable<TestSettings> GetArtifacts()
    {
      List<TestSettings> artifacts = TestSettings.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.GetResultsStoreQuery(TestManagementConstants.Wiql_All_TestSettings), false);
      if (artifacts != null && !artifacts.Exists((Predicate<TestSettings>) (s => s.Id == 0)))
        artifacts.Insert(0, new TestSettings()
        {
          Id = 0,
          Name = TestManagementServerResources.TestSettingsDefault
        });
      return (IEnumerable<TestSettings>) artifacts;
    }

    protected override string GetArtifactName(TestSettings artifact) => artifact.Name;

    protected override int GetArtifactId(TestSettings artifact) => artifact.Id;

    public override string ConvertRawValueToDisplayValue(object value)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestSettingsTypeQueryField.ConvertRawValueToDisplayValue");
        int int32 = Convert.ToInt32(value);
        this.EnsurePopulatedDictionaries();
        TestSettings testSettings;
        return this.ArtifactById.TryGetValue(int32, out testSettings) ? testSettings.Name : string.Empty;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestSettingsTypeQueryField.ConvertRawValueToDisplayValue");
      }
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestSettingsTypeQueryField.ConvertDisplayValueToRawValue");
        if (!string.IsNullOrEmpty(displayValue))
        {
          this.EnsurePopulatedDictionaries();
          TestSettings testSettings;
          if (this.ArtifactByName != null && this.ArtifactByName.TryGetValue(displayValue, out testSettings))
            return (object) testSettings.Id;
        }
        return (object) -1;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestSettingsTypeQueryField.ConvertDisplayValueToRawValue");
      }
    }
  }
}
