// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.ParameterSubstitution
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal sealed class ParameterSubstitution
  {
    private Dictionary<ParameterExpression, Expression> substitutionTable;
    public const string InputParameterName = "root";

    public ParameterSubstitution() => this.substitutionTable = new Dictionary<ParameterExpression, Expression>();

    public void AddSubstitution(ParameterExpression parameter, Expression expression)
    {
      if (parameter == expression)
        throw new InvalidOperationException("Substitution with self attempted");
      this.substitutionTable.Add(parameter, expression);
    }

    public Expression Lookup(ParameterExpression parameter) => this.substitutionTable.ContainsKey(parameter) ? this.substitutionTable[parameter] : (Expression) null;
  }
}
