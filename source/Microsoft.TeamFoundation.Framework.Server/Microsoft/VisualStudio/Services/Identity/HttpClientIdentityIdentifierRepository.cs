// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.HttpClientIdentityIdentifierRepository
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class HttpClientIdentityIdentifierRepository : IIdentityIdentifierRepository
  {
    protected const string TraceArea = "IdentityIdentifierConversion";
    protected const string TraceLayer = "HttpClientIdentityIdentifierRepository";

    public HttpClientIdentityIdentifierRepository(TeamFoundationHostType hostType) => this.HostType = hostType;

    public IdentityDescriptor GetDescriptorByMasterId(
      IVssRequestContext requestContext,
      Guid masterId)
    {
      try
      {
        IdentityDescriptor descriptorByMasterId;
        string str;
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        {
          descriptorByMasterId = (this.GetUserIdentityHttpClient(requestContext).ReadIdentity(masterId).SyncResult<Microsoft.VisualStudio.Services.Identity.Identity>() ?? throw new IdentityDescriptorNotFoundException(masterId, true)).Descriptor;
          str = "UserIdentityHttpClient".ToString();
        }
        else
        {
          descriptorByMasterId = this.GetHttpClient(requestContext).GetDescriptorByIdAsync(masterId, new bool?(true), (object) null, new CancellationToken()).SyncResult<IdentityDescriptor>();
          str = "IdentityHttpClient".ToString();
        }
        requestContext.Trace(6307320, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (HttpClientIdentityIdentifierRepository), "Received descriptor by masterId {0} from {1} - Descriptor Type: [{2}] Identifier Hash: [{3}]", (object) masterId, (object) str, (object) descriptorByMasterId?.IdentityType, (object) descriptorByMasterId?.GetHashCode());
        return descriptorByMasterId;
      }
      catch (IdentityDescriptorNotFoundException ex)
      {
        requestContext.Trace(6307322, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (HttpClientIdentityIdentifierRepository), "Descriptor by masterId not found from HTTP client - {0}", (object) masterId);
        requestContext.TraceConditionally(6307332, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (HttpClientIdentityIdentifierRepository), (Func<string>) (() => EnvironmentWrapper.ToReadableStackTrace().ToString()));
        return (IdentityDescriptor) null;
      }
    }

    public IdentityDescriptor GetDescriptorByLocalId(
      IVssRequestContext requestContext,
      Guid localId)
    {
      try
      {
        IdentityDescriptor descriptorByLocalId = this.GetHttpClient(requestContext).GetDescriptorByIdAsync(localId, new bool?(), (object) null, new CancellationToken()).SyncResult<IdentityDescriptor>();
        requestContext.Trace(6307320, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (HttpClientIdentityIdentifierRepository), "Received descriptor by localId {0} from HTTP client - Descriptor Type: [{1}] Identifier Hash: [{2}]", (object) localId, (object) descriptorByLocalId?.IdentityType, (object) descriptorByLocalId?.GetHashCode());
        return descriptorByLocalId;
      }
      catch (IdentityDescriptorNotFoundException ex)
      {
        requestContext.Trace(6307322, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (HttpClientIdentityIdentifierRepository), "Descriptor by localId not found from HTTP client - {0}", (object) localId);
        requestContext.TraceConditionally(6307332, TraceLevel.Verbose, "IdentityIdentifierConversion", nameof (HttpClientIdentityIdentifierRepository), (Func<string>) (() => EnvironmentWrapper.ToReadableStackTrace().ToString()));
        return (IdentityDescriptor) null;
      }
    }

    public void OnDescriptorRetrievedByMasterId(
      IVssRequestContext requestContext,
      Guid masterId,
      IdentityDescriptor identityDescriptor)
    {
    }

    public void OnDescriptorRetrievedByLocalId(
      IVssRequestContext requestContext,
      Guid localId,
      IdentityDescriptor identityDescriptor)
    {
    }

    public void Unload(IVssRequestContext requestContext)
    {
    }

    private IdentityHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.Elevate().GetClient<IdentityHttpClient>();

    private UserIdentityHttpClient GetUserIdentityHttpClient(IVssRequestContext requestContext) => requestContext.Elevate().GetClient<UserIdentityHttpClient>();

    public TeamFoundationHostType HostType { get; }
  }
}
