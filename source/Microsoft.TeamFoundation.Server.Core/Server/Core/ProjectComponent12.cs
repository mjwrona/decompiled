// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent12
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent12 : ProjectComponent11
  {
    internal override IDictionary<Guid, long> GetProjectWatermarks(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      this.PrepareStoredProcedure("prc_ProjectQueryWatermarks");
      this.BindNullableGuid("@projectId", projectId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<KeyValuePair<Guid, long>>((ObjectBinder<KeyValuePair<Guid, long>>) new ProjectRevisionColumns());
        return (IDictionary<Guid, long>) resultCollection.GetCurrent<KeyValuePair<Guid, long>>().Items.GroupBy<KeyValuePair<Guid, long>, Guid, long>((System.Func<KeyValuePair<Guid, long>, Guid>) (wm => wm.Key), (System.Func<KeyValuePair<Guid, long>, long>) (wm => wm.Value)).ToDictionary<IGrouping<Guid, long>, Guid, long>((System.Func<IGrouping<Guid, long>, Guid>) (group => group.Key), (System.Func<IGrouping<Guid, long>, long>) (group => group.Single<long>()));
      }
    }

    internal override ProjectOperation UpdateProject(
      ProjectInfo project,
      Guid userIdentity,
      out ProjectInfo updatedProject)
    {
      return this.UpdateProject(project, userIdentity, out updatedProject, true);
    }
  }
}
