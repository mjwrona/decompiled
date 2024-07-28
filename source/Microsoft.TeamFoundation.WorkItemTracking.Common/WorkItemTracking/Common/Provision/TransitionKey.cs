// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.TransitionKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  internal sealed class TransitionKey
  {
    private MetaID m_fromStateId;
    private MetaID m_toStateId;

    public TransitionKey(MetaID fromStateId, MetaID toStateId)
    {
      this.m_fromStateId = fromStateId;
      this.m_toStateId = toStateId;
    }

    public override bool Equals(object obj)
    {
      TransitionKey transitionKey = obj != null ? (TransitionKey) obj : throw new ArgumentNullException(nameof (obj));
      return this.m_fromStateId == transitionKey.m_fromStateId && this.m_toStateId == transitionKey.m_toStateId;
    }

    public override int GetHashCode() => this.m_fromStateId.Value << 16 | this.m_toStateId.Value & (int) ushort.MaxValue;

    public MetaID ToStateId => this.m_toStateId;
  }
}
