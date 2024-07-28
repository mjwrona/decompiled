// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.IdentityExtensions
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class IdentityExtensions
  {
    private const string Area = "Licensing";

    public static Guid EnterpriseStorageKey(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      if (identity == null)
      {
        requestContext.TraceDataConditionally(1030271, TraceLevel.Verbose, "Licensing", nameof (EnterpriseStorageKey), "Null identity gets empty storage key", methodName: nameof (EnterpriseStorageKey));
        return Guid.Empty;
      }
      IVssRequestContext vssRequestContext = !requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.To(TeamFoundationHostType.Application) : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      Guid storageKey = vssRequestContext.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(vssRequestContext, identity.SubjectDescriptor);
      bool flag = true;
      if (storageKey == Guid.Empty)
      {
        requestContext.TraceDataConditionally(1030273, TraceLevel.Error, "Licensing", nameof (EnterpriseStorageKey), "Empty storage key from conversion service", (Func<object>) (() => (object) new
        {
          identity = identity
        }), nameof (EnterpriseStorageKey));
        flag = false;
      }
      if (!IdentityIdChecker.IsStorageKey(storageKey))
      {
        requestContext.TraceDataConditionally(1030274, TraceLevel.Error, "Licensing", nameof (EnterpriseStorageKey), "Invalid storage key from conversion service", (Func<object>) (() => (object) new
        {
          storageKey = storageKey,
          identity = identity
        }), nameof (EnterpriseStorageKey));
        flag = false;
      }
      if (flag)
      {
        requestContext.TraceDataConditionally(1030275, TraceLevel.Verbose, "Licensing", nameof (EnterpriseStorageKey), "Returning valid storage key from conversion service", (Func<object>) (() => (object) new
        {
          storageKey = storageKey,
          identity = identity
        }), nameof (EnterpriseStorageKey));
        return storageKey;
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        identity.Descriptor
      }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity1 != null)
        return identity1.Id;
      requestContext.TraceDataConditionally(1030276, TraceLevel.Verbose, "Licensing", nameof (EnterpriseStorageKey), "Attempting member materialization for licensing", (Func<object>) (() => (object) new
      {
        identity = identity
      }), nameof (EnterpriseStorageKey));
      storageKey = IdentityHelper.MaterializeUser(requestContext, (IVssIdentity) identity, nameof (EnterpriseStorageKey));
      requestContext.TraceDataConditionally(1030277, TraceLevel.Verbose, "Licensing", nameof (EnterpriseStorageKey), "Returning valid storage key from member materialization", (Func<object>) (() => (object) new
      {
        storageKey = storageKey,
        identity = identity
      }), nameof (EnterpriseStorageKey));
      return storageKey;
    }

    public static LicensedIdentity ToLicensedIdentity(this Microsoft.VisualStudio.Services.Identity.Identity identity) => identity == null ? (LicensedIdentity) null : new LicensedIdentity(identity.DisplayName, identity.GetProperty<string>("Mail", (string) null), identity.MetaType);
  }
}
