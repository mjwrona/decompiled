// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionManagementPermissions
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class SubscriptionManagementPermissions
  {
    public const int ReadSubscriptionData = 1;
    public const int WriteSubscriptionData = 2;
    public const int AllPermissions = 3;

    public static class Tokens
    {
      public const string SpeicalPurchaseSecurityNamespaceToken = "/PartnerPurchase";
      public static string CommerceChangeSubscriptionSecurityNamespaceToken = "/AccountSubscriptionChange";
      public static string CommerceResourceMigrationAndDualWritesSecurityNamespaceToken = "/MigrateResourcesAndDualWrites";
    }
  }
}
