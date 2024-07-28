// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcItemBatchController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("items")]
  public class TfvcItemBatchController : TfvcApiController, IOverrideLoggingMethodNames
  {
    private List<RequestMediaType> m_acceptHeaderTypes;
    private static readonly List<RequestMediaType> m_supported = new List<RequestMediaType>()
    {
      RequestMediaType.Json,
      RequestMediaType.Zip,
      RequestMediaType.None
    };

    [HttpPost]
    [ClientResponseType(typeof (List<List<TfvcItem>>), null, null)]
    [ClientResponseType(typeof (Stream), "GetItemsBatchZip", "application/zip")]
    [ClientExample("POST__tfvc_itembatch.json", "Multiple items", null, null)]
    public HttpResponseMessage GetItemsBatch(TfvcItemRequestData itemRequestData)
    {
      if (itemRequestData == null || itemRequestData.ItemDescriptors == null || itemRequestData.ItemDescriptors.Length == 0)
        throw new InvalidArgumentValueException(Resources.Get("ErrorPathsNotSpecified")).Expected(this.TfsRequestContext.ServiceName);
      IList<TfvcItemsCollection> tfvcItemsCollectionList = (IList<TfvcItemsCollection>) new List<TfvcItemsCollection>();
      foreach (TfvcItemDescriptor itemDescriptor in itemRequestData.ItemDescriptors)
      {
        if (itemDescriptor != null)
        {
          ArgumentUtility.CheckStringForNullOrEmpty(itemDescriptor.Path, "Path", this.TfsRequestContext.ServiceName);
          TfvcVersionDescriptor versionDescriptor = new TfvcVersionDescriptor(new TfvcVersionOption?(itemDescriptor.VersionOption), new TfvcVersionType?(itemDescriptor.VersionType), itemDescriptor.Version);
          VersionControlRecursionType recursionLevel = itemDescriptor.RecursionLevel;
          try
          {
            if (this.ProjectId != Guid.Empty)
              itemDescriptor.Path = this.ProjectScopedPath(itemDescriptor.Path);
            TfvcItemsCollection itemsCollection = TfvcItemUtility.GetItemsCollection(this.TfsRequestContext, this.Url, itemDescriptor.Path, versionDescriptor, recursionLevel, itemRequestData.IncludeContentMetadata, itemRequestData.IncludeLinks);
            tfvcItemsCollectionList.Add(itemsCollection);
          }
          catch (InvalidPathException ex)
          {
            tfvcItemsCollectionList.Add((TfvcItemsCollection) null);
          }
          catch (ItemNotFoundException ex)
          {
            tfvcItemsCollectionList.Add((TfvcItemsCollection) null);
          }
          catch (InvalidVersionException ex)
          {
            tfvcItemsCollectionList.Add((TfvcItemsCollection) null);
          }
        }
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if ((tfvcItemsCollectionList as List<TfvcItemsCollection>).TrueForAll(TfvcItemBatchController.\u003C\u003EO.\u003C0\u003E__IsNull ?? (TfvcItemBatchController.\u003C\u003EO.\u003C0\u003E__IsNull = new Predicate<TfvcItemsCollection>(TfvcItemBatchController.IsNull))))
        throw new ItemBatchNotFoundException(Resources.Get("ErrorItemBatchNotFound"));
      foreach (RequestMediaType acceptHeaderType in this.m_acceptHeaderTypes)
      {
        switch (acceptHeaderType)
        {
          case RequestMediaType.None:
          case RequestMediaType.Json:
            return this.Request.CreateResponse<IList<TfvcItemsCollection>>(HttpStatusCode.OK, tfvcItemsCollectionList);
          case RequestMediaType.Zip:
            this.ControllerContext.ControllerDescriptor.ControllerName = "TfvcItemsBatchZip";
            // ISSUE: reference to a compiler-generated field
            // ISSUE: reference to a compiler-generated field
            if ((tfvcItemsCollectionList as List<TfvcItemsCollection>).Exists(TfvcItemBatchController.\u003C\u003EO.\u003C0\u003E__IsNull ?? (TfvcItemBatchController.\u003C\u003EO.\u003C0\u003E__IsNull = new Predicate<TfvcItemsCollection>(TfvcItemBatchController.IsNull))))
              throw new ItemBatchNotFoundException(Resources.Get("ErrorItemBatchNotFound"));
            string zipFileName = (this.ProjectId != Guid.Empty ? this.ProjectId.ToString() : this.TfsRequestContext.ServiceHost.Name) + Resources.Get("ZipFileExtension");
            TfvcItemsCollection resultItemBatch = new TfvcItemsCollection();
            (tfvcItemsCollectionList as List<TfvcItemsCollection>).ForEach((Action<TfvcItemsCollection>) (itemSet => resultItemBatch.AddRange((IEnumerable<TfvcItem>) itemSet)));
            return TfvcFileUtility.CreateZipDownloadResponse(this.Request, this.TfsRequestContext, (IEnumerable<TfvcItem>) resultItemBatch, zipFileName, "");
          default:
            continue;
        }
      }
      throw new InvalidArgumentValueException(Resources.Get("UnsupportedMediaType"));
    }

    [NonAction]
    public string GetLoggingMethodName(string methodName, HttpActionContext actionContext)
    {
      this.m_acceptHeaderTypes = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, TfvcItemBatchController.m_supported);
      return this.m_acceptHeaderTypes.Contains(RequestMediaType.Zip) ? methodName + "Zip" : methodName;
    }

    private static bool IsNull(TfvcItemsCollection itemCollection) => itemCollection == null;
  }
}
