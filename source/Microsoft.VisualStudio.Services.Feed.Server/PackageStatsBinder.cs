// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageStatsBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageStatsBinder : ObjectBinder<PackageStats>
  {
    private SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private SqlColumnBinder activeCounts = new SqlColumnBinder("ActiveCount");
    private SqlColumnBinder pinnedCounts = new SqlColumnBinder("PinnedCount");

    protected override PackageStats Bind()
    {
      int int32_1 = this.activeCounts.GetInt32((IDataReader) this.Reader);
      int int32_2 = this.pinnedCounts.GetInt32((IDataReader) this.Reader);
      return new PackageStats(this.packageId.GetGuid((IDataReader) this.Reader, true), int32_1, int32_2);
    }
  }
}
