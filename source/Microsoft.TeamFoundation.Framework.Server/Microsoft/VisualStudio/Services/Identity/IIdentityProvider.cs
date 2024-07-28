// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IIdentityProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public interface IIdentityProvider
  {
    string[] SupportedIdentityTypes();

    IdentityDescriptor CreateDescriptor(IVssRequestContext requestContext, IIdentity identity);

    IdentityDescriptor CreateDescriptor(IVssRequestContext requestContext, string displayName);

    Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity);

    bool TrySyncIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool includeMembership,
      string providerInfo,
      SyncErrors syncErrors,
      out Microsoft.VisualStudio.Services.Identity.Identity identity);

    bool IsSyncable { get; }

    void SyncMembers(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      IIdentitySyncHelper syncHelper,
      IDictionary<string, IIdentityProvider> syncAgents,
      string providerInfo,
      SyncErrors syncErrors);

    IEnumerable<string> AvailableIdentityAttributes { get; }
  }
}
