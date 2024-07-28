// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMetadataFormat
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.OData
{
  internal sealed class ODataMetadataFormat : ODataFormat
  {
    public override string ToString() => "Metadata";

    public override IEnumerable<ODataPayloadKind> DetectPayloadKind(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return !messageInfo.IsResponse ? Enumerable.Empty<ODataPayloadKind>() : ODataMetadataFormat.DetectPayloadKindImplementation(messageInfo, settings);
    }

    public override ODataInputContext CreateInputContext(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      return (ODataInputContext) new ODataMetadataInputContext(messageInfo, messageReaderSettings);
    }

    public override ODataOutputContext CreateOutputContext(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      return (ODataOutputContext) new ODataMetadataOutputContext(messageInfo, messageWriterSettings);
    }

    public override Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      return !messageInfo.IsResponse ? TaskUtils.GetCompletedTask<IEnumerable<ODataPayloadKind>>(Enumerable.Empty<ODataPayloadKind>()) : Task.FromResult<IEnumerable<ODataPayloadKind>>(ODataMetadataFormat.DetectPayloadKindImplementation(messageInfo, settings));
    }

    public override Task<ODataInputContext> CreateInputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, nameof (messageReaderSettings));
      throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataMetadataFormat_CreateInputContextAsync));
    }

    public override Task<ODataOutputContext> CreateOutputContextAsync(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMessageInfo>(messageInfo, nameof (messageInfo));
      ExceptionUtils.CheckArgumentNotNull<ODataMessageWriterSettings>(messageWriterSettings, nameof (messageWriterSettings));
      throw new ODataException(Strings.General_InternalError((object) InternalErrorCodes.ODataMetadataFormat_CreateOutputContextAsync));
    }

    private static IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings settings)
    {
      ODataPayloadKindDetectionInfo kindDetectionInfo = new ODataPayloadKindDetectionInfo(messageInfo, settings);
      try
      {
        using (XmlReader xmlReader = ODataMetadataReaderUtils.CreateXmlReader(messageInfo.MessageStream, kindDetectionInfo.GetEncoding(), kindDetectionInfo.MessageReaderSettings))
        {
          if (xmlReader.TryReadToNextElement())
          {
            if (string.CompareOrdinal("Edmx", xmlReader.LocalName) == 0)
            {
              if (xmlReader.NamespaceURI == "http://docs.oasis-open.org/odata/ns/edmx")
                return (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[1]
                {
                  ODataPayloadKind.MetadataDocument
                };
            }
          }
        }
      }
      catch (XmlException ex)
      {
      }
      return Enumerable.Empty<ODataPayloadKind>();
    }
  }
}
