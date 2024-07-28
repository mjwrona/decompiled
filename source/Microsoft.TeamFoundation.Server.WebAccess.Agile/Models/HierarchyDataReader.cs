// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.HierarchyDataReader
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  public class HierarchyDataReader : GenericDataReader
  {
    public HierarchyDataReader(
      IEnumerable<string> columns,
      IEnumerable<IEnumerable<object>> data,
      IEnumerable<Tuple<int, int>> hierarchy = null,
      IEnumerable<int> orderedIncomingIds = null,
      IEnumerable<int> orderedOutgoingIds = null)
      : base(columns, data)
    {
      this.Hierarchy = hierarchy;
      this.OrderedIncomingIds = orderedIncomingIds;
      this.OrderedOutgoingIds = orderedOutgoingIds;
    }

    public IEnumerable<Tuple<int, int>> Hierarchy { get; private set; }

    public IEnumerable<int> OrderedIncomingIds { get; set; }

    public IEnumerable<int> OrderedOutgoingIds { get; set; }
  }
}
