// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.NuGetEntityTypeSerializer
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.OData;
using Microsoft.Data.OData.Atom;
using Microsoft.VisualStudio.Services.NuGet.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http.OData;
using System.Web.Http.OData.Formatter.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  public class NuGetEntityTypeSerializer : ODataEntityTypeSerializer
  {
    public NuGetEntityTypeSerializer(ODataSerializerProvider serializerProvider)
      : base(serializerProvider)
    {
    }

    public override ODataEntry CreateEntry(
      SelectExpandNode selectExpandNode,
      EntityInstanceContext entityInstanceContext)
    {
      ODataEntry entry = base.CreateEntry(selectExpandNode, entityInstanceContext);
      this.TryAnnotateV2FeedPackage(entry, entityInstanceContext);
      return entry;
    }

    private void TryAnnotateV2FeedPackage(
      ODataEntry entry,
      EntityInstanceContext entityInstanceContext)
    {
      if (!(entityInstanceContext.EntityInstance is V2FeedPackage entityInstance))
        return;
      AtomEntryMetadata annotation = new AtomEntryMetadata();
      annotation.Title = (AtomTextConstruct) entityInstance.Id;
      if (!string.IsNullOrEmpty(entityInstance.Authors))
        annotation.Authors = (IEnumerable<AtomPersonMetadata>) new AtomPersonMetadata[1]
        {
          new AtomPersonMetadata() { Name = entityInstance.Authors }
        };
      if (entityInstance.LastUpdated > DateTime.MinValue)
        annotation.Updated = new DateTimeOffset?((DateTimeOffset) entityInstance.LastUpdated);
      if (!string.IsNullOrEmpty(entityInstance.Summary))
        annotation.Summary = (AtomTextConstruct) entityInstance.Summary;
      entry.SetAnnotation<AtomEntryMetadata>(annotation);
      if (!(entityInstance.DownloadUrl != (Uri) null))
        return;
      entry.MediaResource = new ODataStreamReferenceValue()
      {
        ContentType = "application/zip",
        ReadLink = entityInstance.DownloadUrl
      };
    }
  }
}
