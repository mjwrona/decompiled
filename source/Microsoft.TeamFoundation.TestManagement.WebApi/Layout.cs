// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Layout
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class Layout
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Type { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid Uid { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime StartTimeUTC { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime EndTimeUTC { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public List<Layout> Children { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Dictionary<string, string> Properties { get; set; }
  }
}
