// Decompiled with JetBrains decompiler
// Type: Nest.ChunkingConfigDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ChunkingConfigDescriptor : 
    DescriptorBase<ChunkingConfigDescriptor, IChunkingConfig>,
    IChunkingConfig
  {
    ChunkingMode? IChunkingConfig.Mode { get; set; }

    Time IChunkingConfig.TimeSpan { get; set; }

    public ChunkingConfigDescriptor Mode(ChunkingMode? mode) => this.Assign<ChunkingMode?>(mode, (Action<IChunkingConfig, ChunkingMode?>) ((a, v) => a.Mode = v));

    public ChunkingConfigDescriptor TimeSpan(Time timeSpan) => this.Assign<Time>(timeSpan, (Action<IChunkingConfig, Time>) ((a, v) => a.TimeSpan = v));
  }
}
