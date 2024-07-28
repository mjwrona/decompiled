// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcLabelsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public class TfvcLabelsController : TfvcApiController
  {
    [HttpGet]
    [ClientExample("GET__tfvc_labels.json", "Labels", null, null)]
    [ClientExample("GET__tfvc_labels_itemLabelFilter-_item_.json", "itemLabelFilter", null, null)]
    [ClientExample("GET__tfvc_labels_name-_name_.json", "Name", null, null)]
    [ClientExample("GET__tfvc_labels_owner-_person_.json", "Owner", null, null)]
    [ClientExample("GET__tfvc_labels__skip-_skip___top-_top_.json", "top and skip", null, null)]
    public IEnumerable<TfvcLabelRef> GetLabels(
      [ModelBinder] TfvcLabelRequestData requestData,
      [FromUri(Name = "$top")] int? top = null,
      [FromUri(Name = "$skip")] int? skip = null)
    {
      if (this.ProjectId != Guid.Empty)
        requestData.LabelScope = this.ProjectScopedPath(requestData.LabelScope);
      if (requestData.LabelScope != null)
        VersionControlPath.ValidatePath(requestData.LabelScope);
      if (requestData.ItemLabelFilter != null)
        VersionControlPath.ValidatePath(requestData.ItemLabelFilter);
      IEnumerable<TfvcLabelRef> labels = (IEnumerable<TfvcLabelRef>) new List<TfvcLabelRef>();
      ref int? local = ref top;
      int? nullable = top;
      int num1 = Math.Max(0, nullable ?? 100);
      local = new int?(num1);
      skip = new int?(Math.Max(0, skip.GetValueOrDefault()));
      nullable = top;
      int num2 = 0;
      if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
        labels = (IEnumerable<TfvcLabelRef>) TfvcLabelUtility.GetLabels(this.TfsRequestContext, this.Url, requestData, top.Value, skip.Value);
      return labels;
    }

    [HttpGet]
    [ClientExample("GET__tfvc_labels__labelId_.json", "labelId", null, null)]
    [ClientExample("GET__tfvc_labels__labelId__maxItemCount-100.json", "maxItemCount", null, null)]
    public TfvcLabel GetLabel(string labelId, [ModelBinder] TfvcLabelRequestData requestData)
    {
      int labelId1 = TfvcLabelUtility.ParseLabelId(labelId);
      requestData.MaxItemCount = new int?(Math.Max(0, requestData.MaxItemCount.GetValueOrDefault()));
      TfvcLabel label = TfvcLabelUtility.GetLabel(this.TfsRequestContext, this.Url, labelId1, requestData);
      if (this.ProjectId != Guid.Empty)
      {
        try
        {
          this.ProjectScopedPath(label.LabelScope);
        }
        catch (InvalidPathException ex)
        {
          throw new LabelNotFoundException(labelId1.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
      }
      return label;
    }
  }
}
