// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseDefinitionEnvironmentsSnapshotExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseDefinitionEnvironmentsSnapshotExtensions
  {
    public static IEnumerable<DefinitionEnvironmentData> GetNonEditableEnvironments(
      this ReleaseDefinitionEnvironmentsSnapshot releaseDefinitionEnvironmentsSnapshot,
      Release serverRelease)
    {
      if (releaseDefinitionEnvironmentsSnapshot == null)
        throw new ArgumentNullException(nameof (releaseDefinitionEnvironmentsSnapshot));
      IEnumerable<int> editableEnvironmentIds = serverRelease != null ? serverRelease.GetEditableEnvironmentDefinitionEnvironmentIds() : throw new ArgumentNullException(nameof (serverRelease));
      return releaseDefinitionEnvironmentsSnapshot.Environments.Where<DefinitionEnvironmentData>((Func<DefinitionEnvironmentData, bool>) (env => !editableEnvironmentIds.Contains<int>(env.Id)));
    }
  }
}
