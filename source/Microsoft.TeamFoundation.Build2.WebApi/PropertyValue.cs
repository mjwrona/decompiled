// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.PropertyValue
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete("This contract is not used by any product code")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class PropertyValue
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PropertyName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public object Value { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid? ChangedBy { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? ChangedDate { get; set; }
  }
}
