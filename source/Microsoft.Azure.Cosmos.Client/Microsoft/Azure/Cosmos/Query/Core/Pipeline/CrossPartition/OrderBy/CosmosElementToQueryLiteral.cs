// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.CosmosElementToQueryLiteral
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal sealed class CosmosElementToQueryLiteral : ICosmosElementVisitor
  {
    private readonly StringBuilder stringBuilder;
    private readonly CosmosElementToQueryLiteral.CosmosNumberToQueryLiteral cosmosNumberToQueryLiteral;

    public CosmosElementToQueryLiteral(StringBuilder stringBuilder)
    {
      this.stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof (stringBuilder));
      this.cosmosNumberToQueryLiteral = new CosmosElementToQueryLiteral.CosmosNumberToQueryLiteral(stringBuilder);
    }

    public void Visit(CosmosArray cosmosArray)
    {
      this.stringBuilder.Append("[");
      for (int index = 0; index < cosmosArray.Count; ++index)
      {
        if (index > 0)
          this.stringBuilder.Append(",");
        cosmosArray[index].Accept((ICosmosElementVisitor) this);
      }
      this.stringBuilder.Append("]");
    }

    public void Visit(CosmosBinary cosmosBinary)
    {
      StringBuilder stringBuilder = this.stringBuilder;
      ReadOnlyMemory<byte> readOnlyMemory = cosmosBinary.Value;
      byte[] array = readOnlyMemory.ToArray();
      readOnlyMemory = cosmosBinary.Value;
      int length = readOnlyMemory.Length;
      string hex = PartitionKeyInternal.HexConvert.ToHex(array, 0, length);
      stringBuilder.AppendFormat("C_Binary(\"0x{0}\")", (object) hex);
    }

    public void Visit(CosmosBoolean cosmosBoolean) => this.stringBuilder.Append(cosmosBoolean.Value ? "true" : "false");

    public void Visit(CosmosGuid cosmosGuid) => this.stringBuilder.AppendFormat("C_Guid(\"{0}\")", (object) cosmosGuid.Value);

    public void Visit(CosmosUndefined cosmosUndefined) => throw new ArgumentException("CosmosUndefined is not a legal argument.");

    public void Visit(CosmosNull cosmosNull) => this.stringBuilder.Append("null");

    public void Visit(CosmosNumber cosmosNumber) => cosmosNumber.Accept((ICosmosNumberVisitor) this.cosmosNumberToQueryLiteral);

    public void Visit(CosmosObject cosmosObject)
    {
      this.stringBuilder.Append("{");
      string str = string.Empty;
      foreach (KeyValuePair<string, CosmosElement> keyValuePair in cosmosObject)
      {
        this.stringBuilder.Append(str);
        str = ",";
        CosmosString.Create(keyValuePair.Key).Accept((ICosmosElementVisitor) this);
        this.stringBuilder.Append(":");
        keyValuePair.Value.Accept((ICosmosElementVisitor) this);
      }
      this.stringBuilder.Append("}");
    }

    public void Visit(CosmosString cosmosString) => this.stringBuilder.Append(JsonConvert.SerializeObject((object) cosmosString.Value.ToString(), DefaultJsonSerializationSettings.Value));

    private sealed class CosmosNumberToQueryLiteral : ICosmosNumberVisitor
    {
      private readonly StringBuilder stringBuilder;

      public CosmosNumberToQueryLiteral(StringBuilder stringBuilder) => this.stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof (stringBuilder));

      public void Visit(CosmosFloat32 cosmosFloat32) => this.stringBuilder.AppendFormat("C_Float32({0:G7})", (object) cosmosFloat32.GetValue());

      public void Visit(CosmosFloat64 cosmosFloat64) => this.stringBuilder.AppendFormat("C_Float64({0:R})", (object) cosmosFloat64.GetValue());

      public void Visit(CosmosInt16 cosmosInt16) => this.stringBuilder.AppendFormat("C_Int16({0})", (object) cosmosInt16.GetValue());

      public void Visit(CosmosInt32 cosmosInt32) => this.stringBuilder.AppendFormat("C_Int32({0})", (object) cosmosInt32.GetValue());

      public void Visit(CosmosInt64 cosmosInt64) => this.stringBuilder.AppendFormat("C_Int64({0})", (object) cosmosInt64.GetValue());

      public void Visit(CosmosInt8 cosmosInt8) => this.stringBuilder.AppendFormat("C_Int8({0})", (object) cosmosInt8.GetValue());

      public void Visit(CosmosNumber64 cosmosNumber64) => this.stringBuilder.Append((object) cosmosNumber64.GetValue());

      public void Visit(CosmosUInt32 cosmosUInt32) => this.stringBuilder.AppendFormat("C_UInt32({0})", (object) cosmosUInt32.GetValue());
    }
  }
}
