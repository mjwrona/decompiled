// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpointResourceIds
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  [GenerateAllConstants(null)]
  public static class ServiceEndpointResourceIds
  {
    public const string AreaId = "1814AB31-2F4F-4A9F-8761-F4D77DC5A5D7";
    public const string AreaName = "serviceendpoint";

    [GenerateAllConstants(null)]
    public static class EndpointResource
    {
      public const string ProjectLocationIdString = "E85F1C62-ADFC-4B74-B618-11A150FB195E";
      public static readonly Guid ProjectLocationId = new Guid("E85F1C62-ADFC-4B74-B618-11A150FB195E");
      public const string CollectionLocationIdString = "14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0";
      public static readonly Guid CollectionLocationId = new Guid("14E48FDC-2C8B-41CE-A0C3-E26F6CC55BD0");
      public const string Name = "endpoints";
    }

    [GenerateAllConstants(null)]
    public static class AzureRmManagementResource
    {
      public const string LocationIdString = "9acb984c-4f88-4e13-9691-2e688dddc047";
      public static readonly Guid LocationId = new Guid("9acb984c-4f88-4e13-9691-2e688dddc047");
      public const string Name = "azurermmanagementgroups";
    }

    [GenerateAllConstants(null)]
    public static class AzureRmSubscriptionsResource
    {
      public const string LocationIdString = "18e8f65d-4e19-4a01-a621-cf0f2d938108";
      public static readonly Guid LocationId = new Guid("18e8f65d-4e19-4a01-a621-cf0f2d938108");
      public const string Name = "azurermsubscriptions";
    }

    [GenerateAllConstants(null)]
    public static class EndpointTypeResource
    {
      public static readonly Guid LocationId = new Guid("5A7938A4-655E-486C-B562-B78C54A7E87B");
      public const string Name = "types";
    }

    [GenerateAllConstants(null)]
    public static class EndpointShareResource
    {
      public static readonly Guid LocationId = new Guid("86E77201-C1F7-46C9-8672-9DFC2F6F568A");
      public const string Name = "share";
    }

    [GenerateAllConstants(null)]
    public static class ExectionHistoryResource
    {
      public const string GetLocationIdString = "10A16738-9299-4CD1-9A81-FD23AD6200D0";
      public static readonly Guid GetLocationId = new Guid("10A16738-9299-4CD1-9A81-FD23AD6200D0");
      public const string PostLocationIdString = "55B9ED4B-5404-41B1-B9D2-7ED757D02BB0";
      public static readonly Guid PostLocationId = new Guid("55B9ED4B-5404-41B1-B9D2-7ED757D02BB0");
      public const string Name = "executionhistory";
    }

    [GenerateAllConstants(null)]
    public static class EndpointProxyResource
    {
      public const string LocationIdString = "CC63BB57-2A5F-4A7A-B79C-C142D308657E";
      public static readonly Guid LocationId = new Guid("CC63BB57-2A5F-4A7A-B79C-C142D308657E");
      public const string Name = "endpointproxy";
    }

    [GenerateAllConstants(null)]
    public static class VstsAadOAuthResource
    {
      public const string LocationIdString = "47911d38-53e1-467a-8c32-d871599d5498";
      public static readonly Guid LocationId = new Guid("47911d38-53e1-467a-8c32-d871599d5498");
      public const string Name = "vstsaadoauth";
    }

    [GenerateAllConstants(null)]
    public static class OAuthConfigurationResource
    {
      public const string LocationIdString = "702edb4e-3952-43fe-a4eb-288938f3ba35";
      public static readonly Guid LocationId = new Guid("702edb4e-3952-43fe-a4eb-288938f3ba35");
      public const string Name = "oauthconfiguration";
    }
  }
}
