using RigidChips;
using System;
using System.Windows.Forms;

namespace rcm {
	/// <summary>
	/// �ݒ�_�C�A���O�B
	/// </summary>
	public class frmConfig : System.Windows.Forms.Form {
		RcDrawOptions optDraw;
		RcOutputOptions optOutput;
		RcEditOptions optEdit;

		RcChipBase chipSample;
		frmMain mainwindow;

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox chkShowWeight;
		private System.Windows.Forms.RadioButton rbAlpha;
		private System.Windows.Forms.CheckBox chkShowGhost;
		private System.Windows.Forms.TextBox txtSwellRate;
		private System.Windows.Forms.CheckBox chkBaloonSwell;
		private System.Windows.Forms.CheckBox chkShowCowl;
		private System.Windows.Forms.PictureBox clrWeight;
		private System.Windows.Forms.CheckBox chkShowWeightGuide;
		private System.Windows.Forms.TextBox txtWeightSize;
		private System.Windows.Forms.TextBox txtWeightAlpha;
		private System.Windows.Forms.PictureBox clrXp;
		private System.Windows.Forms.PictureBox clrXn;
		private System.Windows.Forms.PictureBox clrYp;
		private System.Windows.Forms.PictureBox clrYn;
		private System.Windows.Forms.PictureBox clrZp;
		private System.Windows.Forms.PictureBox clrZn;
		private System.Windows.Forms.CheckBox chkXAxis;
		private System.Windows.Forms.CheckBox chkYAxis;
		private System.Windows.Forms.CheckBox chkZAxis;
		private System.Windows.Forms.CheckBox chkXNegAxis;
		private System.Windows.Forms.CheckBox chkYNegAxis;
		private System.Windows.Forms.CheckBox chkZNegAxis;
		private System.Windows.Forms.PictureBox clrSouth;
		private System.Windows.Forms.PictureBox clrEast;
		private System.Windows.Forms.PictureBox clrWest;
		private System.Windows.Forms.PictureBox clrNorth;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ColorDialog dlgColor;
		private System.Windows.Forms.RadioButton rbFrame;
		private System.Windows.Forms.PictureBox clrBack;
		private System.Windows.Forms.PictureBox clrCursorF;
		private System.Windows.Forms.PictureBox clrCursorB;
		private System.Windows.Forms.CheckBox chkShowAlways;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox chkReturnEndChipBracket;
		private System.Windows.Forms.CheckBox chkOpenBracketWithChipDefinition;
		private System.Windows.Forms.CheckBox chkIndentEnable;
		private System.Windows.Forms.CheckBox chkIndentBySpace;
		private System.Windows.Forms.TextBox txtIndentSpaceNum;
		private System.Windows.Forms.CheckBox chkCommaWithSpace;
		private System.Windows.Forms.CheckBox chkPrintAllAttribute;
		private System.Windows.Forms.TextBox txtOutputSample;
		private System.Windows.Forms.CheckBox chkCameraOrtho;
		private System.Windows.Forms.CheckBox chkAttrCopy;
		private System.Windows.Forms.CheckBox chkUnbisibleUnselectable;
		private System.Windows.Forms.TrackBar trkScrollFrame;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.PictureBox clrWeightGuide;
		private System.Windows.Forms.Label lblScrollValue;
		private System.Windows.Forms.CheckBox chkAttrAutoApply;
		private System.Windows.Forms.RadioButton rbGChip;
		private System.Windows.Forms.ToolTip ttDescription;
		private System.Windows.Forms.Label label13;
		private System.ComponentModel.IContainer components;

		public frmConfig(frmMain mainform) {
			//
			// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent �Ăяo���̌�ɁA�R���X�g���N�^ �R�[�h��ǉ����Ă��������B
			//
			mainwindow = mainform;
			optDraw = mainform.rcdata.DrawOption;
			optOutput = mainform.rcdata.OutputOption;
			optEdit = mainform.rcdata.EditOption;

			RcChipBase buff;
			RcAttrValue attr = new RcAttrValue();
			attr.Const = 120f;
			chipSample = new RcChipChip(mainform.rcdata, null, RcJointPosition.NULL);
			buff = new RcChipChip(mainform.rcdata, chipSample, RcJointPosition.North);
			buff["Angle"] = attr;
			attr.Const = 4000f;
			buff["User1"] = attr;
			new RcChipFrame(mainform.rcdata, buff, RcJointPosition.East);
			attr.Const = 0.2f;
			new RcChipFrame(mainform.rcdata, buff, RcJointPosition.West)["Damper"] = attr;
			new RcChipTrim(mainform.rcdata, chipSample, RcJointPosition.North).Comment = "����̓R�����g�ł��B";
			chipSample.Name = "Root";

			mainform.rcdata.CheckBackTrack();
		}

		public int NowTabPage {
			get {
				return tabControl1.SelectedIndex;
			}
			set {
				tabControl1.SelectedIndex = value;
			}
		}

