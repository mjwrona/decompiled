// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationJobReference
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationJobReference
  {
    public TeamFoundationJobReference()
      : this(Guid.Empty)
    {
    }

    public TeamFoundationJobReference(Guid jobId)
      : this(jobId, JobPriorityClass.None)
    {
    }

    public TeamFoundationJobReference(Guid jobId, JobPriorityClass priorityClass)
    {
      this.JobId = jobId;
      this.PriorityClass = priorityClass;
    }

    public Guid JobId { get; private set; }

    public JobPriorityClass PriorityClass { get; private set; }

    public override bool Equals(object obj) => obj is TeamFoundationJobReference foundationJobReference && this.JobId == foundationJobReference.JobId && this.PriorityClass == foundationJobReference.PriorityClass;

    public override int GetHashCode() => this.JobId.GetHashCode() ^ this.PriorityClass.GetHashCode();

    public static List<TeamFoundationJobReference> ConvertJobIdsToJobReferences(
      IEnumerable<Guid> jobIds)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(jobIds, nameof (jobIds));
      List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>();
      foreach (Guid jobId in jobIds)
        jobReferences.Add(new TeamFoundationJobReference(jobId));
      return jobReferences;
    }
  }
}
