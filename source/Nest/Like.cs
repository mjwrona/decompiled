// Decompiled with JetBrains decompiler
// Type: Nest.Like
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;

namespace Nest
{
  [JsonFormatter(typeof (LikeFormatter))]
  public class Like : Union<string, ILikeDocument>
  {
    public Like(string item)
      : base(item)
    {
    }

    public Like(ILikeDocument item)
      : base(item)
    {
    }

    public static implicit operator Like(string likeText) => new Like(likeText);

    public static implicit operator Like(LikeDocumentBase like) => new Like((ILikeDocument) like);

    internal static bool IsConditionless(Like like)
    {
      if (!like.Item1.IsNullOrEmpty())
        return false;
      if (like.Item2 == null)
        return true;
      return like.Item2.Id == (Id) null && like.Item2.Document == null;
    }
  }
}
