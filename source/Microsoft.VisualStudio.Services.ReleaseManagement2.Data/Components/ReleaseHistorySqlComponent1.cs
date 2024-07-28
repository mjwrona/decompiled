// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.ReleaseHistorySqlComponent1
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
  public class ReleaseHistorySqlComponent1 : ReleaseHistorySqlComponent
  {
    public override void SaveRevision(Guid projectId, ReleaseRevision releaseRevision)
    {
      if (releaseRevision == null)
        throw new ArgumentNullException(nameof (releaseRevision));
      this.PrepareStoredProcedure("Release.prc_AddReleaseHistory", projectId);
      this.BindInt("releaseId", releaseRevision.ReleaseId);
      this.BindInt("definitionSnapshotRevision", releaseRevision.DefinitionSnapshotRevision);
      this.BindGuid("changedBy", Guid.Parse(releaseRevision.ChangedBy.Id));
      this.BindDateTime("changedDate", releaseRevision.ChangedDate);
      this.BindByte("changeType", (byte) releaseRevision.ChangeType);
      this.BindString("changeDetails", releaseRevision.ChangeDetails.GetServerFormat(), 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("comment", releaseRevision.Comment, 2048, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("fileId", releaseRevision.FileId);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<ReleaseRevision> GetHistory(Guid projectId, int releaseId)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseHistory", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseRevision>((ObjectBinder<ReleaseRevision>) this.GetReleaseHistoryBinder());
        return (IEnumerable<ReleaseRevision>) resultCollection.GetCurrent<ReleaseRevision>().Items;
      }
    }

    public override ReleaseRevision GetRevision(
      Guid projectId,
      int releaseId,
      int definitionSnapshotRevision)
    {
      this.PrepareStoredProcedure("Release.prc_GetReleaseHistory", projectId);
      this.BindInt(nameof (releaseId), releaseId);
      this.BindInt(nameof (definitionSnapshotRevision), definitionSnapshotRevision);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ReleaseRevision>((ObjectBinder<ReleaseRevision>) this.GetReleaseHistoryBinder());
        return resultCollection.GetCurrent<ReleaseRevision>().Items.FirstOrDefault<ReleaseRevision>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is an override of a base class method.")]
    protected virtual ReleaseHistoryBinder GetReleaseHistoryBinder() => new ReleaseHistoryBinder();
  }
}
