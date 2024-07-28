// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ArtifactTypeDefinitionService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ArtifactTypeDefinitionService : ArtifactTypeServiceBase
  {
    public const string VersionControlFeatureId = "ms.vss-code.version-control";

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf sdk needs it to be non-static")]
    public virtual IEnumerable<ArtifactTypeDefinition> GetArtifactTypeDefinitions(
      IVssRequestContext context,
      Guid projectId)
    {
      IList<string> exceptionList = (IList<string>) new List<string>()
      {
        "nullArtifact"
      };
      return ArtifactTypeDefinitionConverter.ConvertToContractList(this.GetArtifactTypes(context).Where<ArtifactTypeBase>((Func<ArtifactTypeBase, bool>) (artifactType => !exceptionList.Any<string>((Func<string, bool>) (x => x.Equals(artifactType.Name))))));
    }
  }
}
