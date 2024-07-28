// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ProjectAndDefinitionIdSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ProjectAndDefinitionIdSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method.")]
    public IEnumerable<ProjectAndDefinitionId> GetDefinitionIds()
    {
      this.PrepareStoredProcedure("Release.prc_GetProjectAndReleaseDefinitionIds");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectAndDefinitionId>((ObjectBinder<ProjectAndDefinitionId>) new DefinitionIdsBinder());
        return (IEnumerable<ProjectAndDefinitionId>) resultCollection.GetCurrent<ProjectAndDefinitionId>().Items;
      }
    }
  }
}
