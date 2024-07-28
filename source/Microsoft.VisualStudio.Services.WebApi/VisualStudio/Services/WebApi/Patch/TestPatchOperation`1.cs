// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.TestPatchOperation`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections;

namespace Microsoft.VisualStudio.Services.WebApi.Patch
{
  public class TestPatchOperation<TModel> : PatchOperation<TModel>
  {
    public TestPatchOperation() => this.Operation = Operation.Test;

    public TestPatchOperation(string path, object value)
      : this()
    {
      this.Path = path;
      this.Value = value;
    }

    public new static PatchOperation<TModel> CreateFromJson(JsonPatchOperation operation)
    {
      PatchOperation<TModel>.ValidatePath(operation);
      return (PatchOperation<TModel>) new TestPatchOperation<TModel>(operation.Path, PatchOperation<TModel>.ValidateAndGetValue(operation));
    }

    public override void Apply(TModel target) => this.Apply((object) target, (Action<Type, object, string>) ((type, parent, current) =>
    {
      object objA;
      if (type.IsList())
      {
        IList list = (IList) parent;
        int result;
        if (!int.TryParse(current, out result) || list.Count <= result)
          throw new PatchOperationFailedException(PatchResources.IndexOutOfRange((object) this.Path));
        objA = list[result];
      }
      else if (type.IsDictionary())
      {
        IDictionary dictionary = (IDictionary) parent;
        objA = dictionary.Contains((object) current) ? dictionary[(object) current] : throw new InvalidPatchFieldNameException(PatchResources.InvalidFieldName((object) current));
      }
      else
        objA = type.GetMemberValue(current, parent);
      bool flag;
      switch (objA)
      {
        case null:
          flag = object.Equals(objA, this.Value);
          break;
        case IList _:
          throw new PatchOperationFailedException(PatchResources.TestNotImplementedForList());
        case IDictionary _:
          throw new PatchOperationFailedException(PatchResources.TestNotImplementedForDictionary());
        default:
          flag = !objA.GetType().IsAssignableOrConvertibleFrom(this.Value) ? objA.Equals(this.Value) : ConvertUtility.ChangeType(objA, objA.GetType()).Equals(ConvertUtility.ChangeType(this.Value, objA.GetType()));
          break;
      }
      if (!flag)
        throw new TestPatchOperationFailedException(PatchResources.TestFailed((object) this.Path, objA, this.Value));
    }));
  }
}
