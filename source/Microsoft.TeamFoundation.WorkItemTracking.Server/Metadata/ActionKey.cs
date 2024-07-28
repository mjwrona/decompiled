// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ActionKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal struct ActionKey : IEquatable<ActionKey>
  {
    public string fromState;
    public string toState;

    public ActionKey(string from, string to)
    {
      this.fromState = from;
      this.toState = to;
    }

    public override int GetHashCode() => CommonUtils.CombineHashCodes(this.fromState == null ? 0 : this.fromState.GetHashCode(), this.toState == null ? 0 : this.toState.GetHashCode());

    public bool Equals(ActionKey other) => this.fromState == other.fromState && this.toState == other.toState;
  }
}
