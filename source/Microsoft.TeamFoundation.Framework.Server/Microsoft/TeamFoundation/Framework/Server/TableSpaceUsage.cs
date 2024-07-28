// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TableSpaceUsage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Index: {IndexName} ON {SchemaName}.{TableName}, RowCount: {RowCount}, PageCount: {ReservedPageCount}")]
  public class TableSpaceUsage
  {
    public string TableName { get; set; }

    public string SchemaName { get; set; }

    public string IndexName { get; set; }

    public string IndexType { get; set; }

    public int IndexId { get; set; }

    public string Compression { get; set; }

    public long RowCount { get; set; }

    public long ReservedPageCount { get; set; }

    public long UsedPageCount { get; set; }

    public long InRowReservedPageCount { get; set; }

    public long InRowUsedPageCount { get; set; }

    public long LobReservedPageCount { get; set; }

    public long LobUsedPageCount { get; set; }

    public int PartitionNumber { get; set; }

    public string PartitionBoundary { get; set; }

    public override string ToString() => string.Format("table_name:{0} schema_name:{1} index_name:{2} index_type:{3} index_id:{4} compression:{5}row_count:{6} reserved_page_count:{7} used_page_count:{8} in_row_reserved_page_count:{9} in_row_used_page_count:{10}lob_reserved_page_count:{11} lob_used_page_count:{12} partition_number:{13} partition_boundary:{14}", (object) this.TableName, (object) this.SchemaName, (object) this.IndexName, (object) this.IndexType, (object) this.IndexId, (object) this.Compression, (object) this.RowCount, (object) this.ReservedPageCount, (object) this.UsedPageCount, (object) this.InRowReservedPageCount, (object) this.InRowUsedPageCount, (object) this.LobReservedPageCount, (object) this.LobUsedPageCount, (object) this.PartitionNumber, (object) this.PartitionBoundary);
  }
}
