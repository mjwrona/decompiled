// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DeterminedItem
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class DeterminedItem
  {
    private string m_queryPath;
    private string m_filePattern;

    public string QueryPath
    {
      get => this.m_queryPath;
      set => this.m_queryPath = value;
    }

    public string FilePattern
    {
      get => this.m_filePattern;
      set => this.m_filePattern = value;
    }
  }
}
