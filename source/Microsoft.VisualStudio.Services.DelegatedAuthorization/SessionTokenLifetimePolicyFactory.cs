// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionTokenLifetimePolicyFactory
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class SessionTokenLifetimePolicyFactory
  {
    private static readonly Lazy<SessionTokenLifetimePolicyFactory> instance = new Lazy<SessionTokenLifetimePolicyFactory>();

    public static SessionTokenLifetimePolicyFactory Instance => SessionTokenLifetimePolicyFactory.instance.Value;

    public SessionTokenLifetimePolicy Create(
      IVssRequestContext requestContext,
      DelegatedAuthorizationSettings settings,
      Guid targetIdentityId,
      DateTime? validTo = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<DelegatedAuthorizationSettings>(settings, nameof (settings));
      ArgumentUtility.CheckForEmptyGuid(targetIdentityId, nameof (targetIdentityId));
      Guid authenticatedUserIdentityId = requestContext.GetAuthenticatedIdentity().StorageKey(requestContext, TeamFoundationHostType.Deployment);
      return new SessionTokenLifetimePolicy(settings, targetIdentityId, authenticatedUserIdentityId, validTo)
      {
        TraceMethod = (TraceMethod) ((tracepoint, level, area, layer, format, args) => VssRequestContextExtensions.Trace(requestContext, tracepoint, level, area, layer, format, args))
      };
    }
  }
}
