// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.AttachmentDictionaryComparer
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public sealed class AttachmentDictionaryComparer : IEqualityComparer<Attachment>
  {
    public static readonly AttachmentDictionaryComparer Instance = new AttachmentDictionaryComparer();

    private AttachmentDictionaryComparer()
    {
    }

    public bool Equals(Attachment obj1, Attachment obj2) => obj1 == null || obj2 == null ? obj1 == null && obj2 == null : obj1.ReviewId == obj2.ReviewId && obj1.Id == obj2.Id;

    public int GetHashCode(Attachment obj) => obj == null ? 0 : obj.ReviewId * 486187739 ^ obj.Id * 426188617;
  }
}
