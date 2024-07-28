// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataRawValueFormat
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataRawValueFormat : ODataFormat
  {
    public override string ToString() => "RawValue";

    public override IEnumerable<ODataPayloadKind> DetectPayloadKind(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return ODataRawValueFormat.DetectPayloadKindImplementation(messageInfo.MediaType);
    }

    public override ODataInputContext CreateInputContext(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      return (ODataInputContext) new ODataRawInputContext((ODataFormat) this, messageInfo, messageReaderSettings);
    }

    public override ODataOutputContext CreateOutputContext(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      return (ODataOutputContext) new ODataRawOutputContext((ODataFormat) this, messageInfo, messageWriterSettings);
    }

    public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return TaskUtils.GetTaskForSynchronousOperation<IEnumerable<ODataPayloadKind>>((Func<IEnumerable<ODataPayloadKind>>) (() => ODataRawValueFormat.DetectPayloadKindImplementation(messageInfo.MediaType)));
    }

    public override Task<ODataInputContext> CreateInputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      return Task.FromResult<ODataInputContext>((ODataInputContext) new ODataRawInputContext((ODataFormat) this, messageInfo, messageReaderSettings));
    }

    public override Task<ODataOutputContext> CreateOutputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, "message");
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      return Task.FromResult<ODataOutputContext>((ODataOutputContext) new ODataRawOutputContext((ODataFormat) this, messageInfo, messageWriterSettings));
    }

    private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
      ODataMediaType contentType)
    {
      return HttpUtils.CompareMediaTypeNames("text", contentType.Type) && HttpUtils.CompareMediaTypeNames("text/plain", contentType.SubType) ? (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[1]
      {
        ODataPayloadKind.Value
      } : (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[1]
      {
        ODataPayloadKind.BinaryValue
      };
    }
  }
}
