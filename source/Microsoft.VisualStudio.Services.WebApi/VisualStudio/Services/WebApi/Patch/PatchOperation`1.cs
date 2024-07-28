// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.PatchOperation`1
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebApi.Patch
{
  public abstract class PatchOperation<TModel> : 
    IPatchOperation<TModel>,
    IPatchOperationApplied,
    IPatchOperationApplying
  {
    public const string EndOfIndex = "-";
    public const string PathSeparator = "/";
    private static JsonSerializer serializer = new JsonSerializer();
    private IEnumerable<string> evaluatedPath;

    static PatchOperation() => PatchOperation<TModel>.serializer.Converters.Add((JsonConverter) new ObjectDictionaryConverter());

    public event PatchOperationApplyingEventHandler PatchOperationApplying;

    public event PatchOperationAppliedEventHandler PatchOperationApplied;

    public Operation Operation { get; protected set; }

    public string Path { get; protected set; }

    public IEnumerable<string> EvaluatedPath
    {
      get
      {
        if (this.evaluatedPath == null && this.Path != null)
          this.evaluatedPath = PatchOperation<TModel>.SplitPath(this.Path);
        return this.evaluatedPath;
      }
    }

    public string From { get; protected set; }

    public object Value { get; protected set; }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract void Apply(TModel target);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static PatchOperation<TModel> CreateFromJson(JsonPatchOperation operation)
    {
      if (operation == null)
        throw new VssPropertyValidationException("Operation", PatchResources.InvalidOperation());
      switch (operation.Operation)
      {
        case Operation.Add:
          return AddPatchOperation<TModel>.CreateFromJson(operation);
        case Operation.Remove:
          return RemovePatchOperation<TModel>.CreateFromJson(operation);
        case Operation.Replace:
          return ReplacePatchOperation<TModel>.CreateFromJson(operation);
        case Operation.Test:
          return TestPatchOperation<TModel>.CreateFromJson(operation);
        default:
          throw new PatchOperationFailedException(PatchResources.MoveCopyNotImplemented());
      }
    }

    protected static void ValidatePath(JsonPatchOperation operation)
    {
      if (operation.Path == null)
        throw new VssPropertyValidationException("Path", PatchResources.PathCannotBeNull());
      if (!operation.Path.StartsWith("/") && !string.IsNullOrEmpty(operation.Path))
        throw new VssPropertyValidationException("Path", PatchResources.PathInvalidStartValue());
      if (operation.Path.EndsWith("/"))
        throw new VssPropertyValidationException("Path", PatchResources.PathInvalidEndValue());
      if (operation.Operation != Operation.Add && operation.Path.EndsWith("-"))
        throw new VssPropertyValidationException("Path", PatchResources.InsertNotSupported((object) operation.Operation));
    }

    protected static void ValidateType(JsonPatchOperation operation) => PatchOperation<TModel>.ValidateAndGetType(operation);

    protected static Type ValidateAndGetType(JsonPatchOperation operation)
    {
      Type type = PatchOperation<TModel>.GetType(typeof (TModel), operation.Path);
      return !(type == (Type) null) ? type : throw new VssPropertyValidationException("Path", PatchResources.UnableToEvaluatePath((object) operation.Path));
    }

    protected static object ValidateAndGetValue(JsonPatchOperation operation)
    {
      Type type = PatchOperation<TModel>.ValidateAndGetType(operation);
      return operation.Value != null ? PatchOperation<TModel>.DeserializeValue(type, operation.Value) : (object) null;
    }

    private static Type GetType(Type type, string path) => PatchOperation<TModel>.GetType(type, PatchOperation<TModel>.SplitPath(path));

    private static Type GetType(Type type, IEnumerable<string> path)
    {
      string name = path.First<string>();
      Type type1 = !string.IsNullOrEmpty(name) ? (!type.IsList() ? (!type.IsDictionary() ? type.GetMemberType(name) : type.GenericTypeArguments[1]) : type.GenericTypeArguments[0]) : type;
      if (type1 == (Type) null)
        return (Type) null;
      return path.Count<string>() == 1 ? type1 : PatchOperation<TModel>.GetType(type1, path.Skip<string>(1));
    }

    private static object DeserializeValue(Type type, object jsonValue)
    {
      if (jsonValue is JToken)
      {
        try
        {
          return ((JToken) jsonValue).ToObject(type, PatchOperation<TModel>.serializer);
        }
        catch (JsonException ex)
        {
          throw new VssPropertyValidationException("Value", PatchResources.InvalidValue(jsonValue, (object) type), (Exception) ex);
        }
      }
      else
      {
        if (type.IsAssignableOrConvertibleFrom(jsonValue))
          return ConvertUtility.ChangeType(jsonValue, type);
        Guid result;
        if (Guid.TryParse(jsonValue as string, out result))
          return (object) result;
        throw new VssPropertyValidationException("Value", PatchResources.InvalidValue(jsonValue, (object) type));
      }
    }

    private static IEnumerable<string> SplitPath(string path) => (IEnumerable<string>) path.Split(new string[1]
    {
      "/"
    }, StringSplitOptions.None);

    protected void Apply(object target, Action<Type, object, string> actionToApply) => this.Apply(target, this.EvaluatedPath, actionToApply);

    private void Apply(
      object target,
      IEnumerable<string> path,
      Action<Type, object, string> actionToApply)
    {
      string str = path.First<string>();
      Type type = target.GetType();
      if (path.Count<string>() == 1)
      {
        if (this.PatchOperationApplying != null)
          this.PatchOperationApplying((object) this, new PatchOperationApplyingEventArgs(this.EvaluatedPath, this.Operation));
        actionToApply(type, target, str);
        if (this.PatchOperationApplied == null)
          return;
        this.PatchOperationApplied((object) this, new PatchOperationAppliedEventArgs(this.EvaluatedPath, this.Operation));
      }
      else
      {
        object target1 = (object) null;
        if (string.IsNullOrEmpty(str))
          target1 = target;
        else if (type.IsDictionary())
        {
          IDictionary dictionary = (IDictionary) target;
          if (dictionary.Contains((object) str))
            target1 = dictionary[(object) str];
        }
        else if (type.IsList())
        {
          IList list = (IList) target;
          int result;
          if (int.TryParse(str, out result) && list.Count > result)
            target1 = ((IList) target)[result];
        }
        else
          target1 = type.GetMemberValue(str, target);
        if (target1 == null)
          throw new PatchOperationFailedException(PatchResources.TargetCannotBeNull());
        this.Apply(target1, path.Skip<string>(1), actionToApply);
      }
    }
  }
}
