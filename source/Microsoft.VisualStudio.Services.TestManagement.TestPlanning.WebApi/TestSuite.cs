// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestSuite
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class TestSuite : TestSuiteCreateParams
  {
    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public int Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Revision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public TeamProjectReference Project { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastPopulatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IdentityRef LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DefaultValue(typeof (DateTime), "1-1-0001")]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public TestPlanReference Plan { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<TestSuite> Children { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool HasChildren { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LastError { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }
  }
}
