// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Locator
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class Locator : IComparable<Locator>, IComparable, IEquatable<Locator>
  {
    public const char SeparatorChar = '/';
    public static readonly StringComparison DefaultPathSegmentComparisonType = StringComparison.Ordinal;
    public static readonly string Separator = '/'.ToString();
    public static readonly char[] SeparatorCharArray = new char[1]
    {
      '/'
    };
    public static readonly Locator Root;
    private static readonly string[] PathParsingDelimiters = new string[3]
    {
      Locator.Separator,
      Path.AltDirectorySeparatorChar.ToString(),
      Path.DirectorySeparatorChar.ToString()
    };
    private readonly string path;

    static Locator() => Locator.Root = new Locator(Array.Empty<string>());

    public Locator(Locator l1, Locator l2)
      : this(l1.PathSegments.Concat<string>((IEnumerable<string>) l2.PathSegments))
    {
    }

    public Locator(IEnumerable<Locator> locators)
      : this(locators.Select<Locator, IList<string>>((Func<Locator, IList<string>>) (l => l.PathSegments)).SelectMany<IList<string>, string>((Func<IList<string>, IEnumerable<string>>) (l => (IEnumerable<string>) l)))
    {
    }

    public Locator(params string[] pathSegments)
      : this((IEnumerable<string>) pathSegments)
    {
    }

    public Locator(IEnumerable<string> pathSegments) => this.path = Locator.Separator + string.Join(Locator.Separator, pathSegments.SelectMany<string, string>((Func<string, IEnumerable<string>>) (s => (IEnumerable<string>) Locator.GetPathSegments(s, (string[]) null))));

    public string Value => this.path;

    public IList<string> PathSegments => (IList<string>) this.path.Split(Locator.SeparatorCharArray, StringSplitOptions.RemoveEmptyEntries);

    public int PathSegmentCount => this.PathSegments.Count;

    public static bool IsNullOrEmpty(Locator locator) => (Locator) null == locator || locator.PathSegmentCount == 0;

    public static Locator Parse(string delimitedPath, params string[] delimiters) => delimitedPath != null ? new Locator(Locator.GetPathSegments(delimitedPath, delimiters)) : throw new ArgumentNullException(nameof (delimitedPath));

    public static bool operator ==(Locator x, Locator y) => (object) x == null ? (object) y == null : x.Equals(y);

    public static bool operator !=(Locator x, Locator y) => !(x == y);

    public bool MatchesEnumerationQuery(Locator prefix, PathOptions options)
    {
      if (!this.StartsWith(prefix))
        return false;
      int pathSegmentCount1 = this.PathSegmentCount;
      int pathSegmentCount2 = prefix.PathSegmentCount;
      if (options.HasFlag((Enum) PathOptions.Target) && pathSegmentCount1 == pathSegmentCount2 || options.HasFlag((Enum) PathOptions.ImmediateChildren) && pathSegmentCount1 == pathSegmentCount2 + 1)
        return true;
      return options.HasFlag((Enum) PathOptions.DeepChildren) && pathSegmentCount1 > pathSegmentCount2 + 1;
    }

    public Locator GetParent()
    {
      IList<string> pathSegments = this.PathSegments;
      if (pathSegments.Count <= 0)
        return (Locator) null;
      return new Locator(new string[1]
      {
        string.Join(Locator.Separator, pathSegments.Take<string>(pathSegments.Count - 1))
      });
    }

    public bool StartsWith(Locator other, StringComparison comparison)
    {
      if (!this.path.StartsWith(other.path, comparison))
        return false;
      IList<string> pathSegments1 = this.PathSegments;
      IList<string> pathSegments2 = other.PathSegments;
      if (pathSegments1.Count < pathSegments2.Count)
        return false;
      for (int index = 0; index < pathSegments2.Count; ++index)
      {
        if (!pathSegments1[index].Equals(pathSegments2[index], comparison))
          return false;
      }
      return true;
    }

    public bool StartsWith(Locator other) => this.StartsWith(other, Locator.DefaultPathSegmentComparisonType);

    public override string ToString() => this.path;

    public int CompareTo(Locator other, StringComparison comparison) => string.Compare(this.path, other.path, comparison);

    public int CompareTo(Locator other) => this.CompareTo(other, Locator.DefaultPathSegmentComparisonType);

    public int CompareTo(object other, StringComparison comparison)
    {
      Locator other1 = other as Locator;
      if ((Locator) null == other1)
        other1 = Locator.Parse(other.ToString());
      return this.CompareTo(other1, comparison);
    }

    public int CompareTo(object other) => this.CompareTo(other, Locator.DefaultPathSegmentComparisonType);

    public bool Equals(Locator other, StringComparison comparison)
    {
      bool flag = false;
      if ((Locator) null != other)
        flag = other.path.Equals(this.path, comparison);
      return flag;
    }

    public bool Equals(Locator other) => this.Equals(other, Locator.DefaultPathSegmentComparisonType);

    public bool Equals(object other, StringComparison comparison)
    {
      bool flag = false;
      if (other != null)
      {
        if ((object) this == other)
        {
          flag = true;
        }
        else
        {
          Locator other1 = other as Locator;
          if ((Locator) null == other1)
            other1 = Locator.Parse(other.ToString());
          flag = this.Equals(other1, comparison);
        }
      }
      return flag;
    }

    public override bool Equals(object other) => this.Equals(other, Locator.DefaultPathSegmentComparisonType);

    public override int GetHashCode() => this.path.GetHashCode();

    private static string[] GetPathSegments(string delimitedPath, params string[] delimiters) => delimiters == null || !((IEnumerable<string>) delimiters).Any<string>() ? delimitedPath.Split(Locator.PathParsingDelimiters, StringSplitOptions.RemoveEmptyEntries) : delimitedPath.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
  }
}
