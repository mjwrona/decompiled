// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DefinitionEnvironmentTemplateSqlComponent3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class DefinitionEnvironmentTemplateSqlComponent3 : 
    DefinitionEnvironmentTemplateSqlComponent2
  {
    public override DefinitionEnvironmentTemplate AddEnvironmentTemplate(
      Guid projectId,
      DefinitionEnvironmentTemplate environmentTemplate)
    {
      if (environmentTemplate == null)
        throw new ArgumentNullException(nameof (environmentTemplate));
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironmentTemplate_Add", projectId);
      this.BindString("name", environmentTemplate.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("description", environmentTemplate.Description, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("environmentJson", environmentTemplate.EnvironmentJson, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironmentTemplate>((ObjectBinder<DefinitionEnvironmentTemplate>) this.GetEnvironmentTemplateBinder());
        return resultCollection.GetCurrent<DefinitionEnvironmentTemplate>().Single<DefinitionEnvironmentTemplate>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<DefinitionEnvironmentTemplate> ListEnvironmentTemplates(
      Guid projectId,
      bool isDeleted = false)
    {
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironmentTemplate_List", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironmentTemplate>((ObjectBinder<DefinitionEnvironmentTemplate>) this.GetEnvironmentTemplateBinder());
        return (IEnumerable<DefinitionEnvironmentTemplate>) resultCollection.GetCurrent<DefinitionEnvironmentTemplate>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override DefinitionEnvironmentTemplateBinder GetEnvironmentTemplateBinder() => (DefinitionEnvironmentTemplateBinder) new DefinitionEnvironmentTemplateBinder2();
  }
}
