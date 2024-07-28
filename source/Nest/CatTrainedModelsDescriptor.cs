// Decompiled with JetBrains decompiler
// Type: Nest.CatTrainedModelsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatTrainedModelsDescriptor : 
    RequestDescriptorBase<CatTrainedModelsDescriptor, CatTrainedModelsRequestParameters, ICatTrainedModelsRequest>,
    ICatTrainedModelsRequest,
    IRequest<CatTrainedModelsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatTrainedModels;

    public CatTrainedModelsDescriptor()
    {
    }

    public CatTrainedModelsDescriptor(Id modelId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("model_id", (IUrlParameter) modelId)))
    {
    }

    Id ICatTrainedModelsRequest.ModelId => this.Self.RouteValues.Get<Id>("model_id");

    public CatTrainedModelsDescriptor ModelId(Id modelId) => this.Assign<Id>(modelId, (Action<ICatTrainedModelsRequest, Id>) ((a, v) => a.RouteValues.Optional("model_id", (IUrlParameter) v)));

    public CatTrainedModelsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public CatTrainedModelsDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatTrainedModelsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatTrainedModelsDescriptor From(int? from) => this.Qs(nameof (from), (object) from);

    public CatTrainedModelsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatTrainedModelsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatTrainedModelsDescriptor Size(int? size) => this.Qs(nameof (size), (object) size);

    public CatTrainedModelsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatTrainedModelsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
