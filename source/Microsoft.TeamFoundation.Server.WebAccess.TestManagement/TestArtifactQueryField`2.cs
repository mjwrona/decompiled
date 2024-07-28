// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestArtifactQueryField`2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestArtifactQueryField<TQueryItem, TArtifactType> : QueryIntField<TQueryItem>
  {
    private Dictionary<string, TArtifactType> m_artifactByName;
    private Dictionary<int, TArtifactType> m_artifactById;

    public TestArtifactQueryField(
      TestManagerRequestContext testContext,
      string refName,
      string displayName,
      bool isQuerable,
      Func<TQueryItem, int> getDataValuesFunc)
      : base(testContext, refName, displayName, isQuerable, getDataValuesFunc)
    {
      this.valuesMap = this.GetIdToNameMap();
    }

    public override Dictionary<int, string> GetIdToNameMap()
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestArtifactQueryField.GetIdToNameMap");
        this.EnsurePopulatedDictionaries();
        return this.m_artifactById != null ? this.m_artifactById.ToDictionary<KeyValuePair<int, TArtifactType>, int, string>((Func<KeyValuePair<int, TArtifactType>, int>) (c => c.Key), (Func<KeyValuePair<int, TArtifactType>, string>) (c => this.GetArtifactName(c.Value))) : base.GetIdToNameMap();
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestArtifactQueryField.GetIdToNameMap");
      }
    }

    public Dictionary<string, TArtifactType> ArtifactByName
    {
      get
      {
        if (this.m_artifactByName == null)
          this.m_artifactByName = new Dictionary<string, TArtifactType>();
        return this.m_artifactByName;
      }
    }

    public Dictionary<int, TArtifactType> ArtifactById
    {
      get
      {
        if (this.m_artifactById == null)
          this.m_artifactById = new Dictionary<int, TArtifactType>();
        return this.m_artifactById;
      }
    }

    protected override QueryOperator[] GetQueryOperators() => new QueryOperator[4]
    {
      this.OperatorCollection.EqualTo,
      this.OperatorCollection.NotEquals,
      this.OperatorCollection.In,
      this.OperatorCollection.NotIn
    };

    protected virtual IEnumerable<TArtifactType> GetArtifacts() => (IEnumerable<TArtifactType>) new List<TArtifactType>();

    protected virtual string GetArtifactName(TArtifactType artifact) => string.Empty;

    protected virtual int GetArtifactId(TArtifactType artifact) => 0;

    protected virtual void EnsurePopulatedDictionaries()
    {
      try
      {
        this.TestContext.TraceEnter("BusinessLayer", "TestArtifactQueryField.EnsurePopulatedDictionaries");
        if (this.m_artifactByName != null)
          return;
        IEnumerable<TArtifactType> artifacts = this.GetArtifacts();
        this.m_artifactByName = new Dictionary<string, TArtifactType>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
        this.m_artifactById = new Dictionary<int, TArtifactType>();
        foreach (TArtifactType artifact in artifacts)
        {
          this.m_artifactByName[this.GetArtifactName(artifact)] = artifact;
          this.m_artifactById[this.GetArtifactId(artifact)] = artifact;
        }
      }
      finally
      {
        this.TestContext.TraceLeave("BusinessLayer", "TestArtifactQueryField.EnsurePopulatedDictionaries");
      }
    }
  }
}
