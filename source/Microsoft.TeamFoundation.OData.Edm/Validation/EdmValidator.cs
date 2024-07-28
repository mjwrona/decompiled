// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Validation.EdmValidator
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Validation
{
  public static class EdmValidator
  {
    public static bool Validate(this IEdmModel root, out IEnumerable<EdmError> errors)
    {
      IEdmModel root1 = root;
      Version version = root.GetEdmVersion();
      if ((object) version == null)
        version = EdmConstants.EdmVersionDefault;
      ref IEnumerable<EdmError> local = ref errors;
      return root1.Validate(version, out local);
    }

    public static bool Validate(
      this IEdmModel root,
      Version version,
      out IEnumerable<EdmError> errors)
    {
      return root.Validate(ValidationRuleSet.GetEdmModelRuleSet(version), out errors);
    }

    public static bool Validate(
      this IEdmModel root,
      ValidationRuleSet ruleSet,
      out IEnumerable<EdmError> errors)
    {
      EdmUtil.CheckArgumentNull<IEdmModel>(root, nameof (root));
      EdmUtil.CheckArgumentNull<ValidationRuleSet>(ruleSet, nameof (ruleSet));
      errors = InterfaceValidator.ValidateModelStructureAndSemantics(root, ruleSet);
      return errors.FirstOrDefault<EdmError>() == null;
    }
  }
}
