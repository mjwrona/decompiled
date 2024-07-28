// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.FeedRecycleBinPatch
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal
{
  public class FeedRecycleBinPatch
  {
    public static readonly string DeletedPath = "/isDeleted";

    public static JsonPatchDocument RestoreDeletedFeed()
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      jsonPatchDocument.Add(new JsonPatchOperation()
      {
        Path = FeedRecycleBinPatch.DeletedPath,
        Operation = Operation.Replace,
        Value = (object) false
      });
      return jsonPatchDocument;
    }
  }
}
