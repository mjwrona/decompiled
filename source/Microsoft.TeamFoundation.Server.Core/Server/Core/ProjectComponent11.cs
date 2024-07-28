// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent11
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent11 : ProjectComponent10
  {
    internal override ProjectOperation DeleteReservedProject(Guid projectId, Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectDelete");
      this.BindGuid("@projectId", projectId);
      this.BindString("@projectState", "CreatePending", 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectOperation>((ObjectBinder<ProjectOperation>) new ProjectOperationColumns());
        return resultCollection.GetCurrent<ProjectOperation>().Items.First<ProjectOperation>();
      }
    }

    internal override ProjectOperation DeleteProject(Guid projectId, Guid userIdentity)
    {
      this.PrepareStoredProcedure("prc_ProjectDelete");
      this.BindGuid("@projectId", projectId);
      this.BindString("@projectState", (string) null, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid("@userId", userIdentity);
      this.BindGuid("@writerId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectOperation>((ObjectBinder<ProjectOperation>) new ProjectOperationColumns());
        return resultCollection.GetCurrent<ProjectOperation>().Items.First<ProjectOperation>();
      }
    }
  }
}
