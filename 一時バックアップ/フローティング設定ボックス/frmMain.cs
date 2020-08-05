using System;
using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
using System.Windows.Forms;
//using System.Data;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using RigidChips;


//プロジェクト名:「RCD Modeler」(仮
namespace rcm {
	public class frmMain : System.Windows.Forms.Form {
		struct DragSign{
			public bool Draging;
			public int StartX ,StartY;
			public int PrevX,  PrevY;
		}
		enum UndoType{
			None,
			Added,
			Removed,
			Modified,
			LeftRotated,
			RightRotated,
			HorizonalReversed,
			VerticalReversed,
		}

		class UndoInfo{
			public UndoType type = UndoType.None;
			public RcChipBase[] chips = null;
			public UndoInfo next = null;
		}

		Device device = null;              // 1. Create rendering device
		PresentParameters presentParams = new PresentParameters();

		CustomVertex.PositionOnly[] lineCV;
	
		bool CameraOrtho = false;
		float CamTheta = -(float)Math.PI / 4f;
		float CamPhi = (float)Math.PI * 7f / 8f;
		float CamDepth = 8f;
		VertexBuffer vbGuide = null;
		VertexBuffer vbWeight = null;

		DragSign draging;
		int MouseX,MouseY;
		bool LeastIsLeftButton = false;
		bool angledrag = false;
		bool palettedrag = false;

		Vector3 CamNow;
		Vector3 CamNext;
		int ScrollCount = 0;

		ToolBarButton selected;

		bool Pause = false;

		UndoInfo undo = new UndoInfo();

		const double RadToDeg = 180.0 / Math.PI;
		const double DegToRad = Math.PI / 180.0;

		RcChipBase clipboard = null;
		RcJointPosition jointPositionBuffer = RcJointPosition.NULL;

		public RcData rcdata;
		RcDrawOptions drawOption = new RcDrawOptions();
		RcOutputOptions outputOption = new RcOutputOptions();
		RcEditOptions editOption = new RcEditOptions();
		RcXFile weightBall;
		RcXFile multiSelCursor;

		frmConfig configwindow;

		bool modified;
		bool parameterChanged;
		string editingFileName;

		string[] Arguments;

		private System.Windows.Forms.TabControl tabGI;
		private System.Windows.Forms.TabPage tpAngle;
		private System.Windows.Forms.TabPage tpPalette;
		private System.Windows.Forms.Label labelPaletteError;
		private System.Windows.Forms.ContextMenu ctmPalette;
		private System.Windows.Forms.MenuItem miPaletteAllPaint;
		private System.Windows.Forms.MenuItem miPaletteChildPaint;
		private System.Windows.Forms.MenuItem miFile;
		private System.Windows.Forms.MenuItem miFileNew;
		private System.Windows.Forms.MenuItem miFileOpen;
		private System.Windows.Forms.MenuItem miFileSave;
		private System.Windows.Forms.MenuItem miFileSaveAs;
		private System.Windows.Forms.MenuItem miFileImport;
		private System.Windows.Forms.MenuItem miFileExport;
		private System.Windows.Forms.ContextMenu ctmAngles;
		private System.Windows.Forms.MenuItem miListCut;
		private System.Windows.Forms.MenuItem miListCopy;
		private System.Windows.Forms.MenuItem miListAdd;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem miListSelect;
		private System.Windows.Forms.MenuItem miChipComment;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem miCommentEdit;
		private System.Windows.Forms.MenuItem miCommentDelete;
		private System.Windows.Forms.MenuItem miTool;
		private System.Windows.Forms.MenuItem miToolVal;
		private System.Windows.Forms.MenuItem miToolKey;
		private System.Windows.Forms.MenuItem miToolScript;
		private System.Windows.Forms.MenuItem miConfig;
		private System.Windows.Forms.MenuItem miConfigDraw;
		private System.Windows.Forms.MenuItem miConfigOutput;
		private System.Windows.Forms.MenuItem miHelp;
		private System.Windows.Forms.MenuItem miHelpOpen;
		private System.Windows.Forms.MenuItem miHelpReadme;
		private System.Windows.Forms.MenuItem miHelpVersion;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem miToolComment;
		private System.Windows.Forms.MenuItem miToolPrev;
		private System.Windows.Forms.MenuItem miToolSend;
		private System.Windows.Forms.MenuItem miToolTree;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miFileQuit;
		private System.Windows.Forms.MenuItem miConfigEdit;
		private System.Windows.Forms.MenuItem miAngleGrid0;
		private System.Windows.Forms.MenuItem miAngleGrid1;
		private System.Windows.Forms.MenuItem miAngleGrid5;
		private System.Windows.Forms.MenuItem miAngleGrid15;
		private System.Windows.Forms.MenuItem miAngleGrid30;
		private System.Windows.Forms.MenuItem miAngleGrid90;
		private System.Windows.Forms.MenuItem miEdit;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.MenuItem menuItem14;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem menuItem18;
		bool Initialized = false;

		[DllImport("gdi32.dll")]
			static extern int GetPixel(IntPtr hDC, int dwX, int dwY);
		[DllImport("user32.dll")]
			static extern int RegisterWindowMessageA(string msgId);
		[DllImport("user32.dll")]
			static extern int PostMessageA(int hwd,int msg,int wparam,int lparam);
		const int hWnd_Broadcast = -1;

		const int Msg_RcLoadStart = 0;
		const int Msg_RcLoadChar = 1;
		const int Msg_RcLoadEnd = 2;
		int WM_RIGHTCHIP_LOAD;

		private rcm.frmTree treeview;
		private rcm.frmScript scriptform;
																																				
		private System.Windows.Forms.ToolBarButton tbbCursor;
		private System.Windows.Forms.ToolBarButton tbbCut;
		private System.Windows.Forms.ToolBarButton tbbCopy;
		private System.Windows.Forms.ToolBarButton tbbPaste;
		private System.Windows.Forms.ToolBarButton tbbRemove;
		private System.Windows.Forms.Panel panelB;
		private System.Windows.Forms.ComboBox cmbColor;
		private System.Windows.Forms.ListBox lstSouth;
		private System.Windows.Forms.ListBox lstNorth;
		private System.Windows.Forms.ListBox lstEast;
		private System.Windows.Forms.ListBox lstWest;
		private System.Windows.Forms.MainMenu mnuMain;
		private System.Windows.Forms.Label labelTip;
		private System.Windows.Forms.ToolBar tbMain;
		private System.Windows.Forms.TextBox txtName;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelColor;
		private System.Windows.Forms.Button buttonSelChip;
		private System.Windows.Forms.Panel panelAttr;
		private System.Windows.Forms.Panel panelAttrName;
		private System.Windows.Forms.Splitter splAttr;
		private System.Windows.Forms.Panel panelAttrValue;
		private System.Windows.Forms.Label labelAttrName;
		private System.Windows.Forms.Label labelAttrValue;
		private System.Windows.Forms.Label labelAttrItem1;
		private System.Windows.Forms.Label labelAttrItem2;
		private System.Windows.Forms.Label labelAttrItem3;
		private System.Windows.Forms.Label labelAttrItem4;
		private System.Windows.Forms.Label labelAttrItem5;
		private System.Windows.Forms.Label labelAttrItem6;
		private System.Windows.Forms.ComboBox cmbAttrItem1;
		private System.Windows.Forms.ComboBox cmbAttrItem2;
		private System.Windows.Forms.ComboBox cmbAttrItem3;
		private System.Windows.Forms.ComboBox cmbAttrItem4;
		private System.Windows.Forms.ComboBox cmbAttrItem5;
		private System.Windows.Forms.ComboBox cmbAttrItem6;
		private System.Windows.Forms.Button btnVal;
		private System.Windows.Forms.Button btnKey;
		private System.Windows.Forms.Timer tmr;
		private System.Windows.Forms.ToolBarButton tbbChip;
		private System.Windows.Forms.ToolBarButton tbbFrame;
		private System.Windows.Forms.ToolBarButton tbbWeight;
		private System.Windows.Forms.ToolBarButton tbbCowl;
		private System.Windows.Forms.ToolBarButton tbbRudder;
		private System.Windows.Forms.ToolBarButton tbbRudderF;
		private System.Windows.Forms.ToolBarButton tbbTrim;
		private System.Windows.Forms.ToolBarButton tbbTrimF;
		private System.Windows.Forms.ToolBarButton tbbWheel;
		private System.Windows.Forms.ToolBarButton tbbRLW;
		private System.Windows.Forms.ToolBarButton tbbJet;
		private System.Windows.Forms.ToolBarButton tbbArm;
		private System.Windows.Forms.ToolBarButton tbbSeparator1;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.ToolBarButton tbbSeparator2;
		private System.Windows.Forms.ToolBarButton tbbSeparator3;
		private System.Windows.Forms.ToolTip ttMain;

		private System.Windows.Forms.Label[] labelAttrItems;
		private System.Windows.Forms.Timer tmrScroll;
		private System.Windows.Forms.ToolBarButton tbbZoom;
		private System.Windows.Forms.ToolBarButton tbbMooz;
		private System.Windows.Forms.Button btnRootChip;
		private System.Windows.Forms.ToolBarButton tbbInsert;
		private System.Windows.Forms.ToolBarButton tbbSeparator4;
		private System.Windows.Forms.ContextMenu ctmChipType;
		private System.Windows.Forms.MenuItem miChangeChip;
		private System.Windows.Forms.MenuItem miChangeFrame;
		private System.Windows.Forms.MenuItem miChangeRudder;
		private System.Windows.Forms.MenuItem miChangeRudderF;
		private System.Windows.Forms.MenuItem miChangeTrim;
		private System.Windows.Forms.MenuItem miChangeTrimF;
		private System.Windows.Forms.MenuItem ctmSeparator;
		private System.Windows.Forms.MenuItem miChangeWeight;
		private System.Windows.Forms.MenuItem miChangeCowl;
		private System.Windows.Forms.MenuItem miChangeWheel;
		private System.Windows.Forms.MenuItem miChangeRLW;
		private System.Windows.Forms.MenuItem miChangeJet;
		private System.Windows.Forms.MenuItem miChangeArm;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.ImageList imgIconsL;
		private System.Windows.Forms.ComboBox cmbAttrItem7;
		private System.Windows.Forms.Label labelAttrItem7;
		private System.Windows.Forms.Label labelAttrItem8;
		private System.Windows.Forms.Label labelAttrItem9;
		private System.Windows.Forms.ComboBox cmbAttrItem8;
		private System.Windows.Forms.ComboBox cmbAttrItem9;
		private System.Windows.Forms.ContextMenu ctmChildList;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem miCut;
		private System.Windows.Forms.MenuItem miCopy;
		private System.Windows.Forms.MenuItem miDelete;
		private System.Windows.Forms.MenuItem miChange;
		private System.Windows.Forms.MenuItem miRotateRight;
		private System.Windows.Forms.MenuItem miRotateLeft;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem miReverseX;
		private System.Windows.Forms.MenuItem miReverseY;
		private System.Windows.Forms.MenuItem miReverseZ;
		private System.Windows.Forms.PictureBox pictTarget;
		private System.Windows.Forms.ToolBarButton tbbAutoCamera;
		private System.Windows.Forms.ToolBarButton tbbCamera;
		private System.Windows.Forms.PictureBox pictAngle;
		private System.Windows.Forms.OpenFileDialog dlgOpen;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		private System.Windows.Forms.PictureBox pictPalette;
		private System.Windows.Forms.ComboBox[] cmbAttrItems;
		private System.Windows.Forms.MenuItem[] miChangeTypeList;

