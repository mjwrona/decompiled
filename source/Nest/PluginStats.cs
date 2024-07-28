// Decompiled with JetBrains decompiler
// Type: Nest.PluginStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class PluginStats
  {
    [DataMember(Name = "classname")]
    public string ClassName { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "elasticsearch_version")]
    public string ElasticsearchVersion { get; set; }

    [DataMember(Name = "extended_plugins")]
    public IReadOnlyCollection<string> ExtendedPlugins { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "has_native_controller")]
    public bool? HasNativeController { get; set; }

    [DataMember(Name = "java_version")]
    public string JavaVersion { get; set; }

    [DataMember(Name = "version")]
    public string Version { get; set; }
  }
}
