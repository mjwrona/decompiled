// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.Proxy
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class Proxy
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Guid ProxyId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FriendlyName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Site { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? SiteDefault { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool? GlobalDefault { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ProxyAuthorization Authorization { get; set; }
  }
}
