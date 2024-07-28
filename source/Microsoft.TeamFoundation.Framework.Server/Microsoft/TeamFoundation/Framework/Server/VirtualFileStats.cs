// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VirtualFileStats
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VirtualFileStats
  {
    public short DatabaseId { get; set; }

    public short FileId { get; set; }

    public long SampleMs { get; set; }

    public long NumReads { get; set; }

    public long NumBytesRead { get; set; }

    public long IoStallReadMs { get; set; }

    public long NumWrites { get; set; }

    public long NumBytesWritten { get; set; }

    public long IoStallWriteMs { get; set; }

    public long IoStall { get; set; }

    public long SizeOnDiskBytes { get; set; }

    public long IoStallQueuedReadMs { get; set; }

    public long IoStallQueueWriteMs { get; set; }
  }
}
