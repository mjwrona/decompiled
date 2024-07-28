// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.SyncDateColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  internal class SyncDateColumns : ObjectBinder<ProviderSyncData>
  {
    private SqlColumnBinder ModelTableColumn = new SqlColumnBinder("ModelTableName");
    private SqlColumnBinder SourceTableColumn = new SqlColumnBinder("ProviderTableName");
    private SqlColumnBinder SyncDateColumn = new SqlColumnBinder("ProviderSyncDate");

    protected override ProviderSyncData Bind() => new ProviderSyncData()
    {
      ModelTableName = this.ModelTableColumn.GetString((IDataReader) this.Reader, false),
      ProviderTableName = this.SourceTableColumn.GetString((IDataReader) this.Reader, false),
      ProviderSyncDate = this.SyncDateColumn.GetNullableDateTime((IDataReader) this.Reader)
    };
  }
}
