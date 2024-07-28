// Decompiled with JetBrains decompiler
// Type: Nest.ReindexOnServerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class ReindexOnServerDescriptor : 
    RequestDescriptorBase<ReindexOnServerDescriptor, ReindexOnServerRequestParameters, IReindexOnServerRequest>,
    IReindexOnServerRequest,
    IRequest<ReindexOnServerRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceReindexOnServer;

    public ReindexOnServerDescriptor Refresh(bool? refresh = true) => this.Qs(nameof (refresh), (object) refresh);

    public ReindexOnServerDescriptor RequestsPerSecond(long? requestspersecond) => this.Qs("requests_per_second", (object) requestspersecond);

    public ReindexOnServerDescriptor Scroll(Time scroll) => this.Qs(nameof (scroll), (object) scroll);

    public ReindexOnServerDescriptor Slices(long? slices) => this.Qs(nameof (slices), (object) slices);

    public ReindexOnServerDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public ReindexOnServerDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    public ReindexOnServerDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);

    Elasticsearch.Net.Conflicts? IReindexOnServerRequest.Conflicts { get; set; }

    IReindexDestination IReindexOnServerRequest.Destination { get; set; }

    IScript IReindexOnServerRequest.Script { get; set; }

    [Obsolete("Deprecated. Use MaximumDocuments")]
    long? IReindexOnServerRequest.Size { get; set; }

    IReindexSource IReindexOnServerRequest.Source { get; set; }

    long? IReindexOnServerRequest.MaximumDocuments { get; set; }

    public ReindexOnServerDescriptor Source(
      Func<ReindexSourceDescriptor, IReindexSource> selector = null)
    {
      return this.Assign<IReindexSource>(selector.InvokeOrDefault<ReindexSourceDescriptor, IReindexSource>(new ReindexSourceDescriptor()), (Action<IReindexOnServerRequest, IReindexSource>) ((a, v) => a.Source = v));
    }

    public ReindexOnServerDescriptor Destination(
      Func<ReindexDestinationDescriptor, IReindexDestination> selector)
    {
      return this.Assign<Func<ReindexDestinationDescriptor, IReindexDestination>>(selector, (Action<IReindexOnServerRequest, Func<ReindexDestinationDescriptor, IReindexDestination>>) ((a, v) => a.Destination = v != null ? v(new ReindexDestinationDescriptor()) : (IReindexDestination) null));
    }

    public ReindexOnServerDescriptor Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IReindexOnServerRequest, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public ReindexOnServerDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IReindexOnServerRequest, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    [Obsolete("Deprecated. Use MaximumDocuments")]
    public ReindexOnServerDescriptor Size(long? size) => this.Assign<long?>(size, (Action<IReindexOnServerRequest, long?>) ((a, v) => a.Size = v));

    public ReindexOnServerDescriptor Conflicts(Elasticsearch.Net.Conflicts? conflicts) => this.Assign<Elasticsearch.Net.Conflicts?>(conflicts, (Action<IReindexOnServerRequest, Elasticsearch.Net.Conflicts?>) ((a, v) => a.Conflicts = v));

    public ReindexOnServerDescriptor MaximumDocuments(long? maximumDocuments) => this.Assign<long?>(maximumDocuments, (Action<IReindexOnServerRequest, long?>) ((a, v) => a.MaximumDocuments = v));
  }
}
