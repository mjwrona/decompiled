// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Crawler.Entities.DiscussionEntity
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

namespace Microsoft.VisualStudio.Services.Search.Crawler.Entities
{
  public class DiscussionEntity
  {
    public int Id { get; private set; }

    public int Revision { get; private set; }

    public string Discussion { get; private set; }

    public DiscussionEntity(int id, int revision, string discussion)
    {
      this.Id = id;
      this.Revision = revision;
      this.Discussion = discussion;
    }
  }
}
