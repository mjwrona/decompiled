// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.UnqualifiedCallAndEnumPrefixFreeResolver
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData
{
  public class UnqualifiedCallAndEnumPrefixFreeResolver : ODataUriResolver
  {
    private readonly StringAsEnumResolver _stringAsEnum = new StringAsEnumResolver();
    private readonly UnqualifiedODataUriResolver _unqualified = new UnqualifiedODataUriResolver();
    private bool _enableCaseInsensitive;

    public override bool EnableCaseInsensitive
    {
      get => this._enableCaseInsensitive;
      set
      {
        this._enableCaseInsensitive = value;
        this._stringAsEnum.EnableCaseInsensitive = this._enableCaseInsensitive;
        this._unqualified.EnableCaseInsensitive = this._enableCaseInsensitive;
      }
    }

    public override IEnumerable<IEdmOperation> ResolveUnboundOperations(
      IEdmModel model,
      string identifier)
    {
      return this._unqualified.ResolveUnboundOperations(model, identifier);
    }

    public override IEnumerable<IEdmOperation> ResolveBoundOperations(
      IEdmModel model,
      string identifier,
      IEdmType bindingType)
    {
      return this._unqualified.ResolveBoundOperations(model, identifier, bindingType);
    }

    public override void PromoteBinaryOperandTypes(
      BinaryOperatorKind binaryOperatorKind,
      ref SingleValueNode leftNode,
      ref SingleValueNode rightNode,
      out IEdmTypeReference typeReference)
    {
      this._stringAsEnum.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
    }

    public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
      IEdmEntityType type,
      IDictionary<string, string> namedValues,
      Func<IEdmTypeReference, string, object> convertFunc)
    {
      return this._stringAsEnum.ResolveKeys(type, namedValues, convertFunc);
    }

    public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(
      IEdmEntityType type,
      IList<string> positionalValues,
      Func<IEdmTypeReference, string, object> convertFunc)
    {
      return this._stringAsEnum.ResolveKeys(type, positionalValues, convertFunc);
    }

    public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(
      IEdmOperation operation,
      IDictionary<string, SingleValueNode> input)
    {
      return this._stringAsEnum.ResolveOperationParameters(operation, input);
    }
  }
}
