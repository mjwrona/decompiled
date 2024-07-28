// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SqlQuerySpec
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents
{
  [DataContract]
  public sealed class SqlQuerySpec
  {
    private string queryText;
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
      if (parameters == null)
        throw new ArgumentNullException(nameof (parameters));
      this.queryText = queryText;
      this.parameters = parameters;
    }

    [DataMember(Name = "query")]
    public string QueryText
    {
      get => this.queryText;
      set => this.queryText = value;
    }

    [DataMember(Name = "parameters")]
    public SqlParameterCollection Parameters
    {
      get => this.parameters;
      set => this.parameters = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public bool ShouldSerializeParameters() => this.parameters.Count > 0;
  }
}
