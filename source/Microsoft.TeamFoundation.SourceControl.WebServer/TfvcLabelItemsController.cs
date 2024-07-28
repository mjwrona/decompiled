// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcLabelItemsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("labels")]
  public class TfvcLabelItemsController : TfvcApiController
  {
    [HttpGet]
    [ClientExample("GET__tfvc_labels__labelId__items.json", "items", null, null)]
    [ClientExample("GET__tfvc_labels__labelId__items__top-_top___skip-_skip_.json", "top and skip", null, null)]
    public IEnumerable<TfvcItem> GetLabelItems(string labelId, [FromUri(Name = "$top")] int? top = null, [FromUri(Name = "$skip")] int? skip = null)
    {
      int labelId1 = TfvcLabelUtility.ParseLabelId(labelId);
      IEnumerable<TfvcItem> labelItems = (IEnumerable<TfvcItem>) new List<TfvcItem>();
      ref int? local = ref top;
      int? nullable = top;
      int num1 = Math.Max(0, nullable ?? 100);
      local = new int?(num1);
      skip = new int?(Math.Max(0, skip.GetValueOrDefault()));
      nullable = top;
      int num2 = 0;
      if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
        labelItems = (IEnumerable<TfvcItem>) TfvcLabelUtility.GetItems(this.TfsRequestContext, this.Url, labelId1, top.Value, skip.Value);
      return labelItems;
    }
  }
}
