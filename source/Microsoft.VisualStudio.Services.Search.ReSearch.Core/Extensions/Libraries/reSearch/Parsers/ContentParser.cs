// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.ContentParser
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Utils;
using Microsoft.VisualStudio.Services.Search.ReSearch.Core.Parsers.ContentWriters;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  public abstract class ContentParser
  {
    public const int UnknownSize = -1;
    private int m_maxFileSizeSupportedInBytes;

    protected ContentParser(int maxFileSizeSupportedInBytes) => this.m_maxFileSizeSupportedInBytes = maxFileSizeSupportedInBytes;

    protected internal string Content { get; private set; }

    protected bool UseTextTokenizer { get; private set; }

    public virtual IParsedContent Parse(byte[] content) => this.Parse(content, true);

    public virtual IParsedContent Parse(byte[] content, bool useTextTokenizer)
    {
      int num = content != null ? content.Length : throw new ArgumentNullException(nameof (content));
      TextEncoding textEncoding = new TextEncoding();
      int supportedInBytes = this.m_maxFileSizeSupportedInBytes;
      string content1;
      if (num > supportedInBytes)
      {
        content1 = textEncoding.GetString(((IEnumerable<byte>) content).Take<byte>(this.m_maxFileSizeSupportedInBytes).ToArray<byte>());
        Tracer.TraceWarning(1080153, "Indexing Pipeline", nameof (Parse), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Found Parser Content string of greater than 10MB, trimming the file to 10MB size."));
      }
      else
        content1 = textEncoding.GetString(content);
      return this.Parse(content1, useTextTokenizer);
    }

    protected abstract IParsedContent ParseContent();

    public IParsedContent Parse(string content, bool useTextTokenizer = true)
    {
      this.UseTextTokenizer = useTextTokenizer;
      this.Content = content ?? throw new ArgumentNullException(nameof (content));
      return this.ParseContent();
    }
  }
}
