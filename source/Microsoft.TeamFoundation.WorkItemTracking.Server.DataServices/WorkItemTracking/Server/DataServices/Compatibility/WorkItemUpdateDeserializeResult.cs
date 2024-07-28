// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateDeserializeResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  public class WorkItemUpdateDeserializeResult
  {
    private readonly Dictionary<int, WorkItemUpdateWrapper> _updates;

    internal WorkItemUpdateDeserializeResult(
      Dictionary<int, WorkItemUpdateWrapper> updates,
      XElement modifiedPackagElement)
    {
      if (updates == null)
        throw new ArgumentNullException(nameof (updates));
      if (modifiedPackagElement == null)
        throw new ArgumentNullException(nameof (modifiedPackagElement));
      this._updates = updates;
      this.ModifiedPackageElement = modifiedPackagElement;
    }

    public XElement ModifiedPackageElement { get; private set; }

    public ReadOnlyCollection<WorkItemUpdateWrapper> Updates => this._updates.Values.OrderBy<WorkItemUpdateWrapper, string>((Func<WorkItemUpdateWrapper, string>) (x => x.CorrelationId)).ToList<WorkItemUpdateWrapper>().AsReadOnly();

    public bool HasUpdates => this._updates != null && this._updates.Any<KeyValuePair<int, WorkItemUpdateWrapper>>();

    public bool HasLegacyElements => this.ModifiedPackageElement.HasElements;

    public bool BypassRules
    {
      get
      {
        if (!this.HasUpdates)
          return false;
        ReadOnlyCollection<WorkItemUpdateWrapper> updates = this.Updates;
        return updates.Where<WorkItemUpdateWrapper>((Func<WorkItemUpdateWrapper, bool>) (u => u.BypassRules.HasValue && u.BypassRules.Value)).Any<WorkItemUpdateWrapper>() && !updates.Where<WorkItemUpdateWrapper>((Func<WorkItemUpdateWrapper, bool>) (u => u.BypassRules.HasValue && !u.BypassRules.Value)).Any<WorkItemUpdateWrapper>();
      }
    }

    public bool TryGetUpdate(int id, out WorkItemUpdateWrapper value) => this._updates.TryGetValue(id, out value);
  }
}
