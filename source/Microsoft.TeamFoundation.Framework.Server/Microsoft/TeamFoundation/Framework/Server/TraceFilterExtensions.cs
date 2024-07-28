// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceFilterExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class TraceFilterExtensions
  {
    public static bool IsMatch(this Microsoft.VisualStudio.Services.WebApi.TraceFilter tf, ref TraceEvent traceSet) => tf.IsEnabled && (tf.Level == TraceLevel.Off || tf.Level >= traceSet.Level) && (tf.Tracepoint == 0 || tf.Tracepoint == traceSet.Tracepoint) && (tf.ServiceHost == Guid.Empty || tf.ServiceHost == traceSet.ServiceHost) && TraceFilterExtensions.IsMatch(tf.ProcessName, traceSet.ProcessName) && TraceFilterExtensions.IsMatch(tf.UserLogin, traceSet.UserLogin) && TraceFilterExtensions.IsMatch(tf.Service, traceSet.Service) && TraceFilterExtensions.IsMatch(tf.Method, traceSet.Method) && TraceFilterExtensions.IsMatch(tf.Area, traceSet.Area) && TraceFilterExtensions.IsMatch(tf.Layer, traceSet.Layer) && TraceFilterExtensions.IsMatch(tf.UserAgent, traceSet.UserAgent) && TraceFilterExtensions.IsMatch(tf.Uri, traceSet.Uri) && TraceFilterExtensions.IsMatch(tf.Path, traceSet.Path) && TraceFilterExtensions.IsMatch(tf.ExceptionType, traceSet.ExceptionType) && TraceFilterExtensions.AnyMatch(tf.Tags, traceSet.Tags);

    private static bool IsMatch(string reference, string comparand) => string.IsNullOrEmpty(reference) || string.Equals(reference, comparand, StringComparison.OrdinalIgnoreCase);

    private static bool AnyMatch(string[] referenceTags, string[] comparandTags)
    {
      if (referenceTags == null || referenceTags.Length == 0)
        return true;
      if (comparandTags == null || comparandTags.Length == 0)
        return false;
      for (int index = 0; index < comparandTags.Length; ++index)
      {
        if (Array.BinarySearch<string>(referenceTags, comparandTags[index]) >= 0)
          return true;
      }
      return false;
    }
  }
}
