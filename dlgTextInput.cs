using System.Windows.Forms;

namespace rcm {
	/// <summary>
	/// ��������͗p�ȈՃ_�C�A���O
	/// </summary>
	public class dlgTextInput : System.Windows.Forms.Form {
		// ���e�����Ɋ֌W���镶����
		private string ignorelist = "";

		// ignorelist�̕����Q�������ΏۂɂȂ�t���O�B
		private bool ignoremode = false;

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// �K�v�ȃf�U�C�i�ϐ��ł��B
		/// </summary>
		private System.ComponentModel.Container components = null;

		private dlgTextInput() {
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(8, 32);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(256, 19);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(157, 15);
			this.label1.TabIndex = 1;
			this.label1.Text = "���������͂��Ȃ������Ă�����";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(208, 56);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(48, 24);
			this.button1.TabIndex = 2;
			this.button1.Text = "OK";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(152, 56);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(48, 24);
			this.button2.TabIndex = 3;
			this.button2.Text = "Cancel";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// dlgTextInput
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(266, 85);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "dlgTextInput";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "��������̓_�C�A���O";
			this.Load += new System.EventHandler(this.dlgTextInput_Load);
			this.ResumeLayout(false);

		}
		#endregion

		///<summery>
		///����������A�������A���͍ő咷���w�肵�āA�_�C�A���O�𔭐�������B
		///maxlength��0�̏ꍇ�A���͉\�������͂Ȃ��B
		///</summery>
		public static string ShowDialog(string defaulttext, string labelcaption, int maxlength) {
			dlgTextInput f = new dlgTextInput();
			if (defaulttext != null) f.textBox1.Text = defaulttext;
			if (maxlength > 0) f.textBox1.MaxLength = maxlength;
			if (labelcaption != null) f.label1.Text = labelcaption;

			string ret = (f.ShowDialog() == DialogResult.OK) ? f.textBox1.Text : null;
			f.Dispose();
			return ret;
		}

		///<summery>
		///����������A�������A���͍ő咷�A���͎��ɔ�������C�x���g�n���h�����w�肵��
		///�_�C�A���O�𔭐�������B
		///</summery>
		public static string ShowDialog(string defaulttext, string labelcaption, int maxlength, KeyPressEventHandler keypresshandler) {
			dlgTextInput f = new dlgTextInput();
			if (defaulttext != null) f.textBox1.Text = defaulttext;
			if (maxlength > 0) f.textBox1.MaxLength = maxlength;
			if (labelcaption != null) f.label1.Text = labelcaption;
			f.textBox1.KeyPress += keypresshandler;

			string ret = (f.ShowDialog() == DialogResult.OK) ? f.textBox1.Text : null;
			f.Dispose();
			return ret;

		}

		///<summery>
		///����������A�������A���͍ő咷�A���e�������X�g���w�肵�āA�_�C�A���O�𔭐�������B
		///ignore��true�̎��A�������X�g�͖����ΏۂƂȂ�B
		///</summery>
		public static string ShowDialog(string defaulttext, string labelcaption, int maxlength, string checkcharlist, bool ignore) {
			dlgTextInput f = new dlgTextInput();
			if (defaulttext != null) f.textBox1.Text = defaulttext;
			if (maxlength > 0) f.textBox1.MaxLength = maxlength;
			if (labelcaption != null) f.label1.Text = labelcaption;
			f.ignorelist = checkcharlist;
			f.ignoremode = ignore;
			f.textBox1.KeyPress += new KeyPressEventHandler(f.textBox1_KeyPress);

			string ret = (f.ShowDialog() == DialogResult.OK) ? f.textBox1.Text : null;
			f.Dispose();
			return ret;
		}

		private void button1_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.OK;
		}

		private void button2_Click(object sender, System.EventArgs e) {
			this.DialogResult = DialogResult.Cancel;
		}

		private void dlgTextInput_Load(object sender, System.EventArgs e) {
		}

		private void textBox1_KeyPress(object sender, KeyPressEventArgs e) {
			int buff = ignorelist.IndexOf(e.KeyChar);
			if (buff >= 0)
				e.Handled = ignoremode;
			else
				e.Handled = !ignoremode;
		}
	}
}
