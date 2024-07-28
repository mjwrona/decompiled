// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.GroupedRemoteSettings
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class GroupedRemoteSettings : Dictionary<string, RemoteSettingPossibilities>
  {
    public GroupedRemoteSettings(
      DeserializedRemoteSettings deserializedRemoteSettings,
      string origin)
    {
      GroupedRemoteSettings groupedRemoteSettings = this;
      foreach (RemoteSetting setting in deserializedRemoteSettings.Settings)
      {
        setting.Origin = origin;
        RemoteSettingPossibilities settingPossibilities;
        if (!groupedRemoteSettings.TryGetValue(setting.Path, out settingPossibilities))
        {
          settingPossibilities = new RemoteSettingPossibilities();
          groupedRemoteSettings[setting.Path] = settingPossibilities;
        }
        List<RemoteSetting> remoteSettingList;
        if (!settingPossibilities.TryGetValue(setting.Name, out remoteSettingList))
        {
          remoteSettingList = new List<RemoteSetting>();
          settingPossibilities[setting.Name] = remoteSettingList;
        }
        remoteSettingList.Add(setting);
      }
    }

    public void Merge(GroupedRemoteSettings buckets, IRemoteSettingsLogger logger)
    {
      foreach (string key1 in buckets.Keys)
      {
        RemoteSettingPossibilities bucket = buckets[key1];
        RemoteSettingPossibilities settingPossibilities;
        if (!this.TryGetValue(key1, out settingPossibilities))
        {
          settingPossibilities = new RemoteSettingPossibilities();
          this[key1] = settingPossibilities;
        }
        foreach (string key2 in bucket.Keys)
        {
          List<RemoteSetting> remoteSettingList1 = bucket[key2];
          List<RemoteSetting> remoteSettingList2;
          if (!settingPossibilities.TryGetValue(key2, out remoteSettingList2))
          {
            remoteSettingList2 = new List<RemoteSetting>();
            settingPossibilities[key2] = remoteSettingList2;
          }
          for (int index1 = remoteSettingList1.Count - 1; index1 >= 0; --index1)
          {
            RemoteSetting remoteSetting = remoteSettingList1[index1];
            int index2 = remoteSettingList2.FindIndex((Predicate<RemoteSetting>) (s => s.Name == remoteSetting.Name && s.ScopeString == remoteSetting.ScopeString));
            if (index2 != -1)
            {
              logger.LogVerbose(string.Format("Overwriting RemoteSetting during merge: Old value from {0}", (object) remoteSettingList2[index2]));
              logger.LogVerbose(string.Format("Overwriting RemoteSetting during merge: New value from {0}", (object) remoteSetting));
              remoteSettingList2[index2] = remoteSetting;
            }
            else
              remoteSettingList2.Insert(0, remoteSetting);
          }
        }
      }
    }
  }
}
