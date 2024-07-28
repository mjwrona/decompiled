// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServicePrincipalIdentity
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ServicePrincipalIdentity
  {
    [XmlAttribute("servicePrincipal")]
    public string ServicePrincipal { get; set; }

    [XmlAttribute("displayName")]
    public string DisplayName { get; set; }
  }
}
