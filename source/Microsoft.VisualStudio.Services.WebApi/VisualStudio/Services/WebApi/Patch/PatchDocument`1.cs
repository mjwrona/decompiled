// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.PatchDocument`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebApi.Patch
{
  public class PatchDocument<TModel> : 
    IPatchDocument<TModel>,
    IPatchOperationApplied,
    IPatchOperationApplying
  {
    public IEnumerable<IPatchOperation<TModel>> Operations { get; set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event PatchOperationApplyingEventHandler PatchOperationApplying;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public event PatchOperationAppliedEventHandler PatchOperationApplied;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Apply(TModel target)
    {
      if ((object) target == null)
        throw new ArgumentNullException(nameof (target));
      if (this.Operations == null)
        throw new VssPropertyValidationException("Operations", PatchResources.NullOrEmptyOperations());
      foreach (IPatchOperation<TModel> operation in this.Operations)
      {
        if (this.PatchOperationApplying != null)
          operation.PatchOperationApplying += this.PatchOperationApplying;
        if (this.PatchOperationApplied != null)
          operation.PatchOperationApplied += this.PatchOperationApplied;
        operation.Apply(target);
        if (this.PatchOperationApplying != null)
          operation.PatchOperationApplying -= this.PatchOperationApplying;
        if (this.PatchOperationApplied != null)
          operation.PatchOperationApplied -= this.PatchOperationApplied;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PatchDocument<TModel> CreateFromJson(JsonPatchDocument jsonPatchDocument)
    {
      if (jsonPatchDocument == null)
        throw new VssPropertyValidationException("JsonPatch", PatchResources.JsonPatchNull());
      if (!jsonPatchDocument.Any<JsonPatchOperation>() || jsonPatchDocument.Any<JsonPatchOperation>((Func<JsonPatchOperation, bool>) (d => d == null)))
        throw new VssPropertyValidationException("Operations", PatchResources.NullOrEmptyOperations());
      PatchDocument<TModel> fromJson = new PatchDocument<TModel>();
      List<PatchOperation<TModel>> source = new List<PatchOperation<TModel>>();
      foreach (JsonPatchOperation operation in (List<JsonPatchOperation>) jsonPatchDocument)
        source.Add(PatchOperation<TModel>.CreateFromJson(operation));
      if (source.Any<PatchOperation<TModel>>())
        fromJson.Operations = (IEnumerable<IPatchOperation<TModel>>) source;
      return fromJson;
    }
  }
}
