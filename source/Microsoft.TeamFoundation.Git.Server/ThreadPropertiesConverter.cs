// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ThreadPropertiesConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class ThreadPropertiesConverter
  {
    public static PropertiesCollection GetCRThreadProperties(
      IVssRequestContext rc,
      GitPullRequestCommentThread prCommentThread)
    {
      if (prCommentThread.Properties == null)
        return (PropertiesCollection) null;
      PropertiesCollection threadProperties = new PropertiesCollection((IDictionary<string, object>) prCommentThread.Properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (prop => !PullRequestDiscussionConstants.IdentityToIdKeyMap.Keys.Contains<string>(prop.Key))).ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value)));
      foreach (KeyValuePair<string, object> keyValuePair in prCommentThread.Properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (prop => PullRequestDiscussionConstants.IdentityToIdKeyMap.Keys.Contains<string>(prop.Key))))
      {
        string identityToIdKey = PullRequestDiscussionConstants.IdentityToIdKeyMap[keyValuePair.Key];
        if (PullRequestDiscussionConstants.MultiTfIdKeys.Contains(identityToIdKey))
        {
          string[] strArray = JsonConvert.DeserializeObject<string[]>(keyValuePair.Value.ToString());
          Guid[] guidArray = new Guid[strArray.Length];
          for (int index = 0; index < strArray.Length; ++index)
          {
            IdentityRef identityRef;
            if (prCommentThread.Identities.TryGetValue(strArray[index], out identityRef))
              guidArray[index] = Guid.Parse(identityRef.Id);
            else
              throw new ArgumentException("Thread property '" + keyValuePair.Key + "' references identity with non-existent key '" + strArray[index] + "'.");
          }
          threadProperties.Add(identityToIdKey, (object) JsonConvert.SerializeObject((object) guidArray));
        }
        else
        {
          IdentityRef identityRef;
          if (!prCommentThread.Identities.TryGetValue(keyValuePair.Value.ToString(), out identityRef))
            throw new ArgumentException(string.Format("Thread property '{0}' references identity with non-existent key '{1}'.", (object) keyValuePair.Key, keyValuePair.Value));
          threadProperties.Add(identityToIdKey, (object) identityRef.Id);
        }
      }
      return threadProperties;
    }

    private static Guid[] ParseMultiIds(string value)
    {
      if (value.Length != 36 && value.Length != 32)
        return JsonConvert.DeserializeObject<Guid[]>(value);
      return new Guid[1]{ Guid.Parse(value) };
    }

    public static PropertiesCollection GetPRThreadProperties(
      IVssRequestContext rc,
      DiscussionThread discussionThread,
      IDictionary<Guid, IdentityRef> knownIdentities,
      ISecuredObject securedObject,
      out Dictionary<string, IdentityRef> identities)
    {
      PropertiesCollection properties = discussionThread.Properties;
      if (properties == null)
      {
        identities = (Dictionary<string, IdentityRef>) null;
        return (PropertiesCollection) null;
      }
      PropertiesCollection targetProperties = new PropertiesCollection((IDictionary<string, object>) properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (prop => !PullRequestDiscussionConstants.PropertiesToRemoveFromPR.Contains(prop.Key) && !PullRequestDiscussionConstants.IdentityKeys.Contains(prop.Key))).ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, object>, object>) (kvp => kvp.Value)));
      identities = ThreadPropertiesConverter.UpdateIdentityMaps(rc, properties, targetProperties, knownIdentities, securedObject);
      return targetProperties;
    }

    private static Dictionary<string, IdentityRef> UpdateIdentityMaps(
      IVssRequestContext rc,
      PropertiesCollection sourceProperties,
      PropertiesCollection targetProperties,
      IDictionary<Guid, IdentityRef> knownIdentities,
      ISecuredObject securedObject)
    {
      Dictionary<string, Guid[]> multiIdMaps;
      HashSet<Guid> identityIds = ThreadPropertiesConverter.GetIdentityIds(rc, sourceProperties, out multiIdMaps);
      Dictionary<string, IdentityRef> dictionary = (Dictionary<string, IdentityRef>) null;
      if (identityIds.Any<Guid>())
      {
        Dictionary<Guid, string> idToKeyMap = new Dictionary<Guid, string>();
        dictionary = (Dictionary<string, IdentityRef>) new SecuredDictionary<string, IdentityRef>(securedObject);
        int num = 1;
        foreach (IdentityRef identityRef in ThreadPropertiesConverter.GetIdentityRefs(rc, identityIds, knownIdentities))
        {
          dictionary.Add(num.ToString(), identityRef);
          idToKeyMap.Add(Guid.Parse(identityRef.Id), num.ToString());
          ++num;
        }
        foreach (KeyValuePair<string, object> keyValuePair in sourceProperties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => PullRequestDiscussionConstants.SingleTfIdKeys.Contains(kvp.Key))))
        {
          Guid tfId = Guid.Parse(keyValuePair.Value.ToString());
          string uniqueKey;
          if (ThreadPropertiesConverter.TryGetIdentityKey(rc, idToKeyMap, tfId, keyValuePair.Key, out uniqueKey))
            targetProperties.Add(PullRequestDiscussionConstants.IdToIdentityKeyMap[keyValuePair.Key], (object) uniqueKey);
        }
        foreach (KeyValuePair<string, object> keyValuePair in sourceProperties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => PullRequestDiscussionConstants.MultiTfIdKeys.Contains(kvp.Key))))
        {
          HashSet<Guid> guidSet = new HashSet<Guid>((IEnumerable<Guid>) multiIdMaps[keyValuePair.Key]);
          List<string> stringList = new List<string>();
          foreach (Guid tfId in guidSet)
          {
            string uniqueKey;
            if (ThreadPropertiesConverter.TryGetIdentityKey(rc, idToKeyMap, tfId, keyValuePair.Key, out uniqueKey))
              stringList.Add(uniqueKey);
          }
          targetProperties.Add(PullRequestDiscussionConstants.IdToIdentityKeyMap[keyValuePair.Key], (object) JsonConvert.SerializeObject((object) stringList.ToArray()));
        }
      }
      return dictionary;
    }

    private static bool TryGetIdentityKey(
      IVssRequestContext rc,
      Dictionary<Guid, string> idToKeyMap,
      Guid tfId,
      string propertyName,
      out string uniqueKey)
    {
      if (!idToKeyMap.TryGetValue(tfId, out uniqueKey))
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = ThreadPropertiesConverter.FindIdentity(rc, tfId);
        if (identity == null || !idToKeyMap.TryGetValue(identity.Id, out uniqueKey))
        {
          rc.Trace(1013831, TraceLevel.Error, GitServerUtils.TraceArea, nameof (ThreadPropertiesConverter), "Missing identity referenced in thread properties. Key: {0}, Identity: {1}", (object) propertyName, (object) tfId);
          return false;
        }
      }
      return true;
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity FindIdentity(
      IVssRequestContext rc,
      Guid tfId)
    {
      return Microsoft.TeamFoundation.Framework.Server.IdentityHelper.GetIdentityFromList(rc.GetService<IdentityService>().ReadIdentities(rc, (IList<Guid>) new Guid[1]
      {
        tfId
      }, QueryMembership.None, (IEnumerable<string>) null), tfId.ToString());
    }

    private static HashSet<Guid> GetIdentityIds(
      IVssRequestContext rc,
      PropertiesCollection properties,
      out Dictionary<string, Guid[]> multiIdMaps)
    {
      IEnumerable<string> strings = properties.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (kvp => PullRequestDiscussionConstants.SingleTfIdKeys.Contains(kvp.Key))).Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (kvp => kvp.Value.ToString()));
      multiIdMaps = new Dictionary<string, Guid[]>();
      foreach (string multiTfIdKey in PullRequestDiscussionConstants.MultiTfIdKeys)
      {
        object obj;
        if (properties.TryGetValue(multiTfIdKey, out obj))
        {
          Guid[] multiIds = ThreadPropertiesConverter.ParseMultiIds(obj.ToString());
          multiIdMaps.Add(multiTfIdKey, multiIds);
        }
      }
      HashSet<Guid> collection = new HashSet<Guid>();
      foreach (string input in strings)
      {
        Guid result;
        if (Guid.TryParse(input, out result))
          collection.Add(result);
        else
          rc.Trace(1013840, TraceLevel.Error, GitServerUtils.TraceArea, nameof (ThreadPropertiesConverter), "Invalid Guid specified for Id: {0}", (object) input);
      }
      collection.AddRange<Guid, HashSet<Guid>>(multiIdMaps.SelectMany<KeyValuePair<string, Guid[]>, Guid>((Func<KeyValuePair<string, Guid[]>, IEnumerable<Guid>>) (kvp => (IEnumerable<Guid>) kvp.Value)));
      return collection;
    }

    private static IEnumerable<IdentityRef> GetIdentityRefs(
      IVssRequestContext rc,
      HashSet<Guid> ids,
      IDictionary<Guid, IdentityRef> knownIdentities)
    {
      IList<Guid> list = (IList<Guid>) ids.Except<Guid>((IEnumerable<Guid>) knownIdentities.Keys).ToList<Guid>();
      List<IdentityRef> identityRefs = new List<IdentityRef>(ids.Except<Guid>((IEnumerable<Guid>) list).ToList<Guid>().Select<Guid, IdentityRef>((Func<Guid, IdentityRef>) (id => knownIdentities[id])));
      if (list.Any<Guid>())
      {
        foreach (IdentityRef identityRef in rc.GetService<IdentityService>().ReadIdentities(rc, list, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) (x => x.ToIdentityRef(rc))))
        {
          identityRefs.Add(identityRef);
          if (!knownIdentities.TryAdd<Guid, IdentityRef>(Guid.Parse(identityRef.Id), identityRef))
            rc.Trace(1013837, TraceLevel.Error, GitServerUtils.TraceArea, nameof (ThreadPropertiesConverter), "Attempted to insert duplicate identity in cache. Identity: {0}", (object) identityRef.Id);
        }
      }
      return (IEnumerable<IdentityRef>) identityRefs;
    }
  }
}
