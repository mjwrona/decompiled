// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ResourceUsage.Server.ResourceUsageConstants
// Assembly: Microsoft.TeamFoundation.ResourceUsage.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55568389-A340-4F60-8DD1-887E0E3F1980
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ResourceUsage.Server.dll

using System;

namespace Microsoft.TeamFoundation.ResourceUsage.Server
{
  internal static class ResourceUsageConstants
  {
    public const string AreaName = "ResourceUsage";
    public const string AreaString = "{b5d12953-b587-4445-804a-80c4216e1d0f}";
    public static readonly Guid AreaId = new Guid("{b5d12953-b587-4445-804a-80c4216e1d0f}");
    public const string TeamProjectCollectionResourceName = "TeamProjectCollection";
    public const string TeamProjectCollectionResourceString = "{da12b2bb-e6b9-4038-9bb4-d6768fb6cb1c}";
    public static readonly Guid TeamProjectCollectionResourceId = new Guid("{da12b2bb-e6b9-4038-9bb4-d6768fb6cb1c}");
    public const string ProjectResourceName = "Project";
    public const string ProjectResourceString = "{30192dd3-3ea9-4458-8270-7ed1ca8c767d}";
    public static readonly Guid ProjectResourceId = new Guid("{30192dd3-3ea9-4458-8270-7ed1ca8c767d}");
    public const string ResourceUsageProjectServiceEnabled = "ResourceUsage.Service.Project";
    public const string ResourceUsageTeamProjectCollectionServiceEnabled = "ResourceUsage.Service.TeamProjectCollection";
  }
}
