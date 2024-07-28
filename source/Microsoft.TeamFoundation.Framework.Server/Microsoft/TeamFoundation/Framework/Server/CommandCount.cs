// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CommandCount
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CommandCount
  {
    public string Caller { get; set; }

    public long Count { get; set; }

    public double ExecutionTimeInMinutes { get; set; }

    public override string ToString() => string.Format("Caller={0}, CommandName={1}, ExecutionTimeInMinutes={2}", (object) this.Caller, (object) this.Count, (object) this.ExecutionTimeInMinutes);
  }
}
