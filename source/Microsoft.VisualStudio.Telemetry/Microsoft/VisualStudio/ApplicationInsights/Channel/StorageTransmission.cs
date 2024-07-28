// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Channel.StorageTransmission
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.ApplicationInsights.Channel
{
  internal class StorageTransmission : Transmission, IDisposable
  {
    private const int ConvertChunkSize = 30000;
    private const int BufferSize = 40004;
    internal Action<StorageTransmission> Disposing;

    protected StorageTransmission(
      string fullPath,
      Uri address,
      byte[] content,
      string contentType,
      string apiKey,
      string contentEncoding)
      : base(address, content, contentType, apiKey, contentEncoding)
    {
      this.FullFilePath = fullPath;
      this.FileName = Path.GetFileName(fullPath);
    }

    internal string FileName { get; private set; }

    internal string FullFilePath { get; private set; }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public override string ToString() => "FileName: " + this.FileName + ", ContentHash: " + this.ContentHash;

    internal static async Task<StorageTransmission> CreateFromStreamAsync(
      Stream stream,
      string fileName)
    {
      StreamReader reader = new StreamReader(stream);
      Uri address = await StorageTransmission.ReadAddressAsync((TextReader) reader).ConfigureAwait(false);
      string contentType = await StorageTransmission.ReadHeaderAsync((TextReader) reader, "Content-Type").ConfigureAwait(false);
      string contentEncoding = await StorageTransmission.ReadHeaderAsync((TextReader) reader, "Content-Encoding").ConfigureAwait(false);
      string apiKey = string.Empty;
      if ("https://events.data.microsoft.com/OneCollector/1.0".Equals(address.OriginalString))
      {
        address = new Uri("https://mobile.events.data.microsoft.com/OneCollector/1.0");
        apiKey = await StorageTransmission.ReadHeaderAsync((TextReader) reader, "EndPointAPIKey").ConfigureAwait(false);
        if (apiKey.IsNullOrWhiteSpace())
        {
          stream.Dispose();
          ReparsePointAware.DeleteFile(fileName);
          throw new InvalidOperationException();
        }
      }
      StorageTransmission fromStreamAsync = new StorageTransmission(fileName, address, await StorageTransmission.ReadContentAsync((TextReader) reader).ConfigureAwait(false), contentType, contentEncoding, apiKey);
      reader = (StreamReader) null;
      address = (Uri) null;
      contentType = (string) null;
      contentEncoding = (string) null;
      apiKey = (string) null;
      return fromStreamAsync;
    }

    internal static async Task SaveAsync(Transmission transmission, Stream stream)
    {
      StreamWriter writer = new StreamWriter(stream);
      try
      {
        ConfiguredTaskAwaitable configuredTaskAwaitable = writer.WriteLineAsync(transmission.EndpointAddress.ToString()).ConfigureAwait(false);
        await configuredTaskAwaitable;
        configuredTaskAwaitable = writer.WriteLineAsync("Content-Type:" + transmission.ContentType).ConfigureAwait(false);
        await configuredTaskAwaitable;
        configuredTaskAwaitable = writer.WriteLineAsync("Content-Encoding:" + transmission.ContentEncoding).ConfigureAwait(false);
        await configuredTaskAwaitable;
        if ("https://events.data.microsoft.com/OneCollector/1.0".Equals(transmission.EndpointAddress.OriginalString))
        {
          configuredTaskAwaitable = writer.WriteLineAsync(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", new object[2]
          {
            (object) "EndPointAPIKey",
            (object) transmission.ApiKey
          })).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
        configuredTaskAwaitable = writer.WriteLineAsync(string.Empty).ConfigureAwait(false);
        await configuredTaskAwaitable;
        if (transmission.Content.Length * 8 / 3 < 80000)
        {
          configuredTaskAwaitable = writer.WriteAsync(Convert.ToBase64String(transmission.Content)).ConfigureAwait(false);
          await configuredTaskAwaitable;
          writer = (StreamWriter) null;
        }
        else
        {
          char[] buffer = new char[40004];
          for (int i = 0; i < transmission.Content.Length; i += 30000)
          {
            configuredTaskAwaitable = writer.WriteAsync(buffer, 0, Convert.ToBase64CharArray(transmission.Content, i, Math.Min(30000, transmission.Content.Length - i), buffer, 0)).ConfigureAwait(false);
            await configuredTaskAwaitable;
          }
          buffer = (char[]) null;
          writer = (StreamWriter) null;
        }
      }
      finally
      {
        writer.Flush();
      }
    }

    private static async Task<string> ReadHeaderAsync(TextReader reader, string headerName)
    {
      string str = await reader.ReadLineAsync().ConfigureAwait(false);
      string[] strArray = !string.IsNullOrEmpty(str) ? str.Split(':') : throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} header is expected.", new object[1]
      {
        (object) headerName
      }));
      if (strArray.Length != 2)
        throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected header format. {0} header is expected. Actual header: {1}", new object[2]
        {
          (object) headerName,
          (object) str
        }));
      return !(strArray[0] != headerName) ? strArray[1].Trim() : throw new FormatException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} header is expected. Actual header: {1}", new object[2]
      {
        (object) headerName,
        (object) str
      }));
    }

    private static async Task<Uri> ReadAddressAsync(TextReader reader)
    {
      string uriString = await reader.ReadLineAsync().ConfigureAwait(false);
      return !string.IsNullOrEmpty(uriString) ? new Uri(uriString) : throw new FormatException("Transmission address is expected.");
    }

    private static async Task<byte[]> ReadContentAsync(TextReader reader)
    {
      string s = await reader.ReadToEndAsync().ConfigureAwait(false);
      return !string.IsNullOrEmpty(s) && !(s == Environment.NewLine) ? Convert.FromBase64String(s) : throw new FormatException("Content is expected.");
    }

    private void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      Action<StorageTransmission> disposing1 = this.Disposing;
      if (disposing1 == null)
        return;
      disposing1(this);
    }
  }
}
