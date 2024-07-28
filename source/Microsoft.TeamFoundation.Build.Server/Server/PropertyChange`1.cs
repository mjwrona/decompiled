// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.PropertyChange`1
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class PropertyChange<T>
  {
    public T OldValue { get; set; }

    public T NewValue { get; set; }
  }
}
