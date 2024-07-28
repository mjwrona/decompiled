// Decompiled with JetBrains decompiler
// Type: Nest.ISnapshotLifecyclePolicy
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SnapshotLifecyclePolicy))]
  public interface ISnapshotLifecyclePolicy
  {
    [DataMember(Name = "config")]
    ISnapshotLifecycleConfig Config { get; set; }

    [DataMember(Name = "name")]
    string Name { get; set; }

    [DataMember(Name = "repository")]
    string Repository { get; set; }

    [DataMember(Name = "schedule")]
    CronExpression Schedule { get; set; }

    [DataMember(Name = "retention")]
    ISnapshotRetentionConfiguration Retention { get; set; }
  }
}
