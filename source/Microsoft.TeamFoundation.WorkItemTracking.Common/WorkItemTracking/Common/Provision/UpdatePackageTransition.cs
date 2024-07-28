// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageTransition
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  internal class UpdatePackageTransition
  {
    private MetaID m_fromId;
    private MetaID m_toId;
    private MetaID m_forId;
    private MetaID m_notId;

    public UpdatePackageTransition(MetaID fromId, MetaID toId, MetaID forId, MetaID notId)
    {
      this.m_fromId = fromId;
      this.m_toId = toId;
      this.m_forId = forId;
      this.m_notId = notId;
    }

    public override bool Equals(object o)
    {
      UpdatePackageTransition packageTransition = (UpdatePackageTransition) o;
      return this.m_fromId.Equals((object) packageTransition.m_fromId) && this.m_toId.Equals((object) packageTransition.m_toId) && (this.m_forId == null || this.m_forId.Equals((object) packageTransition.m_forId)) && (this.m_forId != null || packageTransition.m_forId == null) && (this.m_notId == null || this.m_notId.Equals((object) packageTransition.m_notId)) && (this.m_notId != null || packageTransition.m_notId == null);
    }

    public override int GetHashCode() => this.m_fromId.GetHashCode();
  }
}
