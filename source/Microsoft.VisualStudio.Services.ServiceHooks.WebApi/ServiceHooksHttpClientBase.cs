// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public abstract class ServiceHooksHttpClientBase : VssHttpClientBase
  {
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();

    static ServiceHooksHttpClientBase()
    {
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("ConsumerActionNotFoundException", typeof (ConsumerActionNotFoundException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("ConsumerNotFoundException", typeof (ConsumerNotFoundException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("NoPublishersDefinedException", typeof (NoPublishersDefinedException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("NoPublisherSpecifiedException", typeof (NoPublisherSpecifiedException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("NotificationNotFoundException", typeof (NotificationNotFoundException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("PublisherEventTypeNotFoundException", typeof (PublisherEventTypeNotFoundException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("PublisherNotFoundException", typeof (PublisherNotFoundException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("ServiceHookException", typeof (ServiceHookException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("SubscriptionInputException", typeof (SubscriptionInputException));
      ServiceHooksHttpClientBase.s_translatedExceptions.Add("SubscriptionNotFoundException", typeof (SubscriptionNotFoundException));
    }

    protected ServiceHooksHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    protected ServiceHooksHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    protected ServiceHooksHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    protected ServiceHooksHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    protected ServiceHooksHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) ServiceHooksHttpClientBase.s_translatedExceptions;
  }
}
