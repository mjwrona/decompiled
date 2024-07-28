// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess.SecretsAccessor
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.SecretAccess
{
  public class SecretsAccessor : ISecretsAccessor
  {
    public const int SqlErrorCodeForIndexKeyColumnsMaxLengthExceeded = 1946;

    public Guid UnlockDrawer(IVssRequestContext requestContext, string drawerName)
    {
      IVssRequestContext vssRequestContext = requestContext != null ? requestContext.Elevate() : throw new ArgumentNullException(nameof (requestContext));
      return vssRequestContext.GetService<ITeamFoundationStrongBoxService>().UnlockDrawer(vssRequestContext, drawerName, false);
    }

    public Guid GetOrCreateDrawer(IVssRequestContext requestContext, string drawerName)
    {
      IVssRequestContext vssRequestContext = requestContext != null ? requestContext.Elevate() : throw new ArgumentNullException(nameof (requestContext));
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

    public void StoreValue(
      IVssRequestContext requestContext,
      Guid drawerId,
      string key,
      string value)
    {
      IVssRequestContext vssRequestContext = requestContext != null ? requestContext.Elevate() : throw new ArgumentNullException(nameof (requestContext));
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      try
      {
        service.AddString(vssRequestContext, drawerId, key, value);
      }
      catch (SqlException ex)
      {
        if (ex.Number == 1946)
          throw new InvalidRequestException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.VariableNameLengthExceeded, (object) key));
        throw;
      }
    }

    public void StoreValues(
      IVssRequestContext requestContext,
      Guid drawerId,
      string keyPrefix,
      IDictionary<string, VariableValue> secretVariables)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (secretVariables == null || !secretVariables.Any<KeyValuePair<string, VariableValue>>())
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      try
      {
        List<Tuple<StrongBoxItemInfo, string>> items = new List<Tuple<StrongBoxItemInfo, string>>();
        secretVariables.ToList<KeyValuePair<string, VariableValue>>().ForEach((Action<KeyValuePair<string, VariableValue>>) (kvp => items.Add(new Tuple<StrongBoxItemInfo, string>(new StrongBoxItemInfo()
        {
          DrawerId = drawerId,
          ItemKind = StrongBoxItemKind.String,
          LookupKey = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) keyPrefix, (object) kvp.Key)
        }, kvp.Value.Value))));
        service.AddStrings(vssRequestContext, items);
      }
      catch (SqlException ex)
      {
        throw new InvalidRequestException(ex.Message);
      }
    }

    public void DeleteValue(IVssRequestContext requestContext, Guid drawerId, string key)
    {
      IVssRequestContext vssRequestContext = requestContext != null ? requestContext.Elevate() : throw new ArgumentNullException(nameof (requestContext));
      vssRequestContext.GetService<ITeamFoundationStrongBoxService>().DeleteItem(vssRequestContext, drawerId, key);
    }

    public void DeleteDrawer(IVssRequestContext requestContext, Guid drawerId)
    {
      IVssRequestContext vssRequestContext = requestContext != null ? requestContext.Elevate() : throw new ArgumentNullException(nameof (requestContext));
      vssRequestContext.GetService<ITeamFoundationStrongBoxService>().DeleteDrawer(vssRequestContext, drawerId);
    }

    public void ReadValues(
      IVssRequestContext requestContext,
      Guid drawerId,
      HashSet<string> lookupKeys,
      Action<string, string> secretSetter)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (lookupKeys.Count == 0)
        return;
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      foreach (StrongBoxItemInfo drawerContent in service.GetDrawerContents(vssRequestContext, drawerId))
      {
        if (lookupKeys.Contains(drawerContent.LookupKey))
          secretSetter(drawerContent.LookupKey, service.GetString(vssRequestContext, drawerContent));
      }
    }

    public bool TryGetStrongBoxValue(
      IVssRequestContext requestContext,
      Guid drawerId,
      string key,
      out string value)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      try
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
        value = service.GetString(vssRequestContext, drawerId, key);
        return true;
      }
      catch (StrongBoxItemNotFoundException ex)
      {
        value = (string) null;
        return false;
      }
    }
  }
}
