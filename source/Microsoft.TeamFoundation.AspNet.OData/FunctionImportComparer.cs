// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.FunctionImportComparer
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData
{
  internal class FunctionImportComparer : IEqualityComparer<IEdmFunctionImport>
  {
    public bool Equals(IEdmFunctionImport left, IEdmFunctionImport right)
    {
      if (left == right)
        return true;
      return left != null && right != null && left.Name == right.Name;
    }

    public int GetHashCode(IEdmFunctionImport functionImport) => functionImport == null || functionImport.Function.Name == null ? 0 : functionImport.Function.Name.GetHashCode();
  }
}
