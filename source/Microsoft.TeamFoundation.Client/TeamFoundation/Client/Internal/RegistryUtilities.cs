// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Internal.RegistryUtilities
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.Win32;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RegistryUtilities
  {
    public static object GetUserOrApplicationRegistryValue(string keyPath, string name) => RegistryUtilities.GetUserRegistryValue(keyPath, name) ?? RegistryUtilities.GetApplicationRegistryValue(keyPath, name);

    public static object GetApplicationRegistryValue(string keyPath, string name)
    {
      if (keyPath == null)
        throw new ArgumentNullException(nameof (keyPath));
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      try
      {
        using (RegistryKey applicationRegistryRoot = UIHost.TryGetApplicationRegistryRoot())
        {
          if (applicationRegistryRoot != null)
          {
            using (RegistryKey registryKey = applicationRegistryRoot.OpenSubKey(keyPath))
            {
              if (registryKey != null)
                return registryKey.GetValue(name);
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceAndDebugFailException(ex);
      }
      return (object) null;
    }

    public static object GetUserRegistryValue(string keyPath, string name)
    {
      if (keyPath == null)
        throw new ArgumentNullException(nameof (keyPath));
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      try
      {
        using (RegistryKey userRegistryRoot = UIHost.TryGetUserRegistryRoot())
        {
          if (userRegistryRoot != null)
          {
            using (RegistryKey registryKey = userRegistryRoot.OpenSubKey(keyPath))
            {
              if (registryKey != null)
                return registryKey.GetValue(name);
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceAndDebugFailException(ex);
      }
      return (object) null;
    }

    public static void SetUserRegistryValue(string keyPath, string name, object value)
    {
      if (keyPath == null)
        throw new ArgumentNullException(nameof (keyPath));
      if (name == null)
        throw new ArgumentNullException(nameof (name));
      try
      {
        using (RegistryKey userRegistryRoot = UIHost.TryGetUserRegistryRoot())
        {
          if (userRegistryRoot == null)
            return;
          using (RegistryKey subKey = userRegistryRoot.CreateSubKey(keyPath))
          {
            if (subKey == null)
              return;
            if (value != null)
              subKey.SetValue(name, value);
            else
              subKey.DeleteValue(name);
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceAndDebugFailException(ex);
      }
    }
  }
}
