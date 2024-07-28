// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsDiagnosticHelper
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TfsDiagnosticHelper
  {
    public static void WriteRegKeyContent(RegistryKey key, XmlWriter writer)
    {
      using (new XmlElementWriterUtility("Key", writer))
      {
        try
        {
          writer.WriteAttributeString("Name", key.Name);
          TfsDiagnosticHelper.WriteRegKeyValues(key, writer);
          TfsDiagnosticHelper.WriteRegSubKeys(key, writer);
        }
        catch (Exception ex)
        {
          using (new XmlElementWriterUtility("Exception", writer))
          {
            writer.WriteAttributeString("Message", ex.Message);
            writer.WriteAttributeString("Type", ex.GetType().ToString());
          }
        }
      }
    }

    public static void GetGuidsInDefaultValues(RegistryKey key, List<Guid> guids)
    {
      string g = (string) key.GetValue((string) null, (object) string.Empty);
      try
      {
        if (!string.IsNullOrEmpty(g))
        {
          Guid guid = new Guid(g);
          if (!guids.Contains(guid))
            guids.Add(guid);
        }
      }
      catch
      {
      }
      string[] subKeyNames = key.GetSubKeyNames();
      if (subKeyNames.Length == 0)
        return;
      foreach (string name in subKeyNames)
      {
        using (RegistryKey key1 = key.OpenSubKey(name))
          TfsDiagnosticHelper.GetGuidsInDefaultValues(key1, guids);
      }
    }

    private static void WriteRegKeyValues(RegistryKey key, XmlWriter writer)
    {
      string[] valueNames = key.GetValueNames();
      if (valueNames.Length == 0)
        return;
      using (new XmlElementWriterUtility("Values", writer))
      {
        foreach (string name in valueNames)
        {
          using (new XmlElementWriterUtility("Value", writer))
          {
            writer.WriteAttributeString("Name", name);
            writer.WriteAttributeString("Value", key.GetValue(name).ToString());
            writer.WriteAttributeString("Type", key.GetValueKind(name).ToString());
          }
        }
      }
    }

    private static void WriteRegSubKeys(RegistryKey key, XmlWriter writer)
    {
      string[] subKeyNames = key.GetSubKeyNames();
      if (subKeyNames.Length == 0)
        return;
      using (new XmlElementWriterUtility("SubKeys", writer))
      {
        foreach (string name in subKeyNames)
        {
          using (RegistryKey key1 = key.OpenSubKey(name))
            TfsDiagnosticHelper.WriteRegKeyContent(key1, writer);
        }
      }
    }
  }
}
