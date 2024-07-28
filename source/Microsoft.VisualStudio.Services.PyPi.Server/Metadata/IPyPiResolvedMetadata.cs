// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Metadata.IPyPiResolvedMetadata
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Metadata
{
  public interface IPyPiResolvedMetadata
  {
    string ProtocolVersion { get; }

    string MetadataVersion { get; }

    string PythonVersion { get; }

    string Summary { get; }

    string DescriptionContentType { get; }

    string Description { get; }

    string Md5 { get; }

    string Sha256 { get; }

    string Blake2 { get; }

    PyPiDistType DistType { get; }

    VersionConstraintList RequiresPython { get; }

    IReadOnlyList<RequirementSpec> RequiresDistributions { get; }

    string AuthorEmail { get; }

    string MaintainerEmail { get; }

    string HomePage { get; }

    string DownloadUrl { get; }

    IReadOnlyList<string> ProjectUrls { get; }

    IReadOnlyList<RequirementSpec> ProvidesDistributions { get; }

    IReadOnlyList<string> ProvidesExtras { get; }

    IReadOnlyList<RequirementSpec> ObsoletesDistributions { get; }
  }
}
