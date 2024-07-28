// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.SqlParameterCollection
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core
{
  internal sealed class SqlParameterCollection : 
    IList<SqlParameter>,
    ICollection<SqlParameter>,
    IEnumerable<SqlParameter>,
    IEnumerable
  {
    private readonly List<SqlParameter> parameters;

    public SqlParameterCollection() => this.parameters = new List<SqlParameter>();

    public SqlParameterCollection(IEnumerable<SqlParameter> parameters) => this.parameters = parameters != null ? new List<SqlParameter>(parameters) : throw new ArgumentNullException(nameof (parameters));

    public int IndexOf(SqlParameter item) => this.parameters.IndexOf(item);

    public void Insert(int index, SqlParameter item) => this.parameters.Insert(index, item);

    public void RemoveAt(int index) => this.parameters.RemoveAt(index);

    public SqlParameter this[int index]
    {
      get => this.parameters[index];
      set => this.parameters[index] = value;
    }

    public void Add(SqlParameter item) => this.parameters.Add(item);

    public void Clear() => this.parameters.Clear();

    public bool Contains(SqlParameter item) => this.parameters.Contains(item);

    public void CopyTo(SqlParameter[] array, int arrayIndex) => this.parameters.CopyTo(array, arrayIndex);

    public int Count => this.parameters.Count;

    public bool IsReadOnly => false;

    public bool Remove(SqlParameter item) => this.parameters.Remove(item);

    public IEnumerator<SqlParameter> GetEnumerator() => (IEnumerator<SqlParameter>) this.parameters.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.parameters.GetEnumerator();
  }
}
