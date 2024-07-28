// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFileNtfsAttributesHelper
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Storage.File
{
  internal class CloudFileNtfsAttributesHelper
  {
    private static Dictionary<CloudFileNtfsAttributes, string> directory = new Dictionary<CloudFileNtfsAttributes, string>()
    {
      {
        CloudFileNtfsAttributes.ReadOnly,
        "ReadOnly"
      },
      {
        CloudFileNtfsAttributes.Hidden,
        "Hidden"
      },
      {
        CloudFileNtfsAttributes.System,
        "System"
      },
      {
        CloudFileNtfsAttributes.Normal,
        "None"
      },
      {
        CloudFileNtfsAttributes.Directory,
        "Directory"
      },
      {
        CloudFileNtfsAttributes.Archive,
        "Archive"
      },
      {
        CloudFileNtfsAttributes.Temporary,
        "Temporary"
      },
      {
        CloudFileNtfsAttributes.Offline,
        "Offline"
      },
      {
        CloudFileNtfsAttributes.NotContentIndexed,
        "NotContentIndexed"
      },
      {
        CloudFileNtfsAttributes.NoScrubData,
        "NoScrubData"
      }
    };

    internal static string ToString(CloudFileNtfsAttributes attributes) => string.Join("|", CloudFileNtfsAttributesHelper.directory.Select<KeyValuePair<CloudFileNtfsAttributes, string>, string>((Func<KeyValuePair<CloudFileNtfsAttributes, string>, string>) (r => attributes.HasFlag((Enum) r.Key) ? r.Value : (string) null)).Where<string>((Func<string, bool>) (r => r != null)));

    internal static CloudFileNtfsAttributes? ToAttributes(string attributesString)
    {
      if (attributesString == null)
        return new CloudFileNtfsAttributes?();
      CloudFileNtfsAttributes fileNtfsAttributes = CloudFileNtfsAttributes.None;
      string str1 = attributesString;
      char[] chArray = new char[1]{ '|' };
      foreach (string str2 in str1.Split(chArray))
      {
        string trimmed = str2.Trim();
        fileNtfsAttributes |= CloudFileNtfsAttributesHelper.directory.FirstOrDefault<KeyValuePair<CloudFileNtfsAttributes, string>>((Func<KeyValuePair<CloudFileNtfsAttributes, string>, bool>) (r => r.Value == trimmed)).Key;
      }
      return new CloudFileNtfsAttributes?(fileNtfsAttributes);
    }
  }
}
