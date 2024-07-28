// Decompiled with JetBrains decompiler
// Type: Nest.HdfsRepositorySettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class HdfsRepositorySettingsDescriptor : 
    DescriptorBase<HdfsRepositorySettingsDescriptor, IHdfsRepositorySettings>,
    IHdfsRepositorySettings,
    IRepositorySettings
  {
    string IHdfsRepositorySettings.ChunkSize { get; set; }

    bool? IHdfsRepositorySettings.Compress { get; set; }

    int? IHdfsRepositorySettings.ConcurrentStreams { get; set; }

    string IHdfsRepositorySettings.ConfigurationLocation { get; set; }

    Dictionary<string, object> IHdfsRepositorySettings.InlineHadoopConfiguration { get; set; }

    bool? IHdfsRepositorySettings.LoadDefaults { get; set; }

    string IHdfsRepositorySettings.Path { get; set; }

    string IHdfsRepositorySettings.Uri { get; set; }

    public HdfsRepositorySettingsDescriptor Uri(string uri) => this.Assign<string>(uri, (Action<IHdfsRepositorySettings, string>) ((a, v) => a.Uri = v));

    public HdfsRepositorySettingsDescriptor Path(string path) => this.Assign<string>(path, (Action<IHdfsRepositorySettings, string>) ((a, v) => a.Path = v));

    public HdfsRepositorySettingsDescriptor LoadDefaults(bool? loadDefaults = true) => this.Assign<bool?>(loadDefaults, (Action<IHdfsRepositorySettings, bool?>) ((a, v) => a.LoadDefaults = v));

    public HdfsRepositorySettingsDescriptor ConfigurationLocation(string configurationLocation) => this.Assign<string>(configurationLocation, (Action<IHdfsRepositorySettings, string>) ((a, v) => a.ConfigurationLocation = v));

    public HdfsRepositorySettingsDescriptor InlinedHadoopConfiguration(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> inlineConfig)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(inlineConfig, (Action<IHdfsRepositorySettings, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.InlineHadoopConfiguration = (Dictionary<string, object>) v(new FluentDictionary<string, object>())));
    }

    public HdfsRepositorySettingsDescriptor Compress(bool? compress = true) => this.Assign<bool?>(compress, (Action<IHdfsRepositorySettings, bool?>) ((a, v) => a.Compress = v));

    public HdfsRepositorySettingsDescriptor ConcurrentStreams(int? concurrentStreams) => this.Assign<int?>(concurrentStreams, (Action<IHdfsRepositorySettings, int?>) ((a, v) => a.ConcurrentStreams = v));

    public HdfsRepositorySettingsDescriptor ChunkSize(string chunkSize) => this.Assign<string>(chunkSize, (Action<IHdfsRepositorySettings, string>) ((a, v) => a.ChunkSize = v));
  }
}
