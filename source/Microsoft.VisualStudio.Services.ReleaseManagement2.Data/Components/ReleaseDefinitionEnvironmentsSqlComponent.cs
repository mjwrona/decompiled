// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionEnvironmentsSqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseDefinitionEnvironmentsSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionEnvironmentsSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionEnvironmentsSqlComponent1>(2)
    }, "ReleaseManagementReleaseDefinitionEnvironments", "ReleaseManagement");

    public virtual IEnumerable<DefinitionEnvironmentReference> GetReleaseDefinitionEnvironments(
      Guid projectId,
      Guid? taskGroupId,
      IEnumerable<int> definitionEnvironmentIds)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseDefinitionEnvironments", projectId);
      this.BindNullableGuid("@taskGroupId", taskGroupId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDefinitionEnvironmentsData>((ObjectBinder<ReleaseDefinitionEnvironmentsData>) new ReleaseDefinitionEnvironmentsBinder((ReleaseManagementSqlResourceComponentBase) this));
        return (IEnumerable<DefinitionEnvironmentReference>) resultCollection.GetCurrent<ReleaseDefinitionEnvironmentsData>().Items.Select<ReleaseDefinitionEnvironmentsData, DefinitionEnvironmentReference>((System.Func<ReleaseDefinitionEnvironmentsData, DefinitionEnvironmentReference>) (r => r.GetDefinitionEnvironmentReference())).ToList<DefinitionEnvironmentReference>();
      }
    }
  }
}
