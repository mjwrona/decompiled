// Decompiled with JetBrains decompiler
// Type: Nest.WebhookActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class WebhookActionDescriptor : 
    ActionsDescriptorBase<WebhookActionDescriptor, IWebhookAction>,
    IWebhookAction,
    IAction,
    IHttpInputRequest
  {
    public WebhookActionDescriptor(string name)
      : base(name)
    {
    }

    protected override ActionType ActionType => ActionType.Webhook;

    IHttpInputAuthentication IHttpInputRequest.Authentication { get; set; }

    string IHttpInputRequest.Body { get; set; }

    Time IHttpInputRequest.ConnectionTimeout { get; set; }

    IDictionary<string, string> IHttpInputRequest.Headers { get; set; }

    string IHttpInputRequest.Host { get; set; }

    HttpInputMethod? IHttpInputRequest.Method { get; set; }

    IDictionary<string, string> IHttpInputRequest.Params { get; set; }

    string IHttpInputRequest.Path { get; set; }

    int? IHttpInputRequest.Port { get; set; }

    IHttpInputProxy IHttpInputRequest.Proxy { get; set; }

    Time IHttpInputRequest.ReadTimeout { get; set; }

    ConnectionScheme? IHttpInputRequest.Scheme { get; set; }

    string IHttpInputRequest.Url { get; set; }

    public WebhookActionDescriptor Authentication(
      Func<HttpInputAuthenticationDescriptor, IHttpInputAuthentication> selector)
    {
      return this.Assign<Func<HttpInputAuthenticationDescriptor, IHttpInputAuthentication>>(selector, (Action<IWebhookAction, Func<HttpInputAuthenticationDescriptor, IHttpInputAuthentication>>) ((a, v) => a.Authentication = v != null ? v(new HttpInputAuthenticationDescriptor()) : (IHttpInputAuthentication) null));
    }

    public WebhookActionDescriptor Body(string body) => this.Assign<string>(body, (Action<IWebhookAction, string>) ((a, v) => a.Body = v));

    public WebhookActionDescriptor ConnectionTimeout(Time connectionTimeout) => this.Assign<Time>(connectionTimeout, (Action<IWebhookAction, Time>) ((a, v) => a.ConnectionTimeout = v));

    public WebhookActionDescriptor Headers(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> headersSelector)
    {
      return this.Assign<FluentDictionary<string, string>>(headersSelector(new FluentDictionary<string, string>()), (Action<IWebhookAction, FluentDictionary<string, string>>) ((a, v) => a.Headers = (IDictionary<string, string>) v));
    }

    public WebhookActionDescriptor Headers(Dictionary<string, string> headersDictionary) => this.Assign<Dictionary<string, string>>(headersDictionary, (Action<IWebhookAction, Dictionary<string, string>>) ((a, v) => a.Headers = (IDictionary<string, string>) v));

    public WebhookActionDescriptor Host(string host) => this.Assign<string>(host, (Action<IWebhookAction, string>) ((a, v) => a.Host = v));

    public WebhookActionDescriptor Method(HttpInputMethod? method) => this.Assign<HttpInputMethod?>(method, (Action<IWebhookAction, HttpInputMethod?>) ((a, v) => a.Method = v));

    public WebhookActionDescriptor Path(string path) => this.Assign<string>(path, (Action<IWebhookAction, string>) ((a, v) => a.Path = v));

    public WebhookActionDescriptor Params(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> paramsSelector)
    {
      return this.Assign<Func<FluentDictionary<string, string>, FluentDictionary<string, string>>>(paramsSelector, (Action<IWebhookAction, Func<FluentDictionary<string, string>, FluentDictionary<string, string>>>) ((a, v) => a.Params = v != null ? (IDictionary<string, string>) v(new FluentDictionary<string, string>()) : (IDictionary<string, string>) null));
    }

    public WebhookActionDescriptor Params(Dictionary<string, string> paramsDictionary) => this.Assign<Dictionary<string, string>>(paramsDictionary, (Action<IWebhookAction, Dictionary<string, string>>) ((a, v) => a.Params = (IDictionary<string, string>) v));

    public WebhookActionDescriptor Port(int? port) => this.Assign<int?>(port, (Action<IWebhookAction, int?>) ((a, v) => a.Port = v));

    public WebhookActionDescriptor Proxy(
      Func<HttpInputProxyDescriptor, IHttpInputProxy> proxySelector)
    {
      return this.Assign<IHttpInputProxy>(proxySelector(new HttpInputProxyDescriptor()), (Action<IWebhookAction, IHttpInputProxy>) ((a, v) => a.Proxy = v));
    }

    public WebhookActionDescriptor ReadTimeout(Time readTimeout) => this.Assign<Time>(readTimeout, (Action<IWebhookAction, Time>) ((a, v) => a.ReadTimeout = v));

    public WebhookActionDescriptor Scheme(ConnectionScheme? scheme) => this.Assign<ConnectionScheme?>(scheme, (Action<IWebhookAction, ConnectionScheme?>) ((a, v) => a.Scheme = v));

    public WebhookActionDescriptor Url(string url) => this.Assign<string>(url, (Action<IWebhookAction, string>) ((a, v) => a.Url = v));
  }
}
