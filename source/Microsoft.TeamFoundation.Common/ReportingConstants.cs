// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ReportingConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ReportingConstants
  {
    public const char FolderSeparatorChar = '/';
    public const string RootPath = "/";
    public const string DefaultRootItemPath = "/TfsReports";
    public const string WebService = "ReportService2005.asmx";
    public const string ObsoleteWebService = "ReportService.asmx";
    public const string BrowseItemPathUrlFormat = "{0}/Pages/Folder.aspx?ItemPath={1}";
    public const string ToolbarQueryParam = "rc:Toolbar";
    public const string RegistryWarehouseConnectionString = "/Configuration/Database/Warehouse/ConnectionString";
    public const string RegistryAnalysisConnectionString = "/Configuration/Database/AnalysisCube/ConnectionString";
    public const string RegistryIsReportingEnabled = "/Configuration/Database/Warehouse/IsReportingEnabled";
    public static readonly int DefaultAnalysisSchemaChangeCommandTimeout = 600;
    public static readonly string[] KnownWebServicePaths = new string[2]
    {
      "ReportService2005.asmx",
      "ReportService.asmx"
    };
  }
}
