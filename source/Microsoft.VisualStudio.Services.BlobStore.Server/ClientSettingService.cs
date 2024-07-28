// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ClientSettingService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ClientSettingService : IClientSettingService, IVssFrameworkService
  {
    public ClientSettingsInfo GetSettings(IVssRequestContext requestContext, Client toolName)
    {
      Dictionary<string, string> properties = new Dictionary<string, string>();
      try
      {
        foreach (RegistryEntry registryEntry in requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, new RegistryQuery(string.Format("{0}/{1}/*", (object) "/Configuration/ClientSettings", (object) toolName))))
          properties.Add(registryEntry.Name, registryEntry.Value);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(ContentTracePoints.ClientTool.GetSettingsException.TracePoint, ContentTracePoints.ClientTool.Area, ContentTracePoints.ClientTool.Layer, ex);
      }
      return new ClientSettingsInfo(toolName, properties);
    }

    public void SetSettings(IVssRequestContext requestContext, ClientSettingsInfo settings)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      List<RegistryItem> registryItemList = new List<RegistryItem>();
      Client client = settings.Client;
      foreach (KeyValuePair<string, string> property in settings.Properties)
        registryItemList.Add(new RegistryItem(string.Format("{0}/{1}/{2}", (object) "/Configuration/ClientSettings", (object) client, (object) property.Key), property.Value));
      if (registryItemList.Count<RegistryItem>() <= 0)
        return;
      service.Write(requestContext, (IEnumerable<RegistryItem>) registryItemList);
    }

    public void DeleteSettings(IVssRequestContext requestContext, ClientSettingsInfo settings)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      List<string> registryPathPatterns = new List<string>();
      Client client = settings.Client;
      foreach (KeyValuePair<string, string> property in settings.Properties)
        registryPathPatterns.Add(string.Format("{0}/{1}/{2}", (object) "/Configuration/ClientSettings", (object) client, (object) property.Key));
      service.DeleteEntries(requestContext, (IEnumerable<string>) registryPathPatterns);
    }

    public void SetValue(
      IVssRequestContext requestContext,
      Client toolName,
      ClientSettingKey key,
      string value)
    {
      string name = Enum.GetName(typeof (ClientSettingKey), (object) key);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      List<RegistryItem> registryItemList = new List<RegistryItem>()
      {
        new RegistryItem(string.Format("{0}/{1}/{2}", (object) "/Configuration/ClientSettings", (object) toolName, (object) name), value)
      };
      IVssRequestContext requestContext1 = requestContext;
      List<RegistryItem> items = registryItemList;
      service.Write(requestContext1, (IEnumerable<RegistryItem>) items);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
