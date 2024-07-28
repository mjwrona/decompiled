// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataRoutePrefixAttribute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;

namespace Microsoft.AspNet.OData.Routing
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public sealed class ODataRoutePrefixAttribute : Attribute
  {
    public ODataRoutePrefixAttribute(string prefix) => this.Prefix = !string.IsNullOrEmpty(prefix) ? prefix : throw Error.ArgumentNullOrEmpty(nameof (prefix));

    public string Prefix { get; private set; }
  }
}
