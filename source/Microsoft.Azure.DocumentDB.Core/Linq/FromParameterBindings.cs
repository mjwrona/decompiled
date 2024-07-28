// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.FromParameterBindings
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
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
        if (isInCollection && collection == null)
          throw new ArgumentNullException("collection cannot be null for in-collection parameter.");
      }
    }
  }
}
