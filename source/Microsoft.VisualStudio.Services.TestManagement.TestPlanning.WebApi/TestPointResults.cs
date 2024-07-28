// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.TestPointResults
// Assembly: Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6FBA62B7-DF7C-48A4-98F0-AF0ACAEA014F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi
{
  [DataContract]
  public class TestPointResults
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public LastResultDetails LastResultDetails { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int LastResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LastRunBuildNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public PointState State { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ResultState LastResultState { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = true)]
    public Outcome Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public FailureType FailureType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public LastResolutionState LastResolutionState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int LastTestRunId { get; set; }
  }
}
