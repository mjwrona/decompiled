// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VariableGroupSecretsHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class VariableGroupSecretsHelper : IVariableGroupSecretsHelper
  {
    public void StoreSecrets(IVssRequestContext context, VariableGroup group)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<VariableGroup>(group, nameof (group));
      if (!this.HasSecretsWithValues(group))
        return;
      string forVariableGroup = this.GetDrawerNameAtCollectionLevelForVariableGroup(group.Id);
      Guid drawer = this.GetOrCreateDrawer(context, forVariableGroup);
      this.StoreSecretVariables(context, drawer, group.Variables);
    }

    public void UpdateSecrets(IVssRequestContext context, VariableGroup group)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<VariableGroup>(group, nameof (group));
      if (!this.HasSecretsWithValues(group))
        return;
      string forVariableGroup = this.GetDrawerNameAtCollectionLevelForVariableGroup(group.Id);
      Guid drawer = this.GetOrCreateDrawer(context, forVariableGroup);
      this.StoreSecretVariables(context, drawer, group.Variables);
    }

    public void ReadSecrets(IVssRequestContext context, Guid projectId, VariableGroup group)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckForNull<VariableGroup>(group, nameof (group));
      if (!this.HasSecrets(group))
        return;
      string forVariableGroup1 = this.GetDrawerNameAtCollectionLevelForVariableGroup(group.Id);
      Guid drawerId = this.UnlockDrawer(context, forVariableGroup1);
      if (drawerId == Guid.Empty)
      {
        string forVariableGroup2 = this.GetDrawerNameAtProjectLevelForVariableGroup(projectId, group.Id);
        drawerId = this.UnlockDrawer(context, forVariableGroup2);
        if (drawerId == Guid.Empty)
        {
          context.TraceInfo("VariableGroup", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "No drawer id exists for variable group id: {0}, name: {1} and drawer name: {2}", (object) group.Id, (object) group.Name, (object) forVariableGroup2));
          return;
        }
      }
      this.ReadSecretVariables(context, drawerId, group.Variables);
    }

    public void DeleteSecrets(IVssRequestContext context, Guid projectId, int groupId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      string forVariableGroup1 = this.GetDrawerNameAtCollectionLevelForVariableGroup(groupId);
      Guid drawerId = this.UnlockDrawer(context, forVariableGroup1);
      if (drawerId == Guid.Empty)
      {
        string forVariableGroup2 = this.GetDrawerNameAtProjectLevelForVariableGroup(projectId, groupId);
        drawerId = this.UnlockDrawer(context, forVariableGroup2);
        if (drawerId == Guid.Empty)
          return;
      }
      this.DeleteDrawer(context, drawerId);
    }

    public void DeleteSecrets(IVssRequestContext context, int groupId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      string forVariableGroup = this.GetDrawerNameAtCollectionLevelForVariableGroup(groupId);
      Guid drawerId = this.UnlockDrawer(context, forVariableGroup);
      if (drawerId == Guid.Empty)
        return;
      this.DeleteDrawer(context, drawerId);
    }

    public void MigrateSecretsFromProjectToCollectionLevel(
      IVssRequestContext context,
      Guid projectId,
      int groupId)
    {
      IVssRequestContext strongBoxRequestContext = context.Elevate();
      string forVariableGroup1 = this.GetDrawerNameAtProjectLevelForVariableGroup(projectId, groupId);
      Guid drawerId = this.UnlockDrawer(strongBoxRequestContext, forVariableGroup1);
      if (drawerId == Guid.Empty)
      {
        context.TraceInfo("VariableGroup", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Secrets for variable group id : {0} and drawer name : {1} are not available at project level so no migration is required.", (object) groupId, (object) forVariableGroup1));
      }
      else
      {
        ITeamFoundationStrongBoxService strongBoxService = strongBoxRequestContext.GetService<ITeamFoundationStrongBoxService>();
        List<StrongBoxItemInfo> drawerContents = strongBoxService.GetDrawerContents(strongBoxRequestContext, drawerId);
        if (drawerContents.Count <= 0)
          return;
        string forVariableGroup2 = this.GetDrawerNameAtCollectionLevelForVariableGroup(groupId);
        Guid drawerIdAtCollection = this.GetOrCreateDrawer(strongBoxRequestContext, forVariableGroup2);
        List<Tuple<StrongBoxItemInfo, string>> items = new List<Tuple<StrongBoxItemInfo, string>>();
        IEnumerable<Tuple<StrongBoxItemInfo, string>> collection = drawerContents.Select<StrongBoxItemInfo, Tuple<StrongBoxItemInfo, string>>((Func<StrongBoxItemInfo, Tuple<StrongBoxItemInfo, string>>) (item => new Tuple<StrongBoxItemInfo, string>(new StrongBoxItemInfo()
        {
          LookupKey = item.LookupKey,
          DrawerId = drawerIdAtCollection,
          ItemKind = StrongBoxItemKind.String
        }, strongBoxService.GetString(strongBoxRequestContext, item))));
        items.AddRange(collection);
        strongBoxService.AddStrings(strongBoxRequestContext, items);
        this.DeleteDrawer(context, drawerId);
      }
    }

    private Guid GetOrCreateDrawer(IVssRequestContext requestContext, string drawerName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawer = service.UnlockDrawer(vssRequestContext, drawerName, false);
      if (drawer != Guid.Empty)
        return drawer;
      try
      {
        return service.CreateDrawer(vssRequestContext, drawerName);
      }
      catch (StrongBoxDrawerExistsException ex)
      {
        return service.UnlockDrawer(vssRequestContext, drawerName, true);
      }
    }

    private void DeleteKey(IVssRequestContext requestContext, Guid drawerId, string key)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<ITeamFoundationStrongBoxService>().DeleteItem(vssRequestContext, drawerId, key);
    }

    private void DeleteDrawer(IVssRequestContext requestContext, Guid drawerId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<ITeamFoundationStrongBoxService>().DeleteDrawer(vssRequestContext, drawerId);
    }

    private Guid UnlockDrawer(IVssRequestContext requestContext, string drawerName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return vssRequestContext.GetService<ITeamFoundationStrongBoxService>().UnlockDrawer(vssRequestContext, drawerName, false);
    }

    private void StoreSecretVariables(
      IVssRequestContext requestContext,
      Guid drawerId,
      IDictionary<string, VariableValue> variables)
    {
      IEnumerable<KeyValuePair<string, VariableValue>> variablesToBeStored = VariableGroupSecretsHelper.GetSecretVariablesToBeStored(variables);
      if (variablesToBeStored == null || variablesToBeStored.Count<KeyValuePair<string, VariableValue>>() <= 0)
        return;
      IVssRequestContext context = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = context.GetService<ITeamFoundationStrongBoxService>();
      List<Tuple<StrongBoxItemInfo, string>> tupleList = new List<Tuple<StrongBoxItemInfo, string>>();
      IEnumerable<Tuple<StrongBoxItemInfo, string>> collection = variablesToBeStored.Select<KeyValuePair<string, VariableValue>, Tuple<StrongBoxItemInfo, string>>((Func<KeyValuePair<string, VariableValue>, Tuple<StrongBoxItemInfo, string>>) (kvp => new Tuple<StrongBoxItemInfo, string>(new StrongBoxItemInfo()
      {
        LookupKey = kvp.Key,
        DrawerId = drawerId,
        ItemKind = StrongBoxItemKind.String
      }, kvp.Value.Value)));
      tupleList.AddRange(collection);
      IVssRequestContext requestContext1 = context;
      List<Tuple<StrongBoxItemInfo, string>> items = tupleList;
      service.AddStrings(requestContext1, items);
    }

    private static IEnumerable<KeyValuePair<string, VariableValue>> GetSecretVariablesToBeStored(
      IDictionary<string, VariableValue> secrets)
    {
      return secrets.Where<KeyValuePair<string, VariableValue>>((Func<KeyValuePair<string, VariableValue>, bool>) (s => s.Value != null && s.Value.IsSecret && !string.IsNullOrEmpty(s.Value.Value)));
    }

    private void ReadSecretVariables(
      IVssRequestContext context,
      Guid drawerId,
      IDictionary<string, VariableValue> variables)
    {
      IVssRequestContext strongBoxRequestContext = context.Elevate();
      ITeamFoundationStrongBoxService strongBoxService = strongBoxRequestContext.GetService<ITeamFoundationStrongBoxService>();
      List<StrongBoxItemInfo> drawerContents = strongBoxService.GetDrawerContents(strongBoxRequestContext, drawerId);
      if (drawerContents.Count <= 0)
        return;
      drawerContents.ForEach((Action<StrongBoxItemInfo>) (item =>
      {
        if (!variables.Keys.Contains(item.LookupKey) || !variables[item.LookupKey].IsSecret)
          return;
        string str = strongBoxService.GetString(strongBoxRequestContext, item);
        variables[item.LookupKey].Value = str;
      }));
    }

    private bool HasSecretsWithValues(VariableGroup group) => VariableGroupUtility.HasSecretWithValue(group.Variables);

    private bool HasSecrets(VariableGroup group) => VariableGroupUtility.HasSecret(group.Variables);

    private string GetDrawerNameAtProjectLevelForVariableGroup(Guid projectId, int groupId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/DistributedTask/{0}/VariableGroup/{1}", (object) projectId, (object) groupId);

    private string GetDrawerNameAtCollectionLevelForVariableGroup(int groupId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Service/DistributedTask/VariableGroup/{0}", (object) groupId);
  }
}
