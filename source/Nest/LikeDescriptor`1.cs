// Decompiled with JetBrains decompiler
// Type: Nest.LikeDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class LikeDescriptor<T> : DescriptorPromiseBase<LikeDescriptor<T>, List<Like>> where T : class
  {
    public LikeDescriptor()
      : base(new List<Like>())
    {
    }

    public LikeDescriptor<T> Text(string likeText) => this.Assign<string>(likeText, (Action<List<Like>, string>) ((a, v) => a.Add((Like) v)));

    public LikeDescriptor<T> Document(
      Func<LikeDocumentDescriptor<T>, ILikeDocument> selector)
    {
      ILikeDocument likeDocument = selector != null ? selector(new LikeDocumentDescriptor<T>()) : (ILikeDocument) null;
      return likeDocument != null ? this.Assign<ILikeDocument>(likeDocument, (Action<List<Like>, ILikeDocument>) ((a, v) => a.Add(new Like(v)))) : this;
    }
  }
}
