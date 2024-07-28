// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.ReplacePatchOperation`1
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
  public class ReplacePatchOperation<TModel> : PatchOperation<TModel>
  {
    public ReplacePatchOperation() => this.Operation = Operation.Replace;

    public ReplacePatchOperation(string path, object value)
      : this()
    {
      this.Path = path;
      this.Value = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public new static PatchOperation<TModel> CreateFromJson(JsonPatchOperation operation)
    {
      PatchOperation<TModel>.ValidatePath(operation);
      return (PatchOperation<TModel>) new ReplacePatchOperation<TModel>(operation.Path, PatchOperation<TModel>.ValidateAndGetValue(operation) ?? throw new VssPropertyValidationException("Value", PatchResources.ValueCannotBeNull()));
    }

    public override void Apply(TModel target) => this.Apply((object) target, (Action<Type, object, string>) ((type, parent, current) =>
    {
      if (type.IsList())
      {
        IList list = (IList) parent;
        int result;
        if (!int.TryParse(current, out result) || list.Count <= result)
          throw new PatchOperationFailedException(PatchResources.CannotReplaceNonExistantValue((object) this.Path));
        list[result] = this.Value;
      }
      else if (type.IsDictionary())
      {
        IDictionary dictionary = (IDictionary) parent;
        if (!dictionary.Contains((object) current))
          throw new InvalidPatchFieldNameException(PatchResources.InvalidFieldName((object) current));
        dictionary[(object) current] = this.Value;
      }
      else
      {
        if (type.GetMemberValue(current, parent) == null)
          throw new PatchOperationFailedException(PatchResources.CannotReplaceNonExistantValue((object) this.Path));
        type.SetMemberValue(current, parent, this.Value);
      }
    }));
  }
}
