// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestRunEx2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestRunEx2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int TestRunId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int FieldId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string FieldName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime CreatedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int? IntValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public double? FloatValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool? BitValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DateTime? DateTimeValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid? GuidValue { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string StringValue { get; set; }
  }
}
