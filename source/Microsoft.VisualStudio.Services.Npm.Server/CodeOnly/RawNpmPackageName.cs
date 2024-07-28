// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.RawNpmPackageName
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class RawNpmPackageName
  {
    public static string Create(string packageScope, string unscopedName)
    {
      unscopedName.ThrowIfNullOrEmpty((Func<Exception>) (() => (Exception) new ArgumentNullException(nameof (unscopedName))));
      return packageScope.IsNullOrEmpty<char>() ? unscopedName : "@" + packageScope + "/" + unscopedName;
    }
  }
}
