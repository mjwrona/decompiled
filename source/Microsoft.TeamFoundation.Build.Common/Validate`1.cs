// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.Validate`1
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public delegate void Validate<T>(
    string argumentName,
    T obj,
    bool allowNull,
    string errorMessage);
}
