// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageVersionBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class PackageVersionBinder : IBindOnto<PackageVersion>
  {
    private SqlColumnBinder packageDescription = new SqlColumnBinder("PackageDescription");
    private SqlColumnBinder packageSummary = new SqlColumnBinder("PackageSummary");
    private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder tags = new SqlColumnBinder("Tags");
    private SqlColumnBinder dependencies = new SqlColumnBinder("Dependencies");
    private SqlColumnBinder protocolMetadata = new SqlColumnBinder("ProtocolMetadata");
    private SqlColumnBinder deletedDate = new SqlColumnBinder("DeletedDate");
    private SqlColumnBinder updateSequenceNumber = new SqlColumnBinder("UpdateSequenceNumber");
    private SqlColumnBinder files = new SqlColumnBinder("Files");
    private SqlColumnBinder sourceChain = new SqlColumnBinder("SourceChain");
    private readonly PackageVersionBindOptions bindOptions;
    private readonly IBindOnto<MinimalPackageVersion> minimalPackageVersionBinder;

    public PackageVersionBinder(
      PackageVersionBindOptions bindOptions,
      IBindOnto<MinimalPackageVersion> minimalPackageVersionBinder)
    {
      this.bindOptions = bindOptions;
      this.minimalPackageVersionBinder = minimalPackageVersionBinder;
    }

    public void BindOnto(SqlDataReader reader, PackageVersion version)
    {
      this.minimalPackageVersionBinder.BindOnto(reader, (MinimalPackageVersion) version);
      version.DeletedDate = this.deletedDate.GetNullableDateTime((IDataReader) reader, new DateTime?());
      version.UpdateSequenceNumber = this.updateSequenceNumber.GetInt64((IDataReader) reader, 0L, 0L);
      if ((this.bindOptions & PackageVersionBindOptions.IncludePackageDescriptionInMinimalPackageVersion) == PackageVersionBindOptions.None)
        version.Description = this.packageDescription.GetString((IDataReader) reader, (string) null);
      string str1 = this.tags.GetString((IDataReader) reader, (string) null);
      IEnumerable<string> strings = string.IsNullOrEmpty(str1) ? (IEnumerable<string>) null : JsonConvert.DeserializeObject<IEnumerable<string>>(str1);
      string str2 = this.dependencies.GetString((IDataReader) reader, (string) null);
      IEnumerable<PackageDependency> packageDependencies = string.IsNullOrEmpty(str2) ? (IEnumerable<PackageDependency>) null : JsonConvert.DeserializeObject<IEnumerable<PackageDependency>>(str2);
      string str3 = this.protocolMetadata.GetString((IDataReader) reader, (string) null);
      ProtocolMetadata protocolMetadata = str3 == null ? (ProtocolMetadata) null : JsonConvert.DeserializeObject<ProtocolMetadata>(str3);
      string str4 = this.files.GetString((IDataReader) reader, (string) null);
      IEnumerable<PackageFile> packageFiles = str4 == null ? (IEnumerable<PackageFile>) null : JsonConvert.DeserializeObject<IEnumerable<PackageFile>>(str4);
      string str5 = this.sourceChain.GetString((IDataReader) reader, (string) null);
      IEnumerable<UpstreamSource> upstreamSources = str5 == null ? (IEnumerable<UpstreamSource>) new List<UpstreamSource>() : JsonConvert.DeserializeObject<IEnumerable<UpstreamSource>>(str5);
      version.Summary = this.packageSummary.GetString((IDataReader) reader, (string) null);
      version.Author = this.createdBy.GetString((IDataReader) reader, (string) null);
      version.Tags = strings;
      version.Dependencies = packageDependencies;
      version.ProtocolMetadata = protocolMetadata;
      version.Files = packageFiles;
      version.SourceChain = upstreamSources;
    }
  }
}
