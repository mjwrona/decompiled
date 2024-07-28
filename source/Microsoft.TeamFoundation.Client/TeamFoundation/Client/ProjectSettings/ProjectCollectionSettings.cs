// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ProjectSettings.ProjectCollectionSettings
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.ProjectSettings
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ProjectCollectionSettings
  {
    public Uri SiteCreationLocation { get; private set; }

    public Uri AdminUrlServiceLocation { get; private set; }

    public string ReportFolder { get; private set; }

    public Uri ReportServerServiceLocation { get; private set; }

    public Uri ReportsManagerServiceLocation { get; private set; }

    public string CubeDatabaseName { get; private set; }

    public string CubeServerName { get; private set; }

    public string CubeConnectionString { get; private set; }

    public string ReportWarehouseDatabaseName { get; private set; }

    public string ReportWarehoueServerName { get; private set; }

    public string ReportWarehouseConnectionString { get; private set; }

    internal ProjectCollectionSettings(
      Uri siteCreationLocation,
      Uri adminUri,
      string reportFolder,
      Uri reportServerUri,
      Uri reportManagerUri,
      string cubeDbName,
      string cubeServerName,
      string cubeConnectionString,
      string warehouseDbName,
      string warehouseServerName,
      string warehouseConnectionString)
    {
      this.SiteCreationLocation = siteCreationLocation;
      this.AdminUrlServiceLocation = adminUri;
      this.ReportFolder = reportFolder;
      this.ReportServerServiceLocation = reportServerUri;
      this.ReportsManagerServiceLocation = reportManagerUri;
      this.CubeDatabaseName = cubeDbName;
      this.CubeServerName = cubeServerName;
      this.CubeConnectionString = cubeConnectionString;
      this.ReportWarehouseDatabaseName = warehouseDbName;
      this.ReportWarehoueServerName = warehouseServerName;
      this.ReportWarehouseConnectionString = warehouseConnectionString;
    }
  }
}
