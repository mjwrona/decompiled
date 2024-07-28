// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Store.GraphMembershipStoreConstants
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

namespace Microsoft.VisualStudio.Services.Graph.Store
{
  internal static class GraphMembershipStoreConstants
  {
    internal static class DefaultSettings
    {
      internal const int DefaultMegaTenantSize = 5000;
    }

    internal static class FeatureFlags
    {
      internal const string GetStorageKeysInScope = "VisualStudio.Services.Graph.Store.EnableGetStorageKeysInScope";
      internal const string IsMegaTenant = "VisualStudio.Services.Graph.Store.EnableIsMegaTenant";
    }

    internal static class RegistryKeys
    {
      private const string c_prefix = "/Service/Sps/Graph/";
      public const string GraphMegaTenant = "/Service/Sps/Graph/MegaTenant";
      public const string GraphMegaTenantSize = "/Service/Sps/Graph/MegaTenantSize";
    }
  }
}
