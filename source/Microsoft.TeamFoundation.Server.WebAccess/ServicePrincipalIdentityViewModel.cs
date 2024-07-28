// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ServicePrincipalIdentityViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Server.Core;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ServicePrincipalIdentityViewModel : UserIdentityViewModelBase
  {
    public ServicePrincipalIdentityViewModel()
    {
    }

    public ServicePrincipalIdentityViewModel(
      TeamFoundationIdentity identity,
      bool loadAadTenantData = false)
      : base(identity, loadAadTenantData)
    {
      this.ApplicationId = identity.GetProperty<string>(nameof (ApplicationId), string.Empty);
    }

    public override string SubHeader => this.ApplicationId;

    public string ApplicationId { get; private set; }

    public override string IdentityType => "servicePrincipal";

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["ApplicationId"] = (object) this.ApplicationId;
      return json;
    }
  }
}
