// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.DownloadUrl
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public struct DownloadUrl
  {
    private string m_url;
    private int m_fileIdInsertPos;
    private int m_fileId;
    [ThreadStatic]
    private static string ts_lastParsedUrl;
    private const string c_fileIdSearchString = "&fid=";

    public DownloadUrl(string url)
    {
      this.m_url = url;
      this.m_fileId = 0;
      this.m_fileIdInsertPos = -1;
      if (string.IsNullOrEmpty(url))
        return;
      int num1;
      if (url.Length >= "&fid=".Length - 1 && string.Compare(url, 0, "&fid=", 1, "&fid=".Length - 1, StringComparison.Ordinal) == 0)
      {
        num1 = "&fid=".Length - 1;
      }
      else
      {
        int num2 = url.IndexOf("&fid=", StringComparison.Ordinal);
        if (num2 < 0)
          return;
        num1 = num2 + "&fid=".Length;
      }
      if (num1 == url.Length)
        return;
      int startIndex = url.IndexOf('&', num1);
      if (startIndex < 0)
        startIndex = url.Length;
      string s = url.Substring(num1, startIndex - num1);
      if (string.IsNullOrEmpty(s) || !int.TryParse(s, out this.m_fileId))
        return;
      StringBuilder stringBuilder = new StringBuilder(url.Length - s.Length);
      stringBuilder.Append(url, 0, num1);
      if (startIndex < url.Length)
        stringBuilder.Append(url, startIndex, url.Length - startIndex);
      this.m_url = stringBuilder.ToString();
      this.m_fileIdInsertPos = num1;
      if (DownloadUrl.ts_lastParsedUrl != null && string.Equals(this.m_url, DownloadUrl.ts_lastParsedUrl))
        this.m_url = DownloadUrl.ts_lastParsedUrl;
      else
        DownloadUrl.ts_lastParsedUrl = this.m_url;
    }

    public bool IsContentDestroyed => this.m_url != null && this.m_fileId == 1023;

    public string Url => this.m_url != null && this.m_fileIdInsertPos >= 0 ? this.m_url.Insert(this.m_fileIdInsertPos, this.m_fileId.ToString((IFormatProvider) CultureInfo.InvariantCulture)) : this.m_url;
  }
}
