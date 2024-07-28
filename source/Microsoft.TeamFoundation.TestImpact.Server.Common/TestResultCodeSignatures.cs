// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestResultCodeSignatures
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class TestResultCodeSignatures
  {
    private List<int> _indexes;

    [XmlAttribute]
    public int TestResultId { get; set; }

    [XmlAttribute]
    public int ConfigurationId { get; set; }

    [XmlArray]
    [XmlArrayItem(typeof (int))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "IndexesInternal", UseClientDefinedProperty = true)]
    public List<int> Indexes
    {
      get
      {
        if (this._indexes == null)
          this._indexes = new List<int>();
        return this._indexes;
      }
    }
  }
}
