// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.ODataUntypedActionParameters
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class ODataUntypedActionParameters : Dictionary<string, object>
  {
    public ODataUntypedActionParameters(IEdmAction action) => this.Action = action != null ? action : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (action));

    public IEdmAction Action { get; private set; }
  }
}
