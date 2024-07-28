// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlansLibraryQuery
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [GenerateAllConstants(null)]
  public enum TestPlansLibraryQuery
  {
    None = 0,
    AllTestCases = 1,
    TestCasesWithActiveBugs = 2,
    TestCasesNotLinkedToRequirements = 3,
    TestCasesLinkedToRequirements = 4,
    AllSharedSteps = 11, // 0x0000000B
    SharedStepsNotLinkedToRequirement = 12, // 0x0000000C
  }
}
