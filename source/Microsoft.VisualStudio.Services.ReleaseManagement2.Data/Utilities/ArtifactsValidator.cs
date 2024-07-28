// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ArtifactsValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ArtifactsValidator
  {
    public static void ValidateArtifactDownloadInput(
      this ArtifactDownloadInputBase artifactDownloadInput,
      string deployPhaseName,
      IDictionary<string, string> linkedArtifactAliasToArtifactTypeMap)
    {
      if (artifactDownloadInput == null)
        throw new ArgumentNullException(nameof (artifactDownloadInput));
      if (linkedArtifactAliasToArtifactTypeMap == null)
        throw new ArgumentNullException(nameof (linkedArtifactAliasToArtifactTypeMap));
      if (!linkedArtifactAliasToArtifactTypeMap.ContainsKey(artifactDownloadInput.Alias) || string.IsNullOrWhiteSpace(linkedArtifactAliasToArtifactTypeMap[artifactDownloadInput.Alias]))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidArtifactAliasInDeploymentInput, (object) artifactDownloadInput.Alias, (object) deployPhaseName));
      if (!linkedArtifactAliasToArtifactTypeMap[artifactDownloadInput.Alias].Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidArtifactTypeInDeploymentInput, (object) artifactDownloadInput.ArtifactType, (object) deployPhaseName));
      string[] source = new string[2]{ "All", "Skip" };
      List<string> stringList = new List<string>();
      if (artifactDownloadInput.IsSelectiveArtifactsDownloadSupported())
      {
        source = new string[3]{ "All", "Selective", "Skip" };
        stringList.AddRange((IEnumerable<string>) artifactDownloadInput.ArtifactItems);
      }
      if (!((IEnumerable<string>) source).Any<string>((Func<string, bool>) (mode => mode.Equals(artifactDownloadInput.ArtifactDownloadMode, StringComparison.OrdinalIgnoreCase))))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidArtifactDownloadModeInDeploymentInput, (object) deployPhaseName, (object) artifactDownloadInput.ArtifactDownloadMode, (object) artifactDownloadInput.ArtifactType, (object) string.Join(", ", source)));
      if (!((IEnumerable<string>) source).Any<string>((Func<string, bool>) (mode => mode.Equals(artifactDownloadInput.ArtifactDownloadMode, StringComparison.OrdinalIgnoreCase))))
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidArtifactDownloadModeInDeploymentInput, (object) deployPhaseName, (object) artifactDownloadInput.ArtifactDownloadMode, (object) artifactDownloadInput.ArtifactType, (object) string.Join(", ", source)));
      if (("All".Equals(artifactDownloadInput.ArtifactDownloadMode, StringComparison.OrdinalIgnoreCase) || "Skip".Equals(artifactDownloadInput.ArtifactDownloadMode, StringComparison.OrdinalIgnoreCase)) && !stringList.IsNullOrEmpty<string>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidSelectiveArtifactsInDeploymentInput, (object) deployPhaseName, (object) string.Join(", ", (IEnumerable<string>) stringList), (object) artifactDownloadInput.Alias, (object) artifactDownloadInput.ArtifactDownloadMode));
      if ("Selective".Equals(artifactDownloadInput.ArtifactDownloadMode, StringComparison.OrdinalIgnoreCase) && stringList.IsNullOrEmpty<string>())
        throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.SelectiveArtifactsCannotBeEmptyInDeploymentInput, (object) deployPhaseName, (object) artifactDownloadInput.Alias, (object) artifactDownloadInput.ArtifactDownloadMode));
    }

    public static bool IsNonTaskifiedArtifact(
      this ArtifactDownloadInputBase artifactDownloadInput)
    {
      if (artifactDownloadInput == null)
        throw new ArgumentNullException(nameof (artifactDownloadInput));
      return "Git".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) || "GitHub".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) || "TFVC".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsSelectiveArtifactsDownloadSupported(
      this ArtifactDownloadInputBase artifactDownloadInput)
    {
      if (artifactDownloadInput == null)
        throw new ArgumentNullException(nameof (artifactDownloadInput));
      return !"Git".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"GitHub".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"TFVC".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"Nuget".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"ExternalGit".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"Svn".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"PackageManagement".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"DockerHub".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase) && !"AzureContainerRepository".Equals(artifactDownloadInput.ArtifactType, StringComparison.OrdinalIgnoreCase);
    }
  }
}
