// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.NonSyncableIdentityProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Identity
{
  public abstract class NonSyncableIdentityProvider : IIdentityProvider
  {
    internal static readonly string c_area = "IdentityProvider";
    private const string c_layer = "NonSyncableIdentityProvider";

    protected virtual IEnumerable<string> AvailableIdentityAttributes => Enumerable.Empty<string>();

    protected abstract IdentityDescriptor CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity);

    protected abstract Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity);

    protected abstract string[] SupportedIdentityTypes();

    IEnumerable<string> IIdentityProvider.AvailableIdentityAttributes => this.AvailableIdentityAttributes;

    bool IIdentityProvider.IsSyncable => false;

    IdentityDescriptor IIdentityProvider.CreateDescriptor(
      IVssRequestContext requestContext,
      string displayName)
    {
      requestContext.Trace(14000001, TraceLevel.Info, NonSyncableIdentityProvider.c_area, nameof (NonSyncableIdentityProvider), "Display name overload of CreateDescriptor called on NonSyncableIdentityProvider");
      return (IdentityDescriptor) null;
    }

    IdentityDescriptor IIdentityProvider.CreateDescriptor(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return this.CreateDescriptor(requestContext, identity);
    }

    Microsoft.VisualStudio.Services.Identity.Identity IIdentityProvider.GetIdentity(
      IVssRequestContext requestContext,
      IIdentity identity)
    {
      return this.GetIdentity(requestContext, identity);
    }

    string[] IIdentityProvider.SupportedIdentityTypes() => this.SupportedIdentityTypes();

    void IIdentityProvider.SyncMembers(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      IIdentitySyncHelper syncHelper,
      IDictionary<string, IIdentityProvider> syncAgents,
      string providerInfo,
      SyncErrors syncErrors)
    {
      requestContext.Trace(14000002, TraceLevel.Error, NonSyncableIdentityProvider.c_area, nameof (NonSyncableIdentityProvider), "Non-implemented method SyncMembers called on " + this.GetType().FullName + ".");
    }

    bool IIdentityProvider.TrySyncIdentity(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      bool includeMembership,
      string providerInfo,
      SyncErrors syncErrors,
      out Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      requestContext.TraceEnter(14000003, NonSyncableIdentityProvider.c_area, nameof (NonSyncableIdentityProvider), "TrySyncIdentity");
      try
      {
        identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
        return false;
      }
      finally
      {
        requestContext.TraceLeave(14000004, NonSyncableIdentityProvider.c_area, nameof (NonSyncableIdentityProvider), "TrySyncIdentity");
      }
    }
  }
}
