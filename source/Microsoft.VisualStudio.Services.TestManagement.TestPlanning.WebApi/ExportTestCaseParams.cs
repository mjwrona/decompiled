// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.ExportTestCaseParams
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class ExportTestCaseParams
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int testPlanId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int testSuiteId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public List<int> testCaseIds { get; set; }
  }
}
