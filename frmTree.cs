using RigidChips;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace rcm {
	/// <summary>
	/// �c���[�\���_�C�A���O
	/// </summary>
	public class frmTree : System.Windows.Forms.Form {
		RcData datasource;
		bool initializing;

		private System.Windows.Forms.ContextMenu ctmChip;
		private System.Windows.Forms.TreeView tvModel;
		private System.Windows.Forms.ImageList imgIcons;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.ComponentModel.IContainer components;

		public frmTree(RcData rcdata, ContextMenu chipmenu) {
			initializing = true;
			//
			// Windows �t�H�[�� �f�U�C�i �T�|�[�g�ɕK�v�ł��B
			//
			InitializeComponent();

			datasource = rcdata;
			ctmChip = chipmenu;

			GenerateTree();

		}

		public void GenerateTree() {
			Debug.WriteLine("GenerateTree");
			initializing = true;
			tvModel.SuspendLayout();
			tvModel.Nodes.Clear();
			//var p = new Queue<RcChipBase>();
			//var q = new Queue<RcChipBase>();
			//var r = new Queue<RcTreeNode>();
			//RcChipBase w;
			//RcTreeNode n;
			RcTreeNode top = new RcTreeNode(datasource.model.root);

			//// ���[�g�`�b�v���L���[�ɓ����
			//p.Enqueue(datasource.model.root);
			//q.Enqueue(datasource.model.root);

			//// q�ɑ΂��āA�`�b�v�̃c���[��W�J���Ă���
			//while(p.Count > 0){
			//    w = p.Dequeue();
			//    foreach(RcChipBase c in w.Child){
			//        if(c == null)break;
			//        q.Enqueue(c);
			//        p.Enqueue(c);
			//    }
			//    q.Enqueue(null);
			//}

			//// �W�J���ꂽ�`�b�v���X�g����c���[���č\�z
			//r.Enqueue(top = new RcTreeNode(q.Dequeue()));
			//while(q.Count > 0){
			//    w = q.Dequeue();
			//    if(w == null){
			//        if((n = r.Dequeue()).ChipType != RcChipType.Cowl)
			//            n.Expand();
			//    }
			//    else{
			//        n = new RcTreeNode(w);
			//        r.Enqueue(n);
			//        r.Peek().Nodes.Add(n);
			//    }
			//}

			tvModel.Nodes.Add(top);
			top.Expand();
			tvModel.ResumeLayout(true);

			initializing = false;
		}

		//	[Obsolete("�������ł��B�����Ɠ��̖ڂ����Ȃ������B")]
		public void UpdateTree(RcChipBase updateRoot) {
			RcTreeNode root = ((RcTreeNode)tvModel.Nodes[0]).Find(updateRoot);

			if (root == null) {
				// �܂��Ȃ��`�b�v -> �e���A�b�v�f�[�g
				if (updateRoot.Parent == null) {
					GenerateTree();
					return;
				}
				Debug.WriteLine(updateRoot.Parent, "Parent Update");
				UpdateTree(updateRoot.Parent);
				root = ((RcTreeNode)tvModel.Nodes[0]).Find(updateRoot);
				if (root == null) throw new ArgumentException("�c���[�r���[�̍X�V�Ɏ��s���܂����B");
			}

			int childCount = 0;
			for (int i = 0; i < RcData.ChildCapasity; i++) {
				if (updateRoot.Child[i] == null) continue;
				childCount++;
				var childChip = updateRoot.Child[i];
				var childNode = root.Find(childChip);
				int nodeIndex = root.Nodes.IndexOf(childNode);

				if (childNode == null) {
					// �ǉ�
					RcTreeNode n;
					root.Nodes.Insert(i, n = new RcTreeNode(childChip));
					Debug.WriteLine(n, "New node");
				}
				else if (nodeIndex != i) {
					// �Ԃɂ���m�[�h���폜
					List<TreeNode> nodesToDelete = new List<TreeNode>();
					for (int j = i; j < nodeIndex; j++) {
						nodesToDelete.Add(root.Nodes[j]);
					}
					foreach (var n in nodesToDelete) {
						root.Nodes.Remove(n);
						Debug.WriteLine(n, "Delete node");
					}
				}
			}

			Debug.WriteLine(root, "Update text");
			root.UpdateText();
			if (updateRoot is RcChipCowl)
				root.Collapse();
			else
				root.Expand();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTree));
			this.tvModel = new System.Windows.Forms.TreeView();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// tvModel
			// 
			this.tvModel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvModel.ImageIndex = 0;
			this.tvModel.ImageList = this.imgIcons;
			this.tvModel.LabelEdit = true;
			this.tvModel.Location = new System.Drawing.Point(0, 0);
			this.tvModel.Name = "tvModel";
			this.tvModel.SelectedImageIndex = 0;
			this.tvModel.Size = new System.Drawing.Size(292, 461);
			this.tvModel.TabIndex = 0;
			this.tvModel.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvModel_AfterLabelEdit);
			this.tvModel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvModel_MouseUp);
			this.tvModel.Enter += new System.EventHandler(this.treeview_Enter);
			this.tvModel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvModel_MouseDown);
			this.tvModel.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvModel_NodeMouseClick);
			this.tvModel.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvModel_BeforeSelect);
			// 
			// imgIcons
			// 
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
			this.imgIcons.Images.SetKeyName(0, "");
			this.imgIcons.Images.SetKeyName(1, "");
			this.imgIcons.Images.SetKeyName(2, "");
			this.imgIcons.Images.SetKeyName(3, "");
			this.imgIcons.Images.SetKeyName(4, "");
			this.imgIcons.Images.SetKeyName(5, "");
			this.imgIcons.Images.SetKeyName(6, "");
			this.imgIcons.Images.SetKeyName(7, "");
			this.imgIcons.Images.SetKeyName(8, "");
			this.imgIcons.Images.SetKeyName(9, "");
			this.imgIcons.Images.SetKeyName(10, "");
			this.imgIcons.Images.SetKeyName(11, "");
			this.imgIcons.Images.SetKeyName(12, "");
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "�e�X�g�ł��B";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// frmTree
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 12);
			this.ClientSize = new System.Drawing.Size(292, 461);
			this.Controls.Add(this.tvModel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmTree";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "�c���[�\��";
			this.Load += new System.EventHandler(this.frmTree_Load);
			this.Activated += new System.EventHandler(this.frmTree_Activated);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmTree_Load(object sender, System.EventArgs e) {

		}

		private void tvModel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				tvModel_BeforeSelect(sender, new TreeViewCancelEventArgs(tvModel.GetNodeAt(e.X, e.Y), false, TreeViewAction.ByMouse));
			}
		}

		private void menuItem1_Click(object sender, System.EventArgs e) {
			datasource.SelectedChip.Comment = "�c���[�ŃR���e�L�X�g";
		}

		private void tvModel_BeforeSelect(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			//if(initializing)return;
			//if (((RcTreeNode)e.Node).Chip is RcChipCore) {
			//    Debug.WriteLine("Core selected", "tvModel_BeforeSelect");
			//}
			//datasource.SelectedChip = ((RcTreeNode)e.Node).Chip;
			//Debug.WriteLine(((RcTreeNode)e.Node).Chip.ToString(), "tvModel_BeforeSelect");
			//DialogResult = DialogResult.OK;
		}

		private void tvModel_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			if (e.Button == MouseButtons.Right) {
				ctmChip.Show(tvModel, new Point(e.X, e.Y));
			}
		}

		private void tvModel_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			if (((RcTreeNode)e.Node).ChipType == RcChipType.Cowl && initializing)
				e.Cancel = true;
		}

		private void tvModel_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
			if (initializing) return;
			if (((RcTreeNode)e.Node).Chip is RcChipCore) {
				Debug.WriteLine("Core selected", "tvModel_BeforeSelect");
			}
			datasource.SelectedChip = ((RcTreeNode)e.Node).Chip;
			Debug.WriteLine(((RcTreeNode)e.Node).Chip.ToString(), "tvModel_BeforeSelect");
			DialogResult = DialogResult.OK;

		}

		private void treeview_Enter(object sender, EventArgs e) {
			//initializing = true;
			tvModel.SelectedNode = ((RcTreeNode)tvModel.Nodes[0]).Find(datasource.SelectedChip);
			//initializing = false;
		}

		private void frmTree_Activated(object sender, EventArgs e) {
			treeview_Enter(sender, e);
		}

		private void tvModel_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
