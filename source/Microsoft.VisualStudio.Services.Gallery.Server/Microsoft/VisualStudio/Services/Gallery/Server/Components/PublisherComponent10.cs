// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublisherComponent10
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublisherComponent10 : PublisherComponent9
  {
    public override IReadOnlyList<string> FetchAllPublisherDisplayNames()
    {
      this.PrepareStoredProcedure("Gallery.prc_FetchPublisherDisplayNames");
      List<string> stringList = new List<string>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_FetchPublisherDisplayNames", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDisplayNameRow>((ObjectBinder<PublisherDisplayNameRow>) new PublisherDisplayNameBinder());
        foreach (PublisherDisplayNameRow publisherDisplayNameRow in resultCollection.GetCurrent<PublisherDisplayNameRow>().Items)
          stringList.Add(publisherDisplayNameRow.DisplayName.ToLower());
      }
      return (IReadOnlyList<string>) stringList;
    }

    public override IReadOnlyList<string> FetchPublisherDisplayNamesHavingExtensions()
    {
      this.PrepareStoredProcedure("Gallery.prc_FetchPublisherDisplayNamesHavingExtensions");
      List<string> stringList = new List<string>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_FetchPublisherDisplayNamesHavingExtensions", this.RequestContext))
      {
        resultCollection.AddBinder<PublisherDisplayNameRow>((ObjectBinder<PublisherDisplayNameRow>) new PublisherDisplayNameBinder());
        foreach (PublisherDisplayNameRow publisherDisplayNameRow in resultCollection.GetCurrent<PublisherDisplayNameRow>().Items)
          stringList.Add(publisherDisplayNameRow.DisplayName.ToLower());
      }
      return (IReadOnlyList<string>) stringList;
    }
  }
}
