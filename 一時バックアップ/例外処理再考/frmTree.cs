using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using RigidChips;
using System.Collections.Generic;

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
			initializing = true;
			tvModel.SuspendLayout();
			tvModel.Nodes.Clear();
			var p = new Queue<RcChipBase>();
			var q = new Queue<RcChipBase>();
			var r = new Queue<RcTreeNode>();
			RcChipBase w;
			RcTreeNode n;
			RcTreeNode top;

			// ルートチップをキューに入れる
			p.Enqueue(datasource.model.root);
			q.Enqueue(datasource.model.root);

			// qに対して、チップのツリーを展開していく
			while(p.Count > 0){
				w = p.Dequeue();
				foreach(RcChipBase c in w.Child){
					if(c == null)break;
					q.Enqueue(c);
					p.Enqueue(c);
				}
				q.Enqueue(null);
			}

			// 展開されたチップリストからツリーを再構築
			r.Enqueue(top = new RcTreeNode(q.Dequeue()));
			while(q.Count > 0){
				w = q.Dequeue();
				if(w == null){
					if((n = r.Dequeue()).ChipType != RcChipType.Cowl)
						n.Expand();
				}
				else{
					n = new RcTreeNode(w);
					r.Enqueue(n);
					r.Peek().Nodes.Add(n);
				}
			}

			tvModel.Nodes.Add(top);
			top.Expand();
			tvModel.ResumeLayout(true);

			initializing = false;
		}

		[Obsolete("未完成です。ずっと日の目を見ないかも。")]
		public void UpdateTree(RcChipBase updateRoot) {
			RcTreeNode root = tvModel.Nodes[0] as RcTreeNode;
			if (root.Chip == updateRoot) {
				// アップデート対象がコアだった場合は完全再構築
				GenerateTree();
			}

			Stack<RcChipBase> treepath = new Stack<RcChipBase>();
			var c = updateRoot;
			while (c != null) {
				treepath.Push(c);
				c = c.Parent;
			}
			RcTreeNode targetNode = root;
			while (treepath.Count > 0) {
				var chip = treepath.Pop();
				bool flag = false;
				foreach (var n in targetNode.Nodes) {
					var rtn = (RcTreeNode)n;
					if (rtn.Chip == chip) {
						targetNode = rtn;
						flag = true;
						break;
					}
				}
				if (!flag) throw new ApplicationException("ツリーの更新に失敗しました");
			}

			targetNode.Chip = updateRoot;
			
		}

		/// <summary>
		/// 使用されているリソースに後処理を実行します。
		/// </summary>
		protected override void Dispose( bool disposing ) {
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmTree));
			this.tvModel = new System.Windows.Forms.TreeView();
			this.imgIcons = new System.Windows.Forms.ImageList(this.components);
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// tvModel
			// 
			this.tvModel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvModel.ImageList = this.imgIcons;
			this.tvModel.Location = new System.Drawing.Point(0, 0);
			this.tvModel.Name = "tvModel";
			this.tvModel.Size = new System.Drawing.Size(292, 461);
			this.tvModel.TabIndex = 0;
			this.tvModel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvModel_MouseDown);
			this.tvModel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvModel_MouseUp);
			this.tvModel.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvModel_BeforeSelect);
			// 
			// imgIcons
			// 
			this.imgIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imgIcons.ImageSize = new System.Drawing.Size(16, 16);
			this.imgIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgIcons.ImageStream")));
			this.imgIcons.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(0)), ((System.Byte)(255)));
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
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "frmTree";
			this.Text = "ツリー構造";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.frmTree_Load);
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
			if(initializing)return;
			datasource.SelectedChip = ((RcTreeNode)e.Node).Chip;
			DialogResult = DialogResult.OK;
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


	}

	public class RcTreeNode : TreeNode{
		RcChipBase c;
		public RcTreeNode(RcChipBase chip) : base(){
			this.Chip = chip;
			this.ImageIndex = this.SelectedImageIndex = (int)RcChipBase.CheckType(chip);
		}

		public RcChipBase Chip{
			get{
				return c;
			}
			set{
				c = value;
				string s;
				switch(c.JointPosition){
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
		}

		public RcChipType ChipType{
			get{
				return (RcChipType)ImageIndex;
			}
		}

	}
	
}
