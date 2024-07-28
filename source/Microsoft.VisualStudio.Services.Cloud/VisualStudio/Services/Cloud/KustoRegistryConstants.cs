// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoRegistryConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal static class KustoRegistryConstants
  {
    public static readonly string SettingsRoot = "/Configuration/Kusto";
    public static readonly string ConnectionUri = RegistryHelpers.CombinePath(KustoRegistryConstants.SettingsRoot, nameof (ConnectionUri));
    public static readonly string ConnectionUriUnion = RegistryHelpers.CombinePath(KustoRegistryConstants.SettingsRoot, nameof (ConnectionUriUnion));
    public static readonly string QuerySettingsRoot = RegistryHelpers.CombinePath(KustoRegistryConstants.SettingsRoot, "QuerySettings");
    public static readonly string AllQuerySettings = RegistryHelpers.CombinePath(KustoRegistryConstants.QuerySettingsRoot, "...");
    public static readonly string MaxQueryRetryCount = RegistryHelpers.CombinePath(KustoRegistryConstants.QuerySettingsRoot, nameof (MaxQueryRetryCount));
    public static readonly string SlowQueryThreshold = RegistryHelpers.CombinePath(KustoRegistryConstants.QuerySettingsRoot, nameof (SlowQueryThreshold));
    public static readonly string QueryTimeout = RegistryHelpers.CombinePath(KustoRegistryConstants.QuerySettingsRoot, nameof (QueryTimeout));
  }
}
