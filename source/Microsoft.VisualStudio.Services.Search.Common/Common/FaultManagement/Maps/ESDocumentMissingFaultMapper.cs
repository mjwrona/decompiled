// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.ESDocumentMissingFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class ESDocumentMissingFaultMapper : FaultMapper
  {
    public const string ESDocumentMissingExceptionMessage = "document_missing_exception";
    public const string ESDocumentMissingExceptionMessageForDocumentId = "[{0}]: document missing";

    public ESDocumentMissingFaultMapper()
      : base("ESDocumentMissing", IndexerFaultSource.ElasticSearch)
    {
    }

    public override bool IsMatch(Exception ex) => ex != null && ex.GetType().IsAssignableFrom(typeof (SearchPlatformException)) && ex.Message.Contains("document_missing_exception");

    public bool IsMatch(Exception ex, string documentId) => ex != null && ex.GetType().IsAssignableFrom(typeof (SearchPlatformException)) && ex.Message.Contains(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[{0}]: document missing", (object) documentId));
  }
}
