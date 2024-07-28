// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionState
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [DataContract]
  public sealed class ExtensionState : InstalledExtensionState
  {
    private string m_fullyQualifiedName;
    private string m_extensionName;
    private string m_publisherName;

    public ExtensionState()
    {
    }

    public ExtensionState(ExtensionState sourceState)
      : base((InstalledExtensionState) sourceState)
    {
      this.PublisherName = sourceState.PublisherName;
      this.ExtensionName = sourceState.ExtensionName;
      this.RegistrationId = sourceState.RegistrationId;
      this.Version = sourceState.Version;
      this.LastVersionCheck = sourceState.LastVersionCheck;
    }

    [DataMember]
    public string PublisherName
    {
      get => this.m_publisherName;
      set
      {
        this.m_publisherName = value;
        this.UpdateFullyQualifiedName();
      }
    }

    [DataMember]
    public string ExtensionName
    {
      get => this.m_extensionName;
      set
      {
        this.m_extensionName = value;
        this.UpdateFullyQualifiedName();
      }
    }

    [IgnoreDataMember]
    public Guid RegistrationId { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public DateTime? LastVersionCheck { get; internal set; }

    public string FullyQualifiedName => this.m_fullyQualifiedName;

    private void UpdateFullyQualifiedName() => this.m_fullyQualifiedName = string.IsNullOrEmpty(this.m_publisherName) || string.IsNullOrEmpty(this.m_extensionName) ? string.Empty : string.Format("{0}.{1}", (object) this.m_publisherName, (object) this.m_extensionName);
  }
}
