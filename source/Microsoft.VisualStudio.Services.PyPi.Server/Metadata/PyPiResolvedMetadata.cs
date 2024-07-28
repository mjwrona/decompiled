// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.PyPiResolvedMetadata
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using Pegasus.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public class PyPiResolvedMetadata : IPyPiResolvedMetadata
  {
    private readonly IReadOnlyDictionary<string, string[]> metadataFields;
    private static readonly TimeSpan ParseTimeout = TimeSpan.FromSeconds(1.0);

    private PyPiResolvedMetadata(
      IReadOnlyDictionary<string, string[]> metadataFields)
    {
      this.metadataFields = metadataFields;
      this.Parse();
    }

    private void Parse()
    {
      PyPiDistType result;
      Enum.TryParse<PyPiDistType>(this.GetRequiredMetadataField("filetype"), out result);
      this.DistType = result != PyPiDistType.Unknown ? result : throw new InvalidPackageException(Resources.Error_MissingIngestionMetadata((object) "filetype"));
      this.ProtocolVersion = this.GetRequiredMetadataField("protocol_version");
      this.MetadataVersion = this.GetRequiredMetadataField("metadata_version");
      this.Summary = this.GetOptionalMetadataField("summary");
      this.DescriptionContentType = this.GetOptionalMetadataField("description_content_type");
      this.Description = this.GetOptionalMetadataField("description");
      this.HomePage = this.GetOptionalMetadataField("home_page");
      this.DownloadUrl = this.GetOptionalMetadataField("download_url");
      this.AuthorEmail = this.GetOptionalMetadataField("author_email");
      this.MaintainerEmail = this.GetOptionalMetadataField("maintainer_email");
      this.PythonVersion = this.GetOptionalMetadataField("pyversion");
      if (result == PyPiDistType.sdist && this.PythonVersion.IsNullOrEmpty<char>())
        this.PythonVersion = "source";
      this.Sha256 = this.GetOptionalMetadataField("sha256_digest");
      this.Md5 = this.GetOptionalMetadataField("md5_digest");
      this.Blake2 = this.GetOptionalMetadataField("blake2_256_digest");
      string optionalMetadataField = this.GetOptionalMetadataField("requires_python");
      this.RequiresPython = !string.IsNullOrWhiteSpace(optionalMetadataField) ? this.ParseVersionConstraintList(optionalMetadataField) : (VersionConstraintList) null;
      List<string> metadataFieldList1 = PyPiMetadataUtils.GetOptionalMetadataFieldList("requires_dist", this.metadataFields);
      this.RequiresDistributions = !metadataFieldList1.IsNullOrEmpty<string>() ? this.GetRequirementSpecList(metadataFieldList1) : (IReadOnlyList<RequirementSpec>) null;
      List<string> metadataFieldList2 = PyPiMetadataUtils.GetOptionalMetadataFieldList("project_urls", this.metadataFields);
      this.ProjectUrls = !metadataFieldList2.IsNullOrEmpty<string>() ? (IReadOnlyList<string>) metadataFieldList2.ToImmutableList<string>() : (IReadOnlyList<string>) null;
      List<string> metadataFieldList3 = PyPiMetadataUtils.GetOptionalMetadataFieldList("provides_extras", this.metadataFields);
      this.ProvidesExtras = !metadataFieldList3.IsNullOrEmpty<string>() ? (IReadOnlyList<string>) metadataFieldList3.ToImmutableList<string>() : (IReadOnlyList<string>) null;
      List<string> metadataFieldList4 = PyPiMetadataUtils.GetOptionalMetadataFieldList("provides_dist", this.metadataFields);
      this.ProvidesDistributions = !metadataFieldList4.IsNullOrEmpty<string>() ? this.GetRequirementSpecList(metadataFieldList4) : (IReadOnlyList<RequirementSpec>) null;
      List<string> metadataFieldList5 = PyPiMetadataUtils.GetOptionalMetadataFieldList("obsoletes_dist", this.metadataFields);
      this.ObsoletesDistributions = !metadataFieldList5.IsNullOrEmpty<string>() ? this.GetRequirementSpecList(metadataFieldList5) : (IReadOnlyList<RequirementSpec>) null;
    }

    public string ProtocolVersion { get; private set; }

    public string MetadataVersion { get; private set; }

    public string PythonVersion { get; private set; }

    public string Summary { get; private set; }

    public string DescriptionContentType { get; private set; }

    public string Description { get; private set; }

    public string Md5 { get; private set; }

    public string Sha256 { get; private set; }

    public string Blake2 { get; private set; }

    public PyPiDistType DistType { get; private set; }

    public VersionConstraintList RequiresPython { get; private set; }

    public IReadOnlyList<RequirementSpec> RequiresDistributions { get; private set; }

    public string AuthorEmail { get; private set; }

    public string MaintainerEmail { get; private set; }

    public string HomePage { get; private set; }

    public IReadOnlyList<string> ProjectUrls { get; private set; }

    public string DownloadUrl { get; private set; }

    public IReadOnlyList<RequirementSpec> ProvidesDistributions { get; private set; }

    public IReadOnlyList<string> ProvidesExtras { get; private set; }

    public IReadOnlyList<RequirementSpec> ObsoletesDistributions { get; private set; }

    public static PyPiResolvedMetadata ParseFrom(
      IReadOnlyDictionary<string, string[]> metadataFields)
    {
      return new PyPiResolvedMetadata(metadataFields);
    }

    private string GetOptionalMetadataField(string key) => PyPiMetadataUtils.GetOptionalMetadataField(key, this.metadataFields);

    private string GetRequiredMetadataField(string key) => PyPiMetadataUtils.GetRequiredMetadataField(key, this.metadataFields);

    private RequirementSpec ParseRequirementSpec(string input) => PyPiResolvedMetadata.ParseCore<RequirementSpec>(input, PyPiResolvedMetadata.\u003C\u003EO.\u003C0\u003E__ParseRequirement ?? (PyPiResolvedMetadata.\u003C\u003EO.\u003C0\u003E__ParseRequirement = new Func<string, TimeSpan, string, ITracer, RequirementSpec>(RequirementParser.ParseRequirement)));

    private VersionConstraintList ParseVersionConstraintList(string input) => PyPiResolvedMetadata.ParseCore<VersionConstraintList>(input, PyPiResolvedMetadata.\u003C\u003EO.\u003C1\u003E__ParseVersionConstraintList ?? (PyPiResolvedMetadata.\u003C\u003EO.\u003C1\u003E__ParseVersionConstraintList = new Func<string, TimeSpan, string, ITracer, VersionConstraintList>(RequirementParser.ParseVersionConstraintList)));

    private IReadOnlyList<RequirementSpec> GetRequirementSpecList(List<string> requiredDistStrings)
    {
      IList<RequirementSpec> list = (IList<RequirementSpec>) requiredDistStrings.Where<string>((Func<string, bool>) (distString => !string.IsNullOrEmpty(distString))).Select<string, RequirementSpec>(new Func<string, RequirementSpec>(this.ParseRequirementSpec)).ToList<RequirementSpec>();
      return list.Count == 0 ? (IReadOnlyList<RequirementSpec>) null : (IReadOnlyList<RequirementSpec>) list.ToImmutableList<RequirementSpec>();
    }

    private static T ParseCore<T>(string input, Func<string, TimeSpan, string, ITracer, T> parser)
    {
      try
      {
        return parser(input, PyPiResolvedMetadata.ParseTimeout, (string) null, (ITracer) null);
      }
      catch (RequirementParseException ex)
      {
        throw new InvalidPackageException(Resources.Error_PackageContainsInvalidRequirement((object) ex.Message), (Exception) ex);
      }
      catch (OperationCanceledException ex)
      {
        throw new InvalidPackageException(Resources.Error_RequirementParseTookTooLong(), (Exception) ex);
      }
    }
  }
}
