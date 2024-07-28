// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.MustacheTextWriter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public sealed class MustacheTextWriter : TextWriter
  {
    private readonly TextWriter m_writer;
    private readonly MustacheOptions m_options;
    private int m_resultLength;

    public MustacheTextWriter(TextWriter writer, MustacheOptions options)
    {
      this.m_writer = writer;
      this.m_options = options;
    }

    public int ResultLength => this.m_resultLength;

    public override Encoding Encoding => this.m_writer.Encoding;

    public override void Write(char value)
    {
      if (++this.m_resultLength > this.m_options.MaxResultLength)
        throw new MustacheEvaluationResultLengthException(WebApiResources.MustacheEvaluationResultLengthExceeded((object) this.m_options.MaxResultLength));
      this.m_writer.Write(value);
    }

    public void Write(string value, bool encode)
    {
      if (encode)
        value = this.m_options.EncodeMethod(value);
      this.Write(value);
    }

    public override void Flush() => this.m_writer.Flush();

    public override Task FlushAsync() => this.m_writer.FlushAsync();
  }
}
