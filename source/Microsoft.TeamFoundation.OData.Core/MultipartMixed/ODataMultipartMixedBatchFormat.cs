// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.ODataMultipartMixedBatchFormat
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.OData.MultipartMixed
{
  internal sealed class ODataMultipartMixedBatchFormat : ODataFormat
  {
    public override string ToString() => "Batch";

    public override IEnumerable<ODataPayloadKind> DetectPayloadKind(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return ODataMultipartMixedBatchFormat.DetectPayloadKindImplementation(messageInfo.MediaType);
    }

    public override ODataInputContext CreateInputContext(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      return (ODataInputContext) new ODataMultipartMixedBatchInputContext((ODataFormat) this, messageInfo, messageReaderSettings);
    }

    public override ODataOutputContext CreateOutputContext(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      return (ODataOutputContext) new ODataMultipartMixedBatchOutputContext((ODataFormat) this, messageInfo, messageWriterSettings);
    }

    public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return TaskUtils.GetTaskForSynchronousOperation<IEnumerable<ODataPayloadKind>>((Func<IEnumerable<ODataPayloadKind>>) (() => ODataMultipartMixedBatchFormat.DetectPayloadKindImplementation(messageInfo.MediaType)));
    }

    public override Task<ODataInputContext> CreateInputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      return Task.FromResult<ODataInputContext>((ODataInputContext) new ODataMultipartMixedBatchInputContext((ODataFormat) this, messageInfo, messageReaderSettings));
    }

    public override Task<ODataOutputContext> CreateOutputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      return Task.FromResult<ODataOutputContext>((ODataOutputContext) new ODataMultipartMixedBatchOutputContext((ODataFormat) this, messageInfo, messageWriterSettings));
    }

    internal override string GetContentType(
      ODataMediaType mediaType,
      Encoding encoding,
      bool writingResponse,
      out IEnumerable<KeyValuePair<string, string>> mediaTypeParameters)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMediaType>(mediaType, nameof (mediaType));
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = mediaType.Parameters != null ? mediaType.Parameters : (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
      IEnumerable<KeyValuePair<string, string>> source = keyValuePairs.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => string.Compare(p.Key, "boundary", StringComparison.OrdinalIgnoreCase) == 0));
      if (source.Count<KeyValuePair<string, string>>() > 1)
        throw new ODataContentTypeException(Strings.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified((object) mediaType.ToText()));
      string batchBoundary;
      if (source.Count<KeyValuePair<string, string>>() == 1)
      {
        batchBoundary = source.First<KeyValuePair<string, string>>().Value;
        mediaTypeParameters = mediaType.Parameters;
      }
      else
      {
        batchBoundary = ODataMultipartMixedBatchWriterUtils.CreateBatchBoundary(writingResponse);
        mediaTypeParameters = (IEnumerable<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>(keyValuePairs)
        {
          new KeyValuePair<string, string>("boundary", batchBoundary)
        };
      }
      return ODataMultipartMixedBatchWriterUtils.CreateMultipartMixedContentType(batchBoundary);
    }

    private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
      ODataMediaType contentType)
    {
      if (!HttpUtils.CompareMediaTypeNames("multipart", contentType.Type) || !HttpUtils.CompareMediaTypeNames("mixed", contentType.SubType) || contentType.Parameters == null || !contentType.Parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kvp => HttpUtils.CompareMediaTypeParameterNames("boundary", kvp.Key))))
        return Enumerable.Empty<ODataPayloadKind>();
      return (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[1]
      {
        ODataPayloadKind.Batch
      };
    }
  }
}
