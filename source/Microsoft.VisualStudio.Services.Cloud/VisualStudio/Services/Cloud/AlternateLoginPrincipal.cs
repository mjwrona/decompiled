// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlternateLoginPrincipal
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public sealed class AlternateLoginPrincipal : ClaimsPrincipal
  {
    private readonly AlternateLoginIdentity m_identity;

    public AlternateLoginPrincipal(string authenticationType, Microsoft.VisualStudio.Services.Identity.Identity identity)
      : base((IEnumerable<ClaimsIdentity>) new ClaimsIdentity[1]
      {
        ClaimsProvider.GetIdentity(identity, authenticationType)
      })
    {
      this.m_identity = new AlternateLoginIdentity(authenticationType, identity);
    }

    private AlternateLoginPrincipal(AlternateLoginPrincipal principalToBeCloned)
      : base((IEnumerable<ClaimsIdentity>) principalToBeCloned.Identities.Select<ClaimsIdentity, ClaimsIdentity>((Func<ClaimsIdentity, ClaimsIdentity>) (x => x.Clone())).ToArray<ClaimsIdentity>())
    {
      this.m_identity = new AlternateLoginIdentity(principalToBeCloned.Identity.AuthenticationType, ((AlternateLoginIdentity) principalToBeCloned.Identity).Identity);
    }

    public override IIdentity Identity => (IIdentity) this.m_identity;

    public override bool IsInRole(string role) => false;
  }
}
