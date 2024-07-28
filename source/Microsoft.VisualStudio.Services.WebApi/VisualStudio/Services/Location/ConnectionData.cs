// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.ConnectionData
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Location
{
  [DataContract]
  public class ConnectionData : ISecuredObject
  {
    private Guid m_catalogResourceId;
    private int m_serverCapabilities;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Microsoft.VisualStudio.Services.Identity.Identity AuthenticatedUser { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Microsoft.VisualStudio.Services.Identity.Identity AuthorizedUser { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid InstanceId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid DeploymentId { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DeploymentFlags DeploymentType { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime? LastUserAccess { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public LocationServiceData LocationServiceData { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string WebApplicationRelativeDirectory { get; [EditorBrowsable(EditorBrowsableState.Never)] set; }

    internal static ConnectionData FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ConnectionData connectionData = new ConnectionData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "CatalogResourceId":
              connectionData.m_catalogResourceId = XmlConvert.ToGuid(reader.Value);
              continue;
            case "InstanceId":
              connectionData.InstanceId = XmlConvert.ToGuid(reader.Value);
              continue;
            case "ServerCapabilities":
              connectionData.m_serverCapabilities = XmlConvert.ToInt32(reader.Value);
              continue;
            case "WebApplicationRelativeDirectory":
              connectionData.WebApplicationRelativeDirectory = reader.Value;
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
            case "AuthenticatedUser":
              connectionData.AuthenticatedUser = Microsoft.VisualStudio.Services.Identity.Identity.FromXml(serviceProvider, reader);
              continue;
            case "AuthorizedUser":
              connectionData.AuthorizedUser = Microsoft.VisualStudio.Services.Identity.Identity.FromXml(serviceProvider, reader);
              continue;
            case "LocationServiceData":
              connectionData.LocationServiceData = LocationServiceData.FromXml(serviceProvider, reader);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return connectionData;
    }

    Guid ISecuredObject.NamespaceId => LocationSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => LocationSecurityConstants.NamespaceRootToken;
  }
}
