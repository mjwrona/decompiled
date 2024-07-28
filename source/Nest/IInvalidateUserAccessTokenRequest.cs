// Decompiled with JetBrains decompiler
// Type: Nest.IInvalidateUserAccessTokenRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("security.invalidate_token.json")]
  public interface IInvalidateUserAccessTokenRequest : 
    IRequest<InvalidateUserAccessTokenRequestParameters>,
    IRequest
  {
    [DataMember(Name = "token")]
    string Token { get; set; }

    [DataMember(Name = "refresh_token")]
    string RefreshToken { get; set; }

    [DataMember(Name = "realm_name")]
    string RealmName { get; set; }

    [DataMember(Name = "username")]
    string Username { get; set; }
  }
}
