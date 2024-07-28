// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AssetInfo
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class AssetInfo
  {
    private const string s_languageSeparator = "::";
    private string m_assetString;

    public AssetInfo(string assetType, string language)
    {
      this.AssetType = assetType;
      this.Language = language;
      if (!string.IsNullOrEmpty(language))
        this.m_assetString = string.Format("{0}{1}{2}", (object) assetType, (object) "::", (object) language);
      else
        this.m_assetString = assetType;
    }

    public AssetInfo(string assetString)
    {
      this.m_assetString = assetString;
      if (string.IsNullOrEmpty(assetString))
        return;
      int length;
      if ((length = assetString.IndexOf("::", StringComparison.OrdinalIgnoreCase)) != -1)
      {
        this.AssetType = assetString.Substring(0, length);
        this.Language = assetString.Substring(length + 2);
      }
      else
        this.AssetType = assetString;
    }

    public string AssetType { get; private set; }

    public string Language { get; private set; }

    public override string ToString() => this.m_assetString;
  }
}