		/// <summary>
		/// �g�p����Ă��郊�\�[�X�Ɍ㏈�������s���܂��B
		/// </summary>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows �t�H�[�� �f�U�C�i�Ő������ꂽ�R�[�h 
		/// <summary>
		/// �f�U�C�i �T�|�[�g�ɕK�v�ȃ��\�b�h�ł��B���̃��\�b�h�̓��e��
		/// �R�[�h �G�f�B�^�ŕύX���Ȃ��ł��������B
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.chkShowAlways = new System.Windows.Forms.CheckBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.clrNorth = new System.Windows.Forms.PictureBox();
			this.clrWest = new System.Windows.Forms.PictureBox();
			this.clrEast = new System.Windows.Forms.PictureBox();
			this.clrSouth = new System.Windows.Forms.PictureBox();
			this.clrCursorB = new System.Windows.Forms.PictureBox();
			this.clrCursorF = new System.Windows.Forms.PictureBox();
			this.clrBack = new System.Windows.Forms.PictureBox();
			this.chkCameraOrtho = new System.Windows.Forms.CheckBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.chkZNegAxis = new System.Windows.Forms.CheckBox();
			this.chkYNegAxis = new System.Windows.Forms.CheckBox();
			this.chkXNegAxis = new System.Windows.Forms.CheckBox();
			this.chkZAxis = new System.Windows.Forms.CheckBox();
			this.chkYAxis = new System.Windows.Forms.CheckBox();
			this.chkXAxis = new System.Windows.Forms.CheckBox();
			this.clrZn = new System.Windows.Forms.PictureBox();
			this.clrZp = new System.Windows.Forms.PictureBox();
			this.clrYn = new System.Windows.Forms.PictureBox();
			this.clrYp = new System.Windows.Forms.PictureBox();
			this.clrXn = new System.Windows.Forms.PictureBox();
			this.clrXp = new System.Windows.Forms.PictureBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.clrWeightGuide = new System.Windows.Forms.PictureBox();
			this.txtWeightAlpha = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtWeightSize = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chkShowWeightGuide = new System.Windows.Forms.CheckBox();
			this.clrWeight = new System.Windows.Forms.PictureBox();
			this.chkShowWeight = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rbGChip = new System.Windows.Forms.RadioButton();
			this.rbFrame = new System.Windows.Forms.RadioButton();
			this.rbAlpha = new System.Windows.Forms.RadioButton();
			this.chkShowGhost = new System.Windows.Forms.CheckBox();
			this.txtSwellRate = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chkBaloonSwell = new System.Windows.Forms.CheckBox();
			this.chkShowCowl = new System.Windows.Forms.CheckBox();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label13 = new System.Windows.Forms.Label();
			this.chkPrintAllAttribute = new System.Windows.Forms.CheckBox();
			this.chkCommaWithSpace = new System.Windows.Forms.CheckBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtIndentSpaceNum = new System.Windows.Forms.TextBox();
			this.chkIndentBySpace = new System.Windows.Forms.CheckBox();
			this.chkIndentEnable = new System.Windows.Forms.CheckBox();
			this.chkOpenBracketWithChipDefinition = new System.Windows.Forms.CheckBox();
			this.chkReturnEndChipBracket = new System.Windows.Forms.CheckBox();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.txtOutputSample = new System.Windows.Forms.TextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.chkAttrAutoApply = new System.Windows.Forms.CheckBox();
			this.lblScrollValue = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.trkScrollFrame = new System.Windows.Forms.TrackBar();
			this.chkUnbisibleUnselectable = new System.Windows.Forms.CheckBox();
			this.chkAttrCopy = new System.Windows.Forms.CheckBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnApply = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.dlgColor = new System.Windows.Forms.ColorDialog();
			this.ttDescription = new System.Windows.Forms.ToolTip(this.components);
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.groupBox4.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrNorth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWest)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrEast)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrSouth)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorB)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorF)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrBack)).BeginInit();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrZn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrZp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXn)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXp)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrWeightGuide)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWeight)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.tabPage3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkScrollFrame)).BeginInit();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(496, 272);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.groupBox4);
			this.tabPage1.Controls.Add(this.groupBox3);
			this.tabPage1.Controls.Add(this.groupBox2);
			this.tabPage1.Controls.Add(this.groupBox1);
			this.tabPage1.Location = new System.Drawing.Point(4, 21);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(488, 247);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "�\��";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.chkShowAlways);
			this.groupBox4.Controls.Add(this.label10);
			this.groupBox4.Controls.Add(this.label9);
			this.groupBox4.Controls.Add(this.label8);
			this.groupBox4.Controls.Add(this.label7);
			this.groupBox4.Controls.Add(this.label6);
			this.groupBox4.Controls.Add(this.label5);
			this.groupBox4.Controls.Add(this.label4);
			this.groupBox4.Controls.Add(this.clrNorth);
			this.groupBox4.Controls.Add(this.clrWest);
			this.groupBox4.Controls.Add(this.clrEast);
			this.groupBox4.Controls.Add(this.clrSouth);
			this.groupBox4.Controls.Add(this.clrCursorB);
			this.groupBox4.Controls.Add(this.clrCursorF);
			this.groupBox4.Controls.Add(this.clrBack);
			this.groupBox4.Controls.Add(this.chkCameraOrtho);
			this.groupBox4.Location = new System.Drawing.Point(248, 120);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(232, 120);
			this.groupBox4.TabIndex = 11;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "��";
			// 
			// chkShowAlways
			// 
			this.chkShowAlways.Location = new System.Drawing.Point(8, 96);
			this.chkShowAlways.Name = "chkShowAlways";
			this.chkShowAlways.Size = new System.Drawing.Size(128, 16);
			this.chkShowAlways.TabIndex = 34;
			this.chkShowAlways.Text = "�K�C�h���͏�ɕ\��";
			this.ttDescription.SetToolTip(this.chkShowAlways, "���_�ύX�����K�C�h�ނ�\������悤�ɂ��܂��B");
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(184, 24);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(16, 16);
			this.label10.TabIndex = 33;
			this.label10.Text = "��";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(96, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(56, 16);
			this.label9.TabIndex = 32;
			this.label9.Text = "�J�[�\���\";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(168, 56);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(32, 16);
			this.label8.TabIndex = 31;
			this.label8.Text = "�w�i";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(56, 72);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(16, 16);
			this.label7.TabIndex = 30;
			this.label7.Text = "��";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(80, 40);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(16, 16);
			this.label6.TabIndex = 29;
			this.label6.Text = "��";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 64);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(16, 16);
			this.label5.TabIndex = 28;
			this.label5.Text = "��";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(56, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(16, 16);
			this.label4.TabIndex = 27;
			this.label4.Text = "�k";
			// 
			// clrNorth
			// 
			this.clrNorth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrNorth.Location = new System.Drawing.Point(32, 16);
			this.clrNorth.Name = "clrNorth";
			this.clrNorth.Size = new System.Drawing.Size(24, 24);
			this.clrNorth.TabIndex = 25;
			this.clrNorth.TabStop = false;
			this.ttDescription.SetToolTip(this.clrNorth, "�F��ݒ肵�܂��B");
			this.clrNorth.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrWest
			// 
			this.clrWest.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrWest.Location = new System.Drawing.Point(8, 40);
			this.clrWest.Name = "clrWest";
			this.clrWest.Size = new System.Drawing.Size(24, 24);
			this.clrWest.TabIndex = 23;
			this.clrWest.TabStop = false;
			this.ttDescription.SetToolTip(this.clrWest, "�F��ݒ肵�܂��B");
			this.clrWest.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrEast
			// 
			this.clrEast.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrEast.Location = new System.Drawing.Point(56, 40);
			this.clrEast.Name = "clrEast";
			this.clrEast.Size = new System.Drawing.Size(24, 24);
			this.clrEast.TabIndex = 21;
			this.clrEast.TabStop = false;
			this.ttDescription.SetToolTip(this.clrEast, "�F��ݒ肵�܂��B");
			this.clrEast.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrSouth
			// 
			this.clrSouth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrSouth.Location = new System.Drawing.Point(32, 64);
			this.clrSouth.Name = "clrSouth";
			this.clrSouth.Size = new System.Drawing.Size(24, 24);
			this.clrSouth.TabIndex = 19;
			this.clrSouth.TabStop = false;
			this.ttDescription.SetToolTip(this.clrSouth, "�F��ݒ肵�܂��B");
			this.clrSouth.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrCursorB
			// 
			this.clrCursorB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrCursorB.Location = new System.Drawing.Point(200, 16);
			this.clrCursorB.Name = "clrCursorB";
			this.clrCursorB.Size = new System.Drawing.Size(24, 24);
			this.clrCursorB.TabIndex = 17;
			this.clrCursorB.TabStop = false;
			this.ttDescription.SetToolTip(this.clrCursorB, "�F��ݒ肵�܂��B");
			this.clrCursorB.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrCursorF
			// 
			this.clrCursorF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrCursorF.Location = new System.Drawing.Point(152, 16);
			this.clrCursorF.Name = "clrCursorF";
			this.clrCursorF.Size = new System.Drawing.Size(24, 24);
			this.clrCursorF.TabIndex = 15;
			this.clrCursorF.TabStop = false;
			this.ttDescription.SetToolTip(this.clrCursorF, "�F��ݒ肵�܂��B");
			this.clrCursorF.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrBack
			// 
			this.clrBack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrBack.Location = new System.Drawing.Point(200, 48);
			this.clrBack.Name = "clrBack";
			this.clrBack.Size = new System.Drawing.Size(24, 24);
			this.clrBack.TabIndex = 13;
			this.clrBack.TabStop = false;
			this.ttDescription.SetToolTip(this.clrBack, "�F��ݒ肵�܂��B");
			this.clrBack.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// chkCameraOrtho
			// 
			this.chkCameraOrtho.Location = new System.Drawing.Point(144, 96);
			this.chkCameraOrtho.Name = "chkCameraOrtho";
			this.chkCameraOrtho.Size = new System.Drawing.Size(80, 16);
			this.chkCameraOrtho.TabIndex = 0;
			this.chkCameraOrtho.Text = "���s���e";
			this.ttDescription.SetToolTip(this.chkCameraOrtho, "���s���̂Ȃ��\�����@��p���܂��B");
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.chkZNegAxis);
			this.groupBox3.Controls.Add(this.chkYNegAxis);
			this.groupBox3.Controls.Add(this.chkXNegAxis);
			this.groupBox3.Controls.Add(this.chkZAxis);
			this.groupBox3.Controls.Add(this.chkYAxis);
			this.groupBox3.Controls.Add(this.chkXAxis);
			this.groupBox3.Controls.Add(this.clrZn);
			this.groupBox3.Controls.Add(this.clrZp);
			this.groupBox3.Controls.Add(this.clrYn);
			this.groupBox3.Controls.Add(this.clrYp);
			this.groupBox3.Controls.Add(this.clrXn);
			this.groupBox3.Controls.Add(this.clrXp);
			this.groupBox3.Location = new System.Drawing.Point(248, 8);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(232, 112);
			this.groupBox3.TabIndex = 10;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "��";
			// 
			// chkZNegAxis
			// 
			this.chkZNegAxis.Location = new System.Drawing.Point(120, 80);
			this.chkZNegAxis.Name = "chkZNegAxis";
			this.chkZNegAxis.Size = new System.Drawing.Size(64, 24);
			this.chkZNegAxis.TabIndex = 27;
			this.chkZNegAxis.Text = "Z���W-";
			this.ttDescription.SetToolTip(this.chkZNegAxis, "Z���W������(��O)�̃��C����\�����܂��B");
			// 
			// chkYNegAxis
			// 
			this.chkYNegAxis.Location = new System.Drawing.Point(120, 48);
			this.chkYNegAxis.Name = "chkYNegAxis";
			this.chkYNegAxis.Size = new System.Drawing.Size(64, 24);
			this.chkYNegAxis.TabIndex = 26;
			this.chkYNegAxis.Text = "Y���W-";
			this.ttDescription.SetToolTip(this.chkYNegAxis, "Y���W������(��)�̃��C����\�����܂��B");
			// 
			// chkXNegAxis
			// 
			this.chkXNegAxis.Location = new System.Drawing.Point(120, 16);
			this.chkXNegAxis.Name = "chkXNegAxis";
			this.chkXNegAxis.Size = new System.Drawing.Size(64, 24);
			this.chkXNegAxis.TabIndex = 25;
			this.chkXNegAxis.Text = "X���W-";
			this.ttDescription.SetToolTip(this.chkXNegAxis, "X���W������(��)�̃��C����\�����܂��B");
			// 
			// chkZAxis
			// 
			this.chkZAxis.Location = new System.Drawing.Point(8, 80);
			this.chkZAxis.Name = "chkZAxis";
			this.chkZAxis.Size = new System.Drawing.Size(64, 24);
			this.chkZAxis.TabIndex = 24;
			this.chkZAxis.Text = "Z���W+";
			this.ttDescription.SetToolTip(this.chkZAxis, "Z���W������(��)�̃��C����\�����܂��B");
			// 
			// chkYAxis
			// 
			this.chkYAxis.Location = new System.Drawing.Point(8, 48);
			this.chkYAxis.Name = "chkYAxis";
			this.chkYAxis.Size = new System.Drawing.Size(64, 24);
			this.chkYAxis.TabIndex = 23;
			this.chkYAxis.Text = "Y���W+";
			this.ttDescription.SetToolTip(this.chkYAxis, "Y���W������(��)�̃��C����\�����܂��B");
			// 
			// chkXAxis
			// 
			this.chkXAxis.Location = new System.Drawing.Point(8, 16);
			this.chkXAxis.Name = "chkXAxis";
			this.chkXAxis.Size = new System.Drawing.Size(64, 24);
			this.chkXAxis.TabIndex = 22;
			this.chkXAxis.Text = "X���W+";
			this.ttDescription.SetToolTip(this.chkXAxis, "X���W������(�E)�̃��C����\�����܂��B");
			// 
			// clrZn
			// 
			this.clrZn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrZn.Location = new System.Drawing.Point(200, 80);
			this.clrZn.Name = "clrZn";
			this.clrZn.Size = new System.Drawing.Size(24, 24);
			this.clrZn.TabIndex = 21;
			this.clrZn.TabStop = false;
			this.ttDescription.SetToolTip(this.clrZn, "�F��ݒ肵�܂��B");
			this.clrZn.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrZp
			// 
			this.clrZp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrZp.Location = new System.Drawing.Point(88, 80);
			this.clrZp.Name = "clrZp";
			this.clrZp.Size = new System.Drawing.Size(24, 24);
			this.clrZp.TabIndex = 19;
			this.clrZp.TabStop = false;
			this.ttDescription.SetToolTip(this.clrZp, "�F��ݒ肵�܂��B");
			this.clrZp.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrYn
			// 
			this.clrYn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrYn.Location = new System.Drawing.Point(200, 48);
			this.clrYn.Name = "clrYn";
			this.clrYn.Size = new System.Drawing.Size(24, 24);
			this.clrYn.TabIndex = 17;
			this.clrYn.TabStop = false;
			this.ttDescription.SetToolTip(this.clrYn, "�F��ݒ肵�܂��B");
			this.clrYn.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrYp
			// 
			this.clrYp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrYp.Location = new System.Drawing.Point(88, 48);
			this.clrYp.Name = "clrYp";
			this.clrYp.Size = new System.Drawing.Size(24, 24);
			this.clrYp.TabIndex = 15;
			this.clrYp.TabStop = false;
			this.ttDescription.SetToolTip(this.clrYp, "�F��ݒ肵�܂��B");
			this.clrYp.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrXn
			// 
			this.clrXn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrXn.Location = new System.Drawing.Point(200, 16);
			this.clrXn.Name = "clrXn";
			this.clrXn.Size = new System.Drawing.Size(24, 24);
			this.clrXn.TabIndex = 13;
			this.clrXn.TabStop = false;
			this.ttDescription.SetToolTip(this.clrXn, "�F��ݒ肵�܂��B");
			this.clrXn.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// clrXp
			// 
			this.clrXp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrXp.Location = new System.Drawing.Point(88, 16);
			this.clrXp.Name = "clrXp";
			this.clrXp.Size = new System.Drawing.Size(24, 24);
			this.clrXp.TabIndex = 11;
			this.clrXp.TabStop = false;
			this.ttDescription.SetToolTip(this.clrXp, "�F��ݒ肵�܂��B");
			this.clrXp.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.clrWeightGuide);
			this.groupBox2.Controls.Add(this.txtWeightAlpha);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.txtWeightSize);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.chkShowWeightGuide);
			this.groupBox2.Controls.Add(this.clrWeight);
			this.groupBox2.Controls.Add(this.chkShowWeight);
			this.groupBox2.Location = new System.Drawing.Point(8, 120);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(240, 120);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "�d�S";
			// 
			// clrWeightGuide
			// 
			this.clrWeightGuide.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrWeightGuide.Location = new System.Drawing.Point(208, 80);
			this.clrWeightGuide.Name = "clrWeightGuide";
			this.clrWeightGuide.Size = new System.Drawing.Size(24, 24);
			this.clrWeightGuide.TabIndex = 15;
			this.clrWeightGuide.TabStop = false;
			this.ttDescription.SetToolTip(this.clrWeightGuide, "�F��ݒ肵�܂��B");
			this.clrWeightGuide.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// txtWeightAlpha
			// 
			this.txtWeightAlpha.Location = new System.Drawing.Point(160, 88);
			this.txtWeightAlpha.Name = "txtWeightAlpha";
			this.txtWeightAlpha.Size = new System.Drawing.Size(40, 19);
			this.txtWeightAlpha.TabIndex = 14;
			this.txtWeightAlpha.Text = "textBox3";
			this.ttDescription.SetToolTip(this.txtWeightAlpha, "�d�S�K�C�h�̕s�����x�ł��B�������قǓ����ɂȂ�܂��B");
			this.txtWeightAlpha.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValueInput_KeyPress);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(104, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 16);
			this.label3.TabIndex = 13;
			this.label3.Text = "�����x(0.0 - 1.0)";
			// 
			// txtWeightSize
			// 
			this.txtWeightSize.Location = new System.Drawing.Point(64, 88);
			this.txtWeightSize.Name = "txtWeightSize";
			this.txtWeightSize.Size = new System.Drawing.Size(40, 19);
			this.txtWeightSize.TabIndex = 12;
			this.txtWeightSize.Text = "textBox2";
			this.ttDescription.SetToolTip(this.txtWeightSize, "�d�S�K�C�h�̑傫���ł��B");
			this.txtWeightSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValueInput_KeyPress);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 16);
			this.label2.TabIndex = 11;
			this.label2.Text = "�傫��(�W��: 1.5)";
			// 
			// chkShowWeightGuide
			// 
			this.chkShowWeightGuide.Location = new System.Drawing.Point(8, 48);
			this.chkShowWeightGuide.Name = "chkShowWeightGuide";
			this.chkShowWeightGuide.Size = new System.Drawing.Size(120, 24);
			this.chkShowWeightGuide.TabIndex = 10;
			this.chkShowWeightGuide.Text = "�d�S�ɃK�C�h��\��";
			this.ttDescription.SetToolTip(this.chkShowWeightGuide, "�d�S�ʒu�ɕH�`�̃K�C�h��\�����܂��B");
			// 
			// clrWeight
			// 
			this.clrWeight.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.clrWeight.Location = new System.Drawing.Point(120, 16);
			this.clrWeight.Name = "clrWeight";
			this.clrWeight.Size = new System.Drawing.Size(24, 24);
			this.clrWeight.TabIndex = 9;
			this.clrWeight.TabStop = false;
			this.ttDescription.SetToolTip(this.clrWeight, "�F��ݒ肵�܂��B");
			this.clrWeight.Click += new System.EventHandler(this.ColorBox_Click);
			// 
			// chkShowWeight
			// 
			this.chkShowWeight.Location = new System.Drawing.Point(8, 16);
			this.chkShowWeight.Name = "chkShowWeight";
			this.chkShowWeight.Size = new System.Drawing.Size(112, 24);
			this.chkShowWeight.TabIndex = 7;
			this.chkShowWeight.Text = "�d�S�ʒu��\��";
			this.ttDescription.SetToolTip(this.chkShowWeight, "�d�S��\�����C����\�����܂��B");
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rbGChip);
			this.groupBox1.Controls.Add(this.rbFrame);
			this.groupBox1.Controls.Add(this.rbAlpha);
			this.groupBox1.Controls.Add(this.chkShowGhost);
			this.groupBox1.Controls.Add(this.txtSwellRate);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.chkBaloonSwell);
			this.groupBox1.Controls.Add(this.chkShowCowl);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(240, 112);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "�`�b�v";
			// 
			// rbGChip
			// 
			this.rbGChip.Location = new System.Drawing.Point(168, 80);
			this.rbGChip.Name = "rbGChip";
			this.rbGChip.Size = new System.Drawing.Size(64, 16);
			this.rbGChip.TabIndex = 14;
			this.rbGChip.Text = "�H�HG.x";
			this.ttDescription.SetToolTip(this.rbGChip, "ChipG.x, RudderG.x�Ƃ������O�̕ʃt�@�C�����g���܂��B");
			// 
			// rbFrame
			// 
			this.rbFrame.Location = new System.Drawing.Point(96, 80);
			this.rbFrame.Name = "rbFrame";
			this.rbFrame.Size = new System.Drawing.Size(64, 16);
			this.rbFrame.TabIndex = 13;
			this.rbFrame.Text = "�t���[��";
			this.ttDescription.SetToolTip(this.rbFrame, "�S�[�X�g�����Ă��Ȃ��t���[���Ɠ���̕\�������܂��B");
			// 
			// rbAlpha
			// 
			this.rbAlpha.Location = new System.Drawing.Point(16, 80);
			this.rbAlpha.Name = "rbAlpha";
			this.rbAlpha.Size = new System.Drawing.Size(64, 16);
			this.rbAlpha.TabIndex = 12;
			this.rbAlpha.Text = "������";
			this.ttDescription.SetToolTip(this.rbAlpha, "Frame�Ȃ�Chip���ARudderF�Ȃ�Rudder�́A�����̔������\�����s���܂��B");
			// 
			// chkShowGhost
			// 
			this.chkShowGhost.Location = new System.Drawing.Point(8, 64);
			this.chkShowGhost.Name = "chkShowGhost";
			this.chkShowGhost.Size = new System.Drawing.Size(128, 16);
			this.chkShowGhost.TabIndex = 11;
			this.chkShowGhost.Text = "�S�[�X�g�`�b�v��\��";
			this.ttDescription.SetToolTip(this.chkShowGhost, "�S�[�X�g�`�b�v���������܂��B");
			// 
			// txtSwellRate
			// 
			this.txtSwellRate.Location = new System.Drawing.Point(192, 40);
			this.txtSwellRate.Name = "txtSwellRate";
			this.txtSwellRate.Size = new System.Drawing.Size(40, 19);
			this.txtSwellRate.TabIndex = 10;
			this.txtSwellRate.Text = "textBox1";
			this.txtSwellRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValueInput_KeyPress);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(136, 40);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 24);
			this.label1.TabIndex = 9;
			this.label1.Text = "�c���W��(�W��:0.5)";
			// 
			// chkBaloonSwell
			// 
			this.chkBaloonSwell.Location = new System.Drawing.Point(8, 40);
			this.chkBaloonSwell.Name = "chkBaloonSwell";
			this.chkBaloonSwell.Size = new System.Drawing.Size(128, 16);
			this.chkBaloonSwell.TabIndex = 8;
			this.chkBaloonSwell.Text = "�o���[����c��������";
			this.ttDescription.SetToolTip(this.chkBaloonSwell, "Jet�o���[����Power�̒l�ɂ���đ傫�����ς��悤�ɂ��܂��B");
			// 
			// chkShowCowl
			// 
			this.chkShowCowl.Location = new System.Drawing.Point(8, 16);
			this.chkShowCowl.Name = "chkShowCowl";
			this.chkShowCowl.Size = new System.Drawing.Size(104, 16);
			this.chkShowCowl.TabIndex = 7;
			this.chkShowCowl.Text = "�J�E����\��";
			this.ttDescription.SetToolTip(this.chkShowCowl, "�`�F�b�N���͂����ƃJ�E�����\���ɂ��܂��B");
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label13);
			this.tabPage2.Controls.Add(this.chkPrintAllAttribute);
			this.tabPage2.Controls.Add(this.chkCommaWithSpace);
			this.tabPage2.Controls.Add(this.label11);
			this.tabPage2.Controls.Add(this.txtIndentSpaceNum);
			this.tabPage2.Controls.Add(this.chkIndentBySpace);
			this.tabPage2.Controls.Add(this.chkIndentEnable);
			this.tabPage2.Controls.Add(this.chkOpenBracketWithChipDefinition);
			this.tabPage2.Controls.Add(this.chkReturnEndChipBracket);
			this.tabPage2.Controls.Add(this.groupBox5);
			this.tabPage2.Location = new System.Drawing.Point(4, 21);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(488, 247);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "RCD�o��";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(24, 216);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(176, 16);
			this.label13.TabIndex = 9;
			this.label13.Text = "�� �u�K�p�v�������Ɣ��f����܂�";
			// 
			// chkPrintAllAttribute
			// 
			this.chkPrintAllAttribute.Location = new System.Drawing.Point(16, 168);
			this.chkPrintAllAttribute.Name = "chkPrintAllAttribute";
			this.chkPrintAllAttribute.Size = new System.Drawing.Size(176, 16);
			this.chkPrintAllAttribute.TabIndex = 8;
			this.chkPrintAllAttribute.Text = "���ׂĂ̑����������o��";
			this.chkPrintAllAttribute.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkCommaWithSpace
			// 
			this.chkCommaWithSpace.Location = new System.Drawing.Point(16, 144);
			this.chkCommaWithSpace.Name = "chkCommaWithSpace";
			this.chkCommaWithSpace.Size = new System.Drawing.Size(176, 16);
			this.chkCommaWithSpace.TabIndex = 7;
			this.chkCommaWithSpace.Text = "�J���}�ɃX�y�[�X��t����";
			this.chkCommaWithSpace.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(128, 112);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(32, 16);
			this.label11.TabIndex = 6;
			this.label11.Text = "��";
			// 
			// txtIndentSpaceNum
			// 
			this.txtIndentSpaceNum.Location = new System.Drawing.Point(160, 112);
			this.txtIndentSpaceNum.Name = "txtIndentSpaceNum";
			this.txtIndentSpaceNum.Size = new System.Drawing.Size(48, 19);
			this.txtIndentSpaceNum.TabIndex = 5;
			this.txtIndentSpaceNum.Text = "1";
			this.txtIndentSpaceNum.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtIndentSpaceNum.TextChanged += new System.EventHandler(this.txtIndentSpaceNum_TextChanged);
			this.txtIndentSpaceNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIndentSpaceNum_KeyPress);
			// 
			// chkIndentBySpace
			// 
			this.chkIndentBySpace.Location = new System.Drawing.Point(32, 88);
			this.chkIndentBySpace.Name = "chkIndentBySpace";
			this.chkIndentBySpace.Size = new System.Drawing.Size(176, 16);
			this.chkIndentBySpace.TabIndex = 4;
			this.chkIndentBySpace.Text = "�^�u�łȂ��X�y�[�X�ŃC���f���g";
			this.chkIndentBySpace.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkIndentEnable
			// 
			this.chkIndentEnable.Location = new System.Drawing.Point(16, 64);
			this.chkIndentEnable.Name = "chkIndentEnable";
			this.chkIndentEnable.Size = new System.Drawing.Size(176, 16);
			this.chkIndentEnable.TabIndex = 3;
			this.chkIndentEnable.Text = "�C���f���g����";
			this.chkIndentEnable.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkOpenBracketWithChipDefinition
			// 
			this.chkOpenBracketWithChipDefinition.Location = new System.Drawing.Point(16, 40);
			this.chkOpenBracketWithChipDefinition.Name = "chkOpenBracketWithChipDefinition";
			this.chkOpenBracketWithChipDefinition.Size = new System.Drawing.Size(176, 16);
			this.chkOpenBracketWithChipDefinition.TabIndex = 2;
			this.chkOpenBracketWithChipDefinition.Text = "�u{�v���`�b�v��`�Ɠ����s�ɒu��";
			this.chkOpenBracketWithChipDefinition.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// chkReturnEndChipBracket
			// 
			this.chkReturnEndChipBracket.Location = new System.Drawing.Point(16, 16);
			this.chkReturnEndChipBracket.Name = "chkReturnEndChipBracket";
			this.chkReturnEndChipBracket.Size = new System.Drawing.Size(176, 16);
			this.chkReturnEndChipBracket.TabIndex = 1;
			this.chkReturnEndChipBracket.Text = "�����̃`�b�v�����s����";
			this.chkReturnEndChipBracket.CheckedChanged += new System.EventHandler(this.updateOutputPreview);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.txtOutputSample);
			this.groupBox5.Location = new System.Drawing.Point(216, 8);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(264, 232);
			this.groupBox5.TabIndex = 0;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "�T���v��";
			this.groupBox5.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBox5_Paint);
			// 
			// txtOutputSample
			// 
			this.txtOutputSample.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtOutputSample.Font = new System.Drawing.Font("�l�r �S�V�b�N", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.txtOutputSample.Location = new System.Drawing.Point(3, 15);
			this.txtOutputSample.Multiline = true;
			this.txtOutputSample.Name = "txtOutputSample";
			this.txtOutputSample.ReadOnly = true;
			this.txtOutputSample.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.txtOutputSample.Size = new System.Drawing.Size(258, 214);
			this.txtOutputSample.TabIndex = 0;
			this.txtOutputSample.WordWrap = false;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.chkAttrAutoApply);
			this.tabPage3.Controls.Add(this.lblScrollValue);
			this.tabPage3.Controls.Add(this.label12);
			this.tabPage3.Controls.Add(this.trkScrollFrame);
			this.tabPage3.Controls.Add(this.chkUnbisibleUnselectable);
			this.tabPage3.Controls.Add(this.chkAttrCopy);
			this.tabPage3.Location = new System.Drawing.Point(4, 21);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(488, 247);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "�G�f�B�b�g";
			// 
			// chkAttrAutoApply
			// 
			this.chkAttrAutoApply.Location = new System.Drawing.Point(16, 80);
			this.chkAttrAutoApply.Name = "chkAttrAutoApply";
			this.chkAttrAutoApply.Size = new System.Drawing.Size(288, 16);
			this.chkAttrAutoApply.TabIndex = 5;
			this.chkAttrAutoApply.Text = "�`�b�v�̑����ύX�͎����œK�p����";
			// 
			// lblScrollValue
			// 
			this.lblScrollValue.Location = new System.Drawing.Point(344, 144);
			this.lblScrollValue.Name = "lblScrollValue";
			this.lblScrollValue.Size = new System.Drawing.Size(48, 16);
			this.lblScrollValue.TabIndex = 4;
			this.lblScrollValue.Text = "0";
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(24, 112);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(256, 16);
			this.label12.TabIndex = 3;
			this.label12.Text = "�X�N���[���̃t���[����(0�ň�u�A�傫���قǒx��)";
			// 
			// trkScrollFrame
			// 
			this.trkScrollFrame.LargeChange = 10;
			this.trkScrollFrame.Location = new System.Drawing.Point(48, 136);
			this.trkScrollFrame.Maximum = 100;
			this.trkScrollFrame.Name = "trkScrollFrame";
			this.trkScrollFrame.Size = new System.Drawing.Size(280, 42);
			this.trkScrollFrame.TabIndex = 2;
			this.trkScrollFrame.TickFrequency = 5;
			this.trkScrollFrame.ValueChanged += new System.EventHandler(this.trkScrollFrame_ValueChanged);
			// 
			// chkUnbisibleUnselectable
			// 
			this.chkUnbisibleUnselectable.Location = new System.Drawing.Point(16, 48);
			this.chkUnbisibleUnselectable.Name = "chkUnbisibleUnselectable";
			this.chkUnbisibleUnselectable.Size = new System.Drawing.Size(312, 16);
			this.chkUnbisibleUnselectable.TabIndex = 1;
			this.chkUnbisibleUnselectable.Text = "��\���̃J�E���ƃt���[���̓N���b�N�őI������Ȃ��悤�ɂ���";
			// 
			// chkAttrCopy
			// 
			this.chkAttrCopy.Location = new System.Drawing.Point(16, 16);
			this.chkAttrCopy.Name = "chkAttrCopy";
			this.chkAttrCopy.Size = new System.Drawing.Size(440, 16);
			this.chkAttrCopy.TabIndex = 0;
			this.chkAttrCopy.Text = "����̃`�b�v��h���`�b�v�Ƃ��Ēǉ������Ƃ��A�e�`�b�v��Angle�ȊO�̑������R�s�[����";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(232, 280);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 24);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnApply
			// 
			this.btnApply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnApply.Location = new System.Drawing.Point(320, 280);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(80, 24);
			this.btnApply.TabIndex = 2;
			this.btnApply.Text = "�K�p";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(408, 280);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 24);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "�L�����Z��";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmConfig
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(496, 309);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.tabControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmConfig";
			this.ShowInTaskbar = false;
			this.Text = "�e��ݒ�";
			this.Load += new System.EventHandler(this.frmConfig_Load);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmConfig_Closing);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrNorth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWest)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrEast)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrSouth)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorB)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrCursorF)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrBack)).EndInit();
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrZn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrZp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrYp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXn)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrXp)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrWeightGuide)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrWeight)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.groupBox5.ResumeLayout(false);
			this.groupBox5.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkScrollFrame)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void frmConfig_Load(object sender, System.EventArgs e) {
			chkBaloonSwell.Checked = optDraw.BaloonSwelling;
			chkShowCowl.Checked = optDraw.ShowCowl;
			chkShowGhost.Checked = optDraw.FrameGhostShow;
			chkShowWeight.Checked = optDraw.WeightEnable;
			chkShowWeightGuide.Checked = optDraw.WeightBallEnable;
			chkXAxis.Checked = optDraw.XAxisEnable;
			chkYAxis.Checked = optDraw.YAxisEnable;
			chkZAxis.Checked = optDraw.ZAxisEnable;
			chkXNegAxis.Checked = optDraw.XNegAxisEnable;
			chkYNegAxis.Checked = optDraw.YNegAxisEnable;
			chkZNegAxis.Checked = optDraw.ZNegAxisEnable;
			switch (optDraw.FrameGhostView) {
				case 0:
					rbAlpha.Checked = true;
					break;
				case 1:
					rbFrame.Checked = true;
					break;
				case 2:
					rbGChip.Checked = true;
					break;
			}
			chkShowAlways.Checked = optDraw.ShowGuideAlways;

			txtSwellRate.Text = optDraw.BaloonSwellingRatio.ToString();
			txtWeightAlpha.Text = optDraw.WeightBallAlpha.ToString();
			txtWeightSize.Text = optDraw.WeightBallSize.ToString();

			clrEast.BackColor = optDraw.EGuideColor;
			clrNorth.BackColor = optDraw.NGuideColor;
			clrSouth.BackColor = optDraw.SGuideColor;
			clrWeight.BackColor = optDraw.WeightColor;
			clrWest.BackColor = optDraw.WGuideColor;
			clrXn.BackColor = optDraw.XNegAxisColor;
			clrXp.BackColor = optDraw.XAxisColor;
			clrYn.BackColor = optDraw.YNegAxisColor;
			clrYp.BackColor = optDraw.YAxisColor;
			clrZn.BackColor = optDraw.ZNegAxisColor;
			clrZp.BackColor = optDraw.ZAxisColor;
			clrCursorB.BackColor = optDraw.CursorBackColor;
			clrCursorF.BackColor = optDraw.CursorFrontColor;
			clrBack.BackColor = optDraw.BackColor;

			chkCameraOrtho.Checked = optDraw.CameraOrtho;

			clrWeightGuide.BackColor = optDraw.WeightBallColor;

			chkCommaWithSpace.Checked = optOutput.CommaWithSpace;
			chkIndentBySpace.Checked = optOutput.IndentBySpace;
			chkIndentEnable.Checked = optOutput.IndentEnable;
			chkOpenBracketWithChipDefinition.Checked = optOutput.OpenBracketWithChipDefinition;
			chkPrintAllAttribute.Checked = optOutput.PrintAllAttributes;
			chkReturnEndChipBracket.Checked = optOutput.ReturnEndChipBracket;
			txtIndentSpaceNum.Text = optOutput.IndentNum.ToString();

			chkAttrCopy.Checked = optEdit.ConvertParentAttributes;
			chkUnbisibleUnselectable.Checked = optEdit.UnvisibleNotSelected;
			trkScrollFrame.Value = optEdit.ScrollFrameNum;
			chkAttrAutoApply.Checked = optEdit.AttributeAutoApply;

		}

		private void Apply() {
			buildDrawOption(null);

			buildOutputOption(null);

			buildEditOption(null);

			mainwindow.SetListBackColor();
		}

		private void buildEditOption(RcEditOptions target) {
			if (target == null) target = optEdit;
			target.ConvertParentAttributes = chkAttrCopy.Checked;
			target.UnvisibleNotSelected = chkUnbisibleUnselectable.Checked;
			target.ScrollFrameNum = trkScrollFrame.Value;
			target.AttributeAutoApply = chkAttrAutoApply.Checked;
		}

		private void buildDrawOption(RcDrawOptions target) {
			if (target == null) target = optDraw;
			target.BaloonSwelling = chkBaloonSwell.Checked;
			target.ShowCowl = chkShowCowl.Checked;
			target.FrameGhostShow = chkShowGhost.Checked;
			target.WeightEnable = chkShowWeight.Checked;
			target.WeightBallEnable = chkShowWeightGuide.Checked;
			target.XAxisEnable = chkXAxis.Checked;
			target.YAxisEnable = chkYAxis.Checked;
			target.ZAxisEnable = chkZAxis.Checked;
			target.XNegAxisEnable = chkXNegAxis.Checked;
			target.YNegAxisEnable = chkYNegAxis.Checked;
			target.ZNegAxisEnable = chkZNegAxis.Checked;
			if (rbAlpha.Checked)
				target.FrameGhostView = 0;
			else if (rbFrame.Checked)
				target.FrameGhostView = 1;
			else
				target.FrameGhostView = 2;
			target.ShowGuideAlways = chkShowAlways.Checked;

			target.BaloonSwellingRatio = float.Parse(txtSwellRate.Text);
			target.WeightBallAlpha = float.Parse(txtWeightAlpha.Text);
			target.WeightBallSize = float.Parse(txtWeightSize.Text);

			target.EGuideColor = clrEast.BackColor;
			target.NGuideColor = clrNorth.BackColor;
			target.SGuideColor = clrSouth.BackColor;
			target.WeightColor = clrWeight.BackColor;
			target.WGuideColor = clrWest.BackColor;
			target.XNegAxisColor = clrXn.BackColor;
			target.XAxisColor = clrXp.BackColor;
			target.YNegAxisColor = clrYn.BackColor;
			target.YAxisColor = clrYp.BackColor;
			target.ZNegAxisColor = clrZn.BackColor;
			target.ZAxisColor = clrZp.BackColor;
			target.CursorBackColor = clrCursorB.BackColor;
			target.CursorFrontColor = clrCursorF.BackColor;
			target.BackColor = clrBack.BackColor;
			target.CameraOrtho = chkCameraOrtho.Checked;
			target.WeightBallColor = clrWeightGuide.BackColor;
		}

		private void buildOutputOption(RcOutputOptions target) {
			if (target == null) target = optOutput;
			target.CommaWithSpace = chkCommaWithSpace.Checked;
			target.IndentBySpace = chkIndentBySpace.Checked;
			target.IndentEnable = chkIndentEnable.Checked;
			target.OpenBracketWithChipDefinition = chkOpenBracketWithChipDefinition.Checked;
			target.PrintAllAttributes = chkPrintAllAttribute.Checked;
			target.ReturnEndChipBracket = chkReturnEndChipBracket.Checked;
			if (!uint.TryParse(txtIndentSpaceNum.Text, out target.IndentNum)) {
				target.IndentNum = 2;
				txtIndentSpaceNum.Text = "2";
			}


		}
		private void ColorBox_Click(object sender, System.EventArgs e) {
			PictureBox box = (PictureBox)sender;

			dlgColor.Color = box.BackColor;
			if (dlgColor.ShowDialog() == DialogResult.OK)
				box.BackColor = dlgColor.Color;

		}

		private void txtValueInput_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (!char.IsNumber(e.KeyChar) && !(e.KeyChar == '.') && !char.IsControl(e.KeyChar))
				e.Handled = true;
		}

		private void btnOK_Click(object sender, System.EventArgs e) {
			Apply();
			this.Close();
		}

		private void btnApply_Click(object sender, System.EventArgs e) {
			Apply();
		}

		private void frmConfig_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			this.Visible = false;
			e.Cancel = true;
		}

		private void chkIndentBySpace_CheckedChanged(object sender, System.EventArgs e) {
		}

		private void groupBox5_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			updateOutputPreview(sender, e);
		}

		private void btnCancel_Click(object sender, System.EventArgs e) {
			this.Hide();
		}

		private void txtIndentSpaceNum_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
		}

		private void trkScrollFrame_ValueChanged(object sender, System.EventArgs e) {
			lblScrollValue.Text = trkScrollFrame.Value.ToString();
		}

		private void updateOutputPreview(object sender, EventArgs e) {
			RcOutputOptions opt = new RcOutputOptions(true);
			buildOutputOption(opt);

			txtOutputSample.Text = chipSample.ToString(0, opt).Replace("\n", "\r\n");
		}

		private void txtIndentSpaceNum_TextChanged(object sender, EventArgs e) {
			int dummy;
			if (int.TryParse(txtIndentSpaceNum.Text, out dummy)) {
				updateOutputPreview(sender, e);
			}
		}
	}
}
