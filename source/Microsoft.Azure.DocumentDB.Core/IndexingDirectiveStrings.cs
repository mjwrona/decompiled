// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.IndexingDirectiveStrings
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
