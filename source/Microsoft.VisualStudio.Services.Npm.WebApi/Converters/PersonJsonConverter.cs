// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.PersonJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class PersonJsonConverter : BaseMultiFormatJsonConverter<Person>
  {
    public const string NamePropertyName = "name";
    public const string EmailPropertyName = "email";
    public const string UrlPropertyName = "url";

    protected override Person ParseStringValue(string name)
    {
      StringBuilder stringBuilder = new StringBuilder(name.Length);
      StringBuilder sb1 = new StringBuilder(name.Length);
      StringBuilder sb2 = new StringBuilder(name.Length);
      for (int index = 0; index < name.Length; ++index)
      {
        int num = index;
        switch (name[index])
        {
          case '(':
            num = this.TryParseUntilDelimiter(name, ')', index, out sb2);
            break;
          case '<':
            num = this.TryParseUntilDelimiter(name, '>', index, out sb1);
            break;
        }
        if (num == index)
          stringBuilder.Append(name[index]);
        else
          index = num;
      }
      return new Person()
      {
        Name = stringBuilder.ToString().Trim(),
        Email = sb1.ToString().Trim(),
        Url = sb2.ToString().Trim()
      };
    }

    protected override Person ParseProperties(Dictionary<string, string> properties) => new Person()
    {
      Name = properties.ContainsKey("name") ? properties["name"] : (string) null,
      Email = properties.ContainsKey("email") ? properties["email"] : (string) null,
      Url = properties.ContainsKey("url") ? properties["url"] : (string) null
    };

    protected override Person ParseArray(List<object> tokens) => (Person) null;

    private int TryParseUntilDelimiter(
      string input,
      char delimiter,
      int start,
      out StringBuilder sb)
    {
      sb = new StringBuilder();
      for (int index = start + 1; index < input.Length; ++index)
      {
        if ((int) input[index] == (int) delimiter)
          return index;
        sb.Append(input[index]);
      }
      sb.Clear();
      return start;
    }
  }
}
