// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.DataProviderConstants
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [GenerateAllConstants(null)]
  public static class DataProviderConstants
  {
    public const string DataProviderContributionTypeId = "ms.vss-web.data-provider";
    public const string ContributionNameProperty = "name";
    public const string ContributionInstanceTypeProperty = "serviceInstanceType";
    public const string ContributionResourceAreaIdProperty = "resourceAreaId";
    public const string ContributionResolutionProperty = "resolution";
    public const string ContributionResolutionServer = "Server";
    public const string ContributionResolutionServerOnly = "ServerOnly";
    public const string ContributionResolutionClient = "Client";
    public const string ContributionPropertyProviderProperty = "propertyProvider";
    public const string ContributionDataTypeProperty = "dataType";
  }
}
