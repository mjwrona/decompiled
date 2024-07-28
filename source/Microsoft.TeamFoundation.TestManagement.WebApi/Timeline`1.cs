// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.Timeline`1
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class Timeline<T>
  {
    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public T Type { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    public string Display { get; set; }

    [DataMember(IsRequired = true, EmitDefaultValue = false)]
    [DefaultValue(typeof (DateTime), "0001-01-01T00:00:00Z")]
    public DateTime TimestampUTC { get; set; }
  }
}
