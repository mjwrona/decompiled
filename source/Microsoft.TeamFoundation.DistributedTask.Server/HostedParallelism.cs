// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.HostedParallelism
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class HostedParallelism
  {
    public HostedParallelism()
      : this(HostedParallelismLevel.PaidOnly, HostedParallelismSource.Default)
    {
    }

    public HostedParallelism(HostedParallelismLevel level)
      : this(level, HostedParallelismSource.Default)
    {
    }

    public HostedParallelism(HostedParallelismLevel level, HostedParallelismSource source)
    {
      this.Level = level;
      this.Source = source;
    }

    public HostedParallelismLevel Level { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime LastUpdated { get; set; }

    public int Revision { get; set; }

    public HostedParallelismSource Source { get; set; }

    public string Description { get; set; }
  }
}
