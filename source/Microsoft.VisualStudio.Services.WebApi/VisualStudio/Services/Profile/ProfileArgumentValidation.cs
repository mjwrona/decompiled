// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Profile.ProfileArgumentValidation
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Profile
{
  public static class ProfileArgumentValidation
  {
    private const string Semicolon = ";";

    public static void ValidateAttributeName(string attributeName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(attributeName, nameof (attributeName));
      ArgumentUtility.CheckStringForInvalidCharacters(attributeName, nameof (attributeName));
      if (attributeName.Contains(";"))
        throw new ArgumentException("Attribute name cannot contain the character ';'", attributeName);
    }

    public static void ValidateContainerName(string containerName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(containerName, nameof (containerName));
      ArgumentUtility.CheckStringForInvalidCharacters(containerName, nameof (containerName));
      if (containerName.Contains(";"))
        throw new ArgumentException("Container name cannot contain the character ';'", containerName);
    }

    public static void ValidateApplicationContainerName(string containerName)
    {
      ProfileArgumentValidation.ValidateContainerName(containerName);
      if (VssStringComparer.AttributesDescriptor.Compare(containerName, "Core") == 0)
        throw new ArgumentException(string.Format("The container name '{0}' is reserved. Please specify a valid application container name", (object) "Core"), nameof (containerName));
    }
  }
}
