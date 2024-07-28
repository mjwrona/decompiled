// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.PublisherEventExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public static class PublisherEventExtensions
  {
    public static string GetDiagnostic(this PublisherEvent publisherEvent, string diagnosticsKey) => publisherEvent?.Diagnostics == null || !publisherEvent.Diagnostics.ContainsKey(diagnosticsKey) ? string.Empty : publisherEvent.Diagnostics[diagnosticsKey];

    public static bool HasResourceForVersion(
      this PublisherEvent publisherEvent,
      string resourceVersion)
    {
      return string.IsNullOrEmpty(resourceVersion) || string.Equals(publisherEvent.Event.ResourceVersion, resourceVersion, StringComparison.OrdinalIgnoreCase) || publisherEvent.OtherResourceVersions.Where<VersionedResource>((Func<VersionedResource, bool>) (vr => string.Equals(vr.ResourceVersion, resourceVersion, StringComparison.OrdinalIgnoreCase))).Any<VersionedResource>();
    }

    public static Event GetEventForVersion(
      this PublisherEvent publisherEvent,
      string resourceVersion)
    {
      if (string.IsNullOrEmpty(resourceVersion) || string.Equals("latest", resourceVersion, StringComparison.OrdinalIgnoreCase) || string.Equals(publisherEvent.Event.ResourceVersion, resourceVersion, StringComparison.OrdinalIgnoreCase) || string.Equals("1.0-preview.1", resourceVersion, StringComparison.OrdinalIgnoreCase) && (string.Equals("workitem.created", publisherEvent.Event.EventType, StringComparison.OrdinalIgnoreCase) || string.Equals("workitem.updated", publisherEvent.Event.EventType, StringComparison.OrdinalIgnoreCase) || string.Equals("workitem.commented", publisherEvent.Event.EventType, StringComparison.OrdinalIgnoreCase)))
        return publisherEvent.Event;
      return new Event()
      {
        CreatedDate = publisherEvent.Event.CreatedDate,
        DetailedMessage = publisherEvent.Event.DetailedMessage,
        EventType = publisherEvent.Event.EventType,
        Id = publisherEvent.Event.Id,
        Message = publisherEvent.Event.Message,
        PublisherId = publisherEvent.Event.PublisherId,
        Resource = publisherEvent.GetOtherResourceForVersion(resourceVersion),
        ResourceVersion = resourceVersion,
        ResourceContainers = publisherEvent.Event.ResourceContainers,
        SessionToken = publisherEvent.Event.SessionToken
      };
    }

    public static string[] GetOrderedResourceVersions(this PublisherEvent publisherEvent)
    {
      List<string> source = new List<string>();
      source.Add(publisherEvent.Event.ResourceVersion);
      if (publisherEvent.OtherResourceVersions != null)
      {
        foreach (VersionedResource otherResourceVersion in publisherEvent.OtherResourceVersions)
          source.Add(otherResourceVersion.ResourceVersion);
      }
      return source.ToArray<string>();
    }

    private static object GetOtherResourceForVersion(
      this PublisherEvent publisherEvent,
      string resourceVersion)
    {
      if (publisherEvent.OtherResourceVersions == null)
        return (object) null;
      VersionedResource versionedResource = publisherEvent.OtherResourceVersions.Where<VersionedResource>((Func<VersionedResource, bool>) (vr => string.Equals(vr.ResourceVersion, resourceVersion, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<VersionedResource>();
      if (versionedResource == null)
        return (object) null;
      if (versionedResource.Resource != null)
        return versionedResource.Resource;
      if (string.Equals(versionedResource.CompatibleWith, publisherEvent.Event.ResourceVersion, StringComparison.OrdinalIgnoreCase))
        return publisherEvent.Event.Resource;
      return publisherEvent.OtherResourceVersions.Where<VersionedResource>((Func<VersionedResource, bool>) (vr => string.Equals(vr.ResourceVersion, versionedResource.CompatibleWith, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<VersionedResource>()?.Resource;
    }
  }
}
