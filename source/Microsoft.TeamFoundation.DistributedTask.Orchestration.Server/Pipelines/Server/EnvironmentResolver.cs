// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.EnvironmentResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class EnvironmentResolver : IEnvironmentResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;

    public EnvironmentResolver(IVssRequestContext requestContext, Guid projectId)
    {
      this.m_requestContext = requestContext;
      this.m_projectId = projectId;
    }

    public EnvironmentInstance Resolve(string environmentName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(environmentName, nameof (environmentName));
      return this.m_requestContext.GetService<IDistributedTaskEnvironmentService>().ResolveEnvironmentByName(this.m_requestContext, this.m_projectId, environmentName, EnvironmentActionFilter.Use, true, true);
    }

    public EnvironmentInstance Resolve(int environmentId)
    {
      ArgumentUtility.CheckForNonPositiveInt(environmentId, nameof (environmentId));
      IDistributedTaskEnvironmentService service = this.m_requestContext.GetService<IDistributedTaskEnvironmentService>();
      try
      {
        return service.GetEnvironmentById(this.m_requestContext, this.m_projectId, environmentId, EnvironmentActionFilter.Use, true);
      }
      catch (EnvironmentNotFoundException ex)
      {
        this.m_requestContext.TraceError("EnvironmentResolverEnvironmentNotFound", ex.Message);
        return (EnvironmentInstance) null;
      }
    }

    internal static IList<EnvironmentInstance> Resolve(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference> references)
    {
      IVssRequestContext elevatedContext = requestContext.Elevate();
      List<EnvironmentInstance> environmentInstanceList = new List<EnvironmentInstance>();
      if (references != null && references.Count > 0)
      {
        IDistributedTaskEnvironmentService service = elevatedContext.GetService<IDistributedTaskEnvironmentService>();
        List<int> ids = references.Where<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference, bool>) (x => x.Id != 0)).Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference, int>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference, int>) (x => x.Id)).ToList<int>();
        if (ids.Count > 0)
        {
          IList<EnvironmentInstance> collection = requestContext.RunSynchronously<IList<EnvironmentInstance>>((Func<Task<IList<EnvironmentInstance>>>) (async () => await service.GetEnvironmentsByIds(elevatedContext, projectId, (IList<int>) ids, EnvironmentActionFilter.Use, true)));
          environmentInstanceList.AddRange((IEnumerable<EnvironmentInstance>) collection);
        }
        List<string> list = references.Where<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference, bool>) (x => !string.IsNullOrEmpty(x.Name?.Literal))).Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference, string>((Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference, string>) (x => x.Name.Literal)).ToList<string>();
        if (list.Count > 0)
        {
          IEnumerable<EnvironmentInstance> collection = list.Select<string, EnvironmentInstance>((Func<string, EnvironmentInstance>) (name => service.ResolveEnvironmentByName(elevatedContext, projectId, name, EnvironmentActionFilter.Use, true, true)));
          environmentInstanceList.AddRange(collection);
        }
      }
      return (IList<EnvironmentInstance>) environmentInstanceList;
    }
  }
}
