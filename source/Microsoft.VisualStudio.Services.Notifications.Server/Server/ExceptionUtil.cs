// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ExceptionUtil
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class ExceptionUtil
  {
    public static string FormatException(Exception e, int stackFrames = 6, bool recurse = true)
    {
      StringBuilder stringBuilder = new StringBuilder();
      StackTrace stackTrace = new StackTrace(e, true);
      stringBuilder.AppendFormat("{0}:{1}", (object) e.GetType().Name, (object) e.Message);
      stringBuilder.AppendLine();
      for (int index = 0; stackFrames > 0 && index < stackTrace.FrameCount; ++index)
      {
        StackFrame frame = stackTrace.GetFrame(index);
        MethodBase method = frame.GetMethod();
        if ((MethodBase) null != method)
        {
          stringBuilder.AppendFormat("@{0}.{1}:{2}", (object) method.DeclaringType?.Name, (object) method.Name, (object) frame.GetFileLineNumber());
          stringBuilder.AppendLine();
        }
        --stackFrames;
      }
      if (recurse && e.InnerException != null)
      {
        stringBuilder.AppendLine("+Inner:");
        stringBuilder.AppendLine(ExceptionUtil.FormatException(e.InnerException, stackFrames, false));
      }
      return stringBuilder.ToString();
    }
  }
}
