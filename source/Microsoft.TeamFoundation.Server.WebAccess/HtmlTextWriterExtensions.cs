// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.HtmlTextWriterExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class HtmlTextWriterExtensions
  {
    public static void AddAttributeIfNotNullOrEmpty(
      this HtmlTextWriter writer,
      string name,
      string value)
    {
      writer.AddAttributeIfNotNullOrEmpty(name, value, false);
    }

    public static void AddAttributeIfNotNullOrEmpty(
      this HtmlTextWriter writer,
      string name,
      string value,
      bool encode)
    {
      if (string.IsNullOrEmpty(value))
        return;
      writer.AddAttribute(name, value, encode);
    }

    public static void AddAttributeIfNotFalse(this HtmlTextWriter writer, string name, bool value)
    {
      if (!value)
        return;
      writer.AddAttribute(name, "1");
    }

    public static void AddClassNames(this HtmlTextWriter writer, params string[] classNames) => writer.AddAttributeIfNotNullOrEmpty("class", string.Join(" ", ((IEnumerable<string>) classNames).Distinct<string>()));
  }
}
