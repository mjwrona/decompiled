// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataResourceSet
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData
{
  public sealed class ODataResourceSet : ODataResourceSetBase
  {
    private List<ODataAction> actions = new List<ODataAction>();
    private List<ODataFunction> functions = new List<ODataFunction>();

    public IEnumerable<ODataAction> Actions => (IEnumerable<ODataAction>) this.actions;

    public IEnumerable<ODataFunction> Functions => (IEnumerable<ODataFunction>) this.functions;

    public void AddAction(ODataAction action)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataAction>(action, nameof (action));
      if (this.actions.Contains(action))
        return;
      this.actions.Add(action);
    }

    public void AddFunction(ODataFunction function)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataFunction>(function, nameof (function));
      if (this.functions.Contains(function))
        return;
      this.functions.Add(function);
    }
  }
}
