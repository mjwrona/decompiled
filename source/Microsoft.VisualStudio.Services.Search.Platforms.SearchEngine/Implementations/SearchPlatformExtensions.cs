// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.SearchPlatformExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  public static class SearchPlatformExtensions
  {
    public static int GetIndexVersion(
      this ISearchPlatform searchPlatform,
      IEntityType entityType,
      string indexName)
    {
      if (string.IsNullOrWhiteSpace(indexName))
        throw new ArgumentException("Index name is invalid", nameof (indexName));
      object obj;
      searchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexName)).GetMetadata().TryGetValue("version", out obj);
      if (obj == null)
        obj = (object) indexName.Split('_')[2];
      if (obj != null)
      {
        try
        {
          return Convert.ToInt32(obj, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch (FormatException ex)
        {
          Tracer.TraceError(1080029, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("Found invalid index version [{0}] for index [{1}].", obj, (object) indexName)));
        }
      }
      return SearchPlatformExtensions.GetDefaultIndexVersion(entityType);
    }

    private static int GetDefaultIndexVersion(IEntityType entityType)
    {
      string name = entityType.Name;
      if (name != null)
      {
        switch (name.Length)
        {
          case 4:
            switch (name[0])
            {
              case 'C':
                if (name == "Code")
                  break;
                goto label_18;
              case 'W':
                if (name == "Wiki")
                  goto label_15;
                else
                  goto label_18;
              default:
                goto label_18;
            }
            break;
          case 5:
            if (name == "Board")
              return 1551;
            goto label_18;
          case 7:
            if (name == "Package")
              return 2;
            goto label_18;
          case 8:
            if (name == "WorkItem")
              return 4;
            goto label_18;
          case 10:
            switch (name[6])
            {
              case 'C':
                if (name == "TenantCode")
                  break;
                goto label_18;
              case 'W':
                if (name == "TenantWiki")
                  goto label_15;
                else
                  goto label_18;
              default:
                goto label_18;
            }
            break;
          case 11:
            if (name == "ProjectRepo")
              return 1310;
            goto label_18;
          default:
            goto label_18;
        }
        return 0;
label_15:
        return 1311;
      }
label_18:
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Versioning of entity type [{0}] was not supported as of releases/M140.", (object) entityType)));
    }
  }
}
