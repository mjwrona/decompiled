// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.Dashboard
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class Dashboard : DashboardSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid? Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? RefreshInterval { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Position { get; set; } = -1;

    [DataMember(EmitDefaultValue = false)]
    public string ETag { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<Widget> Widgets { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid GroupId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid OwnerId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DashboardScope DashboardScope { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public DateTime ModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ModifiedBy { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public DateTime LastAccessedDate { get; set; }

    public Dashboard(IEnumerable<Widget> widgets) => this.Widgets = widgets;

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public Dashboard(string ETag) => this.ETag = ETag;

    public Dashboard()
    {
    }

    public Dashboard(DashboardGroupEntry dashboardGroupEntry)
    {
      this.ETag = dashboardGroupEntry.ETag;
      this.Id = dashboardGroupEntry.Id;
      this.Name = dashboardGroupEntry.Name;
      this.RefreshInterval = dashboardGroupEntry.RefreshInterval;
      this.Position = dashboardGroupEntry.Position;
      this.Description = dashboardGroupEntry.Description;
      this.GroupId = dashboardGroupEntry.GroupId;
      this.OwnerId = dashboardGroupEntry.OwnerId;
      this.DashboardScope = dashboardGroupEntry.DashboardScope;
      this.LastAccessedDate = dashboardGroupEntry.LastAccessedDate;
      this.ModifiedDate = dashboardGroupEntry.ModifiedDate;
      this.ModifiedBy = dashboardGroupEntry.ModifiedBy;
    }

    public bool IsNew()
    {
      if (!this.Id.HasValue)
        return true;
      Guid? id = this.Id;
      Guid empty = Guid.Empty;
      if (!id.HasValue)
        return false;
      return !id.HasValue || id.GetValueOrDefault() == empty;
    }

    protected override void SetSecuredChildren(ISecuredObject securedObject)
    {
      if (this.Widgets == null)
        return;
      List<Widget> widgetList = new List<Widget>(this.Widgets);
      foreach (DashboardSecuredObject dashboardSecuredObject in widgetList)
        dashboardSecuredObject.SetSecuredObject(securedObject);
      this.Widgets = (IEnumerable<Widget>) widgetList;
    }
  }
}
