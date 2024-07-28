// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Binders.DeletedPackageVersionBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Feed.Server.Binders
{
  public class DeletedPackageVersionBinder : IBindOnto<DeletedPackageVersion>
  {
    private static SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private static SqlColumnBinder packageName = new SqlColumnBinder("PackageName");
    private static SqlColumnBinder protocolType = new SqlColumnBinder("ProtocolType");
    private static SqlColumnBinder packageVersion = new SqlColumnBinder("PackageVersion");
    private static SqlColumnBinder deletedDate = new SqlColumnBinder("DeletedDate");
    private static SqlColumnBinder scheduledPermanentDeleteDate = new SqlColumnBinder("ScheduledPermanentDeleteDate");

    public void BindOnto(SqlDataReader reader, DeletedPackageVersion version)
    {
      version.PackageId = DeletedPackageVersionBinder.packageId.GetGuid((IDataReader) reader, false);
      version.PackageName = DeletedPackageVersionBinder.packageName.GetString((IDataReader) reader, true);
      version.Protocol = DeletedPackageVersionBinder.protocolType.GetString((IDataReader) reader, true);
      version.DisplayVersion = DeletedPackageVersionBinder.packageVersion.GetString((IDataReader) reader, true);
      version.DeletedDate = DeletedPackageVersionBinder.deletedDate.GetDateTime((IDataReader) reader);
      version.ScheduledPermanentDeleteDate = DeletedPackageVersionBinder.scheduledPermanentDeleteDate.GetDateTime((IDataReader) reader);
    }
  }
}
