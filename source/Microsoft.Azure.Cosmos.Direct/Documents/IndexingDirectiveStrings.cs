// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexingDirectiveStrings
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal static class IndexingDirectiveStrings
  {
    public static readonly string Default = IndexingDirective.Default.ToString();
    public static readonly string Include = IndexingDirective.Include.ToString();
    public static readonly string Exclude = IndexingDirective.Exclude.ToString();

    public static string FromIndexingDirective(IndexingDirective directive)
    {
      switch (directive)
      {
        case IndexingDirective.Default:
          return IndexingDirectiveStrings.Default;
        case IndexingDirective.Include:
          return IndexingDirectiveStrings.Include;
        case IndexingDirective.Exclude:
          return IndexingDirectiveStrings.Exclude;
        default:
          throw new ArgumentException(string.Format("Missing indexing directive string for {0}", (object) directive));
      }
    }
  }
}
