// Decompiled with JetBrains decompiler
// Type: Nest.IHttpInputRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (HttpInputRequest))]
  public interface IHttpInputRequest
  {
    [DataMember(Name = "auth")]
    IHttpInputAuthentication Authentication { get; set; }

    [DataMember(Name = "body")]
    string Body { get; set; }

    [DataMember(Name = "connection_timeout")]
    Time ConnectionTimeout { get; set; }

    [DataMember(Name = "headers")]
    IDictionary<string, string> Headers { get; set; }

    [DataMember(Name = "host")]
    string Host { get; set; }

    [DataMember(Name = "method")]
    HttpInputMethod? Method { get; set; }

    [DataMember(Name = "params")]
    IDictionary<string, string> Params { get; set; }

    [DataMember(Name = "path")]
    string Path { get; set; }

    [DataMember(Name = "port")]
    int? Port { get; set; }

    [DataMember(Name = "proxy")]
    IHttpInputProxy Proxy { get; set; }

    [DataMember(Name = "read_timeout")]
    Time ReadTimeout { get; set; }

    [DataMember(Name = "scheme")]
    ConnectionScheme? Scheme { get; set; }

    [DataMember(Name = "url")]
    string Url { get; set; }
  }
}
