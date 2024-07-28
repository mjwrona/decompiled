// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.LinkUpdateResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  [System.Diagnostics.DebuggerDisplay("DebuggerDisplay,nq")]
  public abstract class LinkUpdateResult
  {
    public int SourceWorkItemId { get; set; }

    public string CorrelationId { get; set; }

    public LinkUpdateType UpdateType { get; set; }

    public DateTime ChangedDate { get; set; }

    public int ChangeBy { get; set; }

    protected virtual string DebuggerDisplay => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SourceWorkItemId = {0}, CorrelationId = {1}, UpdateType = {2}, ChangedDate = {3}, ChangedBy = {4}", (object) this.SourceWorkItemId, (object) this.CorrelationId, (object) this.UpdateType, (object) this.ChangedDate, (object) this.ChangeBy);
  }
}
