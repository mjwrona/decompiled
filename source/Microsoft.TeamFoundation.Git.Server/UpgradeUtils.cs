// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.UpgradeUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class UpgradeUtils
  {
    public static string[] DeserializeScopes(string settings)
    {
      JObject jobject;
      try
      {
        jobject = JObject.Parse(settings);
      }
      catch (JsonReaderException ex)
      {
        return new string[0];
      }
      if (jobject == null)
        return new string[0];
      JToken source = jobject.GetValue("scope", StringComparison.OrdinalIgnoreCase);
      if (source == null)
        return new string[1]{ "*" };
      if (!source.Any<JToken>())
      {
        if (source.Type != JTokenType.Array && source.Type != JTokenType.Null)
          return new string[0];
        return new string[1]{ "*" };
      }
      List<string> stringList = new List<string>();
      foreach (JObject scope1 in (IEnumerable<JToken>) source)
      {
        string str1;
        bool flag1 = UpgradeUtils.TryGetValue(scope1, "repositoryId", out str1);
        string refName;
        bool flag2 = UpgradeUtils.TryGetValue(scope1, "refName", out refName);
        string str2;
        bool flag3 = UpgradeUtils.TryGetValue(scope1, "matchKind", out str2);
        if (!flag1 || !flag2 || !flag3)
          return new string[0];
        Guid? repositoryId = new Guid?();
        if (str1 != null)
        {
          Guid result;
          if (!Guid.TryParse(str1.ToString(), out result))
            return new string[0];
          repositoryId = new Guid?(result);
        }
        if (refName != null)
        {
          if (refName == string.Empty)
            return new string[0];
          refName = refName.Substring(0, Math.Min(refName.Length, 400));
          if (refName.Contains("*"))
            return new string[0];
        }
        RefNameMatchType? matchKind = new RefNameMatchType?();
        if (str2 != null)
        {
          RefNameMatchType result;
          if (!Enum.TryParse<RefNameMatchType>(str2.ToString(), true, out result))
            return new string[0];
          matchKind = new RefNameMatchType?(result);
        }
        string scope2 = GitPolicyScopeResolver.PolicySettingsToScope(repositoryId, refName, matchKind);
        stringList.Add(scope2);
      }
      return stringList.ToArray();
    }

    private static bool TryGetValue(JObject scope, string name, out string value)
    {
      JToken jtoken = scope.GetValue(name, StringComparison.OrdinalIgnoreCase);
      if (jtoken != null && jtoken.Type != JTokenType.Null)
      {
        if (jtoken.Type != JTokenType.String)
        {
          value = (string) null;
          return false;
        }
        value = jtoken.ToString();
        return true;
      }
      value = (string) null;
      return true;
    }
  }
}
