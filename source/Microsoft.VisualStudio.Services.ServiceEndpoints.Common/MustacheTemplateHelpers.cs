// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.MustacheTemplateHelpers
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public static class MustacheTemplateHelpers
  {
    public static int MaxTimeOutInSeconds = 1;
    internal static Func<FileContentMustacheExpressionArguments, string> processGetFileContentRequest = new Func<FileContentMustacheExpressionArguments, string>(MustacheTemplateHelpers.ProcessGetFileContentRequest);

    public static string RegexHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (strArray.Length < 2)
        return string.Empty;
      Regex regex = new Regex(strArray[0], RegexOptions.None, TimeSpan.FromMilliseconds((double) (MustacheTemplateHelpers.MaxTimeOutInSeconds * 1000)));
      string selector = strArray[1];
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(selector, context));
      if (jtoken == null || jtoken.HasValues)
        return string.Empty;
      string input = jtoken.Value<string>();
      try
      {
        Match match = regex.Match(input);
        return match.Success ? match.Groups[1].Value : string.Empty;
      }
      catch (RegexMatchTimeoutException ex)
      {
        throw new InvalidEndpointResponseException(Resources.RegexMatchTimeExceeded((object) input, (object) (MustacheTemplateHelpers.MaxTimeOutInSeconds * 1000)));
      }
    }

    public static object Add(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (strArray.Length < 2)
        return (object) string.Empty;
      string selector = strArray[0];
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(selector, context));
      int result1 = 0;
      if (!jtoken.HasValues && !int.TryParse(jtoken.ToString(), out result1))
        throw new InvalidOperationException("Argument to the template should be an integer");
      int num1 = result1;
      int result2 = 0;
      int.TryParse(strArray[1], out result2);
      int num2 = result2;
      return (object) (num1 + num2);
    }

    public static object IsEqualNumber(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (strArray.Length < 2)
        return (object) false;
      string selector = strArray[0];
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(selector, context));
      int num = 0;
      if (!jtoken.HasValues)
        num = jtoken.Value<int>();
      string str = strArray[1];
      int result = 0;
      int.TryParse(strArray[1], out result);
      return result == num ? (object) true : (object) false;
    }

    public static string ExtractUrlQueryParameter(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray1 = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (strArray1.Length == 0)
        return string.Empty;
      string uriString;
      string str1;
      if (strArray1.Length == 1)
      {
        uriString = context.ReplacementObject.ToString();
        str1 = strArray1[0];
      }
      else
      {
        uriString = strArray1[0];
        str1 = strArray1[1];
      }
      string empty3 = string.Empty;
      string[] strArray2 = str1.Trim().Split(new char[1]
      {
        ','
      }, StringSplitOptions.None);
      if (uriString != null)
      {
        Dictionary<string, string> dictionary = ((IEnumerable<string>) new Uri(uriString).Query.Substring(1).Split('&')).Select<string, string[]>((Func<string, string[]>) (q => q.Split(new char[1]
        {
          '='
        }, 2))).ToDictionary<string[], string, string>((Func<string[], string>) (q => ((IEnumerable<string>) q).FirstOrDefault<string>()), (Func<string[], string>) (q => ((IEnumerable<string>) q).Skip<string>(1).FirstOrDefault<string>()));
        foreach (string str2 in strArray2)
        {
          if (!dictionary.TryGetValue(str2, out empty3))
          {
            string key = WebUtility.UrlDecode(str2);
            dictionary.TryGetValue(key, out empty3);
          }
          if (!string.IsNullOrEmpty(empty3))
            break;
        }
      }
      return empty3 ?? string.Empty;
    }

    public static string ExtractUrlQueryParamKeyValue(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray1 = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (strArray1.Length == 0)
        return string.Empty;
      if (strArray1.Length == 1)
      {
        empty1 = context.ReplacementObject.ToString();
        empty2 = strArray1[0];
      }
      string empty3 = string.Empty;
      string[] strArray2 = empty2.Trim().Split(new char[1]
      {
        ','
      }, StringSplitOptions.None);
      if (empty1 != null)
      {
        Dictionary<string, string> dictionary = ((IEnumerable<string>) new Uri(empty1).Query.Substring(1).Split('&')).Select<string, string[]>((Func<string, string[]>) (q => new string[2]
        {
          q.Substring(0, q.IndexOf('=')),
          q
        })).ToDictionary<string[], string, string>((Func<string[], string>) (q => ((IEnumerable<string>) q).FirstOrDefault<string>()), (Func<string[], string>) (q => ((IEnumerable<string>) q).ElementAtOrDefault<string>(1)));
        foreach (string str in strArray2)
        {
          if (!dictionary.TryGetValue(str, out empty3))
          {
            string key = WebUtility.UrlDecode(str);
            dictionary.TryGetValue(key, out empty3);
          }
          if (!string.IsNullOrEmpty(empty3))
            break;
        }
      }
      return empty3 ?? string.Empty;
    }

    public static object IsTokenPresent(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (strArray.Length < 1)
        return (object) false;
      string selector = strArray[0];
      return MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(selector, context)) != null ? (object) true : (object) false;
    }

    public static object IsTokenContainsSubstring(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (strArray.Length < 2)
        return (object) false;
      string selector = strArray[0];
      string str = strArray[1];
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(selector, context));
      if (jtoken == null)
        return (object) false;
      return jtoken.ToString().IndexOf(str, StringComparison.OrdinalIgnoreCase) < 0 ? (object) false : (object) true;
    }

    public static object GetTokenValue(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (strArray.Length < 1)
        return (object) string.Empty;
      string selector = strArray[0];
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(selector, context));
      if (jtoken == null)
        return (object) string.Empty;
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = (object) jtoken.Value<string>(),
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      return (object) expression.EvaluateChildExpressions(context1);
    }

    public static string JsonEscapeHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      if (!string.IsNullOrEmpty(expression.Expression) && expression.GetCurrentToken(expression.Expression, context) is JToken currentToken)
      {
        string str1 = currentToken.ToString();
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = JsonConvert.ToString(str1);
          return str2.Substring(1, str2.Length - 2);
        }
      }
      return string.Empty;
    }

    public static string ExtractResourceHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      if (MustacheTemplateHelpers.ToJToken(context.ReplacementObject).Type == JTokenType.Object)
      {
        string[] parts = MustacheTemplateHelpers.SplitExpression(expression.Expression);
        string empty = string.Empty;
        string resourceName = string.Empty;
        ref string local1 = ref empty;
        ref string local2 = ref resourceName;
        MustacheTemplateHelpers.RetrieveKeyAndResourceFromParts(parts, out local1, out local2);
        string parameter = MustacheTemplateHelpers.ParseParameter(empty, expression, context);
        if (parameter != null)
        {
          List<string> list = ((IEnumerable<string>) parameter.Split('/')).ToList<string>();
          int index = list.FindIndex((Predicate<string>) (t => t.Equals(resourceName, StringComparison.OrdinalIgnoreCase))) + 1;
          if (index > 0)
            return list[index];
        }
      }
      return (string) null;
    }

    public static string SplitAndIterateHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() < 2)
        return string.Empty;
      string parameter = MustacheTemplateHelpers.ParseParameter(source[0], expression, context);
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(source[1], context));
      if (jtoken == null)
        return string.Empty;
      JArray jarray = JToken.FromObject((object) jtoken.Value<string>().Split(new string[1]
      {
        parameter
      }, StringSplitOptions.None)) as JArray;
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < jarray.Count; ++index)
      {
        MustacheEvaluationContext context1 = new MustacheEvaluationContext()
        {
          ReplacementObject = (object) jarray[index],
          ParentContext = context,
          CurrentIndex = index,
          ParentItemsCount = jarray.Count,
          PartialExpressions = context.PartialExpressions,
          AdditionalEvaluationData = context.AdditionalEvaluationData,
          Options = context.Options
        };
        MustacheEvaluationContext.CombinePartialsDictionaries(context1, (MustacheAggregateExpression) expression);
        stringBuilder.Append(expression.EvaluateChildExpressions(context1));
      }
      return stringBuilder.ToString();
    }

    public static string StringReplaceHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] expressionParts = MustacheTemplateHelpers.ExtractExpressionParts(expression.Expression);
      int num = 2000;
      if (((IEnumerable<string>) expressionParts).Count<string>() < 3)
        return string.Empty;
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(expressionParts[2], context));
      string str = string.Empty;
      if (jtoken != null && !jtoken.HasValues)
        str = jtoken.Value<string>();
      if (MustacheTemplateHelpers.IsParametersArray(expressionParts[0]) && MustacheTemplateHelpers.IsParametersArray(expressionParts[1]))
      {
        string[] arrayFromParameters1 = MustacheTemplateHelpers.ExtractArrayFromParameters(expressionParts[0]);
        string[] arrayFromParameters2 = MustacheTemplateHelpers.ExtractArrayFromParameters(expressionParts[1]);
        if (arrayFromParameters1.Length > num || arrayFromParameters2.Length > num)
          throw new InvalidOperationException(Resources.MaxArrayLimitReached((object) num));
        if (arrayFromParameters1.Length != arrayFromParameters2.Length)
          return string.Empty;
        for (int index = 0; index < arrayFromParameters1.Length; ++index)
          str = str.Replace(arrayFromParameters1[index], arrayFromParameters2[index]);
        return str;
      }
      string parameter1 = MustacheTemplateHelpers.ParseParameter(expressionParts[0], expression, context);
      string parameter2 = MustacheTemplateHelpers.ParseParameter(expressionParts[1], expression, context);
      return str.Replace(parameter1, parameter2);
    }

    public static string UriDataEncodeHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      int result;
      if (((IEnumerable<string>) source).Count<string>() != 2 || !int.TryParse(source[0], out result) || result < 0)
        return string.Empty;
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(source[1], context));
      string stringToEscape = string.Empty;
      if (jtoken != null && !jtoken.HasValues)
      {
        stringToEscape = jtoken.Value<string>();
        for (int index = 0; index < result; ++index)
          stringToEscape = Uri.EscapeDataString(stringToEscape);
      }
      return stringToEscape;
    }

    public static string SubStringHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      int result1;
      int result2;
      if (((IEnumerable<string>) source).Count<string>() < 3 || !int.TryParse(source[0], out result1) || !int.TryParse(source[1], out result2))
        return string.Empty;
      JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(source[2], context));
      return !jtoken.HasValues ? jtoken.Value<string>().Substring(result1, result2) : string.Empty;
    }

    public static string RecursiveFormatHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      int num1 = 15;
      int num2 = 2000;
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() < 3)
        return string.Empty;
      string empty = string.Empty;
      string parameter = MustacheTemplateHelpers.ParseParameter(source[2], expression, context);
      int result;
      if (!int.TryParse(source[0], out result))
        throw new InvalidServiceEndpointRequestException(Resources.InvalidMustacheExpressionParameter((object) "repeatCount", (object) "recursiveFormat"));
      int num3 = result > num1 ? num1 : result;
      string format = MustacheTemplateHelpers.ParseParameter(source[1], expression, context);
      for (int index = 0; index <= num3; ++index)
      {
        string str = index == num3 ? string.Empty : parameter;
        try
        {
          format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, (object) str);
        }
        catch (FormatException ex)
        {
          throw new InvalidOperationException(Resources.InvalidFormatSpecifierInRecursiveFormat(), (Exception) ex);
        }
        if (format.Length > num2)
          throw new InvalidOperationException(Resources.MaxStringLengthReached((object) num2));
      }
      return format;
    }

    public static object AddFieldHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() != 4)
        return (object) string.Empty;
      string parameter1 = MustacheTemplateHelpers.ParseParameter(source[1], expression, context);
      string parameter2 = MustacheTemplateHelpers.ParseParameter(source[2], expression, context);
      string parameter3 = MustacheTemplateHelpers.ParseParameter(source[3], expression, context);
      if (string.IsNullOrEmpty(parameter1))
        throw new ArgumentNullException("parentPathKey");
      if (string.IsNullOrEmpty(parameter2))
        throw new ArgumentNullException("parentPathProperty");
      if (string.IsNullOrEmpty(parameter3))
        throw new ArgumentNullException("separator");
      JToken jtoken = MustacheTemplateHelpers.AddFieldRecursive((JToken) context.ReplacementObject, source[0], parameter1, parameter2, parameter3);
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = (object) jtoken,
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      return (object) expression.EvaluateChildExpressions(context1);
    }

    public static object RecursiveSelectHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (!((IEnumerable<string>) source).Any<string>())
        return (object) string.Empty;
      JToken currentJtoken = expression.GetCurrentJToken(source[0], context);
      if (currentJtoken == null)
        return (object) string.Empty;
      JArray jarray = new JArray();
      foreach (JToken jtoken in MustacheTemplateHelpers.GetTokensRecursive(currentJtoken, source[0]))
      {
        MustacheEvaluationContext context1 = new MustacheEvaluationContext()
        {
          ParentContext = context,
          ReplacementObject = (object) jtoken,
          PartialExpressions = context.PartialExpressions,
          AdditionalEvaluationData = context.AdditionalEvaluationData
        };
        MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
        string childExpressions = expression.EvaluateChildExpressions(context1);
        if (!string.IsNullOrEmpty(childExpressions))
          jarray.Add((JToken) childExpressions);
      }
      return (object) jarray;
    }

    public static object SplitAndPrefixHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      int num1 = 15;
      int num2 = 2000;
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() < 3)
        return (object) string.Empty;
      string parameter1 = MustacheTemplateHelpers.ParseParameter(source[0], expression, context);
      string parameter2 = MustacheTemplateHelpers.ParseParameter(source[1], expression, context);
      string parameter3 = MustacheTemplateHelpers.ParseParameter(source[2], expression, context);
      if (string.IsNullOrEmpty(parameter1) || string.IsNullOrEmpty(parameter2))
        return (object) string.Empty;
      string str1 = string.Empty;
      string[] strArray = parameter1.Split(new string[1]
      {
        parameter2
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length > num1)
        throw new InvalidOperationException(Resources.MaxTokenLimitReached((object) num1));
      foreach (string str2 in strArray)
      {
        if (!string.IsNullOrEmpty(str2.Trim()))
        {
          if (str1.Length > num2)
            throw new InvalidOperationException(Resources.MaxStringLengthReached((object) num2));
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}{2}", (object) str1, (object) parameter3, (object) str2.Trim());
        }
      }
      return (object) str1;
    }

    public static object SelectMaxOfHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() < 2)
        return (object) string.Empty;
      JToken currentJtoken = expression.GetCurrentJToken(source[0], context);
      string parameter = MustacheTemplateHelpers.ParseParameter(source[1], expression, context);
      JToken jtoken = (JToken) null;
      long result1;
      if (!string.IsNullOrEmpty(parameter) && currentJtoken != null && long.TryParse(MustacheTemplateHelpers.GetJsonProperty(currentJtoken[(object) 0], parameter), out result1))
      {
        jtoken = currentJtoken[(object) 0];
        for (int key = 1; key < currentJtoken.Count<JToken>(); ++key)
        {
          long result2;
          if (long.TryParse(MustacheTemplateHelpers.GetJsonProperty(currentJtoken[(object) key], parameter), out result2) && result2 > result1)
          {
            jtoken = currentJtoken[(object) key];
            result1 = result2;
          }
        }
      }
      if (jtoken == null)
        return (object) jtoken;
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = (object) jtoken,
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      return (object) expression.EvaluateChildExpressions(context1);
    }

    public static object CopyFieldFromParentHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() != 3)
        return (object) string.Empty;
      string path1 = source[0];
      if (string.IsNullOrEmpty(path1))
        throw new InvalidServiceEndpointRequestException(Resources.InvalidMustacheExpressionParameter((object) "parentNodeName", (object) "copyFieldFromParent"));
      string path2 = source[1];
      if (string.IsNullOrEmpty(path2))
        throw new InvalidServiceEndpointRequestException(Resources.InvalidMustacheExpressionParameter((object) "childNodeName", (object) "copyFieldFromParent"));
      string key = source[2];
      if (string.IsNullOrEmpty(key))
        throw new InvalidServiceEndpointRequestException(Resources.InvalidMustacheExpressionParameter((object) "propertyToCopy", (object) "copyFieldFromParent"));
      JToken jtoken1 = ((JToken) context.ReplacementObject).SelectToken(path1);
      if (jtoken1 != null)
      {
        foreach (JToken jtoken2 in (IEnumerable<JToken>) jtoken1)
        {
          if (jtoken2[(object) key] != null)
          {
            JToken jtoken3 = jtoken2.SelectToken(path2);
            if (jtoken3 != null)
            {
              foreach (JToken jtoken4 in (IEnumerable<JToken>) jtoken3)
                jtoken4[(object) key] = jtoken2[(object) key];
            }
          }
        }
      }
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = context.ReplacementObject,
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      return (object) expression.EvaluateChildExpressions(context1);
    }

    public static object SelectTokensHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() != 1)
        return (object) string.Empty;
      string path = source[0];
      if (string.IsNullOrEmpty(path))
        throw new InvalidServiceEndpointRequestException(Resources.InvalidMustacheExpressionParameter((object) "pattern", (object) "selectTokens"));
      IEnumerable<JToken> content = ((JToken) context.ReplacementObject).SelectTokens(path);
      JToken jtoken = (JToken) null;
      if (content != null)
        jtoken = (JToken) new JArray((object) content);
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = (object) jtoken,
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      return (object) expression.EvaluateChildExpressions(context1);
    }

    public static void EndswithHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0, string.Empty);
      string helperArgument2 = expression.GetHelperArgument<string>(context, 1, string.Empty);
      bool helperArgument3 = expression.GetHelperArgument<bool>(context, 2);
      int num = expression.GetHelperArgument<bool>(context, 3) ? 1 : 0;
      bool flag = helperArgument1.EndsWith(helperArgument2, helperArgument3 ? StringComparison.OrdinalIgnoreCase : StringComparison.InvariantCulture);
      if (num != 0 ? !flag : flag)
      {
        if (expression.IsBlockExpression)
          expression.EvaluateChildExpressions(context, writer);
        else
          writer.Write("true");
      }
      else
      {
        if (expression.IsBlockExpression)
          return;
        writer.Write("false");
      }
    }

    public static object SortByHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      int num = 1000;
      if (((IEnumerable<string>) source).Count<string>() < 1)
        return (object) string.Empty;
      string sortByProperty = source[0];
      if (string.IsNullOrEmpty(sortByProperty))
        throw new InvalidServiceEndpointRequestException(Resources.InvalidMustacheExpressionParameter((object) "sortByProperty", (object) "sortBy"));
      JToken replacementObject = (JToken) context.ReplacementObject;
      if (replacementObject.Count<JToken>() > num)
        throw new InvalidServiceEndpointRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.CannotSortBeyondMaxObjectCount((object) num)));
      Func<JToken, long> keySelector = (Func<JToken, long>) (token =>
      {
        long result = 0;
        if (token[(object) sortByProperty] != null && long.TryParse((string) token[(object) sortByProperty], out result))
          return result;
        throw new InvalidServiceEndpointRequestException(Resources.InvalidMustacheExpressionParameter((object) "sortByProperty", (object) "sortBy"));
      });
      bool result1 = false;
      if (source.Length == 2)
        bool.TryParse(source[1], out result1);
      IOrderedEnumerable<JToken> orderedEnumerable = result1 ? replacementObject.OrderByDescending<JToken, long>((Func<JToken, long>) (x => keySelector(x))) : replacementObject.OrderBy<JToken, long>((Func<JToken, long>) (x => keySelector(x)));
      JArray jarray = new JArray();
      if (orderedEnumerable != null)
      {
        foreach (JToken jtoken in (IEnumerable<JToken>) orderedEnumerable)
          jarray.Add(jtoken);
      }
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = (object) jarray,
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      return (object) expression.EvaluateChildExpressions(context1);
    }

    public static object ExtractNumbersAndSortHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() < 1)
        return (object) string.Empty;
      string commonPrefix = (string) null;
      if (((IEnumerable<string>) source).Count<string>() > 1)
        commonPrefix = MustacheTemplateHelpers.ParseParameter(source[1], expression, context);
      string commonSuffix = (string) null;
      if (((IEnumerable<string>) source).Count<string>() > 2)
        commonSuffix = MustacheTemplateHelpers.ParseParameter(source[2], expression, context);
      char[] charactersToTrim = " ".ToCharArray();
      if (((IEnumerable<string>) source).Count<string>() > 3)
        charactersToTrim = MustacheTemplateHelpers.ParseParameter(source[3], expression, context).ToCharArray();
      bool flag = false;
      if (((IEnumerable<string>) source).Count<string>() > 3)
        flag = source[4].ToString().StartsWith("AS");
      List<int> list1 = (context.ReplacementObject as JToken).SelectTokens(source[0].ToString()).Select<JToken, string>((Func<JToken, string>) (token => token.ToString())).Select<string, string>((Func<string, string>) (name => MustacheTemplateHelpers.RemovePrefixAndSuffix(name, commonPrefix, commonSuffix))).Select<string, string>((Func<string, string>) (name => name.Trim(charactersToTrim).Trim())).Where<string>((Func<string, bool>) (name => MustacheTemplateHelpers.IsNumber(name))).Select<string, int>((Func<string, int>) (name => int.Parse(name))).ToList<int>();
      List<int> list2 = (flag ? (IEnumerable<int>) list1.OrderBy<int, int>((Func<int, int>) (i => i)) : (IEnumerable<int>) list1.OrderByDescending<int, int>((Func<int, int>) (i => i))).ToList<int>();
      Func<int, JToken> CreateResultObject = (Func<int, JToken>) (value => (JToken) new JObject()
      {
        ["Value"] = (JToken) value
      });
      MustacheEvaluationContext context1 = new MustacheEvaluationContext()
      {
        ParentContext = context,
        ReplacementObject = (object) new JObject()
        {
          ["results"] = JToken.FromObject((object) list2.Select<int, JToken>((Func<int, JToken>) (name => CreateResultObject(name))))
        },
        PartialExpressions = context.PartialExpressions,
        AdditionalEvaluationData = context.AdditionalEvaluationData
      };
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression);
      return (object) expression.EvaluateChildExpressions(context1);
    }

    public static object JoinPathsHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] expressionParts = MustacheTemplateHelpers.ExtractExpressionParts(expression.Expression);
      if (((IEnumerable<string>) expressionParts).Count<string>() == 0)
        return (object) string.Empty;
      string str = "";
      foreach (string paramValue in expressionParts)
        str = str.Trim().TrimEnd('/') + "/" + MustacheTemplateHelpers.ParseParameter(paramValue, expression, context).Trim().TrimStart('/');
      return (object) str.Trim().TrimStart('/');
    }

    public static object RemovePrefixFromPathHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] expressionParts = MustacheTemplateHelpers.ExtractExpressionParts(expression.Expression);
      if (((IEnumerable<string>) expressionParts).Count<string>() < 1)
        return (object) string.Empty;
      string parameter1 = MustacheTemplateHelpers.ParseParameter(expressionParts[0], expression, context);
      string prefix = "";
      for (int index = 1; index < ((IEnumerable<string>) expressionParts).Count<string>(); ++index)
      {
        string parameter2 = MustacheTemplateHelpers.ParseParameter(expressionParts[index], expression, context);
        if (!string.IsNullOrWhiteSpace(parameter2))
          prefix += parameter2.Trim('/') + (object) '/';
      }
      return (object) MustacheTemplateHelpers.RemovePrefixAndSuffix(parameter1, prefix);
    }

    public static string ToCommaSeparatedKeyValueList(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source1 = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source1).Count<string>() == 0)
        return string.Empty;
      string parameter = MustacheTemplateHelpers.ParseParameter(source1[0], expression, context);
      Dictionary<string, object> source2;
      try
      {
        source2 = JsonConvert.DeserializeObject<Dictionary<string, object>>(parameter);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.InvalidJsonStringArgument((object) parameter)), ex);
      }
      if (source2 == null || !source2.Any<KeyValuePair<string, object>>())
        return string.Empty;
      string separatedKeyValueList = string.Join(", ", source2.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (kvp => string.Format("\"{0}\" : \"{1}\"", (object) kvp.Key, kvp.Value))));
      bool result;
      if (((source1.Length != 2 ? 0 : (bool.TryParse(source1[1], out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
        separatedKeyValueList = "," + separatedKeyValueList;
      return separatedKeyValueList;
    }

    public static object GetFileContentHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      try
      {
        FileContentMustacheExpressionArguments expressionArguments = JsonConvert.DeserializeObject<FileContentMustacheExpressionArguments>((expression.Expression ?? string.Empty).Trim());
        string url = expressionArguments?.Url ?? string.Empty;
        if (string.IsNullOrWhiteSpace(url))
          throw new InvalidOperationException(Resources.UrlCannotBeEmpty());
        if (string.IsNullOrWhiteSpace(expressionArguments?.AuthToken ?? string.Empty))
          throw new InvalidOperationException(Resources.AuthTokenCannotBeEmpty());
        string[] systemWhiteListedUrls = MustacheTemplateHelpers.GetSystemWhiteListedUrls(context);
        if (!MustacheTemplateHelpers.IsUrlWhiteListed(url, systemWhiteListedUrls))
          throw new InvalidOperationException(Resources.UrlIsNotWhiteListed((object) url));
        return MustacheTemplateHelpers.processGetFileContentRequest != null ? (object) MustacheTemplateHelpers.processGetFileContentRequest(expressionArguments) : (object) string.Empty;
      }
      catch (Exception ex)
      {
        throw new ServiceEndpointQueryFailedException(ex.Message, ex);
      }
    }

    public static string ToLower(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() == 0)
        return string.Empty;
      string parameter = MustacheTemplateHelpers.ParseParameter(source[0], expression, context);
      return string.IsNullOrWhiteSpace(parameter) ? string.Empty : parameter.Trim().ToLower();
    }

    public static string ToLowerInvariant(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() == 0)
        return string.Empty;
      string parameter = MustacheTemplateHelpers.ParseParameter(source[0], expression, context);
      return string.IsNullOrWhiteSpace(parameter) ? string.Empty : parameter.Trim().ToLowerInvariant();
    }

    public static string ToAlphaNumericString(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() < 1)
        return string.Empty;
      string parameter1 = MustacheTemplateHelpers.ParseParameter(source[0], expression, context);
      if (string.IsNullOrWhiteSpace(parameter1))
        return string.Empty;
      string alphaNumericString = string.Concat<char>(parameter1.Trim().ToLowerInvariant().Where<char>((Func<char, bool>) (ch => char.IsLetterOrDigit(ch))));
      if (source.Length == 2)
      {
        string parameter2 = MustacheTemplateHelpers.ParseParameter(source[1], expression, context);
        int result = 0;
        if (!string.IsNullOrWhiteSpace(parameter2))
          int.TryParse(parameter2, out result);
        else
          int.TryParse(source[1], out result);
        result = result > alphaNumericString.Length || result < 1 ? alphaNumericString.Length : result;
        alphaNumericString = alphaNumericString.Substring(0, result);
      }
      return alphaNumericString;
    }

    public static string ShortGuid(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      return Guid.NewGuid().ToString().Substring(0, 4);
    }

    public static string NewGuid(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] source = MustacheTemplateHelpers.SplitExpression(expression.Expression);
      if (((IEnumerable<string>) source).Count<string>() <= 0)
        return Guid.NewGuid().ToString();
      string parameter = MustacheTemplateHelpers.ParseParameter(source[0], expression, context);
      try
      {
        return Guid.NewGuid().ToString(parameter);
      }
      catch (FormatException ex)
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(Resources.InvalidGuidFormatException((object) parameter));
        if (!string.IsNullOrEmpty(ex.Message))
          stringBuilder.Append(ex.Message);
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, stringBuilder.ToString()), (Exception) ex);
      }
    }

    public static string ToDateTimeFormat(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] expressionParts = MustacheTemplateHelpers.ExtractExpressionParts(expression.Expression);
      if (((IEnumerable<string>) expressionParts).Count<string>() < 2)
        return string.Empty;
      string parameter1 = MustacheTemplateHelpers.ParseParameter(expressionParts[0], expression, context);
      string parameter2 = MustacheTemplateHelpers.ParseParameter(expressionParts[1], expression, context);
      DateTime dateTime;
      try
      {
        dateTime = DateTime.Parse(parameter1, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidDateTimeStringRepresentationException((object) parameter1)), ex);
      }
      try
      {
        return dateTime.ToUniversalTime().ToString(parameter2, (IFormatProvider) CultureInfo.InvariantCulture);
      }
      catch (FormatException ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.InvalidDateTimeFormatException((object) parameter2)), (Exception) ex);
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.OutofRangeDateTimeException((object) dateTime)), (Exception) ex);
      }
    }

    public static string Base64Helper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(MustacheTemplateHelpers.ResolveExpressionFromContext(expression, context)));
    }

    public static string DecodeBase64Helper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      return Encoding.UTF8.GetString(Convert.FromBase64String(MustacheTemplateHelpers.ResolveExpressionFromContext(expression, context)));
    }

    private static string ResolveExpressionFromContext(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string[] strArray = expression.Expression.Trim().Split(new char[1]
      {
        ' '
      }, StringSplitOptions.None);
      string empty = string.Empty;
      foreach (string selector in strArray)
      {
        if (selector.StartsWith("endpoint.", StringComparison.OrdinalIgnoreCase))
        {
          JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(selector, context));
          if (jtoken != null)
          {
            string str = jtoken.ToString();
            empty += str;
          }
          else
          {
            string str = MustacheTemplateHelpers.FetchEndpointDataFromContext(selector.Substring(selector.IndexOf(".") + 1), context);
            if (str == null)
              throw new ServiceEndpointException(Resources.InvalidEndpointVariable((object) selector));
            empty += str;
          }
        }
        else
          empty += selector.Trim('\'').Trim('"');
      }
      return empty;
    }

    private static string FetchEndpointDataFromContext(
      string key,
      MustacheEvaluationContext context)
    {
      string str = (string) null;
      try
      {
        Dictionary<string, object> dictionary1 = new Dictionary<string, object>((context.ReplacementObject as JObject).ToObject<IDictionary<string, object>>(), (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
        if (dictionary1.ContainsKey("endpoint"))
        {
          Dictionary<string, string> dictionary2 = new Dictionary<string, string>((dictionary1["endpoint"] as JObject).ToObject<IDictionary<string, string>>(), (IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
          if (dictionary2.ContainsKey(key))
            str = dictionary2[key];
        }
      }
      catch (Exception ex)
      {
        return (string) null;
      }
      return str;
    }

    private static string ProcessGetFileContentRequest(
      FileContentMustacheExpressionArguments arguments)
    {
      string fileContentRequest = string.Empty;
      HttpWebRequest httpWebRequest = WebRequest.Create(arguments.Url) as HttpWebRequest;
      string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes("MustacheTemplateHelpers:" + arguments.AuthToken));
      httpWebRequest.ContentType = arguments.ContentType;
      httpWebRequest.Headers["Authorization"] = "Basic " + base64String;
      httpWebRequest.Method = arguments.Method;
      httpWebRequest.Timeout = 20000;
      using (HttpWebResponse result = httpWebRequest.GetResponseAsync().Result as HttpWebResponse)
      {
        if (result.StatusCode == HttpStatusCode.OK)
        {
          using (Stream responseStream = result.GetResponseStream())
          {
            using (StreamReader streamReader = new StreamReader(responseStream))
            {
              char[] buffer = new char[524288];
              Task<int> task = streamReader.ReadBlockAsync(buffer, 0, 524288);
              int length = task.Wait(20000) ? task.Result : throw new TimeoutException(Resources.HttpTimeoutException((object) 20));
              if (!streamReader.EndOfStream)
                throw new InvalidOperationException(Resources.FileContentResponseSizeExceeded());
              string str = JsonConvert.ToString(new string(buffer, 0, length));
              fileContentRequest = str.Substring(1, str.Length - 2);
            }
          }
        }
      }
      return fileContentRequest;
    }

    private static JArray GetTokensRecursive(JToken nodes, string property)
    {
      JArray tokensRecursive = new JArray();
      Stack<Tuple<int, JToken>> source = new Stack<Tuple<int, JToken>>();
      int num1 = 1;
      int num2 = 15;
      foreach (JToken node in (IEnumerable<JToken>) nodes)
        source.Push(Tuple.Create<int, JToken>(num1, node));
      while (source.Any<Tuple<int, JToken>>())
      {
        Tuple<int, JToken> tuple = source.Pop();
        int num3 = tuple.Item1;
        JToken jtoken1 = tuple.Item2;
        tokensRecursive.Add(jtoken1);
        JToken jtoken2 = jtoken1.SelectToken(property);
        if (jtoken2 != null)
        {
          if (num3 > num2)
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MaxResponseParsingDepthReached((object) num2)));
          foreach (JToken jtoken3 in (IEnumerable<JToken>) jtoken2)
            source.Push(Tuple.Create<int, JToken>(num3 + 1, jtoken3));
        }
      }
      return tokensRecursive;
    }

    private static JToken AddFieldRecursive(
      JToken jsonObject,
      string propertyToSearch,
      string parentPathKey,
      string parentPathProperty,
      string separator)
    {
      Stack<Tuple<int, JToken>> source = new Stack<Tuple<int, JToken>>();
      int num1 = 1;
      int num2 = 15;
      foreach (JToken jtoken in (IEnumerable<JToken>) jsonObject.SelectToken(propertyToSearch))
        source.Push(Tuple.Create<int, JToken>(num1, jtoken));
      while (source.Any<Tuple<int, JToken>>())
      {
        Tuple<int, JToken> tuple = source.Pop();
        int num3 = tuple.Item1;
        JToken jtoken1 = tuple.Item2;
        JToken jtoken2 = jtoken1.SelectToken(propertyToSearch);
        if (jtoken2 != null)
        {
          if (num3 > num2)
            throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.MaxResponseParsingDepthReached((object) num2)));
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", jtoken1[(object) parentPathKey] != null ? (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1}", (object) (string) jtoken1[(object) parentPathKey], (object) separator) : (object) string.Empty, (object) (string) jtoken1[(object) parentPathProperty]);
          foreach (JToken jtoken3 in (IEnumerable<JToken>) jtoken2)
          {
            if (!string.IsNullOrEmpty(str))
              jtoken3[(object) parentPathKey] = (JToken) str;
            source.Push(Tuple.Create<int, JToken>(num3 + 1, jtoken3));
          }
        }
      }
      return jsonObject;
    }

    private static string GetJsonProperty(JToken jsonObject, string path) => string.IsNullOrEmpty(path) || jsonObject == null ? (string) null : (string) jsonObject.SelectToken(path);

    private static string[] ExtractExpressionParts(string expression)
    {
      List<string> stringList = new List<string>();
      try
      {
        foreach (object match in Regex.Matches(expression, "[\\'].+?[\\']|[^ ]+", RegexOptions.None, TimeSpan.FromMilliseconds((double) (MustacheTemplateHelpers.MaxTimeOutInSeconds * 1000))))
          stringList.Add(match.ToString());
      }
      catch (RegexMatchTimeoutException ex)
      {
        throw new InvalidEndpointResponseException(Resources.RegexMatchTimeExceeded((object) expression, (object) (MustacheTemplateHelpers.MaxTimeOutInSeconds * 1000)));
      }
      return stringList.ToArray();
    }

    private static bool IsParametersArray(string expression)
    {
      Regex regex = new Regex("\\[.*\\]", RegexOptions.None, TimeSpan.FromMilliseconds((double) (MustacheTemplateHelpers.MaxTimeOutInSeconds * 1000)));
      try
      {
        return regex.Match(expression).Success;
      }
      catch (RegexMatchTimeoutException ex)
      {
        throw new InvalidEndpointResponseException(Resources.RegexMatchTimeExceeded((object) expression, (object) (MustacheTemplateHelpers.MaxTimeOutInSeconds * 1000)));
      }
    }

    private static string[] ExtractArrayFromParameters(string expression) => JArray.Parse(expression).ToObject<string[]>();

    private static string RemovePrefixAndSuffix(
      string stringToRemoveFrom,
      string prefix,
      string suffix = "")
    {
      if (!string.IsNullOrWhiteSpace(stringToRemoveFrom) && !string.IsNullOrWhiteSpace(prefix) && stringToRemoveFrom.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        stringToRemoveFrom = stringToRemoveFrom.Substring(prefix.Length).Trim();
      if (!string.IsNullOrWhiteSpace(stringToRemoveFrom) && !string.IsNullOrWhiteSpace(suffix) && stringToRemoveFrom.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
        stringToRemoveFrom = stringToRemoveFrom.Substring(0, stringToRemoveFrom.Length - suffix.Length).Trim();
      return stringToRemoveFrom;
    }

    private static string[] SplitExpression(string expression) => expression.Trim().Split(new char[1]
    {
      ' '
    }, StringSplitOptions.None);

    private static void RetrieveKeyAndResourceFromParts(
      string[] parts,
      out string itemKeyName,
      out string resourceName)
    {
      if (((IEnumerable<string>) parts).Count<string>() < 2)
      {
        itemKeyName = "defaultResultKey";
        resourceName = parts[0];
      }
      else
      {
        itemKeyName = parts[0];
        resourceName = parts[1];
      }
    }

    private static string ParseParameter(
      string paramValue,
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string parameter = string.Empty;
      if (paramValue.StartsWith("'"))
      {
        parameter = paramValue.Trim('\'');
      }
      else
      {
        JToken jtoken = MustacheTemplateHelpers.ToJToken(expression.GetCurrentToken(paramValue, context));
        if (jtoken != null && !jtoken.HasValues)
          parameter = jtoken.Value<string>();
      }
      return parameter;
    }

    private static JToken ToJToken(object value)
    {
      switch (value)
      {
        case JToken jtoken:
        case null:
          return jtoken;
        default:
          jtoken = JToken.FromObject(value);
          goto case null;
      }
    }

    private static bool IsNumber(string str)
    {
      try
      {
        int.Parse(str);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private static string[] GetSystemWhiteListedUrls(MustacheEvaluationContext context)
    {
      string[] systemWhiteListedUrls = (string[]) null;
      if (context.ReplacementObject != null)
      {
        JToken jtoken = MustacheTemplateHelpers.ToJToken(context.ReplacementObject);
        if (jtoken != null)
        {
          JToken source1 = jtoken.SelectToken("systemWhiteListedUrlList", false);
          if (source1 != null && source1.Count<JToken>() > 0)
          {
            IEnumerable<string> source2 = source1.Values<string>();
            if (source2 != null && source2.Count<string>() > 0)
              systemWhiteListedUrls = source2.ToArray<string>();
          }
        }
      }
      return systemWhiteListedUrls;
    }

    private static bool IsUrlWhiteListed(string url, string[] whiteListedUrls)
    {
      string stringSchemeAndServer1 = MustacheTemplateHelpers.GetUriStringSchemeAndServer(url);
      if (string.IsNullOrWhiteSpace(stringSchemeAndServer1) || whiteListedUrls == null || whiteListedUrls.Length == 0)
        return false;
      foreach (string whiteListedUrl in whiteListedUrls)
      {
        string stringSchemeAndServer2 = MustacheTemplateHelpers.GetUriStringSchemeAndServer(whiteListedUrl);
        if (!string.IsNullOrWhiteSpace(stringSchemeAndServer2) && stringSchemeAndServer2.Equals(stringSchemeAndServer1, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }

    private static string GetUriStringSchemeAndServer(string uriString) => string.IsNullOrWhiteSpace(uriString) ? string.Empty : new Uri(uriString).GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped).ToString().ToLowerInvariant();
  }
}
