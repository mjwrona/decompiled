// Decompiled with JetBrains decompiler
// Type: Nest.GetApiKeyRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.SecurityApi;

namespace Nest
{
  public class GetApiKeyRequest : 
    PlainRequestBase<GetApiKeyRequestParameters>,
    IGetApiKeyRequest,
    IRequest<GetApiKeyRequestParameters>,
    IRequest
  {
    protected IGetApiKeyRequest Self => (IGetApiKeyRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.SecurityGetApiKey;

    public string Id
    {
      get => this.Q<string>("id");
      set => this.Q("id", (object) value);
    }

    public string Name
    {
      get => this.Q<string>("name");
      set => this.Q("name", (object) value);
    }

    public bool? Owner
    {
      get => this.Q<bool?>("owner");
      set => this.Q("owner", (object) value);
    }

    public string RealmName
    {
      get => this.Q<string>("realm_name");
      set => this.Q("realm_name", (object) value);
    }

    public string Username
    {
      get => this.Q<string>("username");
      set => this.Q("username", (object) value);
    }
  }
}
