// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi.Legacy
{
  [DataContract]
  public sealed class TestActionResult
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public LegacyTestCaseResultIdentifier Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int IterationId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ActionPath { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreationDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid LastUpdatedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte Outcome { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string ErrorMessage { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime DateStarted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime DateCompleted { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public long Duration { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int SharedStepId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int SharedStepRevision { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string Comment { get; set; }
  }
}
