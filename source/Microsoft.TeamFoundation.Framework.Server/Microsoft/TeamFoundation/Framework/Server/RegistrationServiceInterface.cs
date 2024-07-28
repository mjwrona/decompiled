// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistrationServiceInterface
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Public)]
  public class RegistrationServiceInterface
  {
    private string m_Name;
    private string m_Url;

    public RegistrationServiceInterface()
    {
    }

    public RegistrationServiceInterface(string name, string url)
    {
      this.m_Name = name;
      this.m_Url = url;
    }

    public string Name
    {
      get => this.m_Name;
      set => this.m_Name = value;
    }

    public string Url
    {
      get => this.m_Url;
      set => this.m_Url = value;
    }

    internal string ProjectUri { get; set; }

    internal Predicate<RegistrationServiceInterface> EqualsByName() => (Predicate<RegistrationServiceInterface>) (that => VssStringComparer.ServiceInterface.Equals(this.Name, that.Name));
  }
}
