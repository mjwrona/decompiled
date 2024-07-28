// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.TestIdentityProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal sealed class TestIdentityProvider : 
    NonSyncableIdentityProvider,
    IIdentityProviderExtension,
    IIdentityProvider
  {
    protected override string[] SupportedIdentityTypes() => new string[1]
    {
      "Microsoft.VisualStudio.Services.Identity.ServerTestIdentity"
    };

    protected override IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return new IdentityDescriptor("Microsoft.VisualStudio.Services.Identity.ServerTestIdentity", identity.Name);
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = new Microsoft.VisualStudio.Services.Identity.Identity();
      identity1.Descriptor = this.CreateDescriptor(requestContext, identity);
      identity1.ProviderDisplayName = identity.Name;
      identity1.IsActive = true;
      identity1.UniqueUserId = 0;
      identity1.IsContainer = false;
      identity1.Members = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      identity1.MemberOf = (ICollection<IdentityDescriptor>) Array.Empty<IdentityDescriptor>();
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
      identity2.SetProperty("Domain", (object) "TESTDOMAIN");
      identity2.SetProperty("Account", (object) identity2.DisplayName);
      identity2.SetProperty("Mail", (object) identity2.DisplayName);
      return identity2;
    }
  }
}
