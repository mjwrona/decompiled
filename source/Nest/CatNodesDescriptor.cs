// Decompiled with JetBrains decompiler
// Type: Nest.CatNodesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatNodesDescriptor : 
    RequestDescriptorBase<CatNodesDescriptor, CatNodesRequestParameters, ICatNodesRequest>,
    ICatNodesRequest,
    IRequest<CatNodesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatNodes;

    public CatNodesDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatNodesDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatNodesDescriptor FullId(bool? fullid = true) => this.Qs("full_id", (object) fullid);

    public CatNodesDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatNodesDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatNodesDescriptor IncludeUnloadedSegments(bool? includeunloadedsegments = true) => this.Qs("include_unloaded_segments", (object) includeunloadedsegments);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.6.0, reason: This parameter does not cause this API to act locally.")]
    public CatNodesDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public CatNodesDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatNodesDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatNodesDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
