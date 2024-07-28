// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters.BuildChangeConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters
{
  public static class BuildChangeConverter
  {
    public static IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change> ToReleaseChanges(
      IList<Microsoft.TeamFoundation.Build.WebApi.Change> buildChanges)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>) buildChanges.Select<Microsoft.TeamFoundation.Build.WebApi.Change, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>(BuildChangeConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseChange ?? (BuildChangeConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseChange = new Func<Microsoft.TeamFoundation.Build.WebApi.Change, Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>(BuildChangeConverter.ToReleaseChange))).ToList<Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change>();
    }

    private static Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change ToReleaseChange(
      this Microsoft.TeamFoundation.Build.WebApi.Change buildChange)
    {
      return new Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change()
      {
        Id = buildChange.Id,
        Message = buildChange.Message,
        ChangeType = buildChange.Type,
        Author = buildChange.Author,
        Timestamp = buildChange.Timestamp,
        Location = buildChange.Location,
        DisplayUri = buildChange.DisplayUri,
        Pusher = buildChange.Pusher,
        PushedBy = new IdentityRef()
        {
          Id = buildChange.Pusher
        }
      };
    }
  }
}
