// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ServiceInterface
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Registration/03")]
  public class ServiceInterface
  {
    private string m_Name;
    private string m_Url;

    public ServiceInterface()
    {
    }

    public ServiceInterface(string name, string url)
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

    internal Predicate<ServiceInterface> EqualsByName() => (Predicate<ServiceInterface>) (that => VssStringComparer.ServiceInterface.Equals(this.Name, that.Name));
  }
}
