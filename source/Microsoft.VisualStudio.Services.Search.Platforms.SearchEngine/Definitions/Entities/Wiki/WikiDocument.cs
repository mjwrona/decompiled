// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki.WikiDocument
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki
{
  public class WikiDocument
  {
    public string Content { get; set; }

    public string ContentMetadata { get; set; }

    public string[] ContentLinks { get; set; }

    public string ContentId { get; set; }

    public DateTime LastUpdated { get; set; }

    public string FileName { get; set; }

    public string FilePath { get; set; }

    public string FileExtension { get; set; }

    public string[] TagsField { get; set; }

    public string CollectionUrl { get; set; }

    public string WikiId { get; set; }

    public string WikiName { get; set; }

    public string MappedPath { get; set; }
  }
}
