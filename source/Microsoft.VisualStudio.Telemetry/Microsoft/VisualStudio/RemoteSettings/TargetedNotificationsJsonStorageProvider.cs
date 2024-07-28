// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.TargetedNotificationsJsonStorageProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal class TargetedNotificationsJsonStorageProvider : 
    ITargetedNotificationsCacheStorageProvider
  {
    private readonly string cacheDirectory;
    private readonly string cacheFileFullPath;
    private const string cacheLockName = "Global\\55F58BAB-BDB9-47D5-B85E-B4D8234E8FAA";
    private const string cacheFileName = "targetnote_v1.json";
    private ITargetedNotificationsTelemetry telemetry;
    private Lazy<Mutex> cacheLock;

    public TargetedNotificationsJsonStorageProvider(RemoteSettingsInitializer initializer)
    {
      this.cacheDirectory = initializer.GetLocalAppDataRoot();
      this.cacheFileFullPath = Path.Combine(this.cacheDirectory, "targetnote_v1.json");
      this.telemetry = initializer.TargetedNotificationsTelemetry;
      this.cacheLock = new Lazy<Mutex>((Func<Mutex>) (() =>
      {
        try
        {
          return new Mutex(false, "Global\\55F58BAB-BDB9-47D5-B85E-B4D8234E8FAA");
        }
        catch (Exception ex)
        {
          this.telemetry.PostDiagnosticFault("VS/Core/TargetedNotifications/MutexFailure", "Failed to create Mutex", ex);
          return (Mutex) null;
        }
      }));
    }

    public bool Lock(int? timeoutMs = null)
    {
      try
      {
        if (this.cacheLock.Value == null)
          return false;
        return timeoutMs.HasValue ? this.cacheLock.Value.WaitOne(timeoutMs.Value) : this.cacheLock.Value.WaitOne();
      }
      catch (AbandonedMutexException ex)
      {
        return true;
      }
      catch (Exception ex)
      {
        this.telemetry.PostDiagnosticFault("VS/Core/TargetedNotifications/MutexFailure", "Failed to lock Mutex", ex);
        return false;
      }
    }

    public void Unlock()
    {
      try
      {
        if (this.cacheLock.Value == null)
          return;
        this.cacheLock.Value.ReleaseMutex();
      }
      catch (ApplicationException ex)
      {
        this.telemetry.PostCriticalFault("VS/Core/TargetedNotifications/CacheUnbalancedUnlock", "Unbalanced call to Unlock", (Exception) ex);
      }
      catch (Exception ex)
      {
        this.telemetry.PostDiagnosticFault("VS/Core/TargetedNotifications/MutexFailure", "Failed to unlock Mutex", ex);
      }
    }

    public void Reset()
    {
      try
      {
        ReparsePointAware.DeleteFile(this.cacheFileFullPath);
      }
      catch (Exception ex)
      {
        this.telemetry.PostCriticalFault("VS/Core/TargetedNotifications/CacheResetFailure", "Failed to reset the local cache", ex);
      }
    }

    public CachedTargetedNotifications GetLocalCacheCopy()
    {
      if (!File.Exists(this.cacheFileFullPath))
        return new CachedTargetedNotifications();
      string end;
      using (StreamReader streamReader = new StreamReader(this.cacheFileFullPath))
        end = streamReader.ReadToEnd();
      try
      {
        return JsonConvert.DeserializeObject<CachedTargetedNotifications>(end) ?? new CachedTargetedNotifications();
      }
      catch (Exception ex)
      {
        this.telemetry.PostCriticalFault("VS/Core/TargetedNotifications/CacheDeserializationFailure", "Failed to deserialize the local cache", ex);
        this.Reset();
        return new CachedTargetedNotifications();
      }
    }

    public void SetLocalCache(CachedTargetedNotifications newCache)
    {
      if (!Directory.Exists(this.cacheDirectory))
        ReparsePointAware.CreateDirectory(this.cacheDirectory);
      ReparsePointAware.WriteAllText(this.cacheFileFullPath, JsonConvert.SerializeObject((object) newCache));
    }
  }
}
