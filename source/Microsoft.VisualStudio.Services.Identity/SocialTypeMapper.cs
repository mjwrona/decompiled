// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SocialTypeMapper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class SocialTypeMapper
  {
    public static readonly SocialTypeMapper Instance = new SocialTypeMapper();
    private string[] socialNameFromId;
    private IReadOnlyDictionary<string, byte> socialIdFromName = (IReadOnlyDictionary<string, byte>) new ReadOnlyDictionary<string, byte>((IDictionary<string, byte>) new Dictionary<string, byte>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "ghb",
        (byte) 1
      }
    });

    private SocialTypeMapper()
    {
      this.socialNameFromId = new string[256];
      foreach (KeyValuePair<string, byte> keyValuePair in (IEnumerable<KeyValuePair<string, byte>>) this.socialIdFromName)
        this.socialNameFromId[(int) keyValuePair.Value] = keyValuePair.Key;
    }

    public byte GetSocialIdFromName(string socialTypeName)
    {
      ArgumentUtility.CheckForNull<string>(socialTypeName, nameof (socialTypeName));
      byte maxValue;
      if (!this.socialIdFromName.TryGetValue(socialTypeName, out maxValue))
        maxValue = byte.MaxValue;
      return maxValue;
    }

    public string GetSocialNameFromId(byte typeId) => this.socialNameFromId[(int) typeId] ?? "ukn";

    public bool IsValidSocialTypeName(string socialTypeName) => !string.IsNullOrEmpty(socialTypeName) && this.GetSocialIdFromName(socialTypeName) != byte.MaxValue;

    private enum SocialTypeId : byte
    {
      GitHub = 1,
      Unknown = 255, // 0xFF
    }
  }
}
