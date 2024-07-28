// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageChangeBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageChangeBinder : ObjectBinder<PackageChange>
  {
    private SqlColumnBinder protocolType = new SqlColumnBinder("ProtocolType");
    private SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private SqlColumnBinder normalizedPackageName = new SqlColumnBinder("NormalizedPackageName");
    private SqlColumnBinder packageName = new SqlColumnBinder("PackageName");
    private SqlColumnBinder token = new SqlColumnBinder("Token");
    private readonly IBindOnto<PackageVersion> packageVersionBinder;

    public PackageChangeBinder(IBindOnto<PackageVersion> packageVersionBinder) => this.packageVersionBinder = packageVersionBinder;

    protected override PackageChange Bind()
    {
      long int64 = this.token.GetInt64((IDataReader) this.Reader);
      Package package = new Package()
      {
        ProtocolType = this.protocolType.GetString((IDataReader) this.Reader, true),
        Id = this.packageId.GetGuid((IDataReader) this.Reader, true),
        NormalizedName = this.normalizedPackageName.GetString((IDataReader) this.Reader, true),
        Name = this.packageName.GetString((IDataReader) this.Reader, true)
      };
      PackageVersion packageVersion1 = new PackageVersion();
      this.packageVersionBinder.BindOnto(this.Reader, packageVersion1);
      PackageVersion packageVersion2 = new PackageVersion();
      packageVersion2.Id = packageVersion1.Id;
      packageVersion2.NormalizedVersion = packageVersion1.NormalizedVersion;
      packageVersion2.Version = packageVersion1.Version;
      packageVersion2.Views = packageVersion1.Views;
      packageVersion2.Description = packageVersion1.Description;
      packageVersion2.Summary = packageVersion1.Summary;
      packageVersion2.Author = packageVersion1.Author;
      packageVersion2.SourceChain = packageVersion1.SourceChain;
      packageVersion2.Tags = packageVersion1.Tags;
      packageVersion2.PublishDate = packageVersion1.PublishDate;
      packageVersion2.DeletedDate = packageVersion1.DeletedDate;
      packageVersion2.IsListed = packageVersion1.IsListed;
      packageVersion2.IsDeleted = packageVersion1.IsDeleted;
      PackageVersion packageVersion3 = packageVersion2;
      PackageVersionChange packageVersionChange = new PackageVersionChange()
      {
        PackageVersion = packageVersion3,
        ChangeType = packageVersion3.IsDeleted ? ChangeType.Delete : ChangeType.AddOrUpdate,
        ContinuationToken = int64
      };
      return new PackageChange()
      {
        Package = package,
        PackageVersionChange = packageVersionChange
      };
    }
  }
}
