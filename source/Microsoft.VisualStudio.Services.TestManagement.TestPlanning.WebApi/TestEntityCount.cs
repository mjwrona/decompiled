// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestEntityCount
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  public class TestEntityCount
  {
    public int Count { get; set; }

    public int TotalCount { get; set; }

    public int TestPlanId { get; set; }

    public int TestSuiteId { get; set; }
  }
}
