// Decompiled with JetBrains decompiler
// Type: Nest.DeleteVotingConfigExclusionsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.ClusterApi;

namespace Nest
{
  public class DeleteVotingConfigExclusionsDescriptor : 
    RequestDescriptorBase<DeleteVotingConfigExclusionsDescriptor, DeleteVotingConfigExclusionsRequestParameters, IDeleteVotingConfigExclusionsRequest>,
    IDeleteVotingConfigExclusionsRequest,
    IRequest<DeleteVotingConfigExclusionsRequestParameters>,
    IRequest
  {
    protected override sealed void RequestDefaults(
      DeleteVotingConfigExclusionsRequestParameters parameters)
    {
      parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new DeleteVotingConfigExclusionsResponseBuilder();
    }

    internal override ApiUrls ApiUrls => ApiUrlsLookups.ClusterDeleteVotingConfigExclusions;

    public DeleteVotingConfigExclusionsDescriptor WaitForRemoval(bool? waitforremoval = true) => this.Qs("wait_for_removal", (object) waitforremoval);
  }
}
