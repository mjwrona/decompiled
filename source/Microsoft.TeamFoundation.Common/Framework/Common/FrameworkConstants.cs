// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.FrameworkConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class FrameworkConstants
  {
    public const string ResourceNameField = "resourcename";
    public const string HashField = "hash";
    public const string ContentField = "content";
    public const string LengthField = "filelength";
    public const string RangeField = "range";
    public const string ExceptionHeader = "X-TeamFoundation-Exception";
    public const int DestroyedFileId = 1023;
  }
}
