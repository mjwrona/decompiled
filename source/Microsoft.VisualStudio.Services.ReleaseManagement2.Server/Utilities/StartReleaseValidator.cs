// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.StartReleaseValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class StartReleaseValidator
  {
    public static void ValidateRelease(Release serverRelease)
    {
      if (serverRelease == null)
        throw new ArgumentNullException(nameof (serverRelease));
      StartReleaseValidator.ValidateArtifactVersions(serverRelease);
    }

    private static void ValidateArtifactVersions(Release serverRelease)
    {
      foreach (ArtifactSource linkedArtifact in (IEnumerable<ArtifactSource>) serverRelease.LinkedArtifacts)
      {
        if (!linkedArtifact.SourceData.ContainsKey("version") || string.IsNullOrWhiteSpace(linkedArtifact.SourceData["version"].Value))
          throw new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions.InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ReleaseArtifactVersionIdCannotBeEmpty, (object) linkedArtifact.Alias));
      }
    }
  }
}
