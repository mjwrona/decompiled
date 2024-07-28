// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent1740
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent1740 : FileComponent1690
  {
    public override FileIdUsage GetFileIdUsage()
    {
      this.PrepareStoredProcedure("prc_GetFileIdUsage", 1800);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<FileIdUsage>((ObjectBinder<FileIdUsage>) new FileComponent1740.FileIdUsageBinder());
      return resultCollection.GetCurrent<FileIdUsage>().Single<FileIdUsage>();
    }

    internal class FileIdUsageBinder : ObjectBinder<FileIdUsage>
    {
      protected SqlColumnBinder MinFileIdColumn = new SqlColumnBinder("MinFileId");
      protected SqlColumnBinder MaxFileIdColumn = new SqlColumnBinder("MaxFileId");
      protected SqlColumnBinder ReusableNegativeIdsColumn = new SqlColumnBinder("ReusableNegativeIds");

      protected override FileIdUsage Bind() => new FileIdUsage()
      {
        MaxNegativeId = (long) this.MinFileIdColumn.GetInt32((IDataReader) this.Reader),
        MaxPositiveId = (long) this.MaxFileIdColumn.GetInt32((IDataReader) this.Reader),
        ReusableNegativeIds = (long) this.ReusableNegativeIdsColumn.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
