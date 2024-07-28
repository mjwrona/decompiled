// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent10
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent10 : ProjectComponent9
  {
    internal override IList<ProjectInfo> GetProjectHistory(
      IVssRequestContext requestContext,
      long minRevision = 0)
    {
      this.PrepareStoredProcedure("prc_ProjectGetHistoryAll");
      this.BindMinRevision("@minRevision", minRevision);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        rc.AddBinder<ProjectInfo>(this.CreateProjectInfoColumnsBinder());
        this.AddProjectIdPropertyColumnsBinder(rc);
        rc.AddBinder<ProjectInfo>(this.CreateProjectHistoryColumnsBinder());
        List<ProjectInfo> items1 = rc.GetCurrent<ProjectInfo>().Items;
        List<Tuple<Guid, ProjectProperty>> propertiesForProjects = this.GetPropertiesForProjects(rc);
        rc.NextResult();
        List<ProjectInfo> items2 = rc.GetCurrent<ProjectInfo>().Items;
        Dictionary<Guid, IList<ProjectInfo>> dictionary1 = new Dictionary<Guid, IList<ProjectInfo>>();
        foreach (IGrouping<Guid, ProjectInfo> source in items2.GroupBy<ProjectInfo, Guid>((System.Func<ProjectInfo, Guid>) (project => project.Id)))
          dictionary1.Add(source.Key, (IList<ProjectInfo>) source.ToList<ProjectInfo>());
        Dictionary<Guid, List<ProjectProperty>> dictionary2 = propertiesForProjects.GroupBy<Tuple<Guid, ProjectProperty>, Guid, ProjectProperty>((System.Func<Tuple<Guid, ProjectProperty>, Guid>) (property => property.Item1), (System.Func<Tuple<Guid, ProjectProperty>, ProjectProperty>) (property => property.Item2)).ToDictionary<IGrouping<Guid, ProjectProperty>, Guid, List<ProjectProperty>>((System.Func<IGrouping<Guid, ProjectProperty>, Guid>) (group => group.Key), (System.Func<IGrouping<Guid, ProjectProperty>, List<ProjectProperty>>) (group => group.ToList<ProjectProperty>()));
        foreach (ProjectInfo projectInfo in items1)
        {
          List<ProjectProperty> projectPropertyList;
          if (dictionary2.TryGetValue(projectInfo.Id, out projectPropertyList))
            projectInfo.Properties = (IList<ProjectProperty>) projectPropertyList;
          IList<ProjectInfo> projectHistory;
          if (dictionary1.TryGetValue(projectInfo.Id, out projectHistory))
          {
            projectHistory.Add(projectInfo);
            this.ResolveProjectPropertyDeltas(requestContext, projectHistory);
          }
          else
            dictionary1.Add(projectInfo.Id, (IList<ProjectInfo>) new List<ProjectInfo>()
            {
              projectInfo
            });
        }
        return (IList<ProjectInfo>) dictionary1.Values.SelectMany<IList<ProjectInfo>, ProjectInfo>((System.Func<IList<ProjectInfo>, IEnumerable<ProjectInfo>>) (_ => (IEnumerable<ProjectInfo>) _)).OrderBy<ProjectInfo, long>((System.Func<ProjectInfo, long>) (project => project.Revision)).ToList<ProjectInfo>();
      }
    }
  }
}
