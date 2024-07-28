// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PageXHRData
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class PageXHRData : WebSdkMetadata
  {
    [DataMember(EmitDefaultValue = false)]
    public DataProviderResult DataProviderData { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ContributionsPageData ContributionsData { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DynamicBundlesCollection Bundles { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public FeatureAvailabilityContext FeatureAvailability { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public NavigationContext Navigation { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ServiceLocations ServiceLocations { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, PerformanceTimingGroup> PerformanceTimings { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ActivityId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string StaticContentVersion { get; set; }
  }
}
