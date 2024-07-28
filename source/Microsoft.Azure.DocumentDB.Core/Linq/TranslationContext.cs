// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.TranslationContext
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal sealed class TranslationContext
  {
    public HashSet<ParameterExpression> InScope;
    public QueryUnderConstruction currentQuery;
    private ParameterSubstitution substitutions;
    private List<MethodCallExpression> methodStack;
    private List<ParameterExpression> lambdaParametersStack;
    private List<Collection> collectionStack;
    private Stack<TranslationContext.SubqueryBinding> subqueryBindingStack;

    public TranslationContext()
    {
      this.InScope = new HashSet<ParameterExpression>();
      this.substitutions = new ParameterSubstitution();
      this.methodStack = new List<MethodCallExpression>();
      this.lambdaParametersStack = new List<ParameterExpression>();
      this.collectionStack = new List<Collection>();
      this.currentQuery = new QueryUnderConstruction(this.GetGenerateNewParameterFunc());
      this.subqueryBindingStack = new Stack<TranslationContext.SubqueryBinding>();
    }

    public Expression LookupSubstitution(ParameterExpression parameter) => this.substitutions.Lookup(parameter);

    public ParameterExpression GenerateNewParamter(Type parameterType, string baseParameterName) => Utilities.NewParameter(baseParameterName, parameterType, this.InScope);

    public Func<string, ParameterExpression> GetGenerateNewParameterFunc() => (Func<string, ParameterExpression>) (paramName => this.GenerateNewParamter(typeof (object), paramName));

    public void PushParameter(ParameterExpression parameter, bool shouldBeOnNewQuery)
    {
      this.lambdaParametersStack.Add(parameter);
      Collection collection = this.collectionStack[this.collectionStack.Count - 1];
      if (collection.isOuter)
      {
        ParameterExpression parameterInContext = this.currentQuery.GetInputParameterInContext(shouldBeOnNewQuery);
        this.substitutions.AddSubstitution(parameter, (Expression) parameterInContext);
      }
      else
        this.currentQuery.Bind(parameter, collection.inner);
    }

    public void PopParameter() => this.lambdaParametersStack.RemoveAt(this.lambdaParametersStack.Count - 1);

    public void PushMethod(MethodCallExpression method)
    {
      if (method == null)
        throw new ArgumentNullException(nameof (method));
      this.methodStack.Add(method);
    }

    public void PopMethod() => this.methodStack.RemoveAt(this.methodStack.Count - 1);

    public MethodCallExpression PeekMethod() => this.methodStack.Count <= 0 ? (MethodCallExpression) null : this.methodStack[this.methodStack.Count - 1];

    public void PushCollection(Collection collection)
    {
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      this.collectionStack.Add(collection);
    }

    public void PopCollection() => this.collectionStack.RemoveAt(this.collectionStack.Count - 1);

    public ParameterExpression SetInputParameter(Type type, string name) => this.currentQuery.fromParameters.SetInputParameter(type, name, this.InScope);

    public void SetFromParameter(ParameterExpression parameter, SqlCollection collection) => this.currentQuery.fromParameters.Add(new FromParameterBindings.Binding(parameter, collection, true));

    public bool IsInMainBranchSelect()
    {
      if (this.methodStack.Count == 0)
        return false;
      bool flag = true;
      string str1 = this.methodStack[0].ToString();
      for (int index = 1; index < this.methodStack.Count; ++index)
      {
        string str2 = this.methodStack[index].ToString();
        if (!str1.StartsWith(str2, StringComparison.Ordinal))
        {
          flag = false;
          break;
        }
        str1 = str2;
      }
      string name = this.methodStack[this.methodStack.Count - 1].Method.Name;
      if (!flag)
        return false;
      return name.Equals("Select") || name.Equals("SelectMany");
    }

    public void PushSubqueryBinding(bool shouldBeOnNewQuery) => this.subqueryBindingStack.Push(new TranslationContext.SubqueryBinding(shouldBeOnNewQuery));

    public TranslationContext.SubqueryBinding PopSubqueryBinding() => this.subqueryBindingStack.Count != 0 ? this.subqueryBindingStack.Pop() : throw new InvalidOperationException("Unexpected empty subquery binding stack.");

    public TranslationContext.SubqueryBinding CurrentSubqueryBinding => this.subqueryBindingStack.Count != 0 ? this.subqueryBindingStack.Peek() : throw new InvalidOperationException("Unexpected empty subquery binding stack.");

    public QueryUnderConstruction PackageCurrentQueryIfNeccessary()
    {
      if (this.CurrentSubqueryBinding.ShouldBeOnNewQuery)
      {
        this.currentQuery = this.currentQuery.PackageQuery(this.InScope);
        this.CurrentSubqueryBinding.ShouldBeOnNewQuery = false;
      }
      return this.currentQuery;
    }

    public class SubqueryBinding
    {
      public static TranslationContext.SubqueryBinding EmptySubqueryBinding = new TranslationContext.SubqueryBinding(false);

      public bool ShouldBeOnNewQuery { get; set; }

      public List<FromParameterBindings.Binding> NewBindings { get; private set; }

      public SubqueryBinding(bool shouldBeOnNewQuery)
      {
        this.ShouldBeOnNewQuery = shouldBeOnNewQuery;
        this.NewBindings = new List<FromParameterBindings.Binding>();
      }

      public List<FromParameterBindings.Binding> TakeBindings()
      {
        List<FromParameterBindings.Binding> newBindings = this.NewBindings;
        this.NewBindings = new List<FromParameterBindings.Binding>();
        return newBindings;
      }
    }
  }
}
