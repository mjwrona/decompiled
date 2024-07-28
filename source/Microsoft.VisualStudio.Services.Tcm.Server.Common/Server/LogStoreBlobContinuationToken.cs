// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreBlobContinuationToken
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreBlobContinuationToken : ILogStoreBlobContinuationToken
  {
    private BlobContinuationToken _blobContinuationToken;
    private string _token;

    public LogStoreBlobContinuationToken(string token)
    {
      this._token = token;
      if (string.IsNullOrWhiteSpace(token))
        return;
      using (StringReader stringReader = new StringReader(token))
      {
        this._blobContinuationToken = new BlobContinuationToken();
        StringReader input = stringReader;
        using (XmlReader reader = XmlReader.Create((TextReader) input, new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null,
          Async = true
        }))
          this._blobContinuationToken.ReadXmlAsync(reader).GetAwaiter().GetResult();
      }
    }

    public LogStoreBlobContinuationToken(BlobContinuationToken token)
    {
      this._blobContinuationToken = token;
      this._token = (string) null;
      if (token == null)
        return;
      using (StringWriter output = new StringWriter())
      {
        using (XmlWriter writer = XmlWriter.Create((TextWriter) output))
          token.WriteXml(writer);
        this._token = output.ToString();
      }
    }

    public BlobContinuationToken GetBlobContinuationToken() => this._blobContinuationToken;

    public string GetContinuationToken() => this._token;
  }
}
