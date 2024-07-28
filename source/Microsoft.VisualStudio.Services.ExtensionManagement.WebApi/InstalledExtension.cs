// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.InstalledExtension
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public class InstalledExtension : ExtensionManifest
  {
    private string m_fullyQualifiedName;
    private string m_extensionName;
    private string m_publisherName;

    public InstalledExtension()
    {
    }

    public InstalledExtension(InstalledExtension sourceExtension)
      : base((ExtensionManifest) sourceExtension)
    {
      this.ExtensionName = sourceExtension.ExtensionName;
      this.ExtensionDisplayName = sourceExtension.ExtensionDisplayName;
      this.PublisherName = sourceExtension.PublisherName;
      this.PublisherDisplayName = sourceExtension.PublisherDisplayName;
      this.Version = sourceExtension.Version;
      this.Flags = sourceExtension.Flags;
      this.RegistrationId = sourceExtension.RegistrationId;
      this.InstallState = sourceExtension.InstallState;
      this.LastPublished = sourceExtension.LastPublished;
      this.Files = sourceExtension.Files;
    }

    [DataMember(Name = "ExtensionId", Order = 20)]
    public string ExtensionName
    {
      get => this.m_extensionName;
      set
      {
        this.m_extensionName = value;
        this.UpdateFullyQualifiedName();
      }
    }

    [DataMember(Name = "ExtensionName", Order = 25)]
    public string ExtensionDisplayName { get; set; }

    [DataMember(Name = "PublisherId", Order = 30)]
    public string PublisherName
    {
      get => this.m_publisherName;
      set
      {
        this.m_publisherName = value;
        this.UpdateFullyQualifiedName();
      }
    }

    [DataMember(Name = "PublisherName", Order = 35)]
    public string PublisherDisplayName { get; set; }

    [DataMember(Name = "Version", Order = 40)]
    public string Version { get; set; }

    [DataMember(Order = 50, EmitDefaultValue = false)]
    public ExtensionFlags Flags { get; set; }

    [DataMember(Order = 60, EmitDefaultValue = false)]
    public Guid RegistrationId { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 120)]
    public InstalledExtensionState InstallState { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 130)]
    public DateTime LastPublished { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 140)]
    public IEnumerable<ExtensionFile> Files { get; set; }

    public string FullyQualifiedName => this.m_fullyQualifiedName;

    private void UpdateFullyQualifiedName() => this.m_fullyQualifiedName = string.IsNullOrEmpty(this.m_publisherName) || string.IsNullOrEmpty(this.m_extensionName) ? string.Empty : string.Format("{0}.{1}", (object) this.m_publisherName, (object) this.m_extensionName);
  }
}
