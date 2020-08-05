using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using RigidChips;
using System.Collections.Generic;
using System.Diagnostics;

namespace rcm {
	/// <summary>
	/// ツリー表示ダイアログ
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

		public frmTree(RcData rcdata,ContextMenu chipmenu) {
			initializing = true;
			//
			// Windows フォーム デザイナ サポートに必要です。
			//
			InitializeComponent();

			datasource = rcdata;
			ctmChip = chipmenu;

			GenerateTree();

		}

		public void GenerateTree(){
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

			//// ルートチップをキューに入れる
			//p.Enqueue(datasource.model.root);
			//q.Enqueue(datasource.model.root);

			//// qに対して、チップのツリーを展開していく
			//while(p.Count > 0){
			//    w = p.Dequeue();
			//    foreach(RcChipBase c in w.Child){
			//        if(c == null)break;
			//        q.Enqueue(c);
			//        p.Enqueue(c);
			//    }
			//    q.Enqueue(null);
			//}

			//// 展開されたチップリストからツリーを再構築
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

	//	[Obsolete("未完成です。ずっと日の目を見ないかも。")]
		public void UpdateTree(RcChipBase updateRoot) {
			RcTreeNode root = ((RcTreeNode)tvModel.Nodes[0]).Find(updateRoot);

			if (root == null) {
				// まだないチップ -> 親をアップデート
				if (updateRoot.Parent == null) {
					GenerateTree();
					return;
				}
				Debug.WriteLine(updateRoot.Parent, "Parent Update");
				UpdateTree(updateRoot.Parent);
				root = ((RcTreeNode)tvModel.Nodes[0]).Find(updateRoot);
				if (root == null) throw new ArgumentException("ツリービューの更新に失敗しました。");
			}

			int childCount = 0;
			for (int i = 0; i < RcData.ChildCapasity; i++) {
				if (updateRoot.Child[i] == null)continue;
				childCount++;
				var childChip = updateRoot.Child[i];
				var childNode = root.Find(childChip);
				int nodeIndex = root.Nodes.IndexOf(childNode);

				if (childNode == null) {
					// 追加
					RcTreeNode n;
					root.Nodes.Insert(i, n = new RcTreeNode(childChip));
					Debug.WriteLine(n, "New node");
				}
				else if (nodeIndex  != i) {
					// 間にあるノードを削除
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
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose(bool disposing) {
			if( disposing ) {
				if(components != null) {
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
			this.menuItem1.Text = "テストです。";
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
			this.Text = "ツリー構造";
			this.Load += new System.EventHandler(this.frmTree_Load);
			this.Activated += new System.EventHandler(this.frmTree_Activated);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmTree_Load(object sender, System.EventArgs e) {
		
		}

		private void tvModel_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Right){
				tvModel_BeforeSelect(sender,new TreeViewCancelEventArgs(tvModel.GetNodeAt(e.X,e.Y),false,TreeViewAction.ByMouse));
			}
		}

		private void menuItem1_Click(object sender, System.EventArgs e) {
			datasource.SelectedChip.Comment = "ツリーでコンテキスト";
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
			if(e.Button == MouseButtons.Right){
				ctmChip.Show(tvModel,new Point(e.X,e.Y));
			}
		}

		private void tvModel_BeforeExpand(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			if(((RcTreeNode)e.Node).ChipType == RcChipType.Cowl && initializing)
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
					MessageBox.Show("派生チップにカウル以外のチップが存在します。すべてカウルに変更しますか？", "ツリービュー編集", MessageBoxButtons.YesNo)
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

	public class RcTreeNode : TreeNode{
		RcChipBase c;
		public RcTreeNode(RcChipBase chip) : base(){
			this.Chip = chip;
			this.ImageIndex = this.SelectedImageIndex = (int)RcChipBase.CheckType(chip);
			if (Array.Exists(chip.Child, x => x != null))
				foreach (var c in chip.Child)
					if (c != null) this.Nodes.Add(new RcTreeNode(c));

			
		}

		

		public RcChipBase Chip{
			get{
				return c;
			}
			set{
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

		public RcChipType ChipType{
			get{
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
