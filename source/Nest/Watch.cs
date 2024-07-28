// Decompiled with JetBrains decompiler
// Type: Nest.Watch
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class Watch : IWatch
  {
    WatchStatus IWatch.Status { get; set; }

    public Actions Actions { get; set; }

    public ConditionContainer Condition { get; set; }

    public InputContainer Input { get; set; }

    public IDictionary<string, object> Metadata { get; set; }

    public WatchStatus Status { get; set; }

    public string ThrottlePeriod { get; set; }

    public TransformContainer Transform { get; set; }

    public TriggerContainer Trigger { get; set; }
  }
}
