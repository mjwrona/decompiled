// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.Internal.PackageVersionPatch
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Feed.WebApi.Internal
{
  public class PackageVersionPatch
  {
    public static readonly string ListedPath = "/islisted";
    public static readonly string DeletedPath = "/isDeleted";
    public static readonly string AddViewsPath = "/addViews";
    public static readonly string RemoveViewsPath = "/removeViews";
    public static readonly string FilesPath = "/files";

    public static JsonPatchDocument ChangeListedState(bool? isListed)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      if (isListed.HasValue)
        jsonPatchDocument.Add(new JsonPatchOperation()
        {
          Path = PackageVersionPatch.ListedPath,
          Operation = Operation.Add,
          Value = (object) isListed.Value
        });
      return jsonPatchDocument;
    }

    public static JsonPatchDocument RestoreRecycleBinPackageToFeed()
    {
      JsonPatchDocument feed = new JsonPatchDocument();
      feed.Add(new JsonPatchOperation()
      {
        Path = PackageVersionPatch.DeletedPath,
        Operation = Operation.Replace,
        Value = (object) false
      });
      return feed;
    }

    public static JsonPatchDocument AddOrRemoveViews(IEnumerable<string> views, bool removeViews)
    {
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      string str = removeViews ? PackageVersionPatch.RemoveViewsPath : PackageVersionPatch.AddViewsPath;
      if (views.Count<string>() > 0)
        jsonPatchDocument.Add(new JsonPatchOperation()
        {
          Path = str,
          Operation = Operation.Add,
          Value = (object) views
        });
      return jsonPatchDocument;
    }

    public static JsonPatchDocument AddFiles(IEnumerable<PackageFile> files)
    {
      JsonPatchDocument jsonPatchDocument = PackageVersionPatch.ChangeListedState(new bool?(true));
      if (files != null && files.Any<PackageFile>())
        jsonPatchDocument.Add(new JsonPatchOperation()
        {
          Path = PackageVersionPatch.FilesPath,
          Operation = Operation.Add,
          Value = (object) files
        });
      return jsonPatchDocument;
    }

    public static void ReadPatchDocument(
      PatchDocument<PackageVersion> patchOperations,
      out bool? isListed)
    {
      List<IPatchOperation<PackageVersion>> list = patchOperations.Operations.Where<IPatchOperation<PackageVersion>>((Func<IPatchOperation<PackageVersion>, bool>) (x =>
      {
        if (!string.Equals(x.Path, PackageVersionPatch.ListedPath, StringComparison.OrdinalIgnoreCase))
          return false;
        return x.Operation == Operation.Replace || x.Operation == Operation.Add;
      })).ToList<IPatchOperation<PackageVersion>>();
      if (patchOperations.Operations == null || patchOperations.Operations.Count<IPatchOperation<PackageVersion>>() != 1 || list.Count<IPatchOperation<PackageVersion>>() != 1)
        throw new InvalidPackageVersionPatchException();
      isListed = list.First<IPatchOperation<PackageVersion>>().Value as bool?;
    }
  }
}
