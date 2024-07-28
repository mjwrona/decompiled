// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlansHubRefreshData
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  [ClientIncludeModel]
  public class TestPlansHubRefreshData
  {
    [DataMember(IsRequired = true, Name = "testPlan")]
    public TestPlanDetailedReference TestPlan { get; set; }

    [DataMember(IsRequired = true, Name = "testSuites")]
    public List<TestSuite> TestSuites { get; set; }

    [DataMember(IsRequired = true, Name = "testCasePageSize", EmitDefaultValue = false)]
    public int TestCasePageSize { get; set; }

    [DataMember(IsRequired = true, Name = "testPointPageSize", EmitDefaultValue = false)]
    public int TestPointPageSize { get; set; }

    [DataMember(IsRequired = false, Name = "testSuitesContinuationToken", EmitDefaultValue = false)]
    public string TestSuitesContinuationToken { get; set; }

    [DataMember(IsRequired = false, Name = "testCases", EmitDefaultValue = false)]
    public List<TestCase> TestCases { get; set; }

    [DataMember(IsRequired = false, Name = "testCasesContinuationToken", EmitDefaultValue = false)]
    public string TestCasesContinuationToken { get; set; }

    [DataMember(IsRequired = false, Name = "testPoints", EmitDefaultValue = false)]
    public List<TestPoint> TestPoints { get; set; }

    [DataMember(IsRequired = false, Name = "testPointsContinuationToken", EmitDefaultValue = false)]
    public string TestPointsContinuationToken { get; set; }

    [DataMember(IsRequired = false, Name = "selectedSuiteId", EmitDefaultValue = false)]
    public int SelectedSuiteId { get; set; }

    [DataMember(IsRequired = false, Name = "isAdvancedExtensionEnabled", EmitDefaultValue = false)]
    public bool IsAdvancedExtensionEnabled { get; set; }

    [DataMember(IsRequired = false, Name = "ExecuteColumnOptionFields", EmitDefaultValue = false)]
    public string[] ExecuteColumnOptionFields { get; set; }

    [DataMember(IsRequired = false, Name = "DefineColumnOptionFields", EmitDefaultValue = false)]
    public string[] DefineColumnOptionFields { get; set; }

    [DataMember(IsRequired = false, Name = "DefineTabCustomColumnFieldMap", EmitDefaultValue = false)]
    public Dictionary<string, string> DefineTabCustomColumnFieldMap { get; set; }

    [DataMember(IsRequired = false, Name = "ExecuteTabCustomColumnFieldMap", EmitDefaultValue = false)]
    public Dictionary<string, string> ExecuteTabCustomColumnFieldMap { get; set; }

    [DataMember(IsRequired = false, Name = "errorMessage", EmitDefaultValue = false)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, Name = "selectedPivotId", EmitDefaultValue = false)]
    public string SelectedPivotId { get; set; }
  }
}
