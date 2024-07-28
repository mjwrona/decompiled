// Decompiled with JetBrains decompiler
// Type: Nest.SlackAttachment
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SlackAttachment : ISlackAttachment
  {
    public string AuthorIcon { get; set; }

    public string AuthorLink { get; set; }

    public string AuthorName { get; set; }

    public string Color { get; set; }

    public string Fallback { get; set; }

    public IEnumerable<ISlackAttachmentField> Fields { get; set; }

    public string Footer { get; set; }

    public string FooterIcon { get; set; }

    public string ImageUrl { get; set; }

    public string Pretext { get; set; }

    public string Text { get; set; }

    public string ThumbUrl { get; set; }

    public string Title { get; set; }

    public string TitleLink { get; set; }

    public DateTimeOffset? Ts { get; set; }
  }
}
