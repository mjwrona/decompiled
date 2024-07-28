// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiFeedIndexPackageInfo
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiFeedIndexPackageInfo
  {
    public string MetadataVersion { get; set; }

    public string Name { get; set; }

    public string Version { get; set; }

    public List<string> Platforms { get; set; }

    public List<string> SupportedPlatforms { get; set; }

    public string Description { get; set; }

    public string DescriptionContentType { get; set; }

    public List<string> Keywords { get; set; }

    public string HomePage { get; set; }

    public string DownloadUrl { get; set; }

    public string Author { get; set; }

    public string AuthorEmail { get; set; }

    public string Maintainer { get; set; }

    public string MaintainerEmail { get; set; }

    public string License { get; set; }

    public List<string> Classifiers { get; set; }

    public string RequiresPython { get; set; }

    public List<string> RequiresExternal { get; set; }

    public List<string> ProjectUrls { get; set; }

    public List<string> ProvidesExtra { get; set; }

    public List<string> ProvidesDistributions { get; set; }

    public List<string> ObsoletesDistributions { get; set; }

    public PyPiFeedIndexPackageInfo(
      IReadOnlyDictionary<string, string[]> metadataFields)
    {
      this.MetadataVersion = PyPiMetadataUtils.GetOptionalMetadataField("metadata_version", metadataFields);
      this.Name = PyPiMetadataUtils.GetOptionalMetadataField("name", metadataFields);
      this.Version = PyPiMetadataUtils.GetOptionalMetadataField("version", metadataFields);
      this.Platforms = PyPiMetadataUtils.GetOptionalMetadataFieldList("platform", metadataFields);
      this.SupportedPlatforms = PyPiMetadataUtils.GetOptionalMetadataFieldList("supported_platform", metadataFields);
      this.DescriptionContentType = PyPiMetadataUtils.GetOptionalMetadataField("description_content_type", metadataFields);
      this.Description = PyPiMetadataUtils.GetOptionalMetadataField("description", metadataFields);
      this.Keywords = PyPiMetadataUtils.GetOptionalMetadataFieldList("keywords", metadataFields);
      this.HomePage = PyPiMetadataUtils.GetOptionalMetadataField("home_page", metadataFields);
      this.DownloadUrl = PyPiMetadataUtils.GetOptionalMetadataField("download_url", metadataFields);
      this.Author = PyPiMetadataUtils.GetOptionalMetadataField("author", metadataFields);
      this.AuthorEmail = PyPiMetadataUtils.GetOptionalMetadataField("author_email", metadataFields);
      this.Maintainer = PyPiMetadataUtils.GetOptionalMetadataField("maintainer", metadataFields);
      this.MaintainerEmail = PyPiMetadataUtils.GetOptionalMetadataField("maintainer_email", metadataFields);
      this.License = PyPiMetadataUtils.GetOptionalMetadataField("license", metadataFields);
      this.Classifiers = PyPiMetadataUtils.GetOptionalMetadataFieldList("classifiers", metadataFields);
      this.RequiresPython = PyPiMetadataUtils.GetOptionalMetadataField("requires_python", metadataFields);
      this.RequiresExternal = PyPiMetadataUtils.GetOptionalMetadataFieldList("requires_external", metadataFields);
      this.ProjectUrls = PyPiMetadataUtils.GetOptionalMetadataFieldList("project_urls", metadataFields);
      this.ProvidesExtra = PyPiMetadataUtils.GetOptionalMetadataFieldList("provides_extras", metadataFields);
      this.ProvidesDistributions = PyPiMetadataUtils.GetOptionalMetadataFieldList("provides_dist", metadataFields);
      this.ObsoletesDistributions = PyPiMetadataUtils.GetOptionalMetadataFieldList("obsoletes_dist", metadataFields);
    }
  }
}
