// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.ServiceProxies.SharedResourceNotifyService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.ServiceModel.Channels;
using System.Xml;

namespace Microsoft.TeamFoundation.Build.Server.ServiceProxies
{
  internal static class SharedResourceNotifyService
  {
    internal static Message NotifySharedResourceAcquired(
      string resourceName,
      string instanceId,
      Uri buildUri,
      string lockedBy)
    {
      return Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting/SharedResource/ResourceAcquired", (BodyWriter) new SharedResourceNotifyService.SharedResourceAvailableBodyWriter(resourceName, instanceId, buildUri, lockedBy));
    }

    private sealed class SharedResourceAvailableBodyWriter : BodyWriter
    {
      private Uri m_buildUri;
      private string m_instanceId;
      private string m_resourceName;
      private string m_lockedBy;

      public SharedResourceAvailableBodyWriter(
        string resourceName,
        string instanceId,
        Uri buildUri,
        string lockedBy)
        : base(true)
      {
        this.m_resourceName = resourceName;
        this.m_instanceId = instanceId;
        this.m_buildUri = buildUri;
        this.m_lockedBy = lockedBy;
      }

      protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
      {
        writer.WriteStartElement("SharedResourceAcquired", "http://schemas.microsoft.com/TeamFoundation/2010/Build/Hosting");
        writer.WriteElementString("resourceName", this.m_resourceName);
        writer.WriteElementString("instanceId", this.m_instanceId);
        if (this.m_buildUri != (Uri) null)
          writer.WriteElementString("buildUri", this.m_buildUri.AbsoluteUri);
        if (this.m_lockedBy != null)
          writer.WriteElementString("lockedBy", this.m_lockedBy);
        writer.WriteEndElement();
      }
    }
  }
}
