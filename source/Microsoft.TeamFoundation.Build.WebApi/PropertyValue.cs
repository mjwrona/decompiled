// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.PropertyValue
// Assembly: Microsoft.TeamFoundation.Build.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 97B7A530-2EF1-42C1-8A2A-360BCF05C7EF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
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
