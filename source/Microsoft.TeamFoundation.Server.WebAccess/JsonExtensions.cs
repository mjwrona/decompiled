// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.JsonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class JsonExtensions
  {
    public const int c_DefaultMaxJsonLength = 20971520;
    private static readonly long DateTimeMinTimeTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

    public static long ToJavascriptTicks(this DateTime dateTime) => (dateTime.ToUniversalTime().Ticks - JsonExtensions.DateTimeMinTimeTicks) / 10000L;

    public static JsObject ToJson(this ActivityLogParameter parameter)
    {
      JsObject json = new JsObject();
      json["name"] = (object) parameter.Name;
      json["value"] = (object) parameter.Value;
      json["index"] = (object) parameter.Index;
      return json;
    }

    public static JsObject ToJson(this ITeamFoundationDatabaseProperties database)
    {
      JsObject json = new JsObject();
      json["databaseId"] = (object) database.DatabaseId;
      json["databaseName"] = (object) database.DatabaseName;
      json["lastTenantAdded"] = (object) database.LastTenantAdded;
      json["maxTenants"] = (object) database.MaxTenants;
      json["poolName"] = (object) database.PoolName;
      json["serviceLevel"] = (object) database.ServiceLevel;
      json["status"] = (object) database.Status;
      json["statusChangedDate"] = (object) database.StatusChangedDate;
      json["statusReason"] = (object) database.StatusReason;
      json["tenants"] = (object) database.Tenants;
      json["pendingDeletes"] = (object) database.TenantsPendingDelete;
      return json;
    }

    public static JsObject ToJson(this TeamFoundationDatabaseTenantUsage tenantUsage)
    {
      JsObject json = new JsObject();
      json["tenantHostId"] = (object) tenantUsage.TenantHostId;
      json["tenantName"] = (object) tenantUsage.TenantName;
      json["executionCount"] = (object) tenantUsage.ExecutionCount;
      json["executionTime"] = (object) tenantUsage.ExecutionTime;
      json["lastAccess"] = (object) tenantUsage.LastAccess;
      json["rows"] = (object) tenantUsage.Rows;
      return json;
    }

    public static JsObject ToJson(this ActivityLogEntry entry)
    {
      JsObject json = new JsObject();
      json["commandId"] = (object) entry.CommandId;
      json["application"] = (object) entry.Application;
      json["command"] = (object) entry.Command;
      json["status"] = (object) entry.Status;
      json["startTime"] = (object) entry.StartTime;
      json["executionTime"] = (object) entry.ExecutionTime;
      json["identityName"] = (object) entry.IdentityName;
      json["ipAddress"] = (object) entry.IpAddress;
      json["uniqueIdentifier"] = (object) entry.UniqueIdentifier;
      json["userAgent"] = (object) entry.UserAgent;
      json["commandIdentifier"] = (object) entry.CommandIdentifier;
      json["executionCount"] = (object) entry.ExecutionCount;
      json["authenticationType"] = (object) entry.AuthenticationType;
      json["responseCode"] = (object) entry.ResponseCode;
      json["parameters"] = entry.Parameters != null ? (object) entry.Parameters.Select<ActivityLogParameter, JsObject>((Func<ActivityLogParameter, JsObject>) (param => param.ToJson())) : (object) (IEnumerable<JsObject>) null;
      return json;
    }

    public static string ToJson<T>(T value, params JavaScriptConverter[] converters) => JsonExtensions.ToJson<T>(value, 20971520, converters);

    public static string ToJson<T>(
      T value,
      int maxJsonLength,
      params JavaScriptConverter[] converters)
    {
      JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
      if (maxJsonLength != -1)
        scriptSerializer.MaxJsonLength = maxJsonLength;
      if (converters != null)
        scriptSerializer.RegisterConverters((IEnumerable<JavaScriptConverter>) converters);
      return scriptSerializer.Serialize((object) value);
    }

    public static T FromJson<T>(
      string json,
      int maxJsonLength,
      params JavaScriptConverter[] converters)
    {
      JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
      if (maxJsonLength != -1)
        scriptSerializer.MaxJsonLength = maxJsonLength;
      if (converters != null)
        scriptSerializer.RegisterConverters((IEnumerable<JavaScriptConverter>) converters);
      return scriptSerializer.Deserialize<T>(json);
    }

    public static object FromJson(
      string json,
      Type targetType,
      params JavaScriptConverter[] converters)
    {
      return JsonExtensions.FromJson(json, targetType, 20971520, converters);
    }

    public static object FromJson(
      string json,
      Type targetType,
      int maxJsonLength,
      params JavaScriptConverter[] converters)
    {
      JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
      scriptSerializer.MaxJsonLength = maxJsonLength;
      if (converters != null)
        scriptSerializer.RegisterConverters((IEnumerable<JavaScriptConverter>) converters);
      return scriptSerializer.Deserialize(json, targetType);
    }

    public static string ToJson<T>(T value, params JsonConverter[] converters)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        Converters = (IList<JsonConverter>) ((IEnumerable<JsonConverter>) converters).ToList<JsonConverter>()
      };
      return JsonConvert.SerializeObject((object) value, settings);
    }

    public static T FromJson<T>(string json, params JsonConverter[] converters)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        Converters = (IList<JsonConverter>) ((IEnumerable<JsonConverter>) converters).ToList<JsonConverter>()
      };
      return JsonConvert.DeserializeObject<T>(json, settings);
    }

    public static object FromJson(string json, Type targetType, params JsonConverter[] converters)
    {
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        Converters = (IList<JsonConverter>) ((IEnumerable<JsonConverter>) converters).ToList<JsonConverter>()
      };
      return JsonConvert.DeserializeObject(json, targetType, settings);
    }

    public static JsObject ToJson(
      this TeamProjectCollectionProperties collectionProperties)
    {
      if (collectionProperties == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("instanceId", (object) collectionProperties.Id);
      json.Add("name", (object) collectionProperties.Name);
      return json;
    }

    public static JsObject ToJson(
      this TfsServiceHostDescriptor serviceHostDescriptor)
    {
      if (serviceHostDescriptor == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("instanceId", (object) serviceHostDescriptor.Id);
      json.Add("name", (object) serviceHostDescriptor.Name);
      json.Add("vDir", (object) serviceHostDescriptor.VirtualDirectory);
      json.Add("relVDir", (object) serviceHostDescriptor.RelativeVirtualDirectory);
      json.Add("hostType", (object) serviceHostDescriptor.HostType);
      return json;
    }

    public static JsObject ToJson(this MRUNavigationContextEntry mruEntry)
    {
      if (mruEntry == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("isMru", (object) true);
      json.Add("topMostLevel", (object) mruEntry.TopMostLevel);
      json.Add("serviceHost", (object) mruEntry.ServiceHost.ToJson());
      json.Add("project", (object) mruEntry.Project);
      json.Add("team", (object) mruEntry.Team);
      json.Add("lastAccessedByUser", (object) mruEntry.LastAccessedByUser);
      return json;
    }

    public static JsObject ToJson(this ProjectInfo projectInfo)
    {
      if (projectInfo == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("name", (object) projectInfo.Name);
      json.Add("status", (object) projectInfo.State.ToString());
      json.Add("uri", (object) projectInfo.Uri);
      return json;
    }

    public static JsObject ToJson(this CommonStructureProjectInfo projectInfo)
    {
      if (projectInfo == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("name", (object) projectInfo.Name);
      json.Add("status", (object) projectInfo.Status.ToString());
      json.Add("uri", (object) projectInfo.Uri);
      return json;
    }

    public static JsObject ToJson(this WebApiTeam team)
    {
      if (team == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("identity", (object) JsonExtensions.ToJson(team.Identity));
      json.Add("name", (object) team.Name);
      return json;
    }

    public static JsObject ToJson(this Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("id", (object) identity.Id);
      json.Add("isContainer", (object) identity.IsContainer);
      json.Add("isActive", (object) identity.IsActive);
      json.Add("displayName", (object) identity.DisplayName);
      json.Add("customDisplayName", (object) identity.CustomDisplayName);
      json.Add("providerDisplayName", (object) identity.ProviderDisplayName);
      json.Add("uniqueName", (object) IdentityHelper.GetUniqueName(identity));
      json.Add("email", (object) identity.GetProperty<string>("Mail", string.Empty));
      return json;
    }

    public static JsObject ToJson(this IdentityRef identity)
    {
      if (identity == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("id", (object) identity.Id);
      json.Add("displayName", (object) identity.DisplayName);
      json.Add("uniqueName", (object) identity.UniqueName);
      json.Add("isContainer", (object) identity.IsContainer);
      json.Add("isAadIdentity", (object) identity.IsAadIdentity);
      json.Add("descriptor", (object) identity.Descriptor.ToString());
      json.Add("url", (object) identity.Url);
      json.Add("imageUrl", (object) identity.ImageUrl);
      json.Add("_links", (object) identity.Links?.Links);
      return json;
    }

    public static JsObject ToJsonIncludingPreferredEmail(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      JsObject json = JsonExtensions.ToJson(identity);
      json.Add("preferredEmail", (object) IdentityHelper.GetPreferredEmailAddress(requestContext, identity.Id));
      return json;
    }
  }
}
