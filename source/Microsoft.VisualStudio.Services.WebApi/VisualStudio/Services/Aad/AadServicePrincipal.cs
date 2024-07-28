// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServicePrincipal
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DataContract]
  public class AadServicePrincipal : AadObject
  {
    [DataMember]
    public Guid AppId { get; }

    [DataMember]
    public bool AccountEnabled { get; }

    [DataMember]
    public string ServicePrincipalType { get; }

    protected AadServicePrincipal()
    {
    }

    private AadServicePrincipal(
      Guid objectId,
      string displayName,
      Guid appId,
      bool accountEnabled,
      string servicePrincipalType)
      : base(objectId, displayName)
    {
      this.AppId = appId;
      this.AccountEnabled = accountEnabled;
      this.ServicePrincipalType = servicePrincipalType;
    }

    public class Factory
    {
      public AadServicePrincipal Create() => new AadServicePrincipal(this.ObjectId, this.DisplayName, this.AppId, this.AccountEnabled, this.ServicePrincipalType);

      public Guid ObjectId { get; set; }

      public string DisplayName { get; set; }

      public Guid AppId { get; set; }

      public bool AccountEnabled { get; set; }

      public string ServicePrincipalType { get; set; }
    }
  }
}
