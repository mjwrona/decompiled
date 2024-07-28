// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.CopyDashboardOptions
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class CopyDashboardOptions
  {
    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public DashboardScope CopyDashboardScope { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = true)]
    public Guid ProjectId { get; set; }

    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Guid? TeamId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? RefreshInterval { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? CopyQueriesFlag { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? QueryFolderPath { get; set; }

    public CopyDashboardOptions()
    {
    }

    public CopyDashboardOptions(CopyDashboardOptions copyDashboardOptions)
    {
      this.CopyDashboardScope = copyDashboardOptions.CopyDashboardScope;
      this.ProjectId = copyDashboardOptions.ProjectId;
      this.TeamId = copyDashboardOptions.TeamId;
      this.Name = copyDashboardOptions.Name;
      this.Description = copyDashboardOptions.Description;
      this.RefreshInterval = copyDashboardOptions.RefreshInterval;
      this.CopyQueriesFlag = copyDashboardOptions.CopyQueriesFlag;
      this.QueryFolderPath = copyDashboardOptions.QueryFolderPath;
    }

    public CopyDashboardOptions(
      DashboardScope copyDashboardScope,
      Guid projectId,
      Guid teamId,
      string name = null,
      string description = null,
      int refreshInterval = 0,
      bool? copyQueriesFlag = false,
      Guid? queryFolderPath = null)
    {
      this.CopyDashboardScope = copyDashboardScope;
      this.ProjectId = projectId;
      this.TeamId = new Guid?(teamId);
      this.Name = name;
      this.Description = description;
      this.RefreshInterval = new int?(refreshInterval);
      this.CopyQueriesFlag = copyQueriesFlag;
      this.QueryFolderPath = queryFolderPath;
    }
  }
}
