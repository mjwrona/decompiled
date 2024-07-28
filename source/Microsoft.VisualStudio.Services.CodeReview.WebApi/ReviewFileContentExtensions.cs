// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ReviewFileContentExtensions
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public static class ReviewFileContentExtensions
  {
    public static byte[] ToSha1HashBytes(string contentHash)
    {
      byte[] bytes;
      if (HexConverter.TryToByteArray(contentHash, out bytes))
        return bytes;
      throw new FormatException(contentHash + " is not a well-formed hex string.");
    }

    public static string ToSha1HashString(byte[] contentHash) => HexConverter.ToString(contentHash);
  }
}
