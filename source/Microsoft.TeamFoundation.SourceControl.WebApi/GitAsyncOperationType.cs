// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitAsyncOperationType
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum GitAsyncOperationType : byte
  {
    CherryPick = 1,
    Revert = 2,
    Import = 3,
    Fork = 4,
    Merge = 5,
  }
}
