// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WorkspaceMapping
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public sealed class WorkspaceMapping
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ServerItem { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LocalItem { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public WorkspaceMappingType MappingType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int Depth { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal string DefinitionUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    internal int WorkspaceId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[WorkspaceMapping ServerItem={0} LocalItem={1} MappingType={2} Depth={3}]", (object) this.ServerItem, (object) this.LocalItem, (object) this.MappingType, (object) this.Depth);
  }
}
