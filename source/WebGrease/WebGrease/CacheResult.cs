// Decompiled with JetBrains decompiler
// Type: WebGrease.CacheResult
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace WebGrease
{
  public class CacheResult
  {
    private CacheResult()
    {
    }

    public string RelativeContentPath { get; private set; }

    public string RelativeHashedContentPath { get; private set; }

    public string CachedFilePath { get; private set; }

    public string FileCategory { get; private set; }

    public string ContentHash { get; private set; }

    public bool EndResult { get; private set; }

    public static CacheResult FromContentFile(
      IWebGreaseContext context,
      string cacheCategory,
      bool endResult,
      string fileCategory,
      ContentItem contentItem)
    {
      return new CacheResult()
      {
        EndResult = endResult,
        FileCategory = fileCategory,
        CachedFilePath = context.Cache.StoreInCache(cacheCategory, contentItem),
        ContentHash = contentItem.GetContentHash(context),
        RelativeContentPath = contentItem.RelativeContentPath,
        RelativeHashedContentPath = contentItem.RelativeHashedContentPath
      };
    }
  }
}
