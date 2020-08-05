using System;
using System.Drawing;
//using System.Collections;
//using System.ComponentModel;
using System.Windows.Forms;
//using System.Data;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using RigidChips;


//プロジェクト名:「RCEの何か。」(仮
namespace manageddx {
	public class frmMain : System.Windows.Forms.Form {
		struct DragSign{
			public bool Draging;
			public int StartX ,StartY;
			public int PrevX,  PrevY;
		}

		Device device = null;              // 1. Create rendering device
		PresentParameters presentParams = new PresentParameters();

		CustomVertex.PositionColored[] guidecv;
	
		float CamTheta = -(float)Math.PI / 8f;
		float CamPhi = (float)Math.PI / 8f;
		float CamDepth = 8f;
		VertexBuffer vbGuide = null;
		DragSign draging;
		int MouseX,MouseY;
		bool LeastIsLeftButton = false;

		Vector3 CamNow;
		Vector3 CamNext;
		int ScrollCount = 0;

		ToolBarButton selected;

		bool Pause = false;

		RcData rcdata;
		bool Initialized = false;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.ToolBarButton tbbCursor;
		private System.Windows.Forms.ToolBarButton tbbCut;
		private System.Windows.Forms.ToolBarButton tbbCopy;
		private System.Windows.Forms.ToolBarButton tbbPaste;
		private System.Windows.Forms.ToolBarButton tbbRemove;
		private System.Windows.Forms.PictureBox pictTarget;
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
		private System.Windows.Forms.Label labelAttrItem7;
		private System.Windows.Forms.ComboBox cmbAttrItem1;
		private System.Windows.Forms.ComboBox cmbAttrItem2;
		private System.Windows.Forms.ComboBox cmbAttrItem3;
		private System.Windows.Forms.ComboBox cmbAttrItem4;
		private System.Windows.Forms.ComboBox cmbAttrItem5;
		private System.Windows.Forms.ComboBox cmbAttrItem6;
		private System.Windows.Forms.ComboBox cmbAttrItem7;
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
		private System.Windows.Forms.TextBox txtDebug;
		private System.Windows.Forms.ComboBox[] cmbAttrItems;

