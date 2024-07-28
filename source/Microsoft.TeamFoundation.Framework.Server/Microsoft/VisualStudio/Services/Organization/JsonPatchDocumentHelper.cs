// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.JsonPatchDocumentHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class JsonPatchDocumentHelper
  {
    public static JsonPatchDocument ConstructJsonPatchDocument(
      Operation operationType,
      string path,
      object value)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      jsonPatchDocument.Add(new JsonPatchOperation()
      {
        Operation = operationType,
        Path = JsonPatchDocumentHelper.BuildPath(path),
        Value = value
      });
      return jsonPatchDocument;
    }

    public static JsonPatchDocument ConstructJsonPatchDocument(
      Operation operationType,
      IDictionary<string, object> dict)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) dict)
      {
        JsonPatchOperation jsonPatchOperation = new JsonPatchOperation()
        {
          Operation = operationType,
          Path = JsonPatchDocumentHelper.BuildPath(keyValuePair.Key),
          Value = keyValuePair.Value
        };
        jsonPatchDocument.Add(jsonPatchOperation);
      }
      return jsonPatchDocument;
    }

    public static JsonPatchDocument ConstructJsonPatchDocument(
      Operation operationType,
      IEnumerable<string> paths)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      foreach (string path in paths)
      {
        JsonPatchOperation jsonPatchOperation = new JsonPatchOperation()
        {
          Operation = operationType,
          Path = JsonPatchDocumentHelper.BuildPath(path)
        };
        jsonPatchDocument.Add(jsonPatchOperation);
      }
      return jsonPatchDocument;
    }

    public static void ValidateUpdatePatchDocument<T>(PatchDocument<T> patchDocument)
    {
      foreach (IPatchOperation<T> operation in patchDocument.Operations)
      {
        if (operation == null)
          throw new OrganizationBadRequestException(FrameworkResources.NullOperationDetected());
        if (operation.Operation != Operation.Replace)
          throw new OrganizationBadRequestException(FrameworkResources.NotSupportedJsonPatchOperation((object) operation.Operation, (object) operation.Path, operation.Value));
      }
    }

    public static string ParseOperationPath(string path)
    {
      if (path.IsNullOrEmpty<char>())
        return string.Empty;
      string str = path;
      if (str.StartsWith("/"))
        str = path.Substring(1);
      return str.Substring(0, 1).ToUpper() + str.Substring(1);
    }

    public static void ValidateAndParseUpdatePropertiesPatchDocument(
      PatchDocument<IDictionary<string, object>> patchDocument,
      out IList<string> propertiesToDelete,
      out PropertyBag propertiesToUpdate)
    {
      propertiesToDelete = (IList<string>) new List<string>();
      propertiesToUpdate = new PropertyBag();
      foreach (IPatchOperation<IDictionary<string, object>> operation in patchDocument.Operations)
      {
        string key = operation != null ? JsonPatchDocumentHelper.ParseOperationPath(operation.Path) : throw new OrganizationBadRequestException(FrameworkResources.NullOperationDetected());
        switch (operation.Operation)
        {
          case Operation.Add:
          case Operation.Replace:
            propertiesToUpdate.Add(key, JsonPatchDocumentHelper.ConvertIntToInt32(operation.Value));
            continue;
          case Operation.Remove:
            propertiesToDelete.Add(key);
            continue;
          default:
            throw new OrganizationBadRequestException(FrameworkResources.NotSupportedJsonPatchOperation((object) operation.Operation, (object) operation.Path, operation.Value));
        }
      }
    }

    private static object ConvertIntToInt32(object value) => value != null && typeof (long).Equals(value.GetType()) ? (object) Convert.ToInt32(value) : value;

    private static string BuildPath(string path) => "/" + path;
  }
}
