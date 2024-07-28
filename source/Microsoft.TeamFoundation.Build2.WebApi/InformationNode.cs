// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.InformationNode
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class InformationNode
  {
    public InformationNode() => this.Fields = new Dictionary<string, string>();

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int NodeId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int ParentId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastModifiedDate { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LastModifiedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Dictionary<string, string> Fields { get; set; }
  }
}
