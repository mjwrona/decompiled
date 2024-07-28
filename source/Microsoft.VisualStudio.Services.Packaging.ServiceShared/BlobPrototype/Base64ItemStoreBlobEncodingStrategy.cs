// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Base64ItemStoreBlobEncodingStrategy
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class Base64ItemStoreBlobEncodingStrategy : IItemStoreBlobEncodingStrategy
  {
    public string Encode(Stream input)
    {
      using (input)
      {
        using (MemoryStream destination = new MemoryStream())
        {
          input.CopyTo((Stream) destination);
          return Convert.ToBase64String(destination.ToArray());
        }
      }
    }

    public Stream Decode(string input) => (Stream) new MemoryStream(Convert.FromBase64String(input), false);

    public void DecodeTo(string input, Stream stream) => this.Decode(input).CopyTo(stream);

    public bool CanOnlyEncodeUtf8Text => false;
  }
}
