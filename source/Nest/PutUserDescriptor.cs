// Decompiled with JetBrains decompiler
// Type: Nest.PutUserDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class PutUserDescriptor : 
    RequestDescriptorBase<PutUserDescriptor, PutUserRequestParameters, IPutUserRequest>,
    IPutUserRequest,
    IRequest<PutUserRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityPutUser;

    public PutUserDescriptor(Name username)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (username), (IUrlParameter) username)))
    {
    }

    [SerializationConstructor]
    protected PutUserDescriptor()
    {
    }

    Name IPutUserRequest.Username => this.Self.RouteValues.Get<Name>("username");

    public PutUserDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    string IPutUserRequest.Email { get; set; }

    string IPutUserRequest.FullName { get; set; }

    IDictionary<string, object> IPutUserRequest.Metadata { get; set; }

    string IPutUserRequest.Password { get; set; }

    string IPutUserRequest.PasswordHash { get; set; }

    IEnumerable<string> IPutUserRequest.Roles { get; set; }

    public PutUserDescriptor Password(string password) => this.Assign<string>(password, (Action<IPutUserRequest, string>) ((a, v) => a.Password = v));

    public PutUserDescriptor PasswordHash(string passwordHash) => this.Assign<string>(passwordHash, (Action<IPutUserRequest, string>) ((a, v) => a.PasswordHash = v));

    public PutUserDescriptor Roles(IEnumerable<string> roles) => this.Assign<IEnumerable<string>>(roles, (Action<IPutUserRequest, IEnumerable<string>>) ((a, v) => a.Roles = v));

    public PutUserDescriptor Roles(params string[] roles) => this.Assign<string[]>(roles, (Action<IPutUserRequest, string[]>) ((a, v) => a.Roles = (IEnumerable<string>) v));

    public PutUserDescriptor FullName(string fullName) => this.Assign<string>(fullName, (Action<IPutUserRequest, string>) ((a, v) => a.FullName = v));

    public PutUserDescriptor Email(string email) => this.Assign<string>(email, (Action<IPutUserRequest, string>) ((a, v) => a.Email = v));

    public PutUserDescriptor Metadata(IDictionary<string, object> metadata) => this.Assign<IDictionary<string, object>>(metadata, (Action<IPutUserRequest, IDictionary<string, object>>) ((a, v) => a.Metadata = v));

    public PutUserDescriptor Metadata(
      Func<FluentDictionary<string, object>, IDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, IDictionary<string, object>>>(selector, (Action<IPutUserRequest, Func<FluentDictionary<string, object>, IDictionary<string, object>>>) ((a, v) => a.Metadata = v != null ? v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