#if false
			RcTreeNode rcnode = (RcTreeNode)e.Node;

			RcChipBase c;
			c = RcChipBase.Parse(datasource, e.Label);

			if (c is RcChipCowl && Array.Exists(rcnode.Chip.Child , x => !(x is RcChipCowl)) ) {
				if (
					MessageBox.Show("�h���`�b�v�ɃJ�E���ȊO�̃`�b�v�����݂��܂��B���ׂăJ�E���ɕύX���܂����H", "�c���[�r���[�ҏW", MessageBoxButtons.YesNo)
					== DialogResult.Yes) {

					for (int i = 0; i < RcData.ChildCapasity; i++) {
						rcnode.Chip.Child[i] = rcnode.Chip.Child[i].ChangeType(RcChipType.Cowl);
					}
				}
				else {
				}
			}
#else
			e.CancelEdit = true;
#endif
		}



	}

	public class RcTreeNode : TreeNode {
		RcChipBase c;
		public RcTreeNode(RcChipBase chip) : base() {
			this.Chip = chip;
			this.ImageIndex = this.SelectedImageIndex = (int)RcChipBase.CheckType(chip);
			if (Array.Exists(chip.Child, x => x != null))
				foreach (var c in chip.Child)
					if (c != null) this.Nodes.Add(new RcTreeNode(c));


		}



		public RcChipBase Chip {
			get {
				return c;
			}
			set {
				c = value;
				UpdateText();
			}
		}

		public void UpdateText() {
			string s;
			switch (c.JointPosition) {
				case RcJointPosition.North:
					s = "N:";
					break;
				case RcJointPosition.South:
					s = "S:";
					break;
				case RcJointPosition.East:
					s = "E:";
					break;
				case RcJointPosition.West:
					s = "W:";
					break;
				default:
					s = "";
					break;
			}
			this.Text = s + c.ToString();
			this.ImageIndex = (int)RcChipBase.CheckType(c);
		}

		public RcChipType ChipType {
			get {
				return (RcChipType)ImageIndex;
			}
		}

		public RcTreeNode Find(RcChipBase chip) {
			if (this.Chip == chip) {
				return this;
			}
			else {
				foreach (TreeNode tn in this.Nodes) {
					RcTreeNode rctn = (RcTreeNode)tn;
					RcTreeNode ret = rctn.Find(chip);
					if (ret != null) return ret;
				}
			}
			return null;
		}

	}

}
