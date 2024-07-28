// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.BitbucketHttpClientFactory
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public static class BitbucketHttpClientFactory
  {
    public static BitbucketHttpClient Create(IVssRequestContext requestContext, TimeSpan? timeout = null)
    {
      BitbucketHttpRequesterFactory httpRequesterFactory = new BitbucketHttpRequesterFactory();
      httpRequesterFactory.Initialize((object) requestContext);
      CancellationToken? cancellationToken = new CancellationToken?();
      if (requestContext.GetType() == typeof (JobRequestContext))
        cancellationToken = new CancellationToken?(requestContext.CancellationToken);
      return new BitbucketHttpClient((IExternalProviderHttpRequesterFactory) httpRequesterFactory, timeout, cancellationToken: cancellationToken);
    }
  }
}
