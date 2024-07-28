// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class StrongBoxConstants
  {
    internal const string TraceArea = "StrongBox";
    internal const string TraceLayer = "Service";
    internal const int AccessDenied = -2146893808;
    public const string ConfigurationSecretsDrawerName = "ConfigurationSecrets";
    public const string ConfigurationSecretsPreviousValueSuffix = "-previous";
    public const string AzureRelaySecretsDrawerName = "AzureRelaySecrets";
    internal const string BatchItemsReencryptionFlag = "Microsoft.AzureDevOps.StrongBox.BatchedItemsReencryption";
    public const string RemoveStrongBoxOrphansFlag = "Microsoft.AzureDevOps.StrongBox.DeleteStrongBoxOrphans";
    public const string PreserveKeyTypeWhenRotatingFlag = "Microsoft.AzureDevOps.StrongBox.PreserveKeyType";
    public const string RegistryPath = "/Service/StrongBox/Constants/ReencryptionBatchSize";
    public const int DefaultBatchSize = 100;
  }
}
