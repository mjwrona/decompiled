// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils.TextEncodingEntry
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils
{
  internal class TextEncodingEntry
  {
    private readonly byte[] m_preamble;
    private readonly Encoding m_encoding;

    public TextEncodingEntry(byte[] preamble, Encoding encoding)
    {
      if (preamble == null)
        throw new ArgumentNullException(nameof (preamble));
      if (encoding == null)
        throw new ArgumentNullException(nameof (encoding));
      this.m_preamble = preamble;
      this.m_encoding = encoding;
    }

    public byte[] Preamble => this.m_preamble;

    public Encoding Encoding => this.m_encoding;
  }
}
