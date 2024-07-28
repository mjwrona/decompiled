// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CatalogResourceTypes
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class CatalogResourceTypes
  {
    public static readonly Guid OrganizationalRoot = new Guid("69A51C5E-C093-447e-A177-A09E47A60974");
    public static readonly Guid InfrastructureRoot = new Guid("14F04669-6779-42d5-8975-184B93650C83");
    public static readonly Guid TeamFoundationServerInstance = new Guid("B36F1BDA-DF2D-482b-993A-F194A31A1FA2");
    public static readonly Guid ProjectCollection = new Guid("26338D9E-D437-44aa-91F2-55880A328B54");
    public static readonly Guid TeamProject = new Guid("48577A4A-801E-412c-B8AE-CF7EF3529616");
    public static readonly Guid ResourceFolder = new Guid("55F97194-EC42-4dfc-B596-7DECC43CDE1E");
    public static readonly Guid GenericLink = new Guid("53D857F7-0197-45fa-BB58-EDF76AD7BFB2");
    public static readonly Guid Machine = new Guid("0584A4A2-475B-460e-A7AC-10C28951518F");
    public static readonly Guid TeamFoundationWebApplication = new Guid("FFAF34BB-ADED-4507-9E52-FCA85E91BA63");
    [Obsolete("This field is obsolete because we no longer support this catalog entry. There is no replacement.", false)]
    public static readonly Guid SqlDatabaseInstance = new Guid("EB1E0B3B-FAA1-49d2-931A-FDC373682BA5");
    public static readonly Guid SqlAnalysisInstance = new Guid("D22D57DA-355D-4a3c-82DE-62B3E157D0B3");
    public static readonly Guid SqlReportingInstance = new Guid("065977D6-00EA-4a77-81EC-1CD011644AAC");
    [Obsolete("This field is obsolete because we no longer support this catalog entry. There is no replacement.", false)]
    public static readonly Guid ApplicationDatabase = new Guid("526301DE-F821-48c8-ABBD-3430DC7946D3");
    [Obsolete("This field is obsolete because we no longer support this catalog entry. There is no replacement.", false)]
    public static readonly Guid ProjectCollectionDatabase = new Guid("1B6B5931-69F6-4c53-90A0-220B177353B7");
    public static readonly Guid ProjectServerMapping = new Guid("0A104630-02B3-4fdb-958A-660B97B103A9");
    public static readonly Guid ProjectServerRegistration = new Guid("289DD275-CECA-4698-8042-38D2E86FC682");
    public static readonly Guid SharePointWebApplication = new Guid("{3DADD190-40E6-4fc1-A306-4906713C87CE}");
    public static readonly Guid SharePointSiteCreationLocation = new Guid("{9FB288AE-9D94-40cb-B5E7-0EFC3FE3599F}");
    public static readonly Guid ProjectPortal = new Guid("{450901B6-B528-4863-9876-5BD3927DF467}");
    public static readonly Guid ProcessGuidanceSite = new Guid("{15DA1594-45F5-47d4-AE52-78F16E67EB1E}");
    public static readonly Guid WarehouseDatabase = new Guid("{CE318CD9-F797-45dc-ACC7-792C3428E39D}");
    public static readonly Guid AnalysisDatabase = new Guid("{64C0C64F-7199-4c0a-A1F7-6D979292E86E}");
    public static readonly Guid ReportingConfiguration = new Guid("{143B22C5-D1B9-494f-B124-68D098ABA598}");
    public static readonly Guid ReportingServer = new Guid("{F756975E-3593-448b-A6B8-E34010908621}");
    public static readonly Guid ReportingFolder = new Guid("{41C8B6DB-39EC-49db-9DB8-0760E836BFBE}");
    public static readonly Guid TeamSystemWebAccess = new Guid("{47FA57A4-8157-4fb5-9A64-A7A4954BD284}");
    public static readonly Guid TestController = new Guid("3C856555-8737-48b6-8B61-4B24DB7FEB15");
    public static readonly Guid TestEnvironment = new Guid("D457AA94-F00E-4342-92E8-FFE81535E74B");
    public static readonly Guid DataCollector = new Guid("B1A784AD-4C46-4574-B18D-2AA07AA2BDDB");
  }
}
