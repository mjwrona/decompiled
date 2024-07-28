// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.AddPatchOperation`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi.Patch
{
  public class AddPatchOperation<TModel> : PatchOperation<TModel>
  {
    public AddPatchOperation() => this.Operation = Operation.Add;

    public AddPatchOperation(string path, object value)
      : this()
    {
      this.Path = path;
      this.Value = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static PatchOperation<TModel> CreateFromJson(JsonPatchOperation operation)
    {
      PatchOperation<TModel>.ValidatePath(operation);
      return (PatchOperation<TModel>) new AddPatchOperation<TModel>(operation.Path, PatchOperation<TModel>.ValidateAndGetValue(operation) ?? throw new VssPropertyValidationException("Value", PatchResources.ValueCannotBeNull()));
    }

    public override void Apply(TModel target) => this.Apply((object) target, (Action<Type, object, string>) ((type, parent, current) =>
    {
      if (string.IsNullOrEmpty(current))
        parent = this.Value;
      else if (type.IsList())
      {
        IList list = (IList) parent;
        if (current == "-")
        {
          list.Add(this.Value);
        }
        else
        {
          int result;
          if (!int.TryParse(current, out result) || list.Count < result)
            throw new PatchOperationFailedException(PatchResources.IndexOutOfRange((object) this.Path));
          list.Insert(result, this.Value);
        }
      }
      else if (type.IsDictionary())
        ((IDictionary) parent)[(object) current] = this.Value;
      else
        type.SetMemberValue(current, parent, this.Value);
    }));
  }
}
