// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataPayloadKindDetectionInfo
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.OData
{
  internal sealed class ODataPayloadKindDetectionInfo
  {
    private readonly ODataMediaType contentType;
    private readonly Encoding encoding;
    private readonly ODataMessageReaderSettings messageReaderSettings;
    private readonly IEdmModel model;

    internal ODataPayloadKindDetectionInfo(
      ODataMessageInfo messageInfo,
      ODataMessageReaderSettings messageReaderSettings)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataMediaType>(messageInfo.MediaType, "messageInfo.MediaType");
      ExceptionUtils.CheckArgumentNotNull<ODataMessageReaderSettings>(messageReaderSettings, "readerSettings");
      this.contentType = messageInfo.MediaType;
      this.encoding = messageInfo.Encoding;
      this.messageReaderSettings = messageReaderSettings;
      this.model = messageInfo.Model;
    }

    public ODataMessageReaderSettings MessageReaderSettings => this.messageReaderSettings;

    public IEdmModel Model => this.model;

    internal ODataMediaType ContentType => this.contentType;

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "There is computation needed to get the encoding from the content type; thus a method.")]
    public Encoding GetEncoding() => this.encoding ?? this.contentType.SelectEncoding();
  }
}
