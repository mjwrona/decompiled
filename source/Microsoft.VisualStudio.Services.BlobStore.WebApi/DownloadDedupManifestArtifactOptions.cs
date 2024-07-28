// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DownloadDedupManifestArtifactOptions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Minimatch;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class DownloadDedupManifestArtifactOptions
  {
    public DedupIdentifier ManifestId { get; private protected set; }

    public IDictionary<string, DedupIdentifier> ArtifactNameAndManifestIds { get; private protected set; }

    public string ArtifactName { get; private protected set; }

    public string AbsoluteManifestPath { get; private protected set; }

    public string TargetDirectory { get; private protected set; }

    public IEnumerable<string> MinimatchPatterns { get; private protected set; }

    public Uri ProxyUri { get; private protected set; }

    public bool MinimatchFilterWithArtifactName { get; private protected set; }

    public Options CustomMinimatchOptions { get; private protected set; }

    private protected DownloadDedupManifestArtifactOptions()
    {
    }

    public static DownloadDedupManifestArtifactOptions CreateWithManifestId(
      DedupIdentifier manifestId,
      string targetDirectory,
      Uri proxyUri = null,
      IEnumerable<string> minimatchPatterns = null,
      string artifactName = "",
      bool minimatchFilterWithArtifactName = true,
      Options customMinimatchOptions = null)
    {
      return new DownloadDedupManifestArtifactOptions()
      {
        ManifestId = manifestId,
        TargetDirectory = targetDirectory,
        ProxyUri = proxyUri,
        MinimatchPatterns = minimatchPatterns,
        ArtifactName = artifactName,
        MinimatchFilterWithArtifactName = minimatchFilterWithArtifactName,
        CustomMinimatchOptions = customMinimatchOptions
      };
    }

    public static DownloadDedupManifestArtifactOptions CreateWithMultiManifestIds(
      IDictionary<string, DedupIdentifier> artifactNameAndManifestIds,
      string targetDirectory,
      Uri proxyUri = null,
      IEnumerable<string> minimatchPatterns = null,
      bool minimatchFilterWithArtifactName = true,
      Options customMinimatchOptions = null)
    {
      return new DownloadDedupManifestArtifactOptions()
      {
        ArtifactNameAndManifestIds = artifactNameAndManifestIds,
        TargetDirectory = targetDirectory,
        ProxyUri = proxyUri,
        MinimatchPatterns = minimatchPatterns,
        MinimatchFilterWithArtifactName = minimatchFilterWithArtifactName,
        CustomMinimatchOptions = customMinimatchOptions
      };
    }

    public static DownloadDedupManifestArtifactOptions CreateWithManifestPath(
      string absoluteManifestPath,
      string targetDirectory,
      Uri proxyUri = null,
      IEnumerable<string> minimatchPatterns = null,
      Options customMinimatchOptions = null)
    {
      return new DownloadDedupManifestArtifactOptions()
      {
        AbsoluteManifestPath = absoluteManifestPath,
        TargetDirectory = targetDirectory,
        MinimatchPatterns = minimatchPatterns,
        ProxyUri = proxyUri,
        CustomMinimatchOptions = customMinimatchOptions
      };
    }

    public void SetAbsoluteManifestPathAndRemoveManifestId(string absoluteManifestPath)
    {
      this.AbsoluteManifestPath = absoluteManifestPath;
      this.ManifestId = (DedupIdentifier) null;
    }
  }
}
