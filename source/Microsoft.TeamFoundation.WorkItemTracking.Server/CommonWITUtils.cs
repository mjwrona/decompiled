// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.CommonWITUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessValidator;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class CommonWITUtils
  {
    private static readonly Regex s_leadingNumbersCheck = new Regex("^\\d+$", RegexOptions.Compiled, CommonWorkItemTrackingConstants.RegexMatchTimeout);
    private const string c_RemoteLinkingEnabledCacheKey = "IsRemoteLinkingEnabled";
    private static readonly Dictionary<Guid, HashSet<string>> m_testWorkItemTypeCollectionForProject = new Dictionary<Guid, HashSet<string>>();
    private static readonly List<string> m_testWorkItemCategories = new List<string>()
    {
      "Microsoft.TestPlanCategory",
      "Microsoft.TestSuiteCategory",
      "Microsoft.TestCaseCategory",
      "Microsoft.SharedParameterCategory",
      "Microsoft.SharedStepCategory"
    };

    public static TargetType ConvertValue<TargetType>(object value)
    {
      if (value == null)
        return default (TargetType);
      if (typeof (TargetType).IsAssignableFrom(value.GetType()))
        return (TargetType) value;
      if (!(value is WorkItemIdentity))
        return (TargetType) Convert.ChangeType(value, typeof (TargetType));
      return typeof (TargetType).Equals(typeof (IdentityRef)) ? CommonWITUtils.ConvertValue<TargetType>((object) (value as WorkItemIdentity).IdentityRef) : CommonWITUtils.ConvertValue<TargetType>((object) (value as WorkItemIdentity).DistinctDisplayName);
    }

    public static T GetValue<T>(this IDictionary<int, object> dictionary, int key) => CommonWITUtils.ConvertValue<T>(dictionary[key]);

    public static T GetValueOrDefault<T>(
      this IDictionary<int, object> dictionary,
      int key,
      T defaultValue = null)
    {
      object obj;
      return dictionary.TryGetValue(key, out obj) ? CommonWITUtils.ConvertValue<T>(obj) : defaultValue;
    }

    public static string ConvertToStringForRuleCheck(object value)
    {
      if (value == null)
        return (string) null;
      Type type = value.GetType();
      if (type == typeof (int))
        return ((int) value).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      if (type == typeof (double))
        return ((double) value).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo);
      if (type == typeof (Guid))
        return ((Guid) value).ToString("D");
      if (type == typeof (string))
        return value as string;
      if (type == typeof (DateTime))
        return new SqlDateTime((DateTime) value).Value.ToString("yyyy-MM-ddTHH:mm:ss.fff");
      if (!(type == typeof (bool)))
        throw new NotSupportedException();
      return !(bool) value ? "0" : "1";
    }

    public static T FromString<T>(string value) => CommonWITUtils.FromString<T>(value, default (T));

    public static T FromString<T>(string value, T defaultValue)
    {
      if (typeof (T).IsAssignableFrom(typeof (string)))
        return (T) value;
      if (string.IsNullOrWhiteSpace(value))
        return defaultValue;
      TypeConverter converter = TypeDescriptor.GetConverter(typeof (T));
      return !converter.CanConvertFrom(typeof (string)) ? defaultValue : (T) converter.ConvertFromInvariantString(value);
    }

    public static bool IsValidSqlDateTime(DateTime date) => date >= SqlDateTime.MinValue.Value;

    public static IEnumerable<TResult> BatchResponse<T, TResult>(
      Func<IEnumerable<T>, IEnumerable<TResult>> responseFunction,
      IEnumerable<T> input,
      int batchSize)
    {
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 0);
      if (input.IsNullOrEmpty<T>())
        return Enumerable.Empty<TResult>();
      return batchSize == 0 ? (IEnumerable<TResult>) responseFunction(input).ToArray<TResult>() : CommonWITUtils._BatchResponse<T, TResult>(responseFunction, input, batchSize);
    }

    private static IEnumerable<TResult> _BatchResponse<T, TResult>(
      Func<IEnumerable<T>, IEnumerable<TResult>> responseFunction,
      IEnumerable<T> input,
      int batchSize)
    {
      List<T> batch = new List<T>();
      int elementsToProcess = input.Count<T>();
      foreach (T obj in input)
      {
        batch.Add(obj);
        if (--elementsToProcess == 0 || batch.Count >= batchSize)
        {
          foreach (TResult result in responseFunction((IEnumerable<T>) batch))
            yield return result;
          if (elementsToProcess == 0)
            break;
          batch.Clear();
        }
      }
    }

    public static void TraceIfEnabled(
      IVssRequestContext requestContext,
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      Func<string> messageFunc)
    {
      if (!requestContext.IsTracing(tracepoint, level, area, layer))
        return;
      requestContext.Trace(tracepoint, level, area, layer, messageFunc());
    }

    public static void Assert(
      IVssRequestContext requestContext,
      int tracepoint,
      string area,
      string layer,
      Func<bool> assertionFunc,
      Func<string> messageFunc)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(area, nameof (area));
      ArgumentUtility.CheckForNull<string>(layer, nameof (layer));
      ArgumentUtility.CheckForNull<Func<bool>>(assertionFunc, nameof (assertionFunc));
      ArgumentUtility.CheckForNull<Func<string>>(messageFunc, nameof (messageFunc));
      if (!requestContext.IsTracing(tracepoint, TraceLevel.Error, area, layer) || assertionFunc())
        return;
      requestContext.Trace(tracepoint, TraceLevel.Error, area, layer, messageFunc());
    }

    public static string GenerateReferenceName(string firstPart, string secondPart)
    {
      string str = CommonWITUtils.RemoveWhiteSpace(firstPart) + "." + CommonWITUtils.RemoveWhiteSpace(secondPart);
      return str.IsASCII() ? str : CommonWITUtils.GenerateUniqueRefName();
    }

    public static string GenerateUniqueRefName() => "Custom." + Guid.NewGuid().ToString();

    public static bool IsASCII(this string value) => Encoding.UTF8.GetByteCount(value) == value.Length;

    public static void CheckValidName(string name, int maxLength) => CommonWITUtils.CheckValidName(name, maxLength, WorkItemTypeMetadata.IllegalNameChars);

    public static void CheckValidName(string name, int maxLength, char[] charsToCheck)
    {
      if (string.IsNullOrWhiteSpace(name) || name.Length > maxLength || name.IndexOfAny(charsToCheck) != -1 || CommonWITUtils.s_leadingNumbersCheck.IsMatch(name))
      {
        string str = string.Join<char>("", (IEnumerable<char>) charsToCheck);
        throw new ArgumentException(ServerResources.WorkItemTrackingNameInvalid((object) name, (object) maxLength, (object) str));
      }
    }

    public static string GetDefaultWorkItemTypeColor() => "FF009CCC";

    public static void CheckColor(string color)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(color, nameof (color));
      try
      {
        Convert.ToInt32(color, 16);
      }
      catch (FormatException ex)
      {
        throw new ArgumentException(ServerResources.WorkItemTrackingInvalidColor((object) color));
      }
      catch (OverflowException ex)
      {
        throw new ArgumentException(ServerResources.WorkItemTrackingInvalidColor((object) color));
      }
      catch (ArgumentOutOfRangeException ex)
      {
        throw new ArgumentException(ServerResources.WorkItemTrackingInvalidColor((object) color));
      }
      if (color.Length != 6)
        throw new ArgumentException(ServerResources.WorkItemTrackingInvalidColor((object) color));
    }

    public static void CheckColorWithHashSymbol(string color)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(color, nameof (color));
      if (!color.StartsWith("#"))
        throw new ArgumentException(ServerResources.WorkItemTrackingInvalidColorWithoutHash((object) color));
      CommonWITUtils.CheckColor(color.Remove(0, 1));
    }

    public static string RemoveWhiteSpace(string name)
    {
      StringBuilder stringBuilder = new StringBuilder(name.Length);
      foreach (char c in name.ToCharArray())
      {
        if (!char.IsWhiteSpace(c))
          stringBuilder.Append(c);
      }
      return stringBuilder.ToString();
    }

    public static string RemoveASCIIControlCharactersAndTrim(string str)
    {
      StringBuilder stringBuilder = new StringBuilder(str.Length);
      foreach (char ch in str)
      {
        int int32 = Convert.ToInt32(ch);
        if (int32 >= 32 && int32 != (int) sbyte.MaxValue)
          stringBuilder.Append(ch);
      }
      return stringBuilder.ToString().Trim();
    }

    public static void TraceRawResultCount(IVssRequestContext requestContext, int rawResultCount) => requestContext.Trace(906009, TraceLevel.Verbose, "Query", "WorkItemQueryService", string.Format("Number of raw result entries: {0}", (object) rawResultCount));

    public static void TraceRawAndFilteredResultCount(
      IVssRequestContext requestContext,
      int rawResultCount,
      int filteredResultCount)
    {
      requestContext.Trace(906012, TraceLevel.Verbose, "Query", "WorkItemQueryService", string.Format("Finished filtering result entries. Number of raw result entries: {0}. Number of filtered result entries: {1}", (object) rawResultCount, (object) filteredResultCount));
    }

    public static string GetNodeFullPathInHexadecimal(TreeNode node)
    {
      List<int> source = new List<int>();
      for (TreeNode treeNode = node; treeNode != null; treeNode = treeNode.Parent)
        source.Add(treeNode.Id);
      return string.Join(string.Empty, source.Reverse<int>().Select<int, string>((Func<int, string>) (id => id.ToString("x8", (IFormatProvider) NumberFormatInfo.InvariantInfo))));
    }

    public static string GetNodeRangeEndPathValueInHexadecimal(string binaryPathHex) => binaryPathHex + "FFFFFFFF";

    [Conditional("DEBUG")]
    private static void AssertIfDebug(Func<bool> assertionFunc, Func<string> messageFunc)
    {
      if (!assertionFunc())
        throw new Exception(messageFunc());
    }

    public static IReadOnlyCollection<string> ExtractConstantsFromRules(
      IList<WorkItemFieldRule> fieldRules)
    {
      return (IReadOnlyCollection<string>) fieldRules.SelectMany<WorkItemFieldRule, string>((Func<WorkItemFieldRule, IEnumerable<string>>) (rule => rule.ExtractConstants())).ToList<string>();
    }

    public static string ValidateAndSanitizeDescription(string description, string metadataName)
    {
      if (description != null)
        description = description.Length <= 256 ? CommonWITUtils.RemoveASCIIControlCharactersAndTrim(description) : throw new ArgumentException(ServerResources.WorkItemTrackingDescriptionInvalid((object) metadataName, (object) 256));
      return description;
    }

    public static bool IsTestWorkItem(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemType)
    {
      bool flag = false;
      HashSet<string> collection;
      if (!CommonWITUtils.m_testWorkItemTypeCollectionForProject.TryGetValue(projectId, out collection))
      {
        collection = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (WorkItemTypeCategory itemTypeCategory in requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, projectId).Where<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (workItemCategory => CommonWITUtils.m_testWorkItemCategories.Contains<string>(workItemCategory.ReferenceName, (IEqualityComparer<string>) TFStringComparer.WorkItemCategoryReferenceName))))
          collection.AddRange<string, HashSet<string>>(itemTypeCategory.WorkItemTypeNames);
        CommonWITUtils.m_testWorkItemTypeCollectionForProject[projectId] = collection;
      }
      if (collection != null && collection.Contains(workItemType))
        flag = true;
      return flag;
    }

    public static string GetSerializedRuleXML(WorkItemFieldRule[] fieldRules) => TeamFoundationSerializationUtility.SerializeToString<WorkItemFieldRule[]>(fieldRules, new XmlRootAttribute("rules")).Replace(" rule-id=\"00000000-0000-0000-0000-000000000000\"", "").Replace(" forVsId=\"00000000-0000-0000-0000-000000000000\"", "").Replace(" notVsId=\"00000000-0000-0000-0000-000000000000\"", "");

    public static string GetSha1HashString(string text)
    {
      if (text == null)
        return "";
      byte[] bytes = Encoding.UTF8.GetBytes(text);
      using (SHA1Managed shA1Managed = new SHA1Managed())
        return HexConverter.ToString(shA1Managed.ComputeHash(bytes));
    }

    public static string NormalizeWiql(string wiql) => wiql == null ? wiql : wiql.Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace(" ", "").ToLower();

    public static bool HasCrossProjectQueryPermission(IVssRequestContext requestContext) => CommonWITUtils.HasPermission(requestContext, QueryItemSecurityConstants.NamespaceGuid, QueryItemSecurityConstants.RootFolder, 32);

    public static bool HasRecordQueryExecutionInfoPermission(IVssRequestContext requestContext) => CommonWITUtils.HasPermission(requestContext, QueryItemSecurityConstants.NamespaceGuid, QueryItemSecurityConstants.RootFolder, 64);

    public static bool CanAccessCrossProjectWorkItems(IVssRequestContext requestContext) => CommonWITUtils.HasPermission(requestContext, WorkItemTrackingNamespaceSecurityConstants.NamespaceId, WorkItemTrackingNamespaceSecurityConstants.WorkItemTrackingToken, 2);

    public static bool HasCrossProjectQueryArtifactUriPermission(IVssRequestContext requestContext) => CommonWITUtils.HasPermission(requestContext, WitConstants.SecurityConstants.WorkItemTrackingNamespaceId, WitConstants.SecurityConstants.WorkItemTrackingArtifactUriQueryToken, 2);

    public static bool HasTrackRecentActivityPermission(IVssRequestContext requestContext) => CommonWITUtils.HasPermission(requestContext, WorkItemTrackingNamespaceSecurityConstants.NamespaceId, WorkItemTrackingNamespaceSecurityConstants.WorkItemTrackingToken, 4);

    private static bool HasPermission(
      IVssRequestContext requestContext,
      Guid namespaceGuid,
      string token,
      int permission,
      bool checkActionDefined = false)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess"))
        return true;
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, namespaceGuid);
      return checkActionDefined && !securityNamespace.Description.Actions.Exists((Predicate<ActionDefinition>) (a => a.Bit == permission)) || securityNamespace.HasPermission(requestContext, token, permission, false);
    }

    public static bool HasReadRulesPermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WorkItemTrackingNamespaceSecurityConstants.NamespaceId).HasPermission(requestContext, WorkItemTrackingNamespaceSecurityConstants.WorkItemTrackingToken, 8, false);

    public static bool HasReadHistoricalWorkItemResourcesPermission(
      IVssRequestContext requestContext)
    {
      return CommonWITUtils.HasPermission(requestContext, WorkItemTrackingNamespaceSecurityConstants.NamespaceId, WorkItemTrackingNamespaceSecurityConstants.WorkItemTrackingToken, 16, true);
    }

    public static bool IsRemoteLinkingEnabled(IVssRequestContext requestContext)
    {
      bool flag;
      if (!requestContext.Items.TryGetValue<bool>(nameof (IsRemoteLinkingEnabled), out flag))
      {
        flag = !requestContext.IsClientOm() && requestContext.WitContext().IsAadBackedAccount;
        requestContext.Items[nameof (IsRemoteLinkingEnabled)] = (object) flag;
      }
      return flag;
    }

    public static string GetOOBProcessChangedETag(
      IVssRequestContext requestContext,
      Guid projectGuid)
    {
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor;
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && service.TryGetLatestProjectProcessDescriptor(requestContext, projectGuid, out processDescriptor))
      {
        if (processDescriptor.IsSystem && processDescriptor.Version != null)
          return processDescriptor.Version.Major.ToString() + "." + processDescriptor.Version.Minor.ToString();
        ProcessDescriptor descriptor;
        if (processDescriptor.IsDerived && requestContext.GetService<ITeamFoundationProcessService>().TryGetProcessDescriptor(requestContext, processDescriptor.Inherits, out descriptor) && descriptor.Version != null)
          return descriptor.Version.Major.ToString() + "." + descriptor.Version.Minor.ToString();
      }
      return string.Empty;
    }

    public static void VerifyNoDuplicates(IEnumerable<string> items)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (string str in items)
      {
        string lowerInvariant = str.ToLowerInvariant();
        if (stringSet.Contains(lowerInvariant))
          throw new ArgumentException(ServerResources.DuplicateListItem((object) str));
        stringSet.Add(lowerInvariant);
      }
    }

    public static IReadOnlyList<string> ValidateAndGetPickListItems(
      IVssRequestContext requestContext,
      IReadOnlyList<string> items,
      InternalFieldType type,
      string listName)
    {
      List<string> items1 = new List<string>();
      for (int index = 0; index < items.Count; ++index)
      {
        string str = items[index]?.Trim();
        ArgumentUtility.CheckStringForNullOrWhiteSpace(str, string.Format("{0}[{1}]", (object) nameof (items), (object) index));
        switch (type)
        {
          case InternalFieldType.Integer:
            if (!int.TryParse(str, out int _))
              throw new ArgumentException(ServerResources.InvalidListItemType((object) items[index], (object) "Integer"), nameof (items));
            break;
          case InternalFieldType.Double:
            if (!double.TryParse(str, out double _))
              throw new ArgumentException(ServerResources.InvalidListItemType((object) items[index], (object) "Double"), nameof (items));
            break;
        }
        items1.Add(str);
      }
      CommonWITUtils.VerifyListItemsAgainstMetadataLimits(requestContext, listName, items);
      CommonWITUtils.VerifyNoDuplicates((IEnumerable<string>) items1);
      return (IReadOnlyList<string>) items1;
    }

    public static void VerifyListItemsAgainstMetadataLimits(
      IVssRequestContext requestContext,
      string name,
      IReadOnlyList<string> items)
    {
      IWitProcessTemplateValidatorConfiguration validatorConfig = requestContext.WitContext().TemplateValidatorConfiguration;
      if (items.Count > validatorConfig.MaxPickListItemsPerList)
        throw new WorkItemPickListItemCountLimitExceededException(name, items.Count, validatorConfig.MaxPickListItemsPerList);
      if (items.Any<string>((Func<string, bool>) (s => s.Length > validatorConfig.MaxPickListItemLength)))
        throw new WorkItemPickListItemLengthLimitExceededException(name, validatorConfig.MaxPickListItemLength);
    }
  }
}
