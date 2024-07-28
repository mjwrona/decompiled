// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServiceConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.Services.Aad
{
  internal static class AadServiceConstants
  {
    internal const string Area = "VisualStudio.Services.Aad";
    internal const string Layer = "Service";

    internal static class FeatureFlags
    {
      internal const string AadGroupIntegration = "VisualStudio.Services.Identity.AadGroupIntegration";
      internal const string TransitiveDownAadGroupIntegration = "VisualStudio.Services.Identity.AadGroupIntegration.TransitiveDown";
      internal const string GroupClaims = "VisualStudio.Services.Aad.GroupClaims";
      internal const string EnableV2EndpointForGetDescendantIds = "VisualStudio.Services.Aad.EnableV2EndpointForGetDescendantIds";
      internal const string EnableMicrosoftGraph = "VisualStudio.Services.Aad.EnableMicrosoftGraph";
      internal const string EnableGraphDataCompare = "VisualStudio.Services.Aad.EnableGraphDataCompare";
      internal const string EnableUseMicrosoftGraphAsDataCompareResult = "VisualStudio.Services.Aad.EnableUseMicrosoftGraphAsDataCompareResult";
      internal const string SkipHasThumbnailPhoto = "VisualStudio.Services.Aad.SkipHasThumbnailPhoto";
      internal const string DisableMicrosoftGraph = "VisualStudio.Services.Aad.DisableMicrosoftGraph";
      internal const string InvalidateStaleAncestorIdsCacheToUseAadService = "VisualStudio.Services.Aad.InvalidateStaleAncestorIdsCacheToUseAadService";
      internal const string DisplayRawHttpExceptions = "VisualStudio.Services.Aad.MicrosoftGraph.DisplayRawHttpExceptions";
      internal const string LogRawHttpExceptions = "VisualStudio.Services.Aad.MicrosoftGraph.LogRawHttpExceptions";
      private static ConcurrentDictionary<string, string> disableMicrosoftGraphOperationLookup = new ConcurrentDictionary<string, string>();
      private static ConcurrentDictionary<string, string> enableMicrosoftGraphOperationLookup = new ConcurrentDictionary<string, string>();

      internal static string DisableMicrosoftGraphByOperation(string operation)
      {
        if (!AadServiceConstants.FeatureFlags.disableMicrosoftGraphOperationLookup.ContainsKey(operation))
          AadServiceConstants.FeatureFlags.disableMicrosoftGraphOperationLookup[operation] = "VisualStudio.Services.Aad.DisableMicrosoftGraph." + operation;
        return AadServiceConstants.FeatureFlags.disableMicrosoftGraphOperationLookup[operation];
      }

      internal static string EnableMicrosoftGraphByOperation(string operation)
      {
        if (!AadServiceConstants.FeatureFlags.enableMicrosoftGraphOperationLookup.ContainsKey(operation))
          AadServiceConstants.FeatureFlags.enableMicrosoftGraphOperationLookup[operation] = "VisualStudio.Services.Aad.EnableMicrosoftGraph." + operation;
        return AadServiceConstants.FeatureFlags.enableMicrosoftGraphOperationLookup[operation];
      }
    }

    internal static class IdentityPropertyKeys
    {
      internal const string Groups = "groups";
      internal const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
    }

    internal static class RegistryKeys
    {
      internal const string Prefix = "/Service/Aad";
      internal const string NotificationFilter = "/Service/Aad/...";
      internal const string AuthAuthority = "/Service/Aad/AuthAuthority";
      internal const string AuthClientId = "/Service/Aad/AuthClientId";
      internal const string IntAuthClientId = "/Service/Aad/IntAuthClientId";
      internal const string GraphApiDomainName = "/Service/Aad/GraphApiDomainName";
      internal const string GraphApiResource = "/Service/Aad/GraphApiResource";
      internal const string GraphApiVersion = "/Service/Aad/GraphApiVersion";
      internal const string MicrosoftServicesTenant = "/Service/Aad/MicrosoftServicesTenant";
      internal const string MaxRequestsPerBatch = "/Service/Aad/MaxRequestsPerBatch";
      internal const string CacheEvictionIntervalInHours = "/Service/Aad/CacheEvictionIntervalInHours";
      internal const string CacheEvictionEnabled = "/Service/Aad/CacheEvictionEnabled";
      internal const string MicrosoftServicesTenantIdForGetTenantsByKey = "/Service/Aad/MicrosoftServicesTenantForGetTenantsByKey";
      internal const string MicrosoftGraphApiDomainName = "Service.MicrosoftGraph.MicrosoftGraphApiDomainName";
      internal const string MicrosoftGraphApiVersion = "Service.MicrosoftGraph.MicrosofGraphApiVersion";
      internal const string MicrosoftGraphApiSlowThresholdInSeconds = "Service.MicrosoftGraph.MicrosoftGraphApiSlowThresholdInSeconds";
      internal const string MicrosoftGraphApiRequestLoggingPercentage = "Service.MicrosoftGraph.MicrosoftGraphRequestLoggingPercentage";
      internal const string MicrosoftGraphApiMaxRetries = "Service.MicrosoftGraph.MicrosoftGraphApiMaxRetries";
      internal const string MicrosoftGraphDataComparePercentage = "/Service/Aad/MicrosoftGraphDataComparePercentage";

      internal static class GetDescendantIds
      {
        private const string GetDescendantIdsPrefix = "/Service/Aad/GetDescendantIds";
        internal const string MaxResults = "/Service/Aad/GetDescendantIds/MaxResults";
        internal const string MaxPages = "/Service/Aad/GetDescendantIds/MaxPages";
        internal const string MaxPageSize = "/Service/Aad/GetDescendantIds/MaxPageSize";
        internal const string GraphApiVersion = "/Service/Aad/GetDescendantIds/GraphApiVersion";
      }
    }

    internal static class MicrosoftGraphConfigPrototypes
    {
      internal static readonly IConfigPrototype<string> MicrosoftGraphApiDomainName = ConfigPrototype.Create<string>("Service.MicrosoftGraph.MicrosoftGraphApiDomainName", "https://graph.microsoft.com");
      internal static readonly IConfigPrototype<string> MicrosoftGraphApiVersion = ConfigPrototype.Create<string>("Service.MicrosoftGraph.MicrosofGraphApiVersion", "v1.0");
      internal static readonly IConfigPrototype<byte> MicrosoftGraphApiLoggingPercentage = ConfigPrototype.Create<byte>("Service.MicrosoftGraph.MicrosoftGraphRequestLoggingPercentage", (byte) 100);
      internal static readonly IConfigPrototype<int> MicrosoftGraphApiSlowThresholdInSeconds = ConfigPrototype.Create<int>("Service.MicrosoftGraph.MicrosoftGraphApiSlowThresholdInSeconds", 5);
      internal static readonly IConfigPrototype<int> MicrosoftGraphApiMaxRetries = ConfigPrototype.Create<int>("Service.MicrosoftGraph.MicrosoftGraphApiMaxRetries", 3);
    }

    internal static class DefaultSettings
    {
      internal const string MicrosoftServicesTenant = "microsoftservices.onmicrosoft.com";
      internal const int MaxRequestsPerBatch = 5;
      internal const string GraphApiVersion = "1.5-internal";
      internal const string GraphApiDomainName = "graph.windows.net";
      internal const string MicrosoftGraphApiDomainName = "https://graph.microsoft.com";
      internal const string MicrosoftGraphApiVersion = "v1.0";
      internal const byte MicrosoftGraphRequestLoggingPercentage = 100;
      internal const int MicrosoftGraphApiSlowThresholdInSeconds = 5;
      internal const int MicrosoftGraphApiMaxRetries = 3;
      internal const int MicrosoftGraphDataComparePercentage = 10;

      internal static class GetDescendantIds
      {
        internal const int MaxPages = 10;
        internal const int KnownMaxPageSizeThreshold = 999;
        internal const int MaxResults = 9990;
        internal const string GraphApiVersion = "1.6-internal";
      }
    }
  }
}
