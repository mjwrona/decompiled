// Decompiled with JetBrains decompiler
// Type: WebGrease.CacheVaryByFile
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace WebGrease
{
  public class CacheVaryByFile
  {
    private CacheVaryByFile()
    {
    }

    public string Hash { get; private set; }

    public string Path { get; private set; }

    public static CacheVaryByFile FromFile(IWebGreaseContext context, ContentItem contentItem) => new CacheVaryByFile()
    {
      Path = contentItem.RelativeContentPath,
      Hash = contentItem.GetContentHash(context)
    };
  }
}
