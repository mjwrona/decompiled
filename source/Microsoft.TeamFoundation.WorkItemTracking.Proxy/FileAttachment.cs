// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Proxy.FileAttachment
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF15D8B4-8AC0-4915-8153-9054E8546EA2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Proxy.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.WorkItemTracking.Proxy
{
  public struct FileAttachment
  {
    private Guid m_fileNameGUID;
    private string m_areaNodeUri;
    private string m_projectUri;
    private Stream m_localFile;

    public Guid FileNameGUID
    {
      get => this.m_fileNameGUID;
      set => this.m_fileNameGUID = value;
    }

    public string AreaNodeUri
    {
      get => this.m_areaNodeUri;
      set => this.m_areaNodeUri = value;
    }

    public string ProjectUri
    {
      get => this.m_projectUri;
      set => this.m_projectUri = value;
    }

    public Stream LocalFile
    {
      get => this.m_localFile;
      set => this.m_localFile = value;
    }
  }
}
