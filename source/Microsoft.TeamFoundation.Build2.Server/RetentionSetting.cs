// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RetentionSetting
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public struct RetentionSetting
  {
    public int Min;
    public int Max;
    public int Value;

    public override string ToString() => string.Format("min: {0}, max: {1}, val: {2}", (object) this.Min, (object) this.Max, (object) this.Value);
  }
}
