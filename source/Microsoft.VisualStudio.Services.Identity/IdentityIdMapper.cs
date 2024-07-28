// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdMapper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityIdMapper : IIdMapper
  {
    private readonly bool m_hostIsMaster;
    private readonly ConcurrentDictionary<Guid, Guid> m_mapFromLocalId;
    private readonly ConcurrentDictionary<Guid, Guid> m_mapToLocalId;
    private const string s_area = "IdentityService";
    private const string s_layer = "IdentityIdMapper";

    internal IdentityIdMapper(
      IVssRequestContext requestContext,
      bool hostIsMaster,
      ConcurrentDictionary<Guid, Guid> mapFromLocalId = null,
      ConcurrentDictionary<Guid, Guid> mapToLocalId = null)
    {
      this.m_hostIsMaster = hostIsMaster;
      if (hostIsMaster)
        return;
      this.m_mapFromLocalId = mapFromLocalId ?? new ConcurrentDictionary<Guid, Guid>();
      this.m_mapToLocalId = mapToLocalId ?? new ConcurrentDictionary<Guid, Guid>();
    }

    public Guid[] MapFromLocalIds(IVssRequestContext requestContext, Guid[] localIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Guid[]>(localIds, nameof (localIds));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || this.m_hostIsMaster || requestContext.ExecutionEnvironment.IsHostedDeployment)
        return localIds;
      int length = localIds.Length;
      Guid[] guidArray = new Guid[length];
      List<Guid> source1 = (List<Guid>) null;
      for (int index = 0; index < length; ++index)
      {
        Guid localId = localIds[index];
        Guid guid;
        if (this.m_mapFromLocalId.TryGetValue(localId, out guid))
          guidArray[index] = guid;
        else if (localId != Guid.Empty)
        {
          if (source1 == null)
            source1 = new List<Guid>();
          source1.Add(localId);
        }
      }
      List<Guid> list = source1 != null ? source1.Distinct<Guid>().ToList<Guid>() : (List<Guid>) null;
      if (list != null)
      {
        List<IdentityIdMapping> source2;
        using (IdentityMapComponent component = requestContext.CreateComponent<IdentityMapComponent>())
        {
          if (component is IdentityMapComponent5 identityMapComponent5)
          {
            source2 = identityMapComponent5.QueryIdentityMappings((ICollection<Guid>) list);
          }
          else
          {
            source2 = new List<IdentityIdMapping>(list.Count);
            foreach (Guid localId in list)
            {
              Guid guid = component.ReadMapping(localId);
              source2.Add(new IdentityIdMapping()
              {
                LocalId = localId,
                MasterId = guid
              });
            }
          }
        }
        Dictionary<Guid, Guid> dictionary = source2.ToDictionary<IdentityIdMapping, Guid, Guid>((Func<IdentityIdMapping, Guid>) (m => m.LocalId), (Func<IdentityIdMapping, Guid>) (m => m.MasterId));
        for (int index = 0; index < length; ++index)
        {
          if (guidArray[index] == Guid.Empty)
          {
            Guid localId = localIds[index];
            Guid masterId;
            if (localId != Guid.Empty && dictionary.TryGetValue(localId, out masterId) && masterId != Guid.Empty)
            {
              guidArray[index] = masterId;
              this.CacheMapping(masterId, localId);
            }
          }
        }
      }
      return guidArray;
    }

    public Guid[] MapToLocalIds(IVssRequestContext requestContext, Guid[] masterIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Guid[]>(masterIds, nameof (masterIds));
      if (this.m_hostIsMaster || requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return masterIds;
      int length = masterIds.Length;
      Guid[] localIds = new Guid[length];
      List<Guid> source1 = (List<Guid>) null;
      for (int index = 0; index < length; ++index)
      {
        Guid masterId = masterIds[index];
        Guid guid;
        if (this.m_mapToLocalId.TryGetValue(masterId, out guid))
          localIds[index] = guid;
        else if (masterId != Guid.Empty)
        {
          if (source1 == null)
            source1 = new List<Guid>();
          source1.Add(masterId);
        }
      }
      if (source1 != null)
      {
        List<Guid> guidList = source1;
        List<Guid> list = source1.Distinct<Guid>().ToList<Guid>();
        if (list.Count != guidList.Count)
        {
          string str = JsonConvert.SerializeObject((object) guidList.ToArray());
          requestContext.TraceException(80501, "IdentityService", nameof (IdentityIdMapper), (Exception) new ArgumentException("Duplicate masterIds in MapToLocalIds: " + str));
        }
        List<IdentityIdMapping> source2;
        using (IdentityMapComponent component = requestContext.CreateComponent<IdentityMapComponent>())
        {
          if (component is IdentityMapComponent5 identityMapComponent5)
          {
            source2 = identityMapComponent5.MapIdentities((ICollection<Guid>) list);
          }
          else
          {
            source2 = new List<IdentityIdMapping>(list.Count);
            foreach (Guid masterId in list)
            {
              Guid guid = component.MapIdentity(masterId);
              source2.Add(new IdentityIdMapping()
              {
                LocalId = guid,
                MasterId = masterId
              });
            }
          }
        }
        Dictionary<Guid, Guid> dictionary = source2.ToDictionary<IdentityIdMapping, Guid, Guid>((Func<IdentityIdMapping, Guid>) (m => m.MasterId), (Func<IdentityIdMapping, Guid>) (m => m.LocalId));
        for (int index = 0; index < length; ++index)
        {
          if (localIds[index] == Guid.Empty)
          {
            Guid masterId = masterIds[index];
            if (masterId != Guid.Empty)
            {
              Guid localId = dictionary[masterId];
              localIds[index] = localId;
              this.CacheMapping(masterId, localId);
            }
          }
        }
      }
      return localIds;
    }

    private void CacheMapping(Guid masterId, Guid localId)
    {
      this.m_mapFromLocalId[localId] = masterId;
      this.m_mapToLocalId[masterId] = localId;
    }

    public void ClearCaches()
    {
      if (this.m_hostIsMaster)
        return;
      this.m_mapFromLocalId.Clear();
      this.m_mapToLocalId.Clear();
    }
  }
}
