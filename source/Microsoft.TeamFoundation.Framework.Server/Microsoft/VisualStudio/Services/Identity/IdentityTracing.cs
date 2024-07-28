// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityTracing
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityTracing
  {
    private const string s_otherSearchFactor = "OtherSearchFactors";

    public static string FormatWithSafeSerialization(string format, params object[] args) => IdentityTracing.FormatWithSafeSerialization(format, args, false);

    public static string FormatWithSafeSerialization(
      string format,
      object[] args,
      bool includeStackTrace)
    {
      int length = args == null ? 0 : args.Length;
      string[] strArray = new string[length];
      for (int index = 0; index < length; ++index)
      {
        try
        {
          strArray[index] = JsonConvert.SerializeObject(args[index]);
        }
        catch (Exception ex)
        {
          strArray[index] = string.Format("[Exception thrown while serializing argument {0}: {1}]", (object) index, (object) ex.ToReadableStackTrace());
        }
      }
      string str;
      try
      {
        str = string.Format(format, (object[]) strArray);
        if (includeStackTrace)
          str = str + "\nStack trace: " + EnvironmentWrapper.ToReadableStackTrace();
      }
      catch (Exception ex)
      {
        str = "Exception thrown while generating formatted message: " + ex.ToReadableStackTrace();
      }
      return str;
    }

    public static void TraceRawSerializedConditionally(
      int tracepoint,
      TraceLevel level,
      string area,
      string layer,
      object arg)
    {
      if (level != TraceLevel.Error && !TeamFoundationTracingService.IsRawTracingEnabled(tracepoint, level, area, layer, (string[]) null))
        return;
      string message;
      try
      {
        message = JsonConvert.SerializeObject(arg);
      }
      catch (Exception ex)
      {
        message = "Exception thrown while generating trace message: " + ex.ToReadableStackTrace();
      }
      TeamFoundationTracingService.TraceRaw(tracepoint, level, area, layer, message);
    }

    public static IdentityTracing.ReadIdentityTraceKind CovertToReadIdentityTraceKind(
      IdentitySearchFilter searchFactor)
    {
      switch (searchFactor)
      {
        case IdentitySearchFilter.AccountName:
          return IdentityTracing.ReadIdentityTraceKind.BySearchFactorAccountName;
        case IdentitySearchFilter.DisplayName:
          return IdentityTracing.ReadIdentityTraceKind.BySearchFactorDisplayName;
        case IdentitySearchFilter.AdministratorsGroup:
          return IdentityTracing.ReadIdentityTraceKind.BySearchFactorAdministratorsGroup;
        case IdentitySearchFilter.MailAddress:
          return IdentityTracing.ReadIdentityTraceKind.BySearchFactorMailAddress;
        case IdentitySearchFilter.General:
          return IdentityTracing.ReadIdentityTraceKind.BySearchFactorGeneral;
        default:
          return IdentityTracing.ReadIdentityTraceKind.BySearchFactorOthers;
      }
    }

    public static int ConvertToReadIdentityTracePoint(
      IdentityTracing.ReadIdentityTraceKind readIdentityTraceKind,
      QueryMembership queryMembership,
      int extendedPropertiesCount,
      int identityCount)
    {
      return 13000000 + (int) readIdentityTraceKind * 100000 + (int) queryMembership * 10000 + extendedPropertiesCount * 100 + identityCount;
    }

    public static void TraceReadIdentity(
      string className,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      ReadIdentitiesOptions options,
      string callStack)
    {
      IdentityTracing.TraceReadIdentityInternal(className, IdentityTracing.CovertToReadIdentityTraceKind(searchFactor).ToString(), factorValue, queryMembership, propertyNameFilters, options.ToString(), callStack);
    }

    public static void TraceReadIdentity(
      string className,
      IList<Guid> identityIds,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      string callStack)
    {
      foreach (Guid identityId in (IEnumerable<Guid>) identityIds)
        IdentityTracing.TraceReadIdentityInternal(className, "ById", identityId.ToString(), queryMembership, propertyNameFilters, string.Format("includeRestrictedVisibility: {0}", (object) includeRestrictedVisibility), callStack);
    }

    public static void TraceReadIdentity(
      string className,
      IList<IdentityDescriptor> descriptors,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      bool includeRestrictedVisibility,
      string callStack)
    {
      foreach (IdentityDescriptor descriptor in (IEnumerable<IdentityDescriptor>) descriptors)
        IdentityTracing.TraceReadIdentityInternal(className, "ByDescriptor", descriptor.ToString(), queryMembership, propertyNameFilters, string.Format("includeRestrictedVisibility: {0}", (object) includeRestrictedVisibility), callStack);
    }

    public static void TraceReadIdentity(
      string className,
      Guid scopeId,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      string callStack)
    {
      IdentityTracing.TraceReadIdentityInternal(className, "ByScope", scopeId.ToString(), queryMembership, propertyNameFilters, string.Empty, callStack);
    }

    public static void TraceReadIdentityFromDatabase(
      IdentityDomain hostDomain,
      IdentitySearchFilter searchFilter,
      string factorValue,
      string domain,
      string account,
      int uniqueUserId,
      QueryMembership queryMembership,
      bool filterResults,
      IEnumerable<Guid> identityIdResults,
      string callStack)
    {
      var data = new
      {
        SearchFilter = searchFilter.ToString(),
        SearchFactorValue = factorValue,
        Domain = domain,
        Account = account,
        UniqueUserId = uniqueUserId,
        FilterResults = filterResults,
        ReadIdentityIdResults = identityIdResults
      };
      string eventValue;
      try
      {
        eventValue = JsonConvert.SerializeObject((object) data);
      }
      catch (Exception ex)
      {
        eventValue = string.Format("Exception thrown while serializing ReadIdentityFromDatabase parameters : {0}", (object) ex);
      }
      IdentityTracing.TraceIdentitySqlChangesInternal(IdentityTracing.IdentityTraceEventType.ReadBySearchFactor, hostDomain, eventValue, queryMembership, callStack);
    }

    public static void TraceReadIdentityFromDatabase(
      IdentityDomain hostDomain,
      IdentityDescriptor descriptor,
      QueryMembership parentQuery,
      QueryMembership childrenQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults,
      string callStack)
    {
      var data = new
      {
        IdentityDescriptor = descriptor,
        ParentQuery = parentQuery,
        ChildrenQuery = childrenQuery,
        IncludeRestrictedMembers = includeRestrictedMembers,
        IncludeInactiveMembers = includeInactivatedMembers,
        FilterResults = filterResults
      };
      string eventValue;
      try
      {
        eventValue = JsonConvert.SerializeObject((object) data);
      }
      catch (Exception ex)
      {
        eventValue = string.Format("Exception thrown while serializing ReadIdentityFromDatabase parameters : {0}", (object) ex);
      }
      IdentityTracing.TraceIdentitySqlChangesInternal(IdentityTracing.IdentityTraceEventType.ReadByIdentifier, hostDomain, eventValue, parentQuery, callStack);
    }

    public static void TraceReadIdentityFromDatabase(
      IdentityDomain hostDomain,
      Guid identityId,
      QueryMembership parentQuery,
      QueryMembership childrenQuery,
      bool includeRestrictedMembers,
      bool includeInactivatedMembers,
      bool filterResults,
      Microsoft.VisualStudio.Services.Identity.Identity returnedIdentity,
      string callStack)
    {
      var data = new
      {
        IdentityId = identityId,
        ParentQuery = parentQuery,
        ChildrenQuery = childrenQuery,
        IncludeRestrictedMembers = includeRestrictedMembers,
        IncludeInactiveMembers = includeInactivatedMembers,
        FilterResults = filterResults,
        ReturnedIdentity = returnedIdentity
      };
      string eventValue;
      try
      {
        eventValue = JsonConvert.SerializeObject((object) data);
      }
      catch (Exception ex)
      {
        eventValue = string.Format("Exception thrown while serializing ReadIdentityFromDatabase parameters : {0}", (object) ex);
      }
      IdentityTracing.TraceIdentitySqlChangesInternal(IdentityTracing.IdentityTraceEventType.ReadByIdentityId, hostDomain, eventValue, parentQuery, callStack);
    }

    public static void TraceUpdateIdentityInDatabase(
      IdentityDomain hostDomain,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool allowMetaDataUpdates,
      HashSet<string> propertiesToUpdate,
      bool isIdentityChanged,
      Microsoft.VisualStudio.Services.Identity.Identity changedIdentity,
      string callStack)
    {
      var data = new
      {
        Identity = identity,
        AllowMetaDataUpdates = allowMetaDataUpdates,
        PropertiesToUpdate = propertiesToUpdate,
        IsIdentityChanged = isIdentityChanged,
        ChangedIdentity = changedIdentity
      };
      string eventValue;
      try
      {
        eventValue = JsonConvert.SerializeObject((object) data);
      }
      catch (Exception ex)
      {
        eventValue = string.Format("Exception thrown while serializing UpdateIdentityInDatabase parameters : {0}", (object) ex);
      }
      IdentityTracing.TraceIdentitySqlChangesInternal(IdentityTracing.IdentityTraceEventType.UpdateByIdentityId, hostDomain, eventValue, QueryMembership.None, callStack);
    }

    public static void TraceIdentitySqlInvalidateById(
      IdentityDomain hostDomain,
      Guid identityId,
      string callStack)
    {
      IdentityTracing.TraceIdentitySqlChangesInternal(IdentityTracing.IdentityTraceEventType.InvalidateByIdentityId, hostDomain, identityId.ToString(), QueryMembership.None, callStack);
    }

    public static void TraceIdentityCacheRead(
      IdentityTracing.TargetStoreType storeType,
      IdentityDomain hostDomain,
      string identifier,
      QueryMembership queryMembership,
      IdentityTracing.CacheResult readResult,
      string callStack)
    {
      IdentityTracing.TraceIdentityCacheChangesInternal(storeType, IdentityTracing.IdentityTraceEventType.ReadByIdentifier, "OtherSearchFactors", hostDomain, identifier, queryMembership, readResult, callStack);
    }

    public static void TraceIdentityCacheRead(
      IdentityTracing.TargetStoreType storeType,
      IdentityDomain hostDomain,
      Guid identityId,
      QueryMembership queryMembership,
      IdentityTracing.CacheResult readResult,
      string callStack)
    {
      IdentityTracing.TraceIdentityCacheChangesInternal(storeType, IdentityTracing.IdentityTraceEventType.ReadByIdentityId, "OtherSearchFactors", hostDomain, identityId.ToString(), queryMembership, readResult, callStack);
    }

    public static void TraceIdentityCacheRead(
      IdentityTracing.TargetStoreType storeType,
      IdentityDomain hostDomain,
      IdentitySearchFilter searchFactor,
      string factorValue,
      QueryMembership queryMembership,
      IdentityTracing.CacheResult readResult,
      string callStack)
    {
      IdentityTracing.TraceIdentityCacheChangesInternal(storeType, IdentityTracing.IdentityTraceEventType.ReadBySearchFactor, searchFactor.ToString(), hostDomain, factorValue, queryMembership, readResult, callStack);
    }

    public static void TraceIdentityCacheUpdate(
      IdentityTracing.TargetStoreType storeType,
      IdentityDomain hostDomain,
      Guid identityId,
      QueryMembership queryMembership,
      string callStack)
    {
      IdentityTracing.TraceIdentityCacheChangesInternal(storeType, IdentityTracing.IdentityTraceEventType.UpdateByIdentityId, string.Empty, hostDomain, identityId.ToString(), queryMembership, IdentityTracing.CacheResult.NotApplicable, callStack);
    }

    public static void TraceIdentityCacheInvalidationByDescriptorChange(
      IdentityTracing.TargetStoreType storeType,
      IdentityDomain hostDomain,
      IEnumerable<Guid> descriptors,
      IdentityTracing.CacheResult result,
      string callStack)
    {
      string eventValue;
      try
      {
        eventValue = JsonConvert.SerializeObject((object) descriptors);
      }
      catch (Exception ex)
      {
        eventValue = string.Format("Exception thrown while serializing descriptors collection : {0}", (object) ex);
      }
      IdentityTracing.TraceIdentityCacheChangesInternal(storeType, IdentityTracing.IdentityTraceEventType.InvalidateByDescriptorChange, string.Empty, hostDomain, eventValue, QueryMembership.None, result, callStack);
    }

    public static void TraceIdentityCacheInvalidationByIdentityChange(
      IdentityTracing.TargetStoreType storeType,
      IdentityDomain hostDomain,
      IEnumerable<Guid> identityIds,
      IdentityTracing.CacheResult result,
      string callStack)
    {
      string eventValue;
      try
      {
        eventValue = JsonConvert.SerializeObject((object) identityIds);
      }
      catch (Exception ex)
      {
        eventValue = string.Format("Exception thrown while serializing identity ID collection : {0}", (object) ex);
      }
      IdentityTracing.TraceIdentityCacheChangesInternal(storeType, IdentityTracing.IdentityTraceEventType.InvalidateByIdentityChange, string.Empty, hostDomain, eventValue, QueryMembership.None, result, callStack);
    }

    public static void TraceIdentityCacheClear(
      IdentityTracing.TargetStoreType storeType,
      IdentityDomain hostDomain,
      IdentityTracing.IdentityTraceEventSize clearSize,
      string callStack = "")
    {
      IdentityTracing.TraceIdentityCacheChangesInternal(storeType, IdentityTracing.IdentityTraceEventType.Clear, string.Empty, hostDomain, clearSize.ToString(), QueryMembership.None, IdentityTracing.CacheResult.Hit, callStack);
    }

    public static void TraceToken(string className, JwtSecurityToken jwtToken, string nonce)
    {
      if (jwtToken == null && string.IsNullOrWhiteSpace(nonce))
        return;
      string header;
      try
      {
        header = JsonConvert.SerializeObject((object) jwtToken?.Header);
      }
      catch (Exception ex)
      {
        header = "Exception thrown while serializing JwtSecurityToken Header : " + ex.ToReadableStackTrace();
      }
      string claims;
      try
      {
        claims = JsonConvert.SerializeObject((object) jwtToken?.Claims);
      }
      catch (Exception ex)
      {
        claims = "Exception thrown while serializing JwtSecurityToken Claims: " + ex.ToReadableStackTrace();
      }
      TeamFoundationTracingService.TraceIdentityTokenOperation(className, header, claims, string.IsNullOrWhiteSpace(nonce) ? string.Empty : nonce);
    }

    private static void TraceReadIdentityInternal(
      string className,
      string flavor,
      string identifier,
      QueryMembership queryMembership,
      IEnumerable<string> propertyNameFilters,
      string options,
      string callStack)
    {
      string empty = string.Empty;
      string propertyNameFilters1;
      try
      {
        propertyNameFilters1 = JsonConvert.SerializeObject((object) propertyNameFilters);
      }
      catch (Exception ex)
      {
        propertyNameFilters1 = "Exception thrown while serializing property name filters : " + ex.ToReadableStackTrace();
      }
      TeamFoundationTracingService.TraceIdentityReadsOperation(className, flavor, identifier, queryMembership.ToString(), propertyNameFilters1, options, callStack);
    }

    private static void TraceIdentityCacheChangesInternal(
      IdentityTracing.TargetStoreType storeType,
      IdentityTracing.IdentityTraceEventType eventType,
      string searchFilter,
      IdentityDomain hostDomain,
      string eventValue,
      QueryMembership queryMembership,
      IdentityTracing.CacheResult result,
      string callStack = "")
    {
      TeamFoundationTracingService.TraceIdentityCacheChangesOperation(storeType.ToString(), eventType.ToString(), searchFilter ?? string.Empty, hostDomain?.ToString() ?? string.Empty, eventValue ?? string.Empty, queryMembership.ToString(), result.ToString(), callStack);
    }

    private static void TraceIdentitySqlChangesInternal(
      IdentityTracing.IdentityTraceEventType eventType,
      IdentityDomain hostDomain,
      string eventValue,
      QueryMembership queryMembership,
      string callStack = "")
    {
      TeamFoundationTracingService.TraceIdentitySqlChangesOperation(eventType.ToString(), hostDomain?.ToString() ?? string.Empty, eventValue ?? string.Empty, queryMembership.ToString(), callStack);
    }

    public enum ReadIdentityTraceKind
    {
      ByDescriptor,
      ById,
      BySearchFactorAccountName,
      BySearchFactorDisplayName,
      BySearchFactorAdministratorsGroup,
      BySearchFactorGeneral,
      BySearchFactorMailAddress,
      BySearchFactorOthers,
      ByScope,
      ByDomainId,
      BySubjectDescriptor,
    }

    public enum TargetStoreType
    {
      L1Cache,
      SQL,
    }

    public enum CacheResult
    {
      NotApplicable = -1, // 0xFFFFFFFF
      Hit = 0,
      Miss = 1,
      Invalidated = 2,
    }

    public enum IdentityTraceEventType
    {
      ReadByIdentifier,
      ReadByIdentityId,
      ReadBySearchFactor,
      UpdateByIdentityId,
      InvalidateByDescriptorChange,
      InvalidateByIdentityChange,
      GetIdentitiesInScope,
      SetIdentitiesInScope,
      InvalidateByIdentityId,
      Clear,
    }

    public enum IdentityTraceEventSize
    {
      Single,
      Bulk,
    }
  }
}
