// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.WebApi.ServicingOrchestrationLocationIds
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Cloud.WebApi
{
  [GenerateAllConstants(null)]
  public static class ServicingOrchestrationLocationIds
  {
    public const string ReparentCollectionAreaId = "{45A344C2-967D-4353-953D-DDA8B88ECA08}";
    public const string ReparentCollectionAreaName = "ReparentCollection";
    public static readonly Guid ReparentCollectionRequestsLocationId = new Guid("7CE8BC16-D4D6-4D42-9AD1-D9F57FA82591");
    public const string ReparentCollectionRequestsResourceName = "Requests";
    public const string DataImportOrchestrationAreaId = "{6C0ED9B4-E87E-4F1D-976A-F3A05794EBE2}";
    public const string DataImportOrchestrationAreaName = "DataImportOrchestration";
    public static readonly Guid DataImportOrchestrationRequestsLocationId = new Guid("85A4EE77-D3BA-4D87-9851-FE22A8C6C636");
    public const string DataImportOrchestrationResourceName = "Requests";
    public const string NewDomainUrlMigrationAreaId = "{48D0B689-8F32-444A-BDA1-6780E34ACA8B}";
    public const string NewDomainUrlMigrationAreaName = "NewDomainUrlMigration";
    public static readonly Guid NewDomainUrlMigrationRequestsLocationId = new Guid("91CC4DD2-7AAD-4182-BB39-940717B86890");
    public const string NewDomainUrlMigrationRequestsResourceName = "Requests";
  }
}
