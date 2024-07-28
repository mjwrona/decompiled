// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ReportingFolderExtensionMethods
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  internal static class ReportingFolderExtensionMethods
  {
    public static ReportingFolder CreateReportingFolder(
      this ProjectCollection projectCollection,
      ReportingServer reportingServer)
    {
      ReportingFolder child = projectCollection.CreateChild<ReportingFolder>();
      child.ReferencedResource = (CatalogObject) reportingServer;
      return child;
    }

    public static ReportingFolder CreateReportingFolder(
      this TeamProject teamProject,
      ReportingFolder tpcReportingFolder)
    {
      ReportingFolder child = teamProject.CreateChild<ReportingFolder>();
      child.ReferencedResource = (CatalogObject) tpcReportingFolder;
      return child;
    }
  }
}
