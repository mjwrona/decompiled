// Decompiled with JetBrains decompiler
// Type: Nest.PostVotingConfigExclusionsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;

namespace Nest
{
  public class PostVotingConfigExclusionsDescriptor : 
    RequestDescriptorBase<PostVotingConfigExclusionsDescriptor, PostVotingConfigExclusionsRequestParameters, IPostVotingConfigExclusionsRequest>,
    IPostVotingConfigExclusionsRequest,
    IRequest<PostVotingConfigExclusionsRequestParameters>,
    IRequest
  {
    protected override sealed void RequestDefaults(
      PostVotingConfigExclusionsRequestParameters parameters)
    {
      parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new PostVotingConfigExclusionsResponseBuilder();
    }

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterPostVotingConfigExclusions;

    public PostVotingConfigExclusionsDescriptor NodeIds(string nodeids) => this.Qs("node_ids", (object) nodeids);

    public PostVotingConfigExclusionsDescriptor NodeNames(string nodenames) => this.Qs("node_names", (object) nodenames);

    public PostVotingConfigExclusionsDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
