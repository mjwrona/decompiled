// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ArtifactLink
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

namespace Microsoft.VisualStudio.Services.Common
{
  public class ArtifactLink
  {
    private string m_ReferringUri;
    private string m_LinkType;
    private string m_ReferencedUri;

    public string ReferringUri
    {
      get => this.m_ReferringUri;
      set => this.m_ReferringUri = value;
    }

    public string LinkType
    {
      get => this.m_LinkType;
      set => this.m_LinkType = value;
    }

    public string ReferencedUri
    {
      get => this.m_ReferencedUri;
      set => this.m_ReferencedUri = value;
    }
  }
}
