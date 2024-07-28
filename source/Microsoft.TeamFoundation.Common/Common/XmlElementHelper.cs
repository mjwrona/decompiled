// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.XmlElementHelper
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("Use Microsoft.TeamFoundation.Services.Common.Internal.XmlElementWriterUtility instead.")]
  public class XmlElementHelper : XmlElementWriterUtility
  {
    public XmlElementHelper(string elementName, XmlWriter xmlWriter)
      : base(elementName, xmlWriter)
    {
    }
  }
}
