// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Services.VersionedContent
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

namespace Microsoft.TeamFoundation.CodeSense.Server.Services
{
  public class VersionedContent
  {
    private CodeSenseResourceVersion version;

    public VersionedContent(string content, CodeSenseResourceVersion version)
      : this(content, version, true)
    {
    }

    public VersionedContent(string content, CodeSenseResourceVersion version, bool modified)
    {
      this.Content = content;
      this.version = version;
      this.Modified = modified;
    }

    public string Content { get; private set; }

    public bool Modified { get; private set; }

    public int Version => (int) this.version;
  }
}
