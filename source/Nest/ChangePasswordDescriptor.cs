// Decompiled with JetBrains decompiler
// Type: Nest.ChangePasswordDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class ChangePasswordDescriptor : 
    RequestDescriptorBase<ChangePasswordDescriptor, ChangePasswordRequestParameters, IChangePasswordRequest>,
    IChangePasswordRequest,
    IRequest<ChangePasswordRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityChangePassword;

    public ChangePasswordDescriptor(Name username)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (username), (IUrlParameter) username)))
    {
    }

    public ChangePasswordDescriptor()
    {
    }

    Name IChangePasswordRequest.Username => this.Self.RouteValues.Get<Name>("username");

    public ChangePasswordDescriptor Username(Name username) => this.Assign<Name>(username, (Action<IChangePasswordRequest, Name>) ((a, v) => a.RouteValues.Optional(nameof (username), (IUrlParameter) v)));

    public ChangePasswordDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    string IChangePasswordRequest.Password { get; set; }

    public ChangePasswordDescriptor Password(string password) => this.Assign<string>(password, (Action<IChangePasswordRequest, string>) ((a, v) => a.Password = v));
  }
}
