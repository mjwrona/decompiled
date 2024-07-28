// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent40
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent40 : PublishedExtensionComponent39
  {
    public override TypoSquattingData GetTyposquattingData(IVssRequestContext requestContext)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetTyposquattingData");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetTyposquattingData", requestContext))
      {
        resultCollection.AddBinder<TyposquattingRow>((ObjectBinder<TyposquattingRow>) new TyposquattingBinder());
        List<TyposquattingRow> items = resultCollection.GetCurrent<TyposquattingRow>().Items;
        List<string> stringList1 = new List<string>();
        List<string> stringList2 = new List<string>();
        foreach (TyposquattingRow typosquattingRow in items)
        {
          switch ((TyposquattingCheckType) typosquattingRow.KeywordType)
          {
            case TyposquattingCheckType.PublisherDisplayName:
              stringList1.Add(typosquattingRow.KeywordValue);
              continue;
            case TyposquattingCheckType.ExtensionDisplayName:
              stringList2.Add(typosquattingRow.KeywordValue);
              continue;
            default:
              continue;
          }
        }
        return new TypoSquattingData()
        {
          PublisherDisplayNames = (IReadOnlyList<string>) stringList1.AsReadOnly(),
          ExtensionDisplayNames = (IReadOnlyList<string>) stringList2.AsReadOnly()
        };
      }
    }
  }
}
