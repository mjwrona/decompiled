// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.ViewUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class ViewUtils
  {
    public static string GetViewFromPatchOperation(JsonPatchOperation viewsOperation)
    {
      if (viewsOperation == null || viewsOperation.Value == null)
        return string.Empty;
      if (!viewsOperation.Operation.Equals((object) Operation.Add))
        throw PackagingExceptionHelper.ViewsPatchOperationNotSupported(viewsOperation.Operation);
      if (!viewsOperation.Path.Equals("/views/-", StringComparison.InvariantCultureIgnoreCase))
        throw PackagingExceptionHelper.ViewsPathNotSupported(viewsOperation.Path);
      if (viewsOperation.Value is string fromPatchOperation)
        return fromPatchOperation;
      throw PackagingExceptionHelper.ViewsPatchValueMustBeString();
    }
  }
}
