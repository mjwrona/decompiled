// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.RetentionSqlComponent2
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components
{
  public class RetentionSqlComponent2 : RetentionSqlComponent
  {
    public override IEnumerable<string> GetBuildsRetainedByReleases(
      Guid projectId,
      IEnumerable<int> releaseIds)
    {
      this.PrepareStoredProcedure("Release.prc_GetBuildsRetainedByReleases", projectId);
      this.BindInt32Table(nameof (releaseIds), releaseIds);
      this.BindString("artifactTypeId", "Build", 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<string>((ObjectBinder<string>) new BuildIdListBinder());
        return (IEnumerable<string>) resultCollection.GetCurrent<string>().Items;
      }
    }
  }
}
