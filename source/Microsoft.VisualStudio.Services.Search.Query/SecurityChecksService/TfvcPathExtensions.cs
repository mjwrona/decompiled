// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.TfvcPathExtensions
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  public static class TfvcPathExtensions
  {
    public static string GetRelativePath(this string aggregatePath)
    {
      if (string.IsNullOrWhiteSpace(aggregatePath) || !aggregatePath.IsServerItem())
        return string.Empty;
      int startIndex = aggregatePath.IndexOf('/', 2);
      return startIndex > 2 ? aggregatePath.Substring(startIndex) : string.Empty;
    }

    public static string GetCompletePath(this string aggregatePath, string projectNameOrGuid)
    {
      if (aggregatePath == null || projectNameOrGuid == null)
        return string.Empty;
      return aggregatePath.IsServerItem() ? aggregatePath : "$/" + projectNameOrGuid + aggregatePath;
    }

    public static string GetRootElement(this string path) => string.IsNullOrEmpty(path) || !path.HasProjectName() ? string.Empty : path.Substring(2, path.IndexOf('/', 2) - 2);

    public static bool HasProjectName(this string path) => !string.IsNullOrEmpty(path) && path.IsServerItem() && path.IndexOf('/', 2) > 0;

    private static bool IsServerItem(this string path) => path.Length >= 2 && path[0] == '$' && path[1] == '/';
  }
}
