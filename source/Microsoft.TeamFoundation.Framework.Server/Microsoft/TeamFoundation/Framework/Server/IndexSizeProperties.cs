// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IndexSizeProperties
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class IndexSizeProperties
  {
    public string Schema { get; set; }

    public string Table { get; set; }

    public string Index { get; set; }

    public short IndexId { get; set; }

    public long ReservedBytes { get; set; }

    public long UsedBytes { get; set; }

    public string FullName => "[" + this.Schema + "].[" + this.Table + "].[" + this.Index + "]";

    public long OverPervisionBytes => this.ReservedBytes - this.UsedBytes;

    public long OverPervisionPercent => this.UsedBytes == 0L ? 0L : (this.ReservedBytes - this.UsedBytes) * 100L / this.UsedBytes;
  }
}
