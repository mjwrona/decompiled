// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CacheFileInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal struct CacheFileInfo
  {
    internal string m_repositoryId;
    internal int m_fileId;
    internal long m_lastAccessTime;
    internal long m_fileSize;

    internal CacheFileInfo(string repositoryId, int fileId, long lastAccessTime, long fileSize)
    {
      this.m_repositoryId = repositoryId;
      this.m_fileId = fileId;
      this.m_lastAccessTime = lastAccessTime;
      this.m_fileSize = fileSize;
    }
  }
}
