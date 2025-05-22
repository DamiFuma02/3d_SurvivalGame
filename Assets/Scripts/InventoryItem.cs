using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts {


    [Serializable]
    public class InventoryItem {
        public string name;   //unique name
        public int quantity;

        public InventoryItem(string name, int quantity=1) {
            this.name = name;
            this.quantity = quantity;
        }
    }
}
