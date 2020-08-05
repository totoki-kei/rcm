using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using RigidChips;

namespace rcm
{
	/// <summary>
	/// frmScript の概要の説明です。
	/// </summary>
	public class frmScript : System.Windows.Forms.Form {
		RcData data;
		bool modified = false;

		string[] functionCategory = {@"#画面サイズ・パフォーマンス",@"#ライン描画",@"#入力検知",@"#モデル情報",@"#チップ情報",@"#切り離し",@"#チップタイプ取得",@"#オブジェクト(ボール)",@"#CCDカメラ",@"#数学関数"	};
		bool[] functionCategoryEnable = new bool[10];

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.ComboBox cmbVals;
		private System.Windows.Forms.Panel panelInsertion;
		private System.Windows.Forms.ComboBox cmbNames;
		private System.Windows.Forms.MenuItem miCommand;
		private System.Windows.Forms.MenuItem miSave;
		private System.Windows.Forms.MenuItem miCloseWithSave;
		private System.Windows.Forms.MenuItem miCloseWithoutSave;
		private System.Windows.Forms.TextBox txtScript;
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
		private System.Windows.Forms.MenuItem miCatEnable9;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmScript(RcData data)
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			this.data = data;
			for(int i = 0;i < functionCategoryEnable.Length ;i++)
				functionCategoryEnable[i] = true;
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
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
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmScript));
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
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
			this.cmbVals = new System.Windows.Forms.ComboBox();
			this.panelInsertion = new System.Windows.Forms.Panel();
			this.cmbFunction = new System.Windows.Forms.ComboBox();
			this.cmbNames = new System.Windows.Forms.ComboBox();
			this.txtScript = new System.Windows.Forms.TextBox();
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
			this.miCommand.Text = "コマンド(&C)";
			// 
			// miSave
			// 
			this.miSave.Index = 0;
			this.miSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.miSave.Text = "変更を保存(&S)";
			this.miSave.Click += new System.EventHandler(this.miSave_Click);
			// 
			// miCloseWithSave
			// 
			this.miCloseWithSave.Index = 1;
			this.miCloseWithSave.Text = "変更を保存して閉じる(&C)";
			this.miCloseWithSave.Click += new System.EventHandler(this.miCloseWithSave_Click);
			// 
			// miCloseWithoutSave
			// 
			this.miCloseWithoutSave.Index = 2;
			this.miCloseWithoutSave.Text = "変更を保存しないで閉じる(&Q)";
			// 
			// miFuncList
			// 
			this.miFuncList.Index = 1;
			this.miFuncList.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.miScriptFuncs,
																					   this.miLuaFuncs,
																					   this.menuItem1,
																					   this.miFuncCategory});
			this.miFuncList.Text = "関数リスト(&F)";
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
																						   this.miCatEnable9});
			this.miFuncCategory.Text = "表示するカテゴリ(&C)";
			// 
			// miCatEnable0
			// 
			this.miCatEnable0.Checked = true;
			this.miCatEnable0.Index = 0;
			this.miCatEnable0.Text = "画面サイズ・パフォーマンス";
			this.miCatEnable0.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable1
			// 
			this.miCatEnable1.Checked = true;
			this.miCatEnable1.Index = 1;
			this.miCatEnable1.Text = "ライン描画";
			this.miCatEnable1.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable2
			// 
			this.miCatEnable2.Checked = true;
			this.miCatEnable2.Index = 2;
			this.miCatEnable2.Text = "入力検知";
			this.miCatEnable2.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable3
			// 
			this.miCatEnable3.Checked = true;
			this.miCatEnable3.Index = 3;
			this.miCatEnable3.Text = "モデル情報";
			this.miCatEnable3.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable4
			// 
			this.miCatEnable4.Checked = true;
			this.miCatEnable4.Index = 4;
			this.miCatEnable4.Text = "チップ情報";
			this.miCatEnable4.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable5
			// 
			this.miCatEnable5.Checked = true;
			this.miCatEnable5.Index = 5;
			this.miCatEnable5.Text = "切り離し";
			this.miCatEnable5.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable6
			// 
			this.miCatEnable6.Checked = true;
			this.miCatEnable6.Index = 6;
			this.miCatEnable6.Text = "チップタイプ取得";
			this.miCatEnable6.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable7
			// 
			this.miCatEnable7.Checked = true;
			this.miCatEnable7.Index = 7;
			this.miCatEnable7.Text = "オブジェクト(ボール)";
			this.miCatEnable7.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable8
			// 
			this.miCatEnable8.Checked = true;
			this.miCatEnable8.Index = 8;
			this.miCatEnable8.Text = "CCDカメラ";
			this.miCatEnable8.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// miCatEnable9
			// 
			this.miCatEnable9.Checked = true;
			this.miCatEnable9.Index = 9;
			this.miCatEnable9.Text = "数学関数";
			this.miCatEnable9.Click += new System.EventHandler(this.miCatEnable_Click);
			// 
			// cmbVals
			// 
			this.cmbVals.DropDownWidth = 400;
			this.cmbVals.Location = new System.Drawing.Point(0, 0);
			this.cmbVals.Name = "cmbVals";
			this.cmbVals.Size = new System.Drawing.Size(121, 20);
			this.cmbVals.TabIndex = 0;
			this.cmbVals.Text = "(変数)";
			this.cmbVals.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbInsertion_KeyPress);
			this.cmbVals.Enter += new System.EventHandler(this.cmbVals_Enter);
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
			this.cmbFunction.Text = "(関数)";
			this.cmbFunction.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbFunction_KeyPress);
			this.cmbFunction.Enter += new System.EventHandler(this.cmbFunction_Enter);
			// 
			// cmbNames
			// 
			this.cmbNames.DropDownWidth = 400;
			this.cmbNames.Location = new System.Drawing.Point(128, 0);
			this.cmbNames.Name = "cmbNames";
			this.cmbNames.Size = new System.Drawing.Size(121, 20);
			this.cmbNames.TabIndex = 1;
			this.cmbNames.Text = "(チップ名)";
			this.cmbNames.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmbInsertion_KeyPress);
			this.cmbNames.Enter += new System.EventHandler(this.cmbNames_Enter);
			// 
			// txtScript
			// 
			this.txtScript.AcceptsReturn = true;
			this.txtScript.AcceptsTab = true;
			this.txtScript.AllowDrop = true;
			this.txtScript.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtScript.Font = new System.Drawing.Font("ＭＳ ゴシック", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(128)));
			this.txtScript.Location = new System.Drawing.Point(0, 24);
			this.txtScript.MaxLength = 0;
			this.txtScript.Multiline = true;
			this.txtScript.Name = "txtScript";
			this.txtScript.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtScript.Size = new System.Drawing.Size(352, 225);
			this.txtScript.TabIndex = 3;
			this.txtScript.Text = "txtScript";
			this.txtScript.TextChanged += new System.EventHandler(this.txtScript_TextChanged);
			// 
			// frmScript
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(352, 249);
			this.Controls.Add(this.txtScript);
			this.Controls.Add(this.panelInsertion);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Menu = this.mainMenu1;
			this.MinimizeBox = false;
			this.Name = "frmScript";
			this.Text = "Script{...}/Lua{...}設定";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmScript_Closing);
			this.Load += new System.EventHandler(this.frmScript_Load);
			this.VisibleChanged += new System.EventHandler(this.frmScript_VisibleChanged);
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
			modified = false;
			data.script = txtScript.Text;
		}

		private void miCloseWithSave_Click(object sender, System.EventArgs e) {
			miSave_Click(sender,e);
			this.Hide();
		}

		private void frmScript_VisibleChanged(object sender, System.EventArgs e) {
			if(Visible){
				this.txtScript.Text = data.script;
				this.miScriptFuncs.Checked = !(this.miLuaFuncs.Checked = data.luascript);
			}
			modified = false;
		}

		private void cmbVals_Enter(object sender, System.EventArgs e) {
			cmbVals.Items.Clear();
			foreach(RcVal v in data.vals.list){
				cmbVals.Items.Add(miLuaFuncs.Checked ? v.ValName.ToUpper() : v.ValName);
			}
		}

		private void cmbNames_Enter(object sender, System.EventArgs e) {
			cmbNames.Items.Clear();
			RcChipBase chip;
			for(int i = 0;i < RcData.MaxChipCount;i++){
				chip = data.GetChipFromLib(i);
				if(chip != null && chip.Name != null && chip.Name != "")
					cmbNames.Items.Add(miLuaFuncs.Checked ? chip.Name.ToUpper() : chip.Name);
			}
		}

		private void cmbInsertion_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(e.KeyChar == 13){
				txtScript.SelectedText = ((ComboBox)sender).Text;
				txtScript.Focus();
			}
		}

		private void frmScript_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			DialogResult res;
			if(modified){
				res = MessageBox.Show("スクリプトは変更されています。\n変更を適用しますか？","ウィンドウが閉じられようとしています",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question);
				switch(res){
					case DialogResult.Yes:
						miSave_Click(sender,EventArgs.Empty);
						goto case DialogResult.No;
					case DialogResult.No:
						e.Cancel = true;
						this.Hide();
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}else{
				e.Cancel = true;
				this.Hide();
			}
		}

		private void txtScript_TextChanged(object sender, System.EventArgs e) {
			modified = true;
		}

		private void cmbFunction_Enter(object sender, System.EventArgs e) {
			string prevCD = System.IO.Directory.GetCurrentDirectory();
			System.IO.Directory.SetCurrentDirectory(Application.StartupPath);
			cmbFunction.Items.Clear();
			System.IO.StreamReader file;
			int i = 0;
			bool inputswitch = false;
			string input;
			if(miScriptFuncs.Checked){
				file = new System.IO.StreamReader("scriptfunc.dat",System.Text.Encoding.Default);
				while( (input = file.ReadLine()) != null){
					if(i < functionCategory.Length && input == functionCategory[i]){
						inputswitch = functionCategoryEnable[i];
						i++;
					}
					else if(inputswitch)
						cmbFunction.Items.Add(input);
				}
			}
			else{
				file = new System.IO.StreamReader("luafunc.dat",System.Text.Encoding.Default);
				while( (input = file.ReadLine()) != null){
					if(i < functionCategory.Length && input == functionCategory[i]){
						inputswitch = functionCategoryEnable[i];
						i++;
					}
					else if(inputswitch)
						cmbFunction.Items.Add(input);
				}
			}
			System.IO.Directory.SetCurrentDirectory(prevCD);
		}

		private void miCatEnable_Click(object sender, System.EventArgs e) {
			MenuItem item = (MenuItem)sender;
			for(int i = 0;i < functionCategory.Length; i++){
				if(functionCategory[i] == "#" + item.Text)
					functionCategoryEnable[i] = item.Checked = !item.Checked;
			}
		}

		private void cmbFunction_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if(e.KeyChar == 13){
				txtScript.SelectedText = ((ComboBox)sender).Text.Split(':')[0];
				txtScript.Focus();
			}
		}

		private void miLuaFuncs_Click(object sender, System.EventArgs e) {
			if(miLuaFuncs.Checked == false){
				if(MessageBox.Show("スクリプト形式を変更しても、コードは変換されません。\n\nそれでも変更しますか？","スクリプト形式変更確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
					return;
			}
			miScriptFuncs.Checked = false;
			data.luascript = miLuaFuncs.Checked = true;
		}

		private void miScriptFuncs_Click(object sender, System.EventArgs e) {
			if(miScriptFuncs.Checked == false){
				if(MessageBox.Show("スクリプト形式を変更しても、コードは変換されません。\n\nそれでも変更しますか？","スクリプト形式変更確認",MessageBoxButtons.OKCancel,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
					return;
			}
			miScriptFuncs.Checked = true;
			data.luascript = miLuaFuncs.Checked = false;
		}

		private void frmScript_Load(object sender, System.EventArgs e) {
			try{
				System.IO.StreamReader file = new System.IO.StreamReader(Application.StartupPath + "\\func.cfg");

				int val = int.Parse(file.ReadLine());

				for(int i = 0;i < functionCategory.Length;i++){
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
			}
			catch{}
		}

		~frmScript(){
			System.IO.StreamWriter file = new System.IO.StreamWriter(Application.StartupPath + "\\func.cfg");

			int val = 0;
			for(int i = 0;i < functionCategory.Length;i++){
				val |= (functionCategoryEnable[i] ? 1 : 0) << i;
			}
			file.Write(val.ToString());

			file.Close();
		}


	}
}
