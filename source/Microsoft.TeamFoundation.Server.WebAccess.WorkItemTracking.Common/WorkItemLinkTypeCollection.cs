// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemLinkTypeCollection
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
  public class WorkItemLinkTypeCollection : ReadOnlyCollection<WorkItemLinkType>
  {
    private Dictionary<string, WorkItemLinkType> m_mapByName;
    private WorkItemLinkTypeEndCollection m_endsCollection;

    internal WorkItemLinkTypeCollection(IEnumerable<WorkItemLinkType> linkTypes)
      : base((IList<WorkItemLinkType>) linkTypes.ToList<WorkItemLinkType>())
    {
    }

    public WorkItemLinkType this[string referenceName]
    {
      get
      {
        WorkItemLinkType linkType;
        if (!this.TryGetByName(referenceName, out linkType))
          throw new LegacyDeniedOrNotExist(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.WorkItemLinkTypeNotFound, (object) referenceName));
        return linkType;
      }
    }

    public WorkItemLinkTypeEndCollection LinkTypeEnds
    {
      get
      {
        if (this.m_endsCollection == null)
          this.m_endsCollection = new WorkItemLinkTypeEndCollection(this.EnumerateEnds());
        return this.m_endsCollection;
      }
    }

    private void EnsureNameMap()
    {
      if (this.m_mapByName != null)
        return;
      this.m_mapByName = this.Items.ToDictionary<WorkItemLinkType, string>((Func<WorkItemLinkType, string>) (lt => lt.ReferenceName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public bool TryGetByName(string referenceName, out WorkItemLinkType linkType)
    {
      this.EnsureNameMap();
      return this.m_mapByName.TryGetValue(referenceName, out linkType);
    }

    private IEnumerable<WorkItemLinkTypeEnd> EnumerateEnds()
    {
      foreach (WorkItemLinkType lt in (ReadOnlyCollection<WorkItemLinkType>) this)
      {
        yield return lt.ForwardEnd;
        if (lt.IsDirectional)
          yield return lt.ReverseEnd;
      }
    }

    public IEnumerable<object> ToJson() => (IEnumerable<object>) this.Select<WorkItemLinkType, JsObject>((Func<WorkItemLinkType, JsObject>) (lt => lt.ToJson()));
  }
}
