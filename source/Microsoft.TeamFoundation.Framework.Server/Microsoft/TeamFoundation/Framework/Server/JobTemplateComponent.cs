// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.JobTemplateComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class JobTemplateComponent : JobComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<JobTemplateComponent>(1, true),
      (IComponentCreator) new ComponentCreator<JobTemplateComponent2>(2),
      (IComponentCreator) new ComponentCreator<JobTemplateComponent3>(3)
    }, "JobTemplate");

    public virtual IList<TeamFoundationJobDefinitionTemplate> QueryJobTemplates(
      bool includeDeleted,
      out long sequenceId)
    {
      sequenceId = 0L;
      return (IList<TeamFoundationJobDefinitionTemplate>) new List<TeamFoundationJobDefinitionTemplate>(0);
    }

    public virtual TeamFoundationJobDefinitionTemplate QueryJobTemplate(
      Guid jobId,
      bool includeDeleted,
      out long sequenceId)
    {
      sequenceId = 0L;
      return (TeamFoundationJobDefinitionTemplate) null;
    }

    public virtual void UpdateJobTemplates(
      IEnumerable<TeamFoundationJobDefinitionTemplate> jobTemplates)
    {
    }

    public virtual void DeleteJobTemplates(IEnumerable<Guid> jobIds)
    {
    }

    public virtual void PurgeDeletedJobTemplates()
    {
    }

    public virtual Guid StaggerPendingJobTemplates(
      Guid hostIdWatermark = default (Guid),
      long maxSequenceId = 9223372036854775807,
      int batchSize = 1000,
      int maxBatches = 2147483647,
      TimeSpan? timeout = null)
    {
      return Guid.Empty;
    }

    public virtual void StaggerJobTemplates(Guid hostId)
    {
    }
  }
}
