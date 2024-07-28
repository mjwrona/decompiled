// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.GraphConnection
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  public class GraphConnection
  {
    public const string ActionGetObjectsByObjectIds = "getObjectsByObjectIds";
    public const string ActionActivateService = "activateService";
    public const string ActionGetTenantsByAlternateSecurityId = "getTenantsByAlternativeSecurityId";
    public const string ActionGetTenantsByKey = "getTenantsByKey";
    public const string ActionGetServicePrincipalsByAppIds = "getServicePrincipalsByAppIds";
    public const string ActionConsentToApp = "consentToApp";
    public const string ActionRevokeUserConsentToApp = "revokeUserConsentToApp";
    public const string ActionEnableDirectoryFeature = "enableDirectoryFeature";
    public const string ActionIsDirectoryFeatureEnabled = "isDirectoryFeatureEnabled";
    public const string RedeemInvitation = "redeemInvitation";
    public const string ActionBackfillApplication = "backfillApplication";
    public const string ActionLinkServicePrincipal = "linkServicePrincipalToApplication";
    private readonly ThreadLocal<bool> returnBatchItem = new ThreadLocal<bool>((Func<bool>) (() => false));
    private readonly ThreadLocal<IList<BatchRequestItem>> batchRequestItems = new ThreadLocal<IList<BatchRequestItem>>((Func<IList<BatchRequestItem>>) (() => (IList<BatchRequestItem>) new System.Collections.Generic.List<BatchRequestItem>()));
    private string tenantId = "myorganization";
    private bool overrideTenantId;

    public virtual bool ActivateService(string serviceName)
    {
      Utils.ThrowIfNullOrEmpty((object) serviceName, nameof (serviceName));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "activateService");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, string>()
      {
        [nameof (serviceName)] = serviceName
      });
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults == null || pagedResults.MixedResults.Count <= 0)
        return false;
      bool result;
      bool.TryParse(pagedResults.MixedResults[0], out result);
      return result;
    }

    public virtual User InviteUser(User user)
    {
      Utils.ThrowIfNullOrEmpty((object) user, nameof (user));
      this.ValidatePropertiesForInvite(user);
      Uri requestUri = Utils.GetRequestUri(this, typeof (User), string.Empty);
      string json = SerializationHelper.SerializeToJson((object) Utils.GetSerializableGraphObject((GraphObject) user));
      string str = "POST";
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem(str, true, requestUri, (WebHeaderCollection) null, json));
        return (User) null;
      }
      PagedResults<User> pagedResults = SerializationHelper.DeserializeJsonResponse<User>(this.ClientConnection.UploadString(requestUri, str, json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults == null || pagedResults.Results == null || pagedResults.Results.Count != 1)
        throw new InvalidOperationException("Unable to deserialize the response");
      user.ChangedProperties.Clear();
      pagedResults.Results[0].ChangedProperties.Clear();
      return pagedResults.Results[0];
    }

    public virtual User RedeemUserInvitation(
      string acceptedAs,
      AlternativeSecurityId altSecId,
      InvitationTicket inviteTicket)
    {
      Utils.ThrowIfNullOrEmpty((object) acceptedAs, nameof (acceptedAs));
      Utils.ThrowIfNullOrEmpty((object) altSecId, nameof (altSecId));
      Utils.ThrowIfNullOrEmpty((object) inviteTicket, nameof (inviteTicket));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, (string) null, "redeemInvitation");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, object>()
      {
        [nameof (acceptedAs)] = (object) acceptedAs,
        ["altSecIds"] = (object) new AlternativeSecurityId[1]
        {
          altSecId
        },
        [nameof (inviteTicket)] = (object) inviteTicket
      });
      string str = "POST";
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem(str, true, requestUri, (WebHeaderCollection) null, json));
        return (User) null;
      }
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, str, json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults == null || pagedResults.Results == null || pagedResults.Results.Count != 1)
        throw new InvalidOperationException("Unable to deserialize the response");
      pagedResults.Results[0].ChangedProperties.Clear();
      return pagedResults.Results[0] as User;
    }

    public virtual IList<GraphObject> GetObjectsByObjectIds(IList<string> objectIds)
    {
      Utils.ThrowIfNullOrEmpty((object) objectIds, nameof (objectIds));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "getObjectsByObjectIds");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, IList<string>>()
      {
        [nameof (objectIds)] = objectIds
      });
      return SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri)?.Results;
    }

    public virtual IList<GuestTenantDetail> GetTenantsByAlternativeSecurityId(
      AlternativeSecurityIdentifierType identifierType,
      string key)
    {
      Utils.ThrowIfNullOrEmpty((object) key, nameof (key));
      System.Collections.Generic.List<GuestTenantDetail> alternativeSecurityId = new System.Collections.Generic.List<GuestTenantDetail>();
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "getTenantsByAlternativeSecurityId");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, AlternativeSecurityId>()
      {
        ["alternativeSecurityId"] = new AlternativeSecurityId()
        {
          Type = new int?((int) identifierType),
          Key = Utils.HexToBinDecode(key)
        }
      });
      foreach (string mixedResult in (IEnumerable<string>) SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri).MixedResults)
        alternativeSecurityId.Add(JsonConvert.DeserializeObject<GuestTenantDetail>(mixedResult));
      return (IList<GuestTenantDetail>) alternativeSecurityId;
    }

    public virtual IList<GuestTenantDetail> GetTenantsByKey(
      AlternativeSecurityIdentifierType identifierType,
      string key,
      string homeTenantId)
    {
      Utils.ThrowIfNullOrEmpty((object) key, nameof (key));
      System.Collections.Generic.List<GuestTenantDetail> tenantsByKey = new System.Collections.Generic.List<GuestTenantDetail>();
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "getTenantsByKey");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, object>()
      {
        ["alternativeSecurityId"] = (object) new AlternativeSecurityId()
        {
          Type = new int?((int) identifierType),
          Key = Utils.HexToBinDecode(key)
        },
        [nameof (homeTenantId)] = (object) homeTenantId
      });
      foreach (string mixedResult in (IEnumerable<string>) SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri).MixedResults)
        tenantsByKey.Add(JsonConvert.DeserializeObject<GuestTenantDetail>(mixedResult));
      return (IList<GuestTenantDetail>) tenantsByKey;
    }

    public virtual IList<ServicePrincipal> GetServicePrincipalsByAppIds(IList<string> appIds)
    {
      Utils.ThrowIfNullOrEmpty((object) appIds, nameof (appIds));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "getServicePrincipalsByAppIds");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, IList<string>>()
      {
        [nameof (appIds)] = appIds
      });
      return SerializationHelper.DeserializeJsonResponse<ServicePrincipal>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri).Results;
    }

    public virtual bool ConsentToApp(
      string clientAppId,
      bool onBehalfOfAll,
      IList<string> tags,
      bool checkOnly)
    {
      Utils.ThrowIfNullOrEmpty((object) clientAppId, nameof (clientAppId));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "consentToApp");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, object>()
      {
        [nameof (clientAppId)] = (object) clientAppId,
        [nameof (onBehalfOfAll)] = (object) onBehalfOfAll,
        [nameof (tags)] = (object) tags,
        [nameof (checkOnly)] = (object) checkOnly
      });
      return bool.Parse(SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri).MixedResults[0]);
    }

    public virtual bool RevokeUserConsentToApp(string clientAppId)
    {
      Utils.ThrowIfNullOrEmpty((object) clientAppId, nameof (clientAppId));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "revokeUserConsentToApp");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, string>()
      {
        [nameof (clientAppId)] = clientAppId
      });
      return bool.Parse(SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri).MixedResults[0]);
    }

    public virtual IList<User> GetTransitiveGroupMembers(
      string groupId,
      bool preserveCycles,
      out Group rootGroup)
    {
      Utils.ThrowIfNullOrEmpty((object) groupId, nameof (groupId));
      if (preserveCycles)
        throw new ArgumentException("preserveCycles is not implemented yet.");
      System.Collections.Generic.List<User> transitiveGroupMembers = new System.Collections.Generic.List<User>();
      rootGroup = this.Get(typeof (Group), groupId) as Group;
      HashSet<string> groupsExhausted = new HashSet<string>();
      HashSet<string> stringSet = new HashSet<string>();
      Dictionary<string, string> pendingGroupsAndPageTokens = new Dictionary<string, string>();
      Dictionary<string, Group> dictionary = new Dictionary<string, Group>();
      pendingGroupsAndPageTokens[groupId] = (string) null;
      dictionary[groupId] = rootGroup;
      while (pendingGroupsAndPageTokens.Keys.Count > 0)
      {
        System.Collections.Generic.List<string> requestedGroupIds;
        System.Collections.Generic.List<BatchRequestItem> batchRequests = this.GetBatchRequests(pendingGroupsAndPageTokens, out requestedGroupIds);
        requestedGroupIds.ForEach((Action<string>) (x => pendingGroupsAndPageTokens.Remove(x)));
        IList<BatchResponseItem> batchResponseItemList = this.ExecuteBatch((IList<BatchRequestItem>) batchRequests);
        requestedGroupIds.ForEach((Action<string>) (x => groupsExhausted.Add(x)));
        int index = 0;
        foreach (BatchResponseItem batchResponseItem in (IEnumerable<BatchResponseItem>) batchResponseItemList)
        {
          if (batchResponseItem.Failed)
          {
            if (batchResponseItem.Exception != null)
              throw batchResponseItem.Exception;
            throw new GraphException("One of the batch items failed.");
          }
          if (!string.IsNullOrEmpty(batchResponseItem.ResultSet.PageToken))
          {
            pendingGroupsAndPageTokens[requestedGroupIds[index]] = batchResponseItem.ResultSet.PageToken;
            groupsExhausted.Remove(requestedGroupIds[index]);
          }
          Group group1 = dictionary[requestedGroupIds[index]];
          foreach (DirectoryObject result in (IEnumerable<GraphObject>) batchResponseItem.ResultSet.Results)
          {
            if (result is User user && !stringSet.Contains(user.ObjectId))
            {
              transitiveGroupMembers.Add(user);
              stringSet.Add(user.ObjectId);
              this.AddGroupMember(group1, (GraphObject) user);
            }
            if (result is Group group2 && !groupsExhausted.Contains(group2.ObjectId))
            {
              this.AddGroupMember(group1, (GraphObject) group2);
              if (!pendingGroupsAndPageTokens.ContainsKey(group2.ObjectId))
                pendingGroupsAndPageTokens[group2.ObjectId] = (string) null;
              dictionary[group2.ObjectId] = group2;
            }
          }
          ++index;
        }
      }
      return (IList<User>) transitiveGroupMembers;
    }

    public virtual void EnableDirectoryFeature(DirectoryFeature feature) => this.ClientConnection.UploadString(Utils.GetRequestUri(this, (Type) null, string.Empty, "enableDirectoryFeature"), "POST", SerializationHelper.SerializeToJson((object) new Dictionary<string, DirectoryFeature>()
    {
      ["directoryFeature"] = feature
    }), (WebHeaderCollection) null, (WebHeaderCollection) null);

    public virtual bool IsDirectoryFeatureEnabled(DirectoryFeature feature)
    {
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, string.Empty, "isDirectoryFeatureEnabled");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, DirectoryFeature>()
      {
        ["directoryFeature"] = feature
      });
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults.MixedResults.Count != 1)
        throw new GraphException("An error was encountered while checking for directory features: The response did not contain exactly one object. ");
      return bool.Parse(pagedResults.MixedResults[0]);
    }

    public virtual bool BackfillApplication(string appId, bool availableToOtherTenants)
    {
      if (string.IsNullOrEmpty(appId))
        throw new ArgumentNullException(nameof (appId));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, (string) null, "backfillApplication");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, object>()
      {
        [nameof (appId)] = (object) appId,
        [nameof (availableToOtherTenants)] = (object) availableToOtherTenants
      });
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults.MixedResults.Count > 0)
        return bool.Parse(pagedResults.MixedResults[0]);
      throw new GraphException("An error was encountered while backfill application object: The response did not contain exactly one object. ");
    }

    public virtual bool LinkServicePrincipalToApplication(string appId)
    {
      if (string.IsNullOrEmpty(appId))
        throw new ArgumentNullException(nameof (appId));
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, (string) null, "linkServicePrincipalToApplication");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, string>()
      {
        [nameof (appId)] = appId
      });
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults.MixedResults.Count > 0)
        return bool.Parse(pagedResults.MixedResults[0]);
      throw new GraphException("An error was encountered while linking service principal to application object: The response did not contain exactly one object. ");
    }

    private void ValidatePropertiesForInvite(User user)
    {
      if (string.IsNullOrEmpty(user.DisplayName))
        throw new PropertyValidationException("Missing DisplayName property.");
      if (string.IsNullOrEmpty(user.PrimarySMTPAddress))
        throw new PropertyValidationException("Missing PrimarySMTPAddress property.");
    }

    private System.Collections.Generic.List<BatchRequestItem> GetBatchRequests(
      Dictionary<string, string> pendingGroupsAndPageTokens,
      out System.Collections.Generic.List<string> requestedGroupIds)
    {
      System.Collections.Generic.List<BatchRequestItem> batchRequests = new System.Collections.Generic.List<BatchRequestItem>();
      requestedGroupIds = new System.Collections.Generic.List<string>();
      int num = 0;
      foreach (string key in pendingGroupsAndPageTokens.Keys)
      {
        string groupsAndPageToken = pendingGroupsAndPageTokens[key];
        Uri requestUri;
        if (string.IsNullOrEmpty(groupsAndPageToken))
          requestUri = Utils.GetRequestUri<Group>(this, key, "members");
        else
          requestUri = Utils.GetRequestUri<Group>(this, key, groupsAndPageToken, -1, "members");
        requestedGroupIds.Add(key);
        if (num < 5)
        {
          BatchRequestItem batchRequestItem = new BatchRequestItem("GET", false, requestUri, (WebHeaderCollection) null, string.Empty);
          batchRequests.Add(batchRequestItem);
          ++num;
        }
        else
          break;
      }
      return batchRequests;
    }

    private void AddGroupMember(Group group, GraphObject graphObject)
    {
      Utils.ValidateGraphObject((GraphObject) group, nameof (group));
      group.Members.Add(graphObject);
    }

    [GraphMethod(true)]
    public virtual IList<string> GetMemberGroups(
      DirectoryObject userOrGroup,
      bool securityEnabledOnly)
    {
      Utils.ValidateGraphObject((GraphObject) userOrGroup, "user");
      System.Collections.Generic.List<string> memberGroups = new System.Collections.Generic.List<string>();
      Uri requestUri;
      switch (userOrGroup)
      {
        case User _:
          requestUri = Utils.GetRequestUri<User>(this, Utils.GetUniqueKeyValue((GraphObject) userOrGroup), "getMemberGroups");
          break;
        case Group _:
          if (securityEnabledOnly)
            throw new ArgumentException("GetMemberGroups - securityEnabledOnly is available only for User.", nameof (userOrGroup));
          requestUri = Utils.GetRequestUri<Group>(this, Utils.GetUniqueKeyValue((GraphObject) userOrGroup), "getMemberGroups");
          break;
        default:
          throw new ArgumentException("GetMemberGroups is available only for User or Group.", nameof (userOrGroup));
      }
      Logger.Instance.Info("POSTing to {0}", (object) requestUri);
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, string>()
      {
        [nameof (securityEnabledOnly)] = securityEnabledOnly.ToString()
      });
      string httpMethod = "POST";
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem(httpMethod, true, requestUri, (WebHeaderCollection) null, json));
        return (IList<string>) null;
      }
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      memberGroups.AddRange((IEnumerable<string>) pagedResults.MixedResults);
      return (IList<string>) memberGroups;
    }

    [GraphMethod(true)]
    public virtual IList<string> CheckMemberGroups(GraphObject graphObject, IList<string> groupIds)
    {
      Utils.ValidateGraphObject(graphObject, nameof (graphObject));
      Utils.ThrowIfNullOrEmpty((object) groupIds, nameof (groupIds));
      System.Collections.Generic.List<string> stringList = new System.Collections.Generic.List<string>();
      Uri requestUri = Utils.GetRequestUri<DirectoryObject>(this, Utils.GetUniqueKeyValue(graphObject), "checkMemberGroups");
      Logger.Instance.Info("POSTing to {0}", (object) requestUri);
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, IList<string>>()
      {
        [nameof (groupIds)] = groupIds
      });
      string httpMethod = "POST";
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem(httpMethod, true, requestUri, (WebHeaderCollection) null, json));
        return (IList<string>) null;
      }
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      stringList.AddRange((IEnumerable<string>) pagedResults.MixedResults);
      return (IList<string>) stringList;
    }

    public virtual Application Restore(Application application, IList<string> identifierUris)
    {
      Utils.ValidateGraphObject((GraphObject) application, nameof (application));
      if (identifierUris == null)
        identifierUris = (IList<string>) new System.Collections.Generic.List<string>();
      Uri requestUri = Utils.GetRequestUri<Application>(this, application.ObjectId, "restore");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, IList<string>>()
      {
        [nameof (identifierUris)] = identifierUris
      });
      PagedResults<Application> pagedResults = SerializationHelper.DeserializeJsonResponse<Application>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      return pagedResults != null && pagedResults.Results.Count > 0 ? pagedResults.Results[0] : (Application) null;
    }

    public virtual User AssignLicense(
      User user,
      IList<AssignedLicense> addLicenses,
      IList<Guid> removeLicenses)
    {
      Utils.ValidateGraphObject((GraphObject) user, nameof (user));
      if (addLicenses == null)
        throw new ArgumentNullException(nameof (addLicenses));
      if (removeLicenses == null)
        throw new ArgumentNullException(nameof (removeLicenses));
      Uri requestUri = Utils.GetRequestUri<User>(this, Utils.GetUniqueKeyValue((GraphObject) user), "assignLicense");
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, object>()
      {
        [nameof (addLicenses)] = (object) addLicenses,
        [nameof (removeLicenses)] = (object) removeLicenses
      });
      PagedResults<User> pagedResults = SerializationHelper.DeserializeJsonResponse<User>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      return pagedResults != null && pagedResults.Results.Count > 0 ? pagedResults.Results[0] : (User) null;
    }

    public virtual bool IsMemberOf(string groupId, string memberId)
    {
      if (string.IsNullOrEmpty(groupId))
        throw new ArgumentNullException(nameof (groupId));
      if (string.IsNullOrEmpty(memberId))
        throw new ArgumentNullException(nameof (memberId));
      bool result = false;
      Uri requestUri = Utils.GetRequestUri(this, (Type) null, (string) null, "isMemberOf");
      Logger.Instance.Info("POSTing to {0}", (object) requestUri);
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, string>()
      {
        [nameof (groupId)] = groupId,
        [nameof (memberId)] = memberId
      });
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, "POST", json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults.MixedResults.Count > 0)
        bool.TryParse(pagedResults.MixedResults[0].ToString(), out result);
      return result;
    }

    protected GraphConnection()
    {
    }

    protected GraphConnection(ConnectionWrapper connectionWrapper, string accessToken)
    {
      this.ClientConnection = connectionWrapper;
      this.AccessToken = accessToken;
    }

    public GraphConnection(string accessToken)
      : this(accessToken, new Guid(), (GraphSettings) null)
    {
    }

    public GraphConnection(string accessToken, Guid clientRequestId)
      : this(accessToken, clientRequestId, (GraphSettings) null)
    {
    }

    public GraphConnection(string accessToken, GraphSettings graphSettings)
      : this(accessToken, Guid.NewGuid(), graphSettings)
    {
    }

    public GraphConnection(string accessToken, Guid clientRequestId, GraphSettings graphSettings)
    {
      graphSettings = graphSettings ?? new GraphSettings();
      this.ClientConnection = new ConnectionWrapper(graphSettings);
      this.AccessToken = accessToken;
      this.ClientRequestId = clientRequestId;
    }

    public GraphConnection(
      ICredentials credentials,
      Guid clientRequestId,
      GraphSettings graphSettings)
    {
      graphSettings = graphSettings ?? new GraphSettings();
      this.ClientConnection = new ConnectionWrapper(graphSettings);
      this.GraphCredentials = credentials;
      this.ClientRequestId = clientRequestId;
    }

    public string GraphApiDomainName => this.ClientConnection.GraphApiDomainName;

    public string GraphApiVersion => this.ClientConnection.GraphApiVersion;

    public ICredentials GraphCredentials
    {
      get => this.ClientConnection.GraphCredentials;
      set => this.ClientConnection.GraphCredentials = value;
    }

    public bool IsRetryEnabled => this.ClientConnection.IsRetryEnabled;

    public HashSet<string> RetryOnExceptions => this.ClientConnection.RetryOnExceptions;

    public TimeSpan WaitBeforeRetry => this.ClientConnection.WaitBeforeRetry;

    public int TotalAttempts => this.ClientConnection.TotalAttempts;

    public ConnectionWrapper ClientConnection { get; private set; }

    public Guid ClientRequestId
    {
      get => this.ClientConnection.ClientRequestId;
      set => this.ClientConnection.ClientRequestId = value;
    }

    public string TenantId
    {
      get => this.tenantId;
      set
      {
        this.overrideTenantId = true;
        this.tenantId = value;
      }
    }

    public string AccessToken
    {
      get => this.ClientConnection.AccessToken;
      set
      {
        this.ClientConnection.AccessToken = value;
        if (this.overrideTenantId)
          return;
        this.tenantId = Utils.GetTenantId(value);
      }
    }

    public string AadGraphEndpoint => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://{0}/{1}", new object[2]
    {
      (object) this.GraphApiDomainName,
      (object) this.TenantId
    });

    [GraphMethod(true)]
    public virtual PagedResults<GraphObject> List(
      Type objectType,
      string pageToken,
      FilterGenerator filter)
    {
      Uri listUri;
      return SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ListCore(objectType, pageToken, filter, out listUri), listUri);
    }

    [GraphMethod(true)]
    public virtual PagedResults<T> List<T>(string pageToken, FilterGenerator filter) where T : GraphObject
    {
      Uri listUri;
      return SerializationHelper.DeserializeJsonResponse<T>(this.ListCore(typeof (T), pageToken, filter, out listUri), listUri);
    }

    public virtual TenantDetail GetTenantDetails()
    {
      PagedResults<TenantDetail> pagedResults = this.List<TenantDetail>((string) null, new FilterGenerator());
      if (pagedResults.Results == null || pagedResults.Results.Count != 1)
        throw new ObjectNotFoundException(HttpStatusCode.NotFound, "Tenant details not found.");
      return pagedResults.Results[0];
    }

    [GraphMethod(true)]
    public virtual T Get<T>(string objectId) where T : GraphObject => this.Get(typeof (T), objectId) as T;

    [GraphMethod(true)]
    public virtual GraphObject Get(Type objectType, string objectId) => this.Get(objectType, objectId, LinkProperty.None);

    [GraphMethod(true)]
    public virtual GraphObject Get(Type objectType, string objectId, LinkProperty expandProperty)
    {
      Uri requestUri;
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.GetCore(objectType, objectId, new FilterGenerator()
      {
        ExpandProperty = expandProperty
      }, out requestUri), requestUri);
      return pagedResults != null ? pagedResults.Results.FirstOrDefault<GraphObject>() : (GraphObject) null;
    }

    [GraphMethod(true)]
    public virtual GraphObject Add(GraphObject entity) => this.AddOrUpdate(entity, true, (RequestOptions) null);

    [GraphMethod(true)]
    public virtual T Add<T>(T entity) where T : GraphObject => this.AddOrUpdate((GraphObject) entity, true, (RequestOptions) null) as T;

    [GraphMethod(true)]
    public virtual GraphObject Add(GraphObject entity, RequestOptions requestOptions) => this.AddOrUpdate(entity, true, requestOptions);

    [GraphMethod(true)]
    public virtual T Add<T>(T entity, RequestOptions requestOptions) where T : GraphObject => this.AddOrUpdate((GraphObject) entity, true, requestOptions) as T;

    [GraphMethod(true)]
    public virtual GraphObject Update(GraphObject entity) => this.AddOrUpdate(entity, false, (RequestOptions) null);

    [GraphMethod(true)]
    public virtual T Update<T>(T entity) where T : GraphObject => this.AddOrUpdate((GraphObject) entity, false, (RequestOptions) null) as T;

    [GraphMethod(true)]
    public virtual void Delete(GraphObject graphObject)
    {
      Utils.ValidateGraphObject(graphObject, nameof (graphObject));
      Uri requestUri = Utils.GetRequestUri(this, graphObject.GetType(), Utils.GetUniqueKeyValue(graphObject));
      if (this.returnBatchItem.Value)
        this.batchRequestItems.Value.Add(new BatchRequestItem("DELETE", true, requestUri, (WebHeaderCollection) null, (string) null));
      else
        this.ClientConnection.DeleteRequest(requestUri);
    }

    [GraphMethod(true)]
    public GraphObject AddContainment(GraphObject parent, GraphObject containment) => this.AddOrUpdateContainment(parent, containment, true);

    [GraphMethod(true)]
    public T AddContainment<T>(GraphObject parent, T containment) where T : GraphObject => this.AddOrUpdateContainment(parent, (GraphObject) containment, true) as T;

    [GraphMethod(true)]
    public T UpdateContainment<T>(GraphObject parent, T containment) where T : GraphObject => this.AddOrUpdateContainment(parent, (GraphObject) containment, false) as T;

    [GraphMethod(true)]
    public virtual PagedResults<GraphObject> ListContainments(
      GraphObject parent,
      Type containmentType,
      string linkToNextPage,
      FilterGenerator filter)
    {
      Uri listUri;
      return SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ListContainmentsCore(parent, containmentType, linkToNextPage, filter, out listUri), listUri);
    }

    [GraphMethod(true)]
    public virtual PagedResults<T> ListContainments<T>(
      GraphObject parent,
      string linkToNextPage,
      FilterGenerator filter)
      where T : GraphObject
    {
      Uri listUri;
      return SerializationHelper.DeserializeJsonResponse<T>(this.ListContainmentsCore(parent, typeof (T), linkToNextPage, filter, out listUri), listUri);
    }

    [GraphMethod(true)]
    public virtual T GetContainment<T>(GraphObject parent, string containmentObjectId) where T : GraphObject => this.GetContainment(parent, typeof (T), containmentObjectId) as T;

    [GraphMethod(true)]
    public virtual GraphObject GetContainment(
      GraphObject parent,
      Type containmentType,
      string containmentObjectId)
    {
      Uri requestUri = Utils.GetRequestUri(this, parent.GetType(), Utils.GetUniqueKeyValue(parent), containmentType, containmentObjectId);
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem("GET", false, requestUri, (WebHeaderCollection) null, (string) null));
        return (GraphObject) null;
      }
      Logger.Instance.Info("Retrieving {0}", (object) requestUri);
      return SerializationHelper.DeserializeJsonResponse<GraphObject>(Encoding.UTF8.GetString(this.ClientConnection.DownloadData(requestUri, (WebHeaderCollection) null)), requestUri).Results.FirstOrDefault<GraphObject>();
    }

    [GraphMethod(true)]
    public virtual void DeleteContainment(GraphObject parent, GraphObject containment)
    {
      Utils.ThrowIfNullOrEmpty((object) parent, nameof (parent));
      Utils.ThrowIfNullOrEmpty((object) containment, nameof (containment));
      Uri requestUri = Utils.GetRequestUri(this, parent.GetType(), Utils.GetUniqueKeyValue(parent), containment.GetType(), Utils.GetUniqueKeyValue(containment));
      if (this.returnBatchItem.Value)
        this.batchRequestItems.Value.Add(new BatchRequestItem("DELETE", true, requestUri, (WebHeaderCollection) null, (string) null));
      else
        this.ClientConnection.DeleteRequest(requestUri);
    }

    [GraphMethod(true)]
    public virtual PagedResults<GraphObject> GetLinkedObjects(
      GraphObject graphObject,
      LinkProperty linkProperty,
      string nextPageToken)
    {
      return this.GetLinkedObjects(graphObject, linkProperty, nextPageToken, -1);
    }

    [GraphMethod(true)]
    public virtual PagedResults<GraphObject> GetLinkedObjects(
      GraphObject graphObject,
      LinkProperty linkProperty,
      string nextPageToken,
      int top)
    {
      Utils.ValidateGraphObject(graphObject, nameof (graphObject));
      Uri requestUri = Utils.GetRequestUri(this, graphObject.GetType(), Utils.GetUniqueKeyValue(graphObject), nextPageToken, top, (IList<QueryParameter>) null, Utils.GetLinkName(linkProperty));
      if (!this.returnBatchItem.Value)
        return SerializationHelper.DeserializeJsonResponse<GraphObject>(Encoding.UTF8.GetString(this.ClientConnection.DownloadData(requestUri, (WebHeaderCollection) null)), requestUri);
      this.batchRequestItems.Value.Add(new BatchRequestItem("GET", false, requestUri, (WebHeaderCollection) null, (string) null));
      return (PagedResults<GraphObject>) null;
    }

    public virtual IList<GraphObject> GetAllDirectLinks(
      GraphObject graphObject,
      LinkProperty linkProperty)
    {
      Utils.ValidateGraphObject(graphObject, nameof (graphObject));
      System.Collections.Generic.List<GraphObject> allDirectLinks = new System.Collections.Generic.List<GraphObject>();
      PagedResults<GraphObject> pagedResults = (PagedResults<GraphObject>) null;
      do
      {
        pagedResults = this.GetLinkedObjects(graphObject, linkProperty, pagedResults == null ? (string) null : pagedResults.PageToken, -1);
        allDirectLinks.AddRange((IEnumerable<GraphObject>) pagedResults.Results);
      }
      while (!pagedResults.IsLastPage);
      return (IList<GraphObject>) allDirectLinks;
    }

    [GraphMethod(true)]
    public virtual void AddLink(
      GraphObject sourceObject,
      GraphObject targetObject,
      LinkProperty linkProperty)
    {
      Utils.ValidateGraphObject(sourceObject, nameof (sourceObject));
      Utils.ValidateGraphObject(targetObject, nameof (targetObject));
      Uri requestUri = Utils.GetRequestUri(this, sourceObject.GetType(), Utils.GetUniqueKeyValue(sourceObject), "$links", Utils.GetLinkName(linkProperty));
      string json = SerializationHelper.SerializeToJson((object) new Dictionary<string, string>()
      {
        {
          "url",
          Utils.GetRequestUri(this, targetObject.GetType(), Utils.GetUniqueKeyValue(targetObject)).ToString()
        }
      });
      string str = Utils.GetLinkAttribute(sourceObject.GetType(), linkProperty).IsSingleValued ? "PUT" : "POST";
      if (this.returnBatchItem.Value)
        this.batchRequestItems.Value.Add(new BatchRequestItem(str, true, requestUri, (WebHeaderCollection) null, json));
      else
        this.ClientConnection.UploadString(requestUri, str, json, (WebHeaderCollection) null, (WebHeaderCollection) null);
    }

    [GraphMethod(true)]
    public virtual void DeleteLink(
      GraphObject sourceObject,
      GraphObject targetObject,
      LinkProperty linkProperty)
    {
      Utils.ValidateGraphObject(sourceObject, nameof (sourceObject));
      Uri requestUri;
      if (Utils.GetLinkAttribute(sourceObject.GetType(), linkProperty).IsSingleValued)
      {
        requestUri = Utils.GetRequestUri(this, sourceObject.GetType(), Utils.GetUniqueKeyValue(sourceObject), "$links", Utils.GetLinkName(linkProperty));
      }
      else
      {
        Utils.ValidateGraphObject(targetObject, nameof (targetObject));
        requestUri = Utils.GetRequestUri(this, sourceObject.GetType(), Utils.GetUniqueKeyValue(sourceObject), "$links", Utils.GetLinkName(linkProperty), Utils.GetUniqueKeyValue(targetObject));
      }
      if (this.returnBatchItem.Value)
        this.batchRequestItems.Value.Add(new BatchRequestItem("DELETE", true, requestUri, (WebHeaderCollection) null, (string) null));
      else
        this.ClientConnection.DeleteRequest(requestUri);
    }

    public virtual Stream GetStreamProperty(
      GraphObject graphObject,
      GraphProperty graphProperty,
      string acceptType)
    {
      Utils.ValidateGraphObject(graphObject, nameof (graphObject));
      Utils.ThrowIfNullOrEmpty((object) acceptType, nameof (acceptType));
      byte[] buffer = this.ClientConnection.DownloadData(Utils.GetRequestUri(this, graphObject.GetType(), Utils.GetUniqueKeyValue(graphObject), Utils.GetPropertyName(graphProperty)), new WebHeaderCollection()
      {
        [HttpRequestHeader.ContentType] = acceptType
      });
      return buffer != null ? (Stream) new MemoryStream(buffer) : (Stream) new MemoryStream();
    }

    public virtual void SetStreamProperty(
      GraphObject graphObject,
      GraphProperty graphProperty,
      MemoryStream memoryStream,
      string contentType)
    {
      Utils.ValidateGraphObject(graphObject, nameof (graphObject));
      Utils.ThrowIfNullOrEmpty((object) memoryStream, nameof (memoryStream));
      Utils.ThrowIfNullOrEmpty((object) contentType, nameof (contentType));
      this.ClientConnection.UploadData(Utils.GetRequestUri(this, graphObject.GetType(), Utils.GetUniqueKeyValue(graphObject), Utils.GetPropertyName(graphProperty)), "PUT", memoryStream.ToArray(), new WebHeaderCollection()
      {
        [HttpRequestHeader.ContentType] = contentType
      });
    }

    public virtual IList<BatchResponseItem> ExecuteBatch(params Expression<Action>[] batchRequests)
    {
      if (batchRequests == null || batchRequests.Length < 1)
        throw new ArgumentException("Invalid batch request. Should contain 1 to 5 items.");
      if (batchRequests.Length > 5)
        throw new ArgumentException("Invalid batch request. Should contain 1 to 5 items.");
      try
      {
        this.returnBatchItem.Value = true;
        foreach (Expression<Action> batchRequest in batchRequests)
        {
          MethodCallExpression body = (MethodCallExpression) batchRequest.Body;
          Utils.ThrowIfNull((object) body, "batchRequest");
          GraphMethodAttribute customAttribute = Utils.GetCustomAttribute<GraphMethodAttribute>(body.Method, false);
          if (customAttribute == null || !customAttribute.SupportsBatching)
            throw new InvalidOperationException(string.Format("Batching is not supported for {0}.", (object) body.Method.Name));
          batchRequest.Compile()();
        }
        if (this.batchRequestItems.Value.Count != batchRequests.Length)
          throw new InvalidOperationException("One or more requests does not support batching.");
        return this.ExecuteBatch(this.batchRequestItems.Value);
      }
      finally
      {
        this.returnBatchItem.Value = false;
        this.batchRequestItems.Value.Clear();
      }
    }

    public virtual IList<BatchResponseItem> ExecuteBatch(IList<BatchRequestItem> batchRequests)
    {
      if (batchRequests == null || batchRequests.Count < 1)
        throw new ArgumentException("Invalid batch request. Should contain 1 to 5 items.");
      if (batchRequests.Count > 5)
        throw new ArgumentException("Invalid batch request. Should contain 1 to 5 items.");
      string requestUri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/$batch?{1}={2}", new object[3]
      {
        (object) this.AadGraphEndpoint,
        (object) "api-version",
        (object) this.GraphApiVersion
      });
      StringBuilder stringBuilder = new StringBuilder();
      foreach (BatchRequestItem batchRequest in (IEnumerable<BatchRequestItem>) batchRequests)
      {
        if (batchRequest.IsChangesetRequired)
        {
          batchRequest.RequestId = this.ClientRequestId;
          Guid guid = Guid.NewGuid();
          batchRequest.ChangeSetId = guid;
          stringBuilder.Append(batchRequest.ToString());
          stringBuilder.AppendFormat("--changeset_{0}--", (object) guid.ToString());
          stringBuilder.AppendLine();
        }
        else
        {
          batchRequest.RequestId = this.ClientRequestId;
          stringBuilder.Append(batchRequest.ToString());
        }
      }
      stringBuilder.AppendFormat("--batch_{0}--", (object) this.ClientRequestId.ToString());
      WebHeaderCollection additionalHeaders = new WebHeaderCollection();
      additionalHeaders[HttpRequestHeader.ContentType] = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "multipart/mixed; boundary=batch_{0}", new object[1]
      {
        (object) this.ClientRequestId.ToString()
      });
      WebHeaderCollection responseHeaders = new WebHeaderCollection();
      string responseString = this.ClientConnection.UploadString(requestUri, "POST", stringBuilder.ToString(), additionalHeaders, responseHeaders);
      return (IList<BatchResponseItem>) SerializationHelper.DeserializeBatchResponse(this.ClientConnection.GetContentTypeOfLastResponse(responseHeaders), responseString, batchRequests);
    }

    private GraphObject AddOrUpdateContainment(
      GraphObject parent,
      GraphObject containment,
      bool isCreate)
    {
      Utils.ThrowIfNullOrEmpty((object) parent, nameof (parent));
      Utils.ThrowIfNullOrEmpty((object) containment, nameof (containment));
      containment.ValidateProperties(isCreate);
      Uri requestUri = Utils.GetRequestUri(this, parent.GetType(), Utils.GetUniqueKeyValue(parent), containment.GetType(), isCreate ? string.Empty : Utils.GetUniqueKeyValue(containment));
      string json = SerializationHelper.SerializeToJson((object) Utils.GetSerializableGraphObject(containment));
      string str = isCreate ? "POST" : "PATCH";
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem(str, true, requestUri, (WebHeaderCollection) null, json));
        return (GraphObject) null;
      }
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, str, json, (WebHeaderCollection) null, (WebHeaderCollection) null), requestUri);
      if (pagedResults == null || pagedResults.Results == null || pagedResults.Results.Count != 1)
        throw new InvalidOperationException("Unable to deserialize the response");
      containment.ChangedProperties.Clear();
      pagedResults.Results[0].ChangedProperties.Clear();
      return pagedResults.Results[0];
    }

    private GraphObject AddOrUpdate(
      GraphObject entity,
      bool isCreate,
      RequestOptions requestOptions)
    {
      Utils.ThrowIfNullOrEmpty((object) entity, nameof (entity));
      entity.ValidateProperties(isCreate);
      Uri requestUri = Utils.GetRequestUri(this, entity.GetType(), isCreate ? string.Empty : Utils.GetUniqueKeyValue(entity), requestOptions == null ? (IList<QueryParameter>) null : requestOptions.queryParameters);
      string json = SerializationHelper.SerializeToJson((object) Utils.GetSerializableGraphObject(entity));
      string str = isCreate ? "POST" : "PATCH";
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem(str, true, requestUri, (WebHeaderCollection) null, json));
        return (GraphObject) null;
      }
      PagedResults<GraphObject> pagedResults = SerializationHelper.DeserializeJsonResponse<GraphObject>(this.ClientConnection.UploadString(requestUri, str, json, (WebHeaderCollection) null, requestOptions == null ? (WebHeaderCollection) null : requestOptions.responseHeaders), requestUri);
      if (pagedResults == null || pagedResults.Results == null || pagedResults.Results.Count != 1)
        throw new InvalidOperationException("Unable to deserialize the response");
      entity.ChangedProperties.Clear();
      pagedResults.Results[0].ChangedProperties.Clear();
      return pagedResults.Results[0];
    }

    private string ListCore(
      Type objectType,
      string linkToNextPage,
      FilterGenerator filter,
      out Uri listUri)
    {
      listUri = Utils.GetListUri(objectType, this, linkToNextPage, filter);
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem("GET", false, listUri, (WebHeaderCollection) null, (string) null));
        return (string) null;
      }
      Logger.Instance.Info("Retrieving {0}", (object) listUri);
      byte[] bytes = this.ClientConnection.DownloadData(listUri, (WebHeaderCollection) null);
      return bytes == null ? (string) null : Encoding.UTF8.GetString(bytes);
    }

    private string GetCore(
      Type objectType,
      string objectId,
      FilterGenerator filterGenerator,
      out Uri requestUri)
    {
      requestUri = Utils.GetRequestUri(this, objectType, objectId);
      if (filterGenerator != null && filterGenerator.ExpandProperty != LinkProperty.None)
      {
        if (filterGenerator.QueryFilter != null)
          throw new ArgumentException("Filter expressions are not allowed when querying a single object.");
        if (filterGenerator.OrderByProperty != GraphProperty.None)
          throw new ArgumentException("OrderBy is not allowed when querying a single object.");
        UriBuilder uriBuilder = new UriBuilder(requestUri);
        Utils.BuildQueryFromFilter(uriBuilder, filterGenerator);
        requestUri = uriBuilder.Uri;
      }
      Logger.Instance.Info("Retrieving {0}", (object) requestUri);
      if (!this.returnBatchItem.Value)
        return Encoding.UTF8.GetString(this.ClientConnection.DownloadData(requestUri, (WebHeaderCollection) null));
      this.batchRequestItems.Value.Add(new BatchRequestItem("GET", false, requestUri, (WebHeaderCollection) null, (string) null));
      return (string) null;
    }

    private string ListContainmentsCore(
      GraphObject parent,
      Type containmentType,
      string linkToNextPage,
      FilterGenerator filter,
      out Uri listUri)
    {
      listUri = Utils.GetListUri(parent, containmentType, this, linkToNextPage, filter);
      if (this.returnBatchItem.Value)
      {
        this.batchRequestItems.Value.Add(new BatchRequestItem("GET", false, listUri, (WebHeaderCollection) null, (string) null));
        return (string) null;
      }
      Logger.Instance.Info("Retrieving {0}", (object) listUri);
      byte[] bytes = this.ClientConnection.DownloadData(listUri, (WebHeaderCollection) null);
      return bytes == null ? (string) null : Encoding.UTF8.GetString(bytes);
    }
  }
}
