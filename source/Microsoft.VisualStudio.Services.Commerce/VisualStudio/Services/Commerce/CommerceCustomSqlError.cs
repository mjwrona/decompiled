// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceCustomSqlError
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public enum CommerceCustomSqlError
  {
    InvalidAzureSubscriptionId = 2100001, // 0x00200B21
    AccountAtAbsoluteMaximumQuantity = 2100002, // 0x00200B22
    MaximumMoreThanIncludedQuantity = 2100003, // 0x00200B23
    MaximumMoreThanUsedQuantity = 2100004, // 0x00200B24
    NewIncludedMoreThanUsedQuantity = 2100005, // 0x00200B25
    NewIncludedMoreThanMaximumQuantity = 2100006, // 0x00200B26
    NewIncludedMoreThanExistingMaximumQuantity = 2100007, // 0x00200B27
    NoPaidBillingModeForCommitmentResources = 2100008, // 0x00200B28
    CreateOfferMeterDefinitionPlan = 2100009, // 0x00200B29
    CreateNextUniqueId = 2100010, // 0x00200B2A
    DuplicateOfferMeterPrice = 2100011, // 0x00200B2B
    ResourceUpdateFailed = 2100012, // 0x00200B2C
  }
}
