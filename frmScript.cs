using RigidChips;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace rcm {
	/// <summary>
	/// �X�N���v�g�ҏW�t�H�[���B
	/// </summary>
	public class frmScript : System.Windows.Forms.Form {
		RcData data;
		frmMain mainForm;

		bool Modified {
			get { return txtScript.Document.IsDirty; }
			set {
				if (!value) {
					txtScript.Document.IsDirty = value;
				}
			}
		}

		string[] functionCategory = {
										@"#��ʃT�C�Y�E�p�t�H�[�}���X"
										,@"#���C���`��"
										,@"#���͌��m"
										,@"#���f�����"
										,@"#�`�b�v���"
										,@"#�؂藣��"
										,@"#�`�b�v�^�C�v�擾"
										,@"#�I�u�W�F�N�g(�{�[��)"
										,@"#CCD�J����"
										,@"#�l�b�g���[�N"
										,@"#���w�֐�"
									};
		bool[] functionCategoryEnable = new bool[11];

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.ComboBox cmbVals;
		private System.Windows.Forms.Panel panelInsertion;
		private System.Windows.Forms.ComboBox cmbNames;
		private System.Windows.Forms.MenuItem miCommand;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MenuItem miCloseWithSave;
		private System.Windows.Forms.MenuItem miCloseWithoutSave;
		private Sgry.Azuki.WinForms.AzukiControl txtScript;
		private System.Windows.Forms.ComboBox cmbFunction;
		private System.Windows.Forms.MenuItem miFuncList;
		private System.Windows.Forms.MenuItem miScriptFuncs;
		private System.Windows.Forms.MenuItem miLuaFuncs;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miFuncCategory;
		private System.Windows.Forms.MenuItem miCatEnable0;
		private System.Windows.Forms.MenuItem miCatEnable1;
		private System.Windows.Forms.MenuItem miCatEnable2;
		private System.Windows.Forms.MenuItem miCatEnable3;
		private System.Windows.Forms.MenuItem miCatEnable4;
		private System.Windows.Forms.MenuItem miCatEnable5;
		private System.Windows.Forms.MenuItem miCatEnable6;
		private System.Windows.Forms.MenuItem miCatEnable7;
		private System.Windows.Forms.MenuItem miCatEnable8;
		private System.Windows.Forms.MenuItem miCatEnable10;
		private System.Windows.Forms.MenuItem miCatEnable9;
		private IContainer components;

		public frmScript(frmMain MainForm, RcData data) {
			//
			// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent �Ăяo���̌�ɁA�R���X�g���N�^ �R�[�h��ǉ����Ă��������B
			//
			this.data = data;
			this.mainForm = MainForm;
			for (int i = 0; i < functionCategoryEnable.Length; i++)
				functionCategoryEnable[i] = true;
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmScript));
			this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
			this.miCommand = new System.Windows.Forms.MenuItem();
			this.miSave = new System.Windows.Forms.MenuItem();
			this.miCloseWithSave = new System.Windows.Forms.MenuItem();
			this.miCloseWithoutSave = new System.Windows.Forms.MenuItem();
			this.miFuncList = new System.Windows.Forms.MenuItem();
			this.miScriptFuncs = new System.Windows.Forms.MenuItem();
			this.miLuaFuncs = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miFuncCategory = new System.Windows.Forms.MenuItem();
			this.miCatEnable0 = new System.Windows.Forms.MenuItem();
			this.miCatEnable1 = new System.Windows.Forms.MenuItem();
			this.miCatEnable2 = new System.Windows.Forms.MenuItem();
			this.miCatEnable3 = new System.Windows.Forms.MenuItem();
			this.miCatEnable4 = new System.Windows.Forms.MenuItem();
			this.miCatEnable5 = new System.Windows.Forms.MenuItem();
			this.miCatEnable6 = new System.Windows.Forms.MenuItem();
			this.miCatEnable7 = new System.Windows.Forms.MenuItem();
			this.miCatEnable8 = new System.Windows.Forms.MenuItem();
			this.miCatEnable9 = new System.Windows.Forms.MenuItem();
			this.miCatEnable10 = new System.Windows.Forms.MenuItem();
			this.cmbVals = new System.Windows.Forms.ComboBox();
			this.panelInsertion = new System.Windows.Forms.Panel();
			this.cmbFunction = new System.Windows.Forms.ComboBox();
			this.cmbNames = new System.Windows.Forms.ComboBox();
			this.txtScript = new Sgry.Azuki.WinForms.AzukiControl();
			this.panelInsertion.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miCommand,
			this.miFuncList});
			// 
			// miCommand
			// 
			this.miCommand.Index = 0;
			this.miCommand.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miSave,
			this.miCloseWithSave,
			this.miCloseWithoutSave});
			this.miCommand.Text = "�R�}���h(&C)";
			// 
			// miSave
			// 
			this.miSave.Index = 0;
			this.miSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miSave.Text = "�ύX��ۑ�(&S)";
			this.miSave.Click += new System.EventHandler(this.miSave_Click);
			// 
			// miCloseWithSave
			// 
			this.miCloseWithSave.Index = 1;
			this.miCloseWithSave.Text = "�ύX��ۑ����ĕ���(&C)";
			this.miCloseWithSave.Click += new System.EventHandler(this.miCloseWithSave_Click);
			// 
			// miCloseWithoutSave
			// 
			this.miCloseWithoutSave.Index = 2;
			this.miCloseWithoutSave.Text = "�ύX��ۑ����Ȃ��ŕ���(&Q)";
			this.miCloseWithoutSave.Click += new System.EventHandler(this.miCloseWithoutSave_Click);
			// 
			// miFuncList
			// 
			this.miFuncList.Index = 1;
			this.miFuncList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miScriptFuncs,
			this.miLuaFuncs,
			this.menuItem1,
			this.miFuncCategory});
			this.miFuncList.Text = "�֐����X�g(&F)";
			// 
			// miScriptFuncs
			// 
			this.miScriptFuncs.Checked = true;
			this.miScriptFuncs.Index = 0;
			this.miScriptFuncs.RadioCheck = true;
			this.miScriptFuncs.Text = "&Script";
			this.miScriptFuncs.Click += new System.EventHandler(this.miScriptFuncs_Click);
			// 
			// miLuaFuncs
			// 
			this.miLuaFuncs.Index = 1;
			this.miLuaFuncs.RadioCheck = true;
			this.miLuaFuncs.Text = "&Lua";
			this.miLuaFuncs.Click += new System.EventHandler(this.miLuaFuncs_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// miFuncCategory
			// 
			this.miFuncCategory.Index = 3;
			this.miFuncCategory.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.miCatEnable0,
			this.miCatEnable1,
			this.miCatEnable2,
			this.miCatEnable3,
			this.miCatEnable4,
			this.miCatEnable5,
			this.miCatEnable6,
			this.miCatEnable7,
			this.miCatEnable8,
			this.miCatEnable9,
			this.miCatEnable10});
			this.miFuncCategory.Text = "�\������J�e�S��(&C)";
			// 
			// miCatEnable0
			// 
			this.miCatEnable0.Checked = true;
			this.miCatEnable0.Index = 0;
			this.miCatEnable0.Text = "��ʃT�C�Y�E�p�t�H�[�}���X";
			this.miCatEnable0.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable1
			// 
			this.miCatEnable1.Checked = true;
			this.miCatEnable1.Index = 1;
			this.miCatEnable1.Text = "���C���`��";
			this.miCatEnable1.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable2
			// 
			this.miCatEnable2.Checked = true;
			this.miCatEnable2.Index = 2;
			this.miCatEnable2.Text = "���͌��m";
			this.miCatEnable2.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable3
			// 
			this.miCatEnable3.Checked = true;
			this.miCatEnable3.Index = 3;
			this.miCatEnable3.Text = "���f�����";
			this.miCatEnable3.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable4
			// 
			this.miCatEnable4.Checked = true;
			this.miCatEnable4.Index = 4;
			this.miCatEnable4.Text = "�`�b�v���";
			this.miCatEnable4.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable5
			// 
			this.miCatEnable5.Checked = true;
			this.miCatEnable5.Index = 5;
			this.miCatEnable5.Text = "�؂藣��";
			this.miCatEnable5.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable6
			// 
			this.miCatEnable6.Checked = true;
			this.miCatEnable6.Index = 6;
			this.miCatEnable6.Text = "�`�b�v�^�C�v�擾";
			this.miCatEnable6.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable7
			// 
			this.miCatEnable7.Checked = true;
			this.miCatEnable7.Index = 7;
			this.miCatEnable7.Text = "�I�u�W�F�N�g(�{�[��)";
			this.miCatEnable7.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable8
			// 
			this.miCatEnable8.Checked = true;
			this.miCatEnable8.Index = 8;
			this.miCatEnable8.Text = "CCD�J����";
			this.miCatEnable8.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable9
			// 
			this.miCatEnable9.Checked = true;
			this.miCatEnable9.Index = 9;
			this.miCatEnable9.Text = "�l�b�g���[�N";
			this.miCatEnable9.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable10
			// 
			this.miCatEnable10.Checked = true;
			this.miCatEnable10.Index = 10;
			this.miCatEnable10.Text = "���w�֐�";
			this.miCatEnable10.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// cmbVals
			// 
			this.cmbVals.DropDownWidth = 400;
			this.cmbVals.Location = new System.Drawing.Point(0, 0);
			this.cmbVals.Name = "cmbVals";
			this.cmbVals.Size = new System.Drawing.Size(121, 20);
			this.cmbVals.TabIndex = 0;
			this.cmbVals.Text = "(�ϐ�)";
			this.cmbVals.Enter += new System.EventHandler(this.cmbVals_Enter);
			this.cmbVals.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbInsertion_KeyPress);
			// 
			// panelInsertion
			// 
			this.panelInsertion.Controls.Add(this.cmbFunction);
			this.panelInsertion.Controls.Add(this.cmbNames);
			this.panelInsertion.Controls.Add(this.cmbVals);
			this.panelInsertion.Dock = System.Windows.Forms.DockStyle.Top;
			this.panelInsertion.Location = new System.Drawing.Point(0, 0);
			this.panelInsertion.Name = "panelInsertion";
			this.panelInsertion.Size = new System.Drawing.Size(352, 24);
			this.panelInsertion.TabIndex = 1;
			this.panelInsertion.Layout += new System.Windows.Forms.LayoutEventHandler(this.panelInsertion_Layout);
			// 
			// cmbFunction
			// 
			this.cmbFunction.DropDownWidth = 400;
			this.cmbFunction.Location = new System.Drawing.Point(248, 0);
			this.cmbFunction.Name = "cmbFunction";
			this.cmbFunction.Size = new System.Drawing.Size(121, 20);
			this.cmbFunction.TabIndex = 2;
			this.cmbFunction.Text = "(�֐�)";
			this.cmbFunction.Enter += new System.EventHandler(this.cmbFunction_Enter);
			this.cmbFunction.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbFunction_KeyPress);
			this.cmbFunction.DropDown += new System.EventHandler(this.cmbFunction_Enter);
			// 
			// cmbNames
			// 
			this.cmbNames.DropDownWidth = 400;
			this.cmbNames.Location = new System.Drawing.Point(128, 0);
			this.cmbNames.Name = "cmbNames";
			this.cmbNames.Size = new System.Drawing.Size(121, 20);
			this.cmbNames.TabIndex = 1;
			this.cmbNames.Text = "(�`�b�v��)";
			this.cmbNames.Enter += new System.EventHandler(this.cmbNames_Enter);
			this.cmbNames.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbInsertion_KeyPress);
			// 
			// txtScript
			// 
			this.txtScript.AllowDrop = true;
			this.txtScript.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.txtScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtScript.DrawingOption = ((Sgry.Azuki.DrawingOption)(((((Sgry.Azuki.DrawingOption.DrawsFullWidthSpace | Sgry.Azuki.DrawingOption.DrawsTab)
						| Sgry.Azuki.DrawingOption.DrawsEol)
						| Sgry.Azuki.DrawingOption.HighlightCurrentLine)
						| Sgry.Azuki.DrawingOption.ShowsLineNumber)));
			this.txtScript.Font = new System.Drawing.Font("�l�r �S�V�b�N", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.txtScript.Location = new System.Drawing.Point(0, 24);
			this.txtScript.Name = "txtScript";
			this.txtScript.Size = new System.Drawing.Size(352, 226);
			this.txtScript.TabIndex = 3;
			this.txtScript.TabWidth = 8;
			this.txtScript.Text = "txtScript";
			this.txtScript.ViewWidth = 328;
			this.txtScript.TextChanged += new System.EventHandler(this.txtScript_TextChanged);
			this.txtScript.Validated += new System.EventHandler(this.txtScript_Validated);
			this.txtScript.Enter += new System.EventHandler(this.txtScript_Enter);
			// 
			// frmScript
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(352, 250);
			this.Controls.Add(this.txtScript);
			this.Controls.Add(this.panelInsertion);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Menu = this.mainMenu1;
			this.MinimizeBox = false;
			this.Name = "frmScript";
			this.Text = "Script{...}/Lua{...}�ݒ�";
			this.Load += new System.EventHandler(this.frmScript_Load);
			this.VisibleChanged += new System.EventHandler(this.frmScript_VisibleChanged);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmScript_Closing);
			this.panelInsertion.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void panelInsertion_Layout(object sender, System.Windows.Forms.LayoutEventArgs e) {
			cmbVals.Top = cmbNames.Top = cmbFunction.Top = 0;
			cmbVals.Width = cmbNames.Width = cmbFunction.Width = cmbNames.Left = panelInsertion.ClientRectangle.Width / 3;
			cmbFunction.Left = cmbFunction.Width * 2;
		}

		private void miSave_Click(object sender, System.EventArgs e) {
			if (Modified) {
				data.script = txtScript.Text;
				mainForm.Modified = true;
			}

			Modified = false;
		}

		private void miCloseWithSave_Click(object sender, System.EventArgs e) {
			miSave_Click(sender, e);
			this.Hide();
		}

		private void frmScript_VisibleChanged(object sender, System.EventArgs e) {
			if (Visible) {
				this.txtScript.Text = data.script;
				this.miScriptFuncs.Checked = !(this.miLuaFuncs.Checked = data.luascript);
				this.txtScript.ClearHistory();
				Modified = false;
			}
		}

		private void cmbVals_Enter(object sender, System.EventArgs e) {
			cmbVals.Items.Clear();
			foreach (RcVal v in data.vals.List) {
				cmbVals.Items.Add(miLuaFuncs.Checked ? v.ValName.ToUpper() : v.ValName);
			}
		}

		private void cmbNames_Enter(object sender, System.EventArgs e) {
			cmbNames.Items.Clear();
			RcChipBase chip;
			for (int i = 0; i < RcData.MaxChipCount; i++) {
				chip = data.GetChipFromLib(i);
				if (chip != null && chip.Name != null && chip.Name != "")
					cmbNames.Items.Add(miLuaFuncs.Checked ? chip.Name.ToUpper() : chip.Name);
			}
		}

		private void cmbInsertion_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (e.KeyChar == 13) {
				int selStart, selEnd;
				txtScript.GetSelection(out selStart, out selEnd);
				txtScript.Document.Replace(((ComboBox)sender).Text, selStart, selEnd);
				//	txtScript.SelectedText = ((ComboBox)sender).Text;
				txtScript.Focus();
			}
		}

		private void frmScript_Closing(object sender, FormClosingEventArgs e) {
			if (mainForm.NowClosing) return;
			if (Modified) {
				DialogResult res;
				res = MessageBox.Show("�X�N���v�g�͕ύX����Ă��܂��B\n�ύX��K�p���܂����H", "�E�B���h�E�������悤�Ƃ��Ă��܂�", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				switch (res) {
					case DialogResult.Yes:
						miSave_Click(sender, EventArgs.Empty);
						goto case DialogResult.No;
					case DialogResult.No:
						e.Cancel = true;
						this.Hide();
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}
			else {
				e.Cancel = true;
				this.Hide();
				Debug.WriteLine("Hide frmScript.");
			}
		}

		private void txtScript_TextChanged(object sender, System.EventArgs e) {
			//Modified = true;
			Debug.WriteLine("txtScript_TextChanged called");
		}

		private struct FunctionEntry {
			public string Text;
			public int Category;
			public FunctionEntry(int cat, string txt) {
				Category = cat;
				Text = txt;
				Debug.WriteLine(string.Format("new FunctionEntry({0},{1})", cat, txt));
			}
			public override string ToString() {
				return Text;
			}
		}
		FunctionEntry[] functionList;
		private void cmbFunction_Enter(object sender, System.EventArgs e) {
			if (functionList == null) updateFunctionList();
			cmbFunction.Items.Clear();
			foreach (var s in functionList) {
				if (functionCategoryEnable[s.Category]) {
					cmbFunction.Items.Add(s.Text);
				}
			}
		}

		private void updateFunctionList() {
			string prevCD = System.IO.Directory.GetCurrentDirectory();
			System.IO.Directory.SetCurrentDirectory(Application.StartupPath);
			System.IO.StreamReader file;
			int i = 0;
			string input;

			var funcs = new List<FunctionEntry>();
			if (miScriptFuncs.Checked) {
				file = new System.IO.StreamReader("scriptfunc.dat", System.Text.Encoding.Default);
				while ((input = file.ReadLine()) != null) {
					if (i < functionCategory.Length && input == functionCategory[i]) {
						i++;
					}
					else // if (inputswitch)
						funcs.Add(new FunctionEntry(i - 1, input));
				}
			}
			else if (miLuaFuncs.Checked) {
				file = new System.IO.StreamReader("luafunc.dat", System.Text.Encoding.Default);
				while ((input = file.ReadLine()) != null) {
					if (i < functionCategory.Length && input == functionCategory[i]) {
						i++;
					}
					else // if (inputswitch)
						funcs.Add(new FunctionEntry(i - 1, input));
				}
			}
			else {
				throw new ApplicationException("�X�N���v�g�^�C�v���s���ł��B");
			}
			System.IO.Directory.SetCurrentDirectory(prevCD);

			functionList = funcs.ToArray();
			Debug.WriteLine("updated.", "functionList");
		}

		private void miCatEnable_Click(object sender, System.EventArgs e) {
			MenuItem item = (MenuItem)sender;
			for (int i = 0; i < functionCategory.Length; i++) {
				if (functionCategory[i] == "#" + item.Text)
					functionCategoryEnable[i] = item.Checked = !item.Checked;
			}
			functionList = null;
		}

		private void cmbFunction_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if (e.KeyChar == 13) {
				int selStart, selEnd;
				txtScript.GetSelection(out selStart, out selEnd);
				txtScript.Document.Replace(((ComboBox)sender).Text.Split(':')[0], selStart, selEnd);
				//	txtScript.SelectedText = ((ComboBox)sender).Text.Split(':')[0];
				txtScript.Focus();
			}
		}

		private void miLuaFuncs_Click(object sender, System.EventArgs e) {
			if (miLuaFuncs.Checked == false) {
				if (MessageBox.Show("�X�N���v�g�`����ύX���Ă��A�R�[�h�͕ϊ�����܂���B\n\n����ł��ύX���܂����H", "�X�N���v�g�`���ύX�m�F", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
					return;
			}
			miScriptFuncs.Checked = false;
			data.luascript = miLuaFuncs.Checked = true;
			mainForm.Modified = true;
			functionList = null;
		}

		private void miScriptFuncs_Click(object sender, System.EventArgs e) {
			if (miScriptFuncs.Checked == false) {
				if (MessageBox.Show("�X�N���v�g�`����ύX���Ă��A�R�[�h�͕ϊ�����܂���B\n\n����ł��ύX���܂����H", "�X�N���v�g�`���ύX�m�F", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
					return;
			}
			miScriptFuncs.Checked = true;
			data.luascript = miLuaFuncs.Checked = false;
			mainForm.Modified = true;
			functionList = null;
		}

		private void frmScript_Load(object sender, System.EventArgs e) {
			try {
				System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + "\\func.cfg");

				int val = int.Parse(file.ReadLine());

				for (int i = 0; i < functionCategory.Length; i++) {
					functionCategoryEnable[i] = ((val & (1 << i)) > 0);
				}
				file.Close();

				miCatEnable0.Checked = functionCategoryEnable[0];
				miCatEnable1.Checked = functionCategoryEnable[1];
				miCatEnable2.Checked = functionCategoryEnable[2];
				miCatEnable3.Checked = functionCategoryEnable[3];
				miCatEnable4.Checked = functionCategoryEnable[4];
				miCatEnable5.Checked = functionCategoryEnable[5];
				miCatEnable6.Checked = functionCategoryEnable[6];
				miCatEnable7.Checked = functionCategoryEnable[7];
				miCatEnable8.Checked = functionCategoryEnable[8];
				miCatEnable9.Checked = functionCategoryEnable[9];
				miCatEnable10.Checked = functionCategoryEnable[10];
			}
			catch { }

			setupAzuki();
		}

		~frmScript() {
			System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\func.cfg");

			int val = 0;
			for (int i = 0; i < functionCategory.Length; i++) {
				val |= (functionCategoryEnable[i] ? 1 : 0) << i;
			}
			file.Write(val.ToString());

			file.Close();
		}

		private void miCloseWithoutSave_Click(object sender, System.EventArgs e) {
			if (MessageBox.Show("�ҏW���e�͕ۑ�����܂���B�E�B���h�E����܂����H", "�ύX�j���m�F", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)
				== DialogResult.Yes) {
				this.Modified = false;
				this.Close();
			}
		}

		private void setupAzuki() {
			Debug.WriteLine("setupAzuki");
			var editor = txtScript;
			var highlighter = new Sgry.Azuki.Highlighter.KeywordHighlighter();

			RcChipBase chip;
			// �`�b�v��
			{
				List<string> list = new List<string>();
				for (int i = 0; i < RcData.MaxChipCount; i++) {
					chip = data.GetChipFromLib(i);
					if (chip != null && chip.Name != null && chip.Name != "")
						list.Add(data.luascript ? chip.Name.ToUpper() : chip.Name);
				}
				list.Sort();
				//highlighter.SetKeywords(list.ToArray(), Sgry.Azuki.CharClass.Keyword);
				highlighter.AddKeywordSet(list.ToArray(), Sgry.Azuki.CharClass.Keyword);
			}

			// �ϐ���
			{
				List<string> list = new List<string>();
				foreach (RcVal v in data.vals.List) {
					list.Add(data.luascript ? v.ValName.ToUpper() : v.ValName);
				}
				list.Sort();
				//highlighter.SetKeywords(list.ToArray(), Sgry.Azuki.CharClass.Keyword2);
				highlighter.AddKeywordSet(list.ToArray(), Sgry.Azuki.CharClass.Keyword2);
			}
			// �֐���
			{
				List<string> list = new List<string>();
				if (functionList == null) updateFunctionList();
				foreach (var s in functionList) {
					if (s.Text[0] != '_' && s.Text[0] != 'm') continue;
					string f = s.Text.Split('(')[0];
					if (f.Contains("?")) {
						list.Add(f.Replace('?', 'X'));
						list.Add(f.Replace('?', 'Y'));
						list.Add(f.Replace('?', 'Z'));
					}
					else {
						list.Add(f);
					}
				}
				list.Sort();
				//highlighter.SetKeywords(list.ToArray(), Sgry.Azuki.CharClass.Keyword3);
				highlighter.AddKeywordSet(list.ToArray(), Sgry.Azuki.CharClass.Keyword3);
			}
			// ���̑�
			highlighter.AddEnclosure("\"", "\"", Sgry.Azuki.CharClass.String);
			if (data.luascript) {
				// Lua�̐ݒ�
				var kwd = new[]{
					"and",
					"break",
					"do",
					"else",
					"elseif",
					"end",
					"false",
					"for",
					"function",
					"if",
					"in",
					"local",
					"nil",
					"not",
					"or",
					"repeat",
					"return",
					"then",
					"true",
					"until",
					"while",
					// ��������Lua�g�ݍ��݊֐�
					"assert",
					"collectgarbage",
					"dofile",
					"error",
					"_G",
					"getfenv",
					"getmetatable",
					"loadfile",
					"loadlib",
					"loadstring",
					"next",
					"pairs",
					"pcall",
					"print",
					"rawequal",
					"rawget",
					"rawset",
					"require",
					"setfenv",
					"setmetatable",
					"tonumber",
					"tostring",
					"type",
					"unpack",
					"_VERSION",
					"xpcall",

				};
				Array.Sort<string>(kwd);
				//highlighter.SetKeywords(kwd, Sgry.Azuki.CharClass.Macro);
				highlighter.AddKeywordSet(kwd, Sgry.Azuki.CharClass.Macro);
				var op = new[] { "+", "-", "*", "/", "^", "=", "~=", "<=", ">=", "<", ">", "==", ",", ".", "..", "...", };
				Array.Sort<string>(op);
				//highlighter.SetKeywords(op, Sgry.Azuki.CharClass.Delimitter);
				//highlighter.SetKeywords(op, Sgry.Azuki.CharClass.Delimiter);
				highlighter.AddKeywordSet(op, Sgry.Azuki.CharClass.Delimiter);
				var lib = new[] {
					"coroutine",
					"string",
					"table",
					"math",
				};
				Array.Sort<string>(lib);
				//highlighter.SetKeywords(lib, Sgry.Azuki.CharClass.Type);
				highlighter.AddKeywordSet(lib, Sgry.Azuki.CharClass.Type);
				highlighter.AddLineHighlight("--", Sgry.Azuki.CharClass.Comment);
				highlighter.AddEnclosure("[[", "]]", Sgry.Azuki.CharClass.Comment);
			}
			else {
				// Script�̐ݒ�
				var kwd = new[] { "if", "goto", "label", "print" };
				Array.Sort<string>(kwd);
				//highlighter.SetKeywords(kwd, Sgry.Azuki.CharClass.Macro);
				highlighter.AddKeywordSet(kwd, Sgry.Azuki.CharClass.Macro);
				var op = new[] { "+", "-", "*", "/", "=", "!=", "<>", ">", "<", ">=", "<=", "&", "|" };
				Array.Sort<string>(op);
				//highlighter.SetKeywords(op, Sgry.Azuki.CharClass.Delimiter);
				highlighter.AddKeywordSet(op, Sgry.Azuki.CharClass.Delimiter);
				highlighter.AddLineHighlight("//", Sgry.Azuki.CharClass.Comment);
			}


			editor.Highlighter = highlighter;

			// �^�u���͏o�̓I�v�V��������p��
			editor.TabWidth = (int)data.OutputOption.IndentNum;

			// �F�ݒ�
			editor.ColorScheme.ForeColor = Color.Black;
			editor.ColorScheme.BackColor = Color.White;
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Keyword, Color.DarkCyan, Color.White); // �`�b�v��
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Keyword2, Color.DarkBlue, Color.White);// �ϐ���
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Keyword3, Color.Red, Color.White); // �֐���
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.String, Color.DarkRed, Color.White); // ������
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Comment, Color.Green, Color.White); // �R�����g
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Macro, Color.Blue, Color.White); // ����\��
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Delimiter, Color.Violet, Color.White); // ���Z�q
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Type, Color.Gold, Color.White); // ���C�u������(Lua�̂�)
			editor.ColorScheme.SetColor(Sgry.Azuki.CharClass.Number, Color.DarkGreen, Color.White); // ���l


		}

		private void txtScript_Validated(object sender, EventArgs e) {
			//	setupAzuki();
		}

		private void txtScript_Enter(object sender, EventArgs e) {
			setupAzuki();

		}
	}
}
