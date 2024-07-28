// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.FrameworkTokenSigningKeyLifecycleService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Partitioning.Server;
using Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.TokenSigningKeyLifecycle
{
  public sealed class FrameworkTokenSigningKeyLifecycleService : TokenSigningKeyLifecycleServiceBase
  {
    private static readonly string EnableDefaultCallToSPSsu1 = "VisualStudio.Services.TokenSigningKeyLifecycleService.EnableDefaultCallToSPSsu1";

    protected internal override TokenSigningKey HandleStrongBoxMiss(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName,
      int keyId,
      bool createDrawer)
    {
      requestContext.TraceEnter(10050300, "SigningKeyLifecycle", "Service", nameof (HandleStrongBoxMiss));
      try
      {
        TokenSigningKey tokenSigningKey = this.GetHttpClient(requestContext).GetSigningKeys(signingKeyNamespaceName, keyId).SyncResult<TokenSigningKey>();
        if (tokenSigningKey == null || string.IsNullOrEmpty(tokenSigningKey.KeyData))
          return (TokenSigningKey) null;
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        Guid empty = Guid.Empty;
        Guid drawerId = !createDrawer ? service.UnlockDrawer(vssRequestContext, this.GetDrawerName(requestContext, signingKeyNamespaceName), true) : service.CreateDrawer(vssRequestContext, this.GetDrawerName(requestContext, signingKeyNamespaceName));
        service.AddString(vssRequestContext, drawerId, keyId.ToString(), tokenSigningKey.KeyData);
        return tokenSigningKey;
      }
      finally
      {
        requestContext.TraceLeave(10050309, "SigningKeyLifecycle", "Service", nameof (HandleStrongBoxMiss));
      }
    }

    protected internal override TokenSigningKeyNamespace RetrieveNamespace(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName)
    {
      requestContext.TraceEnter(10050310, "SigningKeyLifecycle", "Service", nameof (RetrieveNamespace));
      try
      {
        return this.GetHttpClient(requestContext).GetNamespace(signingKeyNamespaceName).SyncResult<TokenSigningKeyNamespace>();
      }
      finally
      {
        requestContext.TraceLeave(10050319, "SigningKeyLifecycle", "Service", nameof (RetrieveNamespace));
      }
    }

    protected override TimeSpan ExpirationInterval { get; } = TimeSpan.FromHours(1.0);

    private TokenSigningKeyHttpClient GetHttpClient(IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.IsFeatureEnabled(FrameworkTokenSigningKeyLifecycleService.EnableDefaultCallToSPSsu1))
      {
        Guid guid = requestContext.RootContext.ServiceInstanceType();
        requestContext.Trace(15190910, TraceLevel.Verbose, "SigningKeyLifecycle", "Service", "GetHttpClient: Service instance type={0}", (object) guid);
        if (guid != ServiceInstanceTypes.SPS)
        {
          TokenSigningKeyHttpClient targetingPrimarySps = SpsToUserMigrationUtil.GetClientTargetingPrimarySps<TokenSigningKeyHttpClient>(requestContext, nameof (GetHttpClient));
          if (targetingPrimarySps != null)
          {
            requestContext.Trace(532164, TraceLevel.Info, "SigningKeyLifecycle", "Service", "Sending request to SPS SU1");
            return targetingPrimarySps;
          }
          requestContext.Trace(552487, TraceLevel.Info, "SigningKeyLifecycle", "Service", "GetClientTargetingPrimarySps returned null. HostId: {0}", (object) requestContext.ServiceHost.InstanceId);
        }
      }
      return !requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? PartitionedClientHelper.GetSpsClientForHostId<TokenSigningKeyHttpClient>(requestContext.Elevate(), requestContext.RootContext.ServiceHost.InstanceId) : requestContext.Elevate().GetClient<TokenSigningKeyHttpClient>();
    }

    protected override string GetDrawerName(
      IVssRequestContext requestContext,
      string signingKeyNamespaceName)
    {
      return base.GetDrawerName(requestContext, signingKeyNamespaceName) + ":" + SpsIdentifier.GetSpsIdentifierForHostContext(requestContext).ToString();
    }
  }
}
