// Decompiled with JetBrains decompiler
// Type: Nest.InvalidateApiKeyDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;
using System;

namespace Nest
{
  public class InvalidateApiKeyDescriptor : 
    RequestDescriptorBase<InvalidateApiKeyDescriptor, InvalidateApiKeyRequestParameters, IInvalidateApiKeyRequest>,
    IInvalidateApiKeyRequest,
    IRequest<InvalidateApiKeyRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityInvalidateApiKey;

    string IInvalidateApiKeyRequest.Id { get; set; }

    string IInvalidateApiKeyRequest.Name { get; set; }

    string IInvalidateApiKeyRequest.RealmName { get; set; }

    string IInvalidateApiKeyRequest.Username { get; set; }

    bool? IInvalidateApiKeyRequest.Owner { get; set; }

    public InvalidateApiKeyDescriptor Id(string id) => this.Assign<string>(id, (Action<IInvalidateApiKeyRequest, string>) ((a, v) => a.Id = v));

    public InvalidateApiKeyDescriptor Name(string name) => this.Assign<string>(name, (Action<IInvalidateApiKeyRequest, string>) ((a, v) => a.Name = v));

    public InvalidateApiKeyDescriptor RealmName(string realmName) => this.Assign<string>(realmName, (Action<IInvalidateApiKeyRequest, string>) ((a, v) => a.RealmName = v));

    public InvalidateApiKeyDescriptor Username(string username) => this.Assign<string>(username, (Action<IInvalidateApiKeyRequest, string>) ((a, v) => a.Username = v));

    public InvalidateApiKeyDescriptor Owner(bool? owner = true) => this.Assign<bool?>(owner, (Action<IInvalidateApiKeyRequest, bool?>) ((a, v) => a.Owner = v));
  }
}
