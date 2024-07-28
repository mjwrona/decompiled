// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.PlanSecretStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class PlanSecretStore
  {
    private readonly Guid m_planId;
    private readonly IVssRequestContext m_requestContext;
    private IDictionary<string, StrongBoxItemInfo> m_items;
    private readonly ITeamFoundationStrongBoxService m_strongbox;

    public PlanSecretStore(IVssRequestContext requestContext, Guid planId)
    {
      this.m_planId = planId;
      this.m_requestContext = requestContext.Elevate();
      this.m_strongbox = this.m_requestContext.GetService<ITeamFoundationStrongBoxService>();
    }

    public void Delete()
    {
      using (new MethodScope(this.m_requestContext, nameof (PlanSecretStore), nameof (Delete)))
      {
        Guid drawerId = this.UnlockDrawer();
        if (!(drawerId != Guid.Empty))
          return;
        this.m_strongbox.DeleteDrawer(this.m_requestContext, drawerId);
      }
    }

    public void DeleteValues(
      IList<OutputVariableSecret> secretVariablesToDelete)
    {
      using (new MethodScope(this.m_requestContext, nameof (PlanSecretStore), nameof (DeleteValues)))
      {
        Guid drawerId = this.UnlockDrawer();
        foreach (OutputVariableSecret outputVariableSecret in (IEnumerable<OutputVariableSecret>) secretVariablesToDelete)
          this.m_strongbox.DeleteItem(this.m_requestContext, drawerId, outputVariableSecret.GetLookupKey());
      }
    }

    public string GetVariable(string name)
    {
      using (new MethodScope(this.m_requestContext, nameof (PlanSecretStore), nameof (GetVariable)))
      {
        if (this.m_items == null)
          this.m_items = this.ReadSecrets(this.m_requestContext, this.m_planId);
        StrongBoxItemInfo strongBoxItemInfo;
        return this.m_items.TryGetValue(VariableSecret.GetLookupKey(name), out strongBoxItemInfo) ? this.m_strongbox.GetString(this.m_requestContext, strongBoxItemInfo) : (string) null;
      }
    }

    public string GetOutputVariable(Guid timelineId, Guid recordId, string name)
    {
      using (new MethodScope(this.m_requestContext, nameof (PlanSecretStore), nameof (GetOutputVariable)))
      {
        if (this.m_items == null)
          this.m_items = this.ReadSecrets(this.m_requestContext, this.m_planId);
        StrongBoxItemInfo strongBoxItemInfo;
        return this.m_items.TryGetValue(OutputVariableSecret.GetLookupKey(timelineId, recordId, name), out strongBoxItemInfo) ? this.m_strongbox.GetString(this.m_requestContext, strongBoxItemInfo) : (string) null;
      }
    }

    public void SetValues(IList<VariableSecret> secrets)
    {
      using (new MethodScope(this.m_requestContext, nameof (PlanSecretStore), nameof (SetValues)))
      {
        if (secrets == null || secrets.Count == 0)
          return;
        this.m_strongbox.AddStrings(this.m_requestContext, this.DedupeSecrets(this.UnlockDrawer(true), secrets));
      }
    }

    private List<Tuple<StrongBoxItemInfo, string>> DedupeSecrets(
      Guid drawerId,
      IList<VariableSecret> secrets)
    {
      if (secrets == null || secrets.Count == 0)
        return new List<Tuple<StrongBoxItemInfo, string>>();
      List<Tuple<StrongBoxItemInfo, string>> list = secrets.ToDedupedDictionary<VariableSecret, string, string>((Func<VariableSecret, string>) (k => k.GetLookupKey()), (Func<VariableSecret, string>) (e => e.Value)).Select<KeyValuePair<string, string>, Tuple<StrongBoxItemInfo, string>>((Func<KeyValuePair<string, string>, Tuple<StrongBoxItemInfo, string>>) (p => new Tuple<StrongBoxItemInfo, string>(new StrongBoxItemInfo()
      {
        DrawerId = drawerId,
        LookupKey = p.Key
      }, p.Value))).ToList<Tuple<StrongBoxItemInfo, string>>();
      if (list.Count == secrets.Count)
        return list;
      this.m_requestContext.TraceWarning("TaskHub", "Duplicate secret keys found. keys=" + string.Join(",", secrets.Select<VariableSecret, string>((Func<VariableSecret, string>) (s => s.Name))));
      return list;
    }

    private Guid UnlockDrawer(bool createIfNotExists = false)
    {
      string planDrawer = PlanSecretStore.GetPlanDrawer(this.m_planId);
      Guid guid = this.m_strongbox.UnlockDrawer(this.m_requestContext, planDrawer, false);
      if (guid == Guid.Empty & createIfNotExists)
      {
        try
        {
          guid = this.m_strongbox.CreateDrawer(this.m_requestContext, planDrawer);
        }
        catch (StrongBoxDrawerExistsException ex)
        {
          guid = this.m_strongbox.UnlockDrawer(this.m_requestContext, planDrawer, false);
        }
        catch (Exception ex)
        {
          this.m_requestContext.TraceException(0, "TaskHub", ex);
          throw;
        }
      }
      return guid;
    }

    private static string GetPlanDrawer(Guid planId) => string.Format("ms.tf.distributedtask.plan.{0}", (object) planId);

    private IDictionary<string, StrongBoxItemInfo> ReadSecrets(
      IVssRequestContext requestContext,
      Guid planId)
    {
      Guid drawerId = this.UnlockDrawer();
      return !(drawerId != Guid.Empty) ? (IDictionary<string, StrongBoxItemInfo>) new Dictionary<string, StrongBoxItemInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, StrongBoxItemInfo>) this.m_strongbox.GetDrawerContents(requestContext, drawerId).ToDictionary<StrongBoxItemInfo, string>((Func<StrongBoxItemInfo, string>) (x => x.LookupKey), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
