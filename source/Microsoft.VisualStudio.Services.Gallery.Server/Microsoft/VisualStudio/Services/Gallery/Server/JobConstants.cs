// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.JobConstants
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public static class JobConstants
  {
    public static readonly Guid AuditLogCleanUpJobId = new Guid("07cea8f7-668d-417d-a08e-6d2e3dc74fe5");
    public static readonly Guid DeleteReviewsAndStatsJobId = new Guid("04172A3C-F93C-48CF-B2FA-5404E1991ED4");
    public static readonly Guid ReleaseDatePopulatorJobId = new Guid("98F51481-0C9D-4712-A28A-9CF6646FBB81");
    public static readonly Guid ReprocessExtensionJobId = new Guid("B767F321-627F-4DC7-BA7E-D7B4A70EA664");
    public static readonly Guid SitemapGeneratorJobGuid = new Guid("D35AD7C8-3ABA-4642-9820-6421534B92B0");
    public static readonly Guid TrendingScoreCalculatorJobId = new Guid("f5f7b7be-a6c3-408b-bbee-c88070d9eb37");
    public static readonly Guid PostUploadExtensionProcessingJob = new Guid("ED4D3B1E-54C9-4754-953B-70E13476D544");
    public static readonly Guid TagPopulatorJobId = new Guid("E348EBD5-D260-4365-AEC9-5C767D657244");
    public static readonly Guid SmallIconGenerationJobId = new Guid("6982E0FD-2787-40B7-8972-202153A8AFC9");
    public static readonly Guid VSCodeSupportedVersionPropertyPopulatorJobId = new Guid("339bd563-c2e9-41aa-8e61-96ceb3842831");
    public static readonly Guid VSIDEVersionDeletionJobId = new Guid("fb283a8e-5c63-4964-aa90-eceb6c0a6683");
    public static readonly Guid TaskIdPropertyPopulatorJob = new Guid("2B262C65-4D5B-49B5-81C6-072E1489FC0F");
    public static readonly Guid VSTSContributionTypesPopulatorJob = new Guid("f0c5ef2c-4bb9-4d57-b454-e6be6e85a85f");
    public static readonly Guid AddVSIXManifestAsAssetJob = new Guid("979018da-b365-4feb-bde0-95b2867af189");
    public static readonly Guid DraftsCleanUpJobId = new Guid("E351FC99-45AE-4D92-AF00-79F833DB85F2");
    public static readonly Guid PartnerDataSyncJobId = new Guid("776B31F1-AC03-4C02-99D6-43CD7E931156");
    public static readonly Guid CDHeaderPreValidatorJob = new Guid("FAF494FD-2821-4360-AB9D-43F9367E20F7");
    public static readonly Guid CDHeaderCorrectionJob = new Guid("FD2077B9-D424-4D62-8504-284F4EED68E6");
    public static readonly Guid CDHeaderPostValidatorJob = new Guid("D41162BA-5903-4525-B5ED-1CD8DF50CA7E");
    public static readonly Guid ProductExtensionsIteratorDriverJob = new Guid("C063B2D5-022C-48AF-8080-2D1C5A795981");
    public static readonly Guid VsExtensionInstallationTargetRangeChangeJobId = new Guid("14AAA068-76EC-426B-801F-69C5373B8857");
    public static readonly Guid DeleteGDPREventsJobId = new Guid("09C662D9-9C15-43C4-A744-1A7C994E045A");
    public static readonly Guid GetGDPRMessageFromServiceBusJobId = new Guid("4EDDF1D1-15B4-4174-9EB5-49BD969D001D");
    public static readonly Guid RemoveGDPRMessageFromServiceBusJobId = new Guid("98BDDE31-C3E5-457F-B0A2-A374939F82EC");
    public static readonly Guid GetAndRemoveGDPRMessageFromDLQJobId = new Guid("EFCE88FE-1321-4282-8797-BB01DBE14A9E");
    public static readonly Guid ServiceEndpointPropertyPopulatorJob = new Guid("14B557C4-BD15-4E39-B67C-CB333534A895");
  }
}
