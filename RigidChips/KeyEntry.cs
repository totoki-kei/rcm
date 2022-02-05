namespace RigidChips {
	/// <summary>
	/// キー設定情報
	/// </summary>
	public class KeyEntry{
		public struct KeyEntryWork{
			public ValEntry Target;
			public float Step;

			public override string ToString() {
				return Target.ValName + string.Format("(Step={0:G})",Step);
			}

		}

		~KeyEntry(){
			foreach(KeyEntryWork kw in Works){
				kw.Target.RefCount --;
			}
		}

		public byte Key_ID;
		public KeyEntryWork[] Works;

		public KeyEntry(){
			Works = new KeyEntryWork[0];
		}

		/// <summary>
		/// キーに変数を関連づける。すでに関連づけられている場合はステップを変更する。
		/// </summary>
		/// <param name="Target">ターゲットとなるRcValへの参照。</param>
		/// <param name="Step">ステップ値。</param>
		public void AssignWork(ValEntry Target,float Step){
			for(int i = 0;i < Works.Length;i++){
				if(Works[i].Target == Target){
					Works[i].Step = Step;
					return;
				}
			}
			Works.CopyTo(Works = new KeyEntryWork[Works.Length + 1],0);
			Works[Works.Length-1].Target = Target;
			Works[Works.Length-1].Step = Step;
			Target.RefCount ++;
		}

		/// <summary>
		/// 変数のキーへの関連づけを削除する。
		/// </summary>
		/// <param name="Target">削除するRcValへの参照。</param>
		public void DeleteWork(ValEntry Target){
			bool removed = false;
			KeyEntryWork[] buff = new KeyEntryWork[Works.Length -1];
			for(int i = 0;i < Works.Length;i++){
				if(removed)
					buff[i-1] = Works[i];

				else if(Works[i].Target == Target)
					removed = true;

				else
					buff[i] = Works[i];
			}

			Works = buff;

			Target.RefCount--;

		}

		/// <summary>
		/// キーの動作を表す文字列を得る。
		/// </summary>
		/// <returns>Keyブロックで使用可能なこのインスタンスの完全文字列。</returns>
		public override string ToString() {
			if(Works.Length == 0)return null;
			string str;
			str = Key_ID.ToString() + ":";
			foreach(KeyEntryWork kw in Works){
				str += kw.Target.ValName + "(step=" + kw.Step.ToString("G9") + "),";
			}
			str = str.TrimEnd(',');
			return str;
		}

	}
}
