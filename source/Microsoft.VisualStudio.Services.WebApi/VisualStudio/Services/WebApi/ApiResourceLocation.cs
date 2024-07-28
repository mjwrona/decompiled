// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ApiResourceLocation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  public class ApiResourceLocation : IEquatable<ApiResourceLocation>, ISecuredObject
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Area { get; set; }

    [DataMember]
    public string ResourceName { get; set; }

    [DataMember]
    public string RouteTemplate { get; set; }

    public string RouteName { get; set; }

    [DataMember]
    public int ResourceVersion { get; set; }

    public Version MinVersion { get; set; }

    [DataMember(Name = "MinVersion")]
    public string MinVersionString
    {
      get => this.MinVersion.ToString(2);
      private set
      {
        if (string.IsNullOrEmpty(value))
          this.MinVersion = new Version(1, 0);
        else
          this.MinVersion = new Version(value);
      }
    }

    public Version MaxVersion { get; set; }

    [DataMember(Name = "MaxVersion")]
    public string MaxVersionString
    {
      get => this.MaxVersion.ToString(2);
      private set
      {
        if (string.IsNullOrEmpty(value))
          this.MaxVersion = new Version(1, 0);
        else
          this.MaxVersion = new Version(value);
      }
    }

    public Version ReleasedVersion { get; set; }

    [DataMember(Name = "ReleasedVersion")]
    public string ReleasedVersionString
    {
      get => this.ReleasedVersion.ToString(2);
      private set
      {
        if (string.IsNullOrEmpty(value))
          this.ReleasedVersion = new Version(1, 0);
        else
          this.ReleasedVersion = new Version(value);
      }
    }

    public ServiceDefinition ToServiceDefinition(InheritLevel level = InheritLevel.None) => new ServiceDefinition()
    {
      Identifier = this.Id,
      ServiceType = this.Area,
      DisplayName = this.ResourceName,
      Description = "Resource Location",
      RelativePath = this.RouteTemplate,
      ResourceVersion = this.ResourceVersion,
      MinVersion = this.MinVersion,
      MaxVersion = this.MaxVersion,
      ReleasedVersion = this.ReleasedVersion,
      ToolId = "Framework",
      InheritLevel = level
    };

    public static ApiResourceLocation FromServiceDefinition(ServiceDefinition definition) => new ApiResourceLocation()
    {
      Id = definition.Identifier,
      Area = definition.ServiceType,
      ResourceName = definition.DisplayName,
      RouteTemplate = definition.RelativePath,
      ResourceVersion = definition.ResourceVersion,
      MinVersion = definition.MinVersion,
      MaxVersion = definition.MaxVersion,
      ReleasedVersion = definition.ReleasedVersion
    };

    public bool Equals(ApiResourceLocation other) => object.Equals((object) this.Id, (object) other.Id) && string.Equals(this.Area, other.Area) && string.Equals(this.ResourceName, other.ResourceName) && string.Equals(this.RouteTemplate, other.RouteTemplate) && string.Equals(this.RouteName, other.RouteName) && object.Equals((object) this.ResourceVersion, (object) other.ResourceVersion) && object.Equals((object) this.MinVersion, (object) other.MinVersion) && object.Equals((object) this.MaxVersion, (object) other.MaxVersion) && object.Equals((object) this.ReleasedVersion, (object) other.ReleasedVersion);

    Guid ISecuredObject.NamespaceId => LocationSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => LocationSecurityConstants.NamespaceRootToken;
  }
}
