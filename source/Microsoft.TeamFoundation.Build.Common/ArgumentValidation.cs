// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.ArgumentValidation
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ArgumentValidation
  {
    public static void Check(
      string argumentName,
      string argument,
      bool allowNullOrEmpty,
      string errorMessage)
    {
      if (!string.IsNullOrEmpty(argument) || allowNullOrEmpty)
        return;
      if (string.IsNullOrEmpty(argumentName))
        throw new ArgumentException(BuildTypeResource.InvalidInputNull());
      if (string.IsNullOrEmpty(errorMessage))
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) argumentName));
      throw new ArgumentException(errorMessage);
    }

    public static void Check(string argumentName, object argument, bool allowNull)
    {
      if (argument != null || allowNull)
        return;
      if (string.IsNullOrEmpty(argumentName))
        throw new ArgumentException(BuildTypeResource.InvalidInputNull());
      throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) argumentName));
    }

    public static void CheckArray<T>(
      string argumentName,
      IList<T> argument,
      Validate<T> validate,
      bool allowNull,
      string errorMessage)
    {
      if (argument == null)
      {
        if (!allowNull)
          throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) argumentName));
      }
      else
      {
        for (int index = 0; index < argument.Count; ++index)
        {
          try
          {
            validate(string.Empty, argument[index], allowNull, errorMessage);
          }
          catch (ArgumentException ex)
          {
            throw new ArgumentException(BuildTypeResource.InvalidInputAtIndex((object) argumentName, (object) index, (object) ex.Message));
          }
        }
      }
    }

    public static void CheckArray<T>(
      string argumentName,
      IList<T> argument,
      ValidateType<T> validate,
      string type,
      bool allowNull,
      string errorMessage)
    {
      if (argument == null)
      {
        if (!allowNull)
          throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) argumentName));
      }
      else
      {
        for (int index = 0; index < argument.Count; ++index)
        {
          try
          {
            validate(string.Empty, argument[index], type, allowNull, errorMessage);
          }
          catch (ArgumentException ex)
          {
            throw new ArgumentException(BuildTypeResource.InvalidInputAtIndex((object) argumentName, (object) index, (object) ex.Message));
          }
        }
      }
    }

    public static void CheckBound(string argumentName, int argument, int lowerBound) => ArgumentValidation.CheckBound(argumentName, argument, lowerBound, int.MaxValue);

    public static void CheckBound(
      string argumentName,
      int argument,
      int lowerBound,
      int upperBound)
    {
      if (argument < lowerBound || argument > upperBound)
        throw new ArgumentException(BuildTypeResource.InvalidInputParameterOutOfRange((object) argument, (object) argumentName));
    }

    public static void CheckUri(
      string argumentName,
      string argument,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.CheckUri(argumentName, argument, (string) null, allowNull, errorMessage);
    }

    public static void CheckUri(
      string argumentName,
      string argument,
      string type,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, errorMessage);
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidUri(argument, type);
    }

    public static void CheckBuildDirectory(
      string argumentName,
      ref string argument,
      bool allowNull)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, BuildTypeResource.MissingBuildDirectory());
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidBuildDirectory(ref argument);
    }

    public static void CheckBuildMachine(string argumentName, string argument, bool allowNull)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, BuildTypeResource.MissingBuildMachine());
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidMachineName(argument);
    }

    public static void CheckBuildNumber(string argumentName, string argument, bool allowNull)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, BuildTypeResource.MissingBuildNumber());
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidBuildNumber(argument);
    }

    public static void CheckBuildType(string argumentName, string argument, bool allowNull)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, BuildTypeResource.MissingBuildType());
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidBuildType(argument);
    }

    public static void CheckConfiguration(
      string argumentName,
      string argument,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, errorMessage);
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidConfigPlatform(argument);
    }

    public static void CheckDropLocation(
      string argumentName,
      ref string argument,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, errorMessage);
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidDropLocation(ref argument);
    }

    public static void CheckDropLocation(
      string argumentName,
      ref string argument,
      bool allowNull,
      bool allowVCDrop,
      string errorMessage)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, errorMessage);
      if (string.IsNullOrEmpty(argument))
        return;
      if (BuildContainerPath.IsServerPath(argument))
        Validation.CheckValidBuildContainerDropLocation(ref argument);
      else if (allowVCDrop)
        Validation.CheckValidDropLocation(ref argument);
      else
        Validation.CheckValidUncDropLocationNotServer(ref argument);
    }

    public static void CheckLogLocation(
      string argumentName,
      ref string argument,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, errorMessage);
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidLogLocation(ref argument);
    }

    public static void CheckItemPath(
      string argumentName,
      ref string argument,
      bool allowNull,
      bool allowWildcards)
    {
      ArgumentValidation.Check(argumentName, argument, allowNull, (string) null);
      if (string.IsNullOrEmpty(argument))
        return;
      Validation.CheckValidItemPath(ref argument, allowWildcards);
    }

    public static void CheckShelvesetName(string argumentName, string argument, bool allowNull)
    {
      ArgumentValidation.Check(argumentName, (object) argument, allowNull);
      if (string.IsNullOrEmpty(argument))
        return;
      string workspaceName;
      string workspaceOwner;
      WorkspaceSpec.Parse(argument, string.Empty, out workspaceName, out workspaceOwner);
      if (!WorkspaceSpec.IsLegalName(workspaceName))
        throw new ArgumentException(BuildTypeResource.InvalidShelvesetName((object) argumentName, (object) workspaceName));
      ArgumentValidation.Check(argumentName, workspaceOwner, false, BuildTypeResource.MissingShelvesetOwner((object) argumentName));
      if (!TFCommonUtil.IsLegalIdentity(workspaceOwner))
        throw new ArgumentException(BuildTypeResource.InvalidShelvesetOwner((object) argumentName, (object) workspaceOwner));
    }

    public static void CheckSharedResourceName(string argumentName, string argument)
    {
      ArgumentValidation.Check(argumentName, argument, false, (string) null);
      if (argument.Length > 256)
        throw new ArgumentException(BuildTypeResource.InvalidSharedResourceName((object) argumentName, (object) argument));
    }

    public static void CheckUriArray(
      string argumentName,
      IList<string> array,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.CheckArray<string>(argumentName, array, new Validate<string>(ArgumentValidation.CheckUri), allowNull, errorMessage);
    }

    public static void CheckUriArray(
      string argumentName,
      IList<string> array,
      string type,
      bool allowNull,
      string errorMessage)
    {
      ArgumentValidation.CheckArray<string>(argumentName, array, new ValidateType<string>(ArgumentValidation.CheckUri), type, allowNull, errorMessage);
    }
  }
}
