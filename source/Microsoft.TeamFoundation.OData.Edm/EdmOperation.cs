// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmOperation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public abstract class EdmOperation : 
    EdmNamedElement,
    IEdmOperation,
    IEdmSchemaElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmFullNamedElement
  {
    private readonly string fullName;
    private readonly List<IEdmOperationParameter> parameters = new List<IEdmOperationParameter>();

    protected EdmOperation(
      string namespaceName,
      string name,
      IEdmTypeReference returnType,
      bool isBound,
      IEdmPathExpression entitySetPathExpression)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<string>(namespaceName, nameof (namespaceName));
      this.Return = returnType == null ? (IEdmOperationReturn) null : (IEdmOperationReturn) new EdmOperationReturn((IEdmOperation) this, returnType);
      this.Namespace = namespaceName;
      this.IsBound = isBound;
      this.EntitySetPath = entitySetPathExpression;
      this.fullName = EdmUtil.GetFullNameForSchemaElement(namespaceName, this.Name);
    }

    protected EdmOperation(string namespaceName, string name, IEdmTypeReference returnType)
      : this(namespaceName, name, returnType, false, (IEdmPathExpression) null)
    {
    }

    public bool IsBound { get; private set; }

    public IEdmPathExpression EntitySetPath { get; private set; }

    public abstract EdmSchemaElementKind SchemaElementKind { get; }

    public string Namespace { get; private set; }

    public string FullName => this.fullName;

    public IEdmTypeReference ReturnType => this.Return != null ? this.Return.Type : (IEdmTypeReference) null;

    public IEnumerable<IEdmOperationParameter> Parameters => (IEnumerable<IEdmOperationParameter>) this.parameters;

    internal IEdmOperationReturn Return { get; private set; }

    public IEdmOperationParameter FindParameter(string name)
    {
      foreach (IEdmOperationParameter parameter in this.Parameters)
      {
        if (parameter.Name == name)
          return parameter;
      }
      return (IEdmOperationParameter) null;
    }

    public EdmOperationParameter AddParameter(string name, IEdmTypeReference type)
    {
      EdmOperationParameter operationParameter = new EdmOperationParameter((IEdmOperation) this, name, type);
      this.parameters.Add((IEdmOperationParameter) operationParameter);
      return operationParameter;
    }

    public EdmOptionalParameter AddOptionalParameter(string name, IEdmTypeReference type) => this.AddOptionalParameter(name, type, (string) null);

    public EdmOptionalParameter AddOptionalParameter(
      string name,
      IEdmTypeReference type,
      string defaultValue)
    {
      EdmOptionalParameter optionalParameter = new EdmOptionalParameter((IEdmOperation) this, name, type, defaultValue);
      this.parameters.Add((IEdmOperationParameter) optionalParameter);
      return optionalParameter;
    }

    public void AddParameter(IEdmOperationParameter parameter)
    {
      EdmUtil.CheckArgumentNull<IEdmOperationParameter>(parameter, nameof (parameter));
      this.parameters.Add(parameter);
    }
  }
}
