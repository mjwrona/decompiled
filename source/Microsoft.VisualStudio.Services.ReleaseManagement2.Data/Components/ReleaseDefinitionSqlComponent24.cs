// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionSqlComponent24
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Versioning mechanism")]
  public class ReleaseDefinitionSqlComponent24 : ReleaseDefinitionSqlComponent23
  {
    protected override void BindIncludeVariables(bool includeVariables) => this.BindBoolean(nameof (includeVariables), includeVariables);

    public override IEnumerable<ShallowReference> QueryAllDefinitionsForDataProvider(Guid projectId)
    {
      this.PrepareStoredProcedure("Release.prc_QueryAllDefinitionsForDataProvider", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDefinition>((ObjectBinder<ReleaseDefinition>) this.GetReleaseDefinitionBinder());
        List<ReleaseDefinition> items1 = resultCollection.GetCurrent<ReleaseDefinition>().Items;
        resultCollection.AddBinder<DefinitionEnvironment>((ObjectBinder<DefinitionEnvironment>) this.GetDefinitionEnvironmentListBinder());
        resultCollection.NextResult();
        List<DefinitionEnvironment> items2 = resultCollection.GetCurrent<DefinitionEnvironment>().Items;
        return items1.Select<ReleaseDefinition, ShallowReference>((System.Func<ReleaseDefinition, ShallowReference>) (e => new ShallowReference()
        {
          Id = e.Id,
          Name = e.Name
        }));
      }
    }
  }
}
