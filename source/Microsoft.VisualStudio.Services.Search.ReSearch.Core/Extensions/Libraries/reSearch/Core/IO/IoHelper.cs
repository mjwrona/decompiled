// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO.IoHelper
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Index;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.IO
{
  public static class IoHelper
  {
    public static readonly bool CheckedIoEnabled = false;
    private static Dictionary<uint, string> s_markers = new Dictionary<uint, string>();

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    private static uint CreateMarker(string name)
    {
      uint hashCode = (uint) name.GetHashCode();
      if (IoHelper.s_markers.ContainsKey(hashCode))
        throw new IndexException("Multiple markers with identical id.");
      IoHelper.s_markers.Add(hashCode, name);
      return hashCode;
    }

    private static string GetMarkerName(uint marker)
    {
      string markerName;
      if (!IoHelper.s_markers.TryGetValue(marker, out markerName))
        markerName = "No Marker";
      return markerName;
    }

    public static void CheckType(IReader reader, uint marker)
    {
      if (!IoHelper.CheckedIoEnabled)
        return;
      uint marker1 = reader.ReadUInt32();
      if ((int) marker != (int) marker1)
        throw new IndexException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Expected {0} but found {1}.", (object) IoHelper.GetMarkerName(marker), (object) IoHelper.GetMarkerName(marker1)));
    }

    public static void WriteType(IWriter writer, uint marker)
    {
      if (!IoHelper.CheckedIoEnabled)
        return;
      writer.WriteUInt32(marker);
    }
  }
}
