using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public class Mapping
    {

        public Dictionary<string, string> Items = new Dictionary<string, string>();

        public Mapping() { 


        }

        public void AddItem(string propertyName, string columnName)
        {
            Items.Add(propertyName, columnName);

        }

        public void Initialize()
        {
            Items.Clear();
        }

        public void SetDefaultPostMapping()
        {
            Items.Clear();
            this.AddItem("InsertedRows", "IINSERTED");
            this.AddItem("UpdatedRows", "IUPDATED");
            this.AddItem("LastId", "IID");

        }

    }
}
