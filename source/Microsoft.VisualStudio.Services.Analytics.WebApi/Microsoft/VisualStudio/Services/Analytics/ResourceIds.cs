// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ResourceIds
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public static class ResourceIds
  {
    public const string AnalyticsAreaName = "Analytics";
    public const string AnalyticsAreaIdString = "{F47C4501-5E41-4A7C-B17B-19B7CEF00B91}";
    public static readonly Guid AnalyticsAreaId = new Guid("{F47C4501-5E41-4A7C-B17B-19B7CEF00B91}");
    public const string AnalyticsInstanceId = "0000003C-0000-8888-8000-000000000000";
    public const string WorkItemResourceString = "{B69F570F-DC0D-43E8-97D1-0CA90C25D8A2}";
    public static readonly Guid WorkItemResourceId = new Guid("{B69F570F-DC0D-43E8-97D1-0CA90C25D8A2}");
    public const string WorkItemResourceName = "WorkItems";
    public const string StageResourceName = "Stage";
    public const string StageResourceIdString = "{9BD3F7D0-E20D-4E7B-95BA-854704939F9E}";
    public static readonly Guid StageResourceId = new Guid("{9BD3F7D0-E20D-4E7B-95BA-854704939F9E}");
    public const string StageShardInvalidResourceName = "Invalid";
    public const string StageShardInvalidResourceIdString = "{328A8D58-1727-4715-9A3D-E236FEEBD247}";
    public static readonly Guid StageShardInvalidResourceId = new Guid("{328A8D58-1727-4715-9A3D-E236FEEBD247}");
    public const string AnalyticsResourceName = "Analytics";
    public const string AnalyticsResourceIdString = "{97758D47-38CF-4AC2-A11D-C161E9B20609}";
    public static readonly Guid AnalyticsResourceId = new Guid("{97758D47-38CF-4AC2-A11D-C161E9B20609}");
    public const string AnalyticsHandleExtensionEventsResourceName = "Handle";
    public const string AnalyticsHandleExtensionEventsResourceIdString = "{94DC2916-BEA3-4E6C-BB1C-46FE3DDBC1C1}";
    public static readonly Guid AnalyticsHandleExtensionEventsResourceId = new Guid("{94DC2916-BEA3-4E6C-BB1C-46FE3DDBC1C1}");
    public const string AnalyticsViewsAreaName = "AnalyticsViews";
    public const string AnalyticsViewsAreaIdString = "f184dc2d-e63e-42ff-9fbc-64abe433bfd2";
    public static readonly Guid AnalyticsViewsAreaId = new Guid("f184dc2d-e63e-42ff-9fbc-64abe433bfd2");
    public const string AnalyticsViewsResourceName = "Views";
    public const string AnalyticsViewsResourceIdString = "{75677c7d-12f5-4dfa-aabd-392b0c7bc585}";
    public static readonly Guid AnalyticsViewsResourceId = new Guid("{75677c7d-12f5-4dfa-aabd-392b0c7bc585}");
    public static readonly string AnalyticsViewsRouteTemplate = "Analytics/{resource}/{viewId}";
    public const string AnalyticsStateResourceName = "State";
    public const string AnalyticsStateResourceIdString = "{0B79C382-D776-40B9-87B4-407FB8F7DF24}";
    public static readonly Guid AnalyticsStateResourceId = new Guid("{0B79C382-D776-40B9-87B4-407FB8F7DF24}");
    public static readonly string AnalyticsStateRouteTemplate = "Analytics/{resource}/{state}";
    public const string AnalyticsSpaceRequirementsResourceName = "SpaceRequirements";
    public const string AnalyticsSpaceRequirementsResourceIdString = "{AEEFB135-59C0-4A10-A477-E1981D657175}";
    public static readonly Guid AnalyticsSpaceRequirementsResourceId = new Guid("{AEEFB135-59C0-4A10-A477-E1981D657175}");
    public static readonly string AnalyticsSpaceRequirementsRouteTemplate = "Analytics/{resource}";
    public const string AnalyticsRoutePrefix = "_odata";
  }
}
