// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Conflict
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents
{
  public class Conflict : Resource
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
      if ((object) typeof (T) != (object) this.ResourceType)
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
      if ((object) typeof (T) != (object) this.ResourceType)
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
        if ((object) value == (object) typeof (Document))
          str = "document";
        else if ((object) value == (object) typeof (StoredProcedure))
          str = "storedProcedure";
        else if ((object) value == (object) typeof (Trigger))
          str = "trigger";
        else if ((object) value == (object) typeof (UserDefinedFunction))
        {
          str = "userDefinedFunction";
        }
        else
        {
          if ((object) value != (object) typeof (Attachment))
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported resource type {0}", (object) value.ToString()));
          str = "attachment";
        }
        this.SetValue("resourceType", (object) str);
      }
    }
  }
}
