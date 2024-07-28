// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.AccessMapping
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Location
{
  [DataContract]
  public class AccessMapping : ISecuredObject
  {
    public AccessMapping()
    {
    }

    public AccessMapping(
      string moniker,
      string displayName,
      string accessPoint,
      Guid serviceOwner = default (Guid))
      : this(moniker, displayName, accessPoint, serviceOwner, (string) null)
    {
    }

    public AccessMapping(
      string moniker,
      string displayName,
      string accessPoint,
      Guid serviceOwner,
      string virtualDirectory)
    {
      this.DisplayName = displayName;
      this.Moniker = moniker;
      this.AccessPoint = accessPoint;
      this.ServiceOwner = serviceOwner;
      this.VirtualDirectory = virtualDirectory;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Moniker { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string AccessPoint { get; set; }

    [DataMember]
    public Guid ServiceOwner { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string VirtualDirectory { get; set; }

    public AccessMapping Clone() => new AccessMapping(this.Moniker, this.DisplayName, this.AccessPoint, this.ServiceOwner, this.VirtualDirectory);

    internal static AccessMapping FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      AccessMapping accessMapping = new AccessMapping();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "AccessPoint":
              accessMapping.AccessPoint = reader.Value;
              continue;
            case "DisplayName":
              accessMapping.DisplayName = reader.Value;
              continue;
            case "Moniker":
              accessMapping.Moniker = reader.Value;
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
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return accessMapping;
    }

    Guid ISecuredObject.NamespaceId => LocationSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => LocationSecurityConstants.NamespaceRootToken;
  }
}
