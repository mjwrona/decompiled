// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.PointsResults2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class PointsResults2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int PointId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int PlanId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int ChangeNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int LastTestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int LastTestResultId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte? LastResultState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte? LastResultOutcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int LastResolutionStateId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte? LastFailureType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid LastUpdatedBy { get; set; }
  }
}
