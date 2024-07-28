// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent6
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent6 : ProjectComponent5
  {
    internal override Tuple<IEnumerable<ProjectInfo>, IEnumerable<ProjectInfo>> QueryForUnpublishedProjects(
      IVssRequestContext requestContext,
      byte[] modifiedWatermark,
      byte[] deletedWatermark)
    {
      this.PrepareStoredProcedure("prc_QueryForUnpublishedProjects");
      this.BindBinary("@modifiedWatermark", modifiedWatermark, SqlDbType.Timestamp);
      this.BindBinary("@deletedWatermark", deletedWatermark, SqlDbType.Timestamp);
      IList<ProjectInfo> items1;
      IList<ProjectInfo> items2;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectWatermarkColumnsBinder());
        items1 = (IList<ProjectInfo>) resultCollection.GetCurrent<ProjectInfo>().Items;
        resultCollection.NextResult();
        resultCollection.AddBinder<ProjectInfo>(this.CreateProjectWatermarkColumnsBinder());
        items2 = (IList<ProjectInfo>) resultCollection.GetCurrent<ProjectInfo>().Items;
      }
      return new Tuple<IEnumerable<ProjectInfo>, IEnumerable<ProjectInfo>>((IEnumerable<ProjectInfo>) items1, (IEnumerable<ProjectInfo>) items2);
    }

    internal override ProjectInfo CreateProject(
      TeamFoundationIdentity requestingIdentity,
      Guid projectId,
      string projectName,
      string projectAbbreviation,
      string nodes,
      Guid pendingProjectGuid,
      out int nodeSeqId)
    {
      throw new NotSupportedException();
    }

    internal override void DeleteProject(Guid projectId, string userName, out int nodeSeqId) => throw new NotSupportedException();
  }
}
