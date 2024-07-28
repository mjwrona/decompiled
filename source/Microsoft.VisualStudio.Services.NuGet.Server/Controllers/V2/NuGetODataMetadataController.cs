// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2.NuGetODataMetadataController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.Data.Edm;
using Microsoft.Data.OData;
using Microsoft.Data.OData.Atom;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.OData;
using System.Web.Http.OData.Extensions;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers.V2
{
  [ClientIgnore]
  [ODataRouting]
  [ODataFormatting]
  [SecureNuGetODataFormatting]
  public class NuGetODataMetadataController : NuGetApiController
  {
    [PackagingPublicProjectRequestRestrictions]
    public IEdmModel GetNuGetV2Metadata(string feedId)
    {
      FeedSecurityHelper.CheckReadFeedPermissions(this.TfsRequestContext, this.GetFeedRequest(feedId).Feed);
      return this.Request.ODataProperties().Model;
    }

    [PackagingPublicProjectRequestRestrictions]
    public ODataWorkspace GetNuGetV2ServiceDocument(string feedId)
    {
      FeedSecurityHelper.CheckReadFeedPermissions(this.TfsRequestContext, this.GetFeedRequest(feedId).Feed);
      IEnumerable<IEdmEntitySet> source = this.Request.ODataProperties().Model.EntityContainers().Single<IEdmEntityContainer>().EntitySets();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return new ODataWorkspace()
      {
        Collections = source.Select<IEdmEntitySet, ODataResourceCollectionInfo>(NuGetODataMetadataController.\u003C\u003EO.\u003C0\u003E__ConvertEntitySetToCollectionInfo ?? (NuGetODataMetadataController.\u003C\u003EO.\u003C0\u003E__ConvertEntitySetToCollectionInfo = new Func<IEdmEntitySet, ODataResourceCollectionInfo>(NuGetODataMetadataController.ConvertEntitySetToCollectionInfo)))
      };
    }

    private static ODataResourceCollectionInfo ConvertEntitySetToCollectionInfo(
      IEdmEntitySet entitySet)
    {
      ODataResourceCollectionInfo collectionInfo = new ODataResourceCollectionInfo();
      collectionInfo.Name = entitySet.Name;
      collectionInfo.Url = new Uri(entitySet.Name, UriKind.Relative);
      collectionInfo.SetAnnotation<AtomResourceCollectionMetadata>(new AtomResourceCollectionMetadata()
      {
        Title = (AtomTextConstruct) entitySet.Name
      });
      return collectionInfo;
    }
  }
}
