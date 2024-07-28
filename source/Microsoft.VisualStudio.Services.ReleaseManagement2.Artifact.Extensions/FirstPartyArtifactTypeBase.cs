// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.FirstPartyArtifactTypeBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public abstract class FirstPartyArtifactTypeBase : ArtifactTypeBase
  {
    public const string ArtifactSourceVersionInputId = "version";
    private const string IsArtifactEditOperation = "isArtifactEditOperation";

    public static bool IsArtifactEditingMode(IDictionary<string, string> currentValues)
    {
      if (currentValues == null)
        throw new ArgumentNullException(nameof (currentValues));
      bool result;
      return currentValues.ContainsKey("isArtifactEditOperation") && bool.TryParse(currentValues["isArtifactEditOperation"], out result) && result;
    }

    public static int GetArtifactSourceVersionId(IDictionary<string, string> sourceInputValues) => sourceInputValues != null ? Convert.ToInt32(ArtifactTypeBase.GetSourceInput(sourceInputValues, "version"), (IFormatProvider) CultureInfo.CurrentCulture) : throw new ArgumentNullException(nameof (sourceInputValues));

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required to Item path.")]
    public static bool HasArtifactItemPath(
      IDictionary<string, string> sourceInputValues,
      out string artifactItemPath)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      return sourceInputValues.TryGetValue("itemPath", out artifactItemPath) && !string.IsNullOrWhiteSpace(artifactItemPath);
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required to get projectId.")]
    public static bool HasProjectInput(
      IDictionary<string, string> sourceInputValues,
      out Guid projectId)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      projectId = Guid.Empty;
      string input;
      return sourceInputValues.TryGetValue("project", out input) && Guid.TryParse(input, out projectId) && !projectId.Equals(Guid.Empty);
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required to get repositoryId.")]
    public static bool HasRepositoryInput(
      IDictionary<string, string> sourceInputValues,
      out string repositoryId)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      repositoryId = (string) null;
      return sourceInputValues.TryGetValue("repository", out repositoryId) && !repositoryId.IsNullOrEmpty<char>();
    }

    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "Required to version Id.")]
    public static bool HasArtifactSourceVersion(
      IDictionary<string, string> sourceInputValues,
      out string artifactSourceVersionId)
    {
      if (sourceInputValues == null)
        throw new ArgumentNullException(nameof (sourceInputValues));
      return sourceInputValues.TryGetValue("version", out artifactSourceVersionId) && !string.IsNullOrWhiteSpace(artifactSourceVersionId);
    }

    public static InputValue GetDetailsFromSourceInputs(
      IDictionary<string, InputValue> sourceInputs,
      string key,
      bool throwOnException,
      string errorMessage)
    {
      if (sourceInputs == null)
        throw new ArgumentNullException(nameof (sourceInputs));
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      return ArtifactTypeBase.GetSourceInput(sourceInputs, key, throwOnException, errorMessage);
    }

    public static InputValue GetArtifactSourceVersion(IDictionary<string, InputValue> sourceInputs) => sourceInputs != null ? FirstPartyArtifactTypeBase.GetDetailsFromSourceInputs(sourceInputs, "version", false, (string) null) : throw new ArgumentNullException(nameof (sourceInputs));

    public override IDictionary<string, string> GetFormatMaskTokensFromReleaseArtifactInstance(
      IDictionary<string, InputValue> sourceInputs)
    {
      Dictionary<string, string> artifactInstance = new Dictionary<string, string>(base.GetFormatMaskTokensFromReleaseArtifactInstance(sourceInputs));
      InputValue fromSourceInputs = FirstPartyArtifactTypeBase.GetDetailsFromSourceInputs(sourceInputs, "definition", false, (string) null);
      if (fromSourceInputs != null)
      {
        artifactInstance["Build.DefinitionId"] = fromSourceInputs.Value;
        artifactInstance["build.definitionName"] = fromSourceInputs.DisplayValue;
      }
      InputValue artifactSourceVersion = FirstPartyArtifactTypeBase.GetArtifactSourceVersion(sourceInputs);
      if (artifactSourceVersion != null && artifactSourceVersion.DisplayValue != null)
        artifactInstance["build.buildNumber"] = artifactSourceVersion.DisplayValue;
      return (IDictionary<string, string>) artifactInstance;
    }
  }
}
