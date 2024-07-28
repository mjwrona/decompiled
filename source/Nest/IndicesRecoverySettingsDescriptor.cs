// Decompiled with JetBrains decompiler
// Type: Nest.IndicesRecoverySettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IndicesRecoverySettingsDescriptor : 
    DescriptorBase<IndicesRecoverySettingsDescriptor, IIndicesRecoverySettings>,
    IIndicesRecoverySettings
  {
    bool? IIndicesRecoverySettings.Compress { get; set; }

    int? IIndicesRecoverySettings.ConcurrentSmallFileStreams { get; set; }

    int? IIndicesRecoverySettings.ConcurrentStreams { get; set; }

    string IIndicesRecoverySettings.FileChunkSize { get; set; }

    string IIndicesRecoverySettings.MaxBytesPerSecond { get; set; }

    int? IIndicesRecoverySettings.TranslogOperations { get; set; }

    string IIndicesRecoverySettings.TranslogSize { get; set; }

    public IndicesRecoverySettingsDescriptor ConcurrentStreams(int? streams) => this.Assign<int?>(streams, (Action<IIndicesRecoverySettings, int?>) ((a, v) => a.ConcurrentStreams = v));

    public IndicesRecoverySettingsDescriptor ConcurrentSmallFileStreams(int? streams) => this.Assign<int?>(streams, (Action<IIndicesRecoverySettings, int?>) ((a, v) => a.ConcurrentSmallFileStreams = v));

    public IndicesRecoverySettingsDescriptor FileChunkSize(string size) => this.Assign<string>(size, (Action<IIndicesRecoverySettings, string>) ((a, v) => a.FileChunkSize = v));

    public IndicesRecoverySettingsDescriptor TranslogOperations(int? ops) => this.Assign<int?>(ops, (Action<IIndicesRecoverySettings, int?>) ((a, v) => a.TranslogOperations = v));

    public IndicesRecoverySettingsDescriptor TranslogSize(string size) => this.Assign<string>(size, (Action<IIndicesRecoverySettings, string>) ((a, v) => a.TranslogSize = v));

    public IndicesRecoverySettingsDescriptor Compress(bool? compress = true) => this.Assign<bool?>(compress, (Action<IIndicesRecoverySettings, bool?>) ((a, v) => a.Compress = v));

    public IndicesRecoverySettingsDescriptor MaxBytesPerSecond(string max) => this.Assign<string>(max, (Action<IIndicesRecoverySettings, string>) ((a, v) => a.MaxBytesPerSecond = v));
  }
}
