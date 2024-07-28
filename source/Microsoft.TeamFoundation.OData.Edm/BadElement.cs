// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadElement
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class BadElement : IEdmElement, IEdmCheckable, IEdmVocabularyAnnotatable
  {
    private readonly IEnumerable<EdmError> errors;

    public BadElement(IEnumerable<EdmError> errors) => this.errors = errors;

    public IEnumerable<EdmError> Errors => this.errors;
  }
}
