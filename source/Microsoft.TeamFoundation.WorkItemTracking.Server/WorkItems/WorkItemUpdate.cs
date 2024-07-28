// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay,nq}")]
  public class WorkItemUpdate
  {
    public int Id { get; set; }

    public int Rev { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public WorkItemFieldData FieldData { get; internal set; }

    public IEnumerable<KeyValuePair<string, object>> Fields { get; internal set; }

    public IEnumerable<WorkItemLinkUpdate> LinkUpdates { get; internal set; }

    public IEnumerable<WorkItemResourceLinkUpdate> ResourceLinkUpdates { get; internal set; }

    public bool IsNew => this.Id < 1;

    public bool HasTipValues => this.FieldData != null && this.FieldData.Revision == this.Rev;

    protected virtual string DebuggerDisplay => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Id = {0}, Rev = {1}, IsNew = {2}, HasFieldUpdates = {3}, HasLinkUpdates = {4}, HasResourceLinkUpdates = {5}, HasTagUpdates = {6}", (object) this.Id, (object) this.Rev, (object) this.IsNew, (object) (bool) (this.Fields == null ? 0 : (this.Fields.Any<KeyValuePair<string, object>>() ? 1 : 0)), (object) (bool) (this.LinkUpdates == null ? 0 : (this.LinkUpdates.Any<WorkItemLinkUpdate>() ? 1 : 0)), (object) (bool) (this.ResourceLinkUpdates == null ? 0 : (this.ResourceLinkUpdates.Any<WorkItemResourceLinkUpdate>() ? 1 : 0)), (object) (bool) (this.Fields == null ? 0 : (this.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (p => StringComparer.OrdinalIgnoreCase.Equals(p.Key, "System.Tags"))) ? 1 : 0)));
  }
}
