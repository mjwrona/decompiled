// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelComponent3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class LabelComponent3 : LabelComponent2
  {
    public override ResultCollection CompareLabels(
      string startLabelName,
      string startLabelScope,
      string endLabelName,
      string endLabelScope,
      bool includeFiles,
      int minChangeSet,
      int maxChangeSets)
    {
      this.PrepareStoredProcedure("prc_CompareLabels");
      this.BindString("@startLabelName", startLabelName, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@startLabelScope", DBPath.ServerToDatabasePath(startLabelScope), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@endLabelName", endLabelName, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindString("@endLabelScope", DBPath.ServerToDatabasePath(endLabelScope), 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindBoolean("@includeFiles", includeFiles);
      this.BindInt("@minChangeSet", minChangeSet);
      this.BindInt("@maxChangeSets", maxChangeSets);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.MaxItemsPerRequest, this.ProcedureName, this.m_resultExHandler, this.RequestContext);
      resultCollection.AddBinder<Changeset>((ObjectBinder<Changeset>) new ChangesetColumns());
      if (includeFiles)
        resultCollection.AddBinder<Change>((ObjectBinder<Change>) new ChangeColumns());
      return resultCollection;
    }
  }
}
