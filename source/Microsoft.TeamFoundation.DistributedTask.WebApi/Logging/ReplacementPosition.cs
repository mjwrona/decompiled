// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Logging.ReplacementPosition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

namespace Microsoft.TeamFoundation.DistributedTask.Logging
{
  internal sealed class ReplacementPosition
  {
    public ReplacementPosition(int start, int length)
    {
      this.Start = start;
      this.Length = length;
    }

    public ReplacementPosition(ReplacementPosition copy)
    {
      this.Start = copy.Start;
      this.Length = copy.Length;
    }

    public int Start { get; set; }

    public int Length { get; set; }

    public int End => this.Start + this.Length;
  }
}
