// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components.BuildsToStartRetainingQueueSqlComponent
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
  public class BuildsToStartRetainingQueueSqlComponent : ReleaseManagementSqlResourceComponentBase
  {
    public virtual IList<BuildsToStartRetainingData> GetBuildToStartRetentionData(Guid projectId)
    {
      this.PrepareStoredProcedure("Release.prc_GetBuildsToStartRetaining", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildsToStartRetainingData>((ObjectBinder<BuildsToStartRetainingData>) this.GetBuildToStartRetentionBinder());
        return (IList<BuildsToStartRetainingData>) resultCollection.GetCurrent<BuildsToStartRetainingData>().Items;
      }
    }

    public virtual BuildsToStartRetainingData AddBuildToStartRetentionData(
      Guid projectId,
      int buildId)
    {
      this.PrepareStoredProcedure("Release.prc_AddBuildsToStartRetaining", projectId);
      this.BindInt(nameof (buildId), buildId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildsToStartRetainingData>((ObjectBinder<BuildsToStartRetainingData>) this.GetBuildToStartRetentionBinder());
        return resultCollection.GetCurrent<BuildsToStartRetainingData>().Items.FirstOrDefault<BuildsToStartRetainingData>();
      }
    }

    public virtual BuildsToStartRetainingData DeleteBuildToStartRetentionData(
      Guid projectId,
      IList<int> buildIds)
    {
      this.PrepareStoredProcedure("Release.prc_DeleteBuildsToStartRetaining", projectId);
      this.BindInt32Table(nameof (buildIds), (IEnumerable<int>) buildIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<BuildsToStartRetainingData>((ObjectBinder<BuildsToStartRetainingData>) this.GetBuildToStartRetentionBinder());
        return resultCollection.GetCurrent<BuildsToStartRetainingData>().Items.FirstOrDefault<BuildsToStartRetainingData>();
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Required to be a method so it can be overriden.")]
    protected virtual BuildsToStartRetainingBinder GetBuildToStartRetentionBinder() => new BuildsToStartRetainingBinder((ReleaseManagementSqlResourceComponentBase) this);
  }
}
