// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ShallowScaledAgileViewRecordBinder3
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ShallowScaledAgileViewRecordBinder3 : ShallowScaledAgileViewRecordBinder2
  {
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder RevisionColumn = new SqlColumnBinder("Revision");

    protected override ShallowScaledAgileViewRecord Bind()
    {
      ShallowScaledAgileViewRecord scaledAgileViewRecord = base.Bind();
      scaledAgileViewRecord.Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true);
      scaledAgileViewRecord.Revision = this.RevisionColumn.GetInt32((IDataReader) this.Reader);
      return scaledAgileViewRecord;
    }
  }
}
