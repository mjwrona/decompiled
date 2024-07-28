// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ContentBytes
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class ContentBytes
  {
    public ContentBytes(byte[] content)
      : this(content, false)
    {
    }

    public ContentBytes(byte[] content, bool areBytesCompressed)
    {
      this.Content = content ?? throw new ArgumentNullException(nameof (content));
      this.AreBytesCompressed = areBytesCompressed;
    }

    public byte[] Content { get; }

    public bool AreBytesCompressed { get; }
  }
}
