// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.QueryDefinition
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.Cosmos
{
  public class QueryDefinition
  {
    private QueryDefinition.ParametersListAdapter parametersAdapter;

    [JsonProperty(PropertyName = "parameters", NullValueHandling = NullValueHandling.Ignore, Order = 1)]
    private List<SqlParameter> parameters { get; set; }

    public QueryDefinition(string query) => this.QueryText = !string.IsNullOrEmpty(query) ? query : throw new ArgumentNullException(nameof (query));

    [JsonProperty(PropertyName = "query", Order = 0)]
    public string QueryText { get; }

    internal static QueryDefinition CreateFromQuerySpec(SqlQuerySpec sqlQuery)
    {
      if (sqlQuery == null)
        return (QueryDefinition) null;
      QueryDefinition fromQuerySpec = new QueryDefinition(sqlQuery.QueryText);
      foreach (SqlParameter parameter in sqlQuery.Parameters)
        fromQuerySpec = fromQuerySpec.WithParameter(parameter.Name, parameter.Value);
      return fromQuerySpec;
    }

    public QueryDefinition WithParameter(string name, object value)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      if (this.parameters == null)
        this.parameters = new List<SqlParameter>();
      int index = this.parameters.FindIndex((Predicate<SqlParameter>) (param => param.Name == name));
      if (index != -1)
        this.parameters.RemoveAt(index);
      this.parameters.Add(new SqlParameter(name, value));
      return this;
    }

    public QueryDefinition WithParameterStream(string name, Stream valueStream)
    {
      string str = (string) null;
      using (StreamReader streamReader = new StreamReader(valueStream))
        str = streamReader.ReadToEnd();
      SerializedParameterValue serializedParameterValue = new SerializedParameterValue()
      {
        rawSerializedJsonValue = str
      };
      return this.WithParameter(name, (object) serializedParameterValue);
    }

    public IReadOnlyList<(string Name, object Value)> GetQueryParameters() => (IReadOnlyList<(string, object)>) this.parametersAdapter ?? (IReadOnlyList<(string, object)>) (this.parametersAdapter = new QueryDefinition.ParametersListAdapter(this));

    internal SqlQuerySpec ToSqlQuerySpec() => new SqlQuerySpec(this.QueryText, new SqlParameterCollection((IEnumerable<SqlParameter>) ((object) this.parameters ?? (object) Array.Empty<SqlParameter>())));

    [JsonIgnore]
    internal IReadOnlyList<SqlParameter> Parameters => (IReadOnlyList<SqlParameter>) this.parameters ?? (IReadOnlyList<SqlParameter>) Array.Empty<SqlParameter>();

    private class ParametersListAdapter : 
      IReadOnlyList<(string Name, object Value)>,
      IEnumerable<(string Name, object Value)>,
      IEnumerable,
      IReadOnlyCollection<(string Name, object Value)>
    {
      private readonly QueryDefinition queryDefinition;

      public ParametersListAdapter(QueryDefinition queryDefinition) => this.queryDefinition = queryDefinition;

      public (string Name, object Value) this[int index]
      {
        get
        {
          SqlParameter parameter = this.queryDefinition.Parameters[index];
          return (parameter.Name, parameter.Value);
        }
      }

      public int Count => this.queryDefinition.Parameters.Count;

      public IEnumerator<(string Name, object Value)> GetEnumerator()
      {
        foreach (SqlParameter parameter in (IEnumerable<SqlParameter>) this.queryDefinition.Parameters)
          yield return (parameter.Name, parameter.Value);
      }

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }
  }
}
