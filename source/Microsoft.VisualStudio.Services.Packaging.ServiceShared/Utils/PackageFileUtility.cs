// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.PackageFileUtility
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class PackageFileUtility
  {
    private const StringComparison DefaultComparisonType = StringComparison.OrdinalIgnoreCase;
    private static readonly char[] DelimitersForPath = new char[2]
    {
      '/',
      '\\'
    };

    public static bool ContainsPath(
      this IEnumerable<PackageFile> entries,
      string filePath,
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
      return entries.MatchByPath(filePath, comparisonType) != null;
    }

    public static PackageFile MatchByPath(
      this IEnumerable<PackageFile> entries,
      string filePath,
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
      ArgumentUtility.CheckForNull<IEnumerable<PackageFile>>(entries, nameof (entries));
      ArgumentUtility.CheckForNull<string>(filePath, nameof (filePath));
      List<PackageFile> source1 = new List<PackageFile>(entries);
      if (source1.Any<PackageFile>())
      {
        string[] source2 = filePath.Split(PackageFileUtility.DelimitersForPath);
        string str = ((IEnumerable<string>) source2).Last<string>();
        PackageFile packageFile = (PackageFile) null;
        for (int index = 0; index < source2.Length; ++index)
        {
          string pathPart = source2[index];
          if (index == 0)
            packageFile = source1.FirstOrDefault<PackageFile>((Func<PackageFile, bool>) (x => x.Name.Equals(pathPart, comparisonType)));
          else if (packageFile != null)
          {
            IEnumerable<PackageFile> children = packageFile.Children;
            packageFile = children != null ? children.FirstOrDefault<PackageFile>((Func<PackageFile, bool>) (x => x.Name.Equals(pathPart, comparisonType))) : (PackageFile) null;
          }
          else
            break;
        }
        if (packageFile != null && packageFile.Name.Equals(str, comparisonType))
          return packageFile;
      }
      return (PackageFile) null;
    }

    public static List<T> CombineWith<T>(
      this IEnumerable<T> left,
      IEnumerable<T> right,
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
      where T : PackageFile
    {
      return left.CombineWith<T>(right, out bool _, comparisonType);
    }

    public static List<T> CombineWith<T>(
      this IEnumerable<T> left,
      IEnumerable<T> right,
      out bool isDirty,
      StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
      where T : PackageFile
    {
      ArgumentUtility.CheckForNull<IEnumerable<T>>(right, nameof (right));
      isDirty = false;
      List<T> source = left == null ? new List<T>() : new List<T>(left);
      foreach (T obj1 in right)
      {
        T item = obj1;
        T obj2 = source.FirstOrDefault<T>((Func<T, bool>) (x => x.Name.Equals(item.Name, comparisonType)));
        if ((object) obj2 == null)
        {
          source.Add(item);
          isDirty = true;
        }
        else if (((T) item).Children != null && ((T) item).Children.Any<PackageFile>())
        {
          bool isDirty1;
          obj2.Children = (IEnumerable<PackageFile>) obj2.Children.CombineWith<PackageFile>(((T) item).Children, out isDirty1, comparisonType);
          if (isDirty1)
            isDirty = true;
        }
      }
      return source;
    }
  }
}
