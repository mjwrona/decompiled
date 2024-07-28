// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Proxy.Client.Helper
// Assembly: Microsoft.TeamFoundation.VersionControl.Server.Proxy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3F3DC329-13F2-42E8-9562-94C7348523BA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.Proxy.dll

using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.VersionControl.Server.Proxy.Client
{
  internal static class Helper
  {
    internal static string ArrayToString<T>(T[] array)
    {
      if (array == null)
        return "<null>";
      int num = Math.Min(array.Length, 100);
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      stringBuilder.Append(array.Length);
      stringBuilder.Append("]");
      for (int index = 0; index < num; ++index)
      {
        stringBuilder.Append((object) array[index]);
        if (index != num - 1)
          stringBuilder.Append(", ");
      }
      if (num < array.Length)
        stringBuilder.Append(", ...");
      return stringBuilder.ToString();
    }

    internal static void StringToXmlElement(XmlWriter writer, string element, string str)
    {
      if (str == null)
        return;
      try
      {
        writer.WriteElementString(element, str);
      }
      catch (ArgumentException ex)
      {
        throw new TeamFoundationServiceException(CommonResources.StringContainsIllegalChars());
      }
    }
  }
}
