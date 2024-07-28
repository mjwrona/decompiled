// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Binders.RecycleBinPackageVersionBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Feed.Server.Binders
{
  public class RecycleBinPackageVersionBinder : IBindOnto<RecycleBinPackageVersion>
  {
    private readonly IBindOnto<PackageVersion> packageVersionBinder;
    private SqlColumnBinder scheduledPermanentDeleteDate = new SqlColumnBinder("ScheduledPermanentDeleteDate");

    public RecycleBinPackageVersionBinder(IBindOnto<PackageVersion> packageVersionBinder) => this.packageVersionBinder = packageVersionBinder;

    public void BindOnto(SqlDataReader reader, RecycleBinPackageVersion version)
    {
      this.packageVersionBinder.BindOnto(reader, (PackageVersion) version);
      version.ScheduledPermanentDeleteDate = this.scheduledPermanentDeleteDate.GetNullableDateTime((IDataReader) reader);
    }
  }
}
