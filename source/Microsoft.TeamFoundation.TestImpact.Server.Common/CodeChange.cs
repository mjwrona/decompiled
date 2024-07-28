// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.CodeChange
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [CLSCompliant(false)]
  public class CodeChange
  {
    private List<int> _changesets;

    [XmlAttribute]
    public string AssemblyName { get; set; }

    [XmlAttribute]
    public Guid AssemblyIdentifier { get; set; }

    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public string Signature { get; set; }

    [XmlAttribute]
    public CodeChangeReason Reason { get; set; }

    [XmlAttribute]
    public MethodKind MethodKind { get; set; }

    [XmlAttribute]
    public MethodAccess MethodAccess { get; set; }

    [XmlAttribute]
    public string FileName { get; set; }

    [XmlArray]
    [XmlArrayItem(typeof (int))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "ChangesetsInternal", UseClientDefinedProperty = true)]
    public List<int> Changesets
    {
      get
      {
        if (this._changesets == null)
          this._changesets = new List<int>();
        return this._changesets;
      }
    }
  }
}
