// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ShallowScaledAgileViewRecordBinder2
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ShallowScaledAgileViewRecordBinder2 : ShallowScaledAgileViewRecordBinder
  {
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder ModifiedDateColumn = new SqlColumnBinder("ModifiedDate");
    private SqlColumnBinder ModifiedByColumn = new SqlColumnBinder("ModifiedBy");

    protected override ShallowScaledAgileViewRecord Bind()
    {
      ShallowScaledAgileViewRecord scaledAgileViewRecord = base.Bind();
      scaledAgileViewRecord.CreatedBy = this.CreatedByColumn.GetGuid((IDataReader) this.Reader);
      scaledAgileViewRecord.ModifiedDate = this.ModifiedDateColumn.GetDateTime((IDataReader) this.Reader);
      scaledAgileViewRecord.ModifiedBy = this.ModifiedByColumn.GetGuid((IDataReader) this.Reader);
      return scaledAgileViewRecord;
    }
  }
}