		public frmMain(string[] args) {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			#region	凡調なる配列の作成
			labelAttrItems = new Label[9];
			labelAttrItems[0] = labelAttrItem1;
			labelAttrItems[1] = labelAttrItem2;
			labelAttrItems[2] = labelAttrItem3;
			labelAttrItems[3] = labelAttrItem4;
			labelAttrItems[4] = labelAttrItem5;
			labelAttrItems[5] = labelAttrItem6;
			labelAttrItems[6] = labelAttrItem7;
			labelAttrItems[7] = labelAttrItem8;
			labelAttrItems[8] = labelAttrItem9;
			cmbAttrItems = new ComboBox[9];
			cmbAttrItems[0] = cmbAttrItem1;
			cmbAttrItems[1] = cmbAttrItem2;
			cmbAttrItems[2] = cmbAttrItem3;
			cmbAttrItems[3] = cmbAttrItem4;
			cmbAttrItems[4] = cmbAttrItem5;
			cmbAttrItems[5] = cmbAttrItem6;
			cmbAttrItems[6] = cmbAttrItem7;
			cmbAttrItems[7] = cmbAttrItem8;
			cmbAttrItems[8] = cmbAttrItem9;
			miChangeTypeList = new MenuItem[13];
			miChangeTypeList[0] = null;
			miChangeTypeList[1] = miChangeChip;
			miChangeTypeList[2] = miChangeRudder;
			miChangeTypeList[3] = miChangeTrim;
			miChangeTypeList[4] = miChangeFrame;
			miChangeTypeList[5] = miChangeRudderF;
			miChangeTypeList[6] = miChangeTrimF;
			miChangeTypeList[7] = miChangeWheel;
			miChangeTypeList[8] = miChangeRLW;
			miChangeTypeList[9] = miChangeJet;
			miChangeTypeList[10] = miChangeWeight;
			miChangeTypeList[11] = miChangeCowl;
			miChangeTypeList[12] = miChangeArm;
			#endregion
			Color clr = Color.FromArgb( 0);
			for( KnownColor i = KnownColor.ActiveBorder;
				i <= KnownColor.YellowGreen; i++) {
				clr = Color.FromKnownColor(i);
				if( clr.IsSystemColor == false) {
					cmbColor.Items.Add(i);
				}
			}

			Modified = false;
			EditingFileName = "";

			Arguments = args;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows フォーム デザイナで生成されたコード 
		/// <summary>
		/// デザイナ サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディタで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMain));
			this.mnuMain = new System.Windows.Forms.MainMenu();
			this.miFile = new System.Windows.Forms.MenuItem();
			this.miFileNew = new System.Windows.Forms.MenuItem();
			this.miFileOpen = new System.Windows.Forms.MenuItem();
			this.miFileSave = new System.Windows.Forms.MenuItem();
			this.miFileSaveAs = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.miFileImport = new System.Windows.Forms.MenuItem();
			this.miFileExport = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miFileQuit = new System.Windows.Forms.MenuItem();
			this.miEdit = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.menuItem15 = new System.Windows.Forms.MenuItem();
			this.miTool = new System.Windows.Forms.MenuItem();
			this.miToolVal = new System.Windows.Forms.MenuItem();
			this.miToolKey = new System.Windows.Forms.MenuItem();
			this.miToolScript = new System.Windows.Forms.MenuItem();
			this.miToolComment = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.miToolTree = new System.Windows.Forms.MenuItem();
			this.miToolPrev = new System.Windows.Forms.MenuItem();
			this.miToolSend = new System.Windows.Forms.MenuItem();
			this.miConfig = new System.Windows.Forms.MenuItem();
			this.miConfigDraw = new System.Windows.Forms.MenuItem();
			this.miConfigOutput = new System.Windows.Forms.MenuItem();
			this.miConfigEdit = new System.Windows.Forms.MenuItem();
			this.miHelp = new System.Windows.Forms.MenuItem();
			this.miHelpOpen = new System.Windows.Forms.MenuItem();
			this.miHelpReadme = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.miHelpVersion = new System.Windows.Forms.MenuItem();
			this.labelTip = new System.Windows.Forms.Label();
			this.tbMain = new System.Windows.Forms.ToolBar();
			this.tbbCursor = new System.Windows.Forms.ToolBarButton();
			this.tbbCut = new System.Windows.Forms.ToolBarButton();
			this.tbbCopy = new System.Windows.Forms.ToolBarButton();
			this.tbbPaste = new System.Windows.Forms.ToolBarButton();
			this.tbbSeparator1 = new System.Windows.Forms.ToolBarButton();
			this.tbbInsert = new System.Windows.Forms.ToolBarButton();
			this.tbbRemove = new System.Windows.Forms.ToolBarButton();
			this.tbbSeparator2 = new System.Windows.Forms.ToolBarButton();
			this.tbbZoom = new System.Windows.Forms.ToolBarButton();
			this.tbbMooz = new System.Windows.Forms.ToolBarButton();
			this.tbbCamera = new System.Windows.Forms.ToolBarButton();
			this.tbbAutoCamera = new System.Windows.Forms.ToolBarButton();
			this.tbbSeparator3 = new System.Windows.Forms.ToolBarButton();
			this.tbbChip = new System.Windows.Forms.ToolBarButton();
			this.tbbFrame = new System.Windows.Forms.ToolBarButton();
			this.tbbRudder = new System.Windows.Forms.ToolBarButton();
			this.tbbRudderF = new System.Windows.Forms.ToolBarButton();
			this.tbbTrim = new System.Windows.Forms.ToolBarButton();
			this.tbbTrimF = new System.Windows.Forms.ToolBarButton();
			this.tbbSeparator4 = new System.Windows.Forms.ToolBarButton();
			this.tbbWheel = new System.Windows.Forms.ToolBarButton();
			this.tbbRLW = new System.Windows.Forms.ToolBarButton();
			this.tbbJet = new System.Windows.Forms.ToolBarButton();
			this.tbbWeight = new System.Windows.Forms.ToolBarButton();
			this.tbbCowl = new System.Windows.Forms.ToolBarButton();
			this.tbbArm = new System.Windows.Forms.ToolBarButton();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.panelB = new System.Windows.Forms.Panel();
			this.tabGI = new System.Windows.Forms.TabControl();
			this.tpAngle = new System.Windows.Forms.TabPage();
			this.pictAngle = new System.Windows.Forms.PictureBox();
			this.ctmAngles = new System.Windows.Forms.ContextMenu();
			this.miAngleGrid0 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid1 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid5 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid15 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid30 = new System.Windows.Forms.MenuItem();
			this.miAngleGrid90 = new System.Windows.Forms.MenuItem();
			this.tpPalette = new System.Windows.Forms.TabPage();
			this.pictPalette = new System.Windows.Forms.PictureBox();
			this.ctmPalette = new System.Windows.Forms.ContextMenu();
			this.miPaletteChildPaint = new System.Windows.Forms.MenuItem();
			this.miPaletteAllPaint = new System.Windows.Forms.MenuItem();
			this.labelPaletteError = new System.Windows.Forms.Label();
			this.btnRootChip = new System.Windows.Forms.Button();
			this.btnKey = new System.Windows.Forms.Button();
			this.btnVal = new System.Windows.Forms.Button();
			this.panelAttr = new System.Windows.Forms.Panel();
			this.panelAttrValue = new System.Windows.Forms.Panel();
			this.cmbAttrItem9 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem8 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem7 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem6 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem5 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem4 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem3 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem2 = new System.Windows.Forms.ComboBox();
			this.cmbAttrItem1 = new System.Windows.Forms.ComboBox();
			this.labelAttrValue = new System.Windows.Forms.Label();
			this.splAttr = new System.Windows.Forms.Splitter();
			this.panelAttrName = new System.Windows.Forms.Panel();
			this.labelAttrItem9 = new System.Windows.Forms.Label();
			this.labelAttrItem8 = new System.Windows.Forms.Label();
			this.labelAttrItem7 = new System.Windows.Forms.Label();
			this.labelAttrItem6 = new System.Windows.Forms.Label();
			this.labelAttrItem5 = new System.Windows.Forms.Label();
			this.labelAttrItem4 = new System.Windows.Forms.Label();
			this.labelAttrItem3 = new System.Windows.Forms.Label();
			this.labelAttrItem2 = new System.Windows.Forms.Label();
			this.labelAttrItem1 = new System.Windows.Forms.Label();
			this.labelAttrName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.labelColor = new System.Windows.Forms.Label();
			this.cmbColor = new System.Windows.Forms.ComboBox();
			this.lstSouth = new System.Windows.Forms.ListBox();
			this.ctmChildList = new System.Windows.Forms.ContextMenu();
			this.miListAdd = new System.Windows.Forms.MenuItem();
			this.miListSelect = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.miListCut = new System.Windows.Forms.MenuItem();
			this.miListCopy = new System.Windows.Forms.MenuItem();
			this.lstNorth = new System.Windows.Forms.ListBox();
			this.lstEast = new System.Windows.Forms.ListBox();
			this.lstWest = new System.Windows.Forms.ListBox();
			this.buttonSelChip = new System.Windows.Forms.Button();
			this.imgIconsL = new System.Windows.Forms.ImageList(this.components);
			this.tmr = new System.Windows.Forms.Timer(this.components);
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.tmrScroll = new System.Windows.Forms.Timer(this.components);
			this.ctmChipType = new System.Windows.Forms.ContextMenu();
			this.miCut = new System.Windows.Forms.MenuItem();
			this.miCopy = new System.Windows.Forms.MenuItem();
			this.miDelete = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.miChange = new System.Windows.Forms.MenuItem();
			this.miChangeChip = new System.Windows.Forms.MenuItem();
			this.miChangeFrame = new System.Windows.Forms.MenuItem();
			this.miChangeRudder = new System.Windows.Forms.MenuItem();
			this.miChangeRudderF = new System.Windows.Forms.MenuItem();
			this.miChangeTrim = new System.Windows.Forms.MenuItem();
			this.miChangeTrimF = new System.Windows.Forms.MenuItem();
			this.ctmSeparator = new System.Windows.Forms.MenuItem();
			this.miChangeWheel = new System.Windows.Forms.MenuItem();
			this.miChangeRLW = new System.Windows.Forms.MenuItem();
			this.miChangeJet = new System.Windows.Forms.MenuItem();
			this.miChangeWeight = new System.Windows.Forms.MenuItem();
			this.miChangeCowl = new System.Windows.Forms.MenuItem();
			this.miChangeArm = new System.Windows.Forms.MenuItem();
			this.miChipComment = new System.Windows.Forms.MenuItem();
			this.miCommentEdit = new System.Windows.Forms.MenuItem();
			this.miCommentDelete = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.miRotateRight = new System.Windows.Forms.MenuItem();
			this.miRotateLeft = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.miReverseX = new System.Windows.Forms.MenuItem();
			this.miReverseY = new System.Windows.Forms.MenuItem();
			this.miReverseZ = new System.Windows.Forms.MenuItem();
			this.pictTarget = new System.Windows.Forms.PictureBox();
			this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
			this.dlgSave = new System.Windows.Forms.SaveFileDialog();
			this.menuItem16 = new System.Windows.Forms.MenuItem();
			this.menuItem17 = new System.Windows.Forms.MenuItem();
			this.menuItem18 = new System.Windows.Forms.MenuItem();
			this.panelB.SuspendLayout();
			this.tabGI.SuspendLayout();
			this.tpAngle.SuspendLayout();
			this.tpPalette.SuspendLayout();
			this.panelAttr.SuspendLayout();
			this.panelAttrValue.SuspendLayout();
			this.panelAttrName.SuspendLayout();
			this.SuspendLayout();
			// 
			// mnuMain
			// 
			this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.miFile,
																					this.miEdit,
																					this.miTool,
																					this.miConfig,
																					this.miHelp});
			// 
			// miFile
			// 
			this.miFile.Index = 0;
			this.miFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miFileNew,
																				   this.miFileOpen,
																				   this.miFileSave,
																				   this.miFileSaveAs,
																				   this.menuItem19,
																				   this.miFileImport,
																				   this.miFileExport,
																				   this.menuItem1,
																				   this.miFileQuit});
			this.miFile.Text = "ファイル(&F)";
			this.miFile.Click += new System.EventHandler(this.miFile_Click);
			// 
			// miFileNew
			// 
			this.miFileNew.Index = 0;
			this.miFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.miFileNew.Text = "新規モデル(&N)";
			this.miFileNew.Click += new System.EventHandler(this.miFileNew_Click);
			// 
			// miFileOpen
			// 
			this.miFileOpen.Index = 1;
			this.miFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.miFileOpen.Text = "開く(&O)";
			this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// miFileSave
			// 
			this.miFileSave.Index = 2;
			this.miFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miFileSave.Text = "上書き保存(&S)";
			this.miFileSave.Click += new System.EventHandler(this.miFileSave_Click);
			// 
			// miFileSaveAs
			// 
			this.miFileSaveAs.Index = 3;
			this.miFileSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.miFileSaveAs.Text = "名前を付けて保存(&A)";
			this.miFileSaveAs.Click += new System.EventHandler(this.miFileSaveAs_Click);
			// 
			// menuItem19
			// 
			this.menuItem19.Index = 4;
			this.menuItem19.Text = "-";
			// 
			// miFileImport
			// 
			this.miFileImport.Index = 5;
			this.miFileImport.Shortcut = System.Windows.Forms.Shortcut.CtrlI;
			this.miFileImport.Text = "RCD/TXTから開く(&I)";
			this.miFileImport.Click += new System.EventHandler(this.miFileImport_Click);
			// 
			// miFileExport
			// 
			this.miFileExport.Index = 6;
			this.miFileExport.Shortcut = System.Windows.Forms.Shortcut.CtrlE;
			this.miFileExport.Text = "RCD形式で保存(&E)";
			this.miFileExport.Click += new System.EventHandler(this.miFileExport_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 7;
			this.menuItem1.Text = "-";
			// 
			// miFileQuit
			// 
			this.miFileQuit.Index = 8;
			this.miFileQuit.Text = "アプリケーションの終了(&Q)";
			this.miFileQuit.Click += new System.EventHandler(this.miFileQuit_Click);
			// 
			// miEdit
			// 
			this.miEdit.Index = 1;
			this.miEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.menuItem4,
																				   this.menuItem2,
																				   this.menuItem16,
																				   this.menuItem6,
																				   this.menuItem9,
																				   this.menuItem17});
			this.miEdit.Text = "編集/視点(&E)";
			this.miEdit.Visible = false;
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.menuItem2.Text = "コピー(&C)";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			this.menuItem4.Text = "切り取り(&T)";
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 3;
			this.menuItem6.Text = "-";
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 4;
			this.menuItem9.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem14,
																					  this.menuItem11,
																					  this.menuItem12,
																					  this.menuItem15});
			this.menuItem9.Text = "視点変更(&V)";
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 0;
			this.menuItem14.Text = "真横(&X)";
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 1;
			this.menuItem11.Text = "真上(&Y)";
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 2;
			this.menuItem12.Text = "正面(&Z)";
			// 
			// menuItem15
			// 
			this.menuItem15.Index = 3;
			this.menuItem15.Text = "数値入力(&U)...";
			// 
			// miTool
			// 
			this.miTool.Index = 2;
			this.miTool.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miToolVal,
																				   this.miToolKey,
																				   this.miToolScript,
																				   this.miToolComment,
																				   this.menuItem3,
																				   this.miToolTree,
																				   this.miToolPrev,
																				   this.miToolSend});
			this.miTool.Text = "ツール(&T)";
			this.miTool.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// miToolVal
			// 
			this.miToolVal.Index = 0;
			this.miToolVal.Shortcut = System.Windows.Forms.Shortcut.F2;
			this.miToolVal.Text = "&Val{...} 編集";
			this.miToolVal.Click += new System.EventHandler(this.btnVal_Click);
			// 
			// miToolKey
			// 
			this.miToolKey.Index = 1;
			this.miToolKey.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.miToolKey.Text = "&Key{...} 編集";
			this.miToolKey.Click += new System.EventHandler(this.btnKey_Click);
			// 
			// miToolScript
			// 
			this.miToolScript.Index = 2;
			this.miToolScript.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.miToolScript.Text = "&Script/Lua{...} 編集";
			this.miToolScript.Click += new System.EventHandler(this.miToolScript_Click);
			// 
			// miToolComment
			// 
			this.miToolComment.Enabled = false;
			this.miToolComment.Index = 3;
			this.miToolComment.Shortcut = System.Windows.Forms.Shortcut.ShiftF4;
			this.miToolComment.Text = "ファイル冒頭コメント編集(&C)";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 4;
			this.menuItem3.Text = "-";
			// 
			// miToolTree
			// 
			this.miToolTree.Index = 5;
			this.miToolTree.Shortcut = System.Windows.Forms.Shortcut.F6;
			this.miToolTree.Text = "ツリー構造表示(&T)";
			this.miToolTree.Click += new System.EventHandler(this.miToolTree_Click);
			// 
			// miToolPrev
			// 
			this.miToolPrev.Enabled = false;
			this.miToolPrev.Index = 6;
			this.miToolPrev.Shortcut = System.Windows.Forms.Shortcut.ShiftF5;
			this.miToolPrev.Text = "簡易動作プレビュー(&P)";
			// 
			// miToolSend
			// 
			this.miToolSend.Index = 7;
			this.miToolSend.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.miToolSend.Text = "RigidChips上でプレビュー(&R)";
			this.miToolSend.Click += new System.EventHandler(this.miToolSend_Click);
			// 
			// miConfig
			// 
			this.miConfig.Index = 3;
			this.miConfig.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.miConfigDraw,
																					 this.miConfigOutput,
																					 this.miConfigEdit});
			this.miConfig.Text = "設定(&C)";
			// 
			// miConfigDraw
			// 
			this.miConfigDraw.Index = 0;
			this.miConfigDraw.Text = "表示設定(&R)";
			this.miConfigDraw.Click += new System.EventHandler(this.miConfigDraw_Click);
			// 
			// miConfigOutput
			// 
			this.miConfigOutput.Index = 1;
			this.miConfigOutput.Text = "出力形式設定(&O)";
			this.miConfigOutput.Click += new System.EventHandler(this.miConfigOutput_Click);
			// 
			// miConfigEdit
			// 
			this.miConfigEdit.Index = 2;
			this.miConfigEdit.Text = "エディット動作設定(&E)";
			this.miConfigEdit.Click += new System.EventHandler(this.miConfigEdit_Click);
			// 
			// miHelp
			// 
			this.miHelp.Index = 4;
			this.miHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				   this.miHelpOpen,
																				   this.miHelpReadme,
																				   this.menuItem13,
																				   this.miHelpVersion});
			this.miHelp.Text = "ヘルプ(&H)";
			// 
			// miHelpOpen
			// 
			this.miHelpOpen.Enabled = false;
			this.miHelpOpen.Index = 0;
			this.miHelpOpen.Text = "ヘルプを表示";
			// 
			// miHelpReadme
			// 
			this.miHelpReadme.Index = 1;
			this.miHelpReadme.Text = "RCM説明書.txt";
			this.miHelpReadme.Click += new System.EventHandler(this.miHelpReadme_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 2;
			this.menuItem13.Text = "-";
			// 
			// miHelpVersion
			// 
			this.miHelpVersion.Index = 3;
			this.miHelpVersion.Text = "バージョン情報";
			this.miHelpVersion.Click += new System.EventHandler(this.menuItem14_Click);
			// 
			// labelTip
			// 
			this.labelTip.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelTip.Location = new System.Drawing.Point(0, 28);
			this.labelTip.Name = "labelTip";
			this.labelTip.Size = new System.Drawing.Size(794, 15);
			this.labelTip.TabIndex = 8;
			this.labelTip.Text = "Welcome to RigidChips Modeler Ver.0.3β";
			// 
			// tbMain
			// 
			this.tbMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					  this.tbbCursor,
																					  this.tbbCut,
																					  this.tbbCopy,
																					  this.tbbPaste,
																					  this.tbbSeparator1,
																					  this.tbbInsert,
																					  this.tbbRemove,
																					  this.tbbSeparator2,
																					  this.tbbZoom,
																					  this.tbbMooz,
																					  this.tbbCamera,
																					  this.tbbAutoCamera,
																					  this.tbbSeparator3,
																					  this.tbbChip,
																					  this.tbbFrame,
																					  this.tbbRudder,
																					  this.tbbRudderF,
																					  this.tbbTrim,
																					  this.tbbTrimF,
																					  this.tbbSeparator4,
																					  this.tbbWheel,
																					  this.tbbRLW,
																					  this.tbbJet,
																					  this.tbbWeight,
																					  this.tbbCowl,
																					  this.tbbArm});
			this.tbMain.ButtonSize = new System.Drawing.Size(16, 16);
			this.tbMain.DropDownArrows = true;
			this.tbMain.ImageList = this.imgIcons;
			this.tbMain.Location = new System.Drawing.Point(0, 0);
			this.tbMain.Name = "tbMain";
			this.tbMain.ShowToolTips = true;
			this.tbMain.Size = new System.Drawing.Size(794, 28);
			this.tbMain.TabIndex = 0;
			this.tbMain.Wrappable = false;
			this.tbMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbMain_ButtonClick);
			// 
			// tbbCursor
			// 
			this.tbbCursor.ImageIndex = 0;
			this.tbbCursor.Pushed = true;
			this.tbbCursor.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbCursor.Text = "選択";
			this.tbbCursor.ToolTipText = "選択";
			// 
			// tbbCut
			// 
			this.tbbCut.ImageIndex = 1;
			this.tbbCut.Text = "現在のチップを切り取り";
			this.tbbCut.ToolTipText = "現在のチップを切り取り";
			// 
			// tbbCopy
			// 
			this.tbbCopy.ImageIndex = 2;
			this.tbbCopy.Text = "現在のチップをコピー";
			this.tbbCopy.ToolTipText = "現在のチップをコピー";
			// 
			// tbbPaste
			// 
			this.tbbPaste.ImageIndex = 3;
			this.tbbPaste.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbPaste.Text = "貼り付け";
			this.tbbPaste.ToolTipText = "貼り付け";
			// 
			// tbbSeparator1
			// 
			this.tbbSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbInsert
			// 
			this.tbbInsert.ImageIndex = 4;
			this.tbbInsert.Text = "チップを挿入";
			this.tbbInsert.ToolTipText = "チップを挿入";
			// 
			// tbbRemove
			// 
			this.tbbRemove.ImageIndex = 5;
			this.tbbRemove.Text = "現在のチップを削除";
			this.tbbRemove.ToolTipText = "現在のチップを削除";
			// 
			// tbbSeparator2
			// 
			this.tbbSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbZoom
			// 
			this.tbbZoom.ImageIndex = 6;
			this.tbbZoom.Text = "拡大";
			this.tbbZoom.ToolTipText = "拡大";
			// 
			// tbbMooz
			// 
			this.tbbMooz.ImageIndex = 7;
			this.tbbMooz.Text = "縮小";
			this.tbbMooz.ToolTipText = "縮小";
			// 
			// tbbCamera
			// 
			this.tbbCamera.ImageIndex = 20;
			this.tbbCamera.Text = "視点";
			this.tbbCamera.ToolTipText = "視点";
			// 
			// tbbAutoCamera
			// 
			this.tbbAutoCamera.ImageIndex = 21;
			this.tbbAutoCamera.Pushed = true;
			this.tbbAutoCamera.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbAutoCamera.ToolTipText = "カーソル自動追跡";
			// 
			// tbbSeparator3
			// 
			this.tbbSeparator3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbChip
			// 
			this.tbbChip.ImageIndex = 8;
			this.tbbChip.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbChip.Text = "チップ";
			this.tbbChip.ToolTipText = "チップ";
			// 
			// tbbFrame
			// 
			this.tbbFrame.ImageIndex = 9;
			this.tbbFrame.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbFrame.Text = "フレーム";
			this.tbbFrame.ToolTipText = "フレーム";
			// 
			// tbbRudder
			// 
			this.tbbRudder.ImageIndex = 10;
			this.tbbRudder.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbRudder.Text = "ラダー";
			this.tbbRudder.ToolTipText = "ラダー";
			// 
			// tbbRudderF
			// 
			this.tbbRudderF.ImageIndex = 11;
			this.tbbRudderF.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbRudderF.Text = "ラダーフレーム";
			this.tbbRudderF.ToolTipText = "ラダーフレーム";
			// 
			// tbbTrim
			// 
			this.tbbTrim.ImageIndex = 12;
			this.tbbTrim.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbTrim.Text = "トリム";
			this.tbbTrim.ToolTipText = "トリム";
			// 
			// tbbTrimF
			// 
			this.tbbTrimF.ImageIndex = 13;
			this.tbbTrimF.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbTrimF.Text = "トリムフレーム";
			this.tbbTrimF.ToolTipText = "トリムフレーム";
			// 
			// tbbSeparator4
			// 
			this.tbbSeparator4.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbWheel
			// 
			this.tbbWheel.ImageIndex = 14;
			this.tbbWheel.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbWheel.Text = "ホイール";
			this.tbbWheel.ToolTipText = "ホイール";
			// 
			// tbbRLW
			// 
			this.tbbRLW.ImageIndex = 15;
			this.tbbRLW.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbRLW.Text = "無反動ホイール";
			this.tbbRLW.ToolTipText = "無反動ホイール";
			// 
			// tbbJet
			// 
			this.tbbJet.ImageIndex = 16;
			this.tbbJet.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbJet.Text = "ジェット";
			this.tbbJet.ToolTipText = "ジェット";
			// 
			// tbbWeight
			// 
			this.tbbWeight.ImageIndex = 17;
			this.tbbWeight.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbWeight.Text = "ウェイト";
			this.tbbWeight.ToolTipText = "ウェイト";
			// 
			// tbbCowl
			// 
			this.tbbCowl.ImageIndex = 18;
			this.tbbCowl.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbCowl.Text = "カウル";
			this.tbbCowl.ToolTipText = "カウル";
			// 
			// tbbArm
			// 
			this.tbbArm.ImageIndex = 19;
			this.tbbArm.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbArm.Text = "アーム";
			this.tbbArm.ToolTipText = "アーム";
			// 
			// imgIcons
			// 
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(0)), ((System.Byte)(255)));
			// 
			// panelB
			// 
			this.panelB.Controls.Add(this.tabGI);
			this.panelB.Controls.Add(this.btnRootChip);
			this.panelB.Controls.Add(this.btnKey);
			this.panelB.Controls.Add(this.btnVal);
			this.panelB.Controls.Add(this.panelAttr);
			this.panelB.Controls.Add(this.txtName);
			this.panelB.Controls.Add(this.labelName);
			this.panelB.Controls.Add(this.labelColor);
			this.panelB.Controls.Add(this.cmbColor);
			this.panelB.Controls.Add(this.lstSouth);
			this.panelB.Controls.Add(this.lstNorth);
			this.panelB.Controls.Add(this.lstEast);
			this.panelB.Controls.Add(this.lstWest);
			this.panelB.Controls.Add(this.buttonSelChip);
			this.panelB.Dock = System.Windows.Forms.DockStyle.Right;
			this.panelB.Location = new System.Drawing.Point(512, 43);
			this.panelB.Name = "panelB";
			this.panelB.Size = new System.Drawing.Size(282, 512);
			this.panelB.TabIndex = 10;
			this.panelB.Paint += new System.Windows.Forms.PaintEventHandler(this.panelB_Paint);
			// 
			// tabGI
			// 
			this.tabGI.Alignment = System.Windows.Forms.TabAlignment.Right;
			this.tabGI.Controls.Add(this.tpAngle);
			this.tabGI.Controls.Add(this.tpPalette);
			this.tabGI.Location = new System.Drawing.Point(24, 200);
			this.tabGI.Multiline = true;
			this.tabGI.Name = "tabGI";
			this.tabGI.SelectedIndex = 0;
			this.tabGI.Size = new System.Drawing.Size(126, 108);
			this.tabGI.TabIndex = 9;
			// 
			// tpAngle
			// 
			this.tpAngle.Controls.Add(this.pictAngle);
			this.tpAngle.Location = new System.Drawing.Point(4, 4);
			this.tpAngle.Name = "tpAngle";
			this.tpAngle.Size = new System.Drawing.Size(100, 100);
			this.tpAngle.TabIndex = 0;
			this.tpAngle.Text = "角度";
			// 
			// pictAngle
			// 
			this.pictAngle.BackColor = System.Drawing.Color.Navy;
			this.pictAngle.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictAngle.ContextMenu = this.ctmAngles;
			this.pictAngle.Location = new System.Drawing.Point(0, 0);
			this.pictAngle.Name = "pictAngle";
			this.pictAngle.Size = new System.Drawing.Size(100, 100);
			this.pictAngle.TabIndex = 16;
			this.pictAngle.TabStop = false;
			this.pictAngle.Click += new System.EventHandler(this.pictAngle_Click);
			this.pictAngle.Paint += new System.Windows.Forms.PaintEventHandler(this.pictAngle_Paint);
			this.pictAngle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictAngle_MouseUp);
			this.pictAngle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictAngle_MouseMove);
			this.pictAngle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictAngle_MouseDown);
			// 
			// ctmAngles
			// 
			this.ctmAngles.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.miAngleGrid0,
																					  this.miAngleGrid1,
																					  this.miAngleGrid5,
																					  this.miAngleGrid15,
																					  this.miAngleGrid30,
																					  this.miAngleGrid90});
			this.ctmAngles.Popup += new System.EventHandler(this.ctmAngles_Popup);
			// 
			// miAngleGrid0
			// 
			this.miAngleGrid0.Index = 0;
			this.miAngleGrid0.Text = "グリッド無し(&0)";
			this.miAngleGrid0.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid1
			// 
			this.miAngleGrid1.Index = 1;
			this.miAngleGrid1.Text = "1°刻み(&1)";
			this.miAngleGrid1.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid5
			// 
			this.miAngleGrid5.Index = 2;
			this.miAngleGrid5.Text = "5°刻み(&2)";
			this.miAngleGrid5.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid15
			// 
			this.miAngleGrid15.Index = 3;
			this.miAngleGrid15.Text = "15°刻み(&3)";
			this.miAngleGrid15.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid30
			// 
			this.miAngleGrid30.Index = 4;
			this.miAngleGrid30.Text = "30°刻み(&4)";
			this.miAngleGrid30.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// miAngleGrid90
			// 
			this.miAngleGrid90.Index = 5;
			this.miAngleGrid90.Text = "90°刻み(&5)";
			this.miAngleGrid90.Click += new System.EventHandler(this.miAngleGrid_Click);
			// 
			// tpPalette
			// 
			this.tpPalette.Controls.Add(this.pictPalette);
			this.tpPalette.Controls.Add(this.labelPaletteError);
			this.tpPalette.Location = new System.Drawing.Point(4, 4);
			this.tpPalette.Name = "tpPalette";
			this.tpPalette.Size = new System.Drawing.Size(100, 100);
			this.tpPalette.TabIndex = 1;
			this.tpPalette.Text = "色";
			// 
			// pictPalette
			// 
			this.pictPalette.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictPalette.ContextMenu = this.ctmPalette;
			this.pictPalette.Location = new System.Drawing.Point(0, 0);
			this.pictPalette.Name = "pictPalette";
			this.pictPalette.Size = new System.Drawing.Size(100, 100);
			this.pictPalette.TabIndex = 0;
			this.pictPalette.TabStop = false;
			this.pictPalette.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictPalette_MouseUp);
			this.pictPalette.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictPalette_MouseMove);
			this.pictPalette.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictPalette_MouseDown);
			// 
			// ctmPalette
			// 
			this.ctmPalette.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.miPaletteChildPaint,
																					   this.miPaletteAllPaint});
			// 
			// miPaletteChildPaint
			// 
			this.miPaletteChildPaint.Index = 0;
			this.miPaletteChildPaint.Text = "派生チップをこの色にする";
			this.miPaletteChildPaint.Click += new System.EventHandler(this.miPaletteChildPaint_Click);
			// 
			// miPaletteAllPaint
			// 
			this.miPaletteAllPaint.Index = 1;
			this.miPaletteAllPaint.Text = "全てのチップをこの色にする";
			this.miPaletteAllPaint.Click += new System.EventHandler(this.miPaletteAllPaint_Click);
			// 
			// labelPaletteError
			// 
			this.labelPaletteError.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelPaletteError.Location = new System.Drawing.Point(0, 0);
			this.labelPaletteError.Name = "labelPaletteError";
			this.labelPaletteError.Size = new System.Drawing.Size(100, 100);
			this.labelPaletteError.TabIndex = 1;
			this.labelPaletteError.Text = "Palette.bmpが\n見つからない為\nパレットを使用\nできません。";
			// 
			// btnRootChip
			// 
			this.btnRootChip.Location = new System.Drawing.Point(8, 32);
			this.btnRootChip.Name = "btnRootChip";
			this.btnRootChip.Size = new System.Drawing.Size(80, 32);
			this.btnRootChip.TabIndex = 1;
			this.btnRootChip.Text = "親チップ";
			this.btnRootChip.Click += new System.EventHandler(this.btnRootChip_Click);
			// 
			// btnKey
			// 
			this.btnKey.Location = new System.Drawing.Point(136, 168);
			this.btnKey.Name = "btnKey";
			this.btnKey.Size = new System.Drawing.Size(80, 24);
			this.btnKey.TabIndex = 8;
			this.btnKey.Text = "Key{...} 編集";
			this.btnKey.Click += new System.EventHandler(this.btnKey_Click);
			// 
			// btnVal
			// 
			this.btnVal.Location = new System.Drawing.Point(56, 168);
			this.btnVal.Name = "btnVal";
			this.btnVal.Size = new System.Drawing.Size(80, 24);
			this.btnVal.TabIndex = 7;
			this.btnVal.Text = "Val{...} 編集";
			this.btnVal.Click += new System.EventHandler(this.btnVal_Click);
			// 
			// panelAttr
			// 
			this.panelAttr.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panelAttr.Controls.Add(this.panelAttrValue);
			this.panelAttr.Controls.Add(this.splAttr);
			this.panelAttr.Controls.Add(this.panelAttrName);
			this.panelAttr.Location = new System.Drawing.Point(0, 312);
			this.panelAttr.Name = "panelAttr";
			this.panelAttr.Size = new System.Drawing.Size(280, 200);
			this.panelAttr.TabIndex = 10;
			// 
			// panelAttrValue
			// 
			this.panelAttrValue.Controls.Add(this.cmbAttrItem9);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem8);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem7);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem6);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem5);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem4);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem3);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem2);
			this.panelAttrValue.Controls.Add(this.cmbAttrItem1);
			this.panelAttrValue.Controls.Add(this.labelAttrValue);
			this.panelAttrValue.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelAttrValue.Location = new System.Drawing.Point(108, 0);
			this.panelAttrValue.Name = "panelAttrValue";
			this.panelAttrValue.Size = new System.Drawing.Size(168, 196);
			this.panelAttrValue.TabIndex = 2;
			// 
			// cmbAttrItem9
			// 
			this.cmbAttrItem9.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem9.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem9.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem9.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem9.Location = new System.Drawing.Point(0, 176);
			this.cmbAttrItem9.Name = "cmbAttrItem9";
			this.cmbAttrItem9.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem9.TabIndex = 20;
			this.cmbAttrItem9.Text = "comboBox1";
			this.cmbAttrItem9.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem9.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem9.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem8
			// 
			this.cmbAttrItem8.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem8.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem8.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem8.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem8.Location = new System.Drawing.Point(0, 156);
			this.cmbAttrItem8.Name = "cmbAttrItem8";
			this.cmbAttrItem8.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem8.TabIndex = 19;
			this.cmbAttrItem8.Text = "comboBox1";
			this.cmbAttrItem8.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem8.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem8.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem7
			// 
			this.cmbAttrItem7.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem7.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem7.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem7.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem7.Location = new System.Drawing.Point(0, 136);
			this.cmbAttrItem7.Name = "cmbAttrItem7";
			this.cmbAttrItem7.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem7.TabIndex = 18;
			this.cmbAttrItem7.Text = "comboBox1";
			this.cmbAttrItem7.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem7.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem7.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem6
			// 
			this.cmbAttrItem6.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem6.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem6.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem6.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem6.Location = new System.Drawing.Point(0, 116);
			this.cmbAttrItem6.Name = "cmbAttrItem6";
			this.cmbAttrItem6.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem6.TabIndex = 17;
			this.cmbAttrItem6.Text = "cmbAttrItem6";
			this.cmbAttrItem6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem6.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem6.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem5
			// 
			this.cmbAttrItem5.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem5.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem5.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem5.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem5.Location = new System.Drawing.Point(0, 96);
			this.cmbAttrItem5.Name = "cmbAttrItem5";
			this.cmbAttrItem5.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem5.TabIndex = 16;
			this.cmbAttrItem5.Text = "cmbAttrItem5";
			this.cmbAttrItem5.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem5.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem5.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem4
			// 
			this.cmbAttrItem4.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem4.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem4.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem4.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem4.Location = new System.Drawing.Point(0, 76);
			this.cmbAttrItem4.Name = "cmbAttrItem4";
			this.cmbAttrItem4.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem4.TabIndex = 15;
			this.cmbAttrItem4.Text = "cmbAttrItem4";
			this.cmbAttrItem4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem4.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem4.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem3
			// 
			this.cmbAttrItem3.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem3.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem3.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem3.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem3.Location = new System.Drawing.Point(0, 56);
			this.cmbAttrItem3.Name = "cmbAttrItem3";
			this.cmbAttrItem3.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem3.TabIndex = 14;
			this.cmbAttrItem3.Text = "cmbAttrItem3";
			this.cmbAttrItem3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem3.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem3.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem2
			// 
			this.cmbAttrItem2.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem2.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem2.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem2.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem2.Location = new System.Drawing.Point(0, 36);
			this.cmbAttrItem2.Name = "cmbAttrItem2";
			this.cmbAttrItem2.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem2.TabIndex = 13;
			this.cmbAttrItem2.Text = "cmbAttrItem2";
			this.cmbAttrItem2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem2.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem2.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// cmbAttrItem1
			// 
			this.cmbAttrItem1.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbAttrItem1.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem1.Items.AddRange(new object[] {
															  "(使用可能な変数はありません)"});
			this.cmbAttrItem1.Location = new System.Drawing.Point(0, 16);
			this.cmbAttrItem1.Name = "cmbAttrItem1";
			this.cmbAttrItem1.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem1.TabIndex = 12;
			this.cmbAttrItem1.Text = "cmbAttrItem1";
			this.cmbAttrItem1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbAttrItems_KeyPress);
			this.cmbAttrItem1.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbAttrItem1.Leave += new System.EventHandler(this.cmbAttrItems_Leave);
			// 
			// labelAttrValue
			// 
			this.labelAttrValue.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.labelAttrValue.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrValue.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.labelAttrValue.Location = new System.Drawing.Point(0, 0);
			this.labelAttrValue.Name = "labelAttrValue";
			this.labelAttrValue.Size = new System.Drawing.Size(168, 16);
			this.labelAttrValue.TabIndex = 1;
			this.labelAttrValue.Text = "値";
			// 
			// splAttr
			// 
			this.splAttr.Location = new System.Drawing.Point(104, 0);
			this.splAttr.Name = "splAttr";
			this.splAttr.Size = new System.Drawing.Size(4, 196);
			this.splAttr.TabIndex = 1;
			this.splAttr.TabStop = false;
			// 
			// panelAttrName
			// 
			this.panelAttrName.Controls.Add(this.labelAttrItem9);
			this.panelAttrName.Controls.Add(this.labelAttrItem8);
			this.panelAttrName.Controls.Add(this.labelAttrItem7);
			this.panelAttrName.Controls.Add(this.labelAttrItem6);
			this.panelAttrName.Controls.Add(this.labelAttrItem5);
			this.panelAttrName.Controls.Add(this.labelAttrItem4);
			this.panelAttrName.Controls.Add(this.labelAttrItem3);
			this.panelAttrName.Controls.Add(this.labelAttrItem2);
			this.panelAttrName.Controls.Add(this.labelAttrItem1);
			this.panelAttrName.Controls.Add(this.labelAttrName);
			this.panelAttrName.Dock = System.Windows.Forms.DockStyle.Left;
			this.panelAttrName.Location = new System.Drawing.Point(0, 0);
			this.panelAttrName.Name = "panelAttrName";
			this.panelAttrName.Size = new System.Drawing.Size(104, 196);
			this.panelAttrName.TabIndex = 0;
			// 
			// labelAttrItem9
			// 
			this.labelAttrItem9.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem9.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem9.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem9.Location = new System.Drawing.Point(0, 176);
			this.labelAttrItem9.Name = "labelAttrItem9";
			this.labelAttrItem9.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem9.TabIndex = 9;
			this.labelAttrItem9.Text = "label2";
			this.labelAttrItem9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem8
			// 
			this.labelAttrItem8.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem8.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem8.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem8.Location = new System.Drawing.Point(0, 156);
			this.labelAttrItem8.Name = "labelAttrItem8";
			this.labelAttrItem8.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem8.TabIndex = 8;
			this.labelAttrItem8.Text = "labelAttrItem8";
			this.labelAttrItem8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem7
			// 
			this.labelAttrItem7.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem7.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem7.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem7.Location = new System.Drawing.Point(0, 136);
			this.labelAttrItem7.Name = "labelAttrItem7";
			this.labelAttrItem7.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem7.TabIndex = 7;
			this.labelAttrItem7.Text = "labelAttrItem7";
			this.labelAttrItem7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem6
			// 
			this.labelAttrItem6.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem6.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem6.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem6.Location = new System.Drawing.Point(0, 116);
			this.labelAttrItem6.Name = "labelAttrItem6";
			this.labelAttrItem6.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem6.TabIndex = 6;
			this.labelAttrItem6.Text = "labelAttrItem6";
			this.labelAttrItem6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem5
			// 
			this.labelAttrItem5.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem5.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem5.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem5.Location = new System.Drawing.Point(0, 96);
			this.labelAttrItem5.Name = "labelAttrItem5";
			this.labelAttrItem5.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem5.TabIndex = 5;
			this.labelAttrItem5.Text = "labelAttrItem5";
			this.labelAttrItem5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem4
			// 
			this.labelAttrItem4.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem4.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem4.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem4.Location = new System.Drawing.Point(0, 76);
			this.labelAttrItem4.Name = "labelAttrItem4";
			this.labelAttrItem4.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem4.TabIndex = 4;
			this.labelAttrItem4.Text = "labelAttrItem4";
			this.labelAttrItem4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem3
			// 
			this.labelAttrItem3.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem3.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem3.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem3.Location = new System.Drawing.Point(0, 56);
			this.labelAttrItem3.Name = "labelAttrItem3";
			this.labelAttrItem3.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem3.TabIndex = 3;
			this.labelAttrItem3.Text = "labelAttrItem3";
			this.labelAttrItem3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem2
			// 
			this.labelAttrItem2.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem2.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem2.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem2.Location = new System.Drawing.Point(0, 36);
			this.labelAttrItem2.Name = "labelAttrItem2";
			this.labelAttrItem2.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem2.TabIndex = 2;
			this.labelAttrItem2.Text = "labelAttrItem2";
			this.labelAttrItem2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrItem1
			// 
			this.labelAttrItem1.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem1.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrItem1.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.labelAttrItem1.Location = new System.Drawing.Point(0, 16);
			this.labelAttrItem1.Name = "labelAttrItem1";
			this.labelAttrItem1.Size = new System.Drawing.Size(104, 20);
			this.labelAttrItem1.TabIndex = 1;
			this.labelAttrItem1.Text = "labelAttrItem1";
			this.labelAttrItem1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAttrName
			// 
			this.labelAttrName.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.labelAttrName.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelAttrName.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.labelAttrName.Location = new System.Drawing.Point(0, 0);
			this.labelAttrName.Name = "labelAttrName";
			this.labelAttrName.Size = new System.Drawing.Size(104, 16);
			this.labelAttrName.TabIndex = 0;
			this.labelAttrName.Text = "属性名";
			// 
			// txtName
			// 
			this.txtName.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.txtName.Location = new System.Drawing.Point(176, 216);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(80, 19);
			this.txtName.TabIndex = 10;
			this.txtName.Text = "";
			this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtName_KeyPress);
			this.txtName.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.txtName.Leave += new System.EventHandler(this.txtAttrs_Leave);
			this.txtName.Enter += new System.EventHandler(this.txtAttrs_Enter);
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(176, 200);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(48, 16);
			this.labelName.TabIndex = 8;
			this.labelName.Text = "識別名";
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(176, 240);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(24, 16);
			this.labelColor.TabIndex = 6;
			this.labelColor.Text = "色";
			// 
			// cmbColor
			// 
			this.cmbColor.ContextMenu = this.ctmPalette;
			this.cmbColor.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.cmbColor.ItemHeight = 12;
			this.cmbColor.Location = new System.Drawing.Point(176, 256);
			this.cmbColor.Name = "cmbColor";
			this.cmbColor.Size = new System.Drawing.Size(80, 20);
			this.cmbColor.TabIndex = 11;
			this.cmbColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbColor_KeyPress);
			this.cmbColor.TextChanged += new System.EventHandler(this.ChipInfo_TextChanged);
			this.cmbColor.Leave += new System.EventHandler(this.txtAttrs_Leave);
			this.cmbColor.Enter += new System.EventHandler(this.txtAttrs_Enter);
			// 
			// lstSouth
			// 
			this.lstSouth.ContextMenu = this.ctmChildList;
			this.lstSouth.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.lstSouth.IntegralHeight = false;
			this.lstSouth.ItemHeight = 12;
			this.lstSouth.Location = new System.Drawing.Point(88, 120);
			this.lstSouth.Name = "lstSouth";
			this.lstSouth.Size = new System.Drawing.Size(104, 56);
			this.lstSouth.TabIndex = 4;
			this.lstSouth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstSouth.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstSouth.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// ctmChildList
			// 
			this.ctmChildList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.miListAdd,
																						 this.miListSelect,
																						 this.menuItem10,
																						 this.miListCut,
																						 this.miListCopy});
			this.ctmChildList.Popup += new System.EventHandler(this.ctmChildList_Popup);
			// 
			// miListAdd
			// 
			this.miListAdd.Index = 0;
			this.miListAdd.Text = "ここに追加(&A)";
			this.miListAdd.Click += new System.EventHandler(this.miListAdd_Click);
			// 
			// miListSelect
			// 
			this.miListSelect.Index = 1;
			this.miListSelect.Text = "選択(&S)";
			this.miListSelect.Click += new System.EventHandler(this.miListSelect_Click);
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 2;
			this.menuItem10.Text = "-";
			// 
			// miListCut
			// 
			this.miListCut.Index = 3;
			this.miListCut.Text = "切り取り(&T)";
			this.miListCut.Click += new System.EventHandler(this.miListCut_Click);
			// 
			// miListCopy
			// 
			this.miListCopy.Index = 4;
			this.miListCopy.Text = "コピー(&C)";
			this.miListCopy.Click += new System.EventHandler(this.miListCopy_Click);
			// 
			// lstNorth
			// 
			this.lstNorth.ContextMenu = this.ctmChildList;
			this.lstNorth.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.lstNorth.IntegralHeight = false;
			this.lstNorth.ItemHeight = 12;
			this.lstNorth.Location = new System.Drawing.Point(88, 8);
			this.lstNorth.Name = "lstNorth";
			this.lstNorth.Size = new System.Drawing.Size(104, 56);
			this.lstNorth.TabIndex = 2;
			this.lstNorth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstNorth.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstNorth.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// lstEast
			// 
			this.lstEast.ContextMenu = this.ctmChildList;
			this.lstEast.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.lstEast.IntegralHeight = false;
			this.lstEast.ItemHeight = 12;
			this.lstEast.Location = new System.Drawing.Point(168, 64);
			this.lstEast.Name = "lstEast";
			this.lstEast.Size = new System.Drawing.Size(104, 56);
			this.lstEast.TabIndex = 3;
			this.lstEast.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstEast.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstEast.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// lstWest
			// 
			this.lstWest.ContextMenu = this.ctmChildList;
			this.lstWest.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.lstWest.IntegralHeight = false;
			this.lstWest.ItemHeight = 12;
			this.lstWest.Location = new System.Drawing.Point(8, 64);
			this.lstWest.Name = "lstWest";
			this.lstWest.Size = new System.Drawing.Size(104, 56);
			this.lstWest.TabIndex = 5;
			this.lstWest.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.lstChild_KeyPress);
			this.lstWest.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstWest.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// buttonSelChip
			// 
			this.buttonSelChip.ImageIndex = 0;
			this.buttonSelChip.ImageList = this.imgIconsL;
			this.buttonSelChip.Location = new System.Drawing.Point(112, 64);
			this.buttonSelChip.Name = "buttonSelChip";
			this.buttonSelChip.Size = new System.Drawing.Size(56, 56);
			this.buttonSelChip.TabIndex = 6;
			this.ttMain.SetToolTip(this.buttonSelChip, "コメントなし");
			this.buttonSelChip.Click += new System.EventHandler(this.buttonSelChip_Click);
			// 
			// imgIconsL
			// 
			this.imgIconsL.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imgIconsL.ImageSize = new System.Drawing.Size(48, 48);
			this.imgIconsL.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIconsL.ImageStream")));
			this.imgIconsL.TransparentColor = System.Drawing.Color.White;
			// 
			// tmr
			// 
			this.tmr.Enabled = true;
			this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
			// 
			// tmrScroll
			// 
			this.tmrScroll.Interval = 1;
			this.tmrScroll.Tick += new System.EventHandler(this.tmrScroll_Tick);
			// 
			// ctmChipType
			// 
			this.ctmChipType.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.miCut,
																						this.miCopy,
																						this.miDelete,
																						this.menuItem8,
																						this.miChange,
																						this.miChipComment,
																						this.menuItem7,
																						this.miRotateRight,
																						this.miRotateLeft,
																						this.menuItem5,
																						this.miReverseX,
																						this.miReverseY,
																						this.miReverseZ});
			this.ctmChipType.Popup += new System.EventHandler(this.ctmChipType_Popup);
			// 
			// miCut
			// 
			this.miCut.Index = 0;
			this.miCut.Text = "切り取り(&T)";
			this.miCut.Click += new System.EventHandler(this.miCut_Click);
			// 
			// miCopy
			// 
			this.miCopy.Index = 1;
			this.miCopy.Text = "コピー(&C)";
			this.miCopy.Click += new System.EventHandler(this.miCopy_Click);
			// 
			// miDelete
			// 
			this.miDelete.Index = 2;
			this.miDelete.Text = "削除(&D)";
			this.miDelete.Click += new System.EventHandler(this.miDelete_Click);
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 3;
			this.menuItem8.Text = "-";
			// 
			// miChange
			// 
			this.miChange.Index = 4;
			this.miChange.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.miChangeChip,
																					 this.miChangeFrame,
																					 this.miChangeRudder,
																					 this.miChangeRudderF,
																					 this.miChangeTrim,
																					 this.miChangeTrimF,
																					 this.ctmSeparator,
																					 this.miChangeWheel,
																					 this.miChangeRLW,
																					 this.miChangeJet,
																					 this.miChangeWeight,
																					 this.miChangeCowl,
																					 this.miChangeArm});
			this.miChange.Text = "チップ種類変更(&H)";
			// 
			// miChangeChip
			// 
			this.miChangeChip.Index = 0;
			this.miChangeChip.Text = "チップ(&Chip)";
			this.miChangeChip.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeFrame
			// 
			this.miChangeFrame.Index = 1;
			this.miChangeFrame.Text = "フレーム(&Frame)";
			this.miChangeFrame.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeRudder
			// 
			this.miChangeRudder.Index = 2;
			this.miChangeRudder.Text = "ラダー(&Rudder)";
			this.miChangeRudder.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeRudderF
			// 
			this.miChangeRudderF.Index = 3;
			this.miChangeRudderF.Text = "ラダーフレーム(Ru&dderF)";
			this.miChangeRudderF.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeTrim
			// 
			this.miChangeTrim.Index = 4;
			this.miChangeTrim.Text = "トリム(&Trim)";
			this.miChangeTrim.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeTrimF
			// 
			this.miChangeTrimF.Index = 5;
			this.miChangeTrimF.Text = "トリムフレーム(Tri&mF)";
			this.miChangeTrimF.Click += new System.EventHandler(this.miChangeType);
			// 
			// ctmSeparator
			// 
			this.ctmSeparator.Index = 6;
			this.ctmSeparator.Text = "-";
			// 
			// miChangeWheel
			// 
			this.miChangeWheel.Index = 7;
			this.miChangeWheel.Text = "ホイール(W&heel)";
			this.miChangeWheel.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeRLW
			// 
			this.miChangeRLW.Index = 8;
			this.miChangeRLW.Text = "無反動ホイール(R&LW)";
			this.miChangeRLW.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeJet
			// 
			this.miChangeJet.Index = 9;
			this.miChangeJet.Text = "ジェット(&Jet)";
			this.miChangeJet.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeWeight
			// 
			this.miChangeWeight.Index = 10;
			this.miChangeWeight.Text = "ウェイト(&Weight)";
			this.miChangeWeight.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeCowl
			// 
			this.miChangeCowl.Index = 11;
			this.miChangeCowl.Text = "カウル(C&owl)";
			this.miChangeCowl.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChangeArm
			// 
			this.miChangeArm.Index = 12;
			this.miChangeArm.Text = "アーム(&Arm)";
			this.miChangeArm.Click += new System.EventHandler(this.miChangeType);
			// 
			// miChipComment
			// 
			this.miChipComment.Index = 5;
			this.miChipComment.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.miCommentEdit,
																						  this.miCommentDelete});
			this.miChipComment.Text = "コメント(&O)";
			this.miChipComment.Popup += new System.EventHandler(this.miChipComment_Popup);
			this.miChipComment.Select += new System.EventHandler(this.miChipComment_Popup);
			// 
			// miCommentEdit
			// 
			this.miCommentEdit.Index = 0;
			this.miCommentEdit.Text = "コメントを編集";
			this.miCommentEdit.Click += new System.EventHandler(this.miCommentEdit_Click);
			// 
			// miCommentDelete
			// 
			this.miCommentDelete.Index = 1;
			this.miCommentDelete.Text = "コメントを削除";
			this.miCommentDelete.Click += new System.EventHandler(this.miCommentDelete_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 6;
			this.menuItem7.Text = "-";
			// 
			// miRotateRight
			// 
			this.miRotateRight.Index = 7;
			this.miRotateRight.Text = "このチップ周りを右回転(&R)";
			this.miRotateRight.Click += new System.EventHandler(this.miRotateRight_Click);
			// 
			// miRotateLeft
			// 
			this.miRotateLeft.Index = 8;
			this.miRotateLeft.Text = "このチップ周りを左回転(&L)";
			this.miRotateLeft.Click += new System.EventHandler(this.miRotateLeft_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 9;
			this.menuItem5.Text = "-";
			// 
			// miReverseX
			// 
			this.miReverseX.Index = 10;
			this.miReverseX.Text = "左右反転(&X)";
			this.miReverseX.Click += new System.EventHandler(this.miReverseX_Click);
			// 
			// miReverseY
			// 
			this.miReverseY.Index = 11;
			this.miReverseY.Text = "上下反転(&Y)";
			this.miReverseY.Click += new System.EventHandler(this.miReverseY_Click);
			// 
			// miReverseZ
			// 
			this.miReverseZ.Index = 12;
			this.miReverseZ.Text = "前後反転(&Z)";
			this.miReverseZ.Click += new System.EventHandler(this.miReverseZ_Click);
			// 
			// pictTarget
			// 
			this.pictTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictTarget.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictTarget.Location = new System.Drawing.Point(0, 43);
			this.pictTarget.Name = "pictTarget";
			this.pictTarget.Size = new System.Drawing.Size(512, 512);
			this.pictTarget.TabIndex = 11;
			this.pictTarget.TabStop = false;
			this.pictTarget.Click += new System.EventHandler(this.pictTarget_Click);
			this.pictTarget.Paint += new System.Windows.Forms.PaintEventHandler(this.pictTarget_Paint);
			this.pictTarget.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseUp);
			this.pictTarget.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseMove);
			this.pictTarget.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseDown);
			// 
			// menuItem16
			// 
			this.menuItem16.Index = 2;
			this.menuItem16.Shortcut = System.Windows.Forms.Shortcut.CtrlDel;
			this.menuItem16.Text = "削除(&D)";
			// 
			// menuItem17
			// 
			this.menuItem17.Index = 5;
			this.menuItem17.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItem18});
			this.menuItem17.Text = "移動(&M)...";
			// 
			// menuItem18
			// 
			this.menuItem18.Index = 0;
			this.menuItem18.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.menuItem18.Text = "親チップ(&P)";
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(794, 555);
			this.Controls.Add(this.pictTarget);
			this.Controls.Add(this.panelB);
			this.Controls.Add(this.labelTip);
			this.Controls.Add(this.tbMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MaximizeBox = false;
			this.Menu = this.mnuMain;
			this.MinimizeBox = false;
			this.Name = "frmMain";
			this.Text = "Rigid Chips Modeler";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.Click += new System.EventHandler(this.frmMain_Click);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frmMain_KeyPress);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.panelB.ResumeLayout(false);
			this.tabGI.ResumeLayout(false);
			this.tpAngle.ResumeLayout(false);
			this.tpPalette.ResumeLayout(false);
			this.panelAttr.ResumeLayout(false);
			this.panelAttrValue.ResumeLayout(false);
			this.panelAttrName.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			try{
				throw new Exception();
			}
			catch{}
			/*			try{
							Application.Run(new frmMain());
						}catch(Exception e){
							MessageBox.Show(e.Message);
						}
			*/
			frmMain mainform = new frmMain(args);
			try{
				Application.Run(mainform);
			}
			catch{
				//	TODO:	ここにレジュームファイルを生成するコードを書く。
				mainform.rcdata.Save(Application.StartupPath + "\\_resume.$cm");
//				MessageBox.Show("以下の予期せぬエラーが発生しました。\nご迷惑をおかけします。\n\n" + e.Message + "\n\nレジュームファイルが作成されました。","RigidChipsModeler",MessageBoxButtons.OK,MessageBoxIcon.Error);
				throw;
			}
		}

		public bool Modified{
			get{
				return modified;
			}
			set{
				if(modified == value)return;
				modified = value;
				miFileSave.Enabled = modified;
				SetWindowTitle();
			}
		}

		public string EditingFileName{
			get{
				return editingFileName;
			}
			set{
				if(editingFileName == value)return;
				editingFileName = value;
				SetWindowTitle();
			}
		}

		private void SetWindowTitle(){
			this.SuspendLayout();
			this.Text = System.IO.Path.GetFileNameWithoutExtension(editingFileName);
			if(this.Text == "")this.Text = "(Untitled)";
			this.Text += (modified ? "*" : "") + " - RigidChips Modeler";
			this.ResumeLayout();
		}

		private void frmMain_Load(object sender, System.EventArgs e) {
			draging.Draging = false;
			selected = tbbCursor;
			InitializeGraphics();

			string resourcepath;

			StreamReader infile;
			StreamWriter outfile;
			try{
				infile = new StreamReader(Application.StartupPath + "\\ResourcePath.txt");
				resourcepath = infile.ReadLine();
				infile.Close();
				if(!File.Exists(resourcepath + "\\Core.x"))
					throw new Exception();
			}catch{
				dlgOpen.Filter = "Core.x in Resource folder|Core.x";
				dlgOpen.AddExtension = false;
				dlgOpen.CheckFileExists = true;
				dlgOpen.CheckPathExists = true;
				dlgOpen.DereferenceLinks = false;
				dlgOpen.Multiselect = false;
				dlgOpen.RestoreDirectory = true;
				dlgOpen.Title = "リソースフォルダのCore.xを指定してください。";

				if(dlgOpen.ShowDialog() == DialogResult.Cancel)
					Application.Exit();

				resourcepath = Path.GetDirectoryName(dlgOpen.FileName);

				outfile = new StreamWriter(Application.StartupPath + "\\ResourcePath.txt");
				outfile.Write(resourcepath);
				outfile.Close();

			}

			try{
				rcdata = new RcData(device,drawOption,outputOption,editOption,resourcepath);
				weightBall = new RcXFile();
				weightBall.Load(device,Application.StartupPath + @"\\Resources\\weight.x");
				this.s
			}
			catch(FileNotFoundException){
				MessageBox.Show("リソースの読み込みに失敗しました。ヘルプを読んで再度インストールを行ってください。","FileNotFountException",MessageBoxButtons.OK,MessageBoxIcon.Error);
				this.Dispose(true);
				Application.Exit();
			}

			rcdata.OnSelectedChipChanged += new MessageCallback(RcData_SelectedChipChanged);
			rcdata.SelectedChip = rcdata.model.root;


			Initialized = true;
			try{
				pictPalette.Image = Image.FromFile("Palette.bmp");
			}
			catch{
				pictPalette.Visible = false;
			}
			//			cmbColor.Items.Add();
			UpdateCameraPosition(0,0,0,Matrix.Identity);
			configwindow = new frmConfig(this);

			bool buffer = editOption.ConvertParentAttributes;
			editOption.ConvertParentAttributes = false;
			if(Arguments.Length > 0){
				if(System.IO.Path.GetExtension(Arguments[0]).ToLower() == ".rcm"){
					rcdata.Load(Arguments[0]);
					EditingFileName = Arguments[0];
				}
				else{
					rcdata.headercomment = rcdata.script = "";
					rcdata.SelectedChip = rcdata.model.root;
					rcdata.RegisterChip(rcdata.model.root);
					infile = new StreamReader(Arguments[0],System.Text.Encoding.Default);

					rcdata.Parse(infile.ReadToEnd());
				}
				rcdata.SelectedChip = rcdata.model.root;
				SetValDropList();
			}
			

			else if(System.IO.File.Exists(Application.StartupPath + "\\_resume.$cm")){
				if(MessageBox.Show("レジュームファイルが存在します。読み込みますか？","RigidChipsModeler",MessageBoxButtons.YesNo) == DialogResult.Yes){
					rcdata.Load(Application.StartupPath + "\\_resume.$cm");
					EditingFileName = Application.StartupPath + "\\_resume.$cm";
				}
				System.IO.File.Delete(Application.StartupPath + "\\_resume.$cm");
			}

			editOption.ConvertParentAttributes = buffer;
			SetListBackColor();
			SetWindowTitle();
		}
		
		public void SetListBackColor(){
			this.lstNorth.BackColor = Color.FromArgb(drawOption.NGuideColor.R / 2 + 128, drawOption.NGuideColor.G/2 + 128,drawOption.NGuideColor.B/2 + 128);
			this.lstSouth.BackColor = Color.FromArgb(drawOption.SGuideColor.R / 2 + 128, drawOption.SGuideColor.G/2 + 128,drawOption.SGuideColor.B/2 + 128);
			this.lstEast.BackColor = Color.FromArgb(drawOption.EGuideColor.R / 2 + 128, drawOption.EGuideColor.G/2 + 128,drawOption.EGuideColor.B/2 + 128);
			this.lstWest.BackColor = Color.FromArgb(drawOption.WGuideColor.R / 2 + 128, drawOption.WGuideColor.G/2 + 128,drawOption.WGuideColor.B/2 + 128);

		}
		private void frmMain_Click(object sender, System.EventArgs e) {
			pictTarget_Paint(this,null);
		}

		private void pictTarget_Paint(object sender, PaintEventArgs e) {
			if(!Initialized || Pause || device == null){
				labelTip.Text = "painting is stopped.";
				return;
			}
			Material m = new Material();

			if(CameraOrtho != drawOption.CameraOrtho){
				CameraOrtho = drawOption.CameraOrtho;
				if(CameraOrtho)
					device.Transform.Projection = Matrix.OrthoLH(CamDepth/2f,CamDepth/2f,-256f,256f);
				else
					device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f ,(float)pictTarget.Width / (float)pictTarget.Height,0.5f,100.0f);
			}

			device.Clear(ClearFlags.Target | ClearFlags.ZBuffer ,drawOption.BackColor ,100.0f,0);
			device.BeginScene();

			rcdata.model.root.DrawChipAll();
			if(drawOption.ShowGuideAlways || !draging.Draging){
				if(rcdata.SelectedChip != null)
					rcdata.DrawCursor();
				else{
					RcChipBase selectedChips = rcdata.SelectedChipList;
					if(selectedChips != null){
					}
				}

				device.RenderState.AlphaBlendEnable = false;
				device.VertexFormat = CustomVertex.PositionOnly.Format;
				device.SetStreamSource(0,vbGuide,0);
				device.Transform.World = Matrix.Identity;

				if(drawOption.XAxisEnable){
					m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.XAxisColor;
					device.Material = m;
					device.DrawPrimitives(PrimitiveType.LineList ,0,1);
				}

				if(drawOption.XNegAxisEnable){
					m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.XNegAxisColor;
					device.Material = m;
					device.DrawPrimitives(PrimitiveType.LineList ,1,1);
				}

				if(drawOption.YAxisEnable){
					m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.YAxisColor;
					device.Material = m;
					device.DrawPrimitives(PrimitiveType.LineList ,3,1);
				}

				if(drawOption.YNegAxisEnable){
					m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.YNegAxisColor;
					device.Material = m;
					device.DrawPrimitives(PrimitiveType.LineList ,4,1);
				}

				if(drawOption.ZAxisEnable){
					m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.ZAxisColor;
					device.Material = m;
					device.DrawPrimitives(PrimitiveType.LineList ,6,1);
				}

				if(drawOption.ZNegAxisEnable){
					m.Ambient = m.Diffuse = m.Emissive = m.Specular = drawOption.ZNegAxisColor;
					device.Material = m;
					device.DrawPrimitives(PrimitiveType.LineList ,7,1);
				}

				device.RenderState.AlphaBlendEnable = true;
				if(drawOption.WeightEnable){
					device.SetStreamSource(0,vbWeight,0);
					device.Transform.World = Matrix.Translation(rcdata.WeightCenter);

					m.Ambient = m.Diffuse = m.Emissive = m.Specular = Color.FromArgb(127,drawOption.WeightColor);
					device.Material = m;
					device.DrawPrimitives(PrimitiveType.LineList,0,3);

					if(drawOption.WeightBallEnable){
					weightBall.Draw(
						rcdata.d3ddevice,
						Color.FromArgb((int)(255*drawOption.WeightBallAlpha),drawOption.WeightBallColor),
						Matrix.Scaling(drawOption.WeightBallSize ,drawOption.WeightBallSize ,drawOption.WeightBallSize ) * device.Transform.World);
					}
				}
				else if(drawOption.WeightBallEnable){
					weightBall.Draw(
						rcdata.d3ddevice,
						Color.FromArgb((int)(255*drawOption.WeightBallAlpha),0,0,0),
						Matrix.Scaling(drawOption.WeightBallSize ,drawOption.WeightBallSize ,drawOption.WeightBallSize ) * Matrix.Translation(rcdata.WeightCenter));
				}


			}

			//			rcx.Draw(device,Color.FromArgb(40,255,0,0) ,Matrix.Identity);

			device.EndScene();
			device.Present();

		}

		private void pictTarget_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			MouseX = e.X;
			MouseY = e.Y;
			if(e.Button == MouseButtons.Right){
				LeastIsLeftButton = false;

				RcHitStatus hit = rcdata.model.root.IsHit(MouseX,MouseY,pictTarget.ClientRectangle.Width,pictTarget.ClientRectangle.Height);
				draging.StartX = draging.PrevX = e.X;
				draging.StartY = draging.PrevY = e.Y;
				draging.Draging = (hit.HitChip == null);
			}
			else if(e.Button == MouseButtons.Left)
				LeastIsLeftButton = true;
			CamDepth += e.Delta * 0.5f;

		}

		private void pictTarget_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button != MouseButtons.Right)return;
			draging.Draging = false;
		}

		private void pictTarget_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			MouseX = e.X;
			MouseY = e.Y;

			if(draging.Draging){
				if((e.Button & MouseButtons.Left) > 0)
					UpdateCameraPosition(0,0,draging.PrevX + draging.PrevY - e.X - e.Y ,CamNow);
				else
					UpdateCameraPosition(e.X - draging.PrevX,draging.PrevY - e.Y,e.Delta,CamNow);
								
				draging.PrevX = e.X;
				draging.PrevY = e.Y;
			}

		}

		private void tmr_Tick(object sender, System.EventArgs e) {
			pictTarget_Paint(sender,null);

		}

		private void miToolSend_Click(object sender, System.EventArgs e) {
			string output = rcdata.vals.ToString() + rcdata.keys.ToString() + rcdata.model.ToString();
			if(rcdata.script != ""){
				output += (rcdata.luascript ? "Lua" : "Script") + "{\n" + rcdata.script + "}\n";
			}
//			string filename = Application.StartupPath + "\\out" + (DateTime.Now.Ticks & 0xFFFF).ToString("X") + ".rcd_";
			string filename = Application.StartupPath + "\\out.rcd_";
			byte[] filenameArray;
			StreamWriter tw = new StreamWriter(filename);
			tw.Write(output);
			tw.Flush();
			tw.Close();

			filenameArray = System.Text.Encoding.Convert(System.Text.Encoding.Unicode,System.Text.Encoding.GetEncoding(932 /* Shift-JIS */),System.Text.Encoding.Unicode.GetBytes(filename));

			WM_RIGHTCHIP_LOAD = RegisterWindowMessageA("WM_RIGHTCHIP_LOAD");

			PostMessageA(hWnd_Broadcast,WM_RIGHTCHIP_LOAD,Msg_RcLoadStart,0);

			for(int i = 0;i < filenameArray.Length;i++){
				PostMessageA(hWnd_Broadcast,WM_RIGHTCHIP_LOAD,Msg_RcLoadChar,filenameArray[i]);
			}

			PostMessageA(hWnd_Broadcast,WM_RIGHTCHIP_LOAD,Msg_RcLoadEnd,0);

			labelTip.Text = "RigidChipsにメッセージを送信しました。";

		}
		private void pictTarget_Click(object sender, System.EventArgs e) {
			RcHitStatus cursors,models;
			cursors = rcdata.Cursor.IsHit(MouseX,MouseY,pictTarget.ClientRectangle.Width,pictTarget.ClientRectangle.Height);
			models = rcdata.model.root.IsHit(MouseX,MouseY,pictTarget.ClientRectangle.Width,pictTarget.ClientRectangle.Height);

			if(!LeastIsLeftButton || draging.Draging){
				if(draging.StartX != draging.PrevX || draging.StartY != draging.PrevY)return;
				if(models.HitChip != null){
					draging.Draging = false;
					rcdata.SelectedChip = models.HitChip;
					ctmChipType.Show(pictTarget,new Point(MouseX,MouseY));
				}
			}
			else{
			
				if(editOption.AttributeAutoApply)
					ApplyChipInfo();

				switch(selected.Text){
					case "選択":
						if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "貼り付け":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								RcChipBase buffer = clipboard.Clone(true,null);
								buffer.Attach(rcdata.SelectedChip,cursors.HitChip.JointPosition);
								rcdata.SelectedChip = buffer;
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = "貼り付けを行いました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}
						rcdata.CheckBackTrack();
						break;
					case "視点":
						if(models.HitChip != null){
							StartScrollCameraPosition(models.HitChip.Matrix);
						}
						break;
					case "チップ":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipChip(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "フレーム":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipFrame(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "ウェイト":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipWeight(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}
						break;
					case "カウル":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipCowl(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
									((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
									 " (" + rcdata.SelectedChip.Comment + ")" : "");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "ラダー":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipRudder(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "ラダーフレーム":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipRudderF(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "トリム":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip =  new RcChipTrim(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "トリムフレーム":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipTrimF(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "ホイール":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipWheel(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "無反動ホイール":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipRLW(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "ジェット":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipJet(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					case "アーム":
						if(cursors.distance < models.distance && cursors.HitChip != rcdata.Cursor){
							try{
								rcdata.SelectedChip = new RcChipArm(rcdata,rcdata.SelectedChip,cursors.HitChip.JointPosition);
								Modified = true;
								if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
								labelTip.Text = selected.Text + "を追加しました。";
							}
							catch(Exception err){
								MessageBox.Show(err.Message,"追加エラー");
								return;
							}
						}
						else if(models.HitChip != null){
							if(models.HitChip != rcdata.SelectedChip){
								if(rcdata.SelectedChip != null){
									rcdata.SelectedChip = models.HitChip;
									labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
										((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
										" (" + rcdata.SelectedChip.Comment + ")" : "");
								}
								else{
									rcdata.AssignSelectedChips(models.HitChip);
								}
							}
						}

						break;
					default:
						break;
				}

				//			UpdateCameraPosition(0,0,0,rcdata.Cursor.Matrix);
			}
		}

		private void actionCut(){
			if(rcdata.SelectedChip is RcChipCore){
				labelTip.Text = "コアを切り取ることはできません。";
				return;
			}
			clipboard = rcdata.SelectedChip;
			rcdata.SelectedChip = clipboard.Parent;
			clipboard.Detach();
			Modified = true;

			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
			labelTip.Text = "切り取りを行いました。";
		}

		private void actionCopy(){
			//	コピー動作
			if(rcdata.SelectedChip is RcChipCore){
				labelTip.Text = "コアはコピーできません。";
				return;
			}
			clipboard = rcdata.SelectedChip.Clone(true,null);
			labelTip.Text = "コピーしました。";
		}

		private void actionRemove(){
			if(rcdata.SelectedChip is RcChipCore){
				labelTip.Text = "コアは削除できません。";
				return;
			}
			foreach(RcChipBase cb in rcdata.SelectedChip.Child){
				if(cb != null)
					if(MessageBox.Show("派生チップもろとも削除しますがよろしいですか？","削除確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question)
						== DialogResult.Cancel)
						return;
					else
						break;
			}
			RcChipBase buff = rcdata.SelectedChip.Parent;
			try{rcdata.SelectedChip.Detach();}catch{};
			rcdata.SelectedChip = buff;
			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
			labelTip.Text = "削除しました。";
		}

		private void tbMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			if(e.Button == tbbCut){
				actionCut();
			}
			else if(e.Button == tbbCopy){
				actionCopy();
			}
			else if(e.Button == tbbRemove){
				actionRemove();
			}
			else if(e.Button == tbbInsert){
				if(rcdata.SelectedChip is RcChipCore){
					labelTip.Text = "挿入する場所がありません。";
					return;
				}
				RcChipBase prevParent = rcdata.SelectedChip.Parent;
				RcJointPosition prevJP = rcdata.SelectedChip.JointPosition;

				RcChipBase buffer;

				switch(selected.Text){
					case "貼り付け":
						labelTip.Text = "貼り付けモードの時は挿入はできません。";
						return;
					case "視点":
						labelTip.Text = "視点モードの時は挿入はできません。";
						return;
					case "チップ":
						try{
							buffer = new RcChipChip(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "フレーム":
						try{
							buffer = new RcChipFrame(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ウェイト":
						try{
							buffer = new RcChipWeight(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "カウル":
						try{
							buffer = new RcChipCowl(rcdata,prevParent,prevJP);
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "ラダー":
						try{
							buffer = new RcChipRudder(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ラダーフレーム":
						try{
							buffer = new RcChipRudderF(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "トリム":
						try{
							buffer = new RcChipTrim(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "トリムフレーム":
						try{
							buffer = new RcChipTrimF(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ホイール":
						try{
							buffer = new RcChipWheel(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "無反動ホイール":
						try{
							buffer = new RcChipRLW(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					case "ジェット":
						try{
							buffer = new RcChipJet(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					
						break;
					case "アーム":
						try{
							buffer = new RcChipArm(rcdata,prevParent,prevJP);

						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
					

						break;
					default:
						labelTip.Text = "不明のモードです。挿入はできません。";
						return;
				}

				try{
					rcdata.SelectedChip.Detach();
					rcdata.SelectedChip.Attach(buffer,prevJP);
				}catch(Exception err){
					MessageBox.Show(err.Message,"挿入エラー",MessageBoxButtons.OK,MessageBoxIcon.Error);
					try{
						buffer.Detach();
					}catch{}
					try{
						rcdata.SelectedChip.Attach(prevParent,prevJP);
					}catch{}
				}

				
				LoadChipInfo();
				ProcessViewPoint(rcdata.Cursor.Matrix);
				Modified = true;
				if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
				labelTip.Text = selected.Text + "を挿入しました。";
			}
			else if(e.Button == tbbZoom){
				CamDepth /= 1.5f;
				UpdateCameraPosition(0,0,0,CamNow);
			}
			else if(e.Button == tbbMooz){
				CamDepth *= 1.5f;
				UpdateCameraPosition(0,0,0,CamNow);
			}
			else if(e.Button == tbbAutoCamera){
				StartScrollCameraPosition(rcdata.Cursor.Matrix);
				labelTip.Text = "カーソル自動追尾 : " + (e.Button.Pushed ? "ON" : "OFF");
			}
			else{
				//	各種モード変更
				foreach(ToolBarButton tbb in tbMain.Buttons)
					if(tbb != tbbAutoCamera) tbb.Pushed = false;
				e.Button.Pushed = true;
				selected = e.Button;
				labelTip.Text = "追加モード : " + e.Button.Text;
			}
		}

		private bool InitializeGraphics() {
			presentParams.Windowed = true;
			//				presentParams.BackBufferCount = 1;
			//				presentParams.BackBufferHeight = presentParams.BackBufferWidth = 0;
			presentParams.SwapEffect = SwapEffect.Discard;
			presentParams.EnableAutoDepthStencil = true;
			presentParams.AutoDepthStencilFormat = DepthFormat.D16;
				
			try {
				device = new Device(
					0,
					DeviceType.Hardware,
					this.pictTarget,
					CreateFlags.SoftwareVertexProcessing,
					presentParams);
			}catch{
				try{
					device = new Device(
						0,
						DeviceType.Software,
						this.pictTarget,
						CreateFlags.SoftwareVertexProcessing,
						presentParams);
				}catch(Exception e){
					MessageBox.Show("Direct3Dデバイスの初期化に失敗しました。",e.Message,MessageBoxButtons.OK,MessageBoxIcon.Error);
				}
			}


			vbGuide = new VertexBuffer(
				typeof(CustomVertex.PositionOnly),
				9,
				device,
				0,
				CustomVertex.PositionOnly.Format,
				Pool.Default);

			//				defaultMat = device.Material;

			GraphicsStream gs;
			gs = vbGuide.Lock(0,0,0);
			lineCV = new Microsoft.DirectX.Direct3D.CustomVertex.PositionOnly[9];	//	PositionColored

			lineCV[0].SetPosition(new Vector3(100f,0f,0f));
			lineCV[1].SetPosition(new Vector3(0f,0f,0f));
			lineCV[2].SetPosition(new Vector3(-100f,0f,0f));
			lineCV[3].SetPosition(new Vector3(0f,100f,0f));
			lineCV[4].SetPosition(new Vector3(0f,0f,0f));
			lineCV[5].SetPosition(new Vector3(0f,-100f,0f));
			lineCV[6].SetPosition(new Vector3(0f,0f,100f));
			lineCV[7].SetPosition(new Vector3(0f,0f,0f));
			lineCV[8].SetPosition(new Vector3(0f,0f,-100f));

			gs.Write(lineCV);
			vbGuide.Unlock();

			vbWeight = new VertexBuffer(
				typeof(CustomVertex.PositionOnly),
				6,
				device,
				0,
				CustomVertex.PositionOnly.Format,
				Pool.Default);
			
			gs = vbWeight.Lock(0,0,0);

			lineCV = new CustomVertex.PositionOnly[6];

			lineCV[0].SetPosition(new Vector3(100f,0f,0f));
			lineCV[1].SetPosition(new Vector3(-100f,0f,0f));
			lineCV[2].SetPosition(new Vector3(0f,100f,0f));
			lineCV[3].SetPosition(new Vector3(0f,-100f,0f));
			lineCV[4].SetPosition(new Vector3(0f,0f,100f));
			lineCV[5].SetPosition(new Vector3(0f,0f,-100f));

			gs.Write(lineCV);
			vbWeight.Unlock();

			device.RenderState.ZBufferEnable = true;
			device.Transform.World = Matrix.Identity;
			device.Transform.View = Matrix.LookAtLH( 
				new Vector3( 0.0f, 0.0f, 0.0f ),
				new Vector3( 0.0f, 0.0f, 0.0f ),
				new Vector3( 0.0f, 1.0f, 0.0f ) );
			//				device.Transform.Projection = Matrix.PerspectiveFovLH(90/180*(float)Math.PI ,1.0f,0.5f,100.0f);
			device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f ,(float)pictTarget.Width / (float)pictTarget.Height,0.5f,100.0f);

/*			device.Lights[0].Ambient = Color.White;
			device.Lights[0].Diffuse = Color.White;
			device.Lights[0].Specular = Color.White;
			device.Lights[0].Type = LightType.Directional;
			device.Lights[0].Direction = new Vector3(1f,1f,1f);
			device.Lights[0].Enabled = true;
*/			device.RenderState.Lighting = true;
			device.RenderState.Ambient = Color.Gray;
			device.RenderState.CullMode = Cull.CounterClockwise;

			//				device.RenderState.AlphaBlendOperation = BlendOperation.Add;
			device.RenderState.SourceBlend = Blend.SourceAlpha;
			device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			device.RenderState.AlphaBlendEnable = true;

			//				rcx = new RcXFile();
			//				rcx.FileName = "cursor3.x";
			//				rcx.Load(device);

			device.DeviceLost += new EventHandler(Device_DeviceLost);

			return true;
		}

		private void Device_DeviceLost(object sender,EventArgs e){
			throw new Exception("デバイスが消失しました。");
		}
		#region 残骸
		/*
		private void Device_DeviceLost(object sender,EventArgs e){
			rcdata.DisposeMeshes();
			vbGuide.Dispose();

			try{
				device.Reset(presentParams);
			}catch(DeviceNotResetException exc){
				MessageBox.Show(exc.Message);
			}

			vbGuide = new VertexBuffer(
				typeof(CustomVertex.PositionColored),
				6,
				device,
				0,
				CustomVertex.PositionColored.Format,
				Pool.Default);

			GraphicsStream gs;
			gs = vbGuide.Lock(0,0,0);
			lineCV = new Microsoft.DirectX.Direct3D.CustomVertex.PositionColored[6];

			lineCV[0].SetPosition(new Vector3(0f,0f,0f));
			lineCV[0].Color = 0xFF0000;
			lineCV[1].SetPosition(new Vector3(-100f,0f,0f));
			lineCV[1].Color = 0xFF0000;
			lineCV[2].SetPosition(new Vector3(0f,0f,0f));
			lineCV[2].Color = 0x00FF00;
			lineCV[3].SetPosition(new Vector3(0f,100f,0f));
			lineCV[3].Color = 0x00FF00;
			lineCV[4].SetPosition(new Vector3(0f,0f,0f));
			lineCV[4].Color = 0x0000FF;
			lineCV[5].SetPosition(new Vector3(0f,0f,-100f));
			lineCV[5].Color = 0x0000FF;

			gs.Write(lineCV);

			vbGuide.Unlock();
			device.RenderState.ZBufferEnable = true;
			device.Transform.World = Matrix.Identity;
			device.Transform.View = Matrix.LookAtLH( 
				new Vector3( 0.0f, 0.0f, 0.0f ),
				new Vector3( 0.0f, 0.0f, 0.0f ),
				new Vector3( 0.0f, 1.0f, 0.0f ) );
			device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f ,1.0f,0.5f,100.0f);

			device.Lights[0].Ambient = Color.White;
			device.Lights[0].Diffuse = Color.White;
			device.Lights[0].Specular = Color.White;
			device.Lights[0].Type = LightType.Directional;
			device.Lights[0].Direction = new Vector3(1f,1f,1f);
			device.Lights[0].Enabled = true;
			device.RenderState.Lighting = true;
			device.RenderState.CullMode = Cull.CounterClockwise;

			device.RenderState.SourceBlend = Blend.SourceAlpha;
			device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
			device.RenderState.AlphaBlendEnable = true;


			rcdata.ReloadMeshes();
		}
		*/
		#endregion

		/// <summary>
		/// 現在選択されているチップの情報を右パネルに表示する。
		/// </summary>
		public void LoadChipInfo(){
			RcChipBase target = rcdata.SelectedChip;
			string[] s = target.AttrNameList;
			RcAttrValue attr;
			if(s == null)
				s = new string[0];
			for(int i = 0;i < labelAttrItems.Length;i++){
				if(s.Length > i){
					labelAttrItems[i].Text = s[i];
					attr = target[s[i]];
					cmbAttrItems[i].Text = attr.ToString();
					if(attr.Val != null){
						cmbAttrItems[i].BackColor = Color.FromKnownColor(KnownColor.Info);
						cmbAttrItems[i].ForeColor = Color.FromKnownColor(KnownColor.InfoText);
					}else{
						cmbAttrItems[i].BackColor = Color.FromKnownColor(KnownColor.Window);
						cmbAttrItems[i].ForeColor = Color.FromKnownColor(KnownColor.WindowText);
					}
					target[s[i]] = attr;
					ttMain.SetToolTip(cmbAttrItems[i],target.AttrTip(s[i]));
					labelAttrItems[i].Visible = true;
					cmbAttrItems[i].Visible = true;
				}
				else{
					labelAttrItems[i].Visible = false;
					cmbAttrItems[i].Visible = false;
				}
			}

			lstNorth.Items.Clear();
			lstSouth.Items.Clear();
			lstWest.Items.Clear();
			lstEast.Items.Clear();
			foreach(RcChipBase cb in target.Child){
				if(cb != null){
					switch(cb.JointPosition){
						case RcJointPosition.North:
							lstNorth.Items.Add(cb);
							break;
						case RcJointPosition.East:
							lstEast.Items.Add(cb);
							break;
						case RcJointPosition.South:
							lstSouth.Items.Add(cb);
							break;
						case RcJointPosition.West:
							lstWest.Items.Add(cb);
							break;
					}
				}
			}

			buttonSelChip.ImageIndex = (int)(sbyte)RcChipBase.CheckType(target);
			cmbColor.Text = target.ChipColor.ToString();

			txtName.Text = target.Name;


			if(btnRootChip.Enabled = (target.Parent != null))
				btnRootChip.Text = "親チップ\n[" + target.Parent.ToString() + "]";
			else
				btnRootChip.Text = "親チップ";

			pictAngle.Refresh();

			if(target.Comment == null || target.Comment == "")
				ttMain.SetToolTip(buttonSelChip,"コメントなし");
			else
				ttMain.SetToolTip(buttonSelChip,"コメント：\n" + target.Comment);

			parameterChanged = false;
		}

		/// <summary>
		/// 右パネルの情報を選択されているチップに適用する。
		/// </summary>
		public void ApplyChipInfo(){
			if(!parameterChanged)return;
			RcChipBase target = rcdata.SelectedChip;
			RcAttrValue attr;
			for(int i = 0;i < labelAttrItems.Length;i++){
				if(labelAttrItems[i].Visible){
					try{
						attr = target[labelAttrItems[i].Text];
						attr.SetValue(cmbAttrItems[i].Text,rcdata.vals);
						if(target[labelAttrItems[i].Text].Val != null)
							target[labelAttrItems[i].Text].Val.RefCount--;
						target[labelAttrItems[i].Text] = attr;
						if(attr.Val != null)
							attr.Val.RefCount++;
					}catch(Exception e){
						labelTip.Text = e.Message;
					}

				}
				else
					break;
			}
			target.Name = txtName.Text;
			target.ChipColor.Read(cmbColor.Text);

			LoadChipInfo();
			target.UpdateMatrix();
			rcdata.Cursor.UpdateMatrix();
			rcdata.CalcWeightCenter();

			ProcessViewPoint (rcdata.Cursor.Matrix);
			pictTarget_Paint(this,null);

			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}


		private void UpdateCameraPosition(int dX,int dY,int dZ,Matrix viewPointMatrix){
			tmrScroll.Enabled = false;
			CamPhi += dX/128f;
			CamTheta += dY/128f;
			CamDepth += dZ * 0.5f;

			if(CamTheta >= Math.PI/2)CamTheta = (float)(Math.PI/2)- float.Epsilon;
			else if(CamTheta <= -Math.PI/2) CamTheta = (float)(-Math.PI/2) + float.Epsilon;
			CamPhi %= (float)(Math.PI*2);

			Vector3 V = new Vector3(0f,0f,CamDepth);
			Vector3 W = new Vector3(0f,1f,0f);
			Vector3 P = new Vector3(0f,0f,0f);
			P.TransformCoordinate(viewPointMatrix);

			V.TransformCoordinate(Matrix.RotationX(CamTheta)*Matrix.RotationY(CamPhi));
			V += P;
			W.TransformCoordinate(Matrix.RotationX(CamTheta)*Matrix.RotationY(CamPhi));

			device.Transform.View = Matrix.LookAtLH(
				V,
				P,
				W
				);

			CamNow = P;
			ScrollCount = editOption.ScrollFrameNum;
			if(CameraOrtho)
				device.Transform.Projection = Matrix.OrthoLH(CamDepth/2f,CamDepth/2f,-256f,256f);
			pictTarget_Paint(this,null);
		}
		private void UpdateCameraPosition(int dX,int dY,int dZ,Vector3 viewPointVector){
			CamPhi += dX/128f;
			CamTheta += dY/128f;
			CamDepth += dZ * 0.5f;

			if(CamTheta >= Math.PI/2)CamTheta = (float)(Math.PI/2)- float.Epsilon;
			else if(CamTheta <= -Math.PI/2) CamTheta = (float)(-Math.PI/2) + float.Epsilon;
			CamPhi %= (float)(Math.PI*2);

			Vector3 V = new Vector3(0f,0f,CamDepth);
			Vector3 W = new Vector3(0f,1f,0f);

			V.TransformCoordinate(Matrix.RotationX(CamTheta)*Matrix.RotationY(CamPhi));
			V += viewPointVector;
			W.TransformCoordinate(Matrix.RotationX(CamTheta)*Matrix.RotationY(CamPhi));

			device.Transform.View = Matrix.LookAtLH(
				V,
				viewPointVector,
				W
				);

			if(CameraOrtho)
				device.Transform.Projection = Matrix.OrthoLH(CamDepth/2f,CamDepth/2f,-256f,256f);
			pictTarget_Paint(this,null);
		}

		public void ProcessViewPoint(Matrix nextPointMatrix){
			if(!tbbAutoCamera.Pushed)return;
			StartScrollCameraPosition(nextPointMatrix);
		}

		private void StartScrollCameraPosition(Matrix nextPointMatrix){
			CamNow = CamNext * (ScrollCount/(float)editOption.ScrollFrameNum) + CamNow * (1f - ScrollCount/(float)editOption.ScrollFrameNum);
			CamNext = new Vector3();
			CamNext.TransformCoordinate(nextPointMatrix);
			ScrollCount = 0;
			tmrScroll.Enabled = true;
		}

		private void tmrScroll_Tick(object sender, System.EventArgs e) {
			if(ScrollCount < editOption.ScrollFrameNum){
				UpdateCameraPosition(0,0,0,CamNext * (ScrollCount/(float)editOption.ScrollFrameNum) + CamNow * (1f - ScrollCount/(float)editOption.ScrollFrameNum));
				ScrollCount++;
//				labelTip.Text = ScrollCount.ToString();
			}
			else{
				CamNow = CamNext;
				UpdateCameraPosition(0,0,0,CamNext);
				tmrScroll.Enabled = false;
			}
		}

		private void frmMain_Resize(object sender, System.EventArgs e) {
			Pause = this.WindowState == FormWindowState.Minimized;
		}

		private void lstChild_SelectedIndexChanged(object sender, System.EventArgs e) {
			ListBox targetlist = (ListBox)sender;
			if(targetlist.SelectedIndex != -1){
				if(targetlist == lstNorth){
					lstEast.SelectedIndex = -1;
					lstSouth.SelectedIndex = -1;
					lstWest.SelectedIndex = -1;
				}
				else if(targetlist == lstEast){
					lstNorth.SelectedIndex = -1;
					lstSouth.SelectedIndex = -1;
					lstWest.SelectedIndex = -1;
				}
				else if(targetlist == lstSouth){
					lstNorth.SelectedIndex = -1;
					lstEast.SelectedIndex = -1;
					lstWest.SelectedIndex = -1;
				}
				else if(targetlist == lstWest){
					lstNorth.SelectedIndex = -1;
					lstEast.SelectedIndex = -1;
					lstSouth.SelectedIndex = -1;
				}
			}
		}

		private void lstChild_DoubleClick(object sender, System.EventArgs e) {
			ListBox listtarget = (ListBox)sender;
			if(listtarget.SelectedIndex != -1){
				rcdata.SelectedChip = (RcChipBase)listtarget.SelectedItem;
			}
		}

		private void btnRootChip_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip = rcdata.SelectedChip.Parent;
			labelTip.Text = "親チップに移動 : " + rcdata.SelectedChip.ToString() +
				((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
				" (" + rcdata.SelectedChip.Comment + ")" : "");
			
		}

		private void btnVal_Click(object sender, System.EventArgs e) {
			frmVals valform = new frmVals(rcdata.vals);
			Pause = true;
			Modified = (valform.ShowDialog() == DialogResult.Yes);
			Pause = false;
			SetValDropList();
			rcdata.model.root.UpdateMatrix();
			rcdata.Cursor.UpdateMatrix();
			LoadChipInfo();

		}

		private void SetValDropList(){
			foreach(ComboBox acb in cmbAttrItems){
				acb.Items.Clear();
				foreach(RcVal v in rcdata.vals.list){
					acb.Items.Add(v.ValName);
					acb.Items.Add("-" + v.ValName);
				}
				if(acb.Items.Count == 0)
					acb.Items.Add("(使用可能な変数はありません)");
			}
		}

		private void buttonSelChip_Click(object sender, System.EventArgs e) {
			ctmChipType.Show((Control)sender,new Point(((Control)sender).Width / 2,((Control)sender).Height / 2));
		}

		private void SetUndo(UndoType type,params RcChipBase[] chips){
			undo.chips = chips;
			undo.type = type;
		}

		private void Undo(){
			switch(undo.type){
				case UndoType.Added:
					#region Added処理
					#endregion
					break;
				case UndoType.Removed:
					#region Removed処理
					#endregion
					break;
				case UndoType.Modified:
					#region Modified処理
					#endregion
					break;
				default:
					break;
			}
		}

		private void menuItem2_Click(object sender, System.EventArgs e) {
		}

		private void pictAngle_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			bool selfget = false;
			Graphics grp;

			if(e == null){
				grp = pictAngle.CreateGraphics();
				selfget = true;
			}else
				grp = e.Graphics;
		
			Rectangle target = pictAngle.ClientRectangle;

			//labelTip.Text = target.ToString();
			
			RcAttrValue attr;
			try{
				attr = rcdata.SelectedChip["Angle"];
			}
			catch{
				grp.FillRectangle(Brushes.Gray,target);
				if(selfget)grp.Dispose();
				return;
			}
			grp.FillRectangle(Brushes.Navy,target);

			Pen p = new Pen(Color.Red,1f);
			p.EndCap = System.Drawing.Drawing2D.LineCap.DiamondAnchor;

			grp.DrawEllipse(Pens.White,target.X,target.Y,target.Width-1,target.Height-1);
			grp.DrawLine(Pens.White,new Point(0, target.Height / 2),new Point(target.Width-1,target.Height / 2));
			grp.DrawLine(Pens.White,new Point(target.Width / 2, 0),new Point(target.Width / 2,target.Height - 1));

			System.Drawing.Font f = new System.Drawing.Font("ＭＳ ゴシック",8f);
			if(attr.Val != null){
				p.Color = Color.Yellow;
				grp.DrawPie(p,
					target,
					attr.isNegative ? -attr.Val.Min : attr.Val.Min,
					attr.isNegative ?  attr.Val.Min - attr.Val.Max : attr.Val.Max - attr.Val.Min
					);
				p.Color = Color.Green;
			}
			else if(editOption.AngleViewGrid > 0){
				grp.DrawString(editOption.AngleViewGrid.ToString(),f,Brushes.Yellow ,0f,0f);
			}

			grp.DrawLine( p,
				new Point(target.Width / 2,target.Height / 2),
				new Point(
				(int)Math.Ceiling(target.Width / 2 * (1 + Math.Cos(attr.Value * DegToRad))),
				(int)Math.Ceiling(target.Height / 2 * (1 + Math.Sin(attr.Value * DegToRad)))
				)
				);

			if(selfget)
				grp.Dispose();

			p.Dispose();
		}

		private void pictAngle_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Left){
				angledrag = true;
				pictAngle_MouseMove(sender,e);
			}
		}

		private void pictAngle_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Left)
				angledrag = false;
		}

		private void pictAngle_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!angledrag)return;
			RcAttrValue buff;
			try{
				buff = rcdata.SelectedChip["Angle"];
				if(buff.Val != null)throw new Exception();
			}catch{
				return;
			}
			buff.Const = (float)(Math.Atan2(e.Y - pictAngle.ClientRectangle.Height / 2  ,e.X - pictAngle.ClientRectangle.Width / 2) * RadToDeg);
			if(editOption.AngleViewGrid > 0){
				buff.Const = ((int)(buff.Const / editOption.AngleViewGrid + 180.5f) - 180) * editOption.AngleViewGrid;
			}
			if(buff.Const == -180f)buff.Const = 180f;
			rcdata.SelectedChip["Angle"] = buff;
			rcdata.SelectedChip.UpdateMatrix();
			rcdata.CalcWeightCenter();
			LoadChipInfo();
			ProcessViewPoint(rcdata.Cursor.Matrix);
			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void miRotateRight_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.RotateRight();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void miRotateLeft_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.RotateLeft();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void miReverseX_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.ReverseX();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void miReverseY_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.ReverseY();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void miReverseZ_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip.ReverseZ();
			rcdata.SelectedChip.UpdateMatrix();
			LoadChipInfo();
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void miCut_Click(object sender, System.EventArgs e) {
			actionCut();
		}

		private void miCopy_Click(object sender, System.EventArgs e) {
			actionCopy();
		}

		private void miDelete_Click(object sender, System.EventArgs e) {
			actionRemove();
		}

		private void ctmChipType_Popup(object sender, System.EventArgs e) {
			foreach(MenuItem m in miChange.MenuItems)
				m.Enabled = true;
			miChange.Enabled = true;

			switch(buttonSelChip.ImageIndex){
				case 0: //	コア
					miChange.Enabled = false;
					miCut.Enabled = false;
					miCopy.Enabled = false;
					miDelete.Enabled = false;
					break;
				case 1:	//	チップ
					miChangeChip.Enabled = false;
					goto default;
				case 2:	//	フレーム
					miChangeFrame.Enabled = false;
					goto default;
				case 3:	//	ラダー
					miChangeRudder.Enabled = false;
					goto default;
				case 4:	//	ラダーフレーム
					miChangeRudderF.Enabled = false;
					goto default;
				case 5:	//	トリム
					miChangeTrim.Enabled = false;
					goto default;
				case 6:	//	トリムフレーム
					miChangeTrimF.Enabled = false;
					goto default;
				case 7: //	ホイール
					miChangeWheel.Enabled = false;
					goto default;
				case 8:	//	無反動ホイール
					miChangeRLW.Enabled = false;
					goto default;
				case 9:	//	ジェット
					miChangeJet.Enabled = false;
					goto default;
				case 10:	//	ウェイト
					miChangeWeight.Enabled = false;
					goto default;
				case 11:	//	カウル
					miChangeCowl.Enabled = false;
					goto default;
				case 12:	//	アーム
					miChangeArm.Enabled = false;
					goto default;
				default:
					miCut.Enabled = true;
					miCopy.Enabled = true;
					miDelete.Enabled = true;
					break;
			}

		}

		private void btnKey_Click(object sender, System.EventArgs e) {
			if(rcdata.vals.Count == 0){
				MessageBox.Show("変数がひとつも宣言されていません。\n先に[Val{...}編集]を行ってください。");
				return;
			}
			frmKeys keyform = new frmKeys(rcdata.vals,rcdata.keys);
			Modified = (keyform.ShowDialog() == DialogResult.Yes);
		}

		private void cmbAttrItems_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(e.KeyChar == 13){
				ApplyChipInfo();
				e.Handled = true;
				labelTip.Text = "属性の変更を行いました。";
			}
			else if(!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && !char.IsControl(e.KeyChar)){
				e.Handled = true;
			}

//			labelTip.Text = e.KeyChar.ToString() + "(" + (int)e.KeyChar + ")";
			

		}

		private void cmbAttrItems_TextChanged(object sender, System.EventArgs e) {
			if(sender is ComboBox){
				ComboBox attrcombo = (ComboBox)sender;
				if(attrcombo.Text == "(使用可能な変数はありません)")
					LoadChipInfo();
			}
		}

		private void txtName_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(e.KeyChar == 13){
				ApplyChipInfo();
				e.Handled = true;
			}
			else if(!char.IsLetter(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '_' && !char.IsControl(e.KeyChar))
				e.Handled = true;
			else if(char.IsDigit(e.KeyChar) && txtName.Text.Length < 1)
				e.Handled = true;

		}

		private void cmbColor_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if('0' <= e.KeyChar && e.KeyChar <= '9' ||
				'A' <= e.KeyChar && e.KeyChar <= 'F' ||
				'a' <= e.KeyChar && e.KeyChar <= 'f' ||
				e.KeyChar == '#'){
				return;
			}
			if(char.IsControl(e.KeyChar)){
				if(e.KeyChar == 13){
					ApplyChipInfo();
					e.Handled = true;
				}
				return;
			}
			e.Handled = true;

		}

		private void pictPalette_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			palettedrag = true;
			pictPalette_MouseMove(sender,e);
		}

		private void pictPalette_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(!palettedrag)return;
			Graphics g = pictPalette.CreateGraphics();
			IntPtr hdc = g.GetHdc();
			int i = GetPixel(hdc,e.X,e.Y);

			g.ReleaseHdc(hdc);
			g.Dispose();

			rcdata.SelectedChip.ChipColor = new RcColor(i & 0xFF,(i & 0xFF00) >> 8 ,(i & 0xFF0000) >> 16);
			//	バイトオーダーが逆向きなのにはワラタ
			LoadChipInfo();
			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void pictPalette_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			palettedrag = false;
		}

		private void pictAngle_Click(object sender, System.EventArgs e) {
		
		}

		private void miFileOpen_Click(object sender, System.EventArgs e) {
			/*			StreamReader test = new StreamReader(@"mmi.txt");
						string buff = test.ReadToEnd();

						rcdata.Parse(buff);
			*/		
			if(!Modified || MessageBox.Show("現在のモデルを破棄します。","初期化確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK){
				dlgOpen.Filter = "RigidChipsModeler File(*.rcm)|*.rcm";
				dlgOpen.Multiselect = false;
				dlgOpen.ShowReadOnly = false;
				bool buffer = editOption.ConvertParentAttributes;
				editOption.ConvertParentAttributes = false;
				if(dlgOpen.ShowDialog() == DialogResult.OK){
					rcdata.UnregisterChipAll(rcdata.model.root);
					rcdata.keys = new RcKeyList(RcData.KeyCount);
					rcdata.vals = new RcValList();
					rcdata.model = new RcModel(rcdata);
					rcdata.headercomment = rcdata.script = "";
					rcdata.SelectedChip = rcdata.model.root;
					rcdata.RegisterChip(rcdata.model.root);
					rcdata.Load(dlgOpen.FileName);

					GC.Collect();
					Modified = false;
					if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
					EditingFileName = dlgOpen.FileName;
					SetValDropList();

					labelTip.Text = "RCMファイルを開きました : " + System.IO.Path.GetFileNameWithoutExtension(dlgOpen.FileName);
				}
				editOption.ConvertParentAttributes = buffer;
			}
			
		}

		private void panelB_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
		
		}

		private void btnPrev_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip = rcdata.GetChipFromLib(rcdata.SelectedChip.RegistID-1);
		}

		private void btnNext_Click(object sender, System.EventArgs e) {
			rcdata.SelectedChip = rcdata.GetChipFromLib(rcdata.SelectedChip.RegistID+1);
		}

		private void miPaletteAllPaint_Click(object sender, System.EventArgs e) {
			RcChipBase c;
			for(int i = 0;i < RcData.MaxChipCount;i++){
				c = rcdata.GetChipFromLib(i);
				if(c == null)return;
				c.ChipColor = rcdata.SelectedChip.ChipColor;
			}
			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
			labelTip.Text = "色を変更しました。";
		}

		private void miPaletteChildPaint_Click(object sender, System.EventArgs e) {
			System.Collections.Queue paintqueue = new System.Collections.Queue();
			RcChipBase buff;

			paintqueue.Enqueue(rcdata.SelectedChip);

			while(paintqueue.Count > 0){
				buff = (RcChipBase)paintqueue.Dequeue();
				foreach(RcChipBase c in buff.Child){
					if(c == null)continue;
					c.ChipColor = buff.ChipColor;
					paintqueue.Enqueue(c);
				}
			}
			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
			labelTip.Text = "色を変更しました。";
		}

		private void miFileSave_Click(object sender, System.EventArgs e) {
			if(EditingFileName == ""){
				miFileSaveAs_Click(sender,e);
				return;
			}
			rcdata.Save(editingFileName);
			Modified = false;
			labelTip.Text = "RCMファイルを保存しました : " + System.IO.Path.GetFileNameWithoutExtension(dlgSave.FileName);
		}

/*		private void miAngle_Click(object sender, System.EventArgs e) {
			RcAttrValue buff;
			try{
				buff = rcdata.SelectedChip["Angle"];
				if(buff.Val != null)throw new Exception();
			}catch{
				return;
			}
			buff.Const = float.Parse(((MenuItem)sender).Text);
			rcdata.SelectedChip["Angle"] = buff;
			rcdata.SelectedChip.UpdateMatrix();
			rcdata.CalcWeightCenter();
			LoadChipInfo();
			ProcessViewPoint(rcdata.Cursor.Matrix);
			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}
*/
		private void ctmAngles_Popup(object sender, System.EventArgs e) {
			ContextMenu menu = (ContextMenu)sender;

		}

		private void miFileNew_Click(object sender, System.EventArgs e) {
			if(!Modified || MessageBox.Show("現在のモデルを破棄します。","初期化確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK){
				rcdata.UnregisterChipAll(rcdata.model.root);
				rcdata.keys = new RcKeyList(RcData.KeyCount);
				rcdata.vals = new RcValList();
				rcdata.model = new RcModel(rcdata);
				rcdata.headercomment = rcdata.script = "";
				rcdata.SelectedChip = rcdata.model.root;
				rcdata.RegisterChip(rcdata.model.root);

				GC.Collect();
				EditingFileName = "";
				Modified = false;
				if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
				labelTip.Text = "情報を破棄し、新しいモデルデータを開始しました。";
			}
		}

		private void ctmChildList_Popup(object sender, System.EventArgs e) {
			ListBox list = (ListBox)((ContextMenu)sender).SourceControl;

			miListCopy.Enabled = miListCut.Enabled = miListSelect.Enabled = ( list.SelectedIndex >= 0);
			miListAdd.Enabled = (selected.Text != "選択") && (selected.Text != "視点") ;
			switch(list.Name){
				case "lstNorth":
					jointPositionBuffer = RcJointPosition.North;
					break;
				case "lstSouth":
					jointPositionBuffer = RcJointPosition.South;
					break;
				case "lstEast":
					jointPositionBuffer = RcJointPosition.East;
					break;
				case "lstWest":
					jointPositionBuffer = RcJointPosition.West;
					break;
				default:
					jointPositionBuffer = RcJointPosition.NULL;
					break;
			}

		}

		private void miListAdd_Click(object sender, System.EventArgs e) {


			switch(selected.Text){
				case "貼り付け":
					try{
						RcChipBase buffer = clipboard.Clone(true,null);
						buffer.Attach(rcdata.SelectedChip,jointPositionBuffer);
						rcdata.SelectedChip = buffer;
						labelTip.Text = "貼り付けを行いました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					rcdata.CheckBackTrack();
					break;
				case "チップ":
					try{
						rcdata.SelectedChip = new RcChipChip(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "フレーム":
					try{
						rcdata.SelectedChip = new RcChipFrame(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ウェイト":
					try{
						rcdata.SelectedChip = new RcChipWeight(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "カウル":
					try{
						rcdata.SelectedChip = new RcChipCowl(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "ラダー":
					try{
						rcdata.SelectedChip = new RcChipRudder(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ラダーフレーム":
					try{
						rcdata.SelectedChip = new RcChipRudderF(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "トリム":
					try{
						rcdata.SelectedChip = new RcChipTrim(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "トリムフレーム":
					try{
						rcdata.SelectedChip = new RcChipTrimF(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ホイール":
					try{
						rcdata.SelectedChip = new RcChipWheel(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "無反動ホイール":
					try{
						rcdata.SelectedChip = new RcChipRLW(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				case "ジェット":
					try{
						rcdata.SelectedChip = new RcChipJet(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					
					break;
				case "アーム":
					try{
						rcdata.SelectedChip = new RcChipArm(rcdata,rcdata.SelectedChip,jointPositionBuffer);
						labelTip.Text = selected.Text + "を追加しました。";
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					

					break;
				default:
					break;
			}

			//			UpdateCameraPosition(0,0,0,rcdata.Cursor.Matrix);
			pictTarget_Paint(this,null);

			Modified = true;
			if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
		}

		private void miFileImport_Click(object sender, System.EventArgs e) {
			if(!Modified || MessageBox.Show("現在のモデルを破棄します。","初期化確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK){
				dlgOpen.Filter = "RigidChips Data File(*.rcd;*.txt)|*.rcd;*.txt|All files(*.*)|*.*";
				dlgOpen.Multiselect = false;
				dlgOpen.ShowReadOnly = false;
				if(dlgOpen.ShowDialog() == DialogResult.OK){
					rcdata.UnregisterChipAll(rcdata.model.root);
					rcdata.keys = new RcKeyList(RcData.KeyCount);
					rcdata.vals = new RcValList();
					rcdata.model = new RcModel(rcdata);


					rcdata.headercomment = rcdata.script = "";
					rcdata.RegisterChip(rcdata.model.root);
					StreamReader file = new StreamReader(dlgOpen.FileName,System.Text.Encoding.Default);
					rcdata.Parse(file.ReadToEnd());

					rcdata.SelectedChip = rcdata.model.root;
					SetValDropList();

					GC.Collect();
				}
				Modified = false;
				if(treeview != null && !treeview.IsDisposed)treeview.GenerateTree();
				labelTip.Text = "モデルデータを読み込みました : " + System.IO.Path.GetFileNameWithoutExtension(dlgOpen.FileName);
			}
		}

		private void menuItem14_Click(object sender, System.EventArgs e) {
			MessageBox.Show(string.Format("RigidChipsModeler\n\tVersion : {0}.{1}\n\tAssembly : {2}-{3}",Application.ProductVersion.Split('.')),"Version Infomation",MessageBoxButtons.OK,MessageBoxIcon.Information);
		}

		private void miFileExport_Click(object sender, System.EventArgs e) {
			dlgSave.Filter = "RigidChips Data File(*.rcd)|*.rcd";
			dlgSave.DefaultExt = "rcd";
			dlgSave.OverwritePrompt = true;
			string output;
			if(dlgSave.ShowDialog() == DialogResult.OK){
				output = rcdata.vals.ToString() + rcdata.keys.ToString() + rcdata.model.ToString();
				if(rcdata.script != ""){
					output += (rcdata.luascript ? "Lua" : "Script") + "{\n" + rcdata.script + "}\n";
				}
				StreamWriter tw = new StreamWriter(dlgSave.FileName);
//				MessageBox.Show(output);
//				MessageBox.Show(rcdata.chipCount.ToString());
				tw.Write(output);
				tw.Flush();
				tw.Close();
                
				labelTip.Text = "モデルデータを出力しました : " + System.IO.Path.GetFileNameWithoutExtension(dlgSave.FileName);

			}
		}

		private void miFileSaveAs_Click(object sender, System.EventArgs e) {
			dlgSave.Filter = "RigidChipsModeler File(*.rcm)|*.rcm";
			dlgSave.DefaultExt = "rcm";
			dlgSave.OverwritePrompt = true;
			if(dlgSave.ShowDialog() == DialogResult.OK){
				rcdata.Save(dlgSave.FileName);
				EditingFileName = dlgSave.FileName;
				labelTip.Text = "RCDファイルを保存しました : " + System.IO.Path.GetFileNameWithoutExtension(dlgSave.FileName);
				Modified = false;
				EditingFileName = dlgSave.FileName;
			}

		}

		private void miListSelect_Click(object sender, System.EventArgs e) {
			ListBox list;
			switch(jointPositionBuffer){
				case RcJointPosition.North:
					list = lstNorth;
					break;
				case RcJointPosition.South:
					list = lstSouth;
					break;
				case RcJointPosition.East:
					list = lstEast;
					break;
				case RcJointPosition.West:
					list = lstWest;
					break;
				default:
					return;
			}
			if(list.SelectedIndex < 0)return;

			rcdata.SelectedChip = (RcChipBase)list.SelectedItem;
			labelTip.Text = "カーソルを移動しました : " + rcdata.SelectedChip.ToString() + 
				((rcdata.SelectedChip.Comment != null && rcdata.SelectedChip.Comment != "") ? 
				" (" + rcdata.SelectedChip.Comment + ")" : "");

		}

		private void miListCut_Click(object sender, System.EventArgs e) {
			ListBox list;
			switch(jointPositionBuffer){
				case RcJointPosition.North:
					list = lstNorth;
					break;
				case RcJointPosition.South:
					list = lstSouth;
					break;
				case RcJointPosition.East:
					list = lstEast;
					break;
				case RcJointPosition.West:
					list = lstWest;
					break;
				default:
					return;
			}
			if(list.SelectedIndex < 0)return;

			clipboard = (RcChipBase)list.SelectedItem;
			rcdata.SelectedChip = clipboard.Parent;
			clipboard.Detach();
		}

		private void miListCopy_Click(object sender, System.EventArgs e) {
			ListBox list;
			switch(jointPositionBuffer){
				case RcJointPosition.North:
					list = lstNorth;
					break;
				case RcJointPosition.South:
					list = lstSouth;
					break;
				case RcJointPosition.East:
					list = lstEast;
					break;
				case RcJointPosition.West:
					list = lstWest;
					break;
				default:
					return;
			}
			if(list.SelectedIndex < 0)return;

			clipboard = ((RcChipBase)list.SelectedItem).Clone(true,null);
			LoadChipInfo();
			ProcessViewPoint(rcdata.SelectedChip.Matrix);

		}

		private void miToolTree_Click(object sender, System.EventArgs e) {
			if(treeview == null)
				treeview = new frmTree(rcdata,ctmChipType);
			else{
				treeview.Dispose();
				treeview = null;
				miToolTree_Click(sender,e);
				return;
			}

			treeview.Show();
		}

		private void RcData_SelectedChipChanged(object param){
			RcChipBase target = (RcChipBase)param;
			LoadChipInfo();
			ProcessViewPoint(target.Matrix);
		}

		private void miCommentEdit_Click(object sender, System.EventArgs e) {
			string s = dlgTextInput.ShowDialog(rcdata.SelectedChip.Comment,"付加するコメントを入力してください(空で消去)。",0);
			if(s != null)
				rcdata.SelectedChip.Comment = s;
		}

		private void miChipComment_Popup(object sender, System.EventArgs e) {

		}

		private void miCommentDisplay_Popup(object sender, System.EventArgs e) {
		}

		private void miFile_Click(object sender, System.EventArgs e) {
		
		}

		private void miHelpReadme_Click(object sender, System.EventArgs e) {
			System.Diagnostics.Process p = new System.Diagnostics.Process();

			p.StartInfo = new System.Diagnostics.ProcessStartInfo("RCM説明書.txt");
			p.Start();
		}

		private void miConfigDraw_Click(object sender, System.EventArgs e) {
			configwindow.NowTabPage = 0;
			configwindow.Show();
			configwindow.Focus();
		}

		private void miChangeType(object sender, System.EventArgs e) {
			MenuItem item = (MenuItem)sender;
			RcChipBase target = rcdata.SelectedChip;

			if(item == miChangeCowl){
				foreach(RcChipBase c in target.Child){
					if(! (c is RcChipCowl)){
						if(MessageBox.Show("カウル以外のチップが接続されているため、カウルに変更できません。\n接続されているチップをすべてカウルに変更しますか？","タイプ変更エラー",MessageBoxButtons.YesNo,MessageBoxIcon.Question)
							== DialogResult.Yes){

							//	派生チップをすべてカウルに変更する再帰処理
						}
						else
							return;
					}
				}
				rcdata.SelectedChip = target.ChangeType(RcChipType.Cowl);
			}
			else if(item == miChangeChip)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Chip);
			else if(item == miChangeFrame)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Frame);
			else if(item == miChangeRudder)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Rudder);
			else if(item == miChangeRudderF)
				rcdata.SelectedChip = target.ChangeType(RcChipType.RudderF);
			else if(item == miChangeTrim)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Trim);
			else if(item == miChangeTrimF)
				rcdata.SelectedChip = target.ChangeType(RcChipType.TrimF);
			else if(item == miChangeWheel)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Wheel);
			else if(item == miChangeRLW)
				rcdata.SelectedChip = target.ChangeType(RcChipType.RLW);
			else if(item == miChangeJet)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Jet);
			else if(item == miChangeWeight)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Weight);
			else if(item == miChangeArm)
				rcdata.SelectedChip = target.ChangeType(RcChipType.Arm);
			else
				throw new Exception("不明なチップ種類への変更が指定されました : " + item.Text);			

		}

		private void miCommentDelete_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show("以下のコメントを削除します。\n\n","コメント削除確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK){
				rcdata.SelectedChip.Comment = "";
			}
		}

		private void miToolScript_Click(object sender, System.EventArgs e) {
			if(scriptform == null)
				scriptform = new frmScript(rcdata);

			scriptform.Show();
		}

		private void miFileQuit_Click(object sender, System.EventArgs e) {
			Application.Exit();
		}

		private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(Modified){
				switch(MessageBox.Show("変更されています。保存しますか？","RigidChipsModeler",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question)){
					case DialogResult.Yes:
						miFileSave_Click(sender,EventArgs.Empty);
						e.Cancel = Modified;
						break;
					case DialogResult.No:
						break;
					default:
						e.Cancel = true;
						break;
				}
			}
		}

		private void miConfigOutput_Click(object sender, System.EventArgs e) {
			configwindow.NowTabPage = 1;
			configwindow.Show();
			configwindow.Focus();
		}


		private void threadPreview(){

		}

		private void miConfigEdit_Click(object sender, System.EventArgs e) {
			configwindow.NowTabPage = 2;
			configwindow.Show();
			configwindow.Focus();
		}

		private void miAngleGrid_Click(object sender, System.EventArgs e) {
			MenuItem item = (MenuItem)sender;
			if(item == miAngleGrid1)
				editOption.AngleViewGrid = 1;
			else if(item == miAngleGrid5)
				editOption.AngleViewGrid = 5;
			else if(item == miAngleGrid15)
				editOption.AngleViewGrid = 15;
			else if(item == miAngleGrid30)
				editOption.AngleViewGrid = 30;
			else if(item == miAngleGrid90)
				editOption.AngleViewGrid = 90;
			else
				editOption.AngleViewGrid = 0;
			
			pictAngle_Paint(sender,null);
		}

		private void frmMain_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(Control.ModifierKeys == Keys.None && !txtName.Focused && !cmbColor.Focused && !char.IsNumber(e.KeyChar) && e.KeyChar != '-'){
				//				labelTip.Text = ((int)e.KeyChar).ToString() + "[" + e.KeyChar.ToString() + "]";

				switch(e.KeyChar){
					case 'q':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbChip));
						break;
					case 'w':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbFrame));
						break;
					case 'e':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbRudder));
						break;
					case 'r':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbRudderF));
						break;
					case 't':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbTrim));
						break;
					case 'y':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbTrimF));
						break;
					case 'a':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbWheel));
						break;
					case 's':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbRLW));
						break;
					case 'd':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbJet));
						break;
					case 'f':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbWeight));
						break;
					case 'g':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbCowl));
						break;
					case 'h':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbArm));
						break;
					case 'z':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbCursor));
						break;
					case 'x':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbCamera));
						break;
					case 'c':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbAutoCamera));
						break;
					case 'v':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbPaste));
						break;
						
/*					case 'b':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbChip));
						break;
					case 'n':
						tbMain_ButtonClick(this,new ToolBarButtonClickEventArgs(tbbChip));
						break;
					default:
						labelTip.Text = ((int)e.KeyChar).ToString();
						break;
*/				
				}
			}
		}

		private void cmbAttrItems_Leave(object sender, System.EventArgs e) {
			if(editOption.AttributeAutoApply)
				ApplyChipInfo();
				
		}

		private void txtAttrs_Enter(object sender, System.EventArgs e) {
			((Control)sender).BackColor = Color.FromArgb(drawOption.CursorFrontColor.R / 2 + 128, drawOption.CursorFrontColor.G/2 + 128,drawOption.CursorFrontColor.B/2 + 128);
		}

		private void txtAttrs_Leave(object sender, System.EventArgs e) {
			((Control)sender).BackColor = Color.FromKnownColor(KnownColor.Window);
			cmbAttrItems_Leave(this,e);
		}

		private void frmMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete)
				if(e.Control)
				actionRemove();
		}

		private void ChipInfo_TextChanged(object sender, System.EventArgs e) {
			parameterChanged = true;
		}

		private void lstChild_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(e.KeyChar == 13){
				lstChild_DoubleClick(sender,null);
			}
		}


	}
	#region 今となっては使わない
	class XModel{
		private Mesh mesh = null;
		private Material[] mat;
		private Texture[] tex;

		private Device dev;

		public XModel(Device device){
			this.dev = device;
		}

		public bool LoadFile(string Filename){
			ExtendedMaterial[] matbuff;
			mesh = Mesh.FromFile(Filename,MeshFlags.SystemMemory,dev,out matbuff);
			tex  = new Texture[matbuff.Length];
			mat = new Material[matbuff.Length];

			for( int i=0; i<matbuff.Length; i++ ) {
				mat[i] = matbuff[i].Material3D;
				mat[i].Ambient = mat[i].Diffuse;
            
				if(matbuff[i].TextureFilename != null)
					tex[i] = TextureLoader.FromFile(dev, matbuff[i].TextureFilename);
				
			}

			return true;	//	とりあえず
		}

		public void DrawModel(){
			if(dev == null)return;

			for( int i=0; i<this.mat.Length; i++ ) {
				// Set the material and texture for this subset.
				dev.Material = mat[i];
				dev.SetTexture(0, tex[i]);
        
				// Draw the mesh subset.
				mesh.DrawSubset(i);
			}
		}

		public void DrawModelWithAlpha(int Alpha){
			if(dev == null)return;

			for( int i=0; i < this.mat.Length;i++){
				Material m = mat[i];
				m.Ambient = Color.FromArgb(Alpha,m.Ambient);
				m.Emissive = Color.FromArgb(Alpha,m.Emissive);
				m.Diffuse = Color.FromArgb(Alpha,m.Diffuse);

				dev.Material = m;
				dev.SetTexture(0,tex[i]);

				mesh.DrawSubset(i);
			}
		}
	}
	#endregion
}
