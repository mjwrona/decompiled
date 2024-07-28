// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionLicensingService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Licensing;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ExtensionLicensingService : IExtensionLicensingService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IDictionary<string, bool> GetExtensionRights(
      IVssRequestContext requestContext,
      IEnumerable<string> extensionIds)
    {
      IDictionary<string, bool> dictionary = (IDictionary<string, bool>) null;
      IDictionary<string, bool> extensionRights = requestContext.GetService<ILicensingService>().GetExtensionRights(requestContext, extensionIds);
      object obj;
      if (requestContext.Items.TryGetValue("extension-rights", out obj))
        dictionary = obj as IDictionary<string, bool>;
      if (dictionary == null)
      {
        requestContext.Items["extension-rights"] = (object) extensionRights;
      }
      else
      {
        foreach (KeyValuePair<string, bool> keyValuePair in (IEnumerable<KeyValuePair<string, bool>>) extensionRights)
          dictionary[keyValuePair.Key] = keyValuePair.Value;
      }
      return extensionRights;
    }

    public bool IsExtensionLicensed(IVssRequestContext requestContext, string extensionId)
    {
      IDictionary<string, bool> dictionary = (IDictionary<string, bool>) null;
      bool flag = false;
      object obj;
      if (requestContext.Items.TryGetValue("extension-rights", out obj))
        dictionary = obj as IDictionary<string, bool>;
      if (dictionary == null)
      {
        dictionary = (IDictionary<string, bool>) new Dictionary<string, bool>();
        requestContext.Items["extension-rights"] = (object) dictionary;
      }
      if (!dictionary.TryGetValue(extensionId, out flag))
      {
        if (!requestContext.GetService<ILicensingService>().GetExtensionRights(requestContext, (IEnumerable<string>) new List<string>()
        {
          extensionId
        }).TryGetValue(extensionId, out flag))
          flag = false;
        else if (!flag)
          flag = ExtensionLicensingUtil.TryApplyingOnDemandLicense(requestContext, extensionId);
        dictionary[extensionId] = flag;
      }
      return flag;
    }
  }
}
