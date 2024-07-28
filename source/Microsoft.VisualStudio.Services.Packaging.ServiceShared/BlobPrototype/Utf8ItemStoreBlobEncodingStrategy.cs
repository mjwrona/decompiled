// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Utf8ItemStoreBlobEncodingStrategy
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class Utf8ItemStoreBlobEncodingStrategy : IItemStoreBlobEncodingStrategy
  {
    private static readonly Encoding Utf8NoBom = (Encoding) new UTF8Encoding(false);

    public string Encode(Stream input)
    {
      using (StreamReader streamReader = new StreamReader(input, Utf8ItemStoreBlobEncodingStrategy.Utf8NoBom, false, 10000, false))
        return streamReader.ReadToEnd();
    }

    public Stream Decode(string input) => (Stream) new MemoryStream(Utf8ItemStoreBlobEncodingStrategy.Utf8NoBom.GetBytes(input), false);

    public void DecodeTo(string input, Stream stream) => this.Decode(input).CopyTo(stream);

    public bool CanOnlyEncodeUtf8Text => true;
  }
}
