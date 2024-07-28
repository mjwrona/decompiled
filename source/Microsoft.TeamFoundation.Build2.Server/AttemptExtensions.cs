// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AttemptExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class AttemptExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.TimelineAttempt ToBuildTimelineAttempt(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt taskAttempt,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (taskAttempt == null)
        return (Microsoft.TeamFoundation.Build.WebApi.TimelineAttempt) null;
      return new Microsoft.TeamFoundation.Build.WebApi.TimelineAttempt(securedObject)
      {
        Attempt = taskAttempt.Attempt,
        RecordId = taskAttempt.RecordId,
        TimelineId = taskAttempt.TimelineId
      };
    }
  }
}
