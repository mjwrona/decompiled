// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.CloneTestCaseOperationInformation
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class CloneTestCaseOperationInformation
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public SourceTestSuiteResponse sourceTestSuite { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestSuiteReferenceWithProject destinationTestSuite { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public CloneTestCaseOptions cloneOptions { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public CloneOperationCommonResponse cloneOperationResponse { get; set; }
  }
}
