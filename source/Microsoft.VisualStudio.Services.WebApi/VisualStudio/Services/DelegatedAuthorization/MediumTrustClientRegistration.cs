// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.MediumTrustClientRegistration
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class MediumTrustClientRegistration : CommonRegistration
  {
    public MediumTrustClientRegistration(Registration registration)
      : base(registration)
    {
      this.PublicKey = registration.PublicKey;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PublicKey { get; set; }

    public override Registration ToRegistration()
    {
      Registration registration = base.ToRegistration();
      registration.PublicKey = this.PublicKey;
      return registration;
    }
  }
}
