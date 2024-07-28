// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DBHelper
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal static class DBHelper
  {
    private static readonly DateTime s_minAllowedDateTime = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime s_maxAllowedDateTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
    private static readonly string s_recursiveEnding = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{0}", (object) BuildPath.PathSeparator, (object) BuildPath.RecursionOperator);

    internal static DateTime MinAllowedDateTime => DBHelper.s_minAllowedDateTime;

    internal static DateTime MaxAllowedDateTime => DBHelper.s_maxAllowedDateTime;

    internal static string DBPathToServerPath(string path) => string.IsNullOrEmpty(path) ? (string) null : DBPath.DatabaseToUserPath(path, true, false);

    internal static string ServerPathToDBPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      string dbPath = DBPath.UserToDatabasePath(path, true, false);
      if (path.EndsWith(DBHelper.s_recursiveEnding, StringComparison.OrdinalIgnoreCase))
        dbPath = dbPath + "*" + BuildPath.PathSeparator;
      return dbPath;
    }

    internal static string LocalPathToDBPath(string path) => string.IsNullOrEmpty(path) ? (string) null : DBPath.UserToDatabasePath(path, true, false);

    internal static string DBPathToLocalPath(string path) => string.IsNullOrEmpty(path) ? (string) null : DBPath.DatabaseToUserPath(path, true, false);

    internal static string VersionControlPathToDBPath(string path) => string.IsNullOrEmpty(path) ? (string) null : DBPath.UserToDatabasePath(path, true, true);

    internal static string DBPathToVersionControlPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return (string) null;
      return string.Equals("$\\", path, StringComparison.Ordinal) ? "$/" : DBPath.DatabaseToUserPath(path, true, true);
    }

    internal static string ServerUrlToDBUrl(string url)
    {
      if (string.IsNullOrEmpty(url))
        return (string) null;
      Uri uri = new Uri(url);
      UriBuilder uriBuilder = new UriBuilder();
      uriBuilder.Host = HttpUtility.UrlEncode(uri.Host, Encoding.UTF8);
      uriBuilder.Port = uri.Port;
      uriBuilder.Scheme = uri.Scheme;
      if (!string.IsNullOrEmpty(uri.AbsolutePath))
      {
        StringBuilder stringBuilder = new StringBuilder("/");
        string[] strArray = uri.AbsolutePath.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.RemoveEmptyEntries);
        for (int index = 0; index < strArray.Length; ++index)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) HttpUtility.UrlEncode(strArray[index], Encoding.UTF8), index < strArray.Length - 1 ? (object) "/" : (object) "");
        uriBuilder.Path = stringBuilder.ToString();
      }
      return uriBuilder.ToString();
    }

    internal static string DBUrlToServerUrl(string url) => string.IsNullOrEmpty(url) ? (string) null : HttpUtility.UrlDecode(url, Encoding.UTF8);

    internal static string CreateArtifactUri(string artifactTypeName, int toolSpecificId) => DBHelper.CreateArtifactUri(artifactTypeName, toolSpecificId.ToString((IFormatProvider) CultureInfo.InvariantCulture));

    internal static string CreateArtifactUri(string artifactTypeName, string toolSpecificId) => LinkingUtilities.EncodeUri(new ArtifactId()
    {
      ToolSpecificId = toolSpecificId,
      ArtifactType = artifactTypeName,
      Tool = "Build"
    });

    internal static string ExtractDbId(string uri) => LinkingUtilities.DecodeUri(uri).ToolSpecificId;

    internal static void Match<TTarget, TSource, TKey>(
      Dictionary<TKey, TTarget> targetDictionary,
      IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector,
      Action<TTarget, TSource> addItem)
    {
      foreach (TSource source1 in source)
      {
        TTarget target;
        if (targetDictionary.TryGetValue(keySelector(source1), out target))
          addItem(target, source1);
      }
    }

    internal static void Match<TParent, TChild, TKey>(
      List<TParent> parents,
      List<TChild> children,
      Func<TParent, TKey> parentKeySelector,
      Func<TChild, TKey> childKeySelector,
      Action<TParent, TChild> appendChild,
      Func<TKey, TKey, bool> keyComparer)
    {
      IOrderedEnumerable<TParent> orderedEnumerable = parents.OrderBy<TParent, TKey>(parentKeySelector);
      TChild[] array = children.OrderBy<TChild, TKey>(childKeySelector).ToArray<TChild>();
      int index = 0;
      foreach (TParent parent in (IEnumerable<TParent>) orderedEnumerable)
      {
        TKey key1 = parentKeySelector(parent);
        for (; index < array.Length; ++index)
        {
          TKey key2 = childKeySelector(array[index]);
          if (keyComparer(key1, key2))
            appendChild(parent, array[index]);
          else
            break;
        }
      }
    }
  }
}
