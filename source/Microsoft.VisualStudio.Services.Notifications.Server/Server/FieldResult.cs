// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FieldResult
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class FieldResult
  {
    private const int c_guidLength = 36;
    private const int c_minIdentityLength = 39;

    public static FieldResult GetFieldResult(object result)
    {
      if (result == null)
        return (FieldResult) null;
      if (result is IEnumerable<object> results)
        return (FieldResult) FieldResult.ParseEnumerable(results);
      string identityId = (string) null;
      string identityName = (string) null;
      return FieldResult.ParseIdentities(result, ref identityId, ref identityName) ? (FieldResult) new IdentityListFieldResult(identityId, identityName) : (FieldResult) new ConstantFieldResult(result);
    }

    private static ArrayFieldResult ParseEnumerable(IEnumerable<object> results)
    {
      List<object> objectList = new List<object>();
      foreach (object result1 in results)
      {
        JValue jvalue = result1 as JValue;
        bool flag = jvalue != null;
        object result2 = flag ? jvalue.Value : result1;
        string identityId = (string) null;
        string identityName = (string) null;
        if (FieldResult.ParseIdentities(result2, ref identityId, ref identityName))
        {
          objectList.Add((object) identityId);
          objectList.Add((object) identityName);
        }
        else
        {
          if (!flag)
            return new ArrayFieldResult((object[]) (results as string[]) ?? results.ToArray<object>());
          objectList.Add(result2);
        }
      }
      return new ArrayFieldResult(objectList.ToArray());
    }

    public static bool ParseIdentities(
      object result,
      ref string identityId,
      ref string identityName)
    {
      if (!(result is string str))
        return false;
      int length = str.Length;
      if (length < 39)
        return false;
      int num = str[0] != '|' || str[length - 1] != '|' ? 0 : (str[length - 36 - 2] == '%' ? 1 : 0);
      if (num == 0)
        return num != 0;
      identityName = str.Substring(1, length - 36 - 3);
      identityId = str.Substring(length - 36 - 1, 36);
      return num != 0;
    }

    public bool CompareSingleValue(
      byte op,
      EvaluationContext evaluationContext,
      object expectedValue,
      object actualValue,
      SubscriptionFieldType type)
    {
      return FieldComparer.GetComparer(type).CompareObject(op, evaluationContext, expectedValue, actualValue);
    }

    public abstract bool CompareResult(
      byte op,
      EvaluationContext evaluationContext,
      object expectedValue,
      SubscriptionFieldType type);

    public abstract void TraceEvaluationString(
      EvaluationContext evaluationContext,
      bool result,
      byte op,
      object expectedValue);

    protected static void TraceEvaluationString(
      EvaluationContext evaluationContext,
      bool result,
      byte op,
      object expectedValue,
      object actual)
    {
      if (!evaluationContext.Subscription.SubscriptionTracingEnabled)
        return;
      string str;
      if (actual is IEnumerable<object> objects)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (object obj in objects)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(string.Format(", {0}", obj));
          else
            stringBuilder.Append(obj);
        }
        str = stringBuilder.ToString();
      }
      else
        str = actual != null ? actual.ToString() : "(null)";
      evaluationContext.Tracer.EvaluationTraceClause(result, string.Format("{0} {1} {2}", expectedValue, (object) Token.GetOperatorString(op), (object) str), evaluationContext, nameof (TraceEvaluationString));
    }
  }
}
