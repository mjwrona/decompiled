// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.ArgumentValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  public class ArgumentValidator
  {
    private static readonly int c_maxPermissionLevelDefinitionNameSize = 256;
    private static readonly int c_maxPermissionLevelDefinitionDescriptionSize = 1024;

    public static void ValidatePermissionLevelDefinitionName(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new PermissionLevelBadRequestException(TFCommonResources.EmptyStringNotAllowed());
      if (name.Length == 0 || name.Length > ArgumentValidator.c_maxPermissionLevelDefinitionNameSize || name.EndsWith(".") || name.IndexOfAny(SidIdentityHelper.IllegalNameChars) >= 0 || ArgumentUtility.IsInvalidString(name))
        throw new PermissionLevelBadRequestException("Invalid PermissionLevelDefinition Name: $" + name + ".");
    }

    public static void ValidatePermissionLevelDefinitionDescription(string description)
    {
      if (string.IsNullOrEmpty(description))
        return;
      description = description.Trim();
      if (description.Length > ArgumentValidator.c_maxPermissionLevelDefinitionDescriptionSize || ArgumentUtility.IsInvalidString(description, true, true))
        throw new PermissionLevelBadRequestException(string.Format("Permission level definition cannot have a Description with length > ${0}. Provided Description parameter: ${1}.", (object) ArgumentValidator.c_maxPermissionLevelDefinitionDescriptionSize, (object) description));
    }

    public static void ValidatePermissionLevelDefinitionType(PermissionLevelDefinitionType type)
    {
      if ((PermissionLevelDefinitionType.All & type) != type)
        throw new PermissionLevelBadRequestException(string.Format("Invalid input value for the PermissionLevelDefinitionType Enum flag: {0}", (object) type));
      if (type == PermissionLevelDefinitionType.RestrictedVisibility)
        throw new PermissionLevelBadRequestException(string.Format("Cannot query/create PermissionLevelDefinition with/by PermissionLevelDefinitionType: {0}. This PermissionLevelDefinitionType flag needs to be used in combination with at-least one or more PermissionLevelDefinitionType flags.", (object) type));
    }
  }
}
