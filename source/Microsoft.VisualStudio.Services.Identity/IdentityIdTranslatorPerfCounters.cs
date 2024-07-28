// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdTranslatorPerfCounters
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityIdTranslatorPerfCounters
  {
    public static class TranslateToMasterId
    {
      public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateToMasterId.CallsPerSecond");
      public static readonly VssPerformanceCounter MissesPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateToMasterId.MissesPerSecond");
      public static readonly VssPerformanceCounter HitsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateToMasterId.HitsPerSecond");
    }

    public static class TranslateFromMasterId
    {
      public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateFromMasterId.CallsPerSecond");
      public static readonly VssPerformanceCounter MissesPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateFromMasterId.MissesPerSecond");
      public static readonly VssPerformanceCounter HitsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.TranslateFromMasterId.HitsPerSecond");
    }

    public static class InitializeIdTranslationMaps
    {
      public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsPerSecond");
      public static readonly VssPerformanceCounter CallsDroppedBeforeTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsDroppedBeforeTakingLock");
      public static readonly VssPerformanceCounter CallsDroppedAfterTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsDroppedAfterTakingLock");
      public static readonly VssPerformanceCounter CallsAccepted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsAccepted");
      public static readonly VssPerformanceCounter CallsCompleted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InitializeIdTranslationMaps.CallsCompleted");
    }

    public static class ClearCaches
    {
      public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsPerSecond");
      public static readonly VssPerformanceCounter CallsDroppedBeforeTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsDroppedBeforeTakingLock");
      public static readonly VssPerformanceCounter CallsDroppedAfterTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsDroppedAfterTakingLock");
      public static readonly VssPerformanceCounter CallsAccepted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsAccepted");
      public static readonly VssPerformanceCounter CallsCompleted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.ClearCaches.CallsCompleted");
    }

    public static class WithReadLockOnIdTranslationMaps
    {
      public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.CallsPerSecond");

      public static class InnerLoop
      {
        public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsPerSecond");
        public static readonly VssPerformanceCounter CallsDroppedBeforeTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsDroppedBeforeTakingLock");
        public static readonly VssPerformanceCounter CallsDroppedAfterTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsDroppedAfterTakingLock");
        public static readonly VssPerformanceCounter CallsAccepted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsAccepted");
        public static readonly VssPerformanceCounter CallsCompleted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.WithReadLockOnIdTranslationMaps.InnerLoop.CallsCompleted");
      }
    }

    public static class InvalidateIdTranslationCache
    {
      public static class ChangeTypeAdded
      {
        public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsPerSecond");
        public static readonly VssPerformanceCounter CallsDroppedBeforeTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsDroppedBeforeTakingLock");
        public static readonly VssPerformanceCounter CallsDroppedAfterTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsDroppedAfterTakingLock");
        public static readonly VssPerformanceCounter CallsAccepted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsAccepted");
        public static readonly VssPerformanceCounter CallsCompleted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeAdded.CallsCompleted");
      }

      public static class ChangeTypeRemoved
      {
        public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsPerSecond");
        public static readonly VssPerformanceCounter CallsDroppedBeforeTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsDroppedBeforeTakingLock");
        public static readonly VssPerformanceCounter CallsDroppedAfterTakingLock = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsDroppedAfterTakingLock");
        public static readonly VssPerformanceCounter CallsAccpted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsAccepted");
        public static readonly VssPerformanceCounter CallsCompleted = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeRemoved.CallsCompleted");
      }

      public static class ChangeTypeBulkChange
      {
        public static readonly VssPerformanceCounter CallsPerSecond = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.VisualStudio.Services.Identity.PerfCounters.IdentityIdTranslator.InvalidateIdTranslationCache.ChangeTypeBulkChange.CallsPerSecond");
      }
    }
  }
}
