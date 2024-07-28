// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.DeletionRecoveryLevel
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault.Models
{
  public static class DeletionRecoveryLevel
  {
    public const string Purgeable = "Purgeable";
    public const string RecoverablePurgeable = "Recoverable+Purgeable";
    public const string Recoverable = "Recoverable";
    public const string RecoverableProtectedSubscription = "Recoverable+ProtectedSubscription";
  }
}
