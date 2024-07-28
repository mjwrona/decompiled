// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.MatchKey
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  internal class MatchKey : IEquatable<MatchKey>
  {
    private UpdatePackageData m_updatePackageData;

    public MatchKey(UpdatePackageData updatePackageData, string forGroup, string notGroup)
    {
      this.m_updatePackageData = updatePackageData;
      this.For = forGroup;
      this.Not = notGroup;
    }

    bool IEquatable<MatchKey>.Equals(MatchKey c)
    {
      StringComparer serverStringComparer = this.m_updatePackageData.Metadata.ServerStringComparer;
      return serverStringComparer.Compare(this.For, c.For) == 0 && serverStringComparer.Compare(this.Not, c.Not) == 0;
    }

    public override int GetHashCode() => this.For.GetHashCode();

    public string For { get; private set; }

    public string Not { get; private set; }
  }
}
