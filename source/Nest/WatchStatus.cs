// Decompiled with JetBrains decompiler
// Type: Nest.WatchStatus
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class WatchStatus
  {
    [DataMember(Name = "actions")]
    public IReadOnlyDictionary<string, ActionStatus> Actions { get; set; }

    [DataMember(Name = "last_checked")]
    public DateTimeOffset? LastChecked { get; set; }

    [DataMember(Name = "last_met_condition")]
    public DateTimeOffset? LastMetCondition { get; set; }

    [DataMember(Name = "state")]
    public ActivationState State { get; set; }

    [DataMember(Name = "version")]
    public int? Version { get; set; }
  }
}
