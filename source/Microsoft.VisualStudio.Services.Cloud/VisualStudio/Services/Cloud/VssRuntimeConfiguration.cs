// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VssRuntimeConfiguration
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class VssRuntimeConfiguration : IVssRuntimeConfiguration
  {
    private readonly Dictionary<string, string> m_settings;

    public VssRuntimeConfiguration(string[] files)
    {
      this.m_settings = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      XmlReaderSettings settings = new XmlReaderSettings()
      {
        DtdProcessing = DtdProcessing.Prohibit
      };
      foreach (string file in files)
      {
        using (XmlReader xmlReader = XmlReader.Create(file, settings))
        {
          while (xmlReader.Read())
          {
            if (xmlReader.IsStartElement() && xmlReader.Name == "add")
              this.m_settings.Add(xmlReader.GetAttribute("key"), xmlReader.GetAttribute("value"));
          }
        }
      }
    }

    public IReadOnlyDictionary<string, string> Settings => (IReadOnlyDictionary<string, string>) this.m_settings;

    public bool GetBooleanSetting(string settingName, bool defaultValue)
    {
      string str;
      bool result;
      return this.m_settings.TryGetValue(settingName, out str) && bool.TryParse(str, out result) ? result : defaultValue;
    }

    public string GetDecryptedString(string settingName)
    {
      string encryptedSecret;
      return this.m_settings.TryGetValue(settingName, out encryptedSecret) ? EncryptionUtility.TryDecryptSecretInsecure(encryptedSecret) : (string) null;
    }

    public string GetStringSetting(string settingName, string defaultValue = null)
    {
      string str;
      return this.m_settings.TryGetValue(settingName, out str) ? str : defaultValue;
    }
  }
}
