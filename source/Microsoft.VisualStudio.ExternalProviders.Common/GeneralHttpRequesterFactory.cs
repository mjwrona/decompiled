// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.GeneralHttpRequesterFactory
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public class GeneralHttpRequesterFactory : IExternalProviderHttpRequesterFactory
  {
    private readonly IVssRequestContext _requestContext;
    private readonly string _providerType;
    private readonly ClientProviderHelper.Options _handlerOptions;
    private const string RegistryPath = "/ExternalProviders/GeneralHttpRequesterFactory";

    public string ProviderType => this._providerType;

    public GeneralHttpRequesterFactory(IVssRequestContext requestContext, string providerType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(providerType, nameof (providerType));
      this._handlerOptions = this.CreateHandlerOptions(requestContext);
      this._requestContext = requestContext;
      this._providerType = providerType;
    }

    private ClientProviderHelper.Options CreateHandlerOptions(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int maxRetryCount = service.GetValue<int>(requestContext, (RegistryQuery) "/ExternalProviders/GeneralHttpRequesterFactory/MaxRetryCount", 3);
      int num = service.GetValue<int>(requestContext, (RegistryQuery) "/ExternalProviders/GeneralHttpRequesterFactory/SlowRequestThresholdSecs", 5);
      byte tracePercentage = service.GetValue<byte>(requestContext, (RegistryQuery) "/ExternalProviders/GeneralHttpRequesterFactory/TracePercentage", (byte) 100);
      return new ClientProviderHelper.Options(maxRetryCount, TimeSpan.FromSeconds((double) num), tracePercentage);
    }

    public IExternalProviderHttpRequester GetRequester(HttpMessageHandler httpMessageHandler) => (IExternalProviderHttpRequester) new GeneralHttpRequester(this.GetMessageHandler(httpMessageHandler));

    private HttpMessageHandler GetMessageHandler(HttpMessageHandler httpMessageHandler)
    {
      List<DelegatingHandler> delegatingHandlers = ClientProviderHelper.GetMinimalDelegatingHandlers(this._requestContext, typeof (GeneralHttpRequester), this._handlerOptions, this._providerType);
      return HttpClientFactory.CreatePipeline(httpMessageHandler, (IEnumerable<DelegatingHandler>) delegatingHandlers);
    }

    public void Initialize(object requestContext)
    {
    }
  }
}
