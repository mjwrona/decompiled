// Decompiled with JetBrains decompiler
// Type: Nest.RestoreDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.SnapshotApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class RestoreDescriptor : 
    RequestDescriptorBase<RestoreDescriptor, RestoreRequestParameters, IRestoreRequest>,
    IRestoreRequest,
    IRequest<RestoreRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.SnapshotRestore;

    public RestoreDescriptor(Name repository, Name snapshot)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (repository), (IUrlParameter) repository).Required(nameof (snapshot), (IUrlParameter) snapshot)))
    {
    }

    [SerializationConstructor]
    protected RestoreDescriptor()
    {
    }

    Name IRestoreRequest.RepositoryName => this.Self.RouteValues.Get<Name>("repository");

    Name IRestoreRequest.Snapshot => this.Self.RouteValues.Get<Name>("snapshot");

    public RestoreDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public RestoreDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);

    List<string> IRestoreRequest.IgnoreIndexSettings { get; set; }

    bool? IRestoreRequest.IgnoreUnavailable { get; set; }

    bool? IRestoreRequest.IncludeAliases { get; set; }

    bool? IRestoreRequest.IncludeGlobalState { get; set; }

    IUpdateIndexSettingsRequest IRestoreRequest.IndexSettings { get; set; }

    Nest.Indices IRestoreRequest.Indices { get; set; }

    bool? IRestoreRequest.Partial { get; set; }

    string IRestoreRequest.RenamePattern { get; set; }

    string IRestoreRequest.RenameReplacement { get; set; }

    public RestoreDescriptor Index(IndexName index) => this.Indices((Nest.Indices) index);

    public RestoreDescriptor Index<T>() where T : class => this.Indices((Nest.Indices) typeof (T));

    public RestoreDescriptor Indices(Nest.Indices indices) => this.Assign<Nest.Indices>(indices, (Action<IRestoreRequest, Nest.Indices>) ((a, v) => a.Indices = v));

    public RestoreDescriptor IgnoreUnavailable(bool? ignoreUnavailable = true) => this.Assign<bool?>(ignoreUnavailable, (Action<IRestoreRequest, bool?>) ((a, v) => a.IgnoreUnavailable = v));

    public RestoreDescriptor IncludeGlobalState(bool? includeGlobalState = true) => this.Assign<bool?>(includeGlobalState, (Action<IRestoreRequest, bool?>) ((a, v) => a.IncludeGlobalState = v));

    public RestoreDescriptor RenamePattern(string renamePattern) => this.Assign<string>(renamePattern, (Action<IRestoreRequest, string>) ((a, v) => a.RenamePattern = v));

    public RestoreDescriptor RenameReplacement(string renameReplacement) => this.Assign<string>(renameReplacement, (Action<IRestoreRequest, string>) ((a, v) => a.RenameReplacement = v));

    public RestoreDescriptor IndexSettings(
      Func<UpdateIndexSettingsDescriptor, IUpdateIndexSettingsRequest> settingsSelector)
    {
      return this.Assign<Func<UpdateIndexSettingsDescriptor, IUpdateIndexSettingsRequest>>(settingsSelector, (Action<IRestoreRequest, Func<UpdateIndexSettingsDescriptor, IUpdateIndexSettingsRequest>>) ((a, v) => a.IndexSettings = v != null ? v(new UpdateIndexSettingsDescriptor()) : (IUpdateIndexSettingsRequest) null));
    }

    public RestoreDescriptor IgnoreIndexSettings(List<string> ignoreIndexSettings) => this.Assign<List<string>>(ignoreIndexSettings, (Action<IRestoreRequest, List<string>>) ((a, v) => a.IgnoreIndexSettings = v));

    public RestoreDescriptor IgnoreIndexSettings(params string[] ignoreIndexSettings) => this.Assign<List<string>>(((IEnumerable<string>) ignoreIndexSettings).ToListOrNullIfEmpty<string>(), (Action<IRestoreRequest, List<string>>) ((a, v) => a.IgnoreIndexSettings = v));

    public RestoreDescriptor IncludeAliases(bool? includeAliases = true) => this.Assign<bool?>(includeAliases, (Action<IRestoreRequest, bool?>) ((a, v) => a.IncludeAliases = v));

    public RestoreDescriptor Partial(bool? partial = true) => this.Assign<bool?>(partial, (Action<IRestoreRequest, bool?>) ((a, v) => a.Partial = v));
  }
}
