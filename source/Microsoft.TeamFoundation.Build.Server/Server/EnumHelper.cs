// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.EnumHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class EnumHelper
  {
    internal static string Convert(BuildStatus status, out int statusId)
    {
      string str = string.Empty;
      switch (status)
      {
        case BuildStatus.InProgress:
          str = BuildTypeResource.Status_InProgress();
          statusId = 0;
          break;
        case BuildStatus.Succeeded:
          str = BuildTypeResource.Status_V1_Succeeded();
          statusId = 100;
          break;
        case BuildStatus.PartiallySucceeded:
          str = BuildTypeResource.Status_PartiallySucceeded();
          statusId = 100;
          break;
        case BuildStatus.Failed:
          str = BuildTypeResource.Status_Failed();
          statusId = 200;
          break;
        case BuildStatus.Stopped:
          str = BuildTypeResource.Status_Stopped();
          statusId = 300;
          break;
        case BuildStatus.NotStarted:
          str = BuildTypeResource.BuildInitializingState();
          statusId = 0;
          break;
        default:
          statusId = 0;
          break;
      }
      return str;
    }

    internal static string ToString(BuildStatus status) => EnumHelper.Convert(status, out int _);

    internal static string ToString(BuildPhaseStatus status)
    {
      string str = string.Empty;
      switch (status)
      {
        case BuildPhaseStatus.Unknown:
          str = BuildTypeResource.Status_Unknown();
          break;
        case BuildPhaseStatus.Failed:
          str = BuildTypeResource.Status_Failed();
          break;
        case BuildPhaseStatus.Succeeded:
          str = BuildTypeResource.Status_Succeeded();
          break;
      }
      return str;
    }
  }
}
