// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPoint
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
  public class TestPoint
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef Tester { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestConfigurationReference Configuration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Comment { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsAutomated { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TeamProjectReference Project { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestPlanReference TestPlan { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestSuiteReference TestSuite { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public IdentityRef LastUpdatedBy { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestPointResults Results { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastResetToActive { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public bool IsActive { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public ReferenceLinks links { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestCaseReference testCaseReference { get; set; }
  }
}
