// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.XEventsConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class XEventsConstants
  {
    public static readonly string MainXEventSessionName = "vsts_xevents";
    public static readonly string ReadScaleOutXEventSessionName = "vsts_xevents_read_scale_out";
    public static readonly string ReadScaleOutXEventFilenameNoExtension = "xeventsReadScaleOut";
    public static readonly string MainXEventFilename = "xevents.xel";
    public static readonly string ReadScaleOutXEventFilename = XEventsConstants.ReadScaleOutXEventFilenameNoExtension + ".xel";
  }
}
