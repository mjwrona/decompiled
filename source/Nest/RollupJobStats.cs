﻿// Decompiled with JetBrains decompiler
// Type: Nest.RollupJobStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class RollupJobStats
  {
    [DataMember(Name = "documents_processed")]
    public long DocumentsProcessed { get; internal set; }

    [DataMember(Name = "pages_processed")]
    public long PagesProcessed { get; internal set; }

    [DataMember(Name = "rollups_indexed")]
    public long RollupsIndexed { get; internal set; }

    [DataMember(Name = "trigger_count")]
    public long TriggerCount { get; internal set; }

    [DataMember(Name = "search_failures")]
    public long? SearchFailures { get; internal set; }

    [DataMember(Name = "index_failures")]
    public long? IndexFailures { get; internal set; }

    [DataMember(Name = "index_time_in_ms")]
    public long? IndexTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "index_total")]
    public long? IndexTotal { get; internal set; }

    [DataMember(Name = "search_time_in_ms")]
    public long? SearchTimeInMilliseconds { get; internal set; }

    [DataMember(Name = "search_total")]
    public long? SearchTotal { get; internal set; }
  }
}
