// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DstsHelper.AlternateLoginProvider
// Assembly: Microsoft.TeamFoundation.DstsHelper, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08D47267-3A15-4307-BBA0-1792E9C6BDF1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DstsHelper.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System.Security.Principal;

namespace Microsoft.TeamFoundation.DstsHelper
{
  internal sealed class AlternateLoginProvider : 
    NonSyncableIdentityProvider,
    IIdentityProviderExtension,
    IIdentityProvider
  {
    protected override string[] SupportedIdentityTypes() => new string[1]
    {
      typeof (AlternateLoginIdentity).FullName
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
