// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.MustacheTemplateEngine
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public class MustacheTemplateEngine
  {
    public static int MaxTimeOutInSeconds = 10;
    private readonly Dictionary<string, MustacheTemplateHelperMethod> mustacheTemplateHelpers = new Dictionary<string, MustacheTemplateHelperMethod>();

    public string EvaluateTemplate(string template, JToken replacementContext)
    {
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      MustacheOptions options = new MustacheOptions()
      {
        MaxDepth = 20,
        CancellationToken = cancellationTokenSource.Token
      };
      try
      {
        cancellationTokenSource.CancelAfter(MustacheTemplateEngine.MaxTimeOutInSeconds * 1000);
        MustacheTemplateParser mustacheTemplateParser = new MustacheTemplateParser(true, true, (Dictionary<string, string>) null, options);
        this.RegisterHelpers(mustacheTemplateParser);
        return mustacheTemplateParser.ReplaceValues(template, (object) replacementContext);
      }
      catch (OperationCanceledException ex)
      {
        throw new ServiceEndpointQueryFailedException(Resources.TemplateEvaluationTimeExceeded((object) template, (object) (MustacheTemplateEngine.MaxTimeOutInSeconds * 1000)));
      }
      catch (MustacheExpressionInvalidException ex)
      {
        throw new ServiceEndpointQueryFailedException(ex.Message, (Exception) ex);
      }
      catch (MustacheEvaluationResultLengthException ex)
      {
        throw new ServiceEndpointQueryFailedException(ex.Message, (Exception) ex);
      }
      finally
      {
        cancellationTokenSource.Dispose();
      }
    }

    public void RegisterHelper(string helperName, MustacheTemplateHelperMethod helper) => this.mustacheTemplateHelpers.Add(helperName, helper);

    private void RegisterHelpers(MustacheTemplateParser mustacheTemplateParser)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("#jsonEscape", MustacheTemplateEngine.\u003C\u003EO.\u003C0\u003E__JsonEscapeHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C0\u003E__JsonEscapeHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.JsonEscapeHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("#regex", MustacheTemplateEngine.\u003C\u003EO.\u003C1\u003E__RegexHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C1\u003E__RegexHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.RegexHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("#stringReplace", MustacheTemplateEngine.\u003C\u003EO.\u003C2\u003E__StringReplaceHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C2\u003E__StringReplaceHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.StringReplaceHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("#subString", MustacheTemplateEngine.\u003C\u003EO.\u003C3\u003E__SubStringHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C3\u003E__SubStringHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.SubStringHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("#extractResource", MustacheTemplateEngine.\u003C\u003EO.\u003C4\u003E__ExtractResourceHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C4\u003E__ExtractResourceHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ExtractResourceHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("#base64", MustacheTemplateEngine.\u003C\u003EO.\u003C5\u003E__Base64Helper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C5\u003E__Base64Helper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.Base64Helper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("#decodeBase64", MustacheTemplateEngine.\u003C\u003EO.\u003C6\u003E__DecodeBase64Helper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C6\u003E__DecodeBase64Helper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.DecodeBase64Helper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("selectMaxOfLong", MustacheTemplateEngine.\u003C\u003EO.\u003C7\u003E__SelectMaxOfHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C7\u003E__SelectMaxOfHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.SelectMaxOfHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("splitAndIterate", MustacheTemplateEngine.\u003C\u003EO.\u003C8\u003E__SplitAndIterateHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C8\u003E__SplitAndIterateHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.SplitAndIterateHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("recursiveFormat", MustacheTemplateEngine.\u003C\u003EO.\u003C9\u003E__RecursiveFormatHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C9\u003E__RecursiveFormatHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.RecursiveFormatHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("addField", MustacheTemplateEngine.\u003C\u003EO.\u003C10\u003E__AddFieldHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C10\u003E__AddFieldHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.AddFieldHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("recursiveSelect", MustacheTemplateEngine.\u003C\u003EO.\u003C11\u003E__RecursiveSelectHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C11\u003E__RecursiveSelectHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.RecursiveSelectHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("splitAndPrefix", MustacheTemplateEngine.\u003C\u003EO.\u003C12\u003E__SplitAndPrefixHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C12\u003E__SplitAndPrefixHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.SplitAndPrefixHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("extractNumbersAndSort", MustacheTemplateEngine.\u003C\u003EO.\u003C13\u003E__ExtractNumbersAndSortHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C13\u003E__ExtractNumbersAndSortHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ExtractNumbersAndSortHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("joinPaths", MustacheTemplateEngine.\u003C\u003EO.\u003C14\u003E__JoinPathsHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C14\u003E__JoinPathsHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.JoinPathsHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("removePrefixFromPath", MustacheTemplateEngine.\u003C\u003EO.\u003C15\u003E__RemovePrefixFromPathHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C15\u003E__RemovePrefixFromPathHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.RemovePrefixFromPathHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("copyFieldFromParent", MustacheTemplateEngine.\u003C\u003EO.\u003C16\u003E__CopyFieldFromParentHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C16\u003E__CopyFieldFromParentHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.CopyFieldFromParentHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("selectTokens", MustacheTemplateEngine.\u003C\u003EO.\u003C17\u003E__SelectTokensHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C17\u003E__SelectTokensHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.SelectTokensHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("sortBy", MustacheTemplateEngine.\u003C\u003EO.\u003C18\u003E__SortByHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C18\u003E__SortByHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.SortByHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("add", MustacheTemplateEngine.\u003C\u003EO.\u003C19\u003E__Add ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C19\u003E__Add = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.Add)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("isEqualNumber", MustacheTemplateEngine.\u003C\u003EO.\u003C20\u003E__IsEqualNumber ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C20\u003E__IsEqualNumber = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.IsEqualNumber)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("isTokenPresent", MustacheTemplateEngine.\u003C\u003EO.\u003C21\u003E__IsTokenPresent ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C21\u003E__IsTokenPresent = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.IsTokenPresent)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("endsWith", MustacheTemplateEngine.\u003C\u003EO.\u003C22\u003E__EndswithHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C22\u003E__EndswithHelper = new MustacheTemplateHelperWriter(MustacheTemplateHelpers.EndswithHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("extractUrlQueryParameter", MustacheTemplateEngine.\u003C\u003EO.\u003C23\u003E__ExtractUrlQueryParameter ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C23\u003E__ExtractUrlQueryParameter = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ExtractUrlQueryParameter)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("extractUrlQueryParamKeyValue", MustacheTemplateEngine.\u003C\u003EO.\u003C24\u003E__ExtractUrlQueryParamKeyValue ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C24\u003E__ExtractUrlQueryParamKeyValue = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ExtractUrlQueryParamKeyValue)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("getTokenValue", MustacheTemplateEngine.\u003C\u003EO.\u003C25\u003E__GetTokenValue ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C25\u003E__GetTokenValue = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.GetTokenValue)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("toCommaSeparatedKeyValueList", MustacheTemplateEngine.\u003C\u003EO.\u003C26\u003E__ToCommaSeparatedKeyValueList ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C26\u003E__ToCommaSeparatedKeyValueList = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ToCommaSeparatedKeyValueList)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("getFileContent", MustacheTemplateEngine.\u003C\u003EO.\u003C27\u003E__GetFileContentHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C27\u003E__GetFileContentHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.GetFileContentHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("toLower", MustacheTemplateEngine.\u003C\u003EO.\u003C28\u003E__ToLower ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C28\u003E__ToLower = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ToLower)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("toLowerInvariant", MustacheTemplateEngine.\u003C\u003EO.\u003C29\u003E__ToLowerInvariant ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C29\u003E__ToLowerInvariant = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ToLowerInvariant)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("shortGuid", MustacheTemplateEngine.\u003C\u003EO.\u003C30\u003E__ShortGuid ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C30\u003E__ShortGuid = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ShortGuid)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("toAlphaNumericString", MustacheTemplateEngine.\u003C\u003EO.\u003C31\u003E__ToAlphaNumericString ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C31\u003E__ToAlphaNumericString = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ToAlphaNumericString)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("newGuid", MustacheTemplateEngine.\u003C\u003EO.\u003C32\u003E__NewGuid ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C32\u003E__NewGuid = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.NewGuid)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("toDateTimeFormat", MustacheTemplateEngine.\u003C\u003EO.\u003C33\u003E__ToDateTimeFormat ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C33\u003E__ToDateTimeFormat = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.ToDateTimeFormat)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("isTokenContainsSubstring", MustacheTemplateEngine.\u003C\u003EO.\u003C34\u003E__IsTokenContainsSubstring ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C34\u003E__IsTokenContainsSubstring = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.IsTokenContainsSubstring)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("base64", MustacheTemplateEngine.\u003C\u003EO.\u003C5\u003E__Base64Helper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C5\u003E__Base64Helper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.Base64Helper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("decodeBase64", MustacheTemplateEngine.\u003C\u003EO.\u003C6\u003E__DecodeBase64Helper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C6\u003E__DecodeBase64Helper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.DecodeBase64Helper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mustacheTemplateParser.RegisterHelper("uriDataEncode", MustacheTemplateEngine.\u003C\u003EO.\u003C35\u003E__UriDataEncodeHelper ?? (MustacheTemplateEngine.\u003C\u003EO.\u003C35\u003E__UriDataEncodeHelper = new MustacheTemplateHelperMethod(MustacheTemplateHelpers.UriDataEncodeHelper)));
      foreach (KeyValuePair<string, MustacheTemplateHelperMethod> mustacheTemplateHelper in this.mustacheTemplateHelpers)
        mustacheTemplateParser.RegisterHelper(mustacheTemplateHelper.Key, mustacheTemplateHelper.Value);
    }
  }
}
