// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap.AadCapConfigHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Cap
{
  public static class AadCapConfigHelper
  {
    public static class Constants
    {
      public static class SettingKeys
      {
        public static readonly string CacheEntryTimeout = "/Configuration/Caching/AadCap/CacheEntryTimeout";
      }

      public static class SettingDefaults
      {
        public static readonly TimeSpan CacheEntryTimeout = TimeSpan.FromMinutes(60.0);
      }

      public static class FeatureFlags
      {
        public static readonly string DisableRedisCacheConditionalAccessValidation = "VisualStudio.Services.Identity.ConditionalAccessPolicyValidation.DisableRedisCache";
        public static readonly string DisableMemoryCacheConditionalAccessValidation = "VisualStudio.Services.Identity.ConditionalAccessPolicyValidation.DisableMemoryCache";
      }
    }
  }
}
