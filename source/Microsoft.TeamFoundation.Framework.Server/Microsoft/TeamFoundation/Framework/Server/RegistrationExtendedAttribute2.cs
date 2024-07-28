// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistrationExtendedAttribute2
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
  public class RegistrationExtendedAttribute2
  {
    private string m_Name;
    private string m_Value;

    public RegistrationExtendedAttribute2()
    {
    }

    public RegistrationExtendedAttribute2(string name, string value)
    {
      this.m_Name = name;
      this.m_Value = value;
    }

    public string Name
    {
      get => this.m_Name;
      set => this.m_Name = value;
    }

    public string Value
    {
      get => this.m_Value;
      set => this.m_Value = value;
    }

    internal string SourceRegistryPath { get; set; }

    internal Predicate<RegistrationExtendedAttribute2> EqualsByName() => (Predicate<RegistrationExtendedAttribute2>) (that => VssStringComparer.RegistrationAttributeName.Equals(this.Name, that.Name));
  }
}
