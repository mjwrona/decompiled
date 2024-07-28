// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent20
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent20 : WorkItemTypeExtensionComponent19
  {
    public override List<Tuple<string, DateTime>> GetWorkItemTypeletsReferenceNamesDeletedSince(
      DateTime deletedSince,
      int typeletType,
      out DateTime asOf)
    {
      asOf = DateTime.MinValue;
      this.PrepareStoredProcedure("prc_GetWorkItemTypeletsDeletedSince");
      this.BindDateTime("@deletedSince", deletedSince);
      this.BindInt("@typeletType", typeletType);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<string, DateTime>>((ObjectBinder<Tuple<string, DateTime>>) new TupleBinder<string, DateTime>());
      resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new DateTimeBinder());
      List<Tuple<string, DateTime>> items = resultCollection.GetCurrent<Tuple<string, DateTime>>().Items;
      resultCollection.NextResult();
      ObjectBinder<DateTime> current = resultCollection.GetCurrent<DateTime>();
      current.MoveNext();
      asOf = current.Current;
      return items;
    }
  }
}
