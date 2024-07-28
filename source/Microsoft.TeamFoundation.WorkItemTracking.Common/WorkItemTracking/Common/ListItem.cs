// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.ListItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public struct ListItem : IEquatable<ListItem>
  {
    public int ConstId { get; set; }

    public string DisplayName { get; set; }

    public override int GetHashCode() => this.ConstId.GetHashCode();

    public override bool Equals(object obj) => obj is ListItem other && this.Equals(other);

    public bool Equals(ListItem other) => this.ConstId == other.ConstId && string.Equals(this.DisplayName, this.DisplayName, StringComparison.Ordinal);

    public static bool operator ==(ListItem op1, ListItem op2) => op1.Equals(op2);

    public static bool operator !=(ListItem op1, ListItem op2) => !op1.Equals(op2);
  }
}
