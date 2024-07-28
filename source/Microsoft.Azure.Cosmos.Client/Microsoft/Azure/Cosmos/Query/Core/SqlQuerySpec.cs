// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.SqlQuerySpec
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Query.Core
{
  [DataContract]
  internal sealed class SqlQuerySpec
  {
    private SqlParameterCollection parameters;

    public SqlQuerySpec()
      : this((string) null)
    {
    }

    public SqlQuerySpec(string queryText)
      : this(queryText, new SqlParameterCollection())
    {
    }

    public SqlQuerySpec(string queryText, SqlParameterCollection parameters)
    {
      this.QueryText = queryText;
      this.parameters = parameters ?? throw new ArgumentNullException(nameof (parameters));
    }

    [DataMember(Name = "query")]
    public string QueryText { get; set; }

    [DataMember(Name = "parameters")]
    public SqlParameterCollection Parameters
    {
      get => this.parameters;
      set => this.parameters = value ?? throw new ArgumentNullException(nameof (value));
    }

    public bool ShouldSerializeParameters() => this.parameters.Count > 0;
  }
}
