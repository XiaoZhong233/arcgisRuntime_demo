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
using System.Windows.Media.Imaging;

namespace WpfApp1
{

    //
    //图层控制树 节点模板
    //
    class PropertyNodeItem : NotifyPropertyBase
    {
        public string Icon { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public BitmapImage Legend { get; set; }
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
                            traveseNode(child, x => {
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
        public static  void traveseNode(PropertyNodeItem node,Action<PropertyNodeItem> action)
        {
            foreach(PropertyNodeItem child in node.Children)
            {
                traveseNode(child,action);
            }
            action(node);
        }

        /// <summary>
        /// 寻找某个节点
        /// </summary>
        /// <param name="root"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static PropertyNodeItem findNode(PropertyNodeItem root,Predicate<PropertyNodeItem> predicate)
        {
            foreach(PropertyNodeItem child in root.Children)
            {
                if (predicate(child))
                    return child;
                else
                    findNode(child, predicate);
            }
            return null;
        }

        /// <summary>
        /// 寻找某个节点
        /// </summary>
        /// <param name="roots"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static PropertyNodeItem findNode(List<PropertyNodeItem> roots, Predicate<PropertyNodeItem> predicate)
        {
            PropertyNodeItem result = null;
            foreach (PropertyNodeItem root in roots)
            {
                result = findNode(root, predicate);
                if (result != null)
                    break;
            }
            return result;
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

        //带图例的子节点构造函数
        public PropertyNodeItem(String name, String displayName, Layer layer, PropertyNodeItem parent, BitmapImage legend)
        {
            Children = new List<PropertyNodeItem>();
            this.nodeType = NodeType.LeafNode;
            this.Name = name;
            this.DisplayName = displayName;
            this.layer = layer;
            this.parent = parent;
            this.level = this.parent.level + 1;
            isChecked = true;
            this.Legend = legend;
        }

        public enum NodeType
        {
            RootNode,//根节点
            LeafNode,//叶子节点
            StructureNode//结构节点，仅起到组织配置文件结构的作用，不参与修改
        }

   
    }
}
