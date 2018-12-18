using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp1
{

    //
    //图层控制树 节点模板
    //
    public class PropertyNodeItem : NotifyPropertyBase
    {
        public string Icon { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public PropertyNodeItem parent { get; set; }
        public NodeType nodeType;
        public Layer layer;
        public List<Layer> layers = new List<Layer>();
        private bool isChecked;
        private int level;

        

        public int Level
        {
            get { return level; } 
        }


        //子节点
        public List<PropertyNodeItem> Children { get; }



        public bool IsChecked {
            get
            {
                return isChecked;
            }
            set
            {
                if (value != isChecked)
                {
                    
                    OnPropertyChanged("isChecked");
                    this.isChecked = value;
                    //如果父项选中 子项也得选中
                    //如果父项取消选中，子项也得取消
                    if (nodeType == NodeType.RootNode)
                    {
                        
                        foreach(PropertyNodeItem child in this.Children)
                        {
                            traverseNode(child, x => {
                                x.IsChecked = this.IsChecked;
                                //通知更改
                                x.OnPropertyChanged("isChecked");
                                });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 遍历某节点下的所有节点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="action"></param>
        public static void traverseNode(PropertyNodeItem node,Action<PropertyNodeItem> action)
        {
            foreach(PropertyNodeItem child in node.Children)
            {
                traverseNode(child,action);
            }
            action(node);
        }

        public static void traverseNode(List<PropertyNodeItem> items, Action<PropertyNodeItem> action)
        {
            foreach(PropertyNodeItem child in items)
            {
                traverseNode(child, action);
            }
        }
            

        //根节点构造函数
        public PropertyNodeItem(String name,String displayName, List<Layer>  layers)
        {
            Children = new List<PropertyNodeItem>();
            this.nodeType = NodeType.RootNode;
            this.Name = name;
            this.DisplayName = displayName;
            this.layers = layers;
            this.level = 0;
            isChecked = true;
            parent = null;
        }

        //子节点构造函数
        public PropertyNodeItem(String name, String displayName,Layer layer, PropertyNodeItem parent)
        {
            Children = new List<PropertyNodeItem>();
            this.nodeType = NodeType.LeafNode;
            this.Name = name;
            this.DisplayName = displayName;
            this.layer = layer;
            this.parent = parent;
            this.level = this.parent.level + 1;
            isChecked = true;
        }

        public enum NodeType
        {
            RootNode,//根节点
            LeafNode,//叶子节点
            StructureNode//结构节点，仅起到组织配置文件结构的作用，不参与修改
        }

   
    }
}
