// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CodeChurnComponent4
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CodeChurnComponent4 : CodeChurnComponent3
  {
    public override ResultCollection GetChurnItemPairs(
      int changeSetId,
      string lastServerItem,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryChurnItemPairs", 3600);
      this.BindInt("@changeSetId", changeSetId);
      if (lastServerItem != null)
        this.BindString("@lastServerItem", DBPath.ServerToDatabasePath(this.ConvertToPathWithProjectGuid(lastServerItem)), -1, false, SqlDbType.NVarChar);
      this.BindInt("@batchSize", batchSize);
      ResultCollection churnItemPairs = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      churnItemPairs.AddBinder<ItemPair>((ObjectBinder<ItemPair>) new ItemPairColumns3((VersionControlSqlResourceComponent) this));
      return churnItemPairs;
    }
  }
}
