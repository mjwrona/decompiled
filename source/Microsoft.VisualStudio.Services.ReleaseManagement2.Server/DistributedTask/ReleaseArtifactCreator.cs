// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.ReleaseArtifactCreator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class ReleaseArtifactCreator
  {
    public static Uri CreateDeployPhaseUri(ReleaseArtifact releaseArtifact)
    {
      if (releaseArtifact == null)
        throw new ArgumentNullException(nameof (releaseArtifact));
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}", (object) new ArtifactUriCreator().CreateArtifactUri("Release", releaseArtifact.ProjectId.ToString("D")), (object) releaseArtifact.ReleaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) releaseArtifact.EnvironmentId, (object) releaseArtifact.ReleaseStepId, (object) releaseArtifact.ReleaseDeployPhaseId));
    }

    public static ReleaseArtifact DecodeDeployPhaseUri(Uri artifactUri)
    {
      string[] uriFragments = !(artifactUri == (Uri) null) ? new UriBuilder(artifactUri).Path.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentNullException(nameof (artifactUri));
      if (uriFragments.Length != 7)
        return ReleaseArtifactCreator.BuildArtifactV2(uriFragments);
      return !int.TryParse(uriFragments[2], out int _) ? ReleaseArtifactCreator.BuildArtifactV3(uriFragments) : ReleaseArtifactCreator.BuildArtifactV1(uriFragments);
    }

    public static Uri CreateGreenlightingUri(ReleaseArtifact artifact)
    {
      if (artifact == null)
        throw new ArgumentNullException(nameof (artifact));
      return new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}/{3}/{4}", (object) new ArtifactUriCreator().CreateArtifactUri("Release", artifact.ProjectId.ToString("D")), (object) artifact.ReleaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (object) artifact.EnvironmentId, (object) artifact.ReleaseStepId, (object) artifact.ReleaseDeployPhaseId));
    }

    public static ReleaseArtifact DecodeGreenlightingUri(Uri artifactUri)
    {
      string[] strArray = !(artifactUri == (Uri) null) ? new UriBuilder(artifactUri).Path.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries) : throw new ArgumentNullException(nameof (artifactUri));
      ReleaseArtifact releaseArtifact = new ReleaseArtifact()
      {
        ProjectId = new Guid(strArray[2]),
        ReleaseId = strArray[3].ToInvariantInt(),
        EnvironmentId = strArray[4].ToInvariantInt(),
        ReleaseStepId = strArray[5].ToInvariantInt()
      };
      if (strArray.Length == 7)
        releaseArtifact.ReleaseDeployPhaseId = strArray[6].ToInvariantInt();
      return releaseArtifact;
    }

    private static ReleaseArtifact BuildArtifactV1(string[] uriFragments) => new ReleaseArtifact()
    {
      ReleaseId = uriFragments[1].ToInvariantInt(),
      ReleaseStepId = uriFragments[2].ToInvariantInt(),
      ProjectId = new Guid(uriFragments[3]),
      EnvironmentId = uriFragments[4].ToInvariantInt()
    };

    private static ReleaseArtifact BuildArtifactV2(string[] uriFragments) => new ReleaseArtifact()
    {
      ProjectId = new Guid(uriFragments[1]),
      ReleaseId = uriFragments[2].ToInvariantInt(),
      EnvironmentId = uriFragments[3].ToInvariantInt(),
      ReleaseStepId = uriFragments[4].ToInvariantInt(),
      ReleaseDeployPhaseId = uriFragments[5].ToInvariantInt()
    };

    private static ReleaseArtifact BuildArtifactV3(string[] uriFragments) => new ReleaseArtifact()
    {
      ProjectId = new Guid(uriFragments[2]),
      ReleaseId = uriFragments[3].ToInvariantInt(),
      EnvironmentId = uriFragments[4].ToInvariantInt(),
      ReleaseStepId = uriFragments[5].ToInvariantInt(),
      ReleaseDeployPhaseId = uriFragments[6].ToInvariantInt()
    };

    private static int ToInvariantInt(this string value) => Convert.ToInt32(value, (IFormatProvider) CultureInfo.InvariantCulture);
  }
}
