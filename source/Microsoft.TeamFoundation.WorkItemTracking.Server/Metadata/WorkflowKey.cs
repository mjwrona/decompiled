// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkflowKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal struct WorkflowKey : IEquatable<WorkflowKey>
  {
    public string fromState;
    public string toState;
    public string reason;

    public WorkflowKey(string fromState)
    {
      this.fromState = fromState ?? "";
      this.toState = "";
      this.reason = "";
    }

    public WorkflowKey(string fromState, string toState)
    {
      this.fromState = fromState ?? "";
      this.toState = toState ?? "";
      this.reason = "";
    }

    public WorkflowKey(string fromState, string toState, string reason)
    {
      this.fromState = fromState ?? "";
      this.toState = toState ?? "";
      this.reason = reason ?? "";
    }

    public override int GetHashCode()
    {
      int hashCode1 = this.fromState == null ? 0 : this.fromState.GetHashCode();
      int hashCode2 = this.toState == null ? 0 : this.toState.GetHashCode();
      return CommonUtils.CombineHashCodes(this.reason == null ? 0 : this.reason.GetHashCode(), CommonUtils.CombineHashCodes(hashCode1, hashCode2));
    }

    public bool Equals(WorkflowKey other) => this.fromState == other.fromState && this.toState == other.toState && this.reason == other.reason;
  }
}
