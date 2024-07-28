// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.Attachment
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;
using System.IO;

namespace Microsoft.AzureAd.Icm.Types
{
  public class Attachment
  {
    public const string InvalidExtensions = ".ade,.adp,.app,.bas,.bat,.cer,.chm,.cmd,.com,.cpl,.crt,.csh,.exe,.fxp,.hlp,.hta,.inf,.ins,.isp,.js,.jse,.ksh,.lnk,.mda,.mdb,.mde,.mdt,.mdw,.mdz,.msc,.msi,.msp,.mst,.ops,.pcd,.pif,.prf,.prg,.pst,.reg,.scf,.scr,.sct,.shb,.shs,.tmp,.url,.vb,.vbe,.vbs,.vsw,.ws,.wsc,.wsf,.wsh,.xnk,.ps,.psm,.ps1";
    public const int MaxAttachmentSize = 10485760;
    public const int MaxAttachmentCount = 50;
    private static readonly Dictionary<string, string> ImageExtensionToMime = new Dictionary<string, string>()
    {
      {
        ".jpg",
        "image/jpeg"
      },
      {
        ".bmp",
        "image/bmp"
      },
      {
        ".png",
        "image/png"
      }
    };

    public Attachment(string filename, Stream contents)
    {
      this.Filename = filename;
      this.Contents = contents;
    }

    public string Filename { get; private set; }

    public Stream Contents { get; private set; }

    public bool IsImage => this.MimeType.StartsWith("image/");

    public string MimeType
    {
      get
      {
        string str;
        Attachment.ImageExtensionToMime.TryGetValue(Path.GetExtension(this.Filename).ToLower(), out str);
        return str ?? "application/x-binary";
      }
    }
  }
}
