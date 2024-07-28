// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ResourceVersionComparer
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public class ResourceVersionComparer : IComparer<string>, IEqualityComparer<string>
  {
    private const string c_pattern = "^(?<major>\\d+)\\.(?<minor>\\d+)(-(?<stage>preview)(\\.(?<phase>\\d+))?)?$";
    private static ResourceVersionComparer s_defaultComparer = new ResourceVersionComparer();
    private static ApiResourceVersion s_defaultVersion = new ApiResourceVersion(1.0);

    public int Compare(string x, string y)
    {
      if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
        return 0;
      if (string.IsNullOrEmpty(y))
        return -1;
      if (string.IsNullOrEmpty(x))
        return 1;
      ApiResourceVersion apiResourceVersion1 = ResourceVersionComparer.Parse(x);
      ApiResourceVersion apiResourceVersion2 = ResourceVersionComparer.Parse(y);
      if (!apiResourceVersion1.IsPreview && apiResourceVersion2.IsPreview)
        return -1;
      if (apiResourceVersion1.IsPreview && !apiResourceVersion2.IsPreview)
        return 1;
      if (apiResourceVersion1.ApiVersion.Major > apiResourceVersion2.ApiVersion.Major)
        return -1;
      if (apiResourceVersion1.ApiVersion.Major < apiResourceVersion2.ApiVersion.Major)
        return 1;
      if (apiResourceVersion1.ApiVersion.Minor > apiResourceVersion2.ApiVersion.Minor)
        return -1;
      if (apiResourceVersion1.ApiVersion.Minor > apiResourceVersion2.ApiVersion.Minor)
        return 1;
      if (apiResourceVersion1.ResourceVersion > apiResourceVersion2.ResourceVersion)
        return -1;
      return apiResourceVersion1.ResourceVersion < apiResourceVersion2.ResourceVersion ? 1 : 0;
    }

    public bool Equals(string x, string y) => this.Compare(x, y) == 0;

    public int GetHashCode(string obj) => obj.GetHashCode();

    public static string GetDefaultVersion(IEnumerable<string> versions, bool ignorePreviews = true)
    {
      string y = (string) null;
      foreach (string version in versions)
      {
        if (ResourceVersionComparer.s_defaultComparer.Compare(version, y) < 0 && (!ignorePreviews || !ResourceVersionComparer.Parse(version).IsPreview))
          y = version;
      }
      if (y == null & ignorePreviews)
        y = ResourceVersionComparer.GetDefaultVersion(versions, false);
      return y;
    }

    public static IEnumerable<string> Sort(IEnumerable<string> versions) => (IEnumerable<string>) versions.OrderBy<string, string>((Func<string, string>) (v => v), (IComparer<string>) ResourceVersionComparer.DefaultComparer);

    public static IEnumerable<string> Intersect(
      IEnumerable<string> versions1,
      IEnumerable<string> versions2)
    {
      if (!versions1.Any<string>())
        return versions2;
      return !versions2.Any<string>() ? versions1 : versions1.Join<string, string, string, string>(versions2, (Func<string, string>) (v1 => v1), (Func<string, string>) (v2 => v2), (Func<string, string, string>) ((v1, v2) => v1), (IEqualityComparer<string>) ResourceVersionComparer.DefaultComparer);
    }

    public static ResourceVersionComparer DefaultComparer => ResourceVersionComparer.s_defaultComparer;

    private static ApiResourceVersion Parse(string s)
    {
      Match match = new Regex("^(?<major>\\d+)\\.(?<minor>\\d+)(-(?<stage>preview)(\\.(?<phase>\\d+))?)?$").Match(s);
      if (!match.Success)
        return ResourceVersionComparer.s_defaultVersion;
      int major = ResourceVersionComparer.GetInt(match.Groups["major"].Value);
      int num = ResourceVersionComparer.GetInt(match.Groups["minor"].Value);
      bool success = match.Groups["stage"].Success;
      int resourceVersion = match.Groups["phase"].Success ? ResourceVersionComparer.GetInt(match.Groups["phase"].Value) : 0;
      int minor = num;
      return new ApiResourceVersion(new Version(major, minor), resourceVersion)
      {
        IsPreview = success
      };
    }

    private static int GetInt(string s)
    {
      int result = 0;
      int.TryParse(s, out result);
      return result;
    }
  }
}
