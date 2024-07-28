// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CustomSqlError
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class CustomSqlError
  {
    public const int GenericWrapperCode = 50000;
    public const int PublisherAlreadyExists = 270000;
    public const int PublisherDoesNotExist = 270001;
    public const int ExtensionAlreadyExists = 270002;
    public const int ExtensionDoesNotExist = 270003;
    public const int CategoryAlreadyExists = 270004;
    public const int CategoryDoesNotExist = 270005;
    public const int ExtensionMustBePrivate = 270006;
    public const int ReviewAlreadyExists = 270007;
    public const int ReviewAlreadyReported = 270008;
    public const int ReviewDoesNotExist = 270009;
    public const int FileIdNull = 270010;
    public const int InvalidTrendingStatisticsType = 270011;
    public const int AzurePublisherAlreadyExists = 270012;
    public const int AzurePublisherDoesNotExist = 270013;
    public const int ReviewDoesNotExist2 = 270014;
    public const int ExtensionIdDoesNotExist = 270015;
    public const int ExtensionVersionDoesNotExist = 270016;
    public const int AssetAlreadyExistsForExtensionVersion = 270017;
    public const int PuidAlreadyExists = 270018;
    public const int NoCollectionIdForAccounts = 270019;
    public const int GenericError = 270020;
    public const int QnAItemDoesNotExist = 270021;
    public const int QnAItemAlreadyReported = 270022;
    public const int QnAQuestionIdNotSpecified = 270023;
    public const int ValidationStepInvalidParentId = 270024;
    public const int ValidationStepInvalidStepId = 270025;
    public const int DraftIdDoesNotExist = 270026;
    public const int InvalidScanItem = 270027;
    public const int PublisherDomainDoesNotExist = 270028;
    public const int DnsTokenNotVerified = 270029;
    public const int InvalidUpdateOperation = 270030;
    public const int VsExtensionConsolidationForInvalidExtension = 270031;
    public const int VsExtensionConsolidationAlreadyEnabled = 270032;
    public const int AssetDoesNotExistsForExtensionVersion = 270033;
    public const int MAX_SQL_ERROR = 270032;
  }
}
