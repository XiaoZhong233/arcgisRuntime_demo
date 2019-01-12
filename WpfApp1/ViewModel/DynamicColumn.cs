using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModel
{
    public class DynamicColumn
    {
        public string Item_Name { get; set; }           //列名称
        public List<string> Item_Value { get; set; }    //列值集合
    }
}
