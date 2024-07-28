// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphFederatedProviderServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal abstract class GraphFederatedProviderServiceBase : 
    IGraphFederatedProviderService,
    IVssFrameworkService
  {
    protected const string Area = "Graph";
    private const string Layer = "GraphFederatedProviderServiceBase";

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    GraphFederatedProviderData IGraphFederatedProviderService.AcquireProviderData(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName,
      long versionHint)
    {
      context.TraceEnter(60330220, "Graph", nameof (GraphFederatedProviderServiceBase), "AcquireProviderData");
      try
      {
        context.TraceDataConditionally(60330221, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Retrieving runtime provider data", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName
        }), "AcquireProviderData");
        GraphFederatedProviderServiceBase.ValidateInput(context, descriptor, providerName);
        GraphFederatedProviderData providerData = this.AcquireProviderDataFromCache(context, descriptor, providerName, versionHint, new Func<IVssRequestContext, SubjectDescriptor, string, long, GraphFederatedProviderData>(this.AcquireProviderDataFromRemote));
        context.TraceDataConditionally(60330228, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Retrieved runtime provider data", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          providerData = providerData.Hashed(context)
        }), "AcquireProviderData");
        return providerData;
      }
      finally
      {
        context.TraceLeave(60330229, "Graph", nameof (GraphFederatedProviderServiceBase), "AcquireProviderData");
      }
    }

    protected static void ValidateInput(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName)
    {
      GraphValidation.CheckDescriptor(descriptor, nameof (descriptor));
      if (!descriptor.IsFederatableUserType())
        throw new ArgumentException(string.Format("Descriptor '{0}' is of a subject type that cannot be federated with an external identity provider", (object) descriptor), nameof (descriptor));
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      GraphFederatedProviderPermissions.CheckPermission(context, providerName, descriptor.ToIdentityDescriptor(context));
    }

    private static IVssRequestContext GetTargetContext(IVssRequestContext context) => context.To(TeamFoundationHostType.Deployment);

    protected GraphFederatedProviderData AcquireProviderDataFromCache(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName,
      long versionHint,
      Func<IVssRequestContext, SubjectDescriptor, string, long, GraphFederatedProviderData> onCacheMiss,
      Func<GraphFederatedProviderData, bool> isCachedDataValid = null)
    {
      context.TraceEnter(60330230, "Graph", nameof (GraphFederatedProviderServiceBase), nameof (AcquireProviderDataFromCache));
      try
      {
        IVssRequestContext targetContext = GraphFederatedProviderServiceBase.GetTargetContext(context);
        IGraphFederatedProviderCache cache = targetContext.GetService<IGraphFederatedProviderCache>();
        GraphFederatedProviderData cachedProviderData = cache.GetProviderData(context, descriptor, providerName);
        if (cachedProviderData == null)
          context.TraceDataConditionally(60330231, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Did not retrieve runtime provider data from cache; falling back to remote", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            providerName = providerName,
            versionHint = versionHint
          }), nameof (AcquireProviderDataFromCache));
        else if (cachedProviderData.Version < versionHint)
        {
          context.TraceDataConditionally(60330232, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Retrieved runtime provider data from cache, but data version is older than the hinted version; falling back to remote", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            providerName = providerName,
            versionHint = versionHint,
            providerData = cachedProviderData.Hashed(context)
          }), nameof (AcquireProviderDataFromCache));
          context.TraceAlways(60330238, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Called with a version bumped, refreshing the access token {0}.", (object) cachedProviderData.Hashed(context).AccessTokenHash);
        }
        else if (isCachedDataValid != null && !isCachedDataValid(cachedProviderData))
        {
          context.TraceDataConditionally(60330233, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Retrieved runtime provider data from cache, but data is not valid; falling back to remote", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            providerName = providerName,
            versionHint = versionHint,
            providerData = cachedProviderData.Hashed(context)
          }), nameof (AcquireProviderDataFromCache));
        }
        else
        {
          context.TraceDataConditionally(60330234, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Retrieved valid, up-to-date runtime provider data from cache; returning from cache", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            providerName = providerName,
            versionHint = versionHint,
            providerData = cachedProviderData.Hashed(context)
          }), nameof (AcquireProviderDataFromCache));
          return cachedProviderData;
        }
        long versionHintForRemoteCall = Math.Max(versionHint, cache.MaxChangeVersion);
        context.TraceDataConditionally(60330235, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Calling remote", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          callerVersionHint = versionHint,
          cacheVersionHint = cache.MaxChangeVersion,
          versionHintForRemoteCall = versionHintForRemoteCall,
          providerData = cachedProviderData.Hashed(context)
        }), nameof (AcquireProviderDataFromCache));
        GraphFederatedProviderData remoteProviderData = onCacheMiss(targetContext, descriptor, providerName, versionHintForRemoteCall);
        if (remoteProviderData == null)
        {
          context.TraceDataConditionally(60330236, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Did not retrieve runtime provider data from remote; returning null", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            providerName = providerName,
            versionHint = versionHint
          }), nameof (AcquireProviderDataFromCache));
          return (GraphFederatedProviderData) null;
        }
        if (string.IsNullOrEmpty(remoteProviderData.AccessToken))
        {
          context.TraceDataConditionally(60330237, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Did not retrieve access token for runtime provider data from remote; returning null", (Func<object>) (() => (object) new
          {
            descriptor = descriptor,
            providerName = providerName,
            versionHint = versionHint
          }), nameof (AcquireProviderDataFromCache));
          return (GraphFederatedProviderData) null;
        }
        context.TraceDataConditionally(60330267, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Retrieved runtime provider data from remote; updating cache", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          versionHint = versionHint,
          providerData = remoteProviderData.Hashed(context)
        }), nameof (AcquireProviderDataFromCache));
        context.TraceAlways(60330239, TraceLevel.Verbose, "Graph", nameof (GraphFederatedProviderServiceBase), "Retrieved runtime access token {0}.", (object) remoteProviderData.Hashed(context).AccessTokenHash);
        cache.SetProviderData(targetContext, descriptor, providerName, remoteProviderData);
        return remoteProviderData;
      }
      finally
      {
        context.TraceLeave(60330230, "Graph", nameof (GraphFederatedProviderServiceBase), nameof (AcquireProviderDataFromCache));
      }
    }

    protected abstract GraphFederatedProviderData AcquireProviderDataFromRemote(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName,
      long versionHint);
  }
}
