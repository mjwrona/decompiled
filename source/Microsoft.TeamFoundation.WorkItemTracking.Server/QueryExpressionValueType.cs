// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpressionValueType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public enum QueryExpressionValueType
  {
    IdentityGuid = -3, // 0xFFFFFFFD
    Array = -2, // 0xFFFFFFFE
    Column = -1, // 0xFFFFFFFF
    String = 16, // 0x00000010
    Number = 32, // 0x00000020
    DateTime = 48, // 0x00000030
    UniqueIdentifier = 208, // 0x000000D0
    Boolean = 224, // 0x000000E0
    Double = 240, // 0x000000F0
  }
}
