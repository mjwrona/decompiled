// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlanDetailedReference
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class TestPlanDetailedReference : TestPlanReference
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AreaPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Iteration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? StartDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? EndDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int RootSuiteId { get; set; }
  }
}
