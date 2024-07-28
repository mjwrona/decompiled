// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.TestSignatureData
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  [CLSCompliant(false)]
  public class TestSignatureData
  {
    private List<string> _signatures;
    private List<Test> _tests;

    [XmlArray]
    [XmlArrayItem(typeof (Test))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "TestsInternal", UseClientDefinedProperty = true)]
    public List<Test> Tests
    {
      get
      {
        if (this._tests == null)
          this._tests = new List<Test>();
        return this._tests;
      }
    }

    [XmlArray]
    [XmlArrayItem(typeof (string))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "SignaturesInternal", UseClientDefinedProperty = true)]
    public List<string> Signatures
    {
      get
      {
        if (this._signatures == null)
          this._signatures = new List<string>();
        return this._signatures;
      }
    }
  }
}
