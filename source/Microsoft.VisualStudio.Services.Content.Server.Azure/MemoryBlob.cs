// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.MemoryBlob
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public sealed class MemoryBlob
  {
    private static int lastEtag;
    public readonly Dictionary<string, string> Metadata = new Dictionary<string, string>();
    public readonly string ETag;
    public readonly string ContentEncoding;
    public readonly MemoryStream Content;

    public MemoryBlob(
      string contentEncoding,
      Dictionary<string, string> metadata,
      MemoryStream content)
    {
      this.ETag = Interlocked.Increment(ref MemoryBlob.lastEtag).ToString();
      this.ContentEncoding = contentEncoding;
      this.Metadata = metadata;
      this.Content = content;
    }
  }
}
