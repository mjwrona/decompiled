// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.TaskTemplateStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TaskTemplateStore : ITaskTemplateStore
  {
    private readonly IList<ITaskTemplateResolver> m_resolvers;

    public TaskTemplateStore(IList<ITaskTemplateResolver> resolvers) => this.m_resolvers = (IList<ITaskTemplateResolver>) new List<ITaskTemplateResolver>((IEnumerable<ITaskTemplateResolver>) resolvers ?? Enumerable.Empty<ITaskTemplateResolver>());

    public void AddProvider(ITaskTemplateResolver resolver)
    {
      ArgumentUtility.CheckForNull<ITaskTemplateResolver>(resolver, nameof (resolver));
      this.m_resolvers.Add(resolver);
    }

    public IEnumerable<TaskStep> ResolveTasks(TaskTemplateStep step) => (IEnumerable<TaskStep>) (this.m_resolvers.FirstOrDefault<ITaskTemplateResolver>((Func<ITaskTemplateResolver, bool>) (x => x.CanResolve(step.Reference))) ?? throw new NotSupportedException(PipelineStrings.TaskTemplateNotSupported((object) step.Reference.Name, (object) step.Reference.Version))).ResolveTasks(step);
  }
}
