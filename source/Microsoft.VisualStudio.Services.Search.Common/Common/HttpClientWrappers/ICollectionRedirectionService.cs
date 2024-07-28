// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.ICollectionRedirectionService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers
{
  [DefaultServiceImplementation(typeof (TfsCollectionRedirectedService))]
  public interface ICollectionRedirectionService : IVssFrameworkService
  {
    T GetClient<T>(IVssRequestContext requestContext, Guid serviceAreaId) where T : VssHttpClientBase;

    T GetClient<T>(IVssRequestContext requestContext, VssHttpClientOptions httpClientOptions = null) where T : VssHttpClientBase;

    T GetFeedClient<T>(IVssRequestContext requestContext) where T : VssHttpClientBase;

    string GetCollectionName(IVssRequestContext requestContext);

    bool HasCollectionRedirected();

    Uri GetCollectionUrl(IVssRequestContext requestContext);
  }
}
