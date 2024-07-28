// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiAttachmentChange
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiAttachmentChange : WikiChangeBase
  {
    public string Name { get; set; }

    public string Path { get; set; }

    public string Content { get; set; }
  }
}
