// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.DefinitionEnvironmentTemplateSqlComponent
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
  public class DefinitionEnvironmentTemplateSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentTemplateSqlComponent>(1),
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentTemplateSqlComponent2>(2),
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentTemplateSqlComponent3>(3),
      (IComponentCreator) new ComponentCreator<DefinitionEnvironmentTemplateSqlComponent4>(4)
    }, "ReleaseManagementDefinitionEnvironmentTemplate", "ReleaseManagement");

    public virtual DefinitionEnvironmentTemplate AddEnvironmentTemplate(
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
    public virtual DefinitionEnvironmentTemplate GetEnvironmentTemplate(
      Guid projectId,
      Guid templateId,
      bool isDeleted = false)
    {
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironmentTemplate_Get", projectId);
      this.BindGuid(nameof (templateId), templateId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironmentTemplate>((ObjectBinder<DefinitionEnvironmentTemplate>) this.GetEnvironmentTemplateBinder());
        return resultCollection.GetCurrent<DefinitionEnvironmentTemplate>().Single<DefinitionEnvironmentTemplate>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "These parameters are optional")]
    public virtual IEnumerable<DefinitionEnvironmentTemplate> ListEnvironmentTemplates(
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

    public virtual void SoftDeleteEnvironmentTemplate(Guid projectId, Guid? templateId)
    {
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironmentTemplate_Delete", projectId);
      this.BindNullableGuid(nameof (templateId), templateId);
      this.ExecuteNonQuery();
    }

    public DefinitionEnvironmentTemplate UpdateEnvironmentTemplate(
      Guid projectId,
      DefinitionEnvironmentTemplate template)
    {
      if (template == null)
        throw new ArgumentNullException(nameof (template));
      this.PrepareStoredProcedure("Release.prc_DefinitionEnvironmentTemplate_Update", projectId);
      this.BindGuid("templateId", template.Id);
      this.BindString("name", template.Name, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("description", template.Description, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("environmentJson", template.EnvironmentJson, -1, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DefinitionEnvironmentTemplate>((ObjectBinder<DefinitionEnvironmentTemplate>) this.GetEnvironmentTemplateBinder());
        return resultCollection.GetCurrent<DefinitionEnvironmentTemplate>().Single<DefinitionEnvironmentTemplate>();
      }
    }

    public virtual void HardDeleteDefinitionEnvironmentTemplates(int daysToRetain)
    {
    }

    public virtual void UndeleteEnvironmentTemplate(Guid projectId, Guid templateId)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual DefinitionEnvironmentTemplateBinder GetEnvironmentTemplateBinder() => new DefinitionEnvironmentTemplateBinder();
  }
}
