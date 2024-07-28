// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.SliceDescriptor
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class SliceDescriptor
  {
    public SliceDescriptor(int fileId, string path, int changesId, SliceSource source)
    {
      this.SliceFileId = fileId;
      this.AggregatePath = path;
      this.ChangesId = changesId;
      this.Source = source;
    }

    public int SliceFileId { get; private set; }

    public string AggregatePath { get; private set; }

    public int ChangesId { get; private set; }

    public SliceSource Source { get; private set; }
  }
}
