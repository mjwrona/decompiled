// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.ParameterSubstitution
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class ParameterSubstitution
  {
    private Dictionary<ParameterExpression, Expression> substitutionTable;
    public const string InputParameterName = "root";

    public ParameterSubstitution() => this.substitutionTable = new Dictionary<ParameterExpression, Expression>();

    public void AddSubstitution(ParameterExpression parameter, Expression with)
    {
      if (parameter == with)
        throw new InvalidOperationException("Substitution with self attempted");
      this.substitutionTable.Add(parameter, with);
    }

    public Expression Lookup(ParameterExpression parameter) => this.substitutionTable.ContainsKey(parameter) ? this.substitutionTable[parameter] : (Expression) null;
  }
}
