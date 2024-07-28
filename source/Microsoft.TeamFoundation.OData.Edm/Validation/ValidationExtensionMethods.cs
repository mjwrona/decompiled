// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.ValidationExtensionMethods
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Validation
{
  public static class ValidationExtensionMethods
  {
    public static bool IsBad(this IEdmElement element)
    {
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      return element.Errors().FirstOrDefault<EdmError>() != null;
    }

    public static IEnumerable<EdmError> Errors(this IEdmElement element)
    {
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      return InterfaceValidator.GetStructuralErrors(element);
    }

    public static IEnumerable<EdmError> TypeErrors(this IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      return InterfaceValidator.GetStructuralErrors((IEdmElement) type).Concat<EdmError>(InterfaceValidator.GetStructuralErrors((IEdmElement) type.Definition));
    }
  }
}
