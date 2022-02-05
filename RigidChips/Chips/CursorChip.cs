using System;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace RigidChips {
	#region システム用

	///<summery>
	///カーソル クラス
	///</summery>
	public class CursorChip : ChipBase{
		XFile mesh;
		ChipAttribute backColor;

		public CursorChip(Environment gen,ChipBase parent,JointPosition pos) : base(gen,parent,pos){
			this.Attach(parent,JointPosition.NULL);
			mesh = Environment.GetMesh(Application.StartupPath + "\\Resources\\Cursor.x",true);
			matRotation = Matrix.Identity;
			backColor = new ChipAttribute(0);
			ChipColor.SetValue(Environment.DrawOption.CursorFrontColor.ToArgb());
			backColor.SetValue(Environment.DrawOption.CursorBackColor.ToArgb());
			GuideEnabled = true;
		}

		public void SetCursorColor(Color front,Color back){
			ChipColor.SetValue(front.ToArgb());
			backColor.SetValue(back.ToArgb());
		}

		public override void Add(JointPosition joint, ChipBase chip,bool Registeration) {
			if(chip is GuideChip){
				for(int i = 0;i < Environment.ChildCapasity;i++){
					if(Children[i] == null){
						Children[i] = chip;
						Children[i].JointPosition = joint;
						Children[i].Parent = this;
						Children[i].UpdateMatrix();
						return;
					}
				}
				return;
			}
			throw new Exception("RcChipCursorに追加できるのはRcChipGuideクラスのインスタンスのみです。");
		}

		public override void Attach(ChipBase to, JointPosition pos) {
			this.Parent = to;	// 一方的な参照であり、Attach先のチップには影響を及ぼさない。
								// これにより、cursor.Parentで選択されたチップを取得できる。
			UpdateMatrix();
		}


		public override string AttrTip(string AttrName) {
			return "!RcChipCursor has no attributes.";
		}

		public override ChipType ChipType => ChipType.SystemCursor;

		public override void DrawChip() {
			if(Parent == null)return;
			Color c = ChipColor.ToColor();
			//			if(c.A > 127)
			//				c = Color.FromArgb(127,c);
			if(mesh == null)return;
			mesh.Draw(Environment.d3ddevice,c,0x7000,matRotation * Matrix);
			c = backColor.ToColor();
			mesh.Draw(Environment.d3ddevice,c,0x2000,Matrix.RotationZ((float)Math.PI) * matRotation * Matrix);
			
		}

		public override void UpdateMatrix() {
			if(Parent == null)return;
			this.matTranslation = Parent.Matrix;
			this.matVersion = System.DateTime.Now.Ticks;

			foreach(ChipBase c in Children)if(c != null){
											  c.UpdateMatrix();
										  }

		}

		//		public override string ToString(int tabs){
		//			string s = "";
		//			for(int i = 0;i < tabs;i++)s += "\t";
		//			return s + "//C:Guide(){}\n";		//	派生チップは持たない
		//		}
		//
		//		public override string ToString() {
		//			return "//" + "Guide()";
		//		}


		public bool GuideEnabled { get; set; }
		public HitStatus IsHit(int X, int Y, int ScrWidth, int ScrHeight, bool guideEnabled) {
			HitStatus dist, buff;
			dist.distance = float.MaxValue;
			dist.HitChip = null;
			if (guideEnabled) {
				foreach (ChipBase c in Children) {
					if (c != null) {
						buff = c.IsHit(X, Y, ScrWidth, ScrHeight);
						if (dist.distance > buff.distance) {
							dist = buff;
						}
					}
				}
			}
			// 投影行列
			Matrix projMat = Environment.d3ddevice.Transform.Projection;
			// ビュー行列
			Matrix viewMat = Environment.d3ddevice.Transform.View;
			// ビューポート
			Viewport viewport = new Viewport();

			IntersectInformation sectinfo = new IntersectInformation();

			viewport.Width = ScrWidth;
			viewport.Height = ScrHeight;
			viewport.X = viewport.Y = 0;
			viewport.MaxZ = 1.0f;
			viewport.MinZ = 0.0f;

			// クリックしたスクリーン座標からレイを計算し、対象メッシュとの交差をチェック 
			Vector3 vNear = Vector3.Unproject(new Vector3(X, Y, viewport.MinZ),
				viewport, projMat, viewMat, this.Matrix /* ここがWorld行列 */);
			Vector3 vFar = Vector3.Unproject(new Vector3(X, Y, viewport.MaxZ),
				viewport, projMat, viewMat, this.Matrix /* ここがWorld行列 */);
			Vector3 vDir = Vector3.Normalize(vFar - vNear);

			buff.distance = (Environment.imesh.Intersect(vNear, vDir, out sectinfo)) ? sectinfo.Dist : float.MaxValue;
			if (dist.distance > buff.distance) {
				dist.distance = buff.distance;
				dist.HitChip = this;
			}

			return dist;
		}

	}
	#endregion
}

