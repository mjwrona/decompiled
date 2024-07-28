// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProxyAuthorization
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class ProxyAuthorization
  {
    [DataMember(EmitDefaultValue = false)]
    public Uri AuthorizationUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ClientId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PublicKey PublicKey { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityDescriptor Identity { get; set; }
  }
}
