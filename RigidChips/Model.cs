using System;

namespace RigidChips {
	/// <summary>
	/// RigidChipsモデルデータ
	/// </summary>
	public class Model{
		Environment gen;
		public ChipBase root;
		public Model(Environment gen){
			this.gen = gen;
			root = new CoreChip(gen,null,JointPosition.NULL);
		}
		/// <summary>
		/// モデル内の特定のチップを得る。
		/// </summary>
		public ChipBase this[int idx]{
			get{
				ChipBase buff = root.GetChildChip(ref idx);
				if(buff != null)
					return buff;
				else
					throw new Exception("インデックス値が範囲を超えています。");
			}
/*			set{
				RcChipBase t = root.GetChildChip(ref idx),p;
				if(t == null)
					throw new Exception("インデックス値が範囲を超えています。");

				int sabun = 0;
				p = t.Parent;
				for(int i = 0;i < RcData.ChildCapasity;i++){
					if(p.Child[i] == t){
						sabun = value.ChipCount - p.Child[i].ChipCount;
						gen.chipCount += sabun;
						p.Child[i] = value;
						return;
					}
				}
				
				throw new Exception("RcChipBase[]で予期せぬエラーが発生しました。");
			}	*/
		}
		/// <summary>
		/// モデル内の、特定の名前が付いたチップを得る。見つからない場合はnull.
		/// </summary>
		public ChipBase this[string name]{
			get{
				return null;	// 未実装
			}
		}

		public override string ToString() {
			return "Body{\r\n" + root.ToString(1) + "\r\n}\r\n";
		}

		public void Parse(string input){
			this.root = ModelParser.Parse(input,(CoreChip)this.root,this.gen);
		}

		public void ForEach(Action<ChipBase> action) {
			void Apply(ChipBase c, Action<ChipBase> a) {
				a(c);
				foreach(var ch in c.Children) {
					if (ch != null) Apply(ch, action);
				}
			}

			Apply(root, action);
		}
	}
}
