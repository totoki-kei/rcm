namespace rcm {
	/// <summary>
	/// ���_�𒼐ڒl�Őݒ肷�邽�߂̃_�C�A���O�A�̗\��ł����B
	/// </summary>
	public class dlgView : System.Windows.Forms.Form {
		private System.Windows.Forms.TextBox txtTheta;
		private System.Windows.Forms.TextBox txtPhi;
		private System.Windows.Forms.TextBox txtDepth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label4;
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		public dlgView() {
			//
			// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B
			//
			InitializeComponent();

			//
			// TODO: InitializeComponent �Ăяo���̌�ɁA�R���X�g���N�^ �R�[�h��ǉ����Ă��������B
			//
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
			this.txtTheta = new System.Windows.Forms.TextBox();
			this.txtPhi = new System.Windows.Forms.TextBox();
			this.txtDepth = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtTheta
			// 
			this.txtTheta.Location = new System.Drawing.Point(72, 16);
			this.txtTheta.Name = "txtTheta";
			this.txtTheta.Size = new System.Drawing.Size(56, 19);
			this.txtTheta.TabIndex = 0;
			this.txtTheta.Text = "textBox1";
			// 
			// txtPhi
			// 
			this.txtPhi.Location = new System.Drawing.Point(72, 40);
			this.txtPhi.Name = "txtPhi";
			this.txtPhi.Size = new System.Drawing.Size(56, 19);
			this.txtPhi.TabIndex = 1;
			this.txtPhi.Text = "textBox2";
			// 
			// txtDepth
			// 
			this.txtDepth.Location = new System.Drawing.Point(72, 88);
			this.txtDepth.Name = "txtDepth";
			this.txtDepth.Size = new System.Drawing.Size(56, 19);
			this.txtDepth.TabIndex = 2;
			this.txtDepth.Text = "textBox3";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "��]�p";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 40);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "�㉺�p";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "�J��������";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(40, 120);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 24);
			this.button1.TabIndex = 6;
			this.button1.Text = "OK";
			// 
			// label4
			// 
			this.label4.Enabled = false;
			this.label4.Location = new System.Drawing.Point(48, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "���ǂ�������W�A��";
			// 
			// dlgView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(146, 151);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtDepth);
			this.Controls.Add(this.txtPhi);
			this.Controls.Add(this.txtTheta);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "dlgView";
			this.Text = "���_����";
			this.Load += new System.EventHandler(this.dlgView_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void dlgView_Load(object sender, System.EventArgs e) {

		}
	}
}
