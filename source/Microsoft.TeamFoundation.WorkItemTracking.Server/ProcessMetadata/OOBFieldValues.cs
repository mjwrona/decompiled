// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.OOBFieldValues
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  public class OOBFieldValues
  {
    private const string PriorityRefName = "Microsoft.VSTS.Common.Priority";

    public static bool TryGetAllowedOrSuggestedValues(
      IVssRequestContext requestContext,
      int fieldId,
      out IReadOnlyCollection<string> values)
    {
      return OOBFieldValues.TryGetAllowedValues(requestContext, fieldId, out values) || OOBFieldValues.TryGetSuggestedValues(requestContext, fieldId, out values);
    }

    public static bool TryGetAllowedOrSuggestedValues(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      out IReadOnlyCollection<string> values)
    {
      return OOBFieldValues.TryGetAllowedValues(requestContext, fieldReferenceName, out values) || OOBFieldValues.TryGetSuggestedValues(requestContext, fieldReferenceName, out values);
    }

    public static bool TryGetAllowedValues(
      IVssRequestContext requestContext,
      int fieldId,
      out IReadOnlyCollection<string> values)
    {
      values = (IReadOnlyCollection<string>) null;
      FieldEntry field;
      int num = !requestContext.WitContext().FieldDictionary.TryGetField(fieldId, out field) ? 0 : (OOBFieldValues.GetFieldAllowedValuesMappings(requestContext).TryGetValue(field.ReferenceName, out values) ? 1 : 0);
      if (num == 0)
        return num != 0;
      values = OOBFieldValues.AddCustomValues(requestContext, field.ReferenceName, values);
      return num != 0;
    }

    public static bool TryGetAllowedValues(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      out IReadOnlyCollection<string> values)
    {
      values = (IReadOnlyCollection<string>) null;
      int num = string.IsNullOrWhiteSpace(fieldReferenceName) ? 0 : (OOBFieldValues.GetFieldAllowedValuesMappings(requestContext).TryGetValue(fieldReferenceName, out values) ? 1 : 0);
      if (num == 0)
        return num != 0;
      values = OOBFieldValues.AddCustomValues(requestContext, fieldReferenceName, values);
      return num != 0;
    }

    public static bool TryGetSuggestedValues(
      IVssRequestContext requestContext,
      int fieldId,
      out IReadOnlyCollection<string> values)
    {
      values = (IReadOnlyCollection<string>) null;
      FieldEntry field;
      return requestContext.WitContext().FieldDictionary.TryGetField(fieldId, out field) && OOBFieldValues.GetFieldSuggestedValuesMappings(requestContext).TryGetValue(field.ReferenceName, out values);
    }

    public static bool TryGetSuggestedValues(
      IVssRequestContext requestContext,
      string fieldReferenceName,
      out IReadOnlyCollection<string> values)
    {
      values = (IReadOnlyCollection<string>) null;
      return !string.IsNullOrWhiteSpace(fieldReferenceName) && OOBFieldValues.GetFieldSuggestedValuesMappings(requestContext).TryGetValue(fieldReferenceName, out values);
    }

    public static bool HasAllowedValues(
      IVssRequestContext requestContext,
      string fieldReferenceName)
    {
      return !string.IsNullOrWhiteSpace(fieldReferenceName) && OOBFieldValues.GetFieldAllowedValuesMappings(requestContext).ContainsKey(fieldReferenceName);
    }

    public static bool HasSuggestedValues(
      IVssRequestContext requestContext,
      string fieldReferenceName)
    {
      return !string.IsNullOrWhiteSpace(fieldReferenceName) && OOBFieldValues.GetFieldSuggestedValuesMappings(requestContext).ContainsKey(fieldReferenceName);
    }

    public static IReadOnlyCollection<string> GetAllowedValuesConstants(
      IVssRequestContext requestContext)
    {
      HashSet<string> allowedValuesConstants = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.AllowedValue);
      foreach (KeyValuePair<string, IReadOnlyCollection<string>> allowedValuesMapping in (IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>>) OOBFieldValues.GetFieldAllowedValuesMappings(requestContext))
      {
        allowedValuesConstants.UnionWith((IEnumerable<string>) allowedValuesMapping.Value);
        allowedValuesConstants.Add(FieldsConstants.AllowedValuesListHead(allowedValuesMapping.Key));
      }
      return (IReadOnlyCollection<string>) allowedValuesConstants;
    }

    public static IReadOnlyCollection<string> GetSuggestedValuesConstants(
      IVssRequestContext requestContext)
    {
      HashSet<string> suggestedValuesConstants = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.AllowedValue);
      foreach (KeyValuePair<string, IReadOnlyCollection<string>> suggestedValuesMapping in (IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>>) OOBFieldValues.GetFieldSuggestedValuesMappings(requestContext))
      {
        suggestedValuesConstants.UnionWith((IEnumerable<string>) suggestedValuesMapping.Value);
        suggestedValuesConstants.Add(FieldsConstants.SuggestedValuesListHead(suggestedValuesMapping.Key));
      }
      return (IReadOnlyCollection<string>) suggestedValuesConstants;
    }

    public static IReadOnlyCollection<string> AddCustomValues(
      IVssRequestContext requestContext,
      string fieldRefName,
      IReadOnlyCollection<string> existingValues)
    {
      List<string> stringList = new List<string>();
      if (TFStringComparer.WorkItemFieldReferenceName.Equals("Microsoft.VSTS.Common.Priority", fieldRefName) && WorkItemTrackingFeatureFlags.IsPriorityZeroEnabled(requestContext))
        stringList = new List<string>() { "0" };
      stringList.AddRange((IEnumerable<string>) existingValues);
      return (IReadOnlyCollection<string>) stringList;
    }

    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetFieldAllowedValuesMappings(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<WorkItemTrackingOutOfBoxPicklistCache>().GetFieldAllowedValuesMappings(requestContext);
    }

    private static IReadOnlyDictionary<string, IReadOnlyCollection<string>> GetFieldSuggestedValuesMappings(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<WorkItemTrackingOutOfBoxPicklistCache>().GetFieldSuggestedValuesMappings(requestContext);
    }
  }
}
