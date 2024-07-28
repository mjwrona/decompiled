// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.LocationServiceData
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Location
{
  [DataContract]
  public class LocationServiceData : ISecuredObject
  {
    private int m_clientCacheTimeToLive = 3600;
    private bool m_accessPointsDoNotIncludeWebAppRelativeDirectory;
    private long m_lastChangeId64;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid ServiceOwner { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ICollection<AccessMapping> AccessMappings { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool ClientCacheFresh { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    [DefaultValue(3600)]
    public int ClientCacheTimeToLive
    {
      get => this.m_clientCacheTimeToLive;
      [EditorBrowsable(EditorBrowsableState.Never)] set => this.m_clientCacheTimeToLive = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DefaultAccessMappingMoniker { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int LastChangeId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long LastChangeId64
    {
      get => this.m_lastChangeId64 == 0L ? (long) this.LastChangeId : this.m_lastChangeId64;
      [EditorBrowsable(EditorBrowsableState.Never)] set => this.m_lastChangeId64 = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ICollection<ServiceDefinition> ServiceDefinitions { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    Guid ISecuredObject.NamespaceId => LocationSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => LocationSecurityConstants.NamespaceRootToken;

    internal static LocationServiceData FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      LocationServiceData locationServiceData = new LocationServiceData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "AccessPointsDoNotIncludeWebAppRelativeDirectory":
              locationServiceData.m_accessPointsDoNotIncludeWebAppRelativeDirectory = XmlConvert.ToBoolean(reader.Value);
              continue;
            case "ClientCacheFresh":
              locationServiceData.ClientCacheFresh = XmlConvert.ToBoolean(reader.Value);
              continue;
            case "DefaultAccessMappingMoniker":
              locationServiceData.DefaultAccessMappingMoniker = reader.Value;
              continue;
            case "LastChangeId":
              locationServiceData.LastChangeId = XmlConvert.ToInt32(reader.Value);
              continue;
            case "ClientCacheTimeToLive":
              locationServiceData.ClientCacheTimeToLive = XmlConvert.ToInt32(reader.Value);
              continue;
            default:
              continue;
          }
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "AccessMappings":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              locationServiceData.AccessMappings = (ICollection<AccessMapping>) XmlUtility.ArrayOfObjectFromXml<AccessMapping>(serviceProvider, reader, "AccessMapping", false, LocationServiceData.\u003C\u003EO.\u003C0\u003E__FromXml ?? (LocationServiceData.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, AccessMapping>(AccessMapping.FromXml)));
              continue;
            case "ServiceDefinitions":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              locationServiceData.ServiceDefinitions = (ICollection<ServiceDefinition>) XmlUtility.ArrayOfObjectFromXml<ServiceDefinition>(serviceProvider, reader, "ServiceDefinition", false, LocationServiceData.\u003C\u003EO.\u003C1\u003E__FromXml ?? (LocationServiceData.\u003C\u003EO.\u003C1\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServiceDefinition>(ServiceDefinition.FromXml)));
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return locationServiceData;
    }
  }
}
