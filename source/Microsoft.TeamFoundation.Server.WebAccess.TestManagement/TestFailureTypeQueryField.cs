// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestFailureTypeQueryField
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestFailureTypeQueryField : TestArtifactQueryField<TestCaseResult, TestFailureType>
  {
    public TestFailureTypeQueryField(TestManagerRequestContext testContext)
      : base(testContext, "FailureType", TestManagementServerResources.QueryColumnNameFailureType, true, (Func<TestCaseResult, int>) (res => (int) res.FailureType))
    {
    }

    protected override IEnumerable<TestFailureType> GetArtifacts() => (IEnumerable<TestFailureType>) TestFailureType.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, -1, this.TestContext.ProjectName);

    protected override string GetArtifactName(TestFailureType artifact) => artifact.Name;

    protected override int GetArtifactId(TestFailureType artifact) => artifact.Id;

    public override string ConvertRawValueToDisplayValue(object value)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestFailureTypeQueryField.ConvertRawValueToDisplayValue");
        int int32 = Convert.ToInt32(value);
        this.EnsurePopulatedDictionaries();
        TestFailureType testFailureType;
        return this.ArtifactById.TryGetValue(int32, out testFailureType) ? testFailureType.Name : string.Empty;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestFailureTypeQueryField.ConvertRawValueToDisplayValue");
      }
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestFailureTypeQueryField.ConvertDisplayValueToRawValue");
        if (!string.IsNullOrEmpty(displayValue))
        {
          this.EnsurePopulatedDictionaries();
          TestFailureType testFailureType;
          if (this.ArtifactByName != null && this.ArtifactByName.TryGetValue(displayValue, out testFailureType))
            return (object) testFailureType.Id;
        }
        return (object) (int) byte.MaxValue;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestFailureTypeQueryField.ConvertDisplayValueToRawValue");
      }
    }
  }
}
