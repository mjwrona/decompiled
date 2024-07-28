// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.IndexType
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public enum IndexType : byte
  {
    [Description("HEAP")] Heap,
    [Description("CLUSTERED")] Clustered,
    [Description("NONCLUSTERED")] NonClustered,
    [Description("XML")] Xml,
    [Description("SPATIAL")] Spatial,
    [Description("CLUSTERED COLUMNSTORE")] ClusteredColumnstore,
    [Description("NONCLUSTERED COLUMNSTORE")] NonclusteredColumnstore,
    [Description("NONCLUSTERED HASH")] NonclusteredHash,
  }
}
