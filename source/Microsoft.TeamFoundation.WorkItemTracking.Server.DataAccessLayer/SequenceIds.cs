// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SequenceIds
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class SequenceIds
  {
    public int IdentitySequenceId;
    public int IdentityGroupSequenceId;
    public int IdentityOrganizationSequenceId;

    public bool HasSequenceIdHigherThan(SequenceIds baseSeqId) => this.IdentitySequenceId > baseSeqId.IdentitySequenceId || this.IdentityGroupSequenceId > baseSeqId.IdentityGroupSequenceId || this.IdentityOrganizationSequenceId > baseSeqId.IdentityOrganizationSequenceId;

    public override string ToString() => string.Format("{{{0}, {1}, {2}}}", (object) this.IdentitySequenceId, (object) this.IdentityGroupSequenceId, (object) this.IdentityOrganizationSequenceId);
  }
}
