// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.ReleaseStatusResolver
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public static class ReleaseStatusResolver
  {
    private static readonly Dictionary<ReleaseStatus, List<ReleaseStatus>> ReleaseStateTransitionMap = new Dictionary<ReleaseStatus, List<ReleaseStatus>>()
    {
      {
        ReleaseStatus.Draft,
        new List<ReleaseStatus>() { ReleaseStatus.Active }
      },
      {
        ReleaseStatus.Active,
        new List<ReleaseStatus>() { ReleaseStatus.Abandoned }
      }
    };

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is to isolate state knowledge to this module")]
    public static Dictionary<Tuple<ReleaseStatus, ReleaseStatus>, Func<Release, string, Release>> GetReleaseOperationsMap(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      OrchestratorServiceProcessorV2 serviceProcessorV2 = new OrchestratorServiceProcessorV2(requestContext, projectId);
      return new Dictionary<Tuple<ReleaseStatus, ReleaseStatus>, Func<Release, string, Release>>()
      {
        {
          Tuple.Create<ReleaseStatus, ReleaseStatus>(ReleaseStatus.Draft, ReleaseStatus.Active),
          new Func<Release, string, Release>(serviceProcessorV2.StartRelease)
        },
        {
          Tuple.Create<ReleaseStatus, ReleaseStatus>(ReleaseStatus.Active, ReleaseStatus.Abandoned),
          new Func<Release, string, Release>(serviceProcessorV2.Abandon)
        }
      };
    }

    public static bool IsReleaseStatusUpdateAllowed(
      ReleaseStatus currentStatus,
      ReleaseStatus desiredStatus)
    {
      return ReleaseStatusResolver.ReleaseStateTransitionMap.ContainsKey(currentStatus) && ReleaseStatusResolver.ReleaseStateTransitionMap[currentStatus].Contains(desiredStatus);
    }
  }
}
