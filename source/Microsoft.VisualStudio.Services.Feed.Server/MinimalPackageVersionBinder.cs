// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.MinimalPackageVersionBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class MinimalPackageVersionBinder : IBindOnto<MinimalPackageVersion>
  {
    private SqlColumnBinder packageVersionId = new SqlColumnBinder("PackageVersionId");
    private SqlColumnBinder normalizedPackageVersion = new SqlColumnBinder("NormalizedPackageVersion");
    private SqlColumnBinder packageVersion = new SqlColumnBinder("PackageVersion");
    private SqlColumnBinder createdDate = new SqlColumnBinder("CreatedDate");
    private SqlColumnBinder storageId = new SqlColumnBinder("StorageId");
    private SqlColumnBinder isListed = new SqlColumnBinder("IsListed");
    private SqlColumnBinder isLatestVersion = new SqlColumnBinder("IsLatestVersion");
    private SqlColumnBinder deletedDate = new SqlColumnBinder("DeletedDate");
    private SqlColumnBinder views = new SqlColumnBinder("Views");
    private SqlColumnBinder isCachedVersion = new SqlColumnBinder("IsCachedVersion");
    private SqlColumnBinder packageDescription = new SqlColumnBinder("PackageDescription");
    private SqlColumnBinder directUpstreamSourceId = new SqlColumnBinder("DirectUpstreamSourceId");
    private readonly PackageVersionBindOptions bindOptions;

    public MinimalPackageVersionBinder(PackageVersionBindOptions bindOptions) => this.bindOptions = bindOptions;

    public void BindOnto(SqlDataReader reader, MinimalPackageVersion version)
    {
      version.Id = this.packageVersionId.GetGuid((IDataReader) reader, true);
      version.NormalizedVersion = this.normalizedPackageVersion.GetString((IDataReader) reader, true);
      version.Version = this.packageVersion.GetString((IDataReader) reader, true);
      version.IsDeleted = this.deletedDate.GetNullableDateTime((IDataReader) reader, new DateTime?()).HasValue;
      version.IsLatest = new bool?(this.isLatestVersion.GetBoolean((IDataReader) reader, false, false));
      string viewsString = this.views.GetString((IDataReader) reader, string.Empty);
      version.IsListed = this.isListed.GetBoolean((IDataReader) reader, true);
      version.StorageId = this.storageId.GetString((IDataReader) reader, (string) null);
      if (version.IsDeleted && !this.bindOptions.HasFlag((Enum) PackageVersionBindOptions.IncludeDetailsForDeletedVersions))
      {
        version.StorageId = (string) null;
        version.IsListed = false;
      }
      version.IsCachedVersion = this.isCachedVersion.GetBoolean((IDataReader) reader, false, false);
      if (this.bindOptions.HasFlag((Enum) PackageVersionBindOptions.DirectViewSerialization))
      {
        version.Views = (IEnumerable<FeedView>) FeedViewsHelper.ToViews(viewsString);
      }
      else
      {
        Dictionary<string, string> source = FeedViewsHelper.DeserializeView(viewsString);
        version.Views = source.Select<KeyValuePair<string, string>, FeedView>((System.Func<KeyValuePair<string, string>, FeedView>) (x => new FeedView()
        {
          Id = Guid.Parse(x.Key),
          Name = x.Value,
          Type = FeedViewType.Release
        }));
      }
      version.PublishDate = this.createdDate.GetNullableDateTime((IDataReader) reader, new DateTime?());
      if (this.bindOptions.HasFlag((Enum) PackageVersionBindOptions.IncludePackageDescriptionInMinimalPackageVersion))
        version.PackageDescription = this.packageDescription.GetString((IDataReader) reader, (string) null);
      Guid? nullable1 = new Guid?(this.directUpstreamSourceId.GetGuid((IDataReader) reader, true, Guid.Empty));
      Guid? nullable2 = nullable1;
      Guid empty = Guid.Empty;
      Guid? nullable3;
      if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
      {
        nullable3 = nullable1;
      }
      else
      {
        nullable2 = new Guid?();
        nullable3 = nullable2;
      }
      Guid? nullable4 = nullable3;
      version.DirectUpstreamSourceId = nullable4;
    }
  }
}
