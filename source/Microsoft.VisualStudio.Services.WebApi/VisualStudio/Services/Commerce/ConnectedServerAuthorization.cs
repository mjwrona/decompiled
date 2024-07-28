// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ConnectedServerAuthorization
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DataContract]
  public sealed class ConnectedServerAuthorization
  {
    public ConnectedServerAuthorization()
    {
    }

    private ConnectedServerAuthorization(ConnectedServerAuthorization objectToBeCloned)
    {
      this.AuthorizationUrl = objectToBeCloned.AuthorizationUrl;
      this.ClientId = objectToBeCloned.ClientId;
      this.PublicKey = objectToBeCloned.PublicKey;
    }

    [DataMember(EmitDefaultValue = false)]
    public Uri AuthorizationUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ClientId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string PublicKey { get; set; }

    public ConnectedServerAuthorization Clone() => new ConnectedServerAuthorization(this);
  }
}
