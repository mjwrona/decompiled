// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PartitionKeyBuilder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class PartitionKeyBuilder
  {
    private readonly List<object> partitionKeyValues;

    public PartitionKeyBuilder() => this.partitionKeyValues = new List<object>();

    public PartitionKeyBuilder Add(string val)
    {
      this.partitionKeyValues.Add((object) val);
      return this;
    }

    public PartitionKeyBuilder Add(double val)
    {
      this.partitionKeyValues.Add((object) val);
      return this;
    }

    public PartitionKeyBuilder Add(bool val)
    {
      this.partitionKeyValues.Add((object) val);
      return this;
    }

    public PartitionKeyBuilder AddNullValue()
    {
      this.partitionKeyValues.Add((object) null);
      return this;
    }

    public PartitionKeyBuilder AddNoneType()
    {
      this.partitionKeyValues.Add((object) PartitionKey.None);
      return this;
    }

    public PartitionKey Build()
    {
      if (this.partitionKeyValues.Count == 0)
        throw new ArgumentException("No partition key value has been specifed");
      if (this.partitionKeyValues.Count == 1 && PartitionKey.None.Equals(this.partitionKeyValues[0]))
        return PartitionKey.None;
      object[] keyValues = new object[this.partitionKeyValues.Count];
      for (int index = 0; index < this.partitionKeyValues.Count; ++index)
      {
        object partitionKeyValue = this.partitionKeyValues[index];
        keyValues[index] = !PartitionKey.None.Equals(partitionKeyValue) ? partitionKeyValue : (object) Undefined.Value;
      }
      return new PartitionKey(new Microsoft.Azure.Documents.PartitionKey(keyValues).InternalKey);
    }
  }
}
