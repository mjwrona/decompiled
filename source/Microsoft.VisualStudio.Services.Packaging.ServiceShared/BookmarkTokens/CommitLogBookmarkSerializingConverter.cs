// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens.CommitLogBookmarkSerializingConverter
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Newtonsoft.Json;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BookmarkTokens
{
  public class CommitLogBookmarkSerializingConverter : 
    IConverter<CommitLogBookmark, string>,
    IHaveInputType<CommitLogBookmark>,
    IHaveOutputType<string>
  {
    public string Convert(CommitLogBookmark input) => Base64UrlStringCodec.ToBase64UrlString(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) input)));
  }
}
