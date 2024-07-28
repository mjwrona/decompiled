// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent35
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent35 : ReleaseDefinitionSqlComponent34
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These are optional parameters")]
    public override IEnumerable<int> ListHardDeleteReleaseDefinitionCandidates(
      Guid projectId,
      DateTime? maxModifiedTime = null,
      int maxReleaseDefinitionsCount = 0,
      int continuationToken = 0)
    {
      this.PrepareStoredProcedure("Release.prc_QueryHardDeleteReleaseDefinitionCandidates", projectId);
      this.BindMaxModifiedTime(maxModifiedTime);
      this.BindMaxReleaseDefinitionsCount(nameof (maxReleaseDefinitionsCount), maxReleaseDefinitionsCount);
      this.BindInt(nameof (continuationToken), continuationToken);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new ReleaseDefinitionIdBinder());
        return (IEnumerable<int>) resultCollection.GetCurrent<int>().Items;
      }
    }

    protected override DefinitionEnvironmentListBinder GetDefinitionEnvironmentListBinder() => (DefinitionEnvironmentListBinder) new DefinitionEnvironmentListBinder3((ReleaseManagementSqlResourceComponentBase) this);
  }
}
