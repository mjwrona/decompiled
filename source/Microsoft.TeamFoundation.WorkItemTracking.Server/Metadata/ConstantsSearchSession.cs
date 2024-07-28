// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ConstantsSearchSession
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class ConstantsSearchSession
  {
    private IDictionary<int, ConstantRecord> m_constantIdLookups;
    private IDictionary<ConstantSetReference, SetRecord[]> m_constantSetLookups;
    private IVssRequestContext m_requestContext;

    public ConstantsSearchSession(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_constantIdLookups = (IDictionary<int, ConstantRecord>) new Dictionary<int, ConstantRecord>();
      this.m_constantSetLookups = (IDictionary<ConstantSetReference, SetRecord[]>) new Dictionary<ConstantSetReference, SetRecord[]>();
    }

    public string GetConstantDisplayPart(int constId, WitReadReplicaContext? readReplicaContext = null) => this.GetConstantRecords(new int[1]
    {
      constId
    }, readReplicaContext).Select<ConstantRecord, string>((Func<ConstantRecord, string>) (x => x.DisplayText)).FirstOrDefault<string>() ?? "";

    public ConstantRecord GetConstantRecord(int constId, WitReadReplicaContext? readReplicaContext = null) => this.GetConstantRecords(new int[1]
    {
      constId
    }, readReplicaContext).FirstOrDefault<ConstantRecord>();

    public IEnumerable<ConstantRecord> GetConstantRecords(
      int[] constIds,
      WitReadReplicaContext? readReplicaContext = null)
    {
      List<ConstantRecord> list = ((IEnumerable<int>) constIds).Where<int>((Func<int, bool>) (x => this.m_constantIdLookups.ContainsKey(x))).Select<int, ConstantRecord>((Func<int, ConstantRecord>) (x => this.m_constantIdLookups[x])).ToList<ConstantRecord>();
      int[] array = ((IEnumerable<int>) constIds).Where<int>((Func<int, bool>) (x => !this.m_constantIdLookups.ContainsKey(x))).ToArray<int>();
      foreach (ConstantRecord constantRecord in this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantRecords(this.m_requestContext, (IEnumerable<int>) array, true, readReplicaContext))
      {
        this.m_constantIdLookups[constantRecord.Id] = constantRecord;
        list.Add(constantRecord);
      }
      return (IEnumerable<ConstantRecord>) list;
    }

    public IDictionary<ConstantSetReference, SetRecord[]> ExpandNonIdentityConstantSet(
      string constName,
      bool direct = true,
      bool includeTop = false,
      bool excludeGroups = false)
    {
      ITeamFoundationWorkItemTrackingMetadataService service = this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      ConstantRecord constantRecord = service.GetNonIdentityConstants(this.m_requestContext, (IEnumerable<string>) new string[1]
      {
        constName
      }).FirstOrDefault<ConstantRecord>();
      if (constantRecord == null)
        return (IDictionary<ConstantSetReference, SetRecord[]>) new Dictionary<ConstantSetReference, SetRecord[]>();
      ConstantSetReference key = new ConstantSetReference()
      {
        Id = constantRecord.Id,
        Direct = direct,
        IncludeTop = includeTop,
        ExcludeGroups = excludeGroups
      };
      SetRecord[] setRecordArray = (SetRecord[]) null;
      if (this.m_constantSetLookups.TryGetValue(key, out setRecordArray))
        return (IDictionary<ConstantSetReference, SetRecord[]>) new Dictionary<ConstantSetReference, SetRecord[]>()
        {
          {
            key,
            setRecordArray
          }
        };
      IDictionary<ConstantSetReference, SetRecord[]> constantSets = service.GetConstantSets(this.m_requestContext, (IEnumerable<ConstantSetReference>) new ConstantSetReference[1]
      {
        key
      });
      if (!constantSets.TryGetValue(key, out setRecordArray))
        return constantSets;
      this.m_constantSetLookups[key] = setRecordArray;
      return constantSets;
    }

    public IDictionary<ConstantSetReference, SetRecord[]> ExpandConstantSets(
      int[] constIds,
      bool direct = true,
      bool includeTop = false,
      bool excludeGroups = false)
    {
      ITeamFoundationWorkItemTrackingMetadataService service = this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      List<ConstantSetReference> source = new List<ConstantSetReference>();
      foreach (int constId in constIds)
      {
        ConstantSetReference constantSetReference = new ConstantSetReference()
        {
          Id = constId,
          Direct = direct,
          IncludeTop = includeTop,
          ExcludeGroups = excludeGroups
        };
        source.Add(constantSetReference);
      }
      Dictionary<ConstantSetReference, SetRecord[]> result = new Dictionary<ConstantSetReference, SetRecord[]>();
      foreach (ConstantSetReference key in source)
      {
        SetRecord[] setRecordArray = (SetRecord[]) null;
        if (this.m_constantSetLookups.TryGetValue(key, out setRecordArray))
          result[key] = setRecordArray;
      }
      List<ConstantSetReference> list = source.Where<ConstantSetReference>((Func<ConstantSetReference, bool>) (x => !result.ContainsKey(x))).ToList<ConstantSetReference>();
      if (list.Any<ConstantSetReference>())
      {
        foreach (KeyValuePair<ConstantSetReference, SetRecord[]> constantSet in (IEnumerable<KeyValuePair<ConstantSetReference, SetRecord[]>>) service.GetConstantSets(this.m_requestContext, (IEnumerable<ConstantSetReference>) list))
        {
          ConstantSetReference key = constantSet.Key;
          this.m_constantSetLookups[key] = constantSet.Value;
          result[key] = constantSet.Value;
        }
      }
      return (IDictionary<ConstantSetReference, SetRecord[]>) result;
    }
  }
}
