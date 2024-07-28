// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildMetric
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildMetric
  {
    public string Name { get; set; }

    public string Scope { get; set; }

    public int IntValue { get; set; }

    public DateTime? Date { get; set; }

    public BuildMetric Clone() => new BuildMetric()
    {
      Name = this.Name,
      Scope = this.Scope,
      IntValue = this.IntValue,
      Date = this.Date
    };
  }
}
