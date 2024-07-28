// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.DashboardGroup
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class DashboardGroup : DashboardSecuredObject
  {
    [DataMember]
    public IEnumerable<DashboardGroupEntry> DashboardEntries { get; set; }

    [DataMember(IsRequired = false)]
    public GroupMemberPermission Permission { get; set; }

    [DataMember(IsRequired = false)]
    public TeamDashboardPermission TeamDashboardPermission { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public DashboardGroup()
    {
    }

    public DashboardGroup(IEnumerable<DashboardGroupEntry> entries) => this.DashboardEntries = entries;

    public DashboardGroup(
      IEnumerable<DashboardGroupEntry> entries,
      GroupMemberPermission permission)
    {
      this.DashboardEntries = entries;
      this.Permission = permission;
    }

    public DashboardGroup(IEnumerable<DashboardGroupEntry> entries, string url)
    {
      this.DashboardEntries = entries;
      this.Url = this.Url;
    }

    public DashboardGroup(
      IEnumerable<DashboardGroupEntry> entries,
      GroupMemberPermission permission,
      string url)
    {
      this.DashboardEntries = entries;
      this.Permission = permission;
      this.Url = url;
    }

    protected override void SetSecuredChildren(ISecuredObject securedObject)
    {
      if (this.DashboardEntries == null)
        return;
      foreach (DashboardSecuredObject dashboardEntry in this.DashboardEntries)
        dashboardEntry.SetSecuredObject(securedObject);
    }
  }
}
