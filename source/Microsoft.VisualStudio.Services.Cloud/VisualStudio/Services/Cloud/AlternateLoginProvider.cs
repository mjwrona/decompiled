// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlternateLoginProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal sealed class AlternateLoginProvider : 
    NonSyncableIdentityProvider,
    IIdentityProviderExtension,
    IIdentityProvider
  {
    protected override string[] SupportedIdentityTypes() => new string[1]
    {
      "Microsoft.VisualStudio.Services.Cloud.AlternateLoginIdentity"
    };

    protected override IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return identity is AlternateLoginIdentity alternateLoginIdentity ? alternateLoginIdentity.Identity.Descriptor : (IdentityDescriptor) null;
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return identity is AlternateLoginIdentity alternateLoginIdentity ? alternateLoginIdentity.Identity : (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }
  }
}
