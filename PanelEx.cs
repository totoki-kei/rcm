using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace rcm {
	class PanelEx : Panel {
		protected override bool IsInputKey(Keys keyData) {
			return true;
		}
	}
}
