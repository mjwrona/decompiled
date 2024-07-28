// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.UnauthenticatedProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public sealed class UnauthenticatedProvider : NonSyncableIdentityProvider
  {
    private static readonly string[] s_supportedIdentityTypes = new string[1]
    {
      "Microsoft.TeamFoundation.UnauthenticatedIdentity"
    };

    protected override string[] SupportedIdentityTypes() => UnauthenticatedProvider.s_supportedIdentityTypes;

    protected override IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      throw new NotImplementedException();
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }
  }
}
