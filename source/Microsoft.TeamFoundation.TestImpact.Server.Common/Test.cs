// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.Test
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  public class Test : IEquatable<Test>
  {
    private List<int> _indexes;

    public Test()
    {
    }

    public Test(Guid automatedTestId, string testName, string testType, DateTime dateCompleted)
      : this(0, automatedTestId, testName, testType, dateCompleted)
    {
    }

    public Test(int testCaseId, string testName, string testType, DateTime dateCompleted)
      : this(testCaseId, Guid.Empty, testName, testType, dateCompleted)
    {
    }

    public Test(
      int testCaseId,
      Guid automatedTestId,
      string testName,
      string testType,
      DateTime dateCompleted)
    {
      this.TestCaseId = testCaseId;
      this.AutomatedTestId = automatedTestId;
      this.TestName = testName;
      this.TestType = testType;
      this.DateCompleted = dateCompleted;
      this.Exists = true;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public int TestCaseId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public Guid AutomatedTestId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string TestName { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string TestType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public DateTime DateCompleted { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public bool Exists { get; set; }

    [XmlIgnore]
    public bool IsTestCase => this.TestCaseId != 0;

    [XmlIgnore]
    public bool IsAutomated => !this.AutomatedTestId.Equals(Guid.Empty);

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.ProductInternal)]
    public AssociationOption AssociationOption { get; set; }

    [XmlArray]
    [XmlArrayItem(typeof (int))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "AssociationIndexesInternal", UseClientDefinedProperty = true)]
    public List<int> AssociationIndexes
    {
      get
      {
        if (this._indexes == null)
          this._indexes = new List<int>();
        return this._indexes;
      }
    }

    public override bool Equals(object obj) => ((IEquatable<Test>) this).Equals(obj as Test);

    bool IEquatable<Test>.Equals(Test other) => other != null && this.TestCaseId == other.TestCaseId && this.AutomatedTestId.Equals(other.AutomatedTestId);

    public override int GetHashCode() => this.TestCaseId.GetHashCode() ^ this.AutomatedTestId.GetHashCode();

    public override string ToString() => this.TestName;
  }
}
