// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Conflict
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal class Conflict : Resource
  {
    public string SourceResourceId
    {
      get => this.GetValue<string>("resourceId");
      internal set => this.SetValue("resourceId", (object) value);
    }

    internal long ConflictLSN
    {
      get => this.GetValue<long>("conflict_lsn");
      set => this.SetValue("conflict_lsn", (object) value);
    }

    public T GetResource<T>() where T : Resource, new()
    {
      if (typeof (T) != this.ResourceType)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidResourceType, (object) typeof (T).Name, (object) this.ResourceType.Name));
      string str = this.GetValue<string>("content");
      if (string.IsNullOrEmpty(str))
        return default (T);
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream))
        {
          streamWriter.Write(str);
          streamWriter.Flush();
          memoryStream.Position = 0L;
          return JsonSerializable.LoadFrom<T>((Stream) memoryStream);
        }
      }
    }

    internal void SetResource<T>(T baseResource) where T : Resource, new()
    {
      if (typeof (T) != this.ResourceType)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidResourceType, (object) typeof (T).Name, (object) this.ResourceType.Name));
      StringBuilder stringBuilder = new StringBuilder();
      baseResource.SaveTo(stringBuilder, SerializationFormattingPolicy.None);
      string str = stringBuilder.ToString();
      if (!string.IsNullOrEmpty(str))
        this.SetValue("content", (object) str);
      this.Id = baseResource.Id;
      this.ResourceId = baseResource.ResourceId;
    }

    public OperationKind OperationKind
    {
      get
      {
        string b = this.GetValue<string>("operationType");
        if (string.Equals("create", b, StringComparison.OrdinalIgnoreCase))
          return OperationKind.Create;
        if (string.Equals("replace", b, StringComparison.OrdinalIgnoreCase) || string.Equals("patch", b, StringComparison.OrdinalIgnoreCase))
          return OperationKind.Replace;
        return string.Equals("delete", b, StringComparison.OrdinalIgnoreCase) ? OperationKind.Delete : OperationKind.Invalid;
      }
      internal set
      {
        string str;
        switch (value)
        {
          case OperationKind.Create:
            str = "create";
            break;
          case OperationKind.Replace:
            str = "replace";
            break;
          case OperationKind.Delete:
            str = "delete";
            break;
          default:
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported operation kind {0}", (object) value.ToString()));
        }
        this.SetValue("operationType", (object) str);
      }
    }

    public Type ResourceType
    {
      get
      {
        string b = this.GetValue<string>("resourceType");
        if (string.Equals("document", b, StringComparison.OrdinalIgnoreCase))
          return typeof (Document);
        if (string.Equals("storedProcedure", b, StringComparison.OrdinalIgnoreCase))
          return typeof (StoredProcedure);
        if (string.Equals("trigger", b, StringComparison.OrdinalIgnoreCase))
          return typeof (Trigger);
        if (string.Equals("userDefinedFunction", b, StringComparison.OrdinalIgnoreCase))
          return typeof (UserDefinedFunction);
        return string.Equals("attachment", b, StringComparison.OrdinalIgnoreCase) ? typeof (Attachment) : (Type) null;
      }
      internal set
      {
        string str;
        if (value == typeof (Document))
          str = "document";
        else if (value == typeof (StoredProcedure))
          str = "storedProcedure";
        else if (value == typeof (Trigger))
          str = "trigger";
        else if (value == typeof (UserDefinedFunction))
        {
          str = "userDefinedFunction";
        }
        else
        {
          if (!(value == typeof (Attachment)))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported resource type {0}", (object) value.ToString()));
          str = "attachment";
        }
        this.SetValue("resourceType", (object) str);
      }
    }
  }
}
