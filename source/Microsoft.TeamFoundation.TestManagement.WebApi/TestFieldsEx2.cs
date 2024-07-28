// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestFieldsEx2
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public sealed class TestFieldsEx2
  {
    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public Guid ProjectId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public int FieldId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public string FieldName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public byte FieldType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsRunScoped { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsResultScoped { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsSystemField { get; set; }
  }
}
