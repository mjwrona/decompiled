// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.NativeMethods
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [SecurityCritical]
  [CLSCompliant(false)]
  public static class NativeMethods
  {
    public const int TRUE = 1;
    public const int FALSE = 0;
    public const uint LOAD_LIBRARY_AS_DATAFILE = 2;
    public const int ERROR_SUCCESS = 0;
    public const int ERROR_FILE_NOT_FOUND = 2;
    public const int ERROR_PATH_NOT_FOUND = 3;
    public const int ERROR_ACCESS_DENIED = 5;
    public const int ERROR_INVALID_HANDLE = 6;
    public const int ERROR_NOT_SAME_DEVICE = 17;
    public const int ERROR_NO_MORE_FILES = 18;
    public const int ERROR_SHARING_VIOLATION = 32;
    public const int ERROR_FILE_EXISTS = 80;
    public const int ERROR_CANNOT_MAKE = 82;
    public const int ERROR_INVALID_PARAMETER = 87;
    public const int ERROR_INSUFFICIENT_BUFFER = 122;
    public const int ERROR_ALREADY_EXISTS = 183;
    public const int ERROR_DIRECTORY = 267;
    public const int ERROR_NOT_CAPABLE = 775;
    public const int ERROR_NO_ASSOCIATION = 1155;
    public const int ERROR_NOT_FOUND = 1168;
    public const int ERROR_NONE_MAPPED = 1332;
    public const int ERROR_CLASS_ALREADY_EXISTS = 1410;
    public const int WM_DESTROY = 2;
    public const int WM_MOVE = 3;
    public const int WM_SETFOCUS = 7;
    public const int WM_KILLFOCUS = 8;
    public const int WM_NOTIFY = 78;
    public const int WM_MOUSEACTIVATE = 33;
    public const int WM_PARENTNOTIFY = 528;
    public const int WM_HSCROLL = 276;
    public const int WM_VSCROLL = 277;
    public const int WM_WINDOWPOSCHANGING = 70;
    public const int WM_WINDOWPOSCHANGED = 71;
    public const int WM_GETDLGCODE = 135;
    public const int WM_TIMER = 275;
    public const int WM_MOUSEWHEEL = 522;
    public const int WM_SETREDRAW = 11;
    public const int WM_KEYDOWN = 256;
    public const int WM_KEYUP = 257;
    public const int WM_CHAR = 258;
    public const int WM_SYSKEYDOWN = 260;
    public const int WM_SYSKEYUP = 261;
    public const int WM_SYSCHAR = 262;
    public const int WM_CLOSE = 16;
    public const int WM_COMMAND = 273;
    public const int WM_SYSCOMMAND = 274;
    public const int WM_NCMOUSEMOVE = 160;
    public const int WM_NCLBUTTONDOWN = 161;
    public const int WM_NCMBUTTONDBLCLK = 169;
    public const int WM_NCPAINT = 133;
    public const int WM_PAINT = 15;
    public const int WM_STYLECHANGED = 125;
    public const int WM_ENABLE = 10;
    public const int WM_INITDIALOG = 272;
    public const int WM_SIZE = 5;
    public const int WM_ERASEBKGND = 20;
    public const int WM_USER = 1024;
    public const int WM_REFLECT = 8192;
    public const int WM_SYSCOLORCHANGE = 21;
    public const int WM_CUT = 768;
    public const int WM_COPY = 769;
    public const int WM_PASTE = 770;
    public const int WM_UNDO = 772;
    public const int WM_PALETTECHANGED = 785;
    public const int WM_THEMECHANGED = 794;
    public const int WM_DISPLAYCHANGE = 126;
    public const int WM_SETTEXT = 12;
    public const int WM_UPDATEUISTATE = 296;
    public const int WM_SETTINGCHANGE = 26;
    public const int WM_NEXTDLGCTL = 40;
    public const int WM_KEYFIRST = 256;
    public const int WM_KEYLAST = 265;
    public const int WM_SETFONT = 48;
    public const int WM_QUERYENDSESSION = 17;
    public const int WM_ENDSESSION = 22;
    public const int WM_SETICON = 128;
    public const int ICON_BIG = 1;
    public const int ICON_SMALL = 0;
    public const int WM_CONTEXTMENU = 123;
    public const int WM_HELP = 83;
    public const int WS_OVERLAPPED = 0;
    public const int WS_BORDER = 8388608;
    public const int WS_POPUP = -2147483648;
    public const int WS_CHILD = 1073741824;
    public const int WS_VISIBLE = 268435456;
    public const int WS_TABSTOP = 65536;
    public const int WS_EX_CLIENTEDGE = 512;
    public const int WS_EX_TOOLWINDOW = 128;
    public const int WS_EX_TOPMOST = 8;
    public const int WS_EX_CONTEXTHELP = 1024;
    public const int WS_EX_DLGMODALFRAME = 1;
    public const int WS_EX_CONTROLPARENT = 65536;
    public const int WS_MINIMIZEBOX = 131072;
    public const int WS_MAXIMIZEBOX = 65536;
    public const int WS_CAPTION = 12582912;
    public const int VK_RETURN = 13;
    public const int VK_SHIFT = 16;
    public const int VK_CONTROL = 17;
    public const int VK_MENU = 18;
    public const int VK_ESCAPE = 27;
    public const int VK_DELETE = 46;
    public const int VK_F2 = 113;
    public const int VK_F10 = 121;
    public const int VK_TAB = 9;
    public const int VK_LEFT = 37;
    public const int VK_UP = 38;
    public const int VK_RIGHT = 39;
    public const int VK_DOWN = 40;
    public const int SC_CLOSE = 61536;
    public const int SC_RESTORE = 61728;
    public const int SC_CONTEXTHELP = 61824;
    public const int MF_BYCOMMAND = 0;
    public const int MF_ENABLED = 0;
    public const int MF_GRAYED = 1;
    public const int MF_DISABLED = 2;
    public const int CS_DROPSHADOW = 131072;
    public const int SB_HORZ = 0;
    public const int SB_VERT = 1;
    public const uint SIF_TRACKPOS = 16;
    public const uint SIF_POS = 4;
    public const int SW_SHOWNORMAL = 1;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SHOW = 5;
    public const int SW_SHOWNA = 8;
    public const int GWL_HWNDPARENT = -8;
    public const int GWL_STYLE = -16;
    public const int GWL_EXSTYLE = -20;
    public const int GWL_USERDATA = -21;
    public const int SYSTEM_FONT = 13;
    public const int DEFAULT_GUI_FONT = 17;
    public const int HDM_GETITEMRECT = 4615;
    public const int HDM_GETITEM = 4619;
    public const int HDM_SETITEM = 4620;
    public const int HDF_CHECKBOX = 64;
    public const int HDF_CHECKED = 128;
    public const int HDF_SORTDOWN = 512;
    public const int HDF_SORTUP = 1024;
    public const int HDF_IMAGE = 2048;
    public const int HDF_BITMAP_ON_RIGHT = 4096;
    public const int HDF_STRING = 16384;
    public const int HDN_BEGINTRACKA = -306;
    public const int HDN_BEGINTRACKW = -326;
    public const int HDN_ITEMCHANGEDA = -301;
    public const int HDN_ITEMCHANGEDW = -321;
    public const int HDN_ITEMCHANGINGA = -300;
    public const int HDN_ITEMCHANGINGW = -320;
    public const int HDN_ITEMSTATEICONCLICK = -316;
    public const int HDM_SETIMAGELIST = 4616;
    public const int HDS_HOTTRACK = 4;
    public const int HDS_CHECKBOXES = 1024;
    public const int HDI_WIDTH = 1;
    public const int HDI_FORMAT = 4;
    public const int HDI_IMAGE = 32;
    public const int TTM_SETDELAYTIME = 1027;
    public const int TTM_POP = 1052;
    public const int TTN_GETDISPINFOW = -530;
    public const int TTDT_AUTOPOP = 2;
    public const int EM_SETCHARFORMAT = 1092;
    public const int EM_SETTYPOGRAPHYOPTIONS = 1226;
    public const int TO_ADVANCEDTYPOGRAPHY = 1;
    public const int CFE_LINK = 32;
    public const int CFM_LINK = 32;
    public const int CFM_COLOR = 1073741824;
    public const int SCF_SELECTION = 1;
    public const int SCF_ALL = 4;
    public const int EN_LINK = 1803;
    public const int EM_GETSEL = 176;
    public const int EM_SETSEL = 177;
    public const int EM_SCROLL = 181;
    public const int EM_SCROLLCARET = 183;
    public const int EM_GETMODIFY = 184;
    public const int EM_SETMODIFY = 185;
    public const int EM_GETLINECOUNT = 186;
    public const int EM_REPLACESEL = 194;
    public const int EM_GETLINE = 196;
    public const int EM_LIMITTEXT = 197;
    public const int EM_CANUNDO = 198;
    public const int EM_UNDO = 199;
    public const int EM_SETPASSWORDCHAR = 204;
    public const int EM_GETPASSWORDCHAR = 210;
    public const int EM_EMPTYUNDOBUFFER = 205;
    public const int EM_SETREADONLY = 207;
    public const int EM_SETMARGINS = 211;
    public const int EM_POSFROMCHAR = 214;
    public const int EM_CHARFROMPOS = 215;
    public const int EM_LINEFROMCHAR = 201;
    public const int EM_LINEINDEX = 187;
    public const int LVM_GETHEADER = 4127;
    public const int LVM_SETIMAGELIST = 4099;
    public const int LVM_GETITEMCOUNT = 4100;
    public const int LVM_GETITEMSTATE = 4140;
    public const int LVM_GETCOLUMNWIDTH = 4125;
    public const int LVM_GETNEXTITEM = 4108;
    public const int LVM_INSERTITEMA = 4103;
    public const int LVM_INSERTITEMW = 4173;
    public const int LVM_DELETEITEM = 4104;
    public const int LVM_DELETEALLITEMS = 4105;
    public const int LVM_INSERTCOLUMNA = 4123;
    public const int LVM_SETITEMSTATE = 4139;
    public const int LVM_INSERTCOLUMNW = 4193;
    public const int LVM_GETITEMRECT = 4110;
    public const int LVM_ENSUREVISIBLE = 4115;
    public const int LVM_SETEXTENDEDLISTVIEWSTYLE = 4150;
    public const int LVM_GETSELECTIONMARK = 4162;
    public const int LVM_SETSELECTIONMARK = 4163;
    public const int LVM_GETITEMW = 4171;
    public const int LVM_SETCOLUMN = 4122;
    public const int LVM_GETTOOLTIPS = 4174;
    public const int LVM_HITTEST = 4114;
    public const int LVM_GETEDITCONTROL = 4120;
    public const int LVS_ILNORMAL = 0;
    public const int LVS_ILSMALL = 1;
    public const int LVCF_WIDTH = 2;
    public const int LVCF_FMT = 1;
    public const int LVCF_IMAGE = 16;
    public const int LVCFMT_LEFT = 0;
    public const int LVCFMT_IMAGE = 2048;
    public const int LVCFMT_BITMAP_ON_RIGHT = 4096;
    public const int LVIR_BOUNDS = 0;
    public const int LVN_KEYDOWN = -155;
    public const int LVHT_ONITEMICON = 2;
    public const int LVHT_ONITEMLABEL = 4;
    public const int LVHT_ONITEMSTATEICON = 8;
    public const int LVHT_ONITEM = 14;
    public const int LVIF_STATE = 8;
    public const int LVIS_STATEIMAGEMASK = 61440;
    public const int LVIS_FOCUSED = 1;
    public const int LVIS_SELECTED = 2;
    public const int LVIS_DROPHILITED = 8;
    public const int LVS_EX_CHECKBOXES = 4;
    public const int LVS_EX_FULLROWSELECT = 32;
    public const int LVS_EX_ONECLICKACTIVATE = 64;
    public const int LVS_EX_TWOCLICKACTIVATE = 128;
    public const int LVS_EX_DOUBLEBUFFER = 65536;
    public const int TVSIL_STATE = 2;
    public const int TV_FIRST = 4352;
    public const int TVM_GETITEMRECT = 4356;
    public const int TVM_GETINDENT = 4358;
    public const int TVM_GETIMAGELIST = 4360;
    public const int TVM_SETIMAGELIST = 4361;
    public const int TVM_SELECTITEM = 4363;
    public const int TVM_SETITEM = 4365;
    public const int TVM_GETEDITCONTROL = 4367;
    public const int TVM_HITTEST = 4369;
    public const int TVM_GETTOOLTIPS = 4377;
    public const int TVM_GETITEMSTATE = 4391;
    public const int TVM_SETEXTENDEDSTYLE = 4396;
    public const int TVM_GETEXTENDEDSTYLE = 4397;
    public const int TVM_GETITEM = 4414;
    public const int TVS_FULLROWSELECT = 4096;
    public const int TVS_EX_FADEINOUTEXPANDOS = 64;
    public const int TVS_EX_DOUBLEBUFFER = 4;
    public const int TVIF_HANDLE = 16;
    public const int TVIF_STATE = 8;
    public const int TVIF_CHILDREN = 64;
    public const int TVIS_STATEIMAGEMASK = 61440;
    public const int TVIS_EXPANDED = 32;
    public const int TVHitOnItemIcon = 2;
    public const int TVHitOnItemLabel = 4;
    public const int NM_CLICK = -2;
    public const int NM_DBLCLK = -3;
    public const int TVGN_DROPHILITE = 8;
    public const int TVGN_CARET = 9;
    public const int TVHitOnItemStateIcon = 64;
    public const int I_CHILDRENCALLBACK = -1;
    public const int NM_CUSTOMDRAW = -12;
    public const int CDRF_NOTIFYITEMDRAW = 32;
    public const int CDRF_NOTIFYSUBITEMDRAW = 32;
    public const int CDRF_NOTIFYPOSTPAINT = 16;
    public const int CDRF_SKIPDEFAULT = 4;
    public const int CDRF_DODEFAULT = 0;
    public const int CDRF_NEWFONT = 2;
    public const int CDRF_NOTIFYPOSTERASE = 64;
    public const int CDDS_PREPAINT = 1;
    public const int CDDS_POSTPAINT = 2;
    public const int CDDS_ITEM = 65536;
    public const int CDDS_ITEMPREPAINT = 65537;
    public const int CDDS_ITEMPOSTPAINT = 65538;
    public const int CDDS_SUBITEM = 131072;
    public const int CDDS_PREERASE = 3;
    public const int CDDS_POSTERASE = 4;
    public const uint CDIS_SELECTED = 1;
    public const uint CDIS_DISABLED = 4;
    public const uint CDIS_FOCUS = 16;
    public const uint CDIS_HOT = 64;
    public const int LB_SETSEL = 389;
    public const int LB_GETANCHORINDEX = 413;
    public const int LB_SETANCHORINDEX = 412;
    public const int SWP_NOACTIVATE = 16;
    public const int SWP_NOSIZE = 1;
    public const int SWP_NOMOVE = 2;
    public const int SWP_NOZORDER = 4;
    public const int SWP_ASYNCWINDOWPOS = 16384;
    public const int SWP_DEFERERASE = 8192;
    public const int SWP_NOCOPYBITS = 256;
    public static readonly HandleRef HWND_BOTTOM = new HandleRef((object) null, (IntPtr) 1);
    public const int HWND_TOPMOST = -1;
    public const int HTCAPTION = 2;
    public const int PM_NOREMOVE = 0;
    public const int PM_REMOVE = 1;
    public const int PM_NOYIELD = 2;
    public const int SERVERCALL_ISHANDLED = 0;
    public const int PENDINGMSG_WAITDEFPROCESS = 2;
    public const int EC_LEFTMARGIN = 1;
    public const int EC_RIGHTMARGIN = 2;
    public const int WS_EX_RIGHT = 4096;
    public const int DT_SINGLELINE = 32;
    public const int DT_CALCRECT = 1024;
    public const int DT_PATH_ELLIPSIS = 16384;
    public const int DT_END_ELLIPSIS = 32768;
    public const int DT_MODIFYSTRING = 65536;
    public const int HTSYSMENU = 3;
    public const int HTCLOSE = 20;
    public const int STD_INPUT_HANDLE = -10;
    public const int STD_OUTPUT_HANDLE = -11;
    public const int STD_ERROR_HANDLE = -12;
    public const int FILE_TYPE_CHAR = 2;
    public const int MAX_PATH = 260;
    public const int CSIDL_DESKTOP = 0;
    public const int BFFM_INITIALIZED = 1;
    public const int BFFM_SELCHANGED = 2;
    public const int BFFM_SETSELECTIONA = 1126;
    public const int BFFM_SETSELECTIONW = 1127;
    public const int BFFM_ENABLEOK = 1125;
    internal static readonly int BFFM_SETSELECTION;
    public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    public const int WM_MOUSEMOVE = 512;
    public const int WM_LBUTTONDOWN = 513;
    public const int WM_LBUTTONUP = 514;
    public const int WM_LBUTTONDBLCLK = 515;
    public const int WM_RBUTTONDOWN = 516;
    public const int WM_RBUTTONUP = 517;
    public const int WM_RBUTTONDBLCLK = 518;
    public const int WM_MBUTTONDOWN = 519;
    public const int WM_MBUTTONUP = 520;
    public const int WM_MBUTTONDBLCLK = 521;
    public const int WM_XBUTTONDOWN = 523;
    public const int WM_XBUTTONUP = 524;
    public const int WM_XBUTTONDBLCLK = 525;
    public const int WM_MOUSELAST = 522;
    public const int WM_CANCELMODE = 31;
    public const int MA_ACTIVATE = 1;
    public const int MA_ACTIVATEANDEAT = 2;
    public const int MA_NOACTIVATE = 3;
    public const int MA_NOACTIVATEANDEAT = 4;
    public const int UIS_SET = 1;
    public const int UIS_CLEAR = 2;
    public const int UIS_INITIALIZE = 3;
    public const int UISF_HIDEFOCUS = 1;
    public const int UISF_HIDEACCEL = 2;
    public const int LOGPIXELSX = 88;
    public const int LOGPIXELSY = 90;
    public static uint FILE_SUPPORTS_HARD_LINKS = 4194304;
    public const uint INVALID_FILE_ATTRIBUTES = 4294967295;
    internal const uint FILE_ATTRIBUTE_READONLY = 1;
    internal const uint FILE_ATTRIBUTE_NORMAL = 128;
    private const int FSCTL_GET_COMPRESSION = 589884;
    private const int FSCTL_SET_COMPRESSION = 639040;
    public static uint COPY_FILE_NO_BUFFERING = 4096;
    public static int AF_INET = 2;
    public static int AF_INET6 = 23;
    private const int FO_MOVE = 1;
    private const int FO_COPY = 2;
    private const int FO_DELETE = 3;
    private const int FO_RENAME = 4;
    private const int FOF_MULTIDESTFILES = 1;
    private const int FOF_CONFIRMMOUSE = 2;
    private const int FOF_SILENT = 4;
    private const int FOF_RENAMEONCOLLISION = 8;
    private const int FOF_NOCONFIRMATION = 16;
    private const int FOF_WANTMAPPINGHANDLE = 32;
    private const int FOF_ALLOWUNDO = 64;
    private const int FOF_FILESONLY = 128;
    private const int FOF_SIMPLEPROGRESS = 256;
    private const int FOF_NOCONFIRMMKDIR = 512;
    private const int FOF_NOERRORUI = 1024;
    private const int FOF_NOCOPYSECURITYATTRIBS = 2048;
    private const int FOF_NORECURSION = 4096;
    private const int FOF_NO_CONNECTED_ELEMENTS = 8192;
    private const int FOF_WANTNUKEWARNING = 16384;
    private const int FOF_NORECURSEREPARSE = 32768;
    public const int SPI_GETFOREGROUNDFLASHCOUNT = 8196;
    public const int FLASHW_STOP = 0;
    public const int FLASHW_CAPTION = 1;
    public const int FLASHW_TRAY = 2;
    public const int FLASHW_ALL = 3;
    public const int FLASHW_TIMER = 4;
    public const int FLASHW_TIMERNOFG = 12;
    public const int GW_HWNDFIRST = 0;
    public const int GW_HWNDLAST = 1;
    public const int GW_HWNDNEXT = 2;
    public const int GW_HWNDPREV = 3;
    public const int GW_OWNER = 4;
    public const int GW_CHILD = 5;
    public const int SHACF_FILESYSTEM = 1;
    public const int SHACF_FILESYS_DIRS = 32;
    public const int SHACF_AUTOSUGGEST_FORCE_ON = 268435456;
    public const int SHACF_USETAB = 8;
    public const int SHACF_AUTOAPPEND_FORCE_ON = 1073741824;
    public const int ShgfiIcon = 256;
    public const int ShgfiLargeIcon = 0;
    public const int ShgfiOpenIcon = 2;
    public const int ShgfiSelected = 65536;
    public const int ShgfiShellIconSize = 4;
    public const int ShgfiSmallIcon = 1;
    public const int ShgfiSysIconIndex = 16384;
    public const int ShgfiTypeName = 1024;
    public const int ShgfiUseFileAttributes = 16;
    public const int SHELL_NO_REGISTERED_APP_FOR_FILE_OR_VERB = 31;
    public const int SE_ERR_NOASSOC = 31;
    public const int SEE_MASK_INVOKEIDLIST = 12;
    public const int SEE_MASK_FLAG_NO_UI = 1024;
    public const int ILD_TRANSPARENT = 1;
    private const int FORMAT_MESSAGE_FROM_SYSTEM = 4096;
    public const int AUTHZ_RM_FLAG_NO_AUDIT = 1;
    public const int FILE_PERSISTENT_ACLS = 8;
    public const int DWL_MSGRESULT = 0;
    public const int IDOK = 1;
    public const int PSHHELP = 1038;
    public const int CDM_FIRST = 1124;
    public const int CDM_GETFILEPATH = 1125;
    public const int CDM_GETFOLDERPATH = 1126;
    public const int CDM_GETFOLDERIDLIST = 1127;
    public const int CDM_SETCONTROLTEXT = 1128;
    public const int CDM_HIDECONTROL = 1129;
    public const int CDM_SETDEFEXT = 1130;
    public const int CDN_FIRST = -601;
    public const int CDN_LAST = -699;
    public const int CDN_INITDONE = -601;
    public const int CDN_SELCHANGE = -602;
    public const int CDN_FOLDERCHANGE = -603;
    public const int CDN_SHAREVIOLATION = -604;
    public const int CDN_HELP = -605;
    public const int CDN_FILEOK = -606;
    public const int CDN_TYPECHANGE = -607;
    public const int CDN_INCLUDEITEM = -608;
    public const int CBN_DROPDOWN = 7;
    internal const uint OFN_READONLY = 1;
    internal const uint OFN_OVERWRITEPROMPT = 2;
    internal const uint OFN_HIDEREADONLY = 4;
    internal const uint OFN_NOCHANGEDIR = 8;
    internal const uint OFN_SHOWHELP = 16;
    internal const uint OFN_ENABLEHOOK = 32;
    internal const uint OFN_ENABLETEMPLATE = 64;
    internal const uint OFN_ENABLETEMPLATEHANDLE = 128;
    internal const uint OFN_NOVALIDATE = 256;
    internal const uint OFN_ALLOWMULTISELECT = 512;
    internal const uint OFN_EXTENSIONDIFFERENT = 1024;
    internal const uint OFN_PATHMUSTEXIST = 2048;
    internal const uint OFN_FILEMUSTEXIST = 4096;
    internal const uint OFN_CREATEPROMPT = 8192;
    internal const uint OFN_SHAREAWARE = 16384;
    internal const uint OFN_NOREADONLYRETURN = 32768;
    internal const uint OFN_NOTESTFILECREATE = 65536;
    internal const uint OFN_NONETWORKBUTTON = 131072;
    internal const uint OFN_NOLONGNAMES = 262144;
    internal const uint OFN_EXPLORER = 524288;
    internal const uint OFN_NODEREFERENCELINKS = 1048576;
    internal const uint OFN_LONGNAMES = 2097152;
    internal const uint OFN_ENABLEINCLUDENOTIFY = 4194304;
    internal const uint OFN_ENABLESIZING = 8388608;
    internal const uint OFN_DONTADDTORECENT = 33554432;
    internal const uint OFN_FORCESHOWHIDDEN = 268435456;
    internal const uint OFN_EX_NOPLACESBAR = 1;
    public const ushort KeyDownFlag = 128;
    internal static readonly int UNIVERSAL_NAME_INFO_LEVEL = 1;
    public static readonly int CRYPT_ACQUIRE_CACHE_FLAG = 1;
    public static readonly int CRYPT_ACQUIRE_USE_PROV_INFO_FLAG = 2;
    public static readonly int CRYPT_ACQUIRE_COMPARE_KEY_FLAG = 4;
    public static readonly int CRYPT_ACQUIRE_NO_HEALING = 8;
    public static readonly int CRYPT_ACQUIRE_SILENT_FLAG = 64;
    public static readonly int CRYPT_ACQUIRE_WINDOW_HANDLE_FLAG = 128;
    public static readonly int CRYPT_ACQUIRE_NCRYPT_KEY_FLAGS_MASK = 458752;
    public static readonly int CRYPT_ACQUIRE_ALLOW_NCRYPT_KEY_FLAG = 65536;
    public static readonly int CRYPT_ACQUIRE_PREFER_NCRYPT_KEY_FLAG = 131072;
    public static readonly int CRYPT_ACQUIRE_ONLY_NCRYPT_KEY_FLAG = 262144;
    public static readonly string NCRYPT_LENGTH_PROPERTY = "Length";
    public static readonly int NTE_BAD_KEYSET = -2146893802;
    public static readonly int NTE_SILENT_CONTEXT = -2146893790;
    public static readonly int SCARD_E_NO_SMARTCARD = -2146435060;
    public const string LEGACY_RSAPRIVATE_BLOB = "CAPIPRIVATEBLOB";
    public const string LEGACY_RSAPUBLIC_BLOB = "CAPIPUBLICBLOB";
    public const byte PRIVATEKEYBLOB = 7;
    public const byte PUBLICKEYBLOB = 6;
    public const string BCRYPT_RSA_ALGORITHM = "RSA";
    public const int NTE_BAD_SIGNATURE = -2146893818;
    public const int NTE_BUFFER_TOO_SMALL = -2146893784;
    public const int BCRYPT_PAD_PKCS1 = 2;
    public const int BCRYPT_PAD_OAEP = 4;
    public static readonly int ERROR_MORE_DATA = 234;
    public const uint CRED_TYPE_GENERIC = 1;
    public const uint CRED_TYPE_DOMAIN_PASSWORD = 2;
    public const int CREDUI_FLAGS_INCORRECT_PASSWORD = 1;
    public const int CREDUI_FLAGS_DO_NOT_PERSIST = 2;
    public const int CREDUI_FLAGS_REQUEST_ADMINISTRATOR = 4;
    public const int CREDUI_FLAGS_EXCLUDE_CERTIFICATES = 8;
    public const int CREDUI_FLAGS_REQUIRE_CERTIFICATE = 16;
    public const int CREDUI_FLAGS_SHOW_SAVE_CHECK_BOX = 64;
    public const int CREDUI_FLAGS_ALWAYS_SHOW_UI = 128;
    public const int CREDUI_FLAGS_REQUIRE_SMARTCARD = 256;
    public const int CREDUI_FLAGS_PASSWORD_ONLY_OK = 512;
    public const int CREDUI_FLAGS_VALIDATE_USERNAME = 1024;
    public const int CREDUI_FLAGS_COMPLETE_USERNAME = 2048;
    public const int CREDUI_FLAGS_PERSIST = 4096;
    public const int CREDUI_FLAGS_SERVER_CREDENTIAL = 16384;
    public const int CREDUI_FLAGS_EXPECT_CONFIRMATION = 131072;
    public const int CREDUI_FLAGS_GENERIC_CREDENTIALS = 262144;
    public const int CREDUI_FLAGS_USERNAME_TARGET_CREDENTIALS = 524288;
    public const int CREDUI_FLAGS_KEEP_USERNAME = 1048576;
    public const int CRED_PACK_PROTECTED_CREDENTIALS = 1;
    public const int NO_ERROR = 0;
    public const int CREDUI_MAX_USERNAME_LENGTH = 513;
    public const int CREDUI_MAX_PASSWORD_LENGTH = 256;
    public const int CREDUI_MAX_CAPTION_LENGTH = 128;
    public const int CREDUI_MAX_MESSAGE_LENGTH = 32767;
    public const int CREDUIWIN_CHECKBOX = 2;
    public const int CREDUIWIN_AUTHPACKAGE_ONLY = 16;
    public const int CRED_PERSIST_LOCAL_MACHINE = 2;
    public static uint SERVICE_NO_CHANGE = uint.MaxValue;
    public static long ERROR_SERVICE_DATABASE_LOCKED = 1055;
    public static long ERROR_INVALID_SERVICE_ACCOUNT = 1057;
    public static int GENERIC_WRITE = 1073741824;
    public static int SERVICE_WIN32_OWN_PROCESS = 16;
    public static int WRITE_OWNER = 524288;
    public static int WRITE_DAC = 262144;
    public static int READ_CONTROL = 131072;
    public static int DELETE = 65536;
    public static int SERVICE_ERROR_NORMAL = 1;
    public static int STANDARD_RIGHTS_REQUIRED = 983040;
    public static int SERVICE_AUTO_START = 2;
    public static uint LSA_POLICY_ALL_ACCESS = 8191;
    public static uint LOGON32_LOGON_INTERACTIVE = 2;
    public static uint LOGON32_LOGON_NETWORK = 3;
    public static uint LOGON32_LOGON_BATCH = 4;
    public static uint LOGON32_LOGON_SERVICE = 5;
    public static uint LOGON32_LOGON_UNLOCK = 7;
    public static uint LOGON32_PROVIDER_DEFAULT = 0;
    public static uint CREATE_UNICODE_ENVIRONMENT = 1024;
    public static uint CREATE_NO_WINDOW = 134217728;
    public static uint CTRL_C_EVENT = 0;
    public static uint CTRL_BREAK_EVENT = 1;
    public const int STGM_DIRECT = 0;
    public const int STGM_TRANSACTED = 65536;
    public const int STGM_SIMPLE = 134217728;
    public const int STGM_READ = 0;
    public const int STGM_WRITE = 1;
    public const int STGM_READWRITE = 2;
    public const int STGM_SHARE_DENY_NONE = 64;
    public const int STGM_SHARE_DENY_READ = 48;
    public const int STGM_SHARE_DENY_WRITE = 32;
    public const int STGM_SHARE_EXCLUSIVE = 16;
    public const int STGM_PRIORITY = 262144;
    public const int STGM_DELETEONRELEASE = 67108864;
    public const int STGM_CREATE = 4096;
    public const int STGM_CONVERT = 131072;
    public const int STGM_FAILIFTHERE = 0;
    public const int STGM_NOSCRATCH = 1048576;
    public const int STGM_NOSNAPSHOT = 2097152;
    public const int STGM_DIRECT_SWMR = 4194304;
    public const int FIND_FIRST_EX_CASE_SENSITIVE = 1;
    public const int FIND_FIRST_EX_LARGE_FETCH = 2;
    public const uint STATUS_DELETE_PENDING = 3221225558;
    public const int FILE_ACTION_ADDED = 1;
    public const int FILE_ACTION_REMOVED = 2;
    public const int FILE_ACTION_MODIFIED = 3;
    public const int FILE_ACTION_RENAMED_OLD_NAME = 4;
    public const int FILE_ACTION_RENAMED_NEW_NAME = 5;
    public const int FILE_NOTIFY_CHANGE_FILE_NAME = 1;
    public const int FILE_NOTIFY_CHANGE_DIR_NAME = 2;
    public const int FILE_NOTIFY_CHANGE_ATTRIBUTES = 4;
    public const int FILE_NOTIFY_CHANGE_SIZE = 8;
    public const int FILE_NOTIFY_CHANGE_LAST_WRITE = 16;
    public const int FILE_NOTIFY_CHANGE_SECURITY = 256;
    public const uint DRIVE_REMOVABLE = 2;
    public const uint DRIVE_FIXED = 3;
    public const uint INTERNET_COOKIE_EVALUATE_P3P = 128;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetComboBoxInfo(
      HandleRef hWnd,
      NativeMethods.COMBOBOXINFO pComboboxInfo);

    [DllImport("Gdi32.dll", CallingConvention = CallingConvention.StdCall)]
    public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

    static NativeMethods()
    {
      if (Marshal.SystemDefaultCharSize == 1)
        NativeMethods.BFFM_SETSELECTION = 1126;
      else
        NativeMethods.BFFM_SETSELECTION = 1127;
    }

    [DllImport("user32.dll")]
    public static extern IntPtr GetLastActivePopup(IntPtr hwnd);

    [DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
    public static extern bool ZeroMemory(IntPtr Destination, int Length);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process([In] IntPtr hProcess, out bool lpSystemInfo);

    public static bool IsWow64
    {
      get
      {
        bool lpSystemInfo;
        if (!NativeMethods.IsWow64Process(Process.GetCurrentProcess().Handle, out lpSystemInfo))
          throw new Win32Exception();
        return lpSystemInfo;
      }
    }

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CreateHardLink(
      string lpFileName,
      string lpExistingFileName,
      IntPtr lpSecurityAttributes);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool GetVolumeInformation(
      string lpRootPathName,
      IntPtr lpVolumeNameBuffer,
      uint nVolumeNameSize,
      IntPtr lpVolumeSerialNumber,
      IntPtr lpMaximumComponentLength,
      ref uint lpFileSystemFlags,
      IntPtr lpFileSystemNameBuffer,
      uint nFileSystemNameSize);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern SafeFileHandle CreateFile(
      string lpFileName,
      [MarshalAs(UnmanagedType.U4)] NativeMethods.FileAccess dwDesiredAccess,
      [MarshalAs(UnmanagedType.U4)] NativeMethods.FileShare dwShareMode,
      IntPtr lpSecurityAttributes,
      [MarshalAs(UnmanagedType.U4)] NativeMethods.CreationDisposition dwCreationDisposition,
      [MarshalAs(UnmanagedType.U4)] NativeMethods.FileAttributes dwFlagsAndAttributes,
      IntPtr hTemplateFile);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool DeleteFile(string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool RemoveDirectory(string lpPathName);

    [DllImport("kernel32", SetLastError = true)]
    public static extern bool FlushFileBuffers(SafeFileHandle hFile);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint GetFileAttributes(string lpFileName);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool SetFileAttributes(string lpFileName, uint dwFileAttributes);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern bool GetFileAttributesEx(
      string name,
      NativeMethods.GET_FILEEX_INFO_LEVELS fileInfoLevel,
      ref NativeMethods.WIN32_FILE_ATTRIBUTE_DATA lpFileInformation);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(
      SafeHandle hDevice,
      uint dwIoControlCode,
      IntPtr lpInBuffer,
      uint nInBufferSize,
      out short lpOutBuffer,
      uint nOutBufferSize,
      out uint lpBytesReturned,
      IntPtr lpOverlapped);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool DeviceIoControl(
      SafeHandle hDevice,
      uint dwIoControlCode,
      ref short lpInBuffer,
      uint nInBufferSize,
      IntPtr lpOutBuffer,
      uint nOutBufferSize,
      out uint lpBytesReturned,
      IntPtr lpOverlapped);

    internal static short GetFileCompression(SafeHandle handle)
    {
      uint lpBytesReturned = 0;
      short lpOutBuffer;
      if (!NativeMethods.DeviceIoControl(handle, 589884U, IntPtr.Zero, 0U, out lpOutBuffer, 2U, out lpBytesReturned, IntPtr.Zero))
        throw new Win32Exception();
      return lpOutBuffer;
    }

    internal static void SetFileCompression(SafeHandle handle, short compressionFormat)
    {
      if (!NativeMethods.DeviceIoControl(handle, 639040U, ref compressionFormat, 2U, IntPtr.Zero, 0U, out uint _, IntPtr.Zero))
        throw new Win32Exception();
    }

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern bool MoveFile(string src, string dst);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern bool MoveFileEx(string src, string dst, int dwFlags);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CopyFileEx(
      string lpExistingFileName,
      string lpNewFileName,
      IntPtr lpProgressRoutine,
      IntPtr lpData,
      IntPtr pbCancel,
      uint dwCopyFlags);

    [DllImport("WS2_32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint WSAStartup(
      short wVersionRequested,
      out NativeMethods.WsaData lpWsaData);

    [DllImport("WS2_32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint WSAStringToAddressW(
      string addressString,
      int addressFamily,
      IntPtr lpProtocolInfo,
      ref NativeMethods.SOCKADDR_IN6 lpAddress,
      ref int lpAddressLength);

    [DllImport("WS2_32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern uint WSAStringToAddressW(
      string addressString,
      int addressFamily,
      IntPtr lpProtocolInfo,
      ref NativeMethods.SOCKADDR_IN lpAddress,
      ref int lpAddressLength);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern bool SetWindowText(HandleRef hWnd, string text);

    [DllImport("user32")]
    public static extern bool MoveWindow(
      HandleRef hWnd,
      int X,
      int Y,
      int Width,
      int Height,
      bool repaint);

    [DllImport("user32.dll")]
    public static extern bool ReleaseCapture();

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(
      IntPtr hWnd,
      int hWndInsertAfter,
      int X,
      int Y,
      int cx,
      int cy,
      int uFlags);

    [DllImport("user32.dll")]
    public static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetDlgItem(HandleRef hWndDlg, int Id);

    [DllImport("shell32")]
    public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);

    [DllImport("shell32", CharSet = CharSet.Auto)]
    public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    [DllImport("shell32", CharSet = CharSet.Auto)]
    public static extern IntPtr SHBrowseForFolder([In, Out] ref NativeMethods.BROWSEINFO lpbi);

    [DllImport("shell32", CharSet = CharSet.Unicode)]
    private static extern int SHFileOperationW([In] ref NativeMethods.SHFILEOPSTRUCT fileOp);

    public static int SHFileOperationDelete(string path) => NativeMethods.SHFileOperationDelete(new string[1]
    {
      path
    }, true);

    public static int SHFileOperationDelete(string path, bool recycle) => NativeMethods.SHFileOperationDelete(new string[1]
    {
      path
    }, recycle);

    public static int SHFileOperationDelete(string[] paths, bool recycle)
    {
      NativeMethods.SHFILEOPSTRUCT fileOp = new NativeMethods.SHFILEOPSTRUCT();
      fileOp.wFunc = 3U;
      fileOp.fFlags = (ushort) 1044;
      if (recycle)
        fileOp.fFlags |= (ushort) 64;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string path in paths)
      {
        stringBuilder.Append(path);
        stringBuilder.Append(char.MinValue);
      }
      if (stringBuilder.Length == 0)
        return 0;
      fileOp.pFrom = stringBuilder.ToString();
      return NativeMethods.SHFileOperationW(ref fileOp);
    }

    [DllImport("gdi32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetStockObject(int nIndex);

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern int FlashWindowEx(ref NativeMethods.FLASHWINFO fwi);

    [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern short RegisterClass(NativeMethods.WNDCLASS wc);

    [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

    [DllImport("User32", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

    public static string GetClassName(IntPtr hwnd)
    {
      StringBuilder lpClassName = new StringBuilder(256);
      NativeMethods.GetClassName(hwnd, lpClassName, lpClassName.Capacity);
      return lpClassName.ToString();
    }

    [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr CreateWindowEx(
      int dwExStyle,
      string lpszClassName,
      string lpszWindowName,
      int style,
      int x,
      int y,
      int width,
      int height,
      IntPtr hWndParent,
      IntPtr hMenu,
      IntPtr hInst,
      [MarshalAs(UnmanagedType.AsAny)] object pvParam);

    [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool DestroyWindow(IntPtr hWnd);

    [DllImport("user32")]
    public static extern IntPtr SetTimer(
      HandleRef hWnd,
      int nIDEvent,
      int uElapse,
      IntPtr lpTimerFunc);

    [DllImport("user32")]
    public static extern bool KillTimer(HandleRef hwnd, int idEvent);

    [DllImport("user32")]
    public static extern bool SystemParametersInfo(
      int nAction,
      int nParam,
      ref int value,
      int ignore);

    [DllImport("user32", SetLastError = true)]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out NativeMethods.POINT lpPoint);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    public static extern IntPtr GetModuleHandle(string modName);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetModuleFileName(
      IntPtr hModule,
      StringBuilder lpFilename,
      int nSize);

    public static string GetModuleFileName() => NativeMethods.GetModuleFileName(IntPtr.Zero);

    public static string GetModuleFileName(IntPtr hModule)
    {
      StringBuilder lpFilename = new StringBuilder(260);
      int moduleFileName = NativeMethods.GetModuleFileName(hModule, lpFilename, lpFilename.Capacity);
      int lastWin32Error = Marshal.GetLastWin32Error();
      if (moduleFileName == 0 || moduleFileName >= lpFilename.Capacity)
        throw new TeamFoundationServerException(NativeMethods.FormatError(lastWin32Error));
      return lpFilename.ToString();
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool QueryFullProcessImageName(
      IntPtr hProcess,
      int dwFlags,
      StringBuilder lpFilename,
      ref int nSize);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool PeekConsoleInput(
      IntPtr hConsoleInput,
      IntPtr buffer,
      uint numInputRecords,
      out uint numEventsRead);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern unsafe bool WriteConsole(
      IntPtr hConsoleOutput,
      char* text,
      uint numCharsToWrite,
      out uint numCharsWritten,
      IntPtr mustBeZero);

    public static bool WriteConsole(IntPtr hConsoleOutput, char[] chars) => NativeMethods.WriteConsole(hConsoleOutput, chars, chars.Length);

    public static bool WriteConsole(IntPtr hConsoleOutput, char[] chars, int charsToWrite) => NativeMethods.WriteConsole(hConsoleOutput, chars, charsToWrite, out uint _);

    public static unsafe bool WriteConsole(
      IntPtr hConsoleOutput,
      char[] chars,
      int charsToWrite,
      out uint numCharsWritten)
    {
      numCharsWritten = 0U;
      if (chars == null)
        return NativeMethods.WriteConsole(hConsoleOutput, (char*) null, 0U, out numCharsWritten, IntPtr.Zero);
      charsToWrite = Math.Min(charsToWrite, chars.Length);
      bool flag = true;
      fixed (char* chPtr = chars)
      {
        uint val1 = (uint) charsToWrite;
        while (flag && numCharsWritten < val1)
        {
          uint numCharsToWrite = Math.Min(Math.Min(val1, 8192U), val1 - numCharsWritten);
          uint numCharsWritten1 = 0;
          flag = NativeMethods.WriteConsole(hConsoleOutput, (char*) ((IntPtr) chPtr + (IntPtr) ((long) numCharsWritten * 2L)), numCharsToWrite, out numCharsWritten1, IntPtr.Zero);
          numCharsWritten += numCharsWritten1;
        }
      }
      return flag;
    }

    [DllImport("kernel32", SetLastError = true)]
    public static extern unsafe bool WriteFile(
      IntPtr handle,
      byte* bytes,
      int numBytesToWrite,
      out uint numBytesWritten,
      IntPtr mustBeZero);

    public static bool WriteFile(IntPtr handle, char[] chars) => NativeMethods.WriteFile(handle, chars, chars.Length);

    public static bool WriteFile(IntPtr handle, char[] chars, int numCharsToWrite) => NativeMethods.WriteFile(handle, chars, numCharsToWrite, out uint _);

    public static unsafe bool WriteFile(
      IntPtr handle,
      char[] chars,
      int numCharsToWrite,
      out uint numCharsWritten)
    {
      if (chars != null && numCharsToWrite > 0)
      {
        numCharsToWrite = Math.Min(numCharsToWrite, chars.Length);
        fixed (char* bytes = chars)
        {
          uint numBytesWritten = 0;
          int num = NativeMethods.WriteFile(handle, (byte*) bytes, numCharsToWrite * 2, out numBytesWritten, IntPtr.Zero) ? 1 : 0;
          numCharsWritten = numBytesWritten / 2U;
          return num != 0;
        }
      }
      else
      {
        numCharsWritten = 0U;
        return true;
      }
    }

    public static bool WriteFile(IntPtr handle, byte[] bytes) => NativeMethods.WriteFile(handle, bytes, bytes.Length);

    public static bool WriteFile(IntPtr handle, byte[] bytes, int numBytesToWrite) => NativeMethods.WriteFile(handle, bytes, numBytesToWrite, out uint _);

    public static unsafe bool WriteFile(
      IntPtr handle,
      byte[] bytes,
      int numBytesToWrite,
      out uint numBytesWritten)
    {
      if (bytes != null && numBytesToWrite > 0)
      {
        numBytesToWrite = Math.Min(numBytesToWrite, bytes.Length);
        fixed (byte* bytes1 = bytes)
        {
          numBytesWritten = 0U;
          return NativeMethods.WriteFile(handle, bytes1, numBytesToWrite, out numBytesWritten, IntPtr.Zero);
        }
      }
      else
      {
        numBytesWritten = 0U;
        return true;
      }
    }

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr GetSystemMenu(HandleRef hwnd, bool bRevert);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool EnableMenuItem(HandleRef hMenu, uint uIDEnableItem, uint uEnable);

    [DllImport("kernel32", SetLastError = true)]
    public static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32", SetLastError = true)]
    public static extern int GetFileType(IntPtr handle);

    [DllImport("kernel32", SetLastError = true)]
    public static extern bool CloseHandle(IntPtr handle);

    [DllImport("kernel32", SetLastError = true)]
    public static extern ushort GetSystemDefaultLangID();

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetFocus();

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr SetFocus(HandleRef hWnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr SetFocus(IntPtr hWnd);

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern int SetForegroundWindow(IntPtr hwnd);

    [DllImport("user32")]
    public static extern bool IsWindow(IntPtr hwnd);

    [DllImport("user32")]
    public static extern bool IsWindowVisible(IntPtr hwnd);

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr GetActiveWindow();

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    public static extern IntPtr SetActiveWindow(IntPtr hwnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int EnumThreadWindows(
      uint dwThreadId,
      NativeMethods.EnumWindowsProc lpEnumFunc,
      IntPtr lParam);

    [DllImport("user32", CharSet = CharSet.Auto)]
    internal static extern int EnumChildWindows(
      IntPtr hWndParent,
      NativeMethods.EnumWindowsProc lpEnumFunc,
      IntPtr lParam);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int EnumWindows(NativeMethods.EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int flags);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool ShowWindow(HandleRef hWnd, int nCmdShow);

    [DllImport("user32")]
    public static extern bool IsWindowEnabled(IntPtr hWnd);

    [DllImport("user32")]
    public static extern bool EnableWindow(IntPtr hWnd, bool bEnable);

    [DllImport("user32")]
    internal static extern bool UpdateWindow(HandleRef hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      HandleRef hWnd,
      int msg,
      IntPtr wParam,
      ref NativeMethods.HDITEM lParam);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      HandleRef hWnd,
      int msg,
      IntPtr wParam,
      ref NativeMethods.RECT rect);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      HandleRef hWnd,
      int msg,
      IntPtr wparam,
      StringBuilder lparam);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      HandleRef hWnd,
      int msg,
      IntPtr wParam,
      ref NativeMethods.LVITEM_NOTEXT lParam);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wparam, string lparam);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      HandleRef hWnd,
      int msg,
      IntPtr wParam,
      ref NativeMethods.LVHITTESTINFO lParam);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    [DllImport("User32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      IntPtr hWnd,
      int msg,
      IntPtr wParam,
      ref NativeMethods.LVITEM lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      IntPtr hWnd,
      int msg,
      IntPtr wParam,
      ref NativeMethods.TVITEM lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessage(
      IntPtr hWnd,
      int msg,
      IntPtr wParam,
      ref NativeMethods.TV_HITTESTINFO lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SendMessageW(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    public static IntPtr SendMessage(
      object control,
      IntPtr handle,
      int msg,
      int wParam,
      int lParam)
    {
      return NativeMethods.SendMessage(new HandleRef(control, handle), msg, (IntPtr) wParam, (IntPtr) lParam);
    }

    [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int PostMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowLong(HandleRef hWnd, int nIndex);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLong(HandleRef hWnd, int nIndex, HandleRef dwNewLong);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool SetWindowPos(
      HandleRef hWnd,
      HandleRef hWndInsertAfter,
      int x,
      int y,
      int cx,
      int cy,
      int flags);

    [DllImport("User32.dll", SetLastError = true)]
    public static extern int SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool bRedraw);

    [DllImport("Gdi32.dll", SetLastError = true)]
    public static extern IntPtr CreateRectRgn(
      int nLeftRect,
      int nTopRect,
      int nRightRect,
      int nBottomRect);

    [DllImport("user32", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto)]
    private static extern IntPtr _WindowFromPoint(NativeMethods.POINTSTRUCT pt);

    public static IntPtr WindowFromPoint(int x, int y) => NativeMethods._WindowFromPoint(new NativeMethods.POINTSTRUCT(x, y));

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool IsChild(IntPtr hWndParent, IntPtr hWnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetParent(HandleRef window);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetParent(IntPtr window);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetAncestor(HandleRef window, NativeMethods.GetAncestorFlags flags);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindowDC(HandleRef hwnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int ReleaseDC(HandleRef hwnd, HandleRef hdc);

    [DllImport("user32")]
    public static extern bool GetWindowRect(HandleRef hwnd, out NativeMethods.RECT rect);

    [DllImport("user32")]
    public static extern bool GetClientRect(HandleRef hwnd, out NativeMethods.RECT rect);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int GetClientRect(IntPtr hWnd, ref NativeMethods.RECT rc);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool ScreenToClient(HandleRef hWnd, ref NativeMethods.POINT pt);

    [DllImport("ole32")]
    public static extern int CoRegisterMessageFilter(
      NativeMethods.IOleMessageFilter newFilter,
      ref IntPtr oldMsgFilter);

    [DllImport("ole32")]
    public static extern int CoRegisterMessageFilter(HandleRef handle, ref IntPtr oldMsgFilter);

    [DllImport("ole32.dll")]
    public static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);

    [DllImport("ole32.dll")]
    public static extern int CreateBindCtx(uint reserved, out IBindCtx pctx);

    [DllImport("ole32.dll")]
    public static extern int ProgIDFromCLSID(ref Guid clsid, ref IntPtr lplpszProgID);

    [DllImport("user32", CharSet = CharSet.Unicode)]
    public static extern int RegisterWindowMessage(string msg);

    [DllImport("user32")]
    public static extern bool DestroyIcon(IntPtr hIcon);

    [DllImport("gdi32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

    [DllImport("gdi32")]
    public static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll")]
    public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int WaitForInputIdle(IntPtr hProcess, int dwMilliseconds);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetWindow(IntPtr hWnd, int wCmd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int GetCaretPos(ref NativeMethods.POINT lpPoint);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern bool InvalidateRect(
      HandleRef hWnd,
      ref NativeMethods.RECT rect,
      bool erase);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr SetCursor(HandleRef hcursor);

    [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int DrawTextExW(
      IntPtr hDC,
      StringBuilder lpszString,
      int nCount,
      ref NativeMethods.RECT lpRect,
      int nFormat,
      [In, Out] NativeMethods.DRAWTEXTPARAMS lpDTParams);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32", CharSet = CharSet.Auto)]
    public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    public static int GetListViewColumnWidth(HandleRef hwnd, int column) => (int) NativeMethods.SendMessage(hwnd, 4125, (IntPtr) column, IntPtr.Zero);

    [DllImport("shlwapi.dll")]
    public static extern int SHAutoComplete(IntPtr hwndEdit, int dwFlags);

    [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
    public static extern long StrFormatByteSize(long qdw, StringBuilder pszBuf, int cchBuf);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)]
    private static extern uint GetTempFileName(
      string tmpPath,
      string prefix,
      uint uniqueIdOrZero,
      StringBuilder tmpFileName);

    internal static string GetTempFileName(string tmpPath, string prefix, uint uniqueIdOrZero)
    {
      StringBuilder tmpFileName = new StringBuilder(260);
      if (NativeMethods.GetTempFileName(tmpPath, prefix, uniqueIdOrZero, tmpFileName) == 0U)
        throw new IOException(TFCommonResources.ErrorCreatingTempFile((object) tmpPath));
      return tmpFileName.ToString();
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr SHGetFileInfoW(
      string pszPath,
      int dwFileAttributes,
      ref NativeMethods.ShFileInfoW psfi,
      int cbSizeFileInfo,
      int uFlags);

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr ShellExecute(
      IntPtr hParentWnd,
      [MarshalAs(UnmanagedType.LPWStr)] string operation,
      [MarshalAs(UnmanagedType.LPWStr)] string file,
      [MarshalAs(UnmanagedType.LPWStr)] string parameters,
      [MarshalAs(UnmanagedType.LPWStr)] string directory,
      int showCmd);

    [DllImport("shell32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool ShellExecuteEx(ref NativeMethods.ShellExecuteInfo execInfo);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr ImageList_GetIcon(IntPtr hImageList, int index, uint flags);

    [DllImport("comctl32")]
    public static extern int ImageList_GetIconSize(IntPtr himage, out int cx, out int cy);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern int ImageList_GetImageCount(IntPtr hImageList);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern int ImageList_ReplaceIcon(IntPtr hImageList, int index, IntPtr hicon);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    internal static extern bool ImageList_GetImageInfo(
      IntPtr hImageList,
      int index,
      ref NativeMethods.IMAGEINFO imageInfo);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    internal static extern int ImageList_Add(IntPtr hImageList, IntPtr bmImage, IntPtr bmMask);

    [DllImport("comctl32.dll", CharSet = CharSet.Auto)]
    public static extern int ImageList_Destroy(IntPtr hImageList);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool GetScrollInfo(
      IntPtr hWnd,
      int fnBar,
      ref NativeMethods.SCROLLINFO si);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern bool PeekMessage(
      out NativeMethods.MSG msg,
      IntPtr hwnd,
      uint filterMin,
      uint filterMax,
      bool removeMsg);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr DispatchMessage(ref NativeMethods.MSG msg);

    [DllImport("comctl32", CharSet = CharSet.Unicode)]
    public static extern IntPtr ImageList_Duplicate(IntPtr himageListSource);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int LoadString(
      IntPtr hInstance,
      uint uID,
      [Out] StringBuilder lpBuffer,
      int nBufferMax);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool FreeLibrary(IntPtr hModule);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll")]
    public static extern bool MessageBeep(uint uType);

    [DllImport("kernel32.dll")]
    public static extern int GetCurrentThreadId();

    [DllImport("Kernel32.dll")]
    public static extern IntPtr GetCurrentThread();

    [DllImport("Kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetThreadPriority(IntPtr hThread, int nPriority);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern void OutputDebugString(string s);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern int FormatMessage(
      uint dwFlags,
      IntPtr lpSource,
      uint dwMessageId,
      int dwLanguageId,
      [Out] StringBuilder lpBuffer,
      int nSize,
      IntPtr Arguments);

    public static string FormatError(int number)
    {
      uint dwMessageId = (uint) number;
      StringBuilder lpBuffer = new StringBuilder(1024);
      NativeMethods.FormatMessage(4096U, IntPtr.Zero, dwMessageId, 0, lpBuffer, lpBuffer.Capacity, IntPtr.Zero);
      return lpBuffer.ToString();
    }

    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    public static extern bool PathIsDirectoryEmpty(string path);

    [DllImport("Advapi32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern unsafe uint GetLengthSid(void* pSid);

    [DllImport("Advapi32.dll", EntryPoint = "LookupAccountNameW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern unsafe bool LookupAccountName(
      string systemName,
      string accountName,
      void* psid,
      ref uint sidSize,
      StringBuilder domainName,
      ref uint domainLength,
      out NativeMethods.AccountType accountType);

    [DllImport("Advapi32.dll", EntryPoint = "LookupAccountSidW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool LookupAccountSid(
      string systemName,
      byte[] psid,
      StringBuilder accountName,
      ref uint nameLength,
      StringBuilder domainName,
      ref uint domainLength,
      out NativeMethods.AccountType accountType);

    [DllImport("Advapi32.dll", EntryPoint = "LookupAccountSidW", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    private static extern bool LookupAccountSid(
      string systemName,
      IntPtr psid,
      StringBuilder accountName,
      ref uint nameLength,
      StringBuilder domainName,
      ref uint domainLength,
      out NativeMethods.AccountType accountType);

    public static byte[] NameToSid(string accountName) => NativeMethods.NameToSid(accountName, out string _, out NativeMethods.AccountType _);

    public static unsafe byte[] NameToSid(
      string accountName,
      out string domainName,
      out NativeMethods.AccountType accountType)
    {
      uint sidSize = 512;
      byte[] sourceArray = new byte[(int) sidSize];
      uint domainLength = 512;
      StringBuilder domainName1 = new StringBuilder((int) domainLength);
      byte[] numArray = sourceArray;
      byte* numPtr = sourceArray == null || numArray.Length == 0 ? (byte*) null : &numArray[0];
      if (!NativeMethods.LookupAccountName((string) null, accountName, (void*) numPtr, ref sidSize, domainName1, ref domainLength, out accountType))
        throw new Win32Exception(Marshal.GetLastWin32Error());
      sidSize = NativeMethods.GetLengthSid((void*) numPtr);
      numArray = (byte[]) null;
      domainName = domainName1.ToString();
      byte[] destinationArray = new byte[(int) sidSize];
      Array.Copy((Array) sourceArray, (Array) destinationArray, (long) sidSize);
      return destinationArray;
    }

    public static string SidToName(
      IntPtr sid,
      out string domainName,
      out NativeMethods.AccountType accountType)
    {
      uint nameLength = 512;
      StringBuilder accountName = new StringBuilder((int) nameLength);
      uint domainLength = 512;
      StringBuilder domainName1 = new StringBuilder((int) domainLength);
      if (!NativeMethods.LookupAccountSid((string) null, sid, accountName, ref nameLength, domainName1, ref domainLength, out accountType))
        throw new Win32Exception(Marshal.GetLastWin32Error());
      domainName = domainName1.ToString();
      return accountName.ToString();
    }

    public static string SidToName(
      byte[] sid,
      out string domainName,
      out NativeMethods.AccountType accountType)
    {
      uint nameLength = 512;
      StringBuilder accountName = new StringBuilder((int) nameLength);
      uint domainLength = 512;
      StringBuilder domainName1 = new StringBuilder((int) domainLength);
      if (!NativeMethods.LookupAccountSid((string) null, sid, accountName, ref nameLength, domainName1, ref domainLength, out accountType))
        throw new Win32Exception(Marshal.GetLastWin32Error());
      domainName = domainName1.ToString();
      return accountName.ToString();
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern bool GetVolumeInformation(
      string lpRootPathName,
      StringBuilder lpVolumeNameBuffer,
      int nVolumeNameSize,
      out uint lpVolumeSerialNumber,
      out uint lpMaximumComponentLength,
      out uint lpFileSystemFlags,
      StringBuilder lpFileSystemNameBuffer,
      int nFileSystemNameSize);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr CreateJobObject(
      [In] ref NativeMethods.SECURITY_ATTRIBUTES lpJobAttributes,
      string lpName);

    [DllImport("kernel32.dll")]
    public static extern bool AssignProcessToJobObject(SafeHandle hJob, SafeHandle hProcess);

    [DllImport("kernel32.dll")]
    public static extern IntPtr OpenProcess(
      uint dwDesiredAccess,
      bool bInheritHandle,
      uint dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool TerminateJobObject(SafeHandle hJob, uint uExitCode);

    [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
    private static extern int StringFromGUID2(IntPtr pguid, StringBuilder lpsz, int cchMax);

    public static Guid GuidFromNativePtr(IntPtr pguid)
    {
      StringBuilder lpsz = new StringBuilder(64);
      NativeMethods.StringFromGUID2(pguid, lpsz, lpsz.Capacity);
      return new Guid(lpsz.ToString());
    }

    public static NativeMethods.DS_NAME_RESULT_ITEM[] DsCrackNames(
      IntPtr hDS,
      NativeMethods.DS_NAME_FLAGS flags,
      NativeMethods.DS_NAME_FORMAT formatOffered,
      NativeMethods.DS_NAME_FORMAT formatDesired,
      string[] names)
    {
      IntPtr ppResult;
      uint error = NativeMethods.DsCrackNames(hDS, flags, formatOffered, formatDesired, names == null ? 0U : (uint) names.Length, names, out ppResult);
      if (error != 0U)
        throw new Win32Exception((int) error);
      NativeMethods.DS_NAME_RESULT_ITEM[] dsNameResultItemArray;
      try
      {
        NativeMethods.DS_NAME_RESULT dsNameResult = new NativeMethods.DS_NAME_RESULT();
        dsNameResult.cItems = (uint) Marshal.ReadInt32(ppResult);
        dsNameResult.rItems = Marshal.ReadIntPtr(ppResult, Marshal.OffsetOf(typeof (NativeMethods.DS_NAME_RESULT), "rItems").ToInt32());
        IntPtr ptr = dsNameResult.rItems;
        dsNameResultItemArray = new NativeMethods.DS_NAME_RESULT_ITEM[(int) dsNameResult.cItems];
        for (int index = 0; index < (int) dsNameResult.cItems; ++index)
        {
          dsNameResultItemArray[index] = (NativeMethods.DS_NAME_RESULT_ITEM) Marshal.PtrToStructure(ptr, typeof (NativeMethods.DS_NAME_RESULT_ITEM));
          ptr = (IntPtr) ((long) ptr + (long) Marshal.SizeOf<NativeMethods.DS_NAME_RESULT_ITEM>(dsNameResultItemArray[index]));
        }
      }
      finally
      {
        NativeMethods.DsFreeNameResult(ppResult);
      }
      return dsNameResultItemArray;
    }

    [DllImport("ntdsapi.dll", CharSet = CharSet.Auto)]
    public static extern uint DsCrackNames(
      IntPtr hDS,
      NativeMethods.DS_NAME_FLAGS flags,
      NativeMethods.DS_NAME_FORMAT formatOffered,
      NativeMethods.DS_NAME_FORMAT formatDesired,
      uint cNames,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4, ArraySubType = UnmanagedType.LPWStr)] string[] rpNames,
      out IntPtr ppResult);

    [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
    public static extern uint DsGetSiteName(string ComputerName, out string SiteName);

    [DllImport("ntdsapi.dll", CharSet = CharSet.Auto)]
    public static extern void DsFreeNameResult(IntPtr pResult);

    [DllImport("ntdsapi.dll", CharSet = CharSet.Unicode)]
    public static extern uint DsBind(
      string DomainControllerName,
      string DnsDomainName,
      out IntPtr phDS);

    [DllImport("ntdsapi.dll", CharSet = CharSet.Unicode)]
    public static extern uint DsUnBind(ref IntPtr phDS);

    [DllImport("User32.dll")]
    public static extern ushort GetKeyState(short key);

    [DllImport("Kernel32.dll", EntryPoint = "GetVersionExW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool GetVersionEx([In, Out] NativeMethods.OSVersionInfoEx ver);

    [DllImport("Kernel32.dll")]
    public static extern bool GetProductInfo(
      int osMajorVersion,
      int osMinorVersion,
      int spMajorVersion,
      int spMinorVersion,
      out int edition);

    [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
    public static extern int WNetAddConnection2(
      NativeMethods.NETRESOURCE resource,
      string lpPassword,
      string lpUserName,
      int dwFlags);

    [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U4)]
    public static extern int WNetGetUniversalName(
      string lpLocalPath,
      [MarshalAs(UnmanagedType.U4)] int dwInfoLevel,
      IntPtr lpBuffer,
      [MarshalAs(UnmanagedType.U4)] ref int lpBufferSize);

    public static unsafe byte[] AccountToSid(string account)
    {
      uint sidSize = 512;
      byte[] sourceArray = new byte[(int) sidSize];
      uint domainLength = 512;
      StringBuilder domainName = new StringBuilder((int) domainLength);
      byte[] numArray = sourceArray;
      byte* numPtr = sourceArray == null || numArray.Length == 0 ? (byte*) null : &numArray[0];
      if (!NativeMethods.LookupAccountName((string) null, account, (void*) numPtr, ref sidSize, domainName, ref domainLength, out NativeMethods.AccountType _))
        throw new Win32Exception(Marshal.GetLastWin32Error());
      sidSize = NativeMethods.GetLengthSid((void*) numPtr);
      numArray = (byte[]) null;
      domainName.ToString();
      byte[] destinationArray = new byte[(int) sidSize];
      Array.Copy((Array) sourceArray, (Array) destinationArray, (long) sidSize);
      return destinationArray;
    }

    public static string SidToAccount(byte[] sid)
    {
      uint nameLength = 512;
      StringBuilder accountName = new StringBuilder((int) nameLength);
      uint domainLength = 512;
      StringBuilder domainName = new StringBuilder((int) domainLength);
      if (!NativeMethods.LookupAccountSid((string) null, sid, accountName, ref nameLength, domainName, ref domainLength, out NativeMethods.AccountType _))
        throw new Win32Exception(Marshal.GetLastWin32Error());
      return domainName.ToString() + "\\" + accountName.ToString();
    }

    [DllImport("crypt32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CryptAcquireCertificatePrivateKey(
      IntPtr pCert,
      int dwFlags,
      IntPtr pvReserved,
      out SafeNCryptKeyHandle phCryptProvOrNCryptKey,
      out uint dwKeySpec,
      [MarshalAs(UnmanagedType.Bool)] out bool pfCallerFreeProvOrNCryptKey);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern int CryptReleaseContext(IntPtr hProv, int dwFlags);

    [DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
    public static extern int NCryptCreatePersistedKey(
      SafeNCryptProviderHandle hProvider,
      out SafeNCryptKeyHandle phKey,
      string pszAlgId,
      string pszKeyName,
      int dwLegacyKeySpec,
      CngKeyCreationOptions dwFlags);

    [DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
    public static extern int NCryptGetProperty(
      SafeNCryptHandle hObject,
      string pszProperty,
      [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbOutput,
      int cbOutput,
      out int pcbResult,
      CngPropertyOptions dwFlags);

    [DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
    public static extern int NCryptSetProperty(
      SafeNCryptHandle hObject,
      string pszProperty,
      [MarshalAs(UnmanagedType.LPArray)] byte[] pbInput,
      int cbInput,
      CngPropertyOptions dwFlags);

    [DllImport("ncrypt.dll")]
    public static extern int NCryptFinalizeKey(SafeNCryptKeyHandle hKey, int dwFlags);

    [DllImport("ncrypt.dll")]
    public static extern int NCryptSignHash(
      SafeNCryptKeyHandle hKey,
      [In] ref NativeMethods.BCRYPT_PKCS1_PADDING_INFO pPaddingInfo,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbHashValue,
      int cbHashValue,
      [MarshalAs(UnmanagedType.LPArray), Out] byte[] pbSignature,
      int cbSignature,
      out int pcbResult,
      int dwFlags);

    [DllImport("ncrypt.dll")]
    public static extern int NCryptVerifySignature(
      SafeNCryptKeyHandle hKey,
      [In] ref NativeMethods.BCRYPT_PKCS1_PADDING_INFO pPaddingInfo,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbHashValue,
      int cbHashValue,
      [MarshalAs(UnmanagedType.LPArray), In] byte[] pbSignature,
      int cbSignature,
      int dwFlags);

    [DllImport("ncrypt.dll")]
    public static extern int NCryptOpenStorageProvider(
      out SafeNCryptProviderHandle phProvider,
      [MarshalAs(UnmanagedType.LPWStr)] string pszProviderName,
      int dwFlags);

    [DllImport("ncrypt.dll", CharSet = CharSet.Unicode)]
    public static extern int NCryptImportKey(
      SafeNCryptProviderHandle hProvider,
      IntPtr hImportKey,
      string pszBlobType,
      IntPtr pParameterList,
      out SafeNCryptKeyHandle phKey,
      [MarshalAs(UnmanagedType.LPArray)] byte[] pbData,
      int cbData,
      int dwFlags);

    [DllImport("ncrypt.dll")]
    public static extern int NCryptEncrypt(
      SafeNCryptKeyHandle hKey,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), In] byte[] pbInput,
      int cbInput,
      [In] ref NativeMethods.BCRYPT_OAEP_PADDING_INFO pPaddingInfo,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5), Out] byte[] pbOutput,
      int cbOutput,
      out int pcbResult,
      int dwFlags);

    [DllImport("ncrypt.dll")]
    public static extern int NCryptDecrypt(
      SafeNCryptKeyHandle hKey,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2), In] byte[] pbInput,
      int cbInput,
      [In] ref NativeMethods.BCRYPT_OAEP_PADDING_INFO pPaddingInfo,
      [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5), Out] byte[] pbOutput,
      int cbOutput,
      out int pcbResult,
      int dwFlags);

    [DllImport("Kernel32.dll", EntryPoint = "GetComputerNameExW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int GetComputerNameEx(
      int nameType,
      StringBuilder nameBuffer,
      ref int bufferSize);

    public static string GetComputerNameEx(NativeMethods.COMPUTER_NAME_FORMAT format)
    {
      StringBuilder nameBuffer = new StringBuilder(1024);
      int capacity1 = nameBuffer.Capacity;
      int computerNameEx = NativeMethods.GetComputerNameEx((int) format, nameBuffer, ref capacity1);
      int lastWin32Error = Marshal.GetLastWin32Error();
      if (computerNameEx == 0 && lastWin32Error == NativeMethods.ERROR_MORE_DATA)
      {
        nameBuffer.Capacity = capacity1 + 1;
        int capacity2 = nameBuffer.Capacity;
        computerNameEx = NativeMethods.GetComputerNameEx((int) format, nameBuffer, ref capacity2);
      }
      return computerNameEx != 0 ? nameBuffer.ToString() : (string) null;
    }

    [DllImport("Kernel32.dll", EntryPoint = "DnsHostnameToComputerNameW", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int DnsHostnameToComputerName(
      string hostname,
      StringBuilder computerName,
      ref int size);

    public static string DnsHostnameToComputerName(string hostname)
    {
      StringBuilder computerName = new StringBuilder(1024);
      int capacity = computerName.Capacity;
      return NativeMethods.DnsHostnameToComputerName(hostname, computerName, ref capacity) != 0 ? computerName.ToString() : (string) null;
    }

    [DllImport("Netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int NetGetJoinInformation(
      string server,
      out IntPtr domain,
      out NativeMethods.NetJoinStatus status);

    public static void NetGetJoinInformation(
      string server,
      out string domain,
      out NativeMethods.NetJoinStatus joinStatus)
    {
      IntPtr domain1 = IntPtr.Zero;
      joinStatus = NativeMethods.NetJoinStatus.NetSetupUnknownStatus;
      int joinInformation = NativeMethods.NetGetJoinInformation(server, out domain1, out joinStatus);
      if (domain1 != IntPtr.Zero)
      {
        domain = Marshal.PtrToStringAuto(domain1);
        int error = NativeMethods.NetApiBufferFree(domain1);
        if (error != 0 && joinInformation == 0)
          throw new Win32Exception(error);
      }
      else
        domain = string.Empty;
      if (joinInformation != 0)
        throw new Win32Exception(joinInformation);
    }

    [DllImport("Netapi32.dll")]
    public static extern int NetApiBufferFree(IntPtr bufferPtr);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredRead(
      string targetName,
      uint type,
      uint flags,
      out IntPtr credential);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredWrite(ref NativeMethods.CREDENTIAL credential, uint flags);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredDelete(string targetName, uint type, uint flags);

    [DllImport("advapi32.dll")]
    public static extern void CredFree(IntPtr buffer);

    [DllImport("credui.dll", CharSet = CharSet.Unicode)]
    public static extern int CredUIPromptForCredentials(
      ref NativeMethods.CREDUI_INFO pUiInfo,
      string pszTargetName,
      IntPtr reserved,
      int dwAuthError,
      StringBuilder pszUserName,
      uint ulUserNameMaxChars,
      StringBuilder pszPassword,
      uint ulPasswordMaxChars,
      ref bool pfSave,
      int dwFlags);

    [DllImport("credui.dll", CharSet = CharSet.Unicode)]
    public static extern int CredUIPromptForWindowsCredentials(
      ref NativeMethods.CREDUI_INFO pUiInfo,
      int dwAuthError,
      ref uint pulAuthPackage,
      byte[] pvInAuthBuffer,
      uint ulInAuthBufferSize,
      out IntPtr ppvOutAuthBuffer,
      out uint pulOutAuthBufferSize,
      ref bool pfSave,
      int dwFlags);

    [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredPackAuthenticationBuffer(
      int dwFlags,
      string pszUserName,
      string pszPassword,
      byte[] pPackedCredentials,
      ref uint pcbPackedCredentials);

    [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CredUnPackAuthenticationBuffer(
      int dwFlags,
      IntPtr pAuthBuffer,
      uint cbAuthBuffer,
      StringBuilder pszUserName,
      ref uint pcchMaxUserName,
      StringBuilder pszDomainName,
      ref uint pcchMaxDomainName,
      StringBuilder pszPassword,
      ref uint pcchMaxPassword);

    [DllImport("credui.dll", CharSet = CharSet.Unicode)]
    public static extern int CredUIParseUserName(
      string pszUserName,
      StringBuilder pszUser,
      uint ulUserMaxChars,
      StringBuilder pszDomain,
      uint ulDomainMaxChars);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern void ZeroMemory(IntPtr address, uint byteCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern bool GetGUIThreadInfo(uint idThread, ref NativeMethods.GUITHREADINFO lpgui);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern ServiceHandle OpenSCManager(
      string machineName,
      string db,
      NativeMethods.ServiceControlAccessRights desiredAccess);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern ServiceHandle OpenService(
      ServiceHandle serviceControlManagerHandle,
      string serviceName,
      NativeMethods.ServiceAccessRights desiredAccess);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool CloseServiceHandle(IntPtr handle);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int ChangeServiceConfig(
      ServiceHandle serviceHandle,
      uint type,
      uint startType,
      uint errorControl,
      string binaryPathName,
      string loadOrderGroup,
      string tagId,
      string dependencies,
      string accountName,
      string password,
      string displayName);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern int ChangeServiceConfig2(
      ServiceHandle serviceHandle,
      NativeMethods.ServiceConfig2InfoLevel dwInfoLevel,
      IntPtr lpInfo);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int QueryServiceConfigW(
      ServiceHandle serviceHandle,
      IntPtr serviceConfigHandle,
      int bufferSize,
      out int bytesNeeded);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern IntPtr LockServiceDatabase(IntPtr handle);

    [DllImport("advapi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnlockServiceDatabase(IntPtr handle);

    [DllImport("userenv.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CreateEnvironmentBlock(
      out IntPtr lpEnvironment,
      SafeHandle hToken,
      bool bInherit);

    [DllImport("userenv.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool CreateProcessAsUser(
      IntPtr hToken,
      string lpApplicationName,
      string lpCommandLine,
      ref NativeMethods.SECURITY_ATTRIBUTES lpProcessAttributes,
      ref NativeMethods.SECURITY_ATTRIBUTES lpThreadAttributes,
      bool bInheritHandle,
      uint dwCreationFlags,
      IntPtr lpEnvironment,
      string lpCurrentDirectory,
      ref NativeMethods.STARTUPINFO lpStartupInfo,
      out NativeMethods.PROCESS_INFORMATION lpProcessInformation);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool DuplicateHandle(
      HandleRef hSourceProcessHandle,
      SafeHandle hSourceHandle,
      HandleRef hTargetProcessHandle,
      out IntPtr lpTargetHandle,
      uint dwDesiredAccess,
      bool bInheritHandle,
      uint dwOptions);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetCurrentProcess();

    [DllImport("kernel32.dll")]
    public static extern bool GenerateConsoleCtrlEvent(uint dwCtrlEvent, uint dwProcessGroupId);

    [DllImport("kernel32.dll")]
    public static extern bool AttachConsole(uint dwProcessId);

    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();

    [DllImport("kernel32.dll")]
    public static extern bool SetConsoleCtrlHandler(
      NativeMethods.ConsoleCtrlEventHandler handlerRoutine,
      bool add);

    [DllImport("userenv.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool LoadUserProfile(
      SafeHandle hToken,
      ref NativeMethods.PROFILEINFO lpProfileInfo);

    [DllImport("userenv.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool UnloadUserProfile(SafeHandle hToken, IntPtr hProfile);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern int LogonUser(
      string userName,
      string domain,
      string password,
      uint logonType,
      uint logonProvider,
      out IntPtr tokenHandle);

    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool WTSQueryUserToken(uint sessionId, out IntPtr phToken);

    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode)]
    public static extern IntPtr WTSOpenServer(string name);

    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode)]
    public static extern void WTSCloseServer(IntPtr hServer);

    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode)]
    public static extern void WTSFreeMemoryEx(
      NativeMethods.WTS_TYPE_CLASS wsTypeClass,
      IntPtr pMemory,
      ulong numberOfEntries);

    [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool WTSEnumerateSessionsEx(
      IntPtr hServer,
      ref uint pLevel,
      uint filter,
      out IntPtr ppSessionInfo,
      out int pCount);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern uint LsaOpenPolicy(
      ref NativeMethods.LSA_UNICODE_STRING SystemName,
      ref NativeMethods.LSA_OBJECT_ATTRIBUTES ObjectAttributes,
      uint DesiredAccess,
      out IntPtr PolicyHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern uint LsaEnumerateAccountRights(
      IntPtr PolicyHandle,
      byte[] AccountSid,
      out IntPtr UserRights,
      out uint CountOfRights);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern uint LsaAddAccountRights(
      IntPtr PolicyHandle,
      byte[] AccountSid,
      NativeMethods.LSA_UNICODE_STRING[] UserRights,
      uint CountOfRights);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern uint LsaRemoveAccountRights(
      IntPtr PolicyHandle,
      byte[] AccountSid,
      byte AllRights,
      NativeMethods.LSA_UNICODE_STRING[] UserRights,
      uint CountOfRights);

    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern uint LsaFreeMemory(IntPtr pBuffer);

    [DllImport("advapi32.dll")]
    public static extern int LsaClose(IntPtr ObjectHandle);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    public static extern uint GetNamedSecurityInfo(
      string objectName,
      System.Security.AccessControl.ResourceType objectType,
      SecurityInfos securityInfo,
      out IntPtr sidOwner,
      out IntPtr sidGroup,
      out IntPtr dacl,
      out IntPtr sacl,
      out IntPtr securityDescriptor);

    [DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
    public static extern uint SetNamedSecurityInfo(
      string objectName,
      System.Security.AccessControl.ResourceType objectType,
      SecurityInfos securityInfo,
      byte[] sidOwner,
      byte[] sidGroup,
      byte[] dacl,
      byte[] sacl);

    [DllImport("advapi32.dll")]
    public static extern uint GetAclInformation(
      IntPtr acl,
      ref NativeMethods.ACL_SIZE_INFORMATION aclInformation,
      uint aclInformationLength,
      NativeMethods.ACL_INFORMATION_CLASS aclInformationClass);

    [DllImport("kernel32.dll")]
    public static extern IntPtr LocalFree(IntPtr handle);

    [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
    public static extern int SetWindowTheme(IntPtr hWnd, string subAppName, string subIdList);

    [DllImport("ole32.dll", PreserveSig = false)]
    public static extern NativeMethods.ILockBytes CreateILockBytesOnHGlobal(
      IntPtr hGlobal,
      bool fDeleteOnRelease);

    [DllImport("OLE32.DLL", CharSet = CharSet.Auto, PreserveSig = false)]
    public static extern IntPtr GetHGlobalFromILockBytes(NativeMethods.ILockBytes pLockBytes);

    [DllImport("OLE32.DLL", CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern NativeMethods.IStorage StgCreateDocfileOnILockBytes(
      NativeMethods.ILockBytes plkbyt,
      uint grfMode,
      uint reserved);

    [DllImport("OLE32.DLL", CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern int StgCreateDocfile(
      [MarshalAs(UnmanagedType.LPWStr)] string pwcsName,
      uint grfMode,
      uint reserved,
      out NativeMethods.IStorage ppstgOpen);

    [DllImport("kernel32.dll")]
    public static extern void GetNativeSystemInfo(ref NativeMethods.SYSTEM_INFO lpSystemInfo);

    [DllImport("oleaut32.dll", CharSet = CharSet.Auto)]
    public static extern void VariantInit(IntPtr pvariant);

    [DllImport("oleaut32.dll", CharSet = CharSet.Auto)]
    public static extern int VariantClear(IntPtr pvariant);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern NativeMethods.SafeFindHandle FindFirstFile(
      string lpFileName,
      ref NativeMethods.WIN32_FIND_DATA lpFindFileData);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern NativeMethods.SafeFindHandle FindFirstFileEx(
      [MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
      NativeMethods.FINDEX_INFO_LEVELS fInfoLevelId,
      ref NativeMethods.WIN32_FIND_DATA lpFindFileData,
      NativeMethods.FINDEX_SEARCH_OPS fSearchOp,
      IntPtr lpSearchFilter,
      uint dwAdditionalFlags);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool FindNextFile(
      NativeMethods.SafeFindHandle hFindFile,
      ref NativeMethods.WIN32_FIND_DATA lpFindfileData);

    [DllImport("kernel32", SetLastError = true)]
    public static extern bool FindClose(IntPtr hFile);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ReadDirectoryChangesW(
      SafeFileHandle hDirectory,
      IntPtr lpBuffer,
      uint nBufferLength,
      bool bWatchSubtree,
      uint dwNotifyFilter,
      out uint lpBytesReturned,
      IntPtr overlappedPointer,
      IntPtr lpCompletionRoutine);

    [DllImport("kernel32", SetLastError = true)]
    public static extern bool CancelIoEx(SafeFileHandle hFile, IntPtr lpOverlapped);

    [DllImport("kernel32", CharSet = CharSet.Auto)]
    public static extern uint GetDriveType([MarshalAs(UnmanagedType.LPWStr)] string lpRootPathName);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern NativeMethods.SafeVolumeMountPointHandle FindFirstVolumeMountPoint(
      [MarshalAs(UnmanagedType.LPWStr)] string lpszRootPathName,
      [MarshalAs(UnmanagedType.LPWStr), Out] StringBuilder lpszVolumeMountPoint,
      uint cchBufferLength);

    [DllImport("kernel32", SetLastError = true)]
    public static extern bool FindVolumeMountPointClose(IntPtr hFindVolumeMountPoint);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool SetFileInformationByHandle(
      SafeFileHandle hFile,
      NativeMethods.FILE_INFO_BY_HANDLE_CLASS FileInformationClass,
      ref NativeMethods.FILE_IO_PRIORITY_HINT_INFO lpFileInformation,
      uint dwBufferSize);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool SetFileInformationByHandle(
      SafeFileHandle hFile,
      NativeMethods.FILE_INFO_BY_HANDLE_CLASS FileInformationClass,
      ref NativeMethods.FILE_END_OF_FILE_INFO lpFileInformation,
      uint dwBufferSize);

    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern IntPtr LoadLibrary(string lpFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GlobalMemoryStatusEx([In, Out] NativeMethods.MEMORYSTATUSEX lpBuffer);

    [DllImport("userenv", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteProfile(
      [MarshalAs(UnmanagedType.LPWStr)] string lpSidString,
      [MarshalAs(UnmanagedType.LPWStr)] string lpProfilePath,
      [MarshalAs(UnmanagedType.LPWStr)] string lpComputerName);

    [DllImport("wininet.dll", SetLastError = true)]
    public static extern bool InternetSetCookieEx(
      [MarshalAs(UnmanagedType.LPWStr)] string lpszURL,
      [MarshalAs(UnmanagedType.LPWStr)] string lpszCookieName,
      [MarshalAs(UnmanagedType.LPWStr)] string lpszCookieData,
      uint dwFlags,
      IntPtr dwReserved);

    [DllImport("bcrypt.dll", SetLastError = true)]
    public static extern int BCryptGetFipsAlgorithmMode([MarshalAs(UnmanagedType.U1)] out bool pfEnabled);

    [DllImport("Netapi32.dll", CharSet = CharSet.Unicode)]
    public static extern NativeMethods.NET_API_STATUS NetUserGetInfo(
      [MarshalAs(UnmanagedType.LPWStr)] string ServerName,
      [MarshalAs(UnmanagedType.LPWStr)] string UserName,
      int level,
      out IntPtr BufPtr);

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern uint GetCurrentProcessId();

    [StructLayout(LayoutKind.Sequential)]
    public class COMBOBOXINFO
    {
      [MarshalAs(UnmanagedType.U4)]
      internal int cbSize = Marshal.SizeOf(typeof (NativeMethods.COMBOBOXINFO));
      internal NativeMethods.RECT rcItem;
      internal NativeMethods.RECT rcButton;
      [MarshalAs(UnmanagedType.U4)]
      internal int stateButton;
      internal IntPtr hwndCombo;
      internal IntPtr hwndItem;
      internal IntPtr hwndList;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8, CharSet = CharSet.Auto)]
    public struct TVITEM
    {
      public uint mask;
      public IntPtr hItem;
      public uint state;
      public uint stateMask;
      public IntPtr pszText;
      public int cchTextMax;
      public int iImage;
      public int iSelectedImage;
      public int cChildren;
      public IntPtr lParam;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TV_HITTESTINFO
    {
      public Point pt;
      public uint flags;
      public IntPtr hItem;
    }

    [Serializable]
    public struct WIN32_FILE_ATTRIBUTE_DATA
    {
      public NativeMethods.FileAttributes fileAttributes;
      public uint ftCreationTimeLow;
      public uint ftCreationTimeHigh;
      public uint ftLastAccessTimeLow;
      public uint ftLastAccessTimeHigh;
      public uint ftLastWriteTimeLow;
      public uint ftLastWriteTimeHigh;
      public int fileSizeHigh;
      public int fileSizeLow;
    }

    public enum GET_FILEEX_INFO_LEVELS
    {
      GetFileExInfoStandard,
      GetFileExMaxInfoLevel,
    }

    [Flags]
    public enum FileAccess : uint
    {
      FILE_LIST_DIRECTORY = 1,
      GenericRead = 2147483648, // 0x80000000
      GenericWrite = 1073741824, // 0x40000000
      GenericExecute = 536870912, // 0x20000000
      GenericAll = 268435456, // 0x10000000
    }

    [Flags]
    public enum FileShare : uint
    {
      None = 0,
      Read = 1,
      Write = 2,
      Delete = 4,
    }

    public enum CreationDisposition : uint
    {
      New = 1,
      CreateAlways = 2,
      OpenExisting = 3,
      OpenAlways = 4,
      TruncateExisting = 5,
    }

    [Flags]
    public enum FileAttributes : uint
    {
      Readonly = 1,
      Hidden = 2,
      System = 4,
      Directory = 16, // 0x00000010
      Archive = 32, // 0x00000020
      Device = 64, // 0x00000040
      Normal = 128, // 0x00000080
      Temporary = 256, // 0x00000100
      SparseFile = 512, // 0x00000200
      ReparsePoint = 1024, // 0x00000400
      Compressed = 2048, // 0x00000800
      Offline = 4096, // 0x00001000
      NotContentIndexed = 8192, // 0x00002000
      Encrypted = 16384, // 0x00004000
      Write_Through = 2147483648, // 0x80000000
      Overlapped = 1073741824, // 0x40000000
      NoBuffering = 536870912, // 0x20000000
      RandomAccess = 268435456, // 0x10000000
      SequentialScan = 134217728, // 0x08000000
      DeleteOnClose = 67108864, // 0x04000000
      BackupSemantics = 33554432, // 0x02000000
      PosixSemantics = 16777216, // 0x01000000
      OpenReparsePoint = 2097152, // 0x00200000
      OpenNoRecall = 1048576, // 0x00100000
      FirstPipeInstance = 524288, // 0x00080000
    }

    [Flags]
    internal enum MoveFileOption
    {
      MOVEFILE_COPY_ALLOWED = 2,
      MOVEFILE_CREATE_HARDLINK = 16, // 0x00000010
      MOVEFILE_DELAY_UNTIL_REBOOT = 4,
      MOVEFILE_FAIL_IF_NOT_TRACKABLE = 32, // 0x00000020
      MOVEFILE_REPLACE_EXISTING = 1,
      MOVEFILE_WRITE_THROUGH = 8,
    }

    public struct WsaData
    {
      public short wVersion;
      public short wHighVersion;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 257)]
      public string szDescription;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
      public string szSystemStatus;
      public short iMaxSockets;
      public short iMaxUdpDg;
      public IntPtr lpVendorInfo;
    }

    public struct SOCKADDR_IN
    {
      public short sin_family;
      public ushort sin_port;
      public uint sin_addr;
      [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
      public byte[] sin_zero;
    }

    public struct SOCKADDR_IN6
    {
      public short sin6_family;
      public ushort sin6_port;
      public uint sin6_flowinfo;
      public uint sin6_addr1;
      public uint sin6_addr2;
      public uint sin6_addr3;
      public uint sin6_addr4;
      public uint sin6_scope_id;
    }

    internal class DlgStyle
    {
      internal const int DsSetFont = 64;
      internal const int Ds3dLook = 4;
      internal const int DsControl = 1024;
      internal const int WsChild = 1073741824;
      internal const int WsClipSiblings = 67108864;
      internal const int WsVisible = 268435456;
      internal const int WsGroup = 131072;
      internal const int SsNotify = 256;
    }

    internal class ExStyle
    {
      internal const int WsExNoParentNotify = 4;
      internal const int WsExControlParent = 65536;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class DlgTemplatePlaceholder
    {
      internal int style = 1140851972;
      internal int extendedStyle = 65536;
      internal short numItems = 1;
      internal short x;
      internal short y;
      internal short cx;
      internal short cy;
      internal short reservedMenu;
      internal short reservedClass;
      internal short reservedTitle;
      internal int itemStyle = 1073741824;
      internal int itemExtendedStyle = 4;
      internal short itemX;
      internal short itemY;
      internal short itemCx;
      internal short itemCy;
      internal short itemId;
      internal ushort itemClassHdr = ushort.MaxValue;
      internal short itemClass = 130;
      internal short itemText;
      internal short itemData;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SHFILEOPSTRUCT
    {
      internal IntPtr hwnd;
      internal uint wFunc;
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string pFrom;
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string pTo;
      internal ushort fFlags;
      internal bool fAnyOperationsAborted;
      internal IntPtr hNameMappings;
      [MarshalAs(UnmanagedType.LPWStr)]
      internal string lpszProgressTitle;
    }

    internal delegate int BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData);

    [Flags]
    internal enum BrowseInfos : uint
    {
      RestrictToFilesystem = 1,
      RestrictToDomain = 2,
      RestrictToSubfolders = 8,
      ShowTextBox = 16, // 0x00000010
      ValidateSelection = 32, // 0x00000020
      NewDialogStyle = 64, // 0x00000040
      HideNewFolderButton = 512, // 0x00000200
      BrowseForComputer = 4096, // 0x00001000
      BrowseForPrinter = 8192, // 0x00002000
      BrowseForEverything = 16384, // 0x00004000
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct BROWSEINFO
    {
      internal IntPtr hwndOwner;
      internal IntPtr pidlRoot;
      internal IntPtr pszDisplayName;
      internal string lpszTitle;
      internal uint ulFlags;
      internal NativeMethods.BrowseCallbackProc lpfn;
      internal IntPtr lParam;
      internal int iImage;
    }

    public struct FLASHWINFO
    {
      public uint cbSize;
      public IntPtr hwnd;
      public uint dwFlags;
      public uint uCount;
      public uint dwTimeout;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class WNDCLASS
    {
      public int style;
      public NativeMethods.WndProc lpfnWndProc;
      public int cbClsExtra;
      public int cbWndExtra;
      public IntPtr hInstance = IntPtr.Zero;
      public IntPtr hIcon = IntPtr.Zero;
      public IntPtr hCursor = IntPtr.Zero;
      public IntPtr hbrBackground = IntPtr.Zero;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpszMenuName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpszClassName;
    }

    public delegate int EnumWindowsProc(IntPtr hwnd, IntPtr lParam);

    public enum GetAncestorFlags
    {
      Parent = 1,
      Root = 2,
      RootOwner = 3,
    }

    [Serializable]
    public struct MSG
    {
      public IntPtr hwnd;
      public int message;
      public IntPtr wParam;
      public IntPtr lParam;
      public int time;
      public int pt_x;
      public int pt_y;
    }

    public struct POINT
    {
      public int x;
      public int y;

      public POINT(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }

    public struct RECT
    {
      public int left;
      public int top;
      public int right;
      public int bottom;

      public RECT(int left, int top, int right, int bottom)
      {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
      }
    }

    public struct WINDOWPOS
    {
      public IntPtr hwnd;
      public IntPtr hwndInsertAfter;
      public int x;
      public int y;
      public int cx;
      public int cy;
      public int flags;
    }

    public struct NMHDR
    {
      public IntPtr hwndFrom;
      public IntPtr idFrom;
      public int code;
    }

    public struct NMCUSTOMDRAW
    {
      public NativeMethods.NMHDR hdr;
      public int dwDrawStage;
      public IntPtr hdc;
      public NativeMethods.RECT rc;
      public IntPtr dwItemSpec;
      public uint uItemState;
      public IntPtr lItemlParam;
    }

    public struct NMTVCUSTOMDRAW
    {
      public NativeMethods.NMCUSTOMDRAW nmcd;
      public uint clrText;
      public uint clrTextBk;
      public int iLevel;
    }

    public struct NMLVCUSTOMDRAW
    {
      public NativeMethods.NMCUSTOMDRAW nmcd;
      public uint clrText;
      public uint clrTextBk;
      public int iSubItem;
      public uint dwItemType;
      public uint clrFace;
      public int iIconEffect;
      public int iIconPhase;
      public int iPartId;
      public int iStateId;
      public NativeMethods.RECT rcText;
      public uint uAlign;
    }

    public struct NMHEADER
    {
      public NativeMethods.NMHDR nmhdr;
      public int iItem;
      public int iButton;
      public IntPtr pItem;
    }

    public struct SCROLLINFO
    {
      public uint cbSize;
      public uint fMask;
      public int nMin;
      public int nMax;
      public uint nPage;
      public int nPos;
      public int nTrackPos;

      public SCROLLINFO(
        uint cbSize,
        uint fMask,
        int nMin,
        int nMax,
        uint nPage,
        int nPos,
        int nTrackPos)
      {
        this.cbSize = cbSize;
        this.fMask = fMask;
        this.nMin = nMin;
        this.nMax = nMax;
        this.nPage = nPage;
        this.nPos = nPos;
        this.nTrackPos = nTrackPos;
      }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct HDITEM
    {
      public int mask;
      public int cxy;
      public IntPtr pszText;
      public IntPtr hbm;
      public int cchTextMax;
      public int fmt;
      public IntPtr lParam;
      public int iImage;
      public int iOrder;
      public int type;
      public IntPtr pvFilter;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct LVITEM
    {
      public int mask;
      public int iItem;
      public int iSubItem;
      public int state;
      public int stateMask;
      public string pszText;
      public int cchTextMax;
      public int iImage;
      public IntPtr lParam;
      public int iIndent;
      public int iGroupId;
      public int cColumns;
      public IntPtr puColumns;
    }

    private struct POINTSTRUCT
    {
      internal int x;
      internal int y;

      internal POINTSTRUCT(int x, int y)
      {
        this.x = x;
        this.y = y;
      }
    }

    public struct NMITEMACTIVATE
    {
      public NativeMethods.NMHDR hdr;
      public int iItem;
      public int iSubItem;
      public uint uNewState;
      public uint uOldState;
      public uint uChanged;
      public NativeMethods.POINT ptAction;
      public IntPtr lParam;
      public uint uKeyFlags;
    }

    public struct ENLINK
    {
      public NativeMethods.NMHDR hdr;
      public uint msg;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class TOOLTIPTEXT
    {
      public NativeMethods.NMHDR hdr;
      public string lpszText;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
      public string szText;
      public IntPtr hinst;
      public uint uFlags;
    }

    public struct LVHITTESTINFO
    {
      public int pt_x;
      public int pt_y;
      public int flags;
      public int iItem;
      public int iSubItem;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NMLVKEYDOWN
    {
      public NativeMethods.NMHDR hdr;
      public short wVKey;
      public uint flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct LVITEM_NOTEXT
    {
      public int mask;
      public int iItem;
      public int iSubItem;
      public int state;
      public int stateMask;
      public IntPtr pszText;
      public int cchTextMax;
      public int iImage;
      public IntPtr lParam;
      public int iIndent;
    }

    [StructLayout(LayoutKind.Sequential)]
    public class DRAWTEXTPARAMS
    {
      private int cbSize = Marshal.SizeOf(typeof (NativeMethods.DRAWTEXTPARAMS));
      public int iTabLength;
      public int iLeftMargin;
      public int iRightMargin;
      public int uiLengthDrawn;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct ShFileInfoW
    {
      public IntPtr hIcon;
      public int iIcon;
      public int dwAttributes;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string szDisplayName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
      public string szTypeName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct ShellExecuteInfo
    {
      public int size;
      public uint mask;
      public IntPtr windowHandle;
      public string verb;
      public string file;
      public string parameters;
      public string directory;
      public int show;
      public int instApp;
      public int IdList;
      public string className;
      public int classKey;
      public uint hotKey;
      public int icon;
      public int process;
    }

    internal struct IMAGEINFO
    {
      internal IntPtr hbmImage;
      internal IntPtr hbmMask;
      internal int Unused1;
      internal int Unused2;
      internal NativeMethods.RECT rcImage;
    }

    public static class ThreadPriorityMode
    {
      public const int THREAD_MODE_BACKGROUND_BEGIN = 65536;
      public const int THREAD_MODE_BACKGROUND_END = 131072;
    }

    public enum AccountType
    {
      SidTypeUser = 1,
      SidTypeGroup = 2,
      SidTypeDomain = 3,
      SidTypeAlias = 4,
      SidTypeWellKnownGroup = 5,
      SidTypeDeletedAccount = 6,
      SidTypeInvalid = 7,
      SidTypeUnknown = 8,
      SidTypeComputer = 9,
    }

    private enum AUTHZ_CONTEXT_INFORMATION_CLASS
    {
      AuthzContextInfoUserSid = 1,
      AuthzContextInfoGroupsSids = 2,
      AuthzContextInfoRestrictedSids = 3,
      AuthzContextInfoPrivileges = 4,
      AuthzContextInfoExpirationTime = 5,
      AuthzContextInfoServerContext = 6,
      AuthzContextInfoIdentifier = 7,
      AuthzContextInfoSource = 8,
      AuthzContextInfoAll = 9,
      AuthzContextInfoAuthenticationId = 10, // 0x0000000A
    }

    internal struct LUID
    {
      internal uint LowPart;
      internal uint HighPart;
    }

    internal struct SID_AND_ATTRIBUTES
    {
      internal IntPtr Sid;
      internal uint Attributes;
    }

    internal struct TOKEN_GROUPS
    {
      internal uint GroupCount;
      internal NativeMethods.SID_AND_ATTRIBUTES Groups;
    }

    [Guid("00000016-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IOleMessageFilter
    {
      [MethodImpl(MethodImplOptions.PreserveSig)]
      int HandleInComingCall(
        int dwCallType,
        IntPtr hTaskCaller,
        int dwTickCount,
        IntPtr lpInterfaceInfo);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int RetryRejectedCall(IntPtr hTaskCallee, int dwTickCount, int dwRejectType);

      [MethodImpl(MethodImplOptions.PreserveSig)]
      int MessagePending(IntPtr hTaskCallee, int dwTickCount, int dwPendingType);
    }

    internal delegate IntPtr OfnHook(IntPtr hdlg, int msg, IntPtr wparam, IntPtr lparam);

    public struct LVCOLUMN
    {
      public int mask;
      public int fmt;
      public int cx;
      public IntPtr pszText;
      public int cchTextMax;
      public int iSubItem;
      public int iImage;
      public int iOrder;
    }

    public class Util
    {
      public static int MAKELONG(int low, int high) => high << 16 | low & (int) ushort.MaxValue;

      internal static IntPtr MAKELPARAM(int low, int high) => (IntPtr) (high << 16 | low & (int) ushort.MaxValue);

      internal static int HIWORD(int n) => n >> 16 & (int) ushort.MaxValue;

      internal static int HIWORD(IntPtr n) => NativeMethods.Util.HIWORD((int) (long) n);

      internal static int LOWORD(int n) => n & (int) ushort.MaxValue;

      internal static int LOWORD(IntPtr n) => NativeMethods.Util.LOWORD((int) (long) n);

      public static int SignedHIWORD(IntPtr n) => NativeMethods.Util.SignedHIWORD((int) (long) n);

      public static int SignedLOWORD(IntPtr n) => NativeMethods.Util.SignedLOWORD((int) (long) n);

      public static int SignedHIWORD(int n) => (int) (short) (n >> 16 & (int) ushort.MaxValue);

      public static int SignedLOWORD(int n) => (int) (short) (n & (int) ushort.MaxValue);
    }

    public struct SECURITY_ATTRIBUTES
    {
      public int nLength;
      public IntPtr lpSecurityDescriptor;
      public bool bInheritHandle;
    }

    public enum DS_SITE_NAME_ERROR
    {
      DS_SITE_NAME_NO_ERROR = 0,
      DS_SITE_NAME_ERROR_NOT_ENOUGH_MEMORY = 8,
      DS_SITE_NAME_ERROR_NOT_SITENAME = 1919, // 0x0000077F
    }

    public enum DS_NAME_ERROR
    {
      DS_NAME_NO_ERROR,
      DS_NAME_ERROR_RESOLVING,
      DS_NAME_ERROR_NOT_FOUND,
      DS_NAME_ERROR_NOT_UNIQUE,
      DS_NAME_ERROR_NO_MAPPING,
      DS_NAME_ERROR_DOMAIN_ONLY,
      DS_NAME_ERROR_NO_SYNTACTICAL_MAPPING,
      DS_NAME_ERROR_TRUST_REFERRAL,
    }

    [Flags]
    public enum DS_NAME_FLAGS
    {
      DS_NAME_NO_FLAGS = 0,
      DS_NAME_FLAG_SYNTACTICAL_ONLY = 1,
      DS_NAME_FLAG_EVAL_AT_DC = 2,
      DS_NAME_FLAG_GCVERIFY = 4,
      DS_NAME_FLAG_TRUST_REFERRAL = 8,
    }

    public enum DS_NAME_FORMAT : uint
    {
      DS_UNKNOWN_NAME = 0,
      DS_FQDN_1779_NAME = 1,
      DS_NT4_ACCOUNT_NAME = 2,
      DS_DISPLAY_NAME = 3,
      DS_UNIQUE_ID_NAME = 6,
      DS_CANONICAL_NAME = 7,
      DS_USER_PRINCIPAL_NAME = 8,
      DS_CANONICAL_NAME_EX = 9,
      DS_SERVICE_PRINCIPAL_NAME = 10, // 0x0000000A
      DS_SID_OR_SID_HISTORY_NAME = 11, // 0x0000000B
      DS_DNS_DOMAIN_NAME = 12, // 0x0000000C
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DS_NAME_RESULT_ITEM
    {
      public NativeMethods.DS_NAME_ERROR status;
      public string pDomain;
      public string pName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct DS_NAME_RESULT
    {
      internal uint cItems;
      internal IntPtr rItems;
    }

    [BestFitMapping(false, ThrowOnUnmappableChar = true)]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal sealed class OSVersionInfoEx
    {
      internal const uint VER_NT_WORKSTATION = 1;
      internal const uint VER_NT_DOMAIN_CONTROLLER = 2;
      internal const uint VER_NT_SERVER = 3;
      internal const uint VER_SUITE_ENTERPRISE = 2;
      internal const uint VER_SUITE_DATACENTER = 128;
      internal const uint VER_SUITE_PERSONAL = 512;
      internal const uint VER_SUITE_BLADE = 1024;
      private static NativeMethods.OSVersionInfoEx s_versionInfo;
      internal int osVersionInfoSize;
      internal int majorVersion;
      internal int minorVersion;
      internal int buildNumber;
      internal int platformId;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
      internal string csdVersion;
      internal short servicePackMajor;
      internal short servicePackMinor;
      internal short suiteMask;
      internal byte productType;
      internal byte reserved;

      internal static NativeMethods.OSVersionInfoEx GetOsVersionInfo()
      {
        if (NativeMethods.OSVersionInfoEx.s_versionInfo == null)
        {
          NativeMethods.OSVersionInfoEx ver = new NativeMethods.OSVersionInfoEx();
          NativeMethods.OSVersionInfoEx.s_versionInfo = NativeMethods.GetVersionEx(ver) ? ver : throw new Win32Exception(Marshal.GetLastWin32Error());
        }
        return NativeMethods.OSVersionInfoEx.s_versionInfo;
      }

      internal OSVersionInfoEx() => this.osVersionInfoSize = Marshal.SizeOf<NativeMethods.OSVersionInfoEx>(this);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4, CharSet = CharSet.Auto)]
    public struct CHARFORMAT2
    {
      public int cbSize;
      public int dwMask;
      public int dwEffects;
      public int yHeight;
      public int yOffset;
      public int crTextColor;
      public byte bCharSet;
      public byte bPitchAndFamily;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
      public string szFaceName;
      public short wWeight;
      public short sSpacing;
      public int crBackColor;
      public int lcid;
      public int dwReserved;
      public short sStyle;
      public short wKerning;
      public byte bUnderlineType;
      public byte bAnimation;
      public byte bRevAuthor;
      public byte bReserved1;
    }

    public static class WindowsEditions
    {
      public const int PRODUCT_UNDEFINED = 0;
      public const int PRODUCT_ULTIMATE = 1;
      public const int PRODUCT_HOME_BASIC = 2;
      public const int PRODUCT_HOME_PREMIUM = 3;
      public const int PRODUCT_HOME_BASIC_N = 5;
      public const int PRODUCT_STARTER = 11;
      public const int PRODUCT_STARTER_N = 47;
      public const int PRODUCT_PROFESSIONAL = 48;
      public const int PRODUCT_PROFESSIONAL_N = 49;
      public const int PRODUCT_HOME_PREMIUM_N = 26;
      public const int PRODUCT_ULTIMATE_N = 28;
      public const int PRODUCT_ENTERPRISE = 4;
      public const int PRODUCT_ENTERPRISE_N = 27;
      public const int PRODUCT_BUSINESS = 6;
      public const int PRODUCT_BUSINESS_N = 16;
      public const int PRODUCT_CORE = 101;
      public const int PRODUCT_CORE_N = 98;
      public const int PRODUCT_CORE_COUNTRYSPECIFIC = 99;
      public const int PRODUCT_CORE_SINGLELANGUAGE = 100;
      public const int PRODUCT_ENTERPRISE_SERVER = 10;
      public const int PRODUCT_ENTERPRISE_SERVER_CORE = 14;
      public const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 41;
      public const int PRODUCT_ENTERPRISE_SERVER_IA64 = 15;
      public const int PRODUCT_ENTERPRISE_SERVER_V = 38;
      public const int PRODUCT_DATACENTER_SERVER = 8;
      public const int PRODUCT_DATACENTER_SERVER_CORE = 12;
      public const int PRODUCT_DATACENTER_SERVER_CORE_V = 39;
      public const int PRODUCT_DATACENTER_SERVER_V = 37;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 59;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 60;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 61;
      public const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 62;
      public const int PRODUCT_STANDARD_SERVER = 7;
      public const int PRODUCT_STANDARD_SERVER_CORE = 13;
      public const int PRODUCT_STANDARD_SERVER_V = 36;
      public const int PRODUCT_STANDARD_SERVER_CORE_V = 40;
      public const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 52;
      public const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 53;
    }

    [Flags]
    public enum ResourceType : uint
    {
      RESOURCETYPE_ANY = 0,
      RESOURCETYPE_DISK = 1,
      RESOURCETYPE_PRINT = 2,
      RESOURCETYPE_RESERVED = 4294967295, // 0xFFFFFFFF
    }

    public enum ResourceScope
    {
      RESOURCE_CONNECTED = 1,
      RESOURCE_GLOBALNET = 2,
      RESOURCE_REMEMBERED = 3,
      RESOURCE_RECENT = 4,
      RESOURCE_CONTEXT = 5,
    }

    [Flags]
    internal enum ConnectFlags : uint
    {
      CONNECT_UPDATE_PROFILE = 1,
      CONNECT_UPDATE_RECENT = 2,
      CONNECT_TEMPORARY = 4,
      CONNECT_INTERACTIVE = 8,
      CONNECT_PROMPT = 16, // 0x00000010
      CONNECT_NEED_DRIVE = 32, // 0x00000020
      CONNECT_REFCOUNT = 64, // 0x00000040
      CONNECT_REDIRECT = 128, // 0x00000080
      CONNECT_LOCALDRIVE = 256, // 0x00000100
      CONNECT_CURRENT_MEDIA = 512, // 0x00000200
      CONNECT_DEFERRED = 1024, // 0x00000400
      CONNECT_COMMANDLINE = 2048, // 0x00000800
      CONNECT_CMD_SAVECRED = 4096, // 0x00001000
      CONNECT_RESERVED = 4278190080, // 0xFF000000
    }

    public enum ResourceDisplayType
    {
      RESOURCEDISPLAYTYPE_GENERIC,
      RESOURCEDISPLAYTYPE_DOMAIN,
      RESOURCEDISPLAYTYPE_SERVER,
      RESOURCEDISPLAYTYPE_SHARE,
      RESOURCEDISPLAYTYPE_FILE,
      RESOURCEDISPLAYTYPE_GROUP,
      RESOURCEDISPLAYTYPE_NETWORK,
      RESOURCEDISPLAYTYPE_ROOT,
      RESOURCEDISPLAYTYPE_SHAREADMIN,
      RESOURCEDISPLAYTYPE_DIRECTORY,
      RESOURCEDISPLAYTYPE_TREE,
      RESOURCEDISPLAYTYPE_NDSCONTAINER,
    }

    [Flags]
    public enum ResourceUsageType
    {
      RESOURCEUSAGE_CONNECTABLE = 1,
      RESOURCEUSAGE_CONTAINER = 2,
      RESOURCEUSAGE_NOLOCALDEVICE = 4,
      RESOURCEUSAGE_SIBLING = 8,
      RESOURCEUSAGE_ATTACHED = 16, // 0x00000010
      RESOURCEUSAGE_ALL = RESOURCEUSAGE_ATTACHED | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_CONNECTABLE, // 0x00000013
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public class NETRESOURCE
    {
      [MarshalAs(UnmanagedType.U4)]
      public NativeMethods.ResourceScope dwScope;
      [MarshalAs(UnmanagedType.U4)]
      public NativeMethods.ResourceType dwType;
      [MarshalAs(UnmanagedType.U4)]
      public NativeMethods.ResourceDisplayType dwDisplayType;
      [MarshalAs(UnmanagedType.U4)]
      public NativeMethods.ResourceUsageType dwUsage;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string LocalName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string RemoteName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Comment;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Provider;
    }

    public struct BCRYPT_PKCS1_PADDING_INFO
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszAlgId;
    }

    public struct BCRYPT_OAEP_PADDING_INFO
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszAlgId;
      public IntPtr pbLabel;
      public uint cbLabel;
    }

    public enum COMPUTER_NAME_FORMAT
    {
      ComputerNameNetBIOS,
      ComputerNameDnsHostname,
      ComputerNameDnsDomain,
      ComputerNameDnsFullyQualified,
      ComputerNamePhysicalNetBIOS,
      ComputerNamePhysicalDnsHostname,
      ComputerNamePhysicalDnsDomain,
      ComputerNamePhysicalDnsFullyQualified,
      ComputerNameMax,
    }

    public enum NetJoinStatus
    {
      NetSetupUnknownStatus,
      NetSetupUnjoined,
      NetSetupWorkgroupName,
      NetSetupDomainName,
    }

    public struct CREDUI_INFO
    {
      public int cbSize;
      public IntPtr hwndParent;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszMessageText;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pszCaptionText;
      public IntPtr hbmBanner;
    }

    public struct CREDENTIAL
    {
      public int Flags;
      public int Type;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string TargetName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Comment;
      public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
      public int CredentialBlobSize;
      public IntPtr CredentialBlob;
      public int Persist;
      public int AttributeCount;
      public IntPtr Attributes;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string TargetAlias;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string UserName;
    }

    public struct CREDENTIAL_ATTRIBUTE
    {
      [MarshalAs(UnmanagedType.LPWStr)]
      public string Keyword;
      public int Flags;
      public int ValueSize;
      public IntPtr Value;
    }

    public struct GUITHREADINFO
    {
      public int cbSize;
      public int flags;
      public IntPtr hwndActive;
      public IntPtr hwndFocus;
      public IntPtr hwndCapture;
      public IntPtr hwndMenuOwner;
      public IntPtr hwndMoveSize;
      public IntPtr hwndCaret;
      public Rectangle rcCaret;
    }

    [Flags]
    public enum ServiceAccessRights : uint
    {
      SERVICE_QUERY_CONFIG = 1,
      SERVICE_CHANGE_CONFIG = 2,
      SERVICE_QUERY_STATUS = 4,
      SERVICE_ENUMERATE_DEPENDENTS = 8,
      SERVICE_START = 16, // 0x00000010
      SERVICE_STOP = 32, // 0x00000020
      SERVICE_PAUSE_CONTINUE = 64, // 0x00000040
      SERVICE_INTERROGATE = 128, // 0x00000080
      SERVICE_USER_DEFINED_CONTROL = 256, // 0x00000100
      SERVICE_ALL_ACCESS = 983551, // 0x000F01FF
    }

    [Flags]
    public enum ServiceControlAccessRights : uint
    {
      SC_MANAGER_CONNECT = 1,
      SC_MANAGER_CREATE_SERVICE = 2,
      SC_MANAGER_ENUMERATE_SERVICE = 4,
      SC_MANAGER_LOCK = 8,
      SC_MANAGER_QUERY_LOCK_STATUS = 16, // 0x00000010
      SC_MANAGER_MODIFY_BOOT_CONFIG = 32, // 0x00000020
      SC_MANAGER_ALL_ACCESS = 983103, // 0x000F003F
    }

    public enum ServiceConfig2InfoLevel : uint
    {
      SERVICE_CONFIG_DESCRIPTION = 1,
      SERVICE_CONFIG_FAILURE_ACTIONS = 2,
      SERVICE_CONFIG_FAILURE_ACTIONS_FLAG = 4,
    }

    public enum SC_ACTION_TYPE : uint
    {
      SC_ACTION_NONE,
      SC_ACTION_RESTART,
      SC_ACTION_REBOOT,
      SC_ACTION_RUN_COMMAND,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct QUERY_SERVICE_CONFIG
    {
      public int serviceType;
      public int startType;
      public int errorControl;
      public string binaryPathName;
      public string loadOrderGroup;
      public int tagId;
      public string dependencies;
      public string serviceStartName;
      public string displayName;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SERVICE_FAILURE_ACTIONS
    {
      public uint dwResetPeriod;
      public string lpRebootMsg;
      public string lpCommand;
      public uint cActions;
      public IntPtr lpsaActions;
    }

    public struct SERVICE_FAILURE_ACTIONS_FLAG
    {
      public bool FailureActionsOnNonCrashFailures;
    }

    public struct SC_ACTION
    {
      public NativeMethods.SC_ACTION_TYPE Type;
      public uint Delay;
    }

    public struct PROCESS_INFORMATION
    {
      public IntPtr hProcess;
      public IntPtr hThread;
      public uint dwProcessId;
      public uint dwThreadId;
    }

    public struct STARTUPINFO
    {
      public int cb;
      public string lpReserved;
      public string lpDesktop;
      public string lpTitle;
      public uint dwX;
      public uint dwY;
      public uint dwXSize;
      public uint dwYSize;
      public uint dwXCountChars;
      public uint dwYCountChars;
      public uint dwFillAttribute;
      public uint dwFlags;
      public short wShowWindow;
      public short cbReserved2;
      public IntPtr lpReserved2;
      public IntPtr hStdInput;
      public IntPtr hStdOutput;
      public IntPtr hStdError;
    }

    public delegate bool ConsoleCtrlEventHandler(uint dwCtrlType);

    public struct PROFILEINFO
    {
      public int dwSize;
      public int dwFlags;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpUserName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpProfilePath;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpDefaultPath;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpServerName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string lpPolicyPath;
      public IntPtr hProfile;
    }

    public enum WTS_TYPE_CLASS
    {
      WTSTypeProcessInfoLevel0,
      WTSTypeProcessInfoLevel1,
      WTSTypeSessionInfoLevel1,
    }

    public enum WTS_CONNECTSTATE_CLASS
    {
      WTSActive,
      WTSConnected,
      WTSConnectQuery,
      WTSShadow,
      WTSDisconnected,
      WTSIdle,
      WTSListen,
      WTSReset,
      WTSDown,
      WTSInit,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WTS_SESSION_INFO_1
    {
      public uint ExecEnvId;
      public NativeMethods.WTS_CONNECTSTATE_CLASS State;
      public uint SessionId;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pSessionName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pHostName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pUserName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pDomainName;
      [MarshalAs(UnmanagedType.LPWStr)]
      public string pFarmName;
    }

    public struct LSA_UNICODE_STRING
    {
      public ushort Length;
      public ushort MaximumLength;
      public IntPtr Buffer;
    }

    public struct LSA_OBJECT_ATTRIBUTES
    {
      public uint Length;
      public IntPtr RootDirectory;
      public NativeMethods.LSA_UNICODE_STRING ObjectName;
      public uint Attributes;
      public IntPtr SecurityDescriptor;
      public IntPtr SecurityQualityOfService;
    }

    public struct ACL_SIZE_INFORMATION
    {
      public uint AceCount;
      public uint AclBytesInUse;
      public uint AclBytesFree;
    }

    public enum ACL_INFORMATION_CLASS
    {
      AclRevisionInformation = 1,
      AclSizeInformation = 2,
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0000000B-0000-0000-C000-000000000046")]
    [ComImport]
    public interface IStorage
    {
      [return: MarshalAs(UnmanagedType.Interface)]
      IStream CreateStream([MarshalAs(UnmanagedType.BStr), In] string pwcsName, [MarshalAs(UnmanagedType.U4), In] int grfMode, [MarshalAs(UnmanagedType.U4), In] int reserved1, [MarshalAs(UnmanagedType.U4), In] int reserved2);

      [return: MarshalAs(UnmanagedType.Interface)]
      IStream OpenStream([MarshalAs(UnmanagedType.BStr), In] string pwcsName, IntPtr reserved1, [MarshalAs(UnmanagedType.U4), In] int grfMode, [MarshalAs(UnmanagedType.U4), In] int reserved2);

      [return: MarshalAs(UnmanagedType.Interface)]
      NativeMethods.IStorage CreateStorage(
        [MarshalAs(UnmanagedType.BStr), In] string pwcsName,
        [MarshalAs(UnmanagedType.U4), In] int grfMode,
        [MarshalAs(UnmanagedType.U4), In] int reserved1,
        [MarshalAs(UnmanagedType.U4), In] int reserved2);

      [return: MarshalAs(UnmanagedType.Interface)]
      NativeMethods.IStorage OpenStorage(
        [MarshalAs(UnmanagedType.BStr), In] string pwcsName,
        IntPtr pstgPriority,
        [MarshalAs(UnmanagedType.U4), In] int grfMode,
        IntPtr snbExclude,
        [MarshalAs(UnmanagedType.U4), In] int reserved);

      void CopyTo(
        int ciidExclude,
        [MarshalAs(UnmanagedType.LPArray), In] Guid[] pIIDExclude,
        IntPtr snbExclude,
        [MarshalAs(UnmanagedType.Interface), In] NativeMethods.IStorage stgDest);

      void MoveElementTo(
        [MarshalAs(UnmanagedType.BStr), In] string pwcsName,
        [MarshalAs(UnmanagedType.Interface), In] NativeMethods.IStorage stgDest,
        [MarshalAs(UnmanagedType.BStr), In] string pwcsNewName,
        [MarshalAs(UnmanagedType.U4), In] int grfFlags);

      void Commit(int grfCommitFlags);

      void Revert();

      void EnumElements([MarshalAs(UnmanagedType.U4), In] int reserved1, IntPtr reserved2, [MarshalAs(UnmanagedType.U4), In] int reserved3, [MarshalAs(UnmanagedType.Interface)] out object ppVal);

      void DestroyElement([MarshalAs(UnmanagedType.BStr), In] string pwcsName);

      void RenameElement([MarshalAs(UnmanagedType.BStr), In] string pwcsOldName, [MarshalAs(UnmanagedType.BStr), In] string pwcsNewName);

      void SetElementTimes([MarshalAs(UnmanagedType.BStr), In] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

      void SetClass([In] ref Guid clsid);

      void SetStateBits(int grfStateBits, int grfMask);

      void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
    }

    [Guid("0000000A-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface ILockBytes
    {
      void ReadAt([MarshalAs(UnmanagedType.U8), In] long ulOffset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1), Out] byte[] pv, [MarshalAs(UnmanagedType.U4), In] int cb, [MarshalAs(UnmanagedType.LPArray), Out] int[] pcbRead);

      void WriteAt([MarshalAs(UnmanagedType.U8), In] long ulOffset, IntPtr pv, [MarshalAs(UnmanagedType.U4), In] int cb, [MarshalAs(UnmanagedType.LPArray), Out] int[] pcbWritten);

      void Flush();

      void SetSize([MarshalAs(UnmanagedType.U8), In] long cb);

      void LockRegion([MarshalAs(UnmanagedType.U8), In] long libOffset, [MarshalAs(UnmanagedType.U8), In] long cb, [MarshalAs(UnmanagedType.U4), In] int dwLockType);

      void UnlockRegion([MarshalAs(UnmanagedType.U8), In] long libOffset, [MarshalAs(UnmanagedType.U8), In] long cb, [MarshalAs(UnmanagedType.U4), In] int dwLockType);

      void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [MarshalAs(UnmanagedType.U4), In] int grfStatFlag);
    }

    public struct SYSTEM_INFO
    {
      public ushort wProcessorArchitecture;
      public ushort wReserved;
      public uint dwPageSize;
      public IntPtr lpMinimumApplicationAddress;
      public IntPtr lpMaximumApplicationAddress;
      public UIntPtr dwActiveProcessorMask;
      public uint dwNumberOfProcessors;
      public uint dwProcessorType;
      public uint dwAllocationGranularity;
      public ushort wProcessorLevel;
      public ushort wProcessorRevision;
    }

    public enum ProcessorArchitecture
    {
      PROCESSOR_ARCHITECTURE_INTEL = 0,
      PROCESSOR_ARCHITECTURE_IA64 = 6,
      PROCESSOR_ARCHITECTURE_AMD64 = 9,
    }

    public enum FINDEX_INFO_LEVELS
    {
      FindExInfoStandard,
      FindExInfoBasic,
    }

    public enum FINDEX_SEARCH_OPS
    {
      FindExSearchNameMatch,
      FindExSearchLimitToDirectories,
      FindExSearchLimitToDevices,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WIN32_FIND_DATA
    {
      public uint dwFileAttributes;
      public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
      public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
      public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
      public uint nFileSizeHigh;
      public uint nFileSizeLow;
      public uint dwReserved0;
      public uint dwReserved1;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
      public string strFileName;
      [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
      public string strAlternateFileName;
    }

    public sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private SafeFindHandle()
        : base(true)
      {
      }

      protected override bool ReleaseHandle() => NativeMethods.FindClose(this.handle);
    }

    public struct OVERLAPPED
    {
      public IntPtr InternalLow;
      public IntPtr InternalHigh;
      public int OffsetLow;
      public int OffsetHigh;
      public IntPtr EventHandle;
    }

    public sealed class SafeVolumeMountPointHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
      private SafeVolumeMountPointHandle()
        : base(true)
      {
      }

      protected override bool ReleaseHandle() => NativeMethods.FindVolumeMountPointClose(this.handle);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct FILE_IO_PRIORITY_HINT_INFO
    {
      public NativeMethods.PRIORITY_HINT PriorityHint;
    }

    public struct FILE_END_OF_FILE_INFO
    {
      public long EndOfFile;
    }

    public enum PRIORITY_HINT : uint
    {
      IoPriorityHintVeryLow,
      IoPriorityHintLow,
      IoPriorityHintNormal,
      MaximumIoPriorityHintType,
    }

    public enum FILE_INFO_BY_HANDLE_CLASS : uint
    {
      FileBasicInfo,
      FileStandardInfo,
      FileNameInfo,
      FileRenameInfo,
      FileDispositionInfo,
      FileAllocationInfo,
      FileEndOfFileInfo,
      FileStreamInfo,
      FileCompressionInfo,
      FileAttributeTagInfo,
      FileIdBothDirectoryInfo,
      FileIdBothDirectoryRestartInfo,
      FileIoPriorityHintInfo,
      FileRemoteProtocolInfo,
      MaximumFileInfoByHandlesClass,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MEMORYSTATUSEX
    {
      public uint dwLength;
      public uint dwMemoryLoad;
      public ulong ullTotalPhys;
      public ulong ullAvailPhys;
      public ulong ullTotalPageFile;
      public ulong ullAvailPageFile;
      public ulong ullTotalVirtual;
      public ulong ullAvailVirtual;
      public ulong ullAvailExtendedVirtual;

      public MEMORYSTATUSEX() => this.dwLength = (uint) Marshal.SizeOf(typeof (NativeMethods.MEMORYSTATUSEX));
    }

    public enum NET_API_STATUS
    {
      NERR_Success = 0,
      ERROR_ACCESS_DENIED = 5,
      ERROR_NOT_ENOUGH_MEMORY = 8,
      ERROR_INVALID_PARAMETER = 87, // 0x00000057
      ERROR_INVALID_NAME = 123, // 0x0000007B
      ERROR_INVALID_LEVEL = 124, // 0x0000007C
      ERROR_MORE_DATA = 234, // 0x000000EA
      ERROR_SESSION_CREDENTIAL_CONFLICT = 1219, // 0x000004C3
      NERR_BadPassword = 2203, // 0x0000089B
      NERR_UserNotFound = 2221, // 0x000008AD
      NERR_NotPrimary = 2226, // 0x000008B2
      NERR_SpeGroupOp = 2234, // 0x000008BA
      NERR_PasswordTooShort = 2245, // 0x000008C5
      NERR_InvalidComputer = 2351, // 0x0000092F
      NERR_LastAdmin = 2452, // 0x00000994
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct USER_INFO_24
    {
      public bool usri24_internet_identity;
      public int usri24_flags;
      public string usri24_internet_provider_name;
      public string usri24_internet_principal_name;
      public IntPtr usri24_user_sid;
    }
  }
}
