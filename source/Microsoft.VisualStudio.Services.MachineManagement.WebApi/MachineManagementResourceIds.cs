// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineManagementResourceIds
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  internal static class MachineManagementResourceIds
  {
    public const string Area = "machinemanagement";
    public const string AreaId = "8C216275-45E8-4745-85D1-1BD9ACF37B03";
    public const string PoolsResource = "pools2";
    public const string PoolsRoute = "machinemanagement:pools2";
    public static readonly Guid PoolsLocationId = new Guid("{B7D55A3D-8213-4F76-92FD-78293EF5DC68}");
    public const string RequestsResource = "requests";
    public const string RequestsRoute = "machinemanagement:requests";
    public static readonly Guid RequestsLocationId = new Guid("{F9C9777E-187B-4A47-A60B-35516662EB6B}");
    public const string RequestNotificationsResource = "requestnotifications";
    public const string RequestNotificationsRoute = "machinemanagement:requestnotifications";
    public static readonly Guid RequestNotificationsLocationId = new Guid("{B457AB1F-8764-48B9-A801-D7193127B13C}");
  }
}
