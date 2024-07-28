// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StreamingCollectionUnsupportedOperationException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class StreamingCollectionUnsupportedOperationException : TeamFoundationServiceException
  {
    public StreamingCollectionUnsupportedOperationException(
      System.Diagnostics.StackTrace constructor,
      List<System.Diagnostics.StackTrace> enumeratorCalls)
      : base(FrameworkResources.StreamingCollectionNotSupportedError() + StreamingCollectionUnsupportedOperationException.ToString(constructor, enumeratorCalls))
    {
      this.LogException = true;
      TeamFoundationTracingService.TraceRaw(7200, TraceLevel.Error, "StreamingCollection", "StreamingCollection", "StreamingCollection reset or enumerated multiple times.");
      if (constructor == null)
        return;
      TeamFoundationTracingService.TraceRaw(7201, TraceLevel.Error, "StreamingCollection", "StreamingCollection", "StreamingCollection reset or enumerated multiple times.{0}", (object) StreamingCollectionUnsupportedOperationException.ToString(constructor, enumeratorCalls));
    }

    private static string ToString(System.Diagnostics.StackTrace constructor, List<System.Diagnostics.StackTrace> enumeratorCalls)
    {
      if (constructor == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine();
      stringBuilder.AppendLine("*** Construction");
      stringBuilder.Append((object) constructor);
      stringBuilder.AppendLine();
      if (enumeratorCalls != null)
      {
        for (int index = 0; index < enumeratorCalls.Count; ++index)
        {
          stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "*** Call #{0}", (object) index));
          stringBuilder.Append((object) enumeratorCalls[index]);
          stringBuilder.AppendLine();
          stringBuilder.AppendLine();
        }
      }
      return stringBuilder.ToString();
    }
  }
}
