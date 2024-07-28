// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceTaggingService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ResourceTaggingService : IResourceTaggingService, IVssFrameworkService
  {
    private const string TraceArea = "Commerce";
    private const string TraceLayer = "ResourceTaggingService";
    private const string RegistryBase = "/commerce/service/resource";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.Trace(5108699, TraceLevel.Info, "Commerce", nameof (ResourceTaggingService), "ResourceTaggingService starting");
    }

    public void SaveTags(
      IVssRequestContext requestContext,
      string resourceName,
      Dictionary<string, string> tags)
    {
      requestContext.TraceEnter(5108701, "Commerce", nameof (ResourceTaggingService), nameof (SaveTags));
      try
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
        if (tags == null || !tags.Any<KeyValuePair<string, string>>())
          return;
        string registryPath = this.GetRegistryPath(resourceName);
        string message = JsonConvert.SerializeObject((object) tags);
        requestContext.Trace(5108702, TraceLevel.Info, "Commerce", nameof (ResourceTaggingService), message);
        if (string.IsNullOrEmpty(message))
          return;
        requestContext.GetService<IVssRegistryService>().SetValue<string>(requestContext, registryPath, message);
      }
      finally
      {
        requestContext.TraceLeave(5108703, "Commerce", nameof (ResourceTaggingService), nameof (SaveTags));
      }
    }

    public Dictionary<string, string> GetTags(
      IVssRequestContext requestContext,
      string resourceName)
    {
      requestContext.TraceEnter(5108706, "Commerce", nameof (ResourceTaggingService), nameof (GetTags));
      try
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
        string registryPath = this.GetRegistryPath(resourceName);
        string message = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) registryPath, string.Empty);
        requestContext.Trace(5108707, TraceLevel.Info, "Commerce", nameof (ResourceTaggingService), message);
        return !string.IsNullOrEmpty(message) ? JsonConvert.DeserializeObject<Dictionary<string, string>>(message) : new Dictionary<string, string>();
      }
      finally
      {
        requestContext.TraceLeave(5108708, "Commerce", nameof (ResourceTaggingService), nameof (GetTags));
      }
    }

    public void DeleteTags(IVssRequestContext requestContext, string resourceName)
    {
      requestContext.TraceEnter(5108704, "Commerce", nameof (ResourceTaggingService), nameof (DeleteTags));
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      try
      {
        string registryPath = this.GetRegistryPath(resourceName);
        requestContext.GetService<IVssRegistryService>().DeleteEntries(requestContext, registryPath);
      }
      finally
      {
        requestContext.TraceLeave(5108705, "Commerce", nameof (ResourceTaggingService), nameof (DeleteTags));
      }
    }

    private string GetRegistryPath(string resourceName) => "/commerce/service/resource/" + resourceName + "/tags";

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.Trace(5108700, TraceLevel.Info, "Commerce", nameof (ResourceTaggingService), "ResourceTaggingService ending");

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
    }
  }
}
