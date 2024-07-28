// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.PlatformGraphMembershipTraversalServiceConstants
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

namespace Microsoft.VisualStudio.Services.Graph
{
  internal static class PlatformGraphMembershipTraversalServiceConstants
  {
    internal static class DefaultSettings
    {
      internal static class TraverseDescendants
      {
        internal const int MaxCacheHoursLife = 48;
        internal const int MaxSubjectsToTraverseInBatch = 25;
        internal const int MaxRemoteGroupExpansions = 10;
        internal const int MaxRemoteDescendantsToTraverse = 10000;
        internal const int MaxDescendantsToTraverse = 1000;
        internal const int ReadIdentitiesBatchSize = 10000;
      }
    }

    internal static class FeatureFlags
    {
      internal const string TraverseDescendants = "VisualStudio.Services.Graph.EnableTraverseDescendants";
      internal const string DisableTraversalCacheInvalidations = "VisualStudio.Services.Graph.DisableTraversalCacheInvalidations";
      internal const string EnableTraverseDescendantsMegaTenantAlternativeFlow = "VisualStudio.Services.Graph.EnableTraverseDescendantsMegaTenantAlternativeFlow";
    }

    internal static class RegistryKeys
    {
      private const string c_prefix = "/Service/Sps/Graph";
      internal const string NotificationFilter = "/Service/Sps/Graph/...";

      internal static class TraverseDescendants
      {
        private const string c_traverseDescendantsPrefix = "/Service/Sps/Graph/TraverseDescendants";
        internal const string MaxSubjectsToTraverseInBatch = "/Service/Sps/Graph/TraverseDescendants/MaxSubjectsToTraverseInBatch";
        internal const string MaxRemoteGroupExpansions = "/Service/Sps/Graph/TraverseDescendants/MaxRemoteGroupExpansions";
        internal const string MaxRemoteDescendantsToTraverse = "/Service/Sps/Graph/TraverseDescendants/MaxRemoteDescendantsToTraverse";
        internal const string MaxDescendantsToTraverse = "/Service/Sps/Graph/TraverseDescendants/MaxDescendantsToTraverse";
        internal const string ReadIdentitiesBatchSize = "/Service/Sps/Graph/TraverseDescendants/ReadIdentitiesBatchSize";
      }
    }
  }
}
