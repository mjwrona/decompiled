// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionSnapshotUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class ReleaseDefinitionSnapshotUtility
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception can occur if some fields are renamed/removed from RD")]
    public static ReleaseDefinitionEnvironmentsSnapshot GetReleaseDefinitionSnapshot(
      IVssRequestContext requestContext,
      Release release,
      string definitionJson)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (string.IsNullOrWhiteSpace(definitionJson))
      {
        bool isDefaultToLatestArtifactVersionEnabled = requestContext.IsFeatureEnabled("VisualStudio.ReleaseManagement.DefaultToLatestArtifactVersion");
        Func<ReleaseDefinitionSqlComponent, ReleaseDefinition> action = (Func<ReleaseDefinitionSqlComponent, ReleaseDefinition>) (component => component.GetReleaseDefinition(release.ProjectId, release.ReleaseDefinitionId, isDefaultToLatestArtifactVersionEnabled: isDefaultToLatestArtifactVersionEnabled));
        return requestContext.ExecuteWithinUsingWithComponent<ReleaseDefinitionSqlComponent, ReleaseDefinition>(action).ToReleaseDefinitionEnvironmentSnapshot();
      }
      try
      {
        return JsonConvert.DeserializeObject<ReleaseDefinitionEnvironmentsSnapshot>(definitionJson);
      }
      catch (Exception ex)
      {
        return JsonConvert.DeserializeObject<ReleaseDefinition>(definitionJson).ToReleaseDefinitionEnvironmentSnapshot();
      }
    }

    public static string ToJson(
      ReleaseDefinitionEnvironmentsSnapshot definitionSnapshot)
    {
      return definitionSnapshot != null ? JsonConvert.SerializeObject((object) definitionSnapshot) : (string) null;
    }
  }
}
