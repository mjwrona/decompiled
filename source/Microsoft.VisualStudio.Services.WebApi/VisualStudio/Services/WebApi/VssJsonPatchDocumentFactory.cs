// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssJsonPatchDocumentFactory
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public static class VssJsonPatchDocumentFactory
  {
    public static JsonPatchDocument ConstructJsonPatchDocument(
      Operation operationType,
      string path,
      string value)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      jsonPatchDocument.Add(new JsonPatchOperation()
      {
        Operation = operationType,
        Path = VssJsonPatchDocumentFactory.BuildPath(path),
        Value = (object) value
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
          Path = VssJsonPatchDocumentFactory.BuildPath(keyValuePair.Key),
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
          Path = VssJsonPatchDocumentFactory.BuildPath(path)
        };
        jsonPatchDocument.Add(jsonPatchOperation);
      }
      return jsonPatchDocument;
    }

    private static string BuildPath(string path) => "/" + path;
  }
}
