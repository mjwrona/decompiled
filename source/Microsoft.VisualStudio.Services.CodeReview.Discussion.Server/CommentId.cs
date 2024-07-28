// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.CommentId
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  [XmlType("CommentId")]
  public class CommentId
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public short Id { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int DiscussionId { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Comment: Id {0}, DiscussionId {1}]", (object) this.Id, (object) this.DiscussionId);

    public class Comparer : IEqualityComparer<CommentId>
    {
      public bool Equals(CommentId x, CommentId y) => x.DiscussionId == y.DiscussionId && (int) x.Id == (int) y.Id;

      public int GetHashCode(CommentId commentId) => commentId.DiscussionId * (int) commentId.Id;
    }
  }
}
