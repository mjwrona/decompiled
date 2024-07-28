// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ShardingInfoColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ShardingInfoColumns : ObjectBinder<ShardingInfo>
  {
    protected SqlColumnBinder MigrationId = new SqlColumnBinder(nameof (MigrationId));
    protected SqlColumnBinder StorageType = new SqlColumnBinder(nameof (StorageType));
    protected SqlColumnBinder VirtualNodes = new SqlColumnBinder(nameof (VirtualNodes));

    protected override ShardingInfo Bind() => new ShardingInfo()
    {
      MigrationId = this.MigrationId.GetGuid((IDataReader) this.Reader),
      StorageType = (Microsoft.VisualStudio.Services.Cloud.StorageType) this.StorageType.GetByte((IDataReader) this.Reader),
      VirtualNodes = this.VirtualNodes.GetInt32((IDataReader) this.Reader)
    };
  }
}
