// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DirectoryEntryFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class DirectoryEntryFactory
  {
    private static readonly Stopwatch s_stopwatch = new Stopwatch();
    private static bool? m_bUseLdaps;

    public static DirectoryEntry CreateDirectoryEntry(string path)
    {
      DirectoryEntry directoryEntry = new DirectoryEntry(path);
      if (DirectoryEntryFactory.UseLdaps())
        directoryEntry.AuthenticationType |= AuthenticationTypes.Encryption;
      return directoryEntry;
    }

    public static DirectoryEntry CreateDirectoryEntry(object adsObject)
    {
      DirectoryEntry directoryEntry = new DirectoryEntry(adsObject);
      if (DirectoryEntryFactory.UseLdaps())
        directoryEntry.AuthenticationType |= AuthenticationTypes.Encryption;
      return directoryEntry;
    }

    private static bool UseLdaps()
    {
      if (!DirectoryEntryFactory.m_bUseLdaps.HasValue || DirectoryEntryFactory.s_stopwatch.ElapsedMilliseconds > 30000L)
      {
        using (SafeHandle registryKey = RegistryHelper.OpenSubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\TeamFoundationServer", RegistryAccessMask.Execute | RegistryAccessMask.Wow6464Key))
        {
          DirectoryEntryFactory.m_bUseLdaps = registryKey == null ? new bool?(false) : new bool?((int) RegistryHelper.GetValue(registryKey, "UseLDAPS", (object) 0) != 0);
          DirectoryEntryFactory.s_stopwatch.Restart();
        }
      }
      return DirectoryEntryFactory.m_bUseLdaps.Value;
    }
  }
}
