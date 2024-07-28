// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.ODataJsonFormat
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.JsonLight;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.OData.Json
{
  internal sealed class ODataJsonFormat : ODataFormat
  {
    public override string ToString() => "JsonLight";

    public override IEnumerable<ODataPayloadKind> DetectPayloadKind(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return ODataJsonFormat.DetectPayloadKindImplementation(messageInfo, settings);
    }

    public override ODataInputContext CreateInputContext(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      return (ODataInputContext) new ODataJsonLightInputContext(messageInfo, messageReaderSettings);
    }

    public override ODataOutputContext CreateOutputContext(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      return (ODataOutputContext) new ODataJsonLightOutputContext(messageInfo, messageWriterSettings);
    }

    public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return ODataJsonFormat.DetectPayloadKindImplementationAsync(messageInfo, settings);
    }

    public override Task<ODataInputContext> CreateInputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      return Task.FromResult<ODataInputContext>((ODataInputContext) new ODataJsonLightInputContext(messageInfo, messageReaderSettings));
    }

    public override Task<ODataOutputContext> CreateOutputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      return Task.FromResult<ODataOutputContext>((ODataOutputContext) new ODataJsonLightOutputContext(messageInfo, messageWriterSettings));
    }

    private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ODataPayloadKindDetectionInfo detectionInfo = new ODataPayloadKindDetectionInfo(messageInfo, settings);
      messageInfo.Encoding = detectionInfo.GetEncoding();
      using (ODataJsonLightInputContext lightInputContext = new ODataJsonLightInputContext(messageInfo, settings))
        return lightInputContext.DetectPayloadKind(detectionInfo);
    }

    private static Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindImplementationAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ODataPayloadKindDetectionInfo detectionInfo = new ODataPayloadKindDetectionInfo(messageInfo, settings);
      messageInfo.Encoding = detectionInfo.GetEncoding();
      ODataJsonLightInputContext jsonLightInputContext = new ODataJsonLightInputContext(messageInfo, settings);
      return jsonLightInputContext.DetectPayloadKindAsync(detectionInfo).FollowAlwaysWith<IEnumerable<ODataPayloadKind>>((Action<Task<IEnumerable<ODataPayloadKind>>>) (t => jsonLightInputContext.Dispose()));
    }
  }
}
