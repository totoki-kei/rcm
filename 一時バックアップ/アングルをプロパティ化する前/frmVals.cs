using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using RigidChips;
namespace manageddx
{
	/// <summary>
	/// frmVals の概要の説明です。
	/// </summary>
	public class frmVals : System.Windows.Forms.Form
	{
		RcValList vallist;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListBox lstVals;
		private System.Windows.Forms.TextBox txtDefault;
		private System.Windows.Forms.TextBox txtMin;
		private System.Windows.Forms.TextBox txtMax;
		private System.Windows.Forms.TextBox txtStep;
		private System.Windows.Forms.CheckBox chkDisp;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnDel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// 必要なデザイナ変数です。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmVals(RcValList list)
		{
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent 呼び出しの後に、コンストラクタ コードを追加してください。
			//
			vallist = list;
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
			this.lstVals = new System.Windows.Forms.ListBox();
			this.txtDefault = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtMin = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtMax = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.txtStep = new System.Windows.Forms.TextBox();
			this.chkDisp = new System.Windows.Forms.CheckBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnDel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lstVals
			// 
			this.lstVals.ItemHeight = 12;
			this.lstVals.Location = new System.Drawing.Point(8, 16);
			this.lstVals.Name = "lstVals";
			this.lstVals.Size = new System.Drawing.Size(128, 208);
			this.lstVals.TabIndex = 0;
			// 
			// txtDefault
			// 
			this.txtDefault.Location = new System.Drawing.Point(240, 24);
			this.txtDefault.Name = "txtDefault";
			this.txtDefault.Size = new System.Drawing.Size(64, 19);
			this.txtDefault.TabIndex = 1;
			this.txtDefault.Text = "textBox1";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(144, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 24);
			this.label1.TabIndex = 2;
			this.label1.Text = "初期値(Default)";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 24);
			this.label2.TabIndex = 4;
			this.label2.Text = "最小値(Min)";
			// 
			// txtMin
			// 
			this.txtMin.Location = new System.Drawing.Point(240, 64);
			this.txtMin.Name = "txtMin";
			this.txtMin.Size = new System.Drawing.Size(64, 19);
			this.txtMin.TabIndex = 3;
			this.txtMin.Text = "textBox2";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(144, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 24);
			this.label3.TabIndex = 6;
			this.label3.Text = "最大値(Max)";
			// 
			// txtMax
			// 
			this.txtMax.Location = new System.Drawing.Point(240, 104);
			this.txtMax.Name = "txtMax";
			this.txtMax.Size = new System.Drawing.Size(64, 19);
			this.txtMax.TabIndex = 5;
			this.txtMax.Text = "textBox3";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(144, 144);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 24);
			this.label4.TabIndex = 8;
			this.label4.Text = "復帰ステップ量(Step)";
			// 
			// txtStep
			// 
			this.txtStep.Location = new System.Drawing.Point(240, 144);
			this.txtStep.Name = "txtStep";
			this.txtStep.Size = new System.Drawing.Size(64, 19);
			this.txtStep.TabIndex = 7;
			this.txtStep.Text = "textBox4";
			// 
			// chkDisp
			// 
			this.chkDisp.Location = new System.Drawing.Point(176, 184);
			this.chkDisp.Name = "chkDisp";
			this.chkDisp.Size = new System.Drawing.Size(112, 32);
			this.chkDisp.TabIndex = 9;
			this.chkDisp.Text = "画面に表示しない(Disp)";
			// 
			// btnAdd
			// 
			this.btnAdd.Location = new System.Drawing.Point(8, 232);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(56, 24);
			this.btnAdd.TabIndex = 10;
			this.btnAdd.Text = "追加";
			// 
			// btnDel
			// 
			this.btnDel.Location = new System.Drawing.Point(80, 232);
			this.btnDel.Name = "btnDel";
			this.btnDel.Size = new System.Drawing.Size(56, 24);
			this.btnDel.TabIndex = 11;
			this.btnDel.Text = "削除";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(168, 232);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(80, 24);
			this.btnOK.TabIndex = 12;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(256, 232);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 24);
			this.btnCancel.TabIndex = 13;
			this.btnCancel.Text = "キャンセル";
			// 
			// frmVals
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(346, 263);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnDel);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.chkDisp);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtStep);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtMax);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtMin);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtDefault);
			this.Controls.Add(this.lstVals);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmVals";
			this.Text = "frmVals";
			this.ResumeLayout(false);

		}
		#endregion

		private void textBox4_TextChanged(object sender, System.EventArgs e) {
		
		}

	}
}