		public frmMain() {
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//

			#region	凡調なる配列の作成
			labelAttrItems = new Label[7];
			labelAttrItems[0] = labelAttrItem1;
			labelAttrItems[1] = labelAttrItem2;
			labelAttrItems[2] = labelAttrItem3;
			labelAttrItems[3] = labelAttrItem4;
			labelAttrItems[4] = labelAttrItem5;
			labelAttrItems[5] = labelAttrItem6;
			labelAttrItems[6] = labelAttrItem7;
			cmbAttrItems = new ComboBox[7];
			cmbAttrItems[0] = cmbAttrItem1;
			cmbAttrItems[1] = cmbAttrItem2;
			cmbAttrItems[2] = cmbAttrItem3;
			cmbAttrItems[3] = cmbAttrItem4;
			cmbAttrItems[4] = cmbAttrItem5;
			cmbAttrItems[5] = cmbAttrItem6;
			cmbAttrItems[6] = cmbAttrItem7;
			#endregion

			
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
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.labelTip = new System.Windows.Forms.Label();
			this.tbMain = new System.Windows.Forms.ToolBar();
			this.tbbCursor = new System.Windows.Forms.ToolBarButton();
			this.tbbCut = new System.Windows.Forms.ToolBarButton();
			this.tbbCopy = new System.Windows.Forms.ToolBarButton();
			this.tbbPaste = new System.Windows.Forms.ToolBarButton();
			this.tbbRemove = new System.Windows.Forms.ToolBarButton();
			this.tbbSeparator1 = new System.Windows.Forms.ToolBarButton();
			this.tbbZoom = new System.Windows.Forms.ToolBarButton();
			this.tbbMooz = new System.Windows.Forms.ToolBarButton();
			this.tbbSeparator2 = new System.Windows.Forms.ToolBarButton();
			this.tbbChip = new System.Windows.Forms.ToolBarButton();
			this.tbbFrame = new System.Windows.Forms.ToolBarButton();
			this.tbbWeight = new System.Windows.Forms.ToolBarButton();
			this.tbbCowl = new System.Windows.Forms.ToolBarButton();
			this.tbbSeparator3 = new System.Windows.Forms.ToolBarButton();
			this.tbbRudder = new System.Windows.Forms.ToolBarButton();
			this.tbbRudderF = new System.Windows.Forms.ToolBarButton();
			this.tbbTrim = new System.Windows.Forms.ToolBarButton();
			this.tbbTrimF = new System.Windows.Forms.ToolBarButton();
			this.tbbWheel = new System.Windows.Forms.ToolBarButton();
			this.tbbRLW = new System.Windows.Forms.ToolBarButton();
			this.tbbJet = new System.Windows.Forms.ToolBarButton();
			this.tbbArm = new System.Windows.Forms.ToolBarButton();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.pictTarget = new System.Windows.Forms.PictureBox();
			this.panelB = new System.Windows.Forms.Panel();
			this.txtDebug = new System.Windows.Forms.TextBox();
			this.btnKey = new System.Windows.Forms.Button();
			this.btnVal = new System.Windows.Forms.Button();
			this.panelAttr = new System.Windows.Forms.Panel();
			this.panelAttrValue = new System.Windows.Forms.Panel();
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
			this.lstNorth = new System.Windows.Forms.ListBox();
			this.lstEast = new System.Windows.Forms.ListBox();
			this.lstWest = new System.Windows.Forms.ListBox();
			this.buttonSelChip = new System.Windows.Forms.Button();
			this.tmr = new System.Windows.Forms.Timer(this.components);
			this.ttMain = new System.Windows.Forms.ToolTip(this.components);
			this.tmrScroll = new System.Windows.Forms.Timer(this.components);
			this.panelB.SuspendLayout();
			this.panelAttr.SuspendLayout();
			this.panelAttrValue.SuspendLayout();
			this.panelAttrName.SuspendLayout();
			this.SuspendLayout();
			// 
			// mnuMain
			// 
			this.mnuMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.menuItem1,
																					this.menuItem2,
																					this.menuItem3,
																					this.menuItem4});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "ファイル(&F)";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "編集(&E)";
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "設定(&C)";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.Text = "ヘルプ(&H)";
			// 
			// labelTip
			// 
			this.labelTip.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelTip.Location = new System.Drawing.Point(0, 28);
			this.labelTip.Name = "labelTip";
			this.labelTip.Size = new System.Drawing.Size(794, 15);
			this.labelTip.TabIndex = 8;
			this.labelTip.Text = "ABCabcXYZxyzあいうえお";
			this.labelTip.Click += new System.EventHandler(this.labelTip_Click);
			// 
			// tbMain
			// 
			this.tbMain.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																					  this.tbbCursor,
																					  this.tbbCut,
																					  this.tbbCopy,
																					  this.tbbPaste,
																					  this.tbbRemove,
																					  this.tbbSeparator1,
																					  this.tbbZoom,
																					  this.tbbMooz,
																					  this.tbbSeparator2,
																					  this.tbbChip,
																					  this.tbbFrame,
																					  this.tbbWeight,
																					  this.tbbCowl,
																					  this.tbbSeparator3,
																					  this.tbbRudder,
																					  this.tbbRudderF,
																					  this.tbbTrim,
																					  this.tbbTrimF,
																					  this.tbbWheel,
																					  this.tbbRLW,
																					  this.tbbJet,
																					  this.tbbArm});
			this.tbMain.ButtonSize = new System.Drawing.Size(16, 16);
			this.tbMain.DropDownArrows = true;
			this.tbMain.ImageList = this.imgIcons;
			this.tbMain.Location = new System.Drawing.Point(0, 0);
			this.tbMain.Name = "tbMain";
			this.tbMain.ShowToolTips = true;
			this.tbMain.Size = new System.Drawing.Size(794, 28);
			this.tbMain.TabIndex = 7;
			this.tbMain.Wrappable = false;
			this.tbMain.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.tbMain_ButtonClick);
			// 
			// tbbCursor
			// 
			this.tbbCursor.Pushed = true;
			this.tbbCursor.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbCursor.Text = "選択";
			// 
			// tbbCut
			// 
			this.tbbCut.ImageIndex = 0;
			this.tbbCut.Text = "現在のチップを切り取り";
			// 
			// tbbCopy
			// 
			this.tbbCopy.ImageIndex = 1;
			this.tbbCopy.Text = "現在のチップをコピー";
			// 
			// tbbPaste
			// 
			this.tbbPaste.ImageIndex = 2;
			this.tbbPaste.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbPaste.Text = "貼り付け";
			// 
			// tbbRemove
			// 
			this.tbbRemove.Text = "現在のチップを削除";
			// 
			// tbbSeparator1
			// 
			this.tbbSeparator1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbZoom
			// 
			this.tbbZoom.Text = "拡大";
			// 
			// tbbMooz
			// 
			this.tbbMooz.Text = "縮小";
			// 
			// tbbSeparator2
			// 
			this.tbbSeparator2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbChip
			// 
			this.tbbChip.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbChip.Text = "チップ";
			// 
			// tbbFrame
			// 
			this.tbbFrame.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbFrame.Text = "フレーム";
			// 
			// tbbWeight
			// 
			this.tbbWeight.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbWeight.Text = "ウェイト";
			// 
			// tbbCowl
			// 
			this.tbbCowl.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbCowl.Text = "カウル";
			// 
			// tbbSeparator3
			// 
			this.tbbSeparator3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// tbbRudder
			// 
			this.tbbRudder.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbRudder.Text = "ラダー";
			// 
			// tbbRudderF
			// 
			this.tbbRudderF.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbRudderF.Text = "ラダーフレーム";
			// 
			// tbbTrim
			// 
			this.tbbTrim.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbTrim.Text = "トリム";
			// 
			// tbbTrimF
			// 
			this.tbbTrimF.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbTrimF.Text = "トリムフレーム";
			// 
			// tbbWheel
			// 
			this.tbbWheel.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbWheel.Text = "ホイール";
			// 
			// tbbRLW
			// 
			this.tbbRLW.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbRLW.Text = "無反動ホイール";
			// 
			// tbbJet
			// 
			this.tbbJet.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbJet.Text = "ジェット";
			// 
			// tbbArm
			// 
			this.tbbArm.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
			this.tbbArm.Text = "アーム";
			// 
			// imgIcons
			// 
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// pictTarget
			// 
			this.pictTarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictTarget.Dock = System.Windows.Forms.DockStyle.Left;
			this.pictTarget.Location = new System.Drawing.Point(0, 43);
			this.pictTarget.Name = "pictTarget";
			this.pictTarget.Size = new System.Drawing.Size(512, 512);
			this.pictTarget.TabIndex = 9;
			this.pictTarget.TabStop = false;
			this.pictTarget.Click += new System.EventHandler(this.pictTarget_Click);
			this.pictTarget.Paint += new System.Windows.Forms.PaintEventHandler(this.pictTarget_Paint);
			this.pictTarget.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseUp);
			this.pictTarget.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseMove);
			this.pictTarget.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictTarget_MouseDown);
			// 
			// panelB
			// 
			this.panelB.Controls.Add(this.txtDebug);
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
			// 
			// txtDebug
			// 
			this.txtDebug.AcceptsReturn = true;
			this.txtDebug.Location = new System.Drawing.Point(16, 432);
			this.txtDebug.Multiline = true;
			this.txtDebug.Name = "txtDebug";
			this.txtDebug.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtDebug.Size = new System.Drawing.Size(256, 72);
			this.txtDebug.TabIndex = 13;
			this.txtDebug.Text = "textBox1";
			// 
			// btnKey
			// 
			this.btnKey.Location = new System.Drawing.Point(152, 392);
			this.btnKey.Name = "btnKey";
			this.btnKey.Size = new System.Drawing.Size(104, 32);
			this.btnKey.TabIndex = 12;
			this.btnKey.Text = "Key{...} 編集";
			// 
			// btnVal
			// 
			this.btnVal.Location = new System.Drawing.Point(32, 392);
			this.btnVal.Name = "btnVal";
			this.btnVal.Size = new System.Drawing.Size(104, 32);
			this.btnVal.TabIndex = 11;
			this.btnVal.Text = "Val{...} 編集";
			// 
			// panelAttr
			// 
			this.panelAttr.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panelAttr.Controls.Add(this.panelAttrValue);
			this.panelAttr.Controls.Add(this.splAttr);
			this.panelAttr.Controls.Add(this.panelAttrName);
			this.panelAttr.Location = new System.Drawing.Point(0, 184);
			this.panelAttr.Name = "panelAttr";
			this.panelAttr.Size = new System.Drawing.Size(280, 200);
			this.panelAttr.TabIndex = 10;
			// 
			// panelAttrValue
			// 
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
			// cmbAttrItem7
			// 
			this.cmbAttrItem7.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem7.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem7.Location = new System.Drawing.Point(0, 136);
			this.cmbAttrItem7.Name = "cmbAttrItem7";
			this.cmbAttrItem7.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem7.TabIndex = 8;
			this.cmbAttrItem7.Text = "cmbAttrItem7";
			this.cmbAttrItem7.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAttrItems_KeyDown);
			// 
			// cmbAttrItem6
			// 
			this.cmbAttrItem6.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem6.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem6.Location = new System.Drawing.Point(0, 116);
			this.cmbAttrItem6.Name = "cmbAttrItem6";
			this.cmbAttrItem6.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem6.TabIndex = 7;
			this.cmbAttrItem6.Text = "cmbAttrItem6";
			this.cmbAttrItem6.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAttrItems_KeyDown);
			// 
			// cmbAttrItem5
			// 
			this.cmbAttrItem5.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem5.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem5.Location = new System.Drawing.Point(0, 96);
			this.cmbAttrItem5.Name = "cmbAttrItem5";
			this.cmbAttrItem5.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem5.TabIndex = 6;
			this.cmbAttrItem5.Text = "cmbAttrItem5";
			this.cmbAttrItem5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAttrItems_KeyDown);
			// 
			// cmbAttrItem4
			// 
			this.cmbAttrItem4.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem4.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem4.Location = new System.Drawing.Point(0, 76);
			this.cmbAttrItem4.Name = "cmbAttrItem4";
			this.cmbAttrItem4.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem4.TabIndex = 5;
			this.cmbAttrItem4.Text = "cmbAttrItem4";
			this.cmbAttrItem4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAttrItems_KeyDown);
			// 
			// cmbAttrItem3
			// 
			this.cmbAttrItem3.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem3.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem3.Location = new System.Drawing.Point(0, 56);
			this.cmbAttrItem3.Name = "cmbAttrItem3";
			this.cmbAttrItem3.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem3.TabIndex = 4;
			this.cmbAttrItem3.Text = "cmbAttrItem3";
			this.cmbAttrItem3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAttrItems_KeyDown);
			// 
			// cmbAttrItem2
			// 
			this.cmbAttrItem2.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem2.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem2.Location = new System.Drawing.Point(0, 36);
			this.cmbAttrItem2.Name = "cmbAttrItem2";
			this.cmbAttrItem2.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem2.TabIndex = 3;
			this.cmbAttrItem2.Text = "cmbAttrItem2";
			this.cmbAttrItem2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAttrItems_KeyDown);
			// 
			// cmbAttrItem1
			// 
			this.cmbAttrItem1.Dock = System.Windows.Forms.DockStyle.Top;
			this.cmbAttrItem1.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.cmbAttrItem1.Location = new System.Drawing.Point(0, 16);
			this.cmbAttrItem1.Name = "cmbAttrItem1";
			this.cmbAttrItem1.Size = new System.Drawing.Size(168, 20);
			this.cmbAttrItem1.TabIndex = 2;
			this.cmbAttrItem1.Text = "cmbAttrItem1";
			this.cmbAttrItem1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAttrItems_KeyDown);
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
			// labelAttrItem7
			// 
			this.labelAttrItem7.BackColor = System.Drawing.SystemColors.Window;
			this.labelAttrItem7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelAttrItem7.Dock = System.Windows.Forms.DockStyle.Top;
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
			this.txtName.Location = new System.Drawing.Point(192, 32);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(80, 19);
			this.txtName.TabIndex = 9;
			this.txtName.Text = "";
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(192, 16);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(48, 16);
			this.labelName.TabIndex = 8;
			this.labelName.Text = "識別名";
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(8, 16);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(24, 16);
			this.labelColor.TabIndex = 6;
			this.labelColor.Text = "色";
			// 
			// cmbColor
			// 
			this.cmbColor.ItemHeight = 12;
			this.cmbColor.Location = new System.Drawing.Point(8, 32);
			this.cmbColor.Name = "cmbColor";
			this.cmbColor.Size = new System.Drawing.Size(80, 20);
			this.cmbColor.TabIndex = 5;
			// 
			// lstSouth
			// 
			this.lstSouth.IntegralHeight = false;
			this.lstSouth.ItemHeight = 12;
			this.lstSouth.Location = new System.Drawing.Point(88, 120);
			this.lstSouth.Name = "lstSouth";
			this.lstSouth.Size = new System.Drawing.Size(104, 56);
			this.lstSouth.TabIndex = 4;
			this.lstSouth.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstSouth.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// lstNorth
			// 
			this.lstNorth.IntegralHeight = false;
			this.lstNorth.ItemHeight = 12;
			this.lstNorth.Location = new System.Drawing.Point(88, 8);
			this.lstNorth.Name = "lstNorth";
			this.lstNorth.Size = new System.Drawing.Size(104, 56);
			this.lstNorth.TabIndex = 3;
			this.lstNorth.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstNorth.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// lstEast
			// 
			this.lstEast.IntegralHeight = false;
			this.lstEast.ItemHeight = 12;
			this.lstEast.Location = new System.Drawing.Point(168, 64);
			this.lstEast.Name = "lstEast";
			this.lstEast.Size = new System.Drawing.Size(104, 56);
			this.lstEast.TabIndex = 2;
			this.lstEast.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstEast.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// lstWest
			// 
			this.lstWest.IntegralHeight = false;
			this.lstWest.ItemHeight = 12;
			this.lstWest.Location = new System.Drawing.Point(8, 64);
			this.lstWest.Name = "lstWest";
			this.lstWest.Size = new System.Drawing.Size(104, 56);
			this.lstWest.TabIndex = 1;
			this.lstWest.DoubleClick += new System.EventHandler(this.lstChild_DoubleClick);
			this.lstWest.SelectedIndexChanged += new System.EventHandler(this.lstChild_SelectedIndexChanged);
			// 
			// buttonSelChip
			// 
			this.buttonSelChip.Location = new System.Drawing.Point(112, 64);
			this.buttonSelChip.Name = "buttonSelChip";
			this.buttonSelChip.Size = new System.Drawing.Size(56, 56);
			this.buttonSelChip.TabIndex = 0;
			// 
			// tmr
			// 
			this.tmr.Enabled = true;
			this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
			// 
			// tmrScroll
			// 
			this.tmrScroll.Interval = 50;
			this.tmrScroll.Tick += new System.EventHandler(this.tmrScroll_Tick);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(794, 555);
			this.Controls.Add(this.panelB);
			this.Controls.Add(this.pictTarget);
			this.Controls.Add(this.labelTip);
			this.Controls.Add(this.tbMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Menu = this.mnuMain;
			this.MinimizeBox = false;
			this.Name = "frmMain";
			this.Text = "frmMain";
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.Click += new System.EventHandler(this.frmMain_Click);
			this.Load += new System.EventHandler(this.frmMain_Load);
			this.panelB.ResumeLayout(false);
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
		static void Main() {
			try{
				Application.Run(new frmMain());
			}catch(Exception e){
				MessageBox.Show(e.Message);
			}
		}

		private void frmMain_Load(object sender, System.EventArgs e) {
			draging.Draging = false;
			selected = tbbCursor;
			InitializeGraphics();
			Initialized = true;
			UpdateCameraPosition(0,0,0,Matrix.Identity);
		}
		
		private void frmMain_Click(object sender, System.EventArgs e) {
			pictTarget_Paint(this,null);
		}

		private void pictTarget_Paint(object sender, PaintEventArgs e) {
			if(!Initialized || Pause || device == null){
				labelTip.Text = "painting is stopped.";
				return;
			}

			device.Clear(ClearFlags.Target | ClearFlags.ZBuffer ,Color.Navy ,100.0f,0);
			device.BeginScene();

			device.RenderState.AlphaBlendEnable = false;
			device.VertexFormat = CustomVertex.PositionColored.Format;
			device.SetStreamSource(0,vbGuide,0);
			device.Transform.World = Matrix.Identity;
			device.Material = new Material();
			device.DrawPrimitives(PrimitiveType.LineList ,0,3);
			device.RenderState.AlphaBlendEnable = true;

			rcdata.model.root.DrawChipAll();
			rcdata.DrawCursor();

			//			rcx.Draw(device,Color.FromArgb(40,255,0,0) ,Matrix.Identity);

			device.EndScene();
			device.Present();

		}

		private void pictTarget_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Right){
				draging.Draging = true;
				draging.StartX = draging.PrevX = e.X;
				draging.StartY = draging.PrevY = e.Y;
				this.Text+= "draging";
				LeastIsLeftButton = false;
			}
			else if(e.Button == MouseButtons.Left)
				LeastIsLeftButton = true;
			CamDepth += e.Delta * 0.5f;

		}

		private void pictTarget_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button != MouseButtons.Right)return;
			draging.Draging = false;
			this.Text = this.Text.Replace("draging","");
		}

		private void pictTarget_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			MouseX = e.X;
			MouseY = e.Y;
			if(draging.Draging){
				UpdateCameraPosition(e.X - draging.PrevX,draging.PrevY - e.Y,e.Delta,rcdata.Cursor.Parent.Matrix);
								
				draging.PrevX = e.X;
				draging.PrevY = e.Y;
			}
		}

		private void tmr_Tick(object sender, System.EventArgs e) {
			pictTarget_Paint(sender,null);

			RcChipBase c;
			txtDebug.Clear();
			for(int i = 0;i < 10;i++){
				if((c = rcdata.GetChipFromLib(i)) != null){
					txtDebug.Text += c.ToString() + "\n";
				}
				else{
					txtDebug.Text += "null\n";
				}
			}
		}

		private void labelTip_Click(object sender, System.EventArgs e) {
			//	テスト用に、RcModelのテキスト出力を行う
			string output = rcdata.model.root.ToString(0,0);
			StreamWriter tw = new StreamWriter("out.rcd_");
			MessageBox.Show(output);
			tw.Write(output);
			tw.Flush();
			tw.Close();
		}

		private void pictTarget_Click(object sender, System.EventArgs e) {
			if(!LeastIsLeftButton)return;
			RcHitStatus h,i;
			labelTip.Text = MouseX + "," + MouseY;
			h = rcdata.Cursor.IsHit(MouseX,MouseY,pictTarget.Width,pictTarget.Width);
			i = rcdata.model.root.IsHit(MouseX,MouseY,pictTarget.Width,pictTarget.Height);
			

			//	↓この辺の動作をmodeによって分けるようにすること

/*			if(h.distance <= i.distance && h.HitChip != null){
				if( h.HitChip == rcdata.Cursor){
					//	カーソルが選択された
					if(rcdata.Cursor.Parent != rcdata.model.root){
						foreach(RcChipBase cb in rcdata.Cursor.Parent.Child){
							if(cb != null)
								if(MessageBox.Show("派生チップもろともごっそり削除しますがよろしいですか？","削除確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.Cancel)
									return;
								else
									break;
						}
						RcChipBase buff = rcdata.Cursor.Parent.Parent;
						try{rcdata.Cursor.Parent.Detach();}catch{};
						rcdata.SetCursor(buff);
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						
					}
				}
				else{
					//	ガイドが選択された
					try{
						rcdata.SetCursor(new RcChipTrim(rcdata,h.HitChip.Parent.Parent,h.HitChip.JointPosition));
					}
					catch(Exception err){
						MessageBox.Show(err.Message,"追加エラー");
						return;
					}
					StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
				}
				labelTip.Text += " : Added";
				LoadChipInfo();
			}
			else if(i.HitChip != null){
				//	チップが選択された
				labelTip.Text += " : Moved";
				if(i.HitChip != rcdata.Cursor.Parent){
					rcdata.SetCursor(i.HitChip);
					StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
				}
				LoadChipInfo();
			}
*/
			switch(selected.Text){
				case "選択":
					if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "貼り付け":
					MessageBox.Show("貼り付け機能はまだ実装されていません");
					break;
				case "チップ":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipChip(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "フレーム":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipFrame(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "ウェイト":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipWeight(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "カウル":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipCowl(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "ラダー":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipRudder(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "ラダーフレーム":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipRudderF(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "トリム":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipTrim(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "トリムフレーム":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipTrimF(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "ホイール":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipWheel(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "無反動ホイール":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipRLW(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "ジェット":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipJet(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				case "アーム":
					if(h.HitChip != null && h.HitChip != rcdata.Cursor){
						try{
							rcdata.SetCursor(new RcChipArm(rcdata,rcdata.Cursor.Parent,h.HitChip.JointPosition));
						}
						catch(Exception err){
							MessageBox.Show(err.Message,"追加エラー");
							return;
						}
						StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
					}
					else if(i.HitChip != null){
						if(i.HitChip != rcdata.Cursor.Parent){
							rcdata.SetCursor(i.HitChip);
							StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
						}
					}
					LoadChipInfo();
					break;
				default:
					break;
			}

//			UpdateCameraPosition(0,0,0,rcdata.Cursor.Parent.Matrix);
			pictTarget_Paint(this,null);
		}

		private void tbMain_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e) {
			if(e.Button == tbbCut){
				//	カット動作
			}
			else if(e.Button == tbbCopy){
				//	コピー動作
			}
			else if(e.Button == tbbRemove){
				if(rcdata.Cursor.Parent is RcChipCore){
					labelTip.Text = "コアは削除できません。";
					return;
				}
				foreach(RcChipBase cb in rcdata.Cursor.Parent.Child){
					if(cb != null)
						if(MessageBox.Show("派生チップもろともごっそり削除しますがよろしいですか？","削除確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.Cancel)
							return;
						else
							break;
				}
				RcChipBase buff = rcdata.Cursor.Parent.Parent;
				try{rcdata.Cursor.Parent.Detach();}catch{};
				rcdata.SetCursor(buff);
				StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
			}
			else if(e.Button == tbbZoom){
				UpdateCameraPosition(0,0,5,rcdata.Cursor.Parent.Matrix);
			}
			else if(e.Button == tbbMooz){
				UpdateCameraPosition(0,0,-5,rcdata.Cursor.Parent.Matrix);
			}
			else{
				//	各種モード変更
				foreach(ToolBarButton tbb in tbMain.Buttons)
					tbb.Pushed = false;
				e.Button.Pushed = true;
				selected = e.Button;
			}
		}

		private void cmbAttrItems_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter){
				ApplyChipInfo();
				e.Handled = true;
			}
		}

		private bool InitializeGraphics() {
			try {
				presentParams.Windowed = true;
//				presentParams.BackBufferCount = 1;
//				presentParams.BackBufferHeight = presentParams.BackBufferWidth = 0;
				presentParams.SwapEffect = SwapEffect.Discard;
				presentParams.EnableAutoDepthStencil = true;
				presentParams.AutoDepthStencilFormat = DepthFormat.D16;
				
				device = new Device(
					0,
					DeviceType.Hardware,
					this.pictTarget,
					CreateFlags.SoftwareVertexProcessing,
					presentParams);




				vbGuide = new VertexBuffer(
					typeof(CustomVertex.PositionColored),
					6,
					device,
					0,
					CustomVertex.PositionColored.Format,
					Pool.Default);


				GraphicsStream gs;
				gs = vbGuide.Lock(0,0,0);
				guidecv = new Microsoft.DirectX.Direct3D.CustomVertex.PositionColored[6];

				guidecv[0].SetPosition(new Vector3(0f,0f,0f));
				guidecv[0].Color = 0xFF0000;
				guidecv[1].SetPosition(new Vector3(-100f,0f,0f));
				guidecv[1].Color = 0xFF0000;
				guidecv[2].SetPosition(new Vector3(0f,0f,0f));
				guidecv[2].Color = 0x00FF00;
				guidecv[3].SetPosition(new Vector3(0f,100f,0f));
				guidecv[3].Color = 0x00FF00;
				guidecv[4].SetPosition(new Vector3(0f,0f,0f));
				guidecv[4].Color = 0x0000FF;
				guidecv[5].SetPosition(new Vector3(0f,0f,-100f));
				guidecv[5].Color = 0x0000FF;

				gs.Write(guidecv);

				vbGuide.Unlock();

				device.RenderState.ZBufferEnable = true;
				device.Transform.World = Matrix.Identity;
				device.Transform.View = Matrix.LookAtLH( 
					new Vector3( 0.0f, 0.0f, 0.0f ),
					new Vector3( 0.0f, 0.0f, 0.0f ),
					new Vector3( 0.0f, 1.0f, 0.0f ) );
//				device.Transform.Projection = Matrix.PerspectiveFovLH(90/180*(float)Math.PI ,1.0f,0.5f,100.0f);
				device.Transform.Projection = Matrix.PerspectiveFovLH(0.5f ,1.0f,0.5f,100.0f);

				device.Lights[0].Ambient = Color.White;
				device.Lights[0].Diffuse = Color.White;
				device.Lights[0].Specular = Color.White;
				device.Lights[0].Type = LightType.Directional;
				device.Lights[0].Direction = new Vector3(1f,1f,1f);
				device.Lights[0].Enabled = true;
				device.RenderState.Lighting = true;
//				device.RenderState.Ambient = Color.LightBlue;
				device.RenderState.CullMode = Cull.CounterClockwise;

				//				device.RenderState.AlphaBlendOperation = BlendOperation.Add;
				device.RenderState.SourceBlend = Blend.SourceAlpha;
				device.RenderState.DestinationBlend = Blend.InvSourceAlpha;
				device.RenderState.AlphaBlendEnable = true;

//				rcx = new RcXFile();
//				rcx.FileName = "cursor3.x";
//				rcx.Load(device);

				rcdata = new RcData(device);
/*				(new RcChipChip(rcdata, new RcChipChip(rcdata, new RcChipChip(rcdata,rcdata.model.root,RcJointPosition.North),RcJointPosition.North),RcJointPosition.North)).ChipColor = Color.Red;
				new RcChipFrame(rcdata, rcdata.model.root.Child[0].Child[0],RcJointPosition.East);
				(new RcChipJet(rcdata, rcdata.model.root.Child[0].Child[0],RcJointPosition.West))["Option"] = atr;
				rcdata.model.root.Child[0].Child[0]["Angle"] = atr;
				atr.Const = 45f;
				rcdata.model.root.Child[0].Child[0].Child[1]["Angle"] = atr;
				rcdata.model.root.Child[0].Child[0].Child[1]["Option"] = atr;
				new RcChipJet(rcdata,rcdata.model.root.Child[0].Child[0].Child[1],RcJointPosition.North);
				rcdata.model.root.Child[0].UpdateMatrix();
*/
				rcdata.SetCursor(rcdata.model.root);
				LoadChipInfo();

				device.DeviceLost += new EventHandler(Device_DeviceLost);

				return true;
			}
			catch(Direct3DXException e) {
				labelTip.Text = e.Message;
				return false; 
			}
		}

		private void Device_DeviceLost(object sender,EventArgs e){
			//	TODO:	ここにレジュームファイルを生成するコードを書く。
			MessageBox.Show("諸事情により、アプリケーションが終了します。\nご迷惑をおかけします。m(_ _;)m");
			Application.Exit();
		}
		/*	残骸
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
			guidecv = new Microsoft.DirectX.Direct3D.CustomVertex.PositionColored[6];

			guidecv[0].SetPosition(new Vector3(0f,0f,0f));
			guidecv[0].Color = 0xFF0000;
			guidecv[1].SetPosition(new Vector3(-100f,0f,0f));
			guidecv[1].Color = 0xFF0000;
			guidecv[2].SetPosition(new Vector3(0f,0f,0f));
			guidecv[2].Color = 0x00FF00;
			guidecv[3].SetPosition(new Vector3(0f,100f,0f));
			guidecv[3].Color = 0x00FF00;
			guidecv[4].SetPosition(new Vector3(0f,0f,0f));
			guidecv[4].Color = 0x0000FF;
			guidecv[5].SetPosition(new Vector3(0f,0f,-100f));
			guidecv[5].Color = 0x0000FF;

			gs.Write(guidecv);

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

		/// <summary>
		/// 現在選択されているチップの情報を右パネルに表示する。
		/// </summary>
		private void LoadChipInfo(){
			RcChipBase target = rcdata.Cursor.Parent;
			string[] s = target.GetAttrList();
			if(s == null)
				s = new string[0];
			for(int i = 0;i < labelAttrItems.Length;i++){
				if(s.Length > i){
					labelAttrItems[i].Text = s[i];
					cmbAttrItems[i].Text = target[s[i]].Value().ToString();	//	仮。のちのち変数にも対応させる。
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
					switch(cb.GetRealAngle()){
						case (byte)RcJointPosition.North:
							lstNorth.Items.Add(cb);
							break;
						case (byte)RcJointPosition.East:
							lstEast.Items.Add(cb);
							break;
						case (byte)RcJointPosition.South:
							lstSouth.Items.Add(cb);
							break;
						case (byte)RcJointPosition.West:
							lstWest.Items.Add(cb);
							break;
					}
				}
			}

			cmbColor.Text = "#";
			cmbColor.Text += target.ChipColor.R.ToString("X").PadLeft(2,'0');
			cmbColor.Text += target.ChipColor.G.ToString("X").PadLeft(2,'0');
			cmbColor.Text += target.ChipColor.B.ToString("X").PadLeft(2,'0');

			txtName.Text = target.Name;
		}

		/// <summary>
		/// 右パネルの情報を選択されているチップに適用する。
		/// </summary>
		private void ApplyChipInfo(){
			RcChipBase target = rcdata.Cursor.Parent;
			RcAttrValue val = new RcAttrValue();
			for(int i = 0;i < labelAttrItems.Length;i++){
				if(labelAttrItems[i].Visible){
					try{
						val.Const = float.Parse(cmbAttrItems[i].Text);
						target[labelAttrItems[i].Text] = val;
					}catch(Exception e){
						labelTip.Text = e.Message;
						cmbAttrItems[i].Text = target[labelAttrItems[i].Text].ToString();
					}
				}
				else
					break;
			}
			target.UpdateMatrix();
			rcdata.Cursor.UpdateMatrix();

			StartScrollCameraPosition (rcdata.Cursor.Parent.Matrix);
			pictTarget_Paint(this,null);

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
			ScrollCount = 20;
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

			pictTarget_Paint(this,null);
		}

		private void StartScrollCameraPosition(Matrix nextPointMatrix){
			CamNow = CamNext * (ScrollCount/20f) + CamNow * (1f - ScrollCount/20f);
			CamNext = new Vector3();
			CamNext.TransformCoordinate(nextPointMatrix);
			ScrollCount = 0;
			tmrScroll.Enabled = true;
		}

		private void tmrScroll_Tick(object sender, System.EventArgs e) {
			if(ScrollCount < 20){
				UpdateCameraPosition(0,0,0,CamNext * (ScrollCount/20f) + CamNow * (1f - ScrollCount/20f));
				ScrollCount++;
			}
			else{
				UpdateCameraPosition(0,0,0,rcdata.Cursor.Parent.Matrix);
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
				rcdata.SetCursor((RcChipBase)listtarget.SelectedItem);
				StartScrollCameraPosition(rcdata.Cursor.Parent.Matrix);
				LoadChipInfo();
			}
		}


	}

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
	}
}
