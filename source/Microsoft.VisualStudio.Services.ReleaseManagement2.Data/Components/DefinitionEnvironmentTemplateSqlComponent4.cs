// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DefinitionEnvironmentTemplateSqlComponent4
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
  public class DefinitionEnvironmentTemplateSqlComponent4 : 
    DefinitionEnvironmentTemplateSqlComponent3
  {
    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override IEnumerable<DefinitionEnvironmentTemplate> ListEnvironmentTemplates(
      Guid projectId,
      bool isDeleted = false)
    {
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironmentTemplate_List", projectId);
      this.BindBoolean(nameof (isDeleted), isDeleted);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironmentTemplate>((ObjectBinder<DefinitionEnvironmentTemplate>) this.GetEnvironmentTemplateBinder());
        return (IEnumerable<DefinitionEnvironmentTemplate>) resultCollection.GetCurrent<DefinitionEnvironmentTemplate>().Items;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public override DefinitionEnvironmentTemplate GetEnvironmentTemplate(
      Guid projectId,
      Guid templateId,
      bool isDeleted = false)
    {
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironmentTemplate_Get", projectId);
      this.BindGuid(nameof (templateId), templateId);
      this.BindBoolean(nameof (isDeleted), isDeleted);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironmentTemplate>((ObjectBinder<DefinitionEnvironmentTemplate>) this.GetEnvironmentTemplateBinder());
        return resultCollection.GetCurrent<DefinitionEnvironmentTemplate>().Single<DefinitionEnvironmentTemplate>();
      }
    }

    public override void SoftDeleteEnvironmentTemplate(Guid projectId, Guid? templateId)
    {
      this.PrepareStoredProcedure("Release.prc_SoftDeleteDefinitionEnvironmentTemplate", projectId);
      this.BindNullableGuid(nameof (templateId), templateId);
      this.ExecuteNonQuery();
    }

    public override void HardDeleteDefinitionEnvironmentTemplates(int daysToRetain)
    {
      this.PrepareStoredProcedure("Release.prc_HardDeleteDefinitionEnvironmentTemplates");
      this.BindInt(nameof (daysToRetain), daysToRetain);
      this.ExecuteNonQuery();
    }

    public override void UndeleteEnvironmentTemplate(Guid projectId, Guid templateId)
    {
      this.PrepareStoredProcedure("Release.prc_UndeleteReleaseDefinitionEnvironmentTemplate", projectId);
      this.BindGuid(nameof (templateId), templateId);
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected override DefinitionEnvironmentTemplateBinder GetEnvironmentTemplateBinder() => (DefinitionEnvironmentTemplateBinder) new DefinitionEnvironmentTemplateBinder2();
  }
}
