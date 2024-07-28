// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.IdentityFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Directories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.IdentityPicker
{
  internal static class IdentityFactory
  {
    internal static Identity Create(
      DirectoryEntity directoryEntity,
      IEnumerable<string> requestedProperties,
      IVssRequestContext requestContext,
      IDictionary<string, object> options,
      IDictionary<string, object> presetProperties = null)
    {
      if (directoryEntity == null)
        return (Identity) null;
      try
      {
        Identity identity = new Identity()
        {
          EntityId = directoryEntity.EntityId,
          EntityType = directoryEntity.EntityType,
          OriginDirectory = directoryEntity.OriginDirectory,
          OriginId = directoryEntity.OriginId,
          LocalDirectory = directoryEntity.LocalDirectory,
          LocalId = directoryEntity.LocalId,
          DisplayName = directoryEntity.DisplayName,
          ScopeName = directoryEntity.ScopeName
        };
        if (identity.Properties == null)
          identity.Properties = (IDictionary<string, object>) new Dictionary<string, object>();
        if (directoryEntity.Properties != null && directoryEntity.Properties.Count > 0)
          identity.Properties = directoryEntity.Properties;
        IdentityFactory.FilterProperties(requestedProperties);
        identity.Properties["IsMru"] = (object) false;
        identity.Properties["Guest"] = (object) false;
        if (directoryEntity.Properties == null || directoryEntity.Properties.Count == 0)
          return identity;
        if (presetProperties != null && presetProperties.Count > 0)
          requestedProperties.ToList<string>().ForEach((Action<string>) (property =>
          {
            if (!presetProperties.ContainsKey(property))
              return;
            identity.Properties[property] = presetProperties[property];
          }));
        if (directoryEntity.Properties != null && directoryEntity.Properties.Count > 0)
          requestedProperties.ToList<string>().ForEach((Action<string>) (property =>
          {
            if (!directoryEntity.Properties.ContainsKey(property))
              return;
            identity.Properties[property] = directoryEntity.Properties[property];
          }));
        return identity;
      }
      catch (Exception ex)
      {
        Tracing.TraceException(requestContext, 8, ex);
        if (!(ex is IdentityPickerException))
          throw new IdentityPickerIdentityCreateException("Identity could not be created", ex);
        throw;
      }
    }

    private static void FilterProperties(IEnumerable<string> requestedProperties) => requestedProperties = (IEnumerable<string>) new HashSet<string>(Identity.AllPropertyKeys.Where<string>((Func<string, bool>) (x => requestedProperties.Contains<string>(x, (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer))));
  }
}
