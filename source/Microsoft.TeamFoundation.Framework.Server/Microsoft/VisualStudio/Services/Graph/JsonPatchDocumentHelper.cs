// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.JsonPatchDocumentHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  public class JsonPatchDocumentHelper
  {
    public static void ValidateUpdatePatchDocument<T>(PatchDocument<T> patchDocument)
    {
      foreach (IPatchOperation<T> operation in patchDocument.Operations)
      {
        if (operation == null)
          throw new GraphBadRequestException(FrameworkResources.NullOperationDetected());
        if (operation.Operation != Operation.Replace)
          throw new GraphBadRequestException(FrameworkResources.NotSupportedJsonPatchOperation((object) operation.Operation, (object) operation.Path, operation.Value));
      }
    }

    public static void ValidateUpdatePatchDocument<T>(
      PatchDocument<T> patchDocument,
      HashSet<string> validPatchPaths)
    {
      foreach (IPatchOperation<T> operation in patchDocument.Operations)
      {
        if (operation == null)
          throw new GraphBadRequestException(FrameworkResources.NullOperationDetected());
        if (operation.Operation != Operation.Replace)
          throw new GraphBadRequestException(FrameworkResources.NotSupportedJsonPatchOperation((object) operation.Operation, (object) operation.Path, operation.Value));
        if (!validPatchPaths.Contains<string>(JsonPatchDocumentHelper.ParseOperationPath(operation.Path), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          throw new GraphBadRequestException(FrameworkResources.UnsupportJsonPatchPath((object) JsonPatchDocumentHelper.ParseOperationPath(operation.Path), (object) operation.Operation));
      }
      if (!patchDocument.Operations.Any<IPatchOperation<T>>())
        throw new GraphBadRequestException(PatchResources.JsonPatchNull());
    }

    public static string ParseOperationPath(string path)
    {
      if (path.IsNullOrEmpty<char>())
        return string.Empty;
      string str = path;
      if (str.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        str = path.Substring(1);
      return str.Substring(0, 1).ToUpper() + str.Substring(1);
    }

    public static void ParseUpdatePropertiesPatchDocument(
      PatchDocument<IDictionary<string, object>> patchDocument,
      out IDictionary<string, string> propertiesToUpdate)
    {
      propertiesToUpdate = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IPatchOperation<IDictionary<string, object>> operation in patchDocument.Operations)
      {
        string operationPath = JsonPatchDocumentHelper.ParseOperationPath(operation.Path);
        if (operation.Operation != Operation.Replace)
          throw new GraphBadRequestException(FrameworkResources.NotSupportedJsonPatchOperation((object) operation.Operation, (object) operation.Path, operation.Value));
        propertiesToUpdate.Add(operationPath, operation.Value.ToString());
      }
    }
  }
}
