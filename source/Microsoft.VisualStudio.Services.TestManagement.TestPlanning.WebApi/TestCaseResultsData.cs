// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestCaseResultsData
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
  public class TestCaseResultsData
  {
    [DataMember(IsRequired = true, Name = "Results")]
    public IList<TestCaseAssociatedResult> Results { get; set; }

    [DataMember(IsRequired = true, Name = "TestCaseName")]
    public string TestCaseName { get; set; }

    [DataMember(IsRequired = false, Name = "ContextPoint")]
    public TestPointDetailedReference ContextPoint { get; set; }
  }
}
