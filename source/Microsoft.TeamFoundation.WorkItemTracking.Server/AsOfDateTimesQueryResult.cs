// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.AsOfDateTimesQueryResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class AsOfDateTimesQueryResult
  {
    private Lazy<IDictionary<WorkItemIdRevisionPair, IEnumerable<DateTime>>> m_lazyQueryDictionary;

    internal AsOfDateTimesQueryResult(QueryType queryType, IEnumerable<DateTime> asOfDateTimes) => this.m_lazyQueryDictionary = new Lazy<IDictionary<WorkItemIdRevisionPair, IEnumerable<DateTime>>>((Func<IDictionary<WorkItemIdRevisionPair, IEnumerable<DateTime>>>) (() => (IDictionary<WorkItemIdRevisionPair, IEnumerable<DateTime>>) this.WorkItemResults.GroupBy<AsOfQueryResultEntry, WorkItemIdRevisionPair>((Func<AsOfQueryResultEntry, WorkItemIdRevisionPair>) (qre => new WorkItemIdRevisionPair()
    {
      Id = qre.Id,
      Revision = qre.Revision
    })).ToDictionary<IGrouping<WorkItemIdRevisionPair, AsOfQueryResultEntry>, WorkItemIdRevisionPair, IEnumerable<DateTime>>((Func<IGrouping<WorkItemIdRevisionPair, AsOfQueryResultEntry>, WorkItemIdRevisionPair>) (qreg => qreg.Key), (Func<IGrouping<WorkItemIdRevisionPair, AsOfQueryResultEntry>, IEnumerable<DateTime>>) (qreg => qreg.Select<AsOfQueryResultEntry, DateTime>((Func<AsOfQueryResultEntry, DateTime>) (qre => qre.AsOfDateTime))))));

    public IEnumerable<DateTime> AsOfDateTimes { get; private set; }

    public IDictionary<WorkItemIdRevisionPair, IEnumerable<DateTime>> QueryResultsByIdRevPair => this.m_lazyQueryDictionary.Value;

    internal IEnumerable<AsOfQueryResultEntry> WorkItemResults { get; set; }
  }
}
