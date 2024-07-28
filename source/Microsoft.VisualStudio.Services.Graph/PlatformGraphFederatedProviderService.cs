// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.PlatformGraphFederatedProviderService
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.MSGraph;
using Microsoft.VisualStudio.Services.MSGraph.Client;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Graph
{
  internal class PlatformGraphFederatedProviderService : GraphFederatedProviderServiceBase
  {
    private const string Layer = "PlatformGraphFederatedProviderService";

    protected override GraphFederatedProviderData AcquireProviderDataFromRemote(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName,
      long versionHint)
    {
      context.TraceEnter(60330320, "Graph", nameof (PlatformGraphFederatedProviderService), nameof (AcquireProviderDataFromRemote));
      try
      {
        context.TraceDataConditionally(60330321, TraceLevel.Verbose, "Graph", nameof (PlatformGraphFederatedProviderService), "Retrieving runtime provider data", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName
        }), nameof (AcquireProviderDataFromRemote));
        context.CheckOrganizationRequestContext();
        PlatformGraphFederatedProviderService.GetStorageKey(context, descriptor);
        string providerAccessToken = this.GetProviderAccessToken(context, descriptor, providerName);
        GraphFederatedProviderData runtimeProviderData = new GraphFederatedProviderData(descriptor, providerName, providerAccessToken, -1L);
        context.TraceDataConditionally(60330328, TraceLevel.Verbose, "Graph", nameof (PlatformGraphFederatedProviderService), "Return corresponding runtime provider data", (Func<object>) (() => (object) new
        {
          descriptor = descriptor,
          providerName = providerName,
          runtimeProviderData = runtimeProviderData.Hashed(context)
        }), nameof (AcquireProviderDataFromRemote));
        return runtimeProviderData;
      }
      finally
      {
        context.TraceLeave(60330329, "Graph", nameof (PlatformGraphFederatedProviderService), nameof (AcquireProviderDataFromRemote));
      }
    }

    private static Guid GetStorageKey(
      IVssRequestContext context,
      SubjectDescriptor subjectDescriptor)
    {
      return context.GetService<IGraphIdentifierConversionService>().GetStorageKeyByDescriptor(context, subjectDescriptor);
    }

    private string GetProviderAccessToken(
      IVssRequestContext context,
      SubjectDescriptor descriptor,
      string providerName)
    {
      try
      {
        IdentityDescriptor identityDescriptor = descriptor.ToIdentityDescriptor(context);
        IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
        IdentityProviderAccessTokenResult accessTokenResult = context.GetService<IPlatformMicrosoftGraphService>().GetIdentityProviderAccessToken(vssRequestContext, providerName, identityDescriptor);
        context.TraceDataConditionally(60330331, TraceLevel.Verbose, "Graph", nameof (PlatformGraphFederatedProviderService), "Retrieved access token", (Func<object>) (() => (object) new
        {
          accessTokenResult = accessTokenResult
        }), nameof (GetProviderAccessToken));
        return accessTokenResult?.Value;
      }
      catch (MicrosoftGraphServiceException ex)
      {
        context.TraceException(60330336, "Graph", nameof (PlatformGraphFederatedProviderService), (Exception) ex);
        return (string) null;
      }
    }
  }
}
