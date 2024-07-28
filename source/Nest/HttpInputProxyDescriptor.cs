// Decompiled with JetBrains decompiler
// Type: Nest.HttpInputProxyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HttpInputProxyDescriptor : 
    DescriptorBase<HttpInputProxyDescriptor, IHttpInputProxy>,
    IHttpInputProxy
  {
    string IHttpInputProxy.Host { get; set; }

    int? IHttpInputProxy.Port { get; set; }

    public HttpInputProxyDescriptor Host(string host) => this.Assign<string>(host, (Action<IHttpInputProxy, string>) ((a, v) => a.Host = v));

    public HttpInputProxyDescriptor Port(int? port) => this.Assign<int?>(port, (Action<IHttpInputProxy, int?>) ((a, v) => a.Port = v));
  }
}
