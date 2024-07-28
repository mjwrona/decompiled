// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.SqlParameter
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Query.Core
{
  [DataContract]
  internal sealed class SqlParameter : IEquatable<SqlParameter>
  {
    public SqlParameter()
    {
    }

    public SqlParameter(string name) => this.Name = name;

    public SqlParameter(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "value")]
    public object Value { get; set; }

    public bool Equals(SqlParameter other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return !(this.Name == other.Name) || other.Value != null ? other.Value.Equals(this.Value) : this.Value == null;
    }

    public override int GetHashCode() => (17 * 233 + (this.Name == null ? 0 : this.Name.GetHashCode())) * 233 + (this.Value == null ? 0 : this.Value.GetHashCode());
  }
}
