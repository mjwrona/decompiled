// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.BundleTextStreamProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public class BundleTextStreamProvider : IBundleStreamProvider
  {
    public byte[] Bytes { get; private set; }

    public long Length { get; private set; }

    public BundleTextStreamProvider(string text)
      : this(text, Encoding.UTF8, true)
    {
    }

    public BundleTextStreamProvider(string text, bool appendNewLine)
      : this(text, Encoding.UTF8, appendNewLine)
    {
    }

    public BundleTextStreamProvider(string text, Encoding encoding, bool appendNewLine)
    {
      text = text ?? string.Empty;
      if (appendNewLine && !string.IsNullOrEmpty(text))
        text += Environment.NewLine;
      this.Bytes = encoding.GetBytes(text);
      this.Length = (long) this.Bytes.Length;
    }

    public Stream GetStream() => (Stream) new MemoryStream(this.Bytes);
  }
}
