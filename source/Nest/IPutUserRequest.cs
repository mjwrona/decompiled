// Decompiled with JetBrains decompiler
// Type: Nest.IPutUserRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("security.put_user.json")]
  public interface IPutUserRequest : IRequest<PutUserRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Name Username { get; }

    [DataMember(Name = "email")]
    string Email { get; set; }

    [DataMember(Name = "full_name")]
    string FullName { get; set; }

    [DataMember(Name = "metadata")]
    IDictionary<string, object> Metadata { get; set; }

    [DataMember(Name = "password")]
    string Password { get; set; }

    [DataMember(Name = "password_hash")]
    string PasswordHash { get; set; }

    [DataMember(Name = "roles")]
    IEnumerable<string> Roles { get; set; }
  }
}
