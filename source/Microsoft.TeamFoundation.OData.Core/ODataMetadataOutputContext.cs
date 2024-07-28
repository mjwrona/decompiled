// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataMetadataOutputContext
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.OData
{
  internal sealed class ODataMetadataOutputContext : ODataOutputContext
  {
    private Stream messageOutputStream;
    private XmlWriter xmlWriter;

    internal ODataMetadataOutputContext(
      ODataMessageInfo messageInfo,
      ODataMessageWriterSettings messageWriterSettings)
      : base(ODataFormat.Metadata, messageInfo, messageWriterSettings)
    {
      try
      {
        this.messageOutputStream = messageInfo.MessageStream;
        this.xmlWriter = ODataMetadataWriterUtils.CreateXmlWriter(this.messageOutputStream, messageWriterSettings, messageInfo.Encoding);
      }
      catch (Exception ex)
      {
        if (ExceptionUtils.IsCatchableExceptionType(ex))
          this.messageOutputStream.Dispose();
        throw;
      }
    }

    internal void Flush() => this.xmlWriter.Flush();

    internal override void WriteInStreamError(ODataError error, bool includeDebugInformation)
    {
      ODataMetadataWriterUtils.WriteError(this.xmlWriter, error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth);
      this.Flush();
    }

    internal override void WriteMetadataDocument()
    {
      IEnumerable<EdmError> errors;
      if (!CsdlWriter.TryWriteCsdl(this.Model, this.xmlWriter, CsdlTarget.OData, out errors))
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (EdmError edmError in errors)
          stringBuilder.AppendLine(edmError.ToString());
        throw new ODataException(Strings.ODataMetadataOutputContext_ErrorWritingMetadata((object) stringBuilder.ToString()));
      }
      this.Flush();
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (this.xmlWriter != null)
        {
          this.xmlWriter.Flush();
          this.messageOutputStream.Dispose();
        }
      }
      finally
      {
        this.messageOutputStream = (Stream) null;
        this.xmlWriter = (XmlWriter) null;
      }
      base.Dispose(disposing);
    }
  }
}
