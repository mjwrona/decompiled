// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DefinitionEnvironmentSqlComponent1
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class DefinitionEnvironmentSqlComponent1 : DefinitionEnvironmentSqlComponent
  {
    public override DefinitionEnvironment GetDefinitionEnvironment(
      Guid projectId,
      int releaseDefinitionId,
      int definitionEnvironmentId)
    {
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironment_Get", projectId);
      this.BindInt(nameof (releaseDefinitionId), releaseDefinitionId);
      this.BindInt(nameof (definitionEnvironmentId), definitionEnvironmentId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentBinder());
        return resultCollection.GetCurrent<DefinitionEnvironment>().SingleOrDefault<DefinitionEnvironment>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DefinitionEnvironmentBinder GetDefinitionEnvironmentBinder() => (DefinitionEnvironmentBinder) new DefinitionEnvironmentBinder1((ReleaseManagementSqlResourceComponentBase) this);
  }
}
