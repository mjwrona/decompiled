// Decompiled with JetBrains decompiler
// Type: Nest.HdfsRepositorySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class HdfsRepositorySettings : IHdfsRepositorySettings, IRepositorySettings
  {
    internal HdfsRepositorySettings()
    {
    }

    public HdfsRepositorySettings(string path) => this.Path = path;

    public string ChunkSize { get; set; }

    public bool? Compress { get; set; }

    public int? ConcurrentStreams { get; set; }

    public string ConfigurationLocation { get; set; }

    public Dictionary<string, object> InlineHadoopConfiguration { get; set; }

    public bool? LoadDefaults { get; set; }

    public string Path { get; set; }

    public string Uri { get; set; }
  }
}
