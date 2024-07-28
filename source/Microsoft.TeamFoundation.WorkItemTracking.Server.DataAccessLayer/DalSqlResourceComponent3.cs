// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent3 : DalSqlResourceComponent2
  {
    public override void DeleteOrphanAttachmentsMetadata(IEnumerable<int> tfsIds)
    {
      this.PrepareStoredProcedure(nameof (DeleteOrphanAttachmentsMetadata));
      this.BindInt32Table("@tfsFileIdsTable", tfsIds);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<int> GetOrphanAttachments()
    {
      this.PrepareStoredProcedure("GetOrphanAttachmentIds");
      List<int> orphanAttachments = new List<int>();
      orphanAttachments.AddRange(WorkItemTrackingResourceComponent.Bind<int>(this.ExecuteReader(), (System.Func<IDataReader, int>) (reader => reader.GetInt32(0))));
      return (IEnumerable<int>) orphanAttachments;
    }

    private static IEnumerable<T> Bind<T>(DbDataReader reader, System.Func<DbDataReader, T> binder)
    {
      while (reader.Read())
        yield return binder(reader);
    }
  }
}
