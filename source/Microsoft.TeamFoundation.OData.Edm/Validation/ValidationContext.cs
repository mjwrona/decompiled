// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ValidationContext
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm.Validation
{
  public sealed class ValidationContext
  {
    private readonly List<EdmError> errors = new List<EdmError>();
    private readonly IEdmModel model;
    private readonly Func<object, bool> isBad;

    internal ValidationContext(IEdmModel model, Func<object, bool> isBad)
    {
      this.model = model;
      this.isBad = isBad;
    }

    public IEdmModel Model => this.model;

    internal IEnumerable<EdmError> Errors => (IEnumerable<EdmError>) this.errors;

    public bool IsBad(IEdmElement element) => this.isBad((object) element);

    public void AddError(EdmLocation location, EdmErrorCode errorCode, string errorMessage) => this.AddError(new EdmError(location, errorCode, errorMessage));

    public void AddError(EdmError error) => this.errors.Add(error);
  }
}
