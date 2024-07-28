// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataFormat
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using Microsoft.OData.MultipartMixed;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataFormat
  {
    private static ODataJsonFormat JsonFormat = new ODataJsonFormat();
    private static ODataRawValueFormat rawValueFormat = new ODataRawValueFormat();
    private static ODataMultipartMixedBatchFormat batchFormat = new ODataMultipartMixedBatchFormat();
    private static ODataMetadataFormat metadataFormat = new ODataMetadataFormat();

    public static ODataFormat Json => (ODataFormat) ODataFormat.JsonFormat;

    public static ODataFormat RawValue => (ODataFormat) ODataFormat.rawValueFormat;

    public static ODataFormat Batch => (ODataFormat) ODataFormat.batchFormat;

    public static ODataFormat Metadata => (ODataFormat) ODataFormat.metadataFormat;

    public abstract IEnumerable<ODataPayloadKind> DetectPayloadKind(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings);

    public abstract ODataInputContext CreateInputContext(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings);

    public abstract ODataOutputContext CreateOutputContext(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings);

    public abstract Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings);

    public abstract Task<ODataInputContext> CreateInputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings);

    public abstract Task<ODataOutputContext> CreateOutputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings);

    internal virtual string GetContentType(
      ODataMediaType mediaType,
      Encoding encoding,
      bool writingResponse,
      out IEnumerable<KeyValuePair<string, string>> mediaTypeParameters)
    {
      mediaTypeParameters = mediaType.Parameters;
      return HttpUtils.BuildContentType(mediaType, encoding);
    }
  }
}
