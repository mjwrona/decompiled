// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.FromParameterBindings
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class FromParameterBindings
  {
    private List<FromParameterBindings.Binding> ParameterDefinitions;

    public FromParameterBindings() => this.ParameterDefinitions = new List<FromParameterBindings.Binding>();

    public ParameterExpression SetInputParameter(
      Type parameterType,
      string parameterName,
      HashSet<ParameterExpression> inScope)
    {
      if (this.ParameterDefinitions.Count > 0)
        throw new InvalidOperationException("First parameter already set");
      ParameterExpression parameter = Expression.Parameter(parameterType, parameterName);
      inScope.Add(parameter);
      this.ParameterDefinitions.Add(new FromParameterBindings.Binding(parameter, (SqlCollection) null, false));
      return parameter;
    }

    public void Add(FromParameterBindings.Binding binding) => this.ParameterDefinitions.Add(binding);

    public IEnumerable<FromParameterBindings.Binding> GetBindings() => (IEnumerable<FromParameterBindings.Binding>) this.ParameterDefinitions;

    public ParameterExpression GetInputParameter()
    {
      int index = this.ParameterDefinitions.Count - 1;
      while (index > 0 && !this.ParameterDefinitions[index].IsInputParameter)
        --index;
      return index < 0 ? (ParameterExpression) null : this.ParameterDefinitions[index].Parameter;
    }

    public sealed class Binding
    {
      public ParameterExpression Parameter;
      public SqlCollection ParameterDefinition;
      public bool IsInCollection;
      public bool IsInputParameter;

      public Binding(
        ParameterExpression parameter,
        SqlCollection collection,
        bool isInCollection,
        bool isInputParameter = true)
      {
        this.ParameterDefinition = collection;
        this.Parameter = parameter;
        this.IsInCollection = isInCollection;
        this.IsInputParameter = isInputParameter;
        if (isInCollection && (SqlObject) collection == (SqlObject) null)
          throw new ArgumentNullException("collection cannot be null for in-collection parameter.");
      }
    }
  }
}
