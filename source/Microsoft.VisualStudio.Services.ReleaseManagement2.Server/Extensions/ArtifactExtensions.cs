// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ArtifactExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ArtifactExtensions
  {
    public static void PopulateReleaseArtifact(
      this ArtifactSource serverArtifact,
      IVssRequestContext context,
      Guid projectId)
    {
      if (serverArtifact == null)
        throw new ArgumentNullException(nameof (serverArtifact));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      using (ReleaseManagementTimer releaseManagementTimer = ReleaseManagementTimer.Create(context, nameof (ArtifactExtensions), "ArtifactTaskMapper.PopulateReleaseArtifacts", 1973205))
      {
        releaseManagementTimer.SetAdditionalData<string>("artifactType", serverArtifact.ArtifactTypeId);
        ArtifactTypeBase artifactType = context.GetService<ArtifactTypeServiceBase>().GetArtifactType(context, serverArtifact.ArtifactTypeId, false);
        releaseManagementTimer.RecordLap(nameof (ArtifactExtensions), "ArtifactTaskMapper.PopulateReleaseArtifacts.GetArtifactTypeComplete", 1973205);
        if (artifactType == null)
          return;
        artifactType.UpdateWebApiArtifact(serverArtifact);
        Uri sourceVersionUrl = artifactType.GetArtifactSourceVersionUrl(context, serverArtifact, projectId);
        releaseManagementTimer.RecordLap(nameof (ArtifactExtensions), "ArtifactTaskMapper.PopulateReleaseArtifacts.SourceVersionUrlFetched", 1973205);
        Dictionary<string, InputValue> dictionary = serverArtifact.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, InputValue>((Func<KeyValuePair<string, InputValue>, string>) (x => x.Key), (Func<KeyValuePair<string, InputValue>, InputValue>) (x => x.Value));
        if (sourceVersionUrl != (Uri) null)
          dictionary["artifactSourceVersionUrl"] = new InputValue()
          {
            Value = sourceVersionUrl.ToString(),
            DisplayValue = string.Empty
          };
        Uri sourceDefinitionUrl = artifactType.GetArtifactSourceDefinitionUrl(context, serverArtifact, projectId);
        if (sourceDefinitionUrl != (Uri) null)
          dictionary["artifactSourceDefinitionUrl"] = new InputValue()
          {
            Value = sourceDefinitionUrl.ToString(),
            DisplayValue = string.Empty
          };
        serverArtifact.SourceData = dictionary;
      }
    }

    public static void PopulateArtifactsWithSourceId(
      IVssRequestContext context,
      IList<ArtifactSource> serverArtifacts)
    {
      if (serverArtifacts == null)
        return;
      foreach (ArtifactSource serverArtifact in (IEnumerable<ArtifactSource>) serverArtifacts)
      {
        if (serverArtifact != null)
          ArtifactExtensions.PopulateArtifactWithSourceId(context, serverArtifact, true);
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA1704", Justification = "Variable name is meaningful.")]
    public static void PopulateArtifactWithSourceId(
      IVssRequestContext context,
      ArtifactSource artifactSource,
      bool addMultipleSourceIdsForMultiBuildArtifact)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (artifactSource == null)
        return;
      ArtifactTypeBase artifactType = context.GetService<ArtifactTypeServiceBase>().GetArtifactType(context, artifactSource.ArtifactTypeId);
      if (artifactType == null)
        return;
      artifactSource.SourceId = artifactSource.CreateArtifactSourceId(artifactType.UniqueSourceIdentifier, addMultipleSourceIdsForMultiBuildArtifact);
    }
  }
}
