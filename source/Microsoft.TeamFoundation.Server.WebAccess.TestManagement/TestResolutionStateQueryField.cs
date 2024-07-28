// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResolutionStateQueryField
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestResolutionStateQueryField : 
    TestArtifactQueryField<TestCaseResult, TestResolutionState>
  {
    public TestResolutionStateQueryField(TestManagerRequestContext testContext)
      : base(testContext, "ResolutionStateId", TestManagementServerResources.QueryColumnNameResolutionState, true, (Func<TestCaseResult, int>) (res => res.ResolutionStateId))
    {
    }

    protected override IEnumerable<TestResolutionState> GetArtifacts()
    {
      List<TestResolutionState> artifacts = TestResolutionState.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, 0, this.TestContext.ProjectName);
      if (artifacts != null && !artifacts.Exists((Predicate<TestResolutionState>) (s => s.Id == 0)))
        artifacts.Insert(0, new TestResolutionState()
        {
          Id = 0,
          Name = TestManagementResources.ResolutionStateNone
        });
      return (IEnumerable<TestResolutionState>) artifacts;
    }

    protected override string GetArtifactName(TestResolutionState artifact) => artifact.Name;

    protected override int GetArtifactId(TestResolutionState artifact) => artifact.Id;

    public override string ConvertRawValueToDisplayValue(object value)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestResolutionStateQueryField.ConvertRawValueToDisplayValue");
        int int32 = Convert.ToInt32(value);
        this.EnsurePopulatedDictionaries();
        TestResolutionState testResolutionState;
        return this.ArtifactById.TryGetValue(int32, out testResolutionState) ? testResolutionState.Name : string.Empty;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestResolutionStateQueryField.ConvertRawValueToDisplayValue");
      }
    }

    public override object ConvertDisplayValueToRawValue(string displayValue)
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestResolutionStateQueryField.ConvertDisplayValueToRawValue");
        if (!string.IsNullOrEmpty(displayValue))
        {
          this.EnsurePopulatedDictionaries();
          TestResolutionState testResolutionState;
          if (this.ArtifactByName != null && this.ArtifactByName.TryGetValue(displayValue, out testResolutionState))
            return (object) testResolutionState.Id;
        }
        return (object) -1;
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestResolutionStateQueryField.ConvertDisplayValueToRawValue");
      }
    }
  }
}
