// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssScriptFileInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class VssScriptFileInfo
  {
    private static ConcurrentDictionary<string, VssScriptFileInfo> s_fileInfoCache = new ConcurrentDictionary<string, VssScriptFileInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static ConcurrentDictionary<string, VssScriptFileInfo> s_fileInfoSiteRootRelativePathCache = new ConcurrentDictionary<string, VssScriptFileInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private string m_firstLine;
    private object m_hashCodeSyncObject = new object();
    private byte[] m_contentHash;

    private VssScriptFileInfo(string path) => this.FileInfo = new FileInfo(path);

    public FileInfo FileInfo { get; private set; }

    public byte[] HashCode => this.m_contentHash;

    public string ReadFirstLine()
    {
      if (this.m_firstLine == null)
      {
        if (this.FileInfo.Exists)
        {
          string str;
          using (StreamReader streamReader = new StreamReader(this.FileInfo.FullName, Encoding.UTF8))
            str = streamReader.ReadLine();
          this.m_firstLine = str ?? string.Empty;
        }
        else
          this.m_firstLine = string.Empty;
      }
      return this.m_firstLine;
    }

    public string[] ReadLinesAndSetHash(Dictionary<string, string[]> contentCache)
    {
      string[] lines;
      if (!contentCache.TryGetValue(this.FileInfo.FullName, out lines))
        lines = !this.FileInfo.Exists ? Array.Empty<string>() : File.ReadAllLines(this.FileInfo.FullName);
      if (this.m_contentHash == null)
      {
        lock (this.m_hashCodeSyncObject)
        {
          if (this.m_contentHash == null)
            this.m_contentHash = BundlingHelper.CalculateHashFromLines(lines);
        }
      }
      return lines;
    }

    public static VssScriptFileInfo GetVssScriptFile(string path)
    {
      if (!Path.IsPathRooted(path))
        path = Path.GetFullPath(path);
      VssScriptFileInfo vssScriptFile;
      if (VssScriptFileInfo.s_fileInfoCache.TryGetValue(path, out vssScriptFile))
        return vssScriptFile;
      vssScriptFile = new VssScriptFileInfo(path);
      VssScriptFileInfo.s_fileInfoCache[path] = vssScriptFile;
      return vssScriptFile;
    }

    public static VssScriptFileInfo GetVssScriptFileFromSitesRootRelativePath(
      string siteRootRelativePath)
    {
      VssScriptFileInfo vssScriptFile;
      if (!VssScriptFileInfo.s_fileInfoSiteRootRelativePathCache.TryGetValue(siteRootRelativePath, out vssScriptFile) && HttpContext.Current != null)
      {
        vssScriptFile = VssScriptFileInfo.GetVssScriptFile(HttpContext.Current.Server.MapPath(siteRootRelativePath));
        VssScriptFileInfo.s_fileInfoSiteRootRelativePathCache[siteRootRelativePath] = vssScriptFile;
      }
      return vssScriptFile;
    }
  }
}
