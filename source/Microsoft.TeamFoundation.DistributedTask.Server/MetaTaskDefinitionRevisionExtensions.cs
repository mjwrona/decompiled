// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskDefinitionRevisionExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class MetaTaskDefinitionRevisionExtensions
  {
    public static IEnumerable<TaskGroupRevision> ResolveIdentityRefs(
      this IEnumerable<TaskGroupRevision> revisions,
      IVssRequestContext requestContext)
    {
      if (!(revisions is IList<TaskGroupRevision> taskGroupRevisionList))
        taskGroupRevisionList = (IList<TaskGroupRevision>) revisions.ToList<TaskGroupRevision>();
      IList<TaskGroupRevision> source = taskGroupRevisionList;
      IDictionary<string, IdentityRef> identities = source.Select<TaskGroupRevision, string>((Func<TaskGroupRevision, string>) (revision => revision.ChangedBy.Id)).ToHashSet<string>().QueryIdentities(requestContext);
      foreach (TaskGroupRevision taskGroupRevision in source.Where<TaskGroupRevision>((Func<TaskGroupRevision, bool>) (r => identities.ContainsKey(r.ChangedBy.Id))))
        taskGroupRevision.ChangedBy = identities[taskGroupRevision.ChangedBy.Id];
      return (IEnumerable<TaskGroupRevision>) source;
    }
  }
}
