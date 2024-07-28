// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ConditionEvalHelper
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class ConditionEvalHelper
  {
    internal static bool EvaluateStringCondition(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      string value,
      string target,
      byte operation)
    {
      switch (operation)
      {
        case 8:
          return VssStringComparer.StringFieldConditionOrdinal.Compare(value, target) < 0;
        case 9:
          return VssStringComparer.StringFieldConditionOrdinal.Compare(value, target) > 0;
        case 10:
          return VssStringComparer.StringFieldConditionOrdinal.Compare(value, target) <= 0;
        case 11:
          return VssStringComparer.StringFieldConditionOrdinal.Compare(value, target) >= 0;
        case 12:
        case 27:
          return VssStringComparer.StringFieldConditionEquality.Equals(value, target);
        case 13:
        case 28:
          return !VssStringComparer.StringFieldConditionEquality.Equals(value, target);
        case 14:
          return ConditionEvalHelper.MatchUnder(value, target);
        case 15:
        case 16:
          return ConditionEvalHelper.Match(requestContext, evaluationContext, value, target);
        case 25:
          return !ConditionEvalHelper.Match(requestContext, evaluationContext, value, target);
        case 26:
          return UnderPathComparer.IsTargetUnder(value, target);
        case 29:
          return ConditionEvalHelper.IsMember(requestContext, value, target);
        default:
          throw new InvalidOperationException();
      }
    }

    private static bool IsMember(IVssRequestContext requestContext, string user, string group)
    {
      bool flag = false;
      Guid result1 = Guid.Empty;
      Guid result2;
      if ((!Guid.TryParse(user, out result2) ? 0 : (Guid.TryParse(group, out result1) ? 1 : 0)) == 0)
        throw new InvalidFieldValueException(CoreRes.InvalidIdentityException(result2 == Guid.Empty ? (object) user : (object) group));
      try
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[2]
        {
          result2,
          result1
        }, QueryMembership.Direct, (IEnumerable<string>) null);
        flag = source.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)) && service.IsMember(requestContext, source[1], source[0]);
      }
      catch (Exception ex)
      {
      }
      return flag;
    }

    private static bool Match(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      string value,
      string target)
    {
      target = StringFieldComparer.OptimizeRegexMatch(target);
      bool flag;
      if ((evaluationContext.UseRegexMatch ? 1 : (target.Contains("?") ? 1 : 0)) == 0)
      {
        try
        {
          target = Regex.Unescape(target);
        }
        catch (ArgumentException ex)
        {
          requestContext.TraceException(1002037, nameof (ConditionEvalHelper), nameof (Match), (Exception) ex);
        }
        flag = VssStringComparer.StringFieldConditionEquality.Contains(value, target);
      }
      else
        flag = Regex.IsMatch(value, target, RegexOptions.IgnoreCase, evaluationContext.RegexTimeout);
      return flag;
    }

    public static bool MatchUnder(string s, string target)
    {
      string[] strArray1 = s.Split('\n', ';');
      string[] strArray2 = target.Split('\n', ';');
      foreach (string str1 in strArray1)
      {
        foreach (string str2 in strArray2)
        {
          if (!str1.Trim().StartsWith(str2.Trim(), StringComparison.CurrentCultureIgnoreCase))
            return false;
          string[] strArray3 = str2.Trim().Split('\\');
          string[] strArray4 = str1.Trim().Split('\\');
          int index = 0;
          foreach (string str3 in strArray3)
          {
            if (!strArray4[index].Equals(str3, StringComparison.CurrentCultureIgnoreCase))
              return false;
            ++index;
          }
        }
      }
      return true;
    }
  }
}
