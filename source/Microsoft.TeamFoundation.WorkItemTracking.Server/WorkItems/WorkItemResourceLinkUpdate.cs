// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  [System.Diagnostics.DebuggerDisplay("DebuggerDisplay,nq")]
  public class WorkItemResourceLinkUpdate : LinkUpdate
  {
    public int? ResourceId { get; set; }

    public ResourceLinkType? Type { get; set; }

    public string Location { get; set; }

    public string Name { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public int? Length { get; set; }

    protected override string DebuggerDisplay => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}, ResourceId = {1}, Type = {2}, Location = {3}, Name = {4}, CreationDate = {5}, LastModifiedDate = {6}, Length = {7}", (object) base.DebuggerDisplay, (object) this.ResourceId, (object) this.Type, (object) this.Location, (object) this.Name, (object) this.CreationDate, (object) this.LastModifiedDate, (object) this.Length);
  }
}
