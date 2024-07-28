// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheOptions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.ComponentModel;
using System.Threading;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class MustacheOptions
  {
    private MustacheEncodeMethod m_encodeMethod;

    public MustacheOptions()
    {
      this.MaxDepth = int.MaxValue;
      this.MaxResultLength = int.MaxValue;
    }

    public CancellationToken CancellationToken { get; set; }

    public bool DisableInlinePartials { get; set; }

    public MustacheEncodeMethod EncodeMethod
    {
      get => this.m_encodeMethod ?? MustacheOptions.\u003C\u003EO.\u003C0\u003E__HtmlEncode ?? (MustacheOptions.\u003C\u003EO.\u003C0\u003E__HtmlEncode = new MustacheEncodeMethod(MustacheEncodeMethods.HtmlEncode));
      set => this.m_encodeMethod = value;
    }

    public int MaxDepth { get; set; }

    public int MaxResultLength { get; set; }
  }
}
