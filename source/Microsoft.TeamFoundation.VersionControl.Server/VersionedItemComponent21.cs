// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionedItemComponent21
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class VersionedItemComponent21 : VersionedItemComponent20
  {
    public override void DeleteCodeMetrics(int timePeriodsToRetain)
    {
      this.PrepareStoredProcedure("Tfvc.prc_DeleteOldMetrics");
      this.BindInt("@timePeriodsToRetain", timePeriodsToRetain);
      this.ExecuteNonQuery();
    }

    public override CodeMetrics QueryCodeMetrics(
      Guid projectId,
      int startingTimeBucket,
      int endTimeBucket)
    {
      this.PrepareStoredProcedure("Tfvc.prc_GetMetrics");
      this.BindGuid("@projectId", projectId);
      this.BindInt("@timePeriod", endTimeBucket - startingTimeBucket);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      SqlColumnBinder authorsCol = new SqlColumnBinder("Authors");
      resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (dataReader => authorsCol.GetInt32(dataReader))));
      resultCollection.AddBinder<ChangesetsTrendItem>((ObjectBinder<ChangesetsTrendItem>) new CodeMetricsTrendBinder());
      int authorsCount = resultCollection.GetCurrent<int>().Items[0];
      int changesetsCount = 0;
      resultCollection.NextResult();
      List<ChangesetsTrendItem> items = resultCollection.GetCurrent<ChangesetsTrendItem>().Items;
      foreach (ChangesetsTrendItem changesetsTrendItem in items)
        changesetsCount += changesetsTrendItem.ChangesetsCount;
      return new CodeMetrics(changesetsCount, authorsCount, items);
    }
  }
}
