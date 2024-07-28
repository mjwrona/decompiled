// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.DerivedTypeValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData
{
  internal sealed class DerivedTypeValidator
  {
    private IEnumerable<string> derivedTypeConstraints;
    private IEdmType expectedType;
    private string resourceKind;
    private string resourceName;

    public DerivedTypeValidator(
      IEdmType expectedType,
      IEnumerable<string> derivedTypeConstraints,
      string resourceKind,
      string resourceName)
    {
      this.derivedTypeConstraints = derivedTypeConstraints;
      this.expectedType = expectedType;
      this.resourceKind = resourceKind;
      this.resourceName = resourceName;
    }

    internal void ValidateResourceType(IEdmType resourceType)
    {
      if (resourceType == null)
        return;
      this.ValidateResourceType(resourceType.FullTypeName());
    }

    internal void ValidateResourceType(string resourceTypeName)
    {
      if (resourceTypeName != null && (this.expectedType == null || !(this.expectedType.FullTypeName() == resourceTypeName)) && this.derivedTypeConstraints != null && !this.derivedTypeConstraints.Any<string>((Func<string, bool>) (c => c == resourceTypeName)))
        throw new ODataException(Strings.ReaderValidationUtils_ValueTypeNotAllowedInDerivedTypeConstraint((object) resourceTypeName, (object) this.resourceKind, (object) this.resourceName));
    }
  }
}
