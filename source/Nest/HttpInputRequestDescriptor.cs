// Decompiled with JetBrains decompiler
// Type: Nest.HttpInputRequestDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class HttpInputRequestDescriptor : 
    DescriptorBase<HttpInputRequestDescriptor, IHttpInputRequest>,
    IHttpInputRequest
  {
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

    public HttpInputRequestDescriptor Authentication(
      Func<HttpInputAuthenticationDescriptor, IHttpInputAuthentication> authSelector)
    {
      return this.Assign<IHttpInputAuthentication>(authSelector(new HttpInputAuthenticationDescriptor()), (Action<IHttpInputRequest, IHttpInputAuthentication>) ((a, v) => a.Authentication = v));
    }

    public HttpInputRequestDescriptor Body(string body) => this.Assign<string>(body, (Action<IHttpInputRequest, string>) ((a, v) => a.Body = v));

    public HttpInputRequestDescriptor ConnectionTimeout(Time connectionTimeout) => this.Assign<Time>(connectionTimeout, (Action<IHttpInputRequest, Time>) ((a, v) => a.ConnectionTimeout = v));

    public HttpInputRequestDescriptor Headers(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> headersSelector)
    {
      return this.Assign<FluentDictionary<string, string>>(headersSelector(new FluentDictionary<string, string>()), (Action<IHttpInputRequest, FluentDictionary<string, string>>) ((a, v) => a.Headers = (IDictionary<string, string>) v));
    }

    public HttpInputRequestDescriptor Headers(Dictionary<string, string> headersDictionary) => this.Assign<Dictionary<string, string>>(headersDictionary, (Action<IHttpInputRequest, Dictionary<string, string>>) ((a, v) => a.Headers = (IDictionary<string, string>) v));

    public HttpInputRequestDescriptor Host(string host) => this.Assign<string>(host, (Action<IHttpInputRequest, string>) ((a, v) => a.Host = v));

    public HttpInputRequestDescriptor Method(HttpInputMethod? method) => this.Assign<HttpInputMethod?>(method, (Action<IHttpInputRequest, HttpInputMethod?>) ((a, v) => a.Method = v));

    public HttpInputRequestDescriptor Path(string path) => this.Assign<string>(path, (Action<IHttpInputRequest, string>) ((a, v) => a.Path = v));

    public HttpInputRequestDescriptor Params(
      Func<FluentDictionary<string, string>, FluentDictionary<string, string>> paramsSelector)
    {
      return this.Assign<FluentDictionary<string, string>>(paramsSelector(new FluentDictionary<string, string>()), (Action<IHttpInputRequest, FluentDictionary<string, string>>) ((a, v) => a.Params = (IDictionary<string, string>) v));
    }

    public HttpInputRequestDescriptor Params(Dictionary<string, string> paramsDictionary) => this.Assign<Dictionary<string, string>>(paramsDictionary, (Action<IHttpInputRequest, Dictionary<string, string>>) ((a, v) => a.Params = (IDictionary<string, string>) v));

    public HttpInputRequestDescriptor Port(int? port) => this.Assign<int?>(port, (Action<IHttpInputRequest, int?>) ((a, v) => a.Port = v));

    public HttpInputRequestDescriptor Proxy(
      Func<HttpInputProxyDescriptor, IHttpInputProxy> proxySelector)
    {
      return this.Assign<IHttpInputProxy>(proxySelector(new HttpInputProxyDescriptor()), (Action<IHttpInputRequest, IHttpInputProxy>) ((a, v) => a.Proxy = v));
    }

    public HttpInputRequestDescriptor ReadTimeout(Time readTimeout) => this.Assign<Time>(readTimeout, (Action<IHttpInputRequest, Time>) ((a, v) => a.ReadTimeout = v));

    public HttpInputRequestDescriptor Scheme(ConnectionScheme? scheme) => this.Assign<ConnectionScheme?>(scheme, (Action<IHttpInputRequest, ConnectionScheme?>) ((a, v) => a.Scheme = v));

    public HttpInputRequestDescriptor Url(string url) => this.Assign<string>(url, (Action<IHttpInputRequest, string>) ((a, v) => a.Url = v));
  }
}
