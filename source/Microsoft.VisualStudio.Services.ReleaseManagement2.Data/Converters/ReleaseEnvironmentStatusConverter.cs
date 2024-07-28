// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleaseEnvironmentStatusConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleaseEnvironmentStatusConverter
  {
    private static readonly IDictionary<EnvironmentStatus, ReleaseEnvironmentStatus> WebApiToServerStatusMap = (IDictionary<EnvironmentStatus, ReleaseEnvironmentStatus>) new Dictionary<EnvironmentStatus, ReleaseEnvironmentStatus>()
    {
      {
        EnvironmentStatus.NotStarted,
        ReleaseEnvironmentStatus.NotStarted
      },
      {
        EnvironmentStatus.Succeeded,
        ReleaseEnvironmentStatus.Succeeded
      },
      {
        EnvironmentStatus.Rejected,
        ReleaseEnvironmentStatus.Rejected
      },
      {
        EnvironmentStatus.InProgress,
        ReleaseEnvironmentStatus.InProgress
      },
      {
        EnvironmentStatus.Canceled,
        ReleaseEnvironmentStatus.Canceled
      },
      {
        EnvironmentStatus.Queued,
        ReleaseEnvironmentStatus.Queued
      },
      {
        EnvironmentStatus.Scheduled,
        ReleaseEnvironmentStatus.Scheduled
      },
      {
        EnvironmentStatus.Undefined,
        ReleaseEnvironmentStatus.Undefined
      },
      {
        EnvironmentStatus.PartiallySucceeded,
        ReleaseEnvironmentStatus.PartiallySucceeded
      }
    };
    private static readonly IDictionary<ReleaseEnvironmentStatus, EnvironmentStatus> ServerToWebApiStatusMap = (IDictionary<ReleaseEnvironmentStatus, EnvironmentStatus>) new Dictionary<ReleaseEnvironmentStatus, EnvironmentStatus>()
    {
      {
        ReleaseEnvironmentStatus.NotStarted,
        EnvironmentStatus.NotStarted
      },
      {
        ReleaseEnvironmentStatus.Succeeded,
        EnvironmentStatus.Succeeded
      },
      {
        ReleaseEnvironmentStatus.Rejected,
        EnvironmentStatus.Rejected
      },
      {
        ReleaseEnvironmentStatus.InProgress,
        EnvironmentStatus.InProgress
      },
      {
        ReleaseEnvironmentStatus.Canceled,
        EnvironmentStatus.Canceled
      },
      {
        ReleaseEnvironmentStatus.Queued,
        EnvironmentStatus.Queued
      },
      {
        ReleaseEnvironmentStatus.Scheduled,
        EnvironmentStatus.Scheduled
      },
      {
        ReleaseEnvironmentStatus.Undefined,
        EnvironmentStatus.NotStarted
      },
      {
        ReleaseEnvironmentStatus.PartiallySucceeded,
        EnvironmentStatus.PartiallySucceeded
      }
    };
    private static readonly IDictionary<EnvironmentStatusOld, EnvironmentStatus> WebApiOldToNewValuesMap = (IDictionary<EnvironmentStatusOld, EnvironmentStatus>) new Dictionary<EnvironmentStatusOld, EnvironmentStatus>()
    {
      {
        EnvironmentStatusOld.NotStarted,
        EnvironmentStatus.NotStarted
      },
      {
        EnvironmentStatusOld.Succeeded,
        EnvironmentStatus.Succeeded
      },
      {
        EnvironmentStatusOld.Rejected,
        EnvironmentStatus.Rejected
      },
      {
        EnvironmentStatusOld.InProgress,
        EnvironmentStatus.InProgress
      },
      {
        EnvironmentStatusOld.Pending,
        EnvironmentStatus.InProgress
      },
      {
        EnvironmentStatusOld.Canceled,
        EnvironmentStatus.Canceled
      },
      {
        EnvironmentStatusOld.Queued,
        EnvironmentStatus.Queued
      },
      {
        EnvironmentStatusOld.Undefined,
        EnvironmentStatus.Undefined
      }
    };

    public static EnvironmentStatus ToWebApi(this ReleaseEnvironmentStatus status) => ReleaseEnvironmentStatusConverter.ServerToWebApiStatusMap[status];

    public static ReleaseEnvironmentStatus FromWebApi(this EnvironmentStatus status) => ReleaseEnvironmentStatusConverter.WebApiToServerStatusMap[status];

    public static EnvironmentStatus ConvertToNewEnvironmentStatusWithUpdatedValue(
      this EnvironmentStatus status)
    {
      if (status == EnvironmentStatus.Canceled)
        return status;
      EnvironmentStatusOld key = (EnvironmentStatusOld) status;
      return ReleaseEnvironmentStatusConverter.WebApiOldToNewValuesMap[key];
    }
  }
}
