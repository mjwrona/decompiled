// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.BindingPathHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  internal static class BindingPathHelper
  {
    public static string ConvertBindingPath(this IEnumerable<MemberInfo> bindingPath) => bindingPath != null ? string.Join("/", bindingPath.Select<MemberInfo, string>((Func<MemberInfo, string>) (e => TypeHelper.GetQualifiedName(e)))) : throw Error.ArgumentNull(nameof (bindingPath));
  }
}
