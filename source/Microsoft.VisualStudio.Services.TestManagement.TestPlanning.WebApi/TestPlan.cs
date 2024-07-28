// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPlan
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class TestPlan : TestPlanUpdateParams
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TeamProjectReference Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime UpdatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef UpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int PreviousBuildId { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestSuiteReference RootSuite { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }
  }
}
