// Decompiled with JetBrains decompiler
// Type: Nest.PostVotingConfigExclusionsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;

namespace Nest
{
  public class PostVotingConfigExclusionsRequest : 
    PlainRequestBase<PostVotingConfigExclusionsRequestParameters>,
    IPostVotingConfigExclusionsRequest,
    IRequest<PostVotingConfigExclusionsRequestParameters>,
    IRequest
  {
    protected override sealed void RequestDefaults(
      PostVotingConfigExclusionsRequestParameters parameters)
    {
      parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new PostVotingConfigExclusionsResponseBuilder();
    }

    protected IPostVotingConfigExclusionsRequest Self => (IPostVotingConfigExclusionsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterPostVotingConfigExclusions;

    public string NodeIds
    {
      get => this.Q<string>("node_ids");
      set => this.Q("node_ids", (object) value);
    }

    public string NodeNames
    {
      get => this.Q<string>("node_names");
      set => this.Q("node_names", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
