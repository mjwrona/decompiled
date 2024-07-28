// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmOperationReturn
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  internal class EdmOperationReturn : 
    EdmElement,
    IEdmOperationReturn,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    public EdmOperationReturn(IEdmOperation declaringOperation, IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmOperation>(declaringOperation, nameof (declaringOperation));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      this.Type = type;
      this.DeclaringOperation = declaringOperation;
    }

    public IEdmTypeReference Type { get; private set; }

    public IEdmOperation DeclaringOperation { get; private set; }
  }
}
