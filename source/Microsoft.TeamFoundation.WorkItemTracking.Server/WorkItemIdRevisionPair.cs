// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemIdRevisionPair
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public struct WorkItemIdRevisionPair : 
    IEquatable<WorkItemIdRevisionPair>,
    IComparable<WorkItemIdRevisionPair>
  {
    public int Id { get; set; }

    public int Revision { get; set; }

    public int Watermark { get; set; }

    public bool Equals(WorkItemIdRevisionPair other) => this.Id == other.Id && this.Revision == other.Revision;

    public override bool Equals(object obj) => obj is WorkItemIdRevisionPair other && this.Equals(other);

    public static bool operator ==(WorkItemIdRevisionPair a, WorkItemIdRevisionPair b) => a.Equals(b);

    public static bool operator !=(WorkItemIdRevisionPair a, WorkItemIdRevisionPair b) => !(a == b);

    public override int GetHashCode() => CommonUtils.CombineHashCodes(this.Id, this.Revision);

    public int CompareTo(WorkItemIdRevisionPair other)
    {
      if (this.Watermark != other.Watermark)
        return this.Watermark.CompareTo(other.Watermark);
      return this.Id != other.Id ? this.Id.CompareTo(other.Id) : this.Revision.CompareTo(other.Revision);
    }

    public override string ToString() => string.Format("Watermark:{0},Id:{1},Revision:{2}", (object) this.Watermark, (object) this.Id, (object) this.Revision);
  }
}
