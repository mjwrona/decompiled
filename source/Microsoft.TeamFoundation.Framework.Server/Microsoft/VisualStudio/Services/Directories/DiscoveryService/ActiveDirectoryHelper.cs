// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.ActiveDirectoryHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class ActiveDirectoryHelper
  {
    public static ActiveDirectoryHelper Instance
    {
      get => ActiveDirectoryHelper.Nested.instance;
      internal set => ActiveDirectoryHelper.Nested.instance = value;
    }

    internal virtual IList<IDirectoryEntity> SearchAd(
      IVssRequestContext context,
      string filter,
      int sizeLimit,
      IEnumerable<string> propertiesToReturn,
      string domainName = null)
    {
      context.TraceConditionally(11336800, TraceLevel.Verbose, "DirectoryDiscovery", "ActiveDirectory", (Func<string>) (() => "Searching with filter:" + filter + ", propertiesToReturn:" + string.Join(",", propertiesToReturn) + " and domainName:" + (domainName ?? string.Empty)));
      SortedSet<IDirectoryEntity> source = new SortedSet<IDirectoryEntity>(DirectoryEntityComparer.DefaultComparer);
      HashSet<string> propertiesToReturn1 = ActiveDirectoryHelper.GetAllPropertiesToReturn(propertiesToReturn);
      HashSet<string> propertiesToLoad = ActiveDirectoryHelper.GetActiveDirectoryPropertiesToLoad((IEnumerable<string>) propertiesToReturn1);
      foreach (SearchResult result in DirectorySearcherHelper.FindAll(context, filter, sizeLimit, propertiesToLoad, domainName))
      {
        IDirectoryEntity directoryEntity = ActiveDirectoryHelper.CreateDirectoryEntity(result, (IEnumerable<string>) propertiesToReturn1);
        if (directoryEntity != null)
          source.Add(directoryEntity);
      }
      return (IList<IDirectoryEntity>) source.ToList<IDirectoryEntity>();
    }

    internal virtual IDictionary<string, IDirectoryEntity> GetDirectoryEntities(
      IVssRequestContext context,
      IList<string> ids,
      IEnumerable<string> propertiesToReturn,
      SearchAttribute searchAttribute = SearchAttribute.ObjectSid)
    {
      Dictionary<string, IDirectoryEntity> directoryEntities = new Dictionary<string, IDirectoryEntity>((IEqualityComparer<string>) VssStringComparer.ActiveDirectoryEntityIdComparer);
      if (ids == null || ids.Count == 0)
        return (IDictionary<string, IDirectoryEntity>) directoryEntities;
      string usersGroupsWithIds = ActiveDirectoryHelper.GetLdapFilterForUsersGroupsWithIds(context, (IEnumerable<string>) ids, searchAttribute);
      int count = ids.Count;
      HashSet<string> propertiesToReturn1 = ActiveDirectoryHelper.GetAllPropertiesToReturn(propertiesToReturn);
      HashSet<string> propertiesToLoad = ActiveDirectoryHelper.GetActiveDirectoryPropertiesToLoad((IEnumerable<string>) propertiesToReturn1);
      foreach (SearchResult result in DirectorySearcherHelper.FindAll(context, usersGroupsWithIds, count, propertiesToLoad))
      {
        string id;
        if (ActiveDirectoryHelper.TryGetId(result, searchAttribute, out id))
        {
          IDirectoryEntity directoryEntity = ActiveDirectoryHelper.CreateDirectoryEntity(result, (IEnumerable<string>) propertiesToReturn1);
          if (directoryEntity != null)
            directoryEntities[id] = directoryEntity;
        }
      }
      return (IDictionary<string, IDirectoryEntity>) directoryEntities;
    }

    internal virtual IDictionary<string, byte[]> GetThumbnailPhotos(
      IVssRequestContext context,
      IList<string> objectSids)
    {
      Dictionary<string, byte[]> thumbnailPhotos = new Dictionary<string, byte[]>((IEqualityComparer<string>) VssStringComparer.ActiveDirectoryEntityIdComparer);
      if (objectSids == null || objectSids.Count == 0)
        return (IDictionary<string, byte[]>) thumbnailPhotos;
      string usersGroupsWithIds = ActiveDirectoryHelper.GetLdapFilterForUsersGroupsWithIds(context, (IEnumerable<string>) objectSids);
      int count = objectSids.Count;
      HashSet<string> propertiesToLoad = ActiveDirectoryHelper.GetActiveDirectoryPropertiesToLoad((IEnumerable<string>) ActiveDirectoryHelper.GetAllPropertiesToReturn((IEnumerable<string>) new string[1]
      {
        "ThumbnailPhoto"
      }));
      foreach (SearchResult result in DirectorySearcherHelper.FindAll(context, usersGroupsWithIds, count, propertiesToLoad))
      {
        SecurityIdentifier objectSid;
        byte[] thumbnailPhoto;
        if (ActiveDirectoryHelper.TryGetObjectSid(result, out objectSid) && ActiveDirectoryHelper.TryGetThumbnailPhoto(result, out thumbnailPhoto) && thumbnailPhoto != null)
          thumbnailPhotos[objectSid.ToString()] = thumbnailPhoto;
      }
      return (IDictionary<string, byte[]>) thumbnailPhotos;
    }

    internal static string GetLdapFilterForUsersGroupsWithIds(
      IVssRequestContext context,
      IEnumerable<string> ids,
      SearchAttribute searchAttribute = SearchAttribute.ObjectSid)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string format;
      if (searchAttribute != SearchAttribute.DistinguishedName)
      {
        if (searchAttribute != SearchAttribute.ObjectSid)
          throw new ArgumentException(nameof (searchAttribute));
        format = "(objectSid={0})";
      }
      else
        format = "(distinguishedName={0})";
      foreach (string id in ids)
      {
        string str = searchAttribute.Equals((object) SearchAttribute.DistinguishedName) ? ActiveDirectoryHelper.SanitizeStringForLdap(id) : id;
        stringBuilder.AppendFormat(format, (object) str);
      }
      return string.Format(context.To(TeamFoundationHostType.Deployment).GetService<ActiveDirectorySettingsService>().LdapSecurityGroupOrUserSearchStringFormat, (object) stringBuilder);
    }

    private static IDirectoryEntity CreateDirectoryEntity(
      SearchResult result,
      IEnumerable<string> propertiesToReturn)
    {
      IDirectoryEntity directoryEntity = (IDirectoryEntity) null;
      if (ActiveDirectoryHelper.ContainsPropertyValue<string>(result.Properties, "objectClass", "user"))
        directoryEntity = (IDirectoryEntity) ActiveDirectoryHelper.CreateDirectoryUser(result, propertiesToReturn);
      else if (ActiveDirectoryHelper.ContainsPropertyValue<string>(result.Properties, "objectClass", "group"))
        directoryEntity = (IDirectoryEntity) ActiveDirectoryHelper.CreateDirectoryGroup(result, propertiesToReturn);
      return directoryEntity;
    }

    internal static IDirectoryUser CreateDirectoryUser(
      SearchResult result,
      IEnumerable<string> propertiesToReturn)
    {
      SecurityIdentifier objectSid;
      if (!ActiveDirectoryHelper.TryGetObjectSid(result, out objectSid))
        return (IDirectoryUser) null;
      DirectoryUser directoryUser = new DirectoryUser();
      directoryUser.EntityId = DirectoryEntityHelper.CreateUserId(objectSid.ToString()).Encode();
      directoryUser.OriginDirectory = "ad";
      directoryUser.OriginId = objectSid.ToString();
      directoryUser.Properties = ActiveDirectoryHelper.GetDirectoryEntityPropertiesToReturn(result, propertiesToReturn, objectSid);
      return (IDirectoryUser) directoryUser;
    }

    internal static IDirectoryGroup CreateDirectoryGroup(
      SearchResult result,
      IEnumerable<string> propertiesToReturn)
    {
      SecurityIdentifier objectSid = (SecurityIdentifier) null;
      if (!ActiveDirectoryHelper.TryGetObjectSid(result, out objectSid))
        return (IDirectoryGroup) null;
      DirectoryGroup directoryGroup = new DirectoryGroup();
      directoryGroup.EntityId = DirectoryEntityHelper.CreateGroupId(objectSid.ToString()).Encode();
      directoryGroup.OriginDirectory = "ad";
      directoryGroup.OriginId = objectSid.ToString();
      directoryGroup.Properties = ActiveDirectoryHelper.GetDirectoryEntityPropertiesToReturn(result, propertiesToReturn, objectSid);
      return (IDirectoryGroup) directoryGroup;
    }

    internal static IDictionary<string, object> GetDirectoryEntityPropertiesToReturn(
      SearchResult searchResult,
      IEnumerable<string> propertiesToReturn,
      SecurityIdentifier sid)
    {
      Dictionary<string, object> propertiesToReturn1 = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer);
      foreach (string key in propertiesToReturn)
      {
        string propertyName = (string) null;
        object obj;
        if (ActiveDirectoryProperty.PropertiesToReturnMap.TryGetValue(key, out propertyName) && ActiveDirectoryHelper.TryGetPropertyValue(searchResult.Properties, propertyName, out obj))
          propertiesToReturn1[key] = obj;
      }
      if (!propertiesToReturn.Contains<string>("ScopeName", (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer))
        return (IDictionary<string, object>) propertiesToReturn1;
      string domain;
      SidIdentityHelper.ResolveSid(new SecurityIdentifierInfo(sid), out domain, out string _, out Microsoft.TeamFoundation.Common.Internal.NativeMethods.AccountType _, out bool _, out bool _);
      propertiesToReturn1["ScopeName"] = (object) domain;
      return (IDictionary<string, object>) propertiesToReturn1;
    }

    internal static HashSet<string> GetActiveDirectoryPropertiesToLoad(
      IEnumerable<string> propertiesToReturn)
    {
      HashSet<string> propertiesToLoad = new HashSet<string>((IEnumerable<string>) ActiveDirectoryProperty.ActiveDirectoryRequiredProperties, (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer);
      if (propertiesToReturn == null)
        return propertiesToLoad;
      foreach (string key in propertiesToReturn)
      {
        string str;
        if (ActiveDirectoryProperty.PropertiesToReturnMap.TryGetValue(key, out str))
          propertiesToLoad.Add(str);
      }
      return propertiesToLoad;
    }

    internal static HashSet<string> GetAllPropertiesToReturn(IEnumerable<string> propertiesToReturn)
    {
      HashSet<string> propertiesToReturn1 = new HashSet<string>((IEnumerable<string>) ActiveDirectoryProperty.DirectoryEntityRequiredProperties, (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer);
      if (propertiesToReturn == null)
        return propertiesToReturn1;
      propertiesToReturn1.UnionWith(propertiesToReturn);
      return propertiesToReturn1;
    }

    internal static bool TryGetId(
      SearchResult result,
      SearchAttribute searchAttribute,
      out string id)
    {
      id = (string) null;
      switch (searchAttribute)
      {
        case SearchAttribute.DistinguishedName:
          return ActiveDirectoryHelper.TryGetDistinguishedName(result, out id);
        case SearchAttribute.ObjectSid:
          SecurityIdentifier objectSid = (SecurityIdentifier) null;
          if (!ActiveDirectoryHelper.TryGetObjectSid(result, out objectSid))
            return false;
          id = objectSid.ToString();
          return true;
        default:
          return false;
      }
    }

    internal static bool TryGetObjectSid(SearchResult result, out SecurityIdentifier objectSid)
    {
      objectSid = (SecurityIdentifier) null;
      object obj = (object) null;
      if (!ActiveDirectoryHelper.TryGetPropertyValue(result.Properties, "ObjectSid", out obj) || !(obj is byte[] binaryForm))
        return false;
      objectSid = new SecurityIdentifier(binaryForm, 0);
      return true;
    }

    internal static bool TryGetThumbnailPhoto(SearchResult result, out byte[] thumbnailPhoto)
    {
      thumbnailPhoto = (byte[]) null;
      object obj = (object) null;
      if (!ActiveDirectoryHelper.TryGetPropertyValue(result.Properties, "ThumbnailPhoto", out obj))
        return false;
      thumbnailPhoto = obj as byte[];
      return thumbnailPhoto != null;
    }

    internal static bool TryGetDistinguishedName(SearchResult result, out string distinguishedName)
    {
      distinguishedName = (string) null;
      object obj = (object) null;
      if (!ActiveDirectoryHelper.TryGetPropertyValue(result.Properties, "Distinguishedname", out obj))
        return false;
      distinguishedName = obj as string;
      return distinguishedName != null;
    }

    internal static bool TryGetName(PropertyCollection properties, out string domainName)
    {
      domainName = (string) null;
      object obj = (object) null;
      if (!ActiveDirectoryHelper.TryGetPropertyValue(properties, "name", out obj))
        return false;
      domainName = obj as string;
      return domainName != null;
    }

    internal static bool TryGetPropertyValue(
      ResultPropertyCollection properties,
      string propertyName,
      out object value)
    {
      value = (object) null;
      if (!properties.Contains(propertyName))
        return false;
      ResultPropertyValueCollection property = properties[propertyName];
      if (property == null || property.Count <= 0)
        return false;
      value = property[0];
      return true;
    }

    internal static bool ContainsPropertyValue<T>(
      ResultPropertyCollection properties,
      string key,
      T value)
    {
      if (!properties.Contains(key))
        return false;
      ResultPropertyValueCollection property = properties[key];
      return property != null && property.Count > 0 && properties[key].Contains((object) value);
    }

    internal static bool TryGetObjectSid(DirectoryEntityIdentifier entityId, out string objectSid)
    {
      objectSid = string.Empty;
      if (entityId.Version != 1 || !(entityId is DirectoryEntityIdentifierV1 entityIdentifierV1) || !"ad".Equals(entityIdentifierV1.Source) || !"user".Equals(entityIdentifierV1.Type) && !"group".Equals(entityIdentifierV1.Type))
        return false;
      try
      {
        objectSid = new SecurityIdentifier(entityIdentifierV1.Id).ToString();
      }
      catch (ArgumentException ex)
      {
        return false;
      }
      return true;
    }

    internal static string SanitizeStringForLdap(string query)
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (char ch in query)
      {
        switch (ch)
        {
          case char.MinValue:
            stringBuilder.Append("\\00");
            break;
          case '(':
            stringBuilder.Append("\\28");
            break;
          case ')':
            stringBuilder.Append("\\29");
            break;
          case '*':
            stringBuilder.Append("\\2a");
            break;
          case '\\':
            stringBuilder.Append("\\5c");
            break;
          default:
            stringBuilder.Append(ch);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    internal static bool ContainsPropertyValue<T>(
      PropertyCollection properties,
      string key,
      T value)
    {
      if (!properties.Contains(key))
        return false;
      PropertyValueCollection property = properties[key];
      return property != null && property.Count > 0 && properties[key].Contains((object) value);
    }

    internal static bool TryGetPropertyValue(
      PropertyCollection properties,
      string propertyName,
      out object value)
    {
      value = (object) null;
      if (!properties.Contains(propertyName))
        return false;
      PropertyValueCollection property = properties[propertyName];
      if (property == null || property.Count <= 0)
        return false;
      value = property[0];
      return true;
    }

    internal static bool IsActiveDirectorySecurityIdentifier(string securityIdentifier)
    {
      try
      {
        return SidIdentityHelper.IsNTAccount(new SecurityIdentifierInfo(new SecurityIdentifier(securityIdentifier)).GetBinaryForm());
      }
      catch (ArgumentException ex)
      {
        return false;
      }
    }

    private class Nested
    {
      internal static ActiveDirectoryHelper instance = new ActiveDirectoryHelper();
    }
  }
}
