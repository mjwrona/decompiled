// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseDefinitionHistorySqlComponent
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class ReleaseDefinitionHistorySqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "This signature is required by VSSF. Don't change.")]
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionHistorySqlComponent>(1),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionHistorySqlComponent1>(2),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionHistorySqlComponent2>(3),
      (IComponentCreator) new ComponentCreator<ReleaseDefinitionHistorySqlComponent3>(4)
    }, "ReleaseManagementReleaseDefinitionHistory", "ReleaseManagement");

    public virtual void SaveRevision(
      Guid projectId,
      ReleaseDefinition clientReleaseDefinition,
      int fileId,
      string apiVersion,
      AuditAction changeType)
    {
      if (clientReleaseDefinition == null)
        throw new ArgumentNullException(nameof (clientReleaseDefinition));
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinitionRevision_Add", projectId);
      this.BindInt("definitionId", clientReleaseDefinition.Id);
      this.BindInt("definitionRevision", clientReleaseDefinition.Revision);
      this.BindGuid("changedBy", Guid.Parse(clientReleaseDefinition.ModifiedBy.Id));
      this.BindDateTime("changedDate", clientReleaseDefinition.ModifiedOn);
      this.BindInt(nameof (fileId), fileId);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteReleaseDefinitionRevisions(
      Guid projectId,
      int definitionId,
      IEnumerable<int> revisionsToBeDeleted)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteReleaseDefinitionRevisions", projectId);
      this.BindInt(nameof (definitionId), definitionId);
      this.BindInt32Table(nameof (revisionsToBeDeleted), revisionsToBeDeleted);
      this.ExecuteNonQuery();
    }

    public virtual IEnumerable<ReleaseDefinitionRevision> GetHistory(
      Guid projectId,
      int definitionId)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinitionHistory_Get", projectId);
      this.BindInt(nameof (definitionId), definitionId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDefinitionRevision>((ObjectBinder<ReleaseDefinitionRevision>) this.GetReleaseDefinitionRevisionBinder());
        return (IEnumerable<ReleaseDefinitionRevision>) resultCollection.GetCurrent<ReleaseDefinitionRevision>().Items;
      }
    }

    public ReleaseDefinitionRevision GetRevision(Guid projectId, int definitionId, int revision)
    {
      this.PrepareStoredProcedure("Release.prc_ReleaseDefinitionRevision_Get", projectId);
      this.BindInt(nameof (definitionId), definitionId);
      this.BindInt(nameof (revision), revision);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseDefinitionRevision>((ObjectBinder<ReleaseDefinitionRevision>) this.GetReleaseDefinitionRevisionBinder());
        return resultCollection.GetCurrent<ReleaseDefinitionRevision>().Items.FirstOrDefault<ReleaseDefinitionRevision>();
      }
    }

    public void DeleteHistory(Guid projectId, int definitionId)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteReleaseDefinitionHistory", projectId);
      this.BindInt(nameof (definitionId), definitionId);
      this.ExecuteNonQuery();
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseDefinitionRevisionBinder GetReleaseDefinitionRevisionBinder() => new ReleaseDefinitionRevisionBinder();
  }
}
