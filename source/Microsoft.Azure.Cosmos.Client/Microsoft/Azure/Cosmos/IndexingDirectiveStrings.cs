// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.IndexingDirectiveStrings
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos
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
