// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ViewStateKeyConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ViewStateKeyConstants
  {
    public static readonly string ViewStateValidationKeyPath = "/Configuration/Application/WebAccessSettings/ViewStateValidationKey";
    public static readonly string ViewStateDecryptionKeyPath = "/Configuration/Application/WebAccessSettings/ViewStateDecryptionKey";
    public static readonly string ViewStateDrawerName = "ViewState";
    public static readonly string ValidationKey = nameof (ValidationKey);
    public static readonly string DecryptionKey = nameof (DecryptionKey);
  }
}
