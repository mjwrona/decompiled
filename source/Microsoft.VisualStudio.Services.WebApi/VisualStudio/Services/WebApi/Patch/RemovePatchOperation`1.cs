// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.RemovePatchOperation`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebApi.Patch
{
  public class RemovePatchOperation<TModel> : PatchOperation<TModel>
  {
    public RemovePatchOperation() => this.Operation = Operation.Remove;

    public RemovePatchOperation(string path)
      : this()
    {
      this.Path = path;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static PatchOperation<TModel> CreateFromJson(JsonPatchOperation operation)
    {
      PatchOperation<TModel>.ValidatePath(operation);
      if (!((IEnumerable<object>) typeof (TModel).GetCustomAttributes(false)).Where<object>((Func<object, bool>) (attrib => attrib is SkipValidateTypeForRemovePatchOperation)).Any<object>())
        PatchOperation<TModel>.ValidateAndGetType(operation);
      return operation.Value == null ? (PatchOperation<TModel>) new RemovePatchOperation<TModel>(operation.Path) : throw new VssPropertyValidationException("Value", PatchResources.ValueNotNull());
    }

    public override void Apply(TModel target) => this.Apply((object) target, (Action<Type, object, string>) ((type, parent, current) =>
    {
      if (type.IsList())
      {
        IList list = (IList) parent;
        int result;
        if (!int.TryParse(current, out result) || list.Count <= result)
          throw new PatchOperationFailedException(PatchResources.IndexOutOfRange((object) this.Path));
        list.RemoveAt(result);
      }
      else if (type.IsDictionary())
        ((IDictionary) parent).Remove((object) current);
      else
        type.SetMemberValue(current, parent, this.Value);
    }));
  }
}
