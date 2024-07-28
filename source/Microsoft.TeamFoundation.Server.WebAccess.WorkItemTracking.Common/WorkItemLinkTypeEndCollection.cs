// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemLinkTypeEndCollection
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WorkItemLinkTypeEndCollection : ReadOnlyCollection<WorkItemLinkTypeEnd>
  {
    private Dictionary<string, WorkItemLinkTypeEnd> m_mapByName;
    private Dictionary<int, WorkItemLinkTypeEnd> m_mapById;

    internal WorkItemLinkTypeEndCollection(IEnumerable<WorkItemLinkTypeEnd> ends)
      : base((IList<WorkItemLinkTypeEnd>) ends.ToList<WorkItemLinkTypeEnd>())
    {
    }

    public WorkItemLinkTypeEnd this[string name]
    {
      get
      {
        WorkItemLinkTypeEnd linkTypeEnd;
        if (!this.TryGetByName(name, out linkTypeEnd))
          throw new LegacyDeniedOrNotExist(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.WorkItemLinkTypeEndNotFound, (object) name));
        return linkTypeEnd;
      }
    }

    public new WorkItemLinkTypeEnd this[int id]
    {
      get
      {
        WorkItemLinkTypeEnd linkTypeEnd;
        if (!this.TryGetById(id, out linkTypeEnd))
          throw new LegacyDeniedOrNotExist(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.WorkItemLinkTypeEndNotFound, (object) id));
        return linkTypeEnd;
      }
    }

    private void EnsureNameMap()
    {
      if (this.m_mapByName != null)
        return;
      this.m_mapByName = this.Items.ToDictionary<WorkItemLinkTypeEnd, string>((Func<WorkItemLinkTypeEnd, string>) (lte => lte.ImmutableName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (WorkItemLinkTypeEnd workItemLinkTypeEnd in (ReadOnlyCollection<WorkItemLinkTypeEnd>) this)
        this.m_mapByName[workItemLinkTypeEnd.Name] = workItemLinkTypeEnd;
    }

    public bool TryGetByName(string referenceName, out WorkItemLinkTypeEnd linkTypeEnd)
    {
      this.EnsureNameMap();
      return this.m_mapByName.TryGetValue(referenceName, out linkTypeEnd);
    }

    private void EnsureIdMap()
    {
      if (this.m_mapById != null)
        return;
      this.m_mapById = this.Items.ToDictionary<WorkItemLinkTypeEnd, int>((Func<WorkItemLinkTypeEnd, int>) (lte => lte.Id));
    }

    public bool TryGetById(int id, out WorkItemLinkTypeEnd linkTypeEnd)
    {
      this.EnsureIdMap();
      return this.m_mapById.TryGetValue(id, out linkTypeEnd);
    }
  }
}
