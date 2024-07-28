// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.JsonDocumentFieldContainer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class JsonDocumentFieldContainer : DocumentFieldContainer
  {
    private static Dictionary<string, string> s_rmApprovalType = new Dictionary<string, string>()
    {
      {
        "1",
        "preDeploy"
      },
      {
        "2",
        "postDeploy"
      }
    };
    private static Dictionary<string, string> s_rmApprovalStatus = new Dictionary<string, string>()
    {
      {
        "2",
        "approved"
      },
      {
        "4",
        "rejected"
      }
    };
    private static Dictionary<string, string> s_rmEnvironmentStatus = new Dictionary<string, string>()
    {
      {
        "8",
        "canceled"
      },
      {
        "16",
        "rejected"
      },
      {
        "4",
        "succeeded"
      },
      {
        "128",
        "partiallySucceeded"
      }
    };
    private JObject m_jObject;
    private string m_jsonString;

    public JsonDocumentFieldContainer(string jsonString) => this.m_jsonString = jsonString;

    public JsonDocumentFieldContainer(JObject jObject) => this.m_jObject = jObject;

    internal JObject GetJObject()
    {
      if (this.m_jObject == null)
      {
        if (this.m_jsonString == null)
          return (JObject) null;
        this.m_jObject = JObject.Parse(this.m_jsonString);
      }
      return this.m_jObject;
    }

    public override string GetDocumentString()
    {
      if (this.m_jsonString == null)
      {
        if (this.m_jsonString != null || this.m_jObject == null)
          return (string) null;
        this.m_jsonString = this.m_jObject.ToString();
      }
      return this.m_jsonString;
    }

    public override IFieldContainer GetDynamicFieldContainer(DynamicFieldContainerType type) => type != DynamicFieldContainerType.Json ? (IFieldContainer) null : (IFieldContainer) this;

    internal override object GetFieldValueInternal(string fieldName)
    {
      if (this.GetJObject() == null)
        return (object) null;
      object fieldValueInternal = (object) null;
      IEnumerable<JToken> source1 = this.m_jObject.SelectTokens(fieldName, false);
      if (source1 == null || !source1.Any<JToken>())
        source1 = this.m_jObject.SelectTokens(this.ToggleCamelCase(fieldName), false);
      if (source1 != null)
      {
        IEnumerable<object> source2 = source1.Values<object>();
        if (source2.Count<object>() == 1)
        {
          fieldValueInternal = JsonDocumentFieldContainer.GetRmEnumFieldMappings(fieldName, source2.First<object>());
          if (!(fieldValueInternal is IEnumerable<string>))
            fieldValueInternal = (object) new object[1]
            {
              fieldValueInternal
            };
        }
        else
          fieldValueInternal = (object) source2;
      }
      return fieldValueInternal;
    }

    private string ToggleCamelCase(string fieldName)
    {
      char[] charArray = fieldName.ToCharArray();
      bool flag = true;
      for (int index = 0; index < charArray.Length; ++index)
      {
        char c = charArray[index];
        if (flag)
        {
          flag = false;
          if (char.IsLower(c))
            charArray[index] = char.ToUpper(c);
          else if (char.IsUpper(c))
            charArray[index] = char.ToLower(c);
        }
        else if (c == '.')
          flag = true;
      }
      return new string(charArray);
    }

    public override void AddOrUpdateNode(string name, string value) => throw new NotImplementedException();

    private static object GetRmEnumFieldMappings(string fieldName, object value)
    {
      if (value == null)
        return value;
      Dictionary<string, string> dictionary;
      if (fieldName.Equals("approval.approvalType", StringComparison.OrdinalIgnoreCase))
        dictionary = JsonDocumentFieldContainer.s_rmApprovalType;
      else if (fieldName.Equals("approval.status", StringComparison.OrdinalIgnoreCase))
      {
        dictionary = JsonDocumentFieldContainer.s_rmApprovalStatus;
      }
      else
      {
        if (!fieldName.Equals("environment.status", StringComparison.OrdinalIgnoreCase))
          return value;
        dictionary = JsonDocumentFieldContainer.s_rmEnvironmentStatus;
      }
      object enumFieldMappings = value;
      string str = value.ToString();
      foreach (KeyValuePair<string, string> keyValuePair in dictionary)
      {
        if (str.Equals(keyValuePair.Key) || str.Equals(keyValuePair.Value, StringComparison.OrdinalIgnoreCase))
        {
          enumFieldMappings = (object) new string[2]
          {
            keyValuePair.Key,
            keyValuePair.Value
          };
          break;
        }
      }
      return enumFieldMappings;
    }
  }
}
