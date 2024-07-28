// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Constants.PyPiPackageIngestionConstants
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

namespace Microsoft.VisualStudio.Services.PyPi.Server.Constants
{
  public class PyPiPackageIngestionConstants
  {
    public const int MetadataSchemaVersion = 1;
    public const string SourceDistPythonVersion = "source";

    public class MultipartFormKeys
    {
      public const string Action = "action";
      public const string Author = "author";
      public const string AuthorEmail = "author_email";
      public const string Blake256Digest = "blake2_256_digest";
      public const string Blake2Digest = "blake2_256_digest";
      public const string Charset = "charset";
      public const string Classifiers = "classifiers";
      public const string CommentText = "comment";
      public const string Content = "content";
      public const string Description = "description";
      public const string DescriptionContentType = "description_content_type";
      public const string DownloadUrl = "download_url";
      public const string FileName = "filename";
      public const string FileType = "filetype";
      public const string GpgSignature = "gpg_signature";
      public const string HomePage = "home_page";
      public const string Keywords = "keywords";
      public const string License = "license";
      public const string Maintainer = "maintainer";
      public const string MaintainerEmail = "maintainer_email";
      public const string Md5Digest = "md5_digest";
      public const string MetadataVersion = "metadata_version";
      public const string Name = "name";
      public const string ObsoletesDistributions = "obsoletes_dist";
      public const string Platform = "platform";
      public const string ProjectUrls = "project_urls";
      public const string ProtocolVersion = "protocol_version";
      public const string ProvidesDistributions = "provides_dist";
      public const string ProvidesExtras = "provides_extras";
      public const string PythonVersion = "pyversion";
      public const string RequiresDistributions = "requires_dist";
      public const string RequiresExternal = "requires_external";
      public const string RequiresPython = "requires_python";
      public const string Sha256Digest = "sha256_digest";
      public const string Summary = "summary";
      public const string SupportedPlatform = "supported_platform";
      public const string Variant = "variant";
      public const string Version = "version";
    }
  }
}
